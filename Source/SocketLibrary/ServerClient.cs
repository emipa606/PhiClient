using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketLibrary;

public class ServerClient : WebSocketBehavior
{
    private readonly Server server;

    public ServerClient(Server server)
    {
        this.server = server;
    }

    public new void Send(string data)
    {
        SendAsync(data, null);
    }

    public new void Send(byte[] data)
    {
        SendAsync(data, null);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        base.OnMessage(e);
        server.MessageCallback(this, e.RawData);
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        server.ConnectionCallback(this);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        base.OnClose(e);
        server.CloseCallback(this);
    }

    public override string ToString()
    {
        return Context.Origin;
    }
}