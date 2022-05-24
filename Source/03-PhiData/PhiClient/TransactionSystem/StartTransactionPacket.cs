using System;

namespace PhiClient.TransactionSystem;

[Serializable]
public class StartTransactionPacket : Packet
{
    public Transaction transaction;

    public override void Apply(User user, RealmData realmData)
    {
        realmData.transactions.Add(transaction);
        user.lastTransactionId = transaction.id;
        user.lastTransactionTime = DateTime.Now;
        realmData.NotifyPacket(transaction.receiver, new ReceiveTransactionPacket
        {
            transaction = transaction
        });
    }
}