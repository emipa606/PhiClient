using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient;

[Serializable]
public class RealmPawn
{
    public string adulthoodKey;

    public long ageBiologicalTicks;

    public long ageChronologicalTicks;

    public List<RealmThing> apparels;

    public string bodyTypeDefName;

    public string childhoodKey;

    public HeadTypeDef crownType;

    public List<RealmThing> equipments;

    public Gender gender;

    public float[] hairColor;

    public string hairDefName;

    public byte healthState = 2;

    public List<RealmHediff> hediffs;

    public List<RealmThing> inventory;

    public string kindDefName;

    public float melanin;

    public string[] name;

    public List<RealmSkillRecord> skills;

    public List<RealmTrainingRecord> training;

    public List<RealmTrait> traits;

    public Dictionary<string, int> workPriorities;

    public static RealmPawn ToRealmPawn(Pawn pawn, RealmData realmData)
    {
        var realmPawn = new RealmPawn();
        var array = pawn.Name.ToString().Split(' ');
        if (array.Length == 3)
        {
            array[1] = array[1].Replace("'", "");
        }

        realmPawn.name = array;
        realmPawn.kindDefName = pawn.kindDef.defName;
        realmPawn.ageBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
        realmPawn.ageChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks;
        realmPawn.gender = pawn.gender;
        if (!pawn.health.hediffSet.HasHediff(PhiHediffDefOf.Phi_Key))
        {
            pawn.health.AddHediff(PhiHediffDefOf.Phi_Key);
            pawn.health.hediffSet.GetFirstHediffOfDef(PhiHediffDefOf.Phi_Key).Severity =
                Rand.Range(0f, float.MaxValue);
        }

        if (pawn.skills != null)
        {
            var list = new List<RealmSkillRecord>();
            foreach (var skillRecord in pawn.skills.skills)
            {
                list.Add(new RealmSkillRecord
                {
                    skillDefLabel = skillRecord.def.label,
                    level = skillRecord.Level,
                    passion = skillRecord.passion
                });
            }

            realmPawn.skills = list;
        }

        if (pawn.story != null)
        {
            var list2 = new List<RealmTrait>();
            foreach (var trait in pawn.story.traits.allTraits)
            {
                list2.Add(new RealmTrait
                {
                    traitDefName = trait.def.defName,
                    degree = trait.Degree
                });
            }

            realmPawn.traits = list2;
            var color = pawn.story.HairColor;
            realmPawn.hairColor = new[]
            {
                color.r,
                color.g,
                color.b,
                color.a
            };
            realmPawn.bodyTypeDefName = pawn.story.bodyType.defName;
            realmPawn.crownType = pawn.story.headType;
            realmPawn.hairDefName = pawn.story.hairDef.defName;
            realmPawn.childhoodKey = pawn.story.Childhood.identifier;
            var realmPawn2 = realmPawn;
            var adulthood = pawn.story.Adulthood;
            realmPawn2.adulthoodKey = adulthood?.identifier;
            realmPawn.melanin = pawn.story.melanin;
        }

        if (pawn.equipment != null)
        {
            var list3 = new List<RealmThing>();
            foreach (var thing in pawn.equipment.AllEquipmentListForReading)
            {
                list3.Add(realmData.ToRealmThing(thing));
            }

            realmPawn.equipments = list3;
        }

        if (pawn.apparel != null)
        {
            var list4 = new List<RealmThing>();
            foreach (var thing2 in pawn.apparel.WornApparel)
            {
                list4.Add(realmData.ToRealmThing(thing2));
            }

            realmPawn.apparels = list4;
        }

        if (pawn.inventory != null)
        {
            var list5 = new List<RealmThing>();
            foreach (var thing3 in pawn.inventory.innerContainer)
            {
                list5.Add(realmData.ToRealmThing(thing3));
            }

            realmPawn.inventory = list5;
        }

        var list6 = new List<RealmHediff>();
        foreach (var hediff in pawn.health.hediffSet.hediffs)
        {
            var immunityRecord = pawn.health.immunity.GetImmunityRecord(hediff.def);
            var bodyPartIndex = -1;
            var part = hediff.Part;
            if (part?.def != null)
            {
                if (!pawn.RaceProps.body.AllParts.Contains(hediff.Part))
                {
                    var format = "Skipping bodypart {0}, not found in body.";
                    var part2 = hediff.Part;
                    object arg;
                    if (part2 == null)
                    {
                        arg = null;
                    }
                    else
                    {
                        var def = part2.def;
                        arg = def?.defName;
                    }

                    Log.Error(string.Format(format, arg));
                    continue;
                }

                bodyPartIndex = pawn.RaceProps.body.GetIndexOfPart(hediff.Part);
            }

            var list7 = list6;
            var realmHediff = new RealmHediff
            {
                hediffDefName = hediff.def.defName,
                bodyPartIndex = bodyPartIndex,
                immunity = immunityRecord?.immunity ?? float.NaN
            };
            var source = hediff.source;
            realmHediff.sourceDefName = source?.defName;
            realmHediff.ageTicks = hediff.ageTicks;
            realmHediff.severity = hediff.Severity;
            list7.Add(realmHediff);
            realmPawn.hediffs = list6;
        }

        var unused = pawn.health.State;
        var dictionary = new Dictionary<string, int>();
        if (pawn.training != null)
        {
            var list8 = new List<RealmTrainingRecord>();
            foreach (var trainableDef in TrainableUtility.TrainableDefsInListOrder)
            {
                list8.Add(new RealmTrainingRecord
                {
                    trainingDefLabel = trainableDef.defName,
                    learned = pawn.training.HasLearned(trainableDef),
                    wanted = pawn.training.GetWanted(trainableDef)
                });
            }

            realmPawn.training = list8;
        }

        if (pawn.workSettings == null)
        {
            return realmPawn;
        }

        foreach (var workTypeDef in DefDatabase<WorkTypeDef>.AllDefs)
        {
            dictionary.Add(workTypeDef.defName, pawn.workSettings.GetPriority(workTypeDef));
        }

        return realmPawn;
    }

    public Pawn FromRealmPawn(RealmData realmData)
    {
        var named = DefDatabase<PawnKindDef>.GetNamed(kindDefName);
        var pawn = (Pawn)ThingMaker.MakeThing(named.race);
        foreach (var pawn2 in Find.WorldPawns.ForcefullyKeptPawns)
        {
            var health = pawn2.health;
            float? num;
            if (health == null)
            {
                num = null;
            }
            else
            {
                var hediffSet = health.hediffSet;
                if (hediffSet == null)
                {
                    num = null;
                }
                else
                {
                    var firstHediffOfDef = hediffSet.GetFirstHediffOfDef(PhiHediffDefOf.Phi_Key);
                    num = firstHediffOfDef != null ? new float?(firstHediffOfDef.Severity) : null;
                }
            }

            var num2 = num ?? -1f;
            var list = hediffs;
            float? num3;
            if (list == null)
            {
                num3 = null;
            }
            else
            {
                var realmHediff = list.First(h => h.hediffDefName == PhiHediffDefOf.Phi_Key.defName);
                num3 = realmHediff != null ? new float?(realmHediff.severity) : null;
            }

            if (num2 != num3)
            {
                continue;
            }

            pawn = pawn2;
            break;
        }

        pawn.kindDef = named;
        pawn.SetFactionDirect(Faction.OfPlayer);
        PawnComponentsUtility.CreateInitialComponents(pawn);
        var pawnName = pawn.Name;
        switch (name.Length)
        {
            case 1:
                pawnName = new NameSingle(name[0]);
                break;
            case 2:
                pawnName = new NameTriple(name[0], name[1], name[1]);
                break;
            case 3:
                pawnName = new NameTriple(name[0], name[1], name[2]);
                break;
        }

        pawn.Name = pawnName;
        pawn.gender = gender;
        pawn.ageTracker.AgeBiologicalTicks = ageBiologicalTicks;
        pawn.ageTracker.AgeChronologicalTicks = ageChronologicalTicks;
        var story = pawn.story;
        if (story != null)
        {
            story.melanin = melanin;
            story.headType = crownType;
            story.HairColor = new Color(hairColor[0], hairColor[1], hairColor[2], hairColor[3]);
            story.Childhood =
                DefDatabase<BackstoryDef>.AllDefsListForReading.First(def => def.identifier == childhoodKey);
            story.Adulthood =
                DefDatabase<BackstoryDef>.AllDefsListForReading.First(def => def.identifier == adulthoodKey);

            story.bodyType = DefDatabase<BodyTypeDef>.GetNamed(bodyTypeDefName);
            story.hairDef = DefDatabase<HairDef>.GetNamed(hairDefName);
            story.traits.allTraits.Clear();
            foreach (var realmTrait in traits)
            {
                var named2 = DefDatabase<TraitDef>.GetNamed(realmTrait.traitDefName);
                story.traits.GainTrait(new Trait(named2, realmTrait.degree));
            }
        }

        if (skills != null)
        {
            using var enumerator3 = skills.AsEnumerable().GetEnumerator();
            while (enumerator3.MoveNext())
            {
                var item2 = enumerator3.Current;
                var skillDef = DefDatabase<SkillDef>.AllDefs.First(def => def.label == item2?.skillDefLabel);
                var skill = pawn.skills.GetSkill(skillDef);
                if (item2 == null)
                {
                    continue;
                }

                skill.Level = item2.level;
                skill.passion = item2.passion;
            }
        }

        var workSettings = pawn.workSettings;
        workSettings?.EnableAndInitialize();

        if (apparels != null)
        {
            var pawn_ApparelTracker = new Pawn_ApparelTracker(pawn);
            foreach (var realmThing in apparels)
            {
                var apparel = (Apparel)realmData.FromRealmThing(realmThing);
                pawn_ApparelTracker.Wear(apparel);
            }
        }

        if (equipments != null)
        {
            var pawn_EquipmentTracker = new Pawn_EquipmentTracker(pawn);
            foreach (var realmThing2 in equipments)
            {
                var newEq = (ThingWithComps)realmData.FromRealmThing(realmThing2);
                pawn_EquipmentTracker.AddEquipment(newEq);
            }
        }

        if (inventory != null)
        {
            var pawn_InventoryTracker = pawn.inventory;
            foreach (var realmThing3 in inventory)
            {
                var item = realmData.FromRealmThing(realmThing3);
                pawn_InventoryTracker.innerContainer.TryAdd(item);
            }
        }

        if (hediffs == null)
        {
            Log.Warning("RealmHediffs is null in received colonist");
        }

        foreach (var realmHediff2 in hediffs ?? new List<RealmHediff>())
        {
            var named3 = DefDatabase<HediffDef>.GetNamed(realmHediff2.hediffDefName);
            BodyPartRecord part = null;
            if (realmHediff2.bodyPartIndex != -1)
            {
                part = pawn.RaceProps.body.GetPartAtIndex(realmHediff2.bodyPartIndex);
            }

            pawn.health.AddHediff(named3, part);
            var hediff = pawn.health.hediffSet.hediffs.Last();
            hediff.source = realmHediff2.sourceDefName == null
                ? null
                : DefDatabase<ThingDef>.GetNamedSilentFail(realmHediff2.sourceDefName);
            hediff.ageTicks = realmHediff2.ageTicks;
            hediff.Severity = realmHediff2.severity;
            if (float.IsNaN(realmHediff2.immunity) || pawn.health.immunity.ImmunityRecordExists(named3))
            {
                continue;
            }

            var immunity = pawn.health.immunity;
            immunity.GetType().GetMethod("TryAddImmunityRecord", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(immunity, new object[]
                {
                    named3
                });
            immunity.GetImmunityRecord(named3).immunity = realmHediff2.immunity;
        }

        if (training != null)
        {
            pawn.training = new Pawn_TrainingTracker(pawn);
            foreach (var realmTrainingRecord in training)
            {
                var named4 = DefDatabase<TrainableDef>.GetNamed(realmTrainingRecord.trainingDefLabel);
                if (realmTrainingRecord.wanted)
                {
                    pawn.training.SetWantedRecursive(named4, false);
                }

                if (realmTrainingRecord.learned)
                {
                    pawn.training.Train(named4, null, true);
                }
            }
        }

        var field = pawn.health.GetType().GetField("healthState", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            Log.Error("Unable to find healthState field");
        }
        else
        {
            field.SetValue(pawn.health, healthState);
        }

        if (workPriorities == null)
        {
            return pawn;
        }

        foreach (var keyValuePair in workPriorities ?? new Dictionary<string, int>())
        {
            var namedSilentFail = DefDatabase<WorkTypeDef>.GetNamedSilentFail(keyValuePair.Key);
            if (namedSilentFail == null)
            {
                Log.Warning($"Ignoring unknown workType: {keyValuePair.Key}");
            }
            else
            {
                pawn.workSettings.SetPriority(namedSilentFail, keyValuePair.Value);
            }
        }

        return pawn;
    }
}