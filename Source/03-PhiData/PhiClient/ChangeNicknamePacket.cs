using System;
using Verse;

namespace PhiClient
{
    // Token: 0x02000008 RID: 8
    [Serializable]
    public class ChangeNicknamePacket : Packet
    {
        // Token: 0x04000012 RID: 18
        public string name;

        // Token: 0x0600000A RID: 10 RVA: 0x000020F0 File Offset: 0x000002F0
        public override void Apply(User user, RealmData realmData)
        {
            var filteredName = TextHelper.StripRichText(name, "size");
            filteredName = TextHelper.Clamp(filteredName, 4, 32);
            if (realmData.users.Any(u => u.name == filteredName))
            {
                realmData.NotifyPacket(user, new ErrorPacket
                {
                    error = "Nickname " + filteredName + " is already taken"
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
}