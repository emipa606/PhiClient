using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp.Server;

namespace SocketLibrary;

public class Server
{
    public delegate void MessageHandler(ServerClient client, byte[] data);

    private readonly List<ServerClient> clients = new List<ServerClient>();

    private readonly WebSocketServer server;

    public Server(IPAddress address, int port)
    {
        server = new WebSocketServer(address, port);
    }

    public event Action<ServerClient> Connection;

    public event MessageHandler Message;

    public event Action<ServerClient> Disconnection;

    public void SendAll(string data)
    {
        foreach (var serverClient in clients)
        {
            serverClient.Send(data);
        }
    }

    public void Start()
    {
        server.Start();
        server.AddWebSocketService("/", () => new ServerClient(this));
    }

    internal void ConnectionCallback(ServerClient client)
    {
        clients.Add(client);
        Connection?.Invoke(client);
    }

    internal void MessageCallback(ServerClient client, byte[] data)
    {
        Message?.Invoke(client, data);
    }

    internal void CloseCallback(ServerClient client)
    {
        clients.Remove(client);
        Disconnection?.Invoke(client);
    }
}