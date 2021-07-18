using System;

namespace PhiClient
{
    // Token: 0x0200001F RID: 31
    [Serializable]
    public class UpdatePreferencesPacket : Packet
    {
        // Token: 0x04000066 RID: 102
        public UserPreferences preferences;

        // Token: 0x06000051 RID: 81 RVA: 0x00003B2A File Offset: 0x00001D2A
        public override void Apply(User user, RealmData realmData)
        {
            user.preferences = preferences;
            realmData.BroadcastPacketExcept(new UpdatePreferencesNotifyPacket
            {
                user = user,
                preferences = preferences
            }, user);
        }
    }
}