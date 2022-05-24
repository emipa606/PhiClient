using System;
using System.Runtime.Serialization;

namespace PhiClient;

[Serializable]
public class ChatMessage
{
    public string message;

    public User user;

    public int userId;

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        userId = user.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmContext = (RealmContext)c.Context;
        if (realmContext.realmData != null)
        {
            user = ID.Find(realmContext.realmData.users, userId);
        }
    }
}