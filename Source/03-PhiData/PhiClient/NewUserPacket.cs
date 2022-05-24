using System;

namespace PhiClient;

[Serializable]
public class NewUserPacket : Packet
{
    public User user;

    public override void Apply(User user, RealmData realmData)
    {
        realmData.AddUser(this.user);
    }
}