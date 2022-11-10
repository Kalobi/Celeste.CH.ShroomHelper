﻿using Celeste.Mod.ShroomHelper.Effects;
using Celeste.Mod.ShroomHelper.Entities;
using Monocle;
using MonoMod.Utils;
using System;

namespace Celeste.Mod.ShroomHelper {
    public class ShroomHelperModule : EverestModule {
        public static readonly string GoldenBerryRestartField = "ShroomHelper_GoldenBerryRestart";
        public static ShroomHelperModule Instance;

        public ShroomHelperModule() {
            Instance = this;
        }

        public override Type SessionType => typeof(ShroomHelperSession);
        public static ShroomHelperSession Session => (ShroomHelperSession)Instance._Session;

        public override void Load() {
            DoubleRefillBooster.Load();
            OneDashWingedStrawberry.Load();
            ShroomDashSwitch.Load();
            Everest.Events.Level.OnLoadBackdrop += Level_OnLoadBackdrop;
            On.Celeste.Session.Restart += Session_Restart;
        }

        public override void Unload() {
            DoubleRefillBooster.Unload();
            OneDashWingedStrawberry.Unload();
            ShroomDashSwitch.Unload();
            Everest.Events.Level.OnLoadBackdrop -= Level_OnLoadBackdrop;
            On.Celeste.Session.Restart -= Session_Restart;
        }

        private Backdrop Level_OnLoadBackdrop(MapData map, BinaryPacker.Element child, BinaryPacker.Element above) {
            if (child.Name.Equals("ShroomHelper/ShroomPetals", StringComparison.OrdinalIgnoreCase)) {
                ShroomPetals shroomPetals = new();
                if (child.HasAttr("color")) {
                    shroomPetals.shroomColor = child.Attr("color");
                }

                return shroomPetals;
            }

            return null;
        }

        private Session Session_Restart(On.Celeste.Session.orig_Restart orig, Session self, string intoLevel) {
            Session restartSession = orig(self, intoLevel);
            if (Engine.Scene is LevelExit exit && DynamicData.For(exit).Get<LevelExit.Mode>("mode") == LevelExit.Mode.GoldenBerryRestart) {
                DynamicData.For(restartSession).Set(GoldenBerryRestartField, true);
            }

            return restartSession;
        }
    }
}
