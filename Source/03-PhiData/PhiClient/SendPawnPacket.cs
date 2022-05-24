using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class SendPawnPacket : Packet
{
    public RealmPawn realmPawn;

    [NonSerialized] public User userTo;

    private int userToId;

    public override void Apply(User user, RealmData realmData)
    {
        realmData.NotifyPacket(userTo, new ReceivePawnPacket
        {
            userFrom = user,
            realmPawn = realmPawn
        });
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        userToId = userTo.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmContext = (RealmContext)c.Context;
        userTo = ID.Find(realmContext.realmData.users, userToId);
    }
}