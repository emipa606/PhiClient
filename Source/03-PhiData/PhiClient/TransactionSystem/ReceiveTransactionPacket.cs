using System;

namespace PhiClient.TransactionSystem
{
    // Token: 0x02000027 RID: 39
    [Serializable]
    public class ReceiveTransactionPacket : Packet
    {
        // Token: 0x04000084 RID: 132
        public Transaction transaction;

        // Token: 0x0600006A RID: 106 RVA: 0x00004534 File Offset: 0x00002734
        public override void Apply(User user, RealmData realmData)
        {
            if (realmData.TryFindTransaction(transaction.id, transaction.sender.id) == null)
            {
                realmData.transactions.Add(transaction);
            }

            if (user == transaction.receiver)
            {
                transaction.OnStartReceiver(realmData);
            }
        }
    }
}