using System;

namespace PhiClient.TransactionSystem;

[Serializable]
public class ReceiveTransactionPacket : Packet
{
    public Transaction transaction;

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