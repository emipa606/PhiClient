using System;
using WebSocketSharp;

namespace SocketLibrary;

public class Client
{
    private readonly WebSocket client;

    public Client(string address, int port)
    {
        client = new WebSocket($"ws://{address}:{port}/");
        client.OnMessage += MessageCallback;
        client.OnOpen += OpenCallback;
        client.OnClose += CloseCallback;
    }

    public WebSocketState state => client.ReadyState;

    public event Action<byte[]> Message;

    public event Action Connection;

    public event Action Disconnection;

    public void Connect()
    {
        client.ConnectAsync();
    }

    public void Disconnect()
    {
        client.CloseAsync();
    }

    public void Send(string data)
    {
        client.SendAsync(data, null);
    }

    public void Send(byte[] data)
    {
        client.SendAsync(data, null);
    }

    private void OpenCallback(object sender, EventArgs e)
    {
        Connection?.Invoke();
    }

    private void CloseCallback(object sender, CloseEventArgs e)
    {
        Disconnection?.Invoke();
    }

    private void MessageCallback(object sender, MessageEventArgs e)
    {
        var rawData = e.RawData;
        Message?.Invoke(rawData);
    }
}