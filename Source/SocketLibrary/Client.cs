using System;
using WebSocketSharp;

namespace SocketLibrary
{
	// Token: 0x02000002 RID: 2
	public class Client
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public WebSocketState state
		{
			get
			{
				return this.client.ReadyState;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		// (remove) Token: 0x06000003 RID: 3 RVA: 0x00002098 File Offset: 0x00000298
		public event Action<byte[]> Message;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000004 RID: 4 RVA: 0x000020D0 File Offset: 0x000002D0
		// (remove) Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
		public event Action Connection;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000006 RID: 6 RVA: 0x00002140 File Offset: 0x00000340
		// (remove) Token: 0x06000007 RID: 7 RVA: 0x00002178 File Offset: 0x00000378
		public event Action Disconnection;

		// Token: 0x06000008 RID: 8 RVA: 0x000021B0 File Offset: 0x000003B0
		public Client(string address, int port)
		{
			this.client = new WebSocket(string.Concat(new object[]
			{
				"ws://",
				address,
				":",
				port,
				"/"
			}), new string[0]);
			this.client.OnMessage += this.MessageCallback;
			this.client.OnOpen += this.OpenCallback;
			this.client.OnClose += this.CloseCallback;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002249 File Offset: 0x00000449
		public void Connect()
		{
			this.client.ConnectAsync();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002256 File Offset: 0x00000456
		public void Disconnect()
		{
			this.client.CloseAsync();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002263 File Offset: 0x00000463
		public void Send(string data)
		{
			this.client.SendAsync(data, null);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002272 File Offset: 0x00000472
		public void Send(byte[] data)
		{
			this.client.SendAsync(data, null);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002281 File Offset: 0x00000481
		private void OpenCallback(object sender, EventArgs e)
		{
			this.Connection();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000228E File Offset: 0x0000048E
		private void CloseCallback(object sender, CloseEventArgs e)
		{
			this.Disconnection();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000229B File Offset: 0x0000049B
		private void MessageCallback(object sender, MessageEventArgs e)
		{
			byte[] rawData = e.RawData;
			this.Message(e.RawData);
		}

		// Token: 0x04000001 RID: 1
		private WebSocket client;
	}
}
