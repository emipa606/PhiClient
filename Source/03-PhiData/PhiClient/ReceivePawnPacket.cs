using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class ReceivePawnPacket : Packet
{
    public RealmPawn realmPawn;

    [NonSerialized] public User userFrom;

    private int userFromId;

    public override void Apply(User user, RealmData realmData)
    {
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        userFromId = userFrom.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmContext = (RealmContext)c.Context;
        userFrom = ID.Find(realmContext.realmData.users, userFromId);
    }
}