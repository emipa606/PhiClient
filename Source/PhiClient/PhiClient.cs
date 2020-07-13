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
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (remove) Token: 0x06000002 RID: 2 RVA: 0x00002088 File Offset: 0x00000288
		public event Action OnUsable;

		// Token: 0x06000003 RID: 3 RVA: 0x000020BD File Offset: 0x000002BD
		public PhiClient()
		{
			PhiClient.instance = this;
			this.serverAddress = this.GetServerAddress();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E8 File Offset: 0x000002E8
		public void TryConnect()
		{
			if (this.client != null)
			{
				this.Disconnect();
			}
			this.client = new Client(this.serverAddress, 16180);
			this.client.Connection += this.ConnectionCallback;
			this.client.Message += this.MessageCallback;
			this.client.Disconnection += this.DisconnectCallback;
			this.Log(LogLevel.INFO, "Try connecting to " + this.serverAddress);
			this.client.Connect();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002180 File Offset: 0x00000380
		public void Disconnect()
		{
			this.client.Disconnect();
			this.client = null;
			this.realmData = null;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000219C File Offset: 0x0000039C
		public void SendPacket(Packet packet)
		{
			try
			{
				byte[] data = Packet.Serialize(packet, this.realmData, this.currentUser);
				this.client.Send(data);
			}
			catch (Exception ex)
			{
				this.Log(LogLevel.ERROR, ex.ToString());
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021EC File Offset: 0x000003EC
		internal void OnUpdate()
		{
			while (this.packetsToProcess.Count > 0)
			{
				try
				{
					Packet packet = Packet.Deserialize((byte[])this.packetsToProcess.Dequeue(), this.realmData, this.currentUser);
					this.Log(LogLevel.DEBUG, "Received packet from server: " + packet);
					this.ProcessPacket(packet);
				}
				catch (Exception ex)
				{
					this.Log(LogLevel.ERROR, ex.ToString());
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002268 File Offset: 0x00000468
		private void ProcessPacket(Packet packet)
		{
			if (packet is SynchronisationPacket)
			{
				SynchronisationPacket synchronisationPacket = (SynchronisationPacket)packet;
				this.realmData = synchronisationPacket.realmData;
				this.currentUser = synchronisationPacket.user;
				this.realmData.PacketToServer += this.PacketToServerCallback;
				this.realmData.Log += this.Log;
				this.SaveCredentials();
				if (this.OnUsable != null)
				{
					this.OnUsable();
					return;
				}
			}
			else
			{
				packet.Apply(this.currentUser, this.realmData);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000022F8 File Offset: 0x000004F8
		private void SaveCredentials()
		{
			string text = (!File.Exists("phikey.txt")) ? this.GetAuthKey() : File.ReadAllLines("phikey.txt")[0];
			File.WriteAllLines("phikey.txt", new string[]
			{
				text,
				this.currentUser.id.ToString()
			});
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000234D File Offset: 0x0000054D
		private void Log(LogLevel level, string message)
		{
			if (level == LogLevel.ERROR)
			{
				Verse.Log.Error(message, false);
				return;
			}
			if (level != LogLevel.INFO)
			{
				return;
			}
			Verse.Log.Message(message, false);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002367 File Offset: 0x00000567
		private void PacketToServerCallback(Packet packet)
		{
			this.SendPacket(packet);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002370 File Offset: 0x00000570
		public bool IsConnected()
		{
			return this.client != null && this.client.state == WebSocketState.Open;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000238A File Offset: 0x0000058A
		public bool IsUsable()
		{
			return this.IsConnected() && this.realmData != null;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000023A0 File Offset: 0x000005A0
		public string RandomString(int length)
		{
			Random random = new Random();
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[random.Next(s.Length)]).ToArray<char>());
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023E4 File Offset: 0x000005E4
		private void ConnectionCallback()
		{
			this.Log(LogLevel.INFO, "Connected to the server");
			string text = (SteamUtility.SteamPersonaName != "???") ? SteamUtility.SteamPersonaName : this.RandomString(10);
			string hashedAuthKey = this.GetHashedAuthKey();
			int? id = this.GetId();
			this.SendPacket(new AuthentificationPacket
			{
				name = text,
				id = id,
				hashedKey = hashedAuthKey,
				version = "0.14.1"
			});
			this.Log(LogLevel.INFO, "Trying to authenticate as " + text);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002469 File Offset: 0x00000669
		private void MessageCallback(byte[] data)
		{
			this.packetsToProcess.Enqueue(data);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002478 File Offset: 0x00000678
		private string GetHashedAuthKey()
		{
			string authKey = this.GetAuthKey();
			byte[] array = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(authKey));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
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
			string text = this.GenerateKey(32);
			File.WriteAllLines("phikey.txt", new string[]
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
				return new int?(int.Parse(File.ReadAllLines("phikey.txt")[1]));
			}
			return null;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000256C File Offset: 0x0000076C
		private string GenerateKey(int length)
		{
			Random random = new Random();
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[random.Next(s.Length)]).ToArray<char>());
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000025B0 File Offset: 0x000007B0
		private void DisconnectCallback()
		{
			Verse.Log.Message("Disconnected from the server", false);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000025C0 File Offset: 0x000007C0
		private string GetServerAddress()
		{
			if (!File.Exists("phiserver.txt"))
			{
				using (StreamWriter streamWriter = File.AppendText("phiserver.txt"))
				{
					streamWriter.WriteLine("longwelwind.net");
					return "longwelwind.net";
				}
			}
			return File.ReadAllLines("phiserver.txt")[0].Trim();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002624 File Offset: 0x00000824
		public void SetServerAddress(string address)
		{
			address = string.Join("", address.Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
			File.WriteAllLines("phiserver.txt", new string[1]
			{
				address
			});
			this.serverAddress = address;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002655 File Offset: 0x00000855
		public void UpdatePreferences()
		{
			this.SendPacket(new UpdatePreferencesPacket
			{
				preferences = this.currentUser.preferences
			});
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002673 File Offset: 0x00000873
		public void SendMessage(string message)
		{
			if (this.IsUsable())
			{
				this.SendPacket(new PostMessagePacket
				{
					message = message
				});
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002690 File Offset: 0x00000890
		public bool SendThings(User user, Dictionary<List<Thing>, int> chosenThings)
		{
			if (!this.CheckCanStartTransaction(user))
			{
				return false;
			}
			List<KeyValuePair<RealmThing, int>> list = new List<KeyValuePair<RealmThing, int>>();
			foreach (KeyValuePair<List<Thing>, int> keyValuePair in chosenThings)
			{
				RealmThing key = this.realmData.ToRealmThing(keyValuePair.Key[0]);
				list.Add(new KeyValuePair<RealmThing, int>(key, keyValuePair.Value));
			}
			User user2 = this.currentUser;
			int num = user2.lastTransactionId + 1;
			user2.lastTransactionId = num;
			ItemTransaction itemTransaction = new ItemTransaction(num, this.currentUser, user, chosenThings, list);
			this.realmData.transactions.Add(itemTransaction);
			this.SendPacket(new StartTransactionPacket
			{
				transaction = itemTransaction
			});
			Messages.Message("Offer sent, waiting for confirmation", MessageTypeDefOf.SilentInput, true);
			return true;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002774 File Offset: 0x00000974
		public bool CheckCanStartTransaction(User receiver)
		{
			this.realmData.CanStartTransaction(this.currentUser, receiver);
			return true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000278C File Offset: 0x0000098C
		public void SendPawn(User user, Pawn pawn, TransactionType transaction)
		{
			if (this.CheckCanStartTransaction(user))
			{
				RealmPawn realmPawn = RealmPawn.ToRealmPawn(pawn, this.realmData);
				User user2 = this.currentUser;
				int num = user2.lastTransactionId + 1;
				user2.lastTransactionId = num;
				PawnTransaction pawnTransaction = new PawnTransaction(num, this.currentUser, user, pawn, realmPawn, transaction);
				this.realmData.transactions.Add(pawnTransaction);
				this.SendPacket(new StartTransactionPacket
				{
					transaction = pawnTransaction
				});
				Messages.Message("Offer sent, waiting for confirmation", MessageTypeDefOf.SilentInput, true);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002808 File Offset: 0x00000A08
		public void ChangeNickname(string newNickname)
		{
			this.SendPacket(new ChangeNicknamePacket
			{
				name = newNickname
			});
		}

		// Token: 0x04000001 RID: 1
		public static PhiClient instance;

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

		// Token: 0x04000007 RID: 7
		public RealmData realmData;

		// Token: 0x04000008 RID: 8
		public User currentUser;

		// Token: 0x04000009 RID: 9
		public Client client;

		// Token: 0x0400000A RID: 10
		private Queue packetsToProcess = Queue.Synchronized(new Queue());

		// Token: 0x0400000B RID: 11
		public string serverAddress;
	}
}
