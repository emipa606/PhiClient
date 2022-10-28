using System;
using System.Collections.Generic;
using System.Linq;
using PhiClient.Legacy;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PhiClient.TransactionSystem;

[Serializable]
public class ItemTransaction : Transaction
{
    public List<KeyValuePair<RealmThing, int>> realmThings;

    [NonSerialized] public Dictionary<List<Thing>, int> things;

    public ItemTransaction(int id, User sender, User receiver, Dictionary<List<Thing>, int> things,
        List<KeyValuePair<RealmThing, int>> realmThings) : base(id, sender, receiver)
    {
        this.things = things;
        this.realmThings = realmThings;
    }

    public override void OnStartReceiver(RealmData realmData)
    {
        if (!receiver.preferences.receiveItems)
        {
            realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
            {
                transaction = this,
                response = TransactionResponse.DECLINED
            });
            return;
        }

        var source = realmThings.Select(delegate(KeyValuePair<RealmThing, int> r)
        {
            var realmData2 = realmData;
            var keyValuePair = r;
            var key = realmData2.FromRealmThing(keyValuePair.Key);
            keyValuePair = r;
            return new KeyValuePair<Thing, int>(key, keyValuePair.Value);
        }).ToList();
        var str = string.Join("\n", source.Select(delegate(KeyValuePair<Thing, int> t)
        {
            var keyValuePair = t;
            var str2 = keyValuePair.Value.ToString();
            var str3 = "x ";
            keyValuePair = t;
            return str2 + str3 + keyValuePair.Key.LabelCapNoCount;
        }).ToArray());
        var window = new Dialog_GeneralChoice(new DialogChoiceConfig
        {
            text = $"{sender.name} wants to ship you:\n{str}",
            buttonAText = "Accept",
            buttonAAction = delegate
            {
                realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
                {
                    transaction = this,
                    response = TransactionResponse.ACCEPTED
                });
            },
            buttonBText = "Refuse",
            buttonBAction = delegate
            {
                realmData.NotifyPacketToServer(new ConfirmServerTransactionPacket
                {
                    transaction = this,
                    response = TransactionResponse.DECLINED
                });
            }
        });
        Find.WindowStack.Add(window);
    }

    public override void OnEndReceiver(RealmData realmData)
    {
        if (!receiver.preferences.receiveItems)
        {
            state = TransactionResponse.DECLINED;
        }

        if (state == TransactionResponse.ACCEPTED)
        {
            var list = new List<Thing>();
            foreach (var keyValuePair in realmThings)
            {
                var key = keyValuePair.Key;
                var thing = realmData.FromRealmThing(key);
                for (var i = keyValuePair.Value; i > 0; i -= key.stackCount)
                {
                    key.stackCount = Math.Min(i, thing.def.stackLimit);
                    list.Add(realmData.FromRealmThing(key));
                }
            }

            var intVec = DropCellFinder.RandomDropSpot(Find.CurrentMap);
            DropPodUtility.DropThingsNear(intVec, Find.CurrentMap, list);
            Find.LetterStack.ReceiveLetter("Ship pod", $"A pod was sent from {sender.name} containing items",
                LetterDefOf.PositiveEvent, new GlobalTargetInfo(intVec, Find.CurrentMap));
            return;
        }

        if (state == TransactionResponse.INTERRUPTED)
        {
            Messages.Message($"Unexpected interruption during item transaction with {sender.name}",
                MessageTypeDefOf.RejectInput);
            return;
        }

        var unused = state;
    }

    public override void OnEndSender(RealmData realmData)
    {
        if (!receiver.preferences.receiveItems)
        {
            state = TransactionResponse.DECLINED;
        }

        if (state == TransactionResponse.ACCEPTED)
        {
            foreach (var keyValuePair in things)
            {
                var num = keyValuePair.Value;
                foreach (var thing in keyValuePair.Key)
                {
                    if (thing.Destroyed)
                    {
                        continue;
                    }

                    var num2 = Math.Min(num, thing.stackCount);
                    if (num2 == thing.stackCount)
                    {
                        thing.Destroy();
                    }
                    else
                    {
                        thing.stackCount -= num2;
                    }

                    num -= num2;
                }

                if (num > 0)
                {
                    Log.Warning(
                        $"Trying to destroy {keyValuePair.Key[0].LabelShort} but couldn't destroy the {num} remaining");
                }
            }

            Messages.Message($"{receiver.name} accepted your items", MessageTypeDefOf.NeutralEvent);
            return;
        }

        if (state == TransactionResponse.DECLINED)
        {
            Messages.Message($"{receiver.name} declined your items", MessageTypeDefOf.RejectInput);
            return;
        }

        if (state == TransactionResponse.INTERRUPTED)
        {
            Messages.Message($"Unexpected interruption during item transaction with {receiver.name}",
                MessageTypeDefOf.RejectInput);
            return;
        }

        if (state == TransactionResponse.TOOFAST)
        {
            Messages.Message(
                $"Transaction with {receiver.name} was declined by the server. Are you sending items too quickly?",
                MessageTypeDefOf.RejectInput);
        }
    }
}