using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class SynchronisationPacket : Packet
{
    public RealmData realmData;

    [NonSerialized] public User user;

    private int userId;

    public override void Apply(User user, RealmData realmData)
    {
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        userId = user.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        user = ID.Find(realmData.users, userId);
    }
}