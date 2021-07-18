using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem
{
    // Token: 0x02000025 RID: 37
    [Serializable]
    public class ConfirmTransactionPacket : Packet
    {
        // Token: 0x04000080 RID: 128
        public TransactionResponse response;

        // Token: 0x0400007F RID: 127
        private int senderTransactionId;

        // Token: 0x04000081 RID: 129
        public bool toSender;

        // Token: 0x0400007D RID: 125
        [NonSerialized] public Transaction transaction;

        // Token: 0x0400007E RID: 126
        private int transactionId;

        // Token: 0x06000062 RID: 98 RVA: 0x00003FF8 File Offset: 0x000021F8
        public override void Apply(User user, RealmData realmData)
        {
            transaction.state = response;
            if (user == transaction.sender && toSender)
            {
                transaction.OnEndSender(realmData);
                return;
            }

            if (user == transaction.receiver && !toSender)
            {
                transaction.OnEndReceiver(realmData);
            }
        }

        // Token: 0x06000063 RID: 99 RVA: 0x0000405B File Offset: 0x0000225B
        [OnSerializing]
        internal void OnSerializingCallback(StreamingContext c)
        {
            transactionId = transaction.id;
            senderTransactionId = transaction.sender.id;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00004084 File Offset: 0x00002284
        [OnDeserialized]
        internal void OnDeserializedCallback(StreamingContext c)
        {
            var realmContext = (RealmContext) c.Context;
            transaction = realmContext.realmData.FindTransaction(transactionId, senderTransactionId);
        }
    }
}