using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp.Server;

namespace SocketLibrary
{
    // Token: 0x02000003 RID: 3
    public class Server
    {
        // Token: 0x02000005 RID: 5
        // (Invoke) Token: 0x06000025 RID: 37
        public delegate void MessageHandler(ServerClient client, byte[] data);

        // Token: 0x04000006 RID: 6
        private readonly List<ServerClient> clients = new List<ServerClient>();

        // Token: 0x04000005 RID: 5
        private readonly WebSocketServer server;

        // Token: 0x06000016 RID: 22 RVA: 0x00002405 File Offset: 0x00000605
        public Server(IPAddress address, int port)
        {
            server = new WebSocketServer(address, port);
        }

        // Token: 0x14000004 RID: 4
        // (add) Token: 0x06000010 RID: 16 RVA: 0x000022B8 File Offset: 0x000004B8
        // (remove) Token: 0x06000011 RID: 17 RVA: 0x000022F0 File Offset: 0x000004F0
        public event Action<ServerClient> Connection;

        // Token: 0x14000005 RID: 5
        // (add) Token: 0x06000012 RID: 18 RVA: 0x00002328 File Offset: 0x00000528
        // (remove) Token: 0x06000013 RID: 19 RVA: 0x00002360 File Offset: 0x00000560
        public event MessageHandler Message;

        // Token: 0x14000006 RID: 6
        // (add) Token: 0x06000014 RID: 20 RVA: 0x00002398 File Offset: 0x00000598
        // (remove) Token: 0x06000015 RID: 21 RVA: 0x000023D0 File Offset: 0x000005D0
        public event Action<ServerClient> Disconnection;

        // Token: 0x06000017 RID: 23 RVA: 0x00002428 File Offset: 0x00000628
        public void SendAll(string data)
        {
            foreach (var serverClient in clients)
            {
                serverClient.Send(data);
            }
        }

        // Token: 0x06000018 RID: 24 RVA: 0x0000247C File Offset: 0x0000067C
        public void Start()
        {
            server.Start();
            server.AddWebSocketService("/", () => new ServerClient(this));
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000024A5 File Offset: 0x000006A5
        internal void ConnectionCallback(ServerClient client)
        {
            clients.Add(client);
            Connection?.Invoke(client);
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000024BF File Offset: 0x000006BF
        internal void MessageCallback(ServerClient client, byte[] data)
        {
            Message?.Invoke(client, data);
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000024CE File Offset: 0x000006CE
        internal void CloseCallback(ServerClient client)
        {
            clients.Remove(client);
            Disconnection?.Invoke(client);
        }
    }
}