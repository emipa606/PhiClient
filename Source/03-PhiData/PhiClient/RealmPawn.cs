using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace PhiClient
{
	// Token: 0x02000015 RID: 21
	[Serializable]
	public class RealmPawn
	{
		// Token: 0x06000037 RID: 55 RVA: 0x000029D0 File Offset: 0x00000BD0
		public static RealmPawn ToRealmPawn(Pawn pawn, RealmData realmData)
		{
			RealmPawn realmPawn = new RealmPawn();
			string[] array = pawn.Name.ToString().Split(new char[]
			{
				' '
			});
			if (array.Count<string>() == 3)
			{
				array[1] = array[1].Replace("'", "");
			}
			realmPawn.name = array;
			realmPawn.kindDefName = pawn.kindDef.defName;
			realmPawn.ageBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
			realmPawn.ageChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks;
			realmPawn.gender = pawn.gender;
			if (!pawn.health.hediffSet.HasHediff(PhiHediffDefOf.Phi_Key, false))
			{
				pawn.health.AddHediff(PhiHediffDefOf.Phi_Key, null, null, null);
				pawn.health.hediffSet.GetFirstHediffOfDef(PhiHediffDefOf.Phi_Key, false).Severity = Rand.Range(0f, float.MaxValue);
			}
			if (pawn.skills != null)
			{
				List<RealmSkillRecord> list = new List<RealmSkillRecord>();
				foreach (SkillRecord skillRecord in pawn.skills.skills)
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
				List<RealmTrait> list2 = new List<RealmTrait>();
				foreach (Trait trait in pawn.story.traits.allTraits)
				{
					list2.Add(new RealmTrait
					{
						traitDefName = trait.def.defName,
						degree = trait.Degree
					});
				}
				realmPawn.traits = list2;
				Color color = pawn.story.hairColor;
				realmPawn.hairColor = new float[]
				{
					color.r,
					color.g,
					color.b,
					color.a
				};
				realmPawn.bodyTypeDefName = pawn.story.bodyType.defName;
				realmPawn.crownType = pawn.story.crownType;
				realmPawn.hairDefName = pawn.story.hairDef.defName;
				realmPawn.childhoodKey = pawn.story.childhood.identifier;
				RealmPawn realmPawn2 = realmPawn;
				Backstory adulthood = pawn.story.adulthood;
				realmPawn2.adulthoodKey = ((adulthood != null) ? adulthood.identifier : null);
				realmPawn.melanin = pawn.story.melanin;
			}
			if (pawn.equipment != null)
			{
				List<RealmThing> list3 = new List<RealmThing>();
				foreach (ThingWithComps thing in pawn.equipment.AllEquipmentListForReading)
				{
					list3.Add(realmData.ToRealmThing(thing));
				}
				realmPawn.equipments = list3;
			}
			if (pawn.apparel != null)
			{
				List<RealmThing> list4 = new List<RealmThing>();
				foreach (Apparel thing2 in pawn.apparel.WornApparel)
				{
					list4.Add(realmData.ToRealmThing(thing2));
				}
				realmPawn.apparels = list4;
			}
			if (pawn.inventory != null)
			{
				List<RealmThing> list5 = new List<RealmThing>() ?? new List<RealmThing>();
				foreach (Thing thing3 in pawn.inventory.innerContainer)
				{
					list5.Add(realmData.ToRealmThing(thing3));
				}
				realmPawn.inventory = list5;
			}
			List<RealmHediff> list6 = new List<RealmHediff>();
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
			{
				ImmunityRecord immunityRecord = pawn.health.immunity.GetImmunityRecord(hediff.def);
				int bodyPartIndex = -1;
				BodyPartRecord part = hediff.Part;
				if (((part != null) ? part.def : null) != null)
				{
					if (!pawn.RaceProps.body.AllParts.Contains(hediff.Part))
					{
						string format = "Skipping bodypart {0}, not found in body.";
						BodyPartRecord part2 = hediff.Part;
						object arg;
						if (part2 == null)
						{
							arg = null;
						}
						else
						{
							BodyPartDef def = part2.def;
							arg = ((def != null) ? def.defName : null);
						}
						Log.Error(string.Format(format, arg), false);
						continue;
					}
					bodyPartIndex = pawn.RaceProps.body.GetIndexOfPart(hediff.Part);
				}
				List<RealmHediff> list7 = list6;
				RealmHediff realmHediff = new RealmHediff();
				realmHediff.hediffDefName = hediff.def.defName;
				realmHediff.bodyPartIndex = bodyPartIndex;
				realmHediff.immunity = ((immunityRecord != null) ? immunityRecord.immunity : float.NaN);
				ThingDef source = hediff.source;
				realmHediff.sourceDefName = ((source != null) ? source.defName : null);
				realmHediff.ageTicks = hediff.ageTicks;
				realmHediff.severity = hediff.Severity;
				list7.Add(realmHediff);
				realmPawn.hediffs = list6;
			}
			PawnHealthState state = pawn.health.State;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (pawn.training != null)
			{
				List<RealmTrainingRecord> list8 = new List<RealmTrainingRecord>();
				foreach (TrainableDef trainableDef in TrainableUtility.TrainableDefsInListOrder)
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
			if (pawn.workSettings != null)
			{
				foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefs)
				{
					dictionary.Add(workTypeDef.defName, pawn.workSettings.GetPriority(workTypeDef));
				}
			}
			return realmPawn;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000030B0 File Offset: 0x000012B0
		public Pawn FromRealmPawn(RealmData realmData)
		{
			PawnKindDef named = DefDatabase<PawnKindDef>.GetNamed(this.kindDefName, true);
			Pawn pawn = (Pawn)ThingMaker.MakeThing(named.race, null);
			foreach (Pawn pawn2 in Find.WorldPawns.ForcefullyKeptPawns)
			{
				Pawn_HealthTracker health = pawn2.health;
				float? num;
				if (health == null)
				{
					num = null;
				}
				else
				{
					HediffSet hediffSet = health.hediffSet;
					if (hediffSet == null)
					{
						num = null;
					}
					else
					{
						Hediff firstHediffOfDef = hediffSet.GetFirstHediffOfDef(PhiHediffDefOf.Phi_Key, false);
						num = ((firstHediffOfDef != null) ? new float?(firstHediffOfDef.Severity) : null);
					}
				}
				float num2 = num ?? -1f;
				List<RealmHediff> list = this.hediffs;
				float? num3;
				if (list == null)
				{
					num3 = null;
				}
				else
				{
					RealmHediff realmHediff = list.First((RealmHediff h) => h.hediffDefName == PhiHediffDefOf.Phi_Key.defName);
					num3 = ((realmHediff != null) ? new float?(realmHediff.severity) : null);
				}
				if (num2 == num3)
				{
					pawn = pawn2;
					break;
				}
			}
			pawn.kindDef = named;
			pawn.SetFactionDirect(Faction.OfPlayer);
			PawnComponentsUtility.CreateInitialComponents(pawn);
			Name name = pawn.Name;
			switch (this.name.Count<string>())
			{
			case 1:
				name = new NameSingle(this.name[0], false);
				break;
			case 2:
				name = new NameTriple(this.name[0], this.name[1], this.name[1]);
				break;
			case 3:
				name = new NameTriple(this.name[0], this.name[1], this.name[2]);
				break;
			}
			pawn.Name = name;
			pawn.gender = this.gender;
			pawn.ageTracker.AgeBiologicalTicks = this.ageBiologicalTicks;
			pawn.ageTracker.AgeChronologicalTicks = this.ageChronologicalTicks;
			Pawn_StoryTracker story = pawn.story;
			if (story != null)
			{
				story.melanin = this.melanin;
				story.crownType = this.crownType;
				story.hairColor = new Color(this.hairColor[0], this.hairColor[1], this.hairColor[2], this.hairColor[3]);
				if (!BackstoryDatabase.TryGetWithIdentifier(this.childhoodKey, out story.childhood, true))
				{
					throw new Exception(string.Format("Couldn't find backstory '{0}'", this.childhoodKey));
				}
				if (!string.IsNullOrEmpty(this.adulthoodKey) && !BackstoryDatabase.TryGetWithIdentifier(this.adulthoodKey, out story.adulthood, true))
				{
					throw new Exception(string.Format("Couldn't find backstory '{0}'", this.adulthoodKey));
				}
				story.bodyType = DefDatabase<BodyTypeDef>.GetNamed(this.bodyTypeDefName, true);
				story.hairDef = DefDatabase<HairDef>.GetNamed(this.hairDefName, true);
				story.traits.allTraits.Clear();
				foreach (RealmTrait realmTrait in this.traits)
				{
					TraitDef named2 = DefDatabase<TraitDef>.GetNamed(realmTrait.traitDefName, true);
					story.traits.GainTrait(new Trait(named2, realmTrait.degree, false));
				}
			}
			if (this.skills != null)
			{
				using (IEnumerator<RealmSkillRecord> enumerator3 = this.skills.AsEnumerable<RealmSkillRecord>().GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						RealmSkillRecord item2 = enumerator3.Current;
						SkillDef skillDef = DefDatabase<SkillDef>.AllDefs.First((SkillDef def) => def.label == item2.skillDefLabel);
						SkillRecord skill = pawn.skills.GetSkill(skillDef);
						skill.Level = item2.level;
						skill.passion = item2.passion;
					}
				}
			}
			Pawn_WorkSettings workSettings = pawn.workSettings;
			if (workSettings != null)
			{
				workSettings.EnableAndInitialize();
			}
			if (this.apparels != null)
			{
				Pawn_ApparelTracker pawn_ApparelTracker = new Pawn_ApparelTracker(pawn);
				foreach (RealmThing realmThing in this.apparels)
				{
					Apparel apparel = (Apparel)realmData.FromRealmThing(realmThing);
					pawn_ApparelTracker.Wear(apparel, true);
				}
			}
			if (this.equipments != null)
			{
				Pawn_EquipmentTracker pawn_EquipmentTracker = new Pawn_EquipmentTracker(pawn);
				foreach (RealmThing realmThing2 in this.equipments)
				{
					ThingWithComps newEq = (ThingWithComps)realmData.FromRealmThing(realmThing2);
					pawn_EquipmentTracker.AddEquipment(newEq);
				}
			}
			if (this.inventory != null)
			{
				Pawn_InventoryTracker pawn_InventoryTracker = pawn.inventory;
				foreach (RealmThing realmThing3 in this.inventory)
				{
					Thing item = realmData.FromRealmThing(realmThing3);
					pawn_InventoryTracker.innerContainer.TryAdd(item, true);
				}
			}
			if (this.hediffs == null)
			{
				Log.Warning("RealmHediffs is null in received colonist", false);
			}
			foreach (RealmHediff realmHediff2 in (this.hediffs ?? new List<RealmHediff>()))
			{
				HediffDef named3 = DefDatabase<HediffDef>.GetNamed(realmHediff2.hediffDefName, true);
				BodyPartRecord part = null;
				if (realmHediff2.bodyPartIndex != -1)
				{
					part = pawn.RaceProps.body.GetPartAtIndex(realmHediff2.bodyPartIndex);
				}
				pawn.health.AddHediff(named3, part, null, null);
				Hediff hediff = pawn.health.hediffSet.hediffs.Last<Hediff>();
				hediff.source = ((realmHediff2.sourceDefName == null) ? null : DefDatabase<ThingDef>.GetNamedSilentFail(realmHediff2.sourceDefName));
				hediff.ageTicks = realmHediff2.ageTicks;
				hediff.Severity = realmHediff2.severity;
				if (!float.IsNaN(realmHediff2.immunity) && !pawn.health.immunity.ImmunityRecordExists(named3))
				{
					ImmunityHandler immunity = pawn.health.immunity;
					immunity.GetType().GetMethod("TryAddImmunityRecord", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(immunity, new object[]
					{
						named3
					});
					immunity.GetImmunityRecord(named3).immunity = realmHediff2.immunity;
				}
			}
			if (this.training != null)
			{
				pawn.training = new Pawn_TrainingTracker(pawn);
				foreach (RealmTrainingRecord realmTrainingRecord in this.training)
				{
					TrainableDef named4 = DefDatabase<TrainableDef>.GetNamed(realmTrainingRecord.trainingDefLabel, true);
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
			FieldInfo field = pawn.health.GetType().GetField("healthState", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field == null)
			{
				Log.Error("Unable to find healthState field", false);
			}
			else
			{
				field.SetValue(pawn.health, this.healthState);
			}
			if (this.workPriorities != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair in (this.workPriorities ?? new Dictionary<string, int>()))
				{
					WorkTypeDef namedSilentFail = DefDatabase<WorkTypeDef>.GetNamedSilentFail(keyValuePair.Key);
					if (namedSilentFail == null)
					{
						Log.Warning(string.Format("Ignoring unknown workType: {0}", keyValuePair.Key), false);
					}
					else
					{
						pawn.workSettings.SetPriority(namedSilentFail, keyValuePair.Value);
					}
				}
			}
			return pawn;
		}

		// Token: 0x04000032 RID: 50
		public string kindDefName;

		// Token: 0x04000033 RID: 51
		public string[] name;

		// Token: 0x04000034 RID: 52
		public long ageBiologicalTicks;

		// Token: 0x04000035 RID: 53
		public long ageChronologicalTicks;

		// Token: 0x04000036 RID: 54
		public string bodyTypeDefName;

		// Token: 0x04000037 RID: 55
		public float[] hairColor;

		// Token: 0x04000038 RID: 56
		public CrownType crownType;

		// Token: 0x04000039 RID: 57
		public string hairDefName;

		// Token: 0x0400003A RID: 58
		public string childhoodKey;

		// Token: 0x0400003B RID: 59
		public string adulthoodKey;

		// Token: 0x0400003C RID: 60
		public List<RealmSkillRecord> skills;

		// Token: 0x0400003D RID: 61
		public List<RealmTrainingRecord> training;

		// Token: 0x0400003E RID: 62
		public List<RealmTrait> traits;

		// Token: 0x0400003F RID: 63
		public Gender gender;

		// Token: 0x04000040 RID: 64
		public float melanin;

		// Token: 0x04000041 RID: 65
		public List<RealmThing> equipments;

		// Token: 0x04000042 RID: 66
		public List<RealmThing> apparels;

		// Token: 0x04000043 RID: 67
		public List<RealmThing> inventory;

		// Token: 0x04000044 RID: 68
		public List<RealmHediff> hediffs;

		// Token: 0x04000045 RID: 69
		public byte healthState = 2;

		// Token: 0x04000046 RID: 70
		public Dictionary<string, int> workPriorities;
	}
}
