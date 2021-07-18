using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketLibrary
{
    // Token: 0x02000004 RID: 4
    public class ServerClient : WebSocketBehavior
    {
        // Token: 0x0400000A RID: 10
        private readonly Server server;

        // Token: 0x0600001D RID: 29 RVA: 0x000024F1 File Offset: 0x000006F1
        public ServerClient(Server server)
        {
            this.server = server;
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002500 File Offset: 0x00000700
        public new void Send(string data)
        {
            SendAsync(data, null);
        }

        // Token: 0x0600001F RID: 31 RVA: 0x0000250A File Offset: 0x0000070A
        public new void Send(byte[] data)
        {
            SendAsync(data, null);
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00002514 File Offset: 0x00000714
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            server.MessageCallback(this, e.RawData);
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000252F File Offset: 0x0000072F
        protected override void OnOpen()
        {
            base.OnOpen();
            server.ConnectionCallback(this);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002543 File Offset: 0x00000743
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            server.CloseCallback(this);
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00002558 File Offset: 0x00000758
        public override string ToString()
        {
            return Context.Origin;
        }
    }
}