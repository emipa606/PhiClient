using System;
using System.Runtime.Serialization;

namespace PhiClient.TransactionSystem;

[Serializable]
public abstract class Transaction : IDable
{
    public int id;

    [NonSerialized] public User receiver;

    private int receiverId;

    [NonSerialized] public User sender;

    private int senderId;

    public TransactionResponse state;

    public Transaction(int id, User sender, User receiver)
    {
        this.id = id;
        this.sender = sender;
        this.receiver = receiver;
    }

    public int getID()
    {
        return id;
    }

    public abstract void OnStartReceiver(RealmData realmData);

    public abstract void OnEndReceiver(RealmData realmData);

    public abstract void OnEndSender(RealmData realmData);

    public bool IsFinished()
    {
        return state == TransactionResponse.ACCEPTED || state == TransactionResponse.DECLINED ||
               state == TransactionResponse.INTERRUPTED || state == TransactionResponse.TOOFAST;
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        senderId = sender.id;
        receiverId = receiver.id;
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        var realmData = ((RealmContext)c.Context).realmData;
        if (realmData == null)
        {
            return;
        }

        sender = ID.Find(realmData.users, senderId);
        receiver = ID.Find(realmData.users, receiverId);
    }
}