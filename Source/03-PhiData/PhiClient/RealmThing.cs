using System;

namespace PhiClient;

[Serializable]
public class RealmThing
{
    public int compQuality;

    public int hitPoints;

    public RealmThing innerThing;

    public int senderThingId;

    public int stackCount;

    public string stuffDefName;

    public string thingDefName;
}