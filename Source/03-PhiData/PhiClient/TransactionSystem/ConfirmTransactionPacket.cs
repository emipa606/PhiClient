using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem;

[Serializable]
public class ConfirmTransactionPacket : Packet
{
    public TransactionResponse response;

    private int senderTransactionId;

    public bool toSender;

    [NonSerialized] public Transaction transaction;

    private int transactionId;

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