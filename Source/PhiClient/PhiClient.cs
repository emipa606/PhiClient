using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PhiClient.TransactionSystem;
using RimWorld;
using SocketLibrary;
using Verse;
using WebSocketSharp;

namespace PhiClient;

public class PhiClient
{
    private const string KEY_FILE = "phikey.txt";

    private const string SERVER_FILE = "phiserver.txt";

    private const string DEFAULT_SERVER_ADDRESS = "longwelwind.net";

    private const int KEY_LENGTH = 32;

    private const int PORT = 16180;

    public static PhiClient instance;

    private readonly Queue packetsToProcess = Queue.Synchronized(new Queue());

    public Client client;

    public User currentUser;

    public RealmData realmData;

    public string serverAddress;

    public PhiClient()
    {
        instance = this;
        serverAddress = GetServerAddress();
    }

    public event Action OnUsable;

    public void TryConnect()
    {
        if (client != null)
        {
            Disconnect();
        }

        // Edited by [NOT-FOUND-404-UI]
        // ---------------------------------------------------------------
        int serverPort;
        if (serverAddress.Contains(":"))
        {
            serverPort = int.Parse(serverAddress.Split(':')[1]);
            serverAddress = serverAddress.Split(':')[0];
        }
        else
        {
            serverPort = PORT;
        }

        if (serverPort <= 0)
        {
            serverPort = 1;
        }

        if (serverPort > 65535)
        {
            serverPort = 65535;
        }

        client = new Client(serverAddress, serverPort);
        // ---------------------------------------------------------------
        //client = new Client(serverAddress,16180);
        client.Connection += ConnectionCallback;
        client.Message += MessageCallback;
        client.Disconnection += DisconnectCallback;
        Log(LogLevel.INFO, $"Try connecting to {serverAddress} on port {serverPort}");
        client.Connect();
    }

    public void Disconnect()
    {
        client.Disconnect();
        client = null;
        realmData = null;
    }

    public void SendPacket(Packet packet)
    {
        try
        {
            var data = Packet.Serialize(packet, realmData, currentUser);
            client.Send(data);
        }
        catch (Exception ex)
        {
            Log(LogLevel.ERROR, ex.ToString());
        }
    }

    internal void OnUpdate()
    {
        while (packetsToProcess.Count > 0)
        {
            try
            {
                var packet = Packet.Deserialize((byte[])packetsToProcess.Dequeue(), realmData, currentUser);
                Log(LogLevel.DEBUG, $"Received packet from server: {packet}");
                ProcessPacket(packet);
            }
            catch (Exception ex)
            {
                Log(LogLevel.ERROR, ex.ToString());
            }
        }
    }

    private void ProcessPacket(Packet packet)
    {
        if (packet is SynchronisationPacket synchronisationPacket)
        {
            realmData = synchronisationPacket.realmData;
            currentUser = synchronisationPacket.user;
            realmData.PacketToServer += PacketToServerCallback;
            realmData.Log += Log;
            SaveCredentials();
            OnUsable?.Invoke();
        }
        else
        {
            packet.Apply(currentUser, realmData);
        }
    }

    private void SaveCredentials()
    {
        var text = !File.Exists("phikey.txt") ? GetAuthKey() : File.ReadAllLines("phikey.txt")[0];
        File.WriteAllLines("phikey.txt", new[]
        {
            text,
            currentUser.id.ToString()
        });
    }

    private void Log(LogLevel level, string message)
    {
        if (level == LogLevel.ERROR)
        {
            Verse.Log.Error(message);
            return;
        }

        if (level != LogLevel.INFO)
        {
            return;
        }

        Verse.Log.Message(message);
    }

    private void PacketToServerCallback(Packet packet)
    {
        SendPacket(packet);
    }

    public bool IsConnected()
    {
        return client is { state: WebSocketState.Open };
    }

    public bool IsUsable()
    {
        return IsConnected() && realmData != null;
    }

    public string RandomString(int length)
    {
        var random = new Random();
        return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
            select s[random.Next(s.Length)]).ToArray());
    }

    private void ConnectionCallback()
    {
        Log(LogLevel.INFO, "Connected to the server");
        var text = SteamUtility.SteamPersonaName != "???" ? SteamUtility.SteamPersonaName : RandomString(10);
        var hashedAuthKey = GetHashedAuthKey();
        var id = GetId();
        SendPacket(new AuthentificationPacket
        {
            name = text,
            id = id,
            hashedKey = hashedAuthKey,
            version = "0.14.1"
        });
        Log(LogLevel.INFO, $"Trying to authenticate as {text}");
    }

    private void MessageCallback(byte[] data)
    {
        packetsToProcess.Enqueue(data);
    }

    private string GetHashedAuthKey()
    {
        var authKey = GetAuthKey();
        var array = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(authKey));
        var stringBuilder = new StringBuilder();
        foreach (var b in array)
        {
            stringBuilder.Append(b.ToString("X2"));
        }

        return stringBuilder.ToString();
    }

    private string GetAuthKey()
    {
        if (File.Exists("phikey.txt"))
        {
            return File.ReadAllLines("phikey.txt")[0];
        }

        var text = GenerateKey(32);
        File.WriteAllLines("phikey.txt", new[]
        {
            text
        });
        return text;
    }

    private int? GetId()
    {
        if (!File.Exists("phikey.txt"))
        {
            return null;
        }

        if (File.ReadAllLines("phikey.txt").Length > 1)
        {
            return int.Parse(File.ReadAllLines("phikey.txt")[1]);
        }

        return null;
    }

    private string GenerateKey(int length)
    {
        var random = new Random();
        return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
            select s[random.Next(s.Length)]).ToArray());
    }

    private void DisconnectCallback()
    {
        Verse.Log.Message("Disconnected from the server");
    }

    private string GetServerAddress()
    {
        if (File.Exists("phiserver.txt"))
        {
            return File.ReadAllLines("phiserver.txt")[0].Trim();
        }

        using var streamWriter = File.AppendText("phiserver.txt");
        streamWriter.WriteLine("longwelwind.net");
        return "longwelwind.net";
    }

    public void SetServerAddress(string address)
    {
        address = string.Join("", address.Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
        File.WriteAllLines("phiserver.txt", new[]
        {
            address
        });
        serverAddress = address;
    }

    public void UpdatePreferences()
    {
        SendPacket(new UpdatePreferencesPacket
        {
            preferences = currentUser.preferences
        });
    }

    public void SendMessage(string message)
    {
        if (IsUsable())
        {
            SendPacket(new PostMessagePacket
            {
                message = message
            });
        }
    }

    public bool SendThings(User user, Dictionary<List<Thing>, int> chosenThings)
    {
        if (!CheckCanStartTransaction(user))
        {
            return false;
        }

        var list = new List<KeyValuePair<RealmThing, int>>();
        foreach (var keyValuePair in chosenThings)
        {
            var key = realmData.ToRealmThing(keyValuePair.Key[0]);
            list.Add(new KeyValuePair<RealmThing, int>(key, keyValuePair.Value));
        }

        var user2 = currentUser;
        var num = user2.lastTransactionId + 1;
        user2.lastTransactionId = num;
        var itemTransaction = new ItemTransaction(num, currentUser, user, chosenThings, list);
        realmData.transactions.Add(itemTransaction);
        SendPacket(new StartTransactionPacket
        {
            transaction = itemTransaction
        });
        Messages.Message("Offer sent, waiting for confirmation", MessageTypeDefOf.SilentInput);
        return true;
    }

    public bool CheckCanStartTransaction(User receiver)
    {
        realmData.CanStartTransaction(currentUser, receiver);
        return true;
    }

    public void SendPawn(User user, Pawn pawn, TransactionType transaction)
    {
        if (!CheckCanStartTransaction(user))
        {
            return;
        }

        var realmPawn = RealmPawn.ToRealmPawn(pawn, realmData);
        var user2 = currentUser;
        var num = user2.lastTransactionId + 1;
        user2.lastTransactionId = num;
        var pawnTransaction = new PawnTransaction(num, currentUser, user, pawn, realmPawn, transaction);
        realmData.transactions.Add(pawnTransaction);
        SendPacket(new StartTransactionPacket
        {
            transaction = pawnTransaction
        });
        Messages.Message("Offer sent, waiting for confirmation", MessageTypeDefOf.SilentInput);
    }

    public void ChangeNickname(string newNickname)
    {
        SendPacket(new ChangeNicknamePacket
        {
            name = newNickname
        });
    }
}