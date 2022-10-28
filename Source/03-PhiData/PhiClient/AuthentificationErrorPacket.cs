using System;
using Verse;

namespace PhiClient;

[Serializable]
public class AuthentificationErrorPacket : Packet
{
    public string error;

    public override void Apply(User user, RealmData realmData)
    {
        Log.Warning($"Couldn't authenticate:{error}");
    }
}