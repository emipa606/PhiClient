using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhiClient;

[Serializable]
public abstract class Packet
{
    public abstract void Apply(User user, RealmData realmData);

    public static byte[] Serialize(Packet packet, RealmData realmData, User user)
    {
        var realmContext = new RealmContext
        {
            realmData = realmData,
            user = user
        };
        var binaryFormatter =
            new BinaryFormatter(null, new StreamingContext(StreamingContextStates.All, realmContext));
        var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, packet);
        return memoryStream.ToArray();
    }

    public static Packet Deserialize(byte[] data, RealmData realmData, User user)
    {
        var realmContext = new RealmContext
        {
            realmData = realmData,
            user = user
        };
        var binaryFormatter =
            new BinaryFormatter(null, new StreamingContext(StreamingContextStates.All, realmContext));
        var serializationStream = new MemoryStream(data);
        return (Packet)binaryFormatter.Deserialize(serializationStream);
    }
}