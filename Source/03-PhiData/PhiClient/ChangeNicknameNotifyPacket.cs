using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class ChangeNicknameNotifyPacket : Packet
{
    public string name;

    [NonSerialized] public User user;

    public int userId;

    public override void Apply(User user, RealmData realmData)
    {
        this.user.name = name;
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