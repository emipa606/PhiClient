using System;

namespace PhiClient;

[Serializable]
public class UpdatePreferencesPacket : Packet
{
    public UserPreferences preferences;

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