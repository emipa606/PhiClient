using System;
using Verse;

namespace PhiClient;

[Serializable]
public class ChangeNicknamePacket : Packet
{
    public string name;

    public override void Apply(User user, RealmData realmData)
    {
        var filteredName = TextHelper.StripRichText(name, "size");
        filteredName = TextHelper.Clamp(filteredName, 4, 32);
        if (realmData.users.Any(u => u.name == filteredName))
        {
            realmData.NotifyPacket(user, new ErrorPacket
            {
                error = $"Nickname {filteredName} is already taken"
            });
            return;
        }

        user.name = filteredName;
        realmData.BroadcastPacket(new ChangeNicknameNotifyPacket
        {
            user = user,
            name = user.name
        });
    }
}