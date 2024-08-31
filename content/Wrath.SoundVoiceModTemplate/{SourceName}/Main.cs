﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Localization;
using Kingmaker.Sound;
using Kingmaker.Visual.Sound;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using UnityModManagerNet;

namespace {SourceName};

//-:cnd:noEmit
#if DEBUG
[EnableReloading]
#endif
//+:cnd:noEmit
public static class Main {
    internal static Harmony HarmonyInstance;
    internal static UnityModManager.ModEntry.ModLogger log;

    public static bool Load(UnityModManager.ModEntry modEntry) {
        log = modEntry.Logger;
//-:cnd:noEmit
#if DEBUG
        modEntry.OnUnload = OnUnload;
#endif
//+:cnd:noEmit
        modEntry.OnGUI = OnGUI;
        HarmonyInstance = new Harmony(modEntry.Info.Id);
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        return true;
    }

    public static void OnGUI(UnityModManager.ModEntry modEntry) {

    }

    [HarmonyPatch]
    public static class Soundbanks {
        public static readonly HashSet<uint> LoadedBankIds = [];

        [HarmonyPatch(typeof(AkAudioService), nameof(AkAudioService.Initialize))]
        [HarmonyPostfix]
        public static void LoadSoundbanks() {
            var banksPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try {
                log.Log($"Add soundbank base path {banksPath}");
                AkSoundEngine.AddBasePath(banksPath);

                foreach (var f in Directory.EnumerateFiles(banksPath, "*.bnk")) {
                    var bankName = Path.GetFileName(f);
                    var akResult = AkSoundEngine.LoadBank(bankName, out var bankId);

                    if (bankName == "Init.bnk")
                        throw new InvalidOperationException("Do not include Init.bnk");

                    if (akResult == AKRESULT.AK_BankAlreadyLoaded)
                        continue;

                    log.Log($"Loading soundbank {f}");

                    if (akResult == AKRESULT.AK_Success) {
                        LoadedBankIds.Add(bankId);
                    } else {
                        log.Error($"Loading soundbank {f} failed with result {akResult}");
                    }
                }
            } catch (Exception e) {
                log.LogException(e);
                UnloadSoundbanks();
            }
        }

        public static void UnloadSoundbanks() {
            foreach (var bankId in LoadedBankIds) {
                try {
                    AkSoundEngine.UnloadBank(bankId, IntPtr.Zero);
                    LoadedBankIds.Remove(bankId);
                } catch (Exception e) {
                    log.LogException(e);
                }
            }
        }

        [HarmonyPatch(typeof(BlueprintsCache), nameof(BlueprintsCache.Init))]
        [HarmonyPostfix]
        static void AddAsksListBlueprint()
        {
            LocalizationManager.CurrentPack.PutString("{SourceName}", "{Description}");

            var blueprint = new BlueprintUnitAsksList
            {
				// Every mod requires the creation of its own unique GUID.
				AssetGuid = new(System.Guid.Parse("00000000000000000000000000000000")),
                name = "{SourceName}_Barks",
                DisplayName = new() { m_Key = "{SourceName}" }
            };

            blueprint.ComponentsArray =
            [
                new UnitAsksComponent()
            {
                OwnerBlueprint = blueprint,

                // Since the blueprint is added manually by the mod, remove the usual reference
                // to the bank name to prevent a Wwise "already loaded" error.
                SoundBanks = [],
                PreviewSound = "{SourceName}_Test",
                Aggro = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CombatStart_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CombatStart_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CombatStart_03",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = true,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Pain = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Pain",
                            RandomWeight = 0.0f,
                            ExcludeTime = 0,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 2.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Fatigue = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Fatigue",
                            RandomWeight = 0.0f,
                            ExcludeTime = 0,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 60.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Death = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Death",
                            RandomWeight = 0.0f,
                            ExcludeTime = 0,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = true,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Unconscious = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Unconscious",
                            RandomWeight = 0.0f,
                            ExcludeTime = 0,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = true,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                LowHealth = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_LowHealth_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_LowHealth_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 10.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                CriticalHit = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CharCrit_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CharCrit_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CharCrit_03",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 0.7f,
                    ShowOnScreen = false
                },
                Order = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_AttackOrder_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 3,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_AttackOrder_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 3,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_AttackOrder_03",
                            RandomWeight = 0.0f,
                            ExcludeTime = 3,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_AttackOrder_04",
                            RandomWeight = 0.0f,
                            ExcludeTime = 3,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                OrderMove = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_03",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_04",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_05",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_06",
                            RandomWeight = 0.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Move_07",
                            RandomWeight = 0.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 10.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 0.1f,
                    ShowOnScreen = false
                },
                Selected = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_01",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_02",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_03",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_04",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_05",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Select_06",
                            RandomWeight = 1.0f,
                            ExcludeTime = 4,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_SelectJoke",
                            RandomWeight = 0.1f,
                            ExcludeTime = 30,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                RefuseEquip = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CantEquip_01",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CantEquip_02",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = true,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                RefuseCast = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CantCast",
                            RandomWeight = 0.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = true,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                CheckSuccess = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CheckSuccess_01",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CheckSuccess_02",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                CheckFail = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CheckFail_01",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_CheckFail_02",
                            RandomWeight = 1.0f,
                            ExcludeTime = 2,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                RefuseUnequip = new()
                {
                    Entries = [],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Discovery = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Discovery_01",
                            RandomWeight = 0.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        },
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_Discovery_02",
                            RandomWeight = 0.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                Stealth = new()
                {
                    Entries =
                    [
                        new()
                        {
                            Text = null,
                            AkEvent = "{SourceName}_StealthMode",
                            RandomWeight = 1.0f,
                            ExcludeTime = 1,
                            m_RequiredFlags = [],
                            m_ExcludedFlags = [],
                            m_RequiredEtudes = null,
                            m_ExcludedEtudes = null
                        }
                    ],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                StormRain = new()
                {
                    Entries = [],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                StormSnow = new()
                {
                    Entries = [],
                    Cooldown = 0.0f,
                    InterruptOthers = false,
                    DelayMin = 0.0f,
                    DelayMax = 0.0f,
                    Chance = 1.0f,
                    ShowOnScreen = false
                },
                AnimationBarks =
                [
                    new()
                    {
                        Entries =
                        [
                            new()
                            {
                                Text = null,
                                AkEvent = "{SourceName}_AttackShort",
                                RandomWeight = 0.0f,
                                ExcludeTime = 0,
                                m_RequiredFlags = [],
                                m_ExcludedFlags = [],
                                m_RequiredEtudes = null,
                                m_ExcludedEtudes = null
                            }
                        ],
                        Cooldown = 0.0f,
                        InterruptOthers = false,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 0.7f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.AttackShort
                    },
                    new()
                    {
                        Entries =
                        [
                            new()
                            {
                                Text = null,
                                AkEvent = "{SourceName}_CoupDeGrace",
                                RandomWeight = 0.0f,
                                ExcludeTime = 0,
                                m_RequiredFlags = [],
                                m_ExcludedFlags = [],
                                m_RequiredEtudes = null,
                                m_ExcludedEtudes = null
                            }
                        ],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CoupDeGrace
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.Cast
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CastDirect
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CastLong
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CastShort
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CastTouch
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.CastYourself
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.Omnicast
                    },
                    new()
                    {
                        Entries = [],
                        Cooldown = 0.0f,
                        InterruptOthers = true,
                        DelayMin = 0.0f,
                        DelayMax = 0.0f,
                        Chance = 1.0f,
                        ShowOnScreen = false,
                        AnimationEvent = MappedAnimationEventType.Precast
                    },
                ],
            },
            ];

                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(blueprint.AssetGuid, blueprint);

                BlueprintRoot.Instance.CharGen.m_MaleVoices = BlueprintRoot.Instance.CharGen.m_MaleVoices
                    .Append(blueprint.ToReference<BlueprintUnitAsksListReference>())
                    .ToArray();
        }
    }
//-:cnd:noEmit
#if DEBUG
    public static bool OnUnload(UnityModManager.ModEntry modEntry) {
        HarmonyInstance.UnpatchAll(modEntry.Info.Id);
        return true;
    }
#endif
//+:cnd:noEmit
}
