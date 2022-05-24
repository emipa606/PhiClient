using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem;

[Serializable]
public class ConfirmServerTransactionPacket : Packet
{
    public TransactionResponse response;

    private int senderTransactionId;

    [NonSerialized] public Transaction transaction;

    private int transactionId;

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

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        transactionId = transaction.id;
        senderTransactionId = transaction.sender.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmContext = (RealmContext)c.Context;
        transaction = realmContext.realmData.FindTransaction(transactionId, senderTransactionId);
    }
}