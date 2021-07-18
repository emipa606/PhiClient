using System;
using System.Runtime.Serialization;

namespace PhiClient
{
    // Token: 0x02000009 RID: 9
    [Serializable]
    public class ChatMessage
    {
        // Token: 0x04000015 RID: 21
        public string message;

        // Token: 0x04000013 RID: 19
        public User user;

        // Token: 0x04000014 RID: 20
        public int userId;

        // Token: 0x0600000C RID: 12 RVA: 0x000021A3 File Offset: 0x000003A3
        [OnSerializing]
        internal void OnSerializingCallback(StreamingContext c)
        {
            userId = user.id;
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000021B8 File Offset: 0x000003B8
        [OnDeserialized]
        internal void OnDeserializedCallback(StreamingContext c)
        {
            var realmContext = (RealmContext) c.Context;
            if (realmContext.realmData != null)
            {
                user = ID.Find(realmContext.realmData.users, userId);
            }
        }
    }
}