using System;
using PhiClient.Legacy;
using Verse;

namespace PhiClient;

[Serializable]
public class ErrorPacket : Packet
{
    public string error;

    public override void Apply(User user, RealmData realmData)
    {
        var window = new Dialog_Confirm(error, delegate { });
        Find.WindowStack.Add(window);
    }
}