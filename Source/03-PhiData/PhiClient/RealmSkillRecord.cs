using System;
using RimWorld;

namespace PhiClient;

[Serializable]
public class RealmSkillRecord
{
    public int level;

    public Passion passion;

    public string skillDefLabel;
}