using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class UserConnectedPacket : Packet
{
    public bool connected;

    [NonSerialized] public User user;

    public int userId;

    public override void Apply(User cUser, RealmData realmData)
    {
        user.connected = connected;
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        userId = user.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmContext = (RealmContext)c.Context;
        user = ID.Find(realmContext.realmData.users, userId);
    }
}