using System;

namespace PhiClient;

[Serializable]
public class AuthentificationPacket : Packet
{
    public string hashedKey;

    public int? id;

    public string name;

    public string version;

    public override void Apply(User user, RealmData realmData)
    {
    }
}