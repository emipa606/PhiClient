using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem
{
    // Token: 0x02000029 RID: 41
    [Serializable]
    public abstract class Transaction : IDable
    {
        // Token: 0x04000086 RID: 134
        public int id;

        // Token: 0x04000089 RID: 137
        [NonSerialized] public User receiver;

        // Token: 0x0400008A RID: 138
        private int receiverId;

        // Token: 0x04000087 RID: 135
        [NonSerialized] public User sender;

        // Token: 0x04000088 RID: 136
        private int senderId;

        // Token: 0x0400008B RID: 139
        public TransactionResponse state;

        // Token: 0x0600006E RID: 110 RVA: 0x000045EC File Offset: 0x000027EC
        public Transaction(int id, User sender, User receiver)
        {
            this.id = id;
            this.sender = sender;
            this.receiver = receiver;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00004609 File Offset: 0x00002809
        public int getID()
        {
            return id;
        }

        // Token: 0x0600006F RID: 111
        public abstract void OnStartReceiver(RealmData realmData);

        // Token: 0x06000070 RID: 112
        public abstract void OnEndReceiver(RealmData realmData);

        // Token: 0x06000071 RID: 113
        public abstract void OnEndSender(RealmData realmData);

        // Token: 0x06000073 RID: 115 RVA: 0x00004611 File Offset: 0x00002811
        public bool IsFinished()
        {
            return state == TransactionResponse.ACCEPTED || state == TransactionResponse.DECLINED ||
                   state == TransactionResponse.INTERRUPTED || state == TransactionResponse.TOOFAST;
        }

        // Token: 0x06000074 RID: 116 RVA: 0x00004639 File Offset: 0x00002839
        [OnSerializing]
        internal void OnSerializingCallback(StreamingContext c)
        {
            senderId = sender.id;
            receiverId = receiver.id;
        }

        // Token: 0x06000075 RID: 117 RVA: 0x00004660 File Offset: 0x00002860
        [OnDeserialized]
        internal void OnDeserializedCallback(StreamingContext c)
        {
            var realmData = ((RealmContext) c.Context).realmData;
            if (realmData == null)
            {
                return;
            }

            sender = ID.Find(realmData.users, senderId);
            receiver = ID.Find(realmData.users, receiverId);
        }
    }
}