using System;
using System.Runtime.Serialization;

namespace PhiClient
{
    // Token: 0x0200001E RID: 30
    [Serializable]
    public class UpdatePreferencesNotifyPacket : Packet
    {
        // Token: 0x04000065 RID: 101
        public UserPreferences preferences;

        // Token: 0x04000063 RID: 99
        [NonSerialized] public User user;

        // Token: 0x04000064 RID: 100
        public int userId;

        // Token: 0x0600004D RID: 77 RVA: 0x00003ACD File Offset: 0x00001CCD
        public override void Apply(User user, RealmData realmData)
        {
            this.user.preferences = preferences;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x00003AE0 File Offset: 0x00001CE0
        [OnSerializing]
        internal void OnSerializingCallback(StreamingContext c)
        {
            userId = user.id;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00003AF4 File Offset: 0x00001CF4
        [OnDeserialized]
        internal void OnDeserializedCallback(StreamingContext c)
        {
            var realmContext = (RealmContext) c.Context;
            user = ID.Find(realmContext.realmData.users, userId);
        }
    }
}