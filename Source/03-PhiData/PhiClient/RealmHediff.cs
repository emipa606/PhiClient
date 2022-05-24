using System;

namespace PhiClient;

[Serializable]
public class RealmHediff
{
    public int ageTicks;

    public int bodyPartIndex;

    public string hediffDefName;

    public float immunity = float.NaN;

    public float severity;

    public string sourceDefName;
}