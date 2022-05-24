using System;

namespace PhiClient;

[Serializable]
public class PostMessagePacket : Packet
{
    public string message;

    public override void Apply(User user, RealmData realmData)
    {
        realmData.ServerPostMessage(user, message);
    }
}