using System;

namespace PhiClient;

[Serializable]
public class ChatMessagePacket : Packet
{
    public ChatMessage message;

    public override void Apply(User user, RealmData realmData)
    {
        realmData.AddChatMessage(message);
    }
}