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

namespace PhiClient
{
    // Token: 0x02000002 RID: 2
    public class PhiClient
    {
        // Token: 0x04000002 RID: 2
        private const string KEY_FILE = "phikey.txt";

        // Token: 0x04000003 RID: 3
        private const string SERVER_FILE = "phiserver.txt";

        // Token: 0x04000004 RID: 4
        private const string DEFAULT_SERVER_ADDRESS = "longwelwind.net";

        // Token: 0x04000005 RID: 5
        private const int KEY_LENGTH = 32;

        // Token: 0x04000006 RID: 6
        private const int PORT = 16180;

        // Token: 0x04000001 RID: 1
        public static PhiClient instance;

        // Token: 0x0400000A RID: 10
        private readonly Queue packetsToProcess = Queue.Synchronized(new Queue());

        // Token: 0x04000009 RID: 9
        public Client client;

        // Token: 0x04000008 RID: 8
        public User currentUser;

        // Token: 0x04000007 RID: 7
        public RealmData realmData;

        // Token: 0x0400000B RID: 11
        public string serverAddress;

        // Token: 0x06000003 RID: 3 RVA: 0x000020BD File Offset: 0x000002BD
        public PhiClient()
        {
            instance = this;
            serverAddress = GetServerAddress();
        }

        // Token: 0x14000001 RID: 1
        // (add) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        // (remove) Token: 0x06000002 RID: 2 RVA: 0x00002088 File Offset: 0x00000288
        public event Action OnUsable;

        // Token: 0x06000004 RID: 4 RVA: 0x000020E8 File Offset: 0x000002E8
        public void TryConnect()
        {
            if (client != null)
            {
                Disconnect();
            }

            // Edited by [NOT-FOUND-404-UI]
            // ---------------------------------------------------------------
            int serverPort = new int();
            if (0 <= serverAddress.IndexOf(":")) {
                string[] temp = new string[2];
                temp = serverAddress.Split(':');
                serverAddress = temp[0];
                serverPort = int.Parse(temp[1]);
            } else {
                serverPort = PORT;
            }
            client = new Client(serverAddress, serverPort);
            // ---------------------------------------------------------------
            //client = new Client(serverAddress,16180);
            client.Connection += ConnectionCallback;
            client.Message += MessageCallback;
            client.Disconnection += DisconnectCallback;
            Log(LogLevel.INFO, "Try connecting to " + serverAddress);
            client.Connect();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002180 File Offset: 0x00000380
        public void Disconnect()
        {
            client.Disconnect();
            client = null;
            realmData = null;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x0000219C File Offset: 0x0000039C
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

        // Token: 0x06000007 RID: 7 RVA: 0x000021EC File Offset: 0x000003EC
        internal void OnUpdate()
        {
            while (packetsToProcess.Count > 0)
            {
                try
                {
                    var packet = Packet.Deserialize((byte[]) packetsToProcess.Dequeue(), realmData, currentUser);
                    Log(LogLevel.DEBUG, "Received packet from server: " + packet);
                    ProcessPacket(packet);
                }
                catch (Exception ex)
                {
                    Log(LogLevel.ERROR, ex.ToString());
                }
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002268 File Offset: 0x00000468
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

        // Token: 0x06000009 RID: 9 RVA: 0x000022F8 File Offset: 0x000004F8
        private void SaveCredentials()
        {
            var text = !File.Exists("phikey.txt") ? GetAuthKey() : File.ReadAllLines("phikey.txt")[0];
            File.WriteAllLines("phikey.txt", new[]
            {
                text,
                currentUser.id.ToString()
            });
        }

        // Token: 0x0600000A RID: 10 RVA: 0x0000234D File Offset: 0x0000054D
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

        // Token: 0x0600000B RID: 11 RVA: 0x00002367 File Offset: 0x00000567
        private void PacketToServerCallback(Packet packet)
        {
            SendPacket(packet);
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002370 File Offset: 0x00000570
        public bool IsConnected()
        {
            return client != null && client.state == WebSocketState.Open;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x0000238A File Offset: 0x0000058A
        public bool IsUsable()
        {
            return IsConnected() && realmData != null;
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000023A0 File Offset: 0x000005A0
        public string RandomString(int length)
        {
            var random = new Random();
            return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                select s[random.Next(s.Length)]).ToArray());
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000023E4 File Offset: 0x000005E4
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
            Log(LogLevel.INFO, "Trying to authenticate as " + text);
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002469 File Offset: 0x00000669
        private void MessageCallback(byte[] data)
        {
            packetsToProcess.Enqueue(data);
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002478 File Offset: 0x00000678
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

        // Token: 0x06000012 RID: 18 RVA: 0x000024D4 File Offset: 0x000006D4
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

        // Token: 0x06000013 RID: 19 RVA: 0x00002518 File Offset: 0x00000718
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

        // Token: 0x06000014 RID: 20 RVA: 0x0000256C File Offset: 0x0000076C
        private string GenerateKey(int length)
        {
            var random = new Random();
            return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                select s[random.Next(s.Length)]).ToArray());
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000025B0 File Offset: 0x000007B0
        private void DisconnectCallback()
        {
            Verse.Log.Message("Disconnected from the server");
        }

        // Token: 0x06000016 RID: 22 RVA: 0x000025C0 File Offset: 0x000007C0
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

        // Token: 0x06000017 RID: 23 RVA: 0x00002624 File Offset: 0x00000824
        public void SetServerAddress(string address)
        {
            address = string.Join("", address.Split((string[]) null, StringSplitOptions.RemoveEmptyEntries));
            File.WriteAllLines("phiserver.txt", new[]
            {
                address
            });
            serverAddress = address;
        }

        // Token: 0x06000018 RID: 24 RVA: 0x00002655 File Offset: 0x00000855
        public void UpdatePreferences()
        {
            SendPacket(new UpdatePreferencesPacket
            {
                preferences = currentUser.preferences
            });
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00002673 File Offset: 0x00000873
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

        // Token: 0x0600001A RID: 26 RVA: 0x00002690 File Offset: 0x00000890
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

        // Token: 0x0600001B RID: 27 RVA: 0x00002774 File Offset: 0x00000974
        public bool CheckCanStartTransaction(User receiver)
        {
            realmData.CanStartTransaction(currentUser, receiver);
            return true;
        }

        // Token: 0x0600001C RID: 28 RVA: 0x0000278C File Offset: 0x0000098C
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

        // Token: 0x0600001D RID: 29 RVA: 0x00002808 File Offset: 0x00000A08
        public void ChangeNickname(string newNickname)
        {
            SendPacket(new ChangeNicknamePacket
            {
                name = newNickname
            });
        }
    }
}
