using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhiClient
{
    // Token: 0x02000010 RID: 16
    [Serializable]
    public abstract class Packet
    {
        // Token: 0x06000019 RID: 25
        public abstract void Apply(User user, RealmData realmData);

        // Token: 0x0600001A RID: 26 RVA: 0x00002320 File Offset: 0x00000520
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

        // Token: 0x0600001B RID: 27 RVA: 0x00002370 File Offset: 0x00000570
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
            return (Packet) binaryFormatter.Deserialize(serializationStream);
        }
    }
}