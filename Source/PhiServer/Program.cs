using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PhiClient;
using PhiClient.TransactionSystem;
using SocketLibrary;

namespace PhiServer
{
    public class Program
    {
        private readonly Dictionary<ServerClient, User> connectedUsers = new Dictionary<ServerClient, User>();

        private readonly object lockProcessPacket = new object();
        private readonly Dictionary<int, string> userKeys = new Dictionary<int, string>();
        private LogLevel level;
        private RealmData realmData;
        private Server server;

        public void Start(IPAddress ipAddress, int port, LogLevel logLevel)
        {
            level = logLevel;

            server = new Server(ipAddress, port);
            server.Start();
            Log(LogLevel.INFO, $"Server launched on port {port}, loglevel {level}");

            server.Connection += ConnectionCallback;
            server.Message += MessageCallback;
            server.Disconnection += DisconnectionCallback;

            realmData = new RealmData();
            realmData.PacketToClient += RealmPacketCallback;
            realmData.Log += Log;
        }

        private void ConnectionCallback(ServerClient client)
        {
            Log(LogLevel.INFO, $"Connection from {client.ID}");
        }

        private void Log(LogLevel logLevel, string message)
        {
            if (logLevel < level)
            {
                return;
            }

            var tag = "";
            switch (logLevel)
            {
                case LogLevel.DEBUG:
                case LogLevel.ERROR:
                    tag = "DEBUG";
                    break;
                case LogLevel.INFO:
                    tag = "INFO";
                    break;
            }

            Console.WriteLine("[{0}] [{1}] {2}", DateTime.Now, tag, message);
        }

        private void RealmPacketCallback(User user, Packet packet)
        {
            // We have to transmit a packet to a user, we find the connection
            // attached to this user and transmit the packet
            foreach (var client in connectedUsers.Keys)
            {
                connectedUsers.TryGetValue(client, out var u);
                if (u != user)
                {
                    continue;
                }

                SendPacket(client, user, packet);
                return; // No need to continue iterating once we've found the right user
            }
        }

        private void SendPacket(ServerClient client, User user, Packet packet)
        {
            Log(LogLevel.DEBUG, $"Server -> {(user != null ? user.name : "No")}: {packet}");
            client.Send(Packet.Serialize(packet, realmData, user));
        }

        private void DisconnectionCallback(ServerClient client)
        {
            connectedUsers.TryGetValue(client, out var user);
            if (user == null)
            {
                return;
            }

            Log(LogLevel.INFO, $"{user.name} disconnected");
            connectedUsers.Remove(client);
            user.connected = false;
            realmData.BroadcastPacket(new UserConnectedPacket { user = user, connected = false });
        }

        private void MessageCallback(ServerClient client, byte[] data)
        {
            lock (lockProcessPacket)
            {
                connectedUsers.TryGetValue(client, out var user);

                var packet = Packet.Deserialize(data, realmData, user);
                Log(LogLevel.DEBUG, $"{(user != null ? user.name : client.ID)} -> Server: {packet}");

                if (packet is AuthentificationPacket authPacket)
                {
                    // Special packets, (first sent from the client)

                    // We first check if the version corresponds
                    if (authPacket.version != RealmData.VERSION)
                    {
                        SendPacket(client, user, new AuthentificationErrorPacket
                            {
                                error =
                                    $"Server is version {RealmData.VERSION} but client is version {authPacket.version}"
                            }
                        );
                        return;
                    }

                    // Check if the user wants to use a specific id
                    var userId = authPacket.id != null
                        ? RegisterUserKey(authPacket.id.Value, authPacket.hashedKey)
                        : RegisterUserKey(++realmData.lastUserGivenId, authPacket.hashedKey);

                    user = realmData.users.FindLast(u => userId == u.id);
                    if (user == null)
                    {
                        user = realmData.ServerAddUser(authPacket.name, userId);
                        user.connected = true;

                        // We send a notify to all users connected about the new user
                        realmData.BroadcastPacketExcept(new NewUserPacket { user = user }, user);
                    }
                    else
                    {
                        user.connected = true;

                        // We send a connect notification to all users
                        realmData.BroadcastPacketExcept(new UserConnectedPacket { user = user, connected = true },
                            user);
                    }

                    connectedUsers.Add(client, user);
                    Log(LogLevel.INFO,
                        $"Client {client.ID} connected as {user.name} ({user.id})");

                    // We respond with a StatePacket that contains all synchronisation data
                    SendPacket(client, user, new SynchronisationPacket { user = user, realmData = realmData });
                }
                else if (packet is StartTransactionPacket transactionPacket)
                {
                    if (user == null)
                    {
                        // We ignore this packet
                        Log(LogLevel.ERROR, $"{packet} ignored because unknown user {client.ID}");
                        return;
                    }

                    // Check whether the packet was sent too quickly
                    var timeSinceLastTransaction = DateTime.Now - user.lastTransactionTime;
                    if (timeSinceLastTransaction > TimeSpan.FromSeconds(3))
                    {
                        // Apply the packet as normal
                        transactionPacket.Apply(user, realmData);
                    }
                    else
                    {
                        // Intercept the packet, returning it to sender
                        transactionPacket.transaction.state = TransactionResponse.TOOFAST;
                        SendPacket(client, user,
                            new ConfirmTransactionPacket
                            {
                                response = transactionPacket.transaction.state, toSender = true,
                                transaction = transactionPacket.transaction
                            });

                        // Report the packet to the log
                        Log(LogLevel.ERROR,
                            $"{packet} ignored because user {client.ID} sent a packet less than 3 seconds ago");
                    }
                }
                else
                {
                    if (user == null)
                    {
                        // We ignore this package
                        Log(LogLevel.ERROR, $"{packet} ignored because unknown user {client.ID}");
                        return;
                    }

                    // Normal packets, we defer the execution
                    packet.Apply(user, realmData);
                }
            }
        }

        /// <summary>
        ///     Checks if the key matches an existing id. If it does not match, returns new id which the key is linked to. Returns
        ///     the input id otherwise.
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <param name="hashedKey">The user's hashed key. This should only be kept on the server.</param>
        private int RegisterUserKey(int id, string hashedKey)
        {
            // Check if this user exists
            if (userKeys.ContainsKey(id) && id <= realmData.lastUserGivenId)
            {
                // Check if the two keys are different
                if (hashedKey == userKeys[id])
                {
                    return id;
                }

                // Register a new id and key pair
                id = ++realmData.lastUserGivenId;
                userKeys.Add(id, hashedKey);
            }
            else
            {
                // Register a new id and key pair
                id = ++realmData.lastUserGivenId;
                userKeys.Add(id, hashedKey);
            }

            return id;
        }

        private static void Main(string[] args)
        {
            var program = new Program();

            var logLevel = LogLevel.ERROR;
            var port = 16180;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.Equals("debug"))
                    {
                        logLevel = LogLevel.DEBUG;
                        continue;
                    }

                    if (!arg.All(char.IsNumber))
                    {
                        Console.Write(
                            $"Unknown command {arg}. Valid commands are\ndebug - will log all events\nportnumber - a valid portnumber for the server to start on\n");
                        continue;
                    }

                    if (int.TryParse(arg, out var customPort) && customPort > 0 && customPort <= 65535)
                    {
                        port = customPort;
                    }
                }
            }

            program.Start(IPAddress.Any, port, logLevel);

            Console.Read();
        }
    }
}