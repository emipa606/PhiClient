using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PhiClient.TransactionSystem;
using RimWorld;
using Verse;

namespace PhiClient;

[Serializable]
public class RealmData
{
    public delegate void LogHandler(LogLevel level, string message);

    public delegate void PacketHandler(User user, Packet packet);

    public const string VERSION = "1.3.0";

    public const int CHAT_MESSAGES_TO_SEND = 30;

    public const int CHAT_MESSAGE_MAX_LENGTH = 250;

    [NonSerialized] public List<ChatMessage> chat = new List<ChatMessage>();

    public int lastUserGivenId;

    private List<ChatMessage> serializeChat;

    private List<Transaction> serializeTransactions;

    [NonSerialized] public List<Transaction> transactions = new List<Transaction>();

    public List<User> users = new List<User>();

    [field: NonSerialized] public event PacketHandler PacketToClient;

    [field: NonSerialized] public event LogHandler Log;

    public event Action<Packet> PacketToServer;

    public void AddUser(User user)
    {
        users.Add(user);
    }

    public void AddChatMessage(ChatMessage message)
    {
        chat.Add(message);
    }

    public bool CanStartTransaction(User sender, User receiver)
    {
        return !transactions.Exists(t => !t.IsFinished() && t.sender == sender && t.receiver == receiver);
    }

    public Transaction TryFindTransaction(int transactionId, int transactionSenderId)
    {
        return transactions.FindLast(t =>
            t?.sender != null && t.getID() == transactionId && t.sender.getID() == transactionSenderId);
    }

    public void EmitLog(LogLevel level, string message)
    {
        Log?.Invoke(level, message);
    }

    public Transaction FindTransaction(int transactionId, int transactionSenderId)
    {
        var transaction = TryFindTransaction(transactionId, transactionSenderId);
        if (transaction == null)
        {
            throw new Exception($"Couldn't find Transaction {transactionId} from sender {transactionSenderId}");
        }

        return transaction;
    }

    public void NotifyPacketToServer(Packet packet)
    {
        PacketToServer?.Invoke(packet);
    }

    public void NotifyPacket(User user, Packet packet)
    {
        PacketToClient?.Invoke(user, packet);
    }

    public void BroadcastPacket(Packet packet)
    {
        foreach (var user in users)
        {
            NotifyPacket(user, packet);
        }
    }

    public void BroadcastPacketExcept(Packet packet, User excludedUser)
    {
        foreach (var user in users)
        {
            if (user != excludedUser)
            {
                NotifyPacket(user, packet);
            }
        }
    }

    public User ServerAddUser(string name, int id)
    {
        var user = new User
        {
            id = id,
            name = name,
            connected = true,
            inGame = false
        };
        AddUser(user);
        EmitLog(LogLevel.INFO, $"Created user {name} ({id})");
        return user;
    }

    public void ServerPostMessage(User user, string message)
    {
        if (message.Length < 1)
        {
            return;
        }

        var text = TextHelper.StripRichText(message, "size");
        text = TextHelper.Clamp(text, 1, 250);
        var message2 = new ChatMessage
        {
            user = user,
            message = text
        };
        AddChatMessage(message2);
        EmitLog(LogLevel.INFO, $"{user.name}: {message}");
        BroadcastPacket(new ChatMessagePacket
        {
            message = message2
        });
    }

    public RealmThing ToRealmThing(Thing thing)
    {
        var stuffDefName = thing.Stuff != null ? thing.Stuff.defName : "";
        var compQuality = -1;
        if (thing.TryGetComp<CompQuality>() != null)
        {
            compQuality = (int)thing.TryGetComp<CompQuality>().Quality;
        }

        if (thing is not MinifiedThing)
        {
            return new RealmThing
            {
                thingDefName = thing.def.defName,
                stackCount = thing.stackCount,
                stuffDefName = stuffDefName,
                compQuality = compQuality,
                hitPoints = thing.HitPoints,
                innerThing = null
            };
        }

        var minifiedThing = (MinifiedThing)thing;
        var innerThing = ToRealmThing(minifiedThing.InnerThing);

        return new RealmThing
        {
            thingDefName = thing.def.defName,
            stackCount = thing.stackCount,
            stuffDefName = stuffDefName,
            compQuality = compQuality,
            hitPoints = thing.HitPoints,
            innerThing = innerThing
        };
    }

    public Thing FromRealmThing(RealmThing realmThing)
    {
        var def2 = DefDatabase<ThingDef>.AllDefs.First(def => def.defName == realmThing.thingDefName);
        ThingDef stuff = null;
        if (realmThing.stuffDefName != "")
        {
            stuff = DefDatabase<ThingDef>.AllDefs.First(def => def.defName == realmThing.stuffDefName);
        }

        var thing = ThingMaker.MakeThing(def2, stuff);
        thing.stackCount = realmThing.stackCount;
        var compQuality = thing.TryGetComp<CompQuality>();
        if (compQuality != null && realmThing.compQuality != -1)
        {
            compQuality.SetQuality((QualityCategory)realmThing.compQuality, ArtGenerationContext.Outsider);
        }

        thing.HitPoints = realmThing.hitPoints;
        if (thing is MinifiedThing minifiedThing)
        {
            minifiedThing.InnerThing = FromRealmThing(realmThing.innerThing);
        }

        return thing;
    }

    [OnSerializing]
    internal void OnSerializingCallback(StreamingContext c)
    {
        var index = Math.Max(0, chat.Count - 30);
        var count = Math.Min(chat.Count, 30);
        serializeChat = chat.GetRange(index, count);
        serializeTransactions = new List<Transaction>();
    }

    [OnDeserialized]
    internal void OnDeserializedCallback(StreamingContext c)
    {
        chat = serializeChat;
        transactions = serializeTransactions;
    }
}