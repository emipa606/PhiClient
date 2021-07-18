using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem
{
    // Token: 0x02000024 RID: 36
    [Serializable]
    public class ConfirmServerTransactionPacket : Packet
    {
        // Token: 0x0400007C RID: 124
        public TransactionResponse response;

        // Token: 0x0400007A RID: 122
        private int senderTransactionId;

        // Token: 0x04000079 RID: 121
        [NonSerialized] public Transaction transaction;

        // Token: 0x0400007B RID: 123
        private int transactionId;

        // Token: 0x0600005E RID: 94 RVA: 0x00003EF8 File Offset: 0x000020F8
        public override void Apply(User user, RealmData realmData)
        {
            if (transaction.receiver != user || transaction.state != TransactionResponse.WAITING)
            {
                return;
            }

            transaction.state = response;
            realmData.NotifyPacket(transaction.sender, new ConfirmTransactionPacket
            {
                transaction = transaction,
                response = response,
                toSender = true
            });
            realmData.NotifyPacket(transaction.receiver, new ConfirmTransactionPacket
            {
                transaction = transaction,
                response = response
            });
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00003F97 File Offset: 0x00002197
        [OnSerializing]
        internal void OnSerializingCallback(StreamingContext c)
        {
            transactionId = transaction.id;
            senderTransactionId = transaction.sender.id;
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00003FC0 File Offset: 0x000021C0
        [OnDeserialized]
        internal void OnDeserializedCallback(StreamingContext c)
        {
            var realmContext = (RealmContext) c.Context;
            transaction = realmContext.realmData.FindTransaction(transactionId, senderTransactionId);
        }
    }
}