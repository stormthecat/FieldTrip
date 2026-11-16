using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watcher;
using static FieldTrip.OptionCriterion;


namespace FieldTrip
{
    public partial class FieldTripMain
    {
        public void BindProtectionsOnHooks()
        {
            On.Watcher.Barnacle.Update += BarnacleUpdate_onHook;
            On.Player.LungUpdate += LungUpdate_onHook;
            On.MirosBirdAI.DoIWantToBiteCreature += MirosDoIBite_onHook;

        }
        public void BindProtectionsILHooks()
        {
            IL.Player.ClassMechanicsArtificer += ArtificerAbility_ilHook;
            IL.Creature.Update += CreatureUpdate_ilHook;
            IL.Explosion.Update += explosionUpdateILHook;
            IL.Leech.ConsiderOtherCreature += leechConsiderationHook;
            IL.Leech.Crawl += leechCrawlHook;
            IL.PlayerGraphics.MSCUpdate += playerGraphicsMSCUpdateHook;
            IL.Player.Stun += playerStunILHook;
            IL.Player.TerrainImpact += playerTerrainImpactILHook;
            IL.Spider.ConsiderPrey += spiderConsiderationHook;

        }
        #region On Hooks
        private void BarnacleUpdate_onHook(On.Watcher.Barnacle.orig_Update orig, Barnacle self, bool eu)
        {
            bool collide = true;
            List<Creature.Grasp> grabbingMe = self.grabbedBy;
            if (grabbingMe.Count > 0)
            {
                for (int i = 0; i < grabbingMe.Count; i++)
                {
                    Player player = grabbingMe[i]?.grabber as Player;
                    if (player != null && player.onBack != null)
                        collide = false;
                }
            }
            self.CollideWithObjects = collide;
            orig(self, eu);
        }
        private void LungUpdate_onHook(On.Player.orig_LungUpdate orig, Player self)
        {
            float air = self.airInLungs;
            bool lungsExhausted = self.lungsExhausted;
            float drown = self.drown;

            orig(self);

            if (RivDrownProtectionOptionEligible(self))
            {
                // put these back
                self.airInLungs = air;
                self.lungsExhausted = lungsExhausted;
                self.drown = drown;

                // let them breathe
                self.airInLungs += 1f / (float)(self.lungsExhausted ? 240 : 60);
                if (self.airInLungs >= 1f)
                {
                    self.airInLungs = 1f;
                    self.lungsExhausted = false;
                    self.drown = 0f;
                }
            }
        }
        private bool MirosDoIBite_onHook(On.MirosBirdAI.orig_DoIWantToBiteCreature orig, MirosBirdAI self, AbstractCreature creature)
        {
            return orig(self, creature) && !MirosBirdProtectionOptionEligible(creature);
        }
        #endregion
        #region IL Hooks
        private void ArtificerAbility_ilHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                //prevent arti from launching piggybacked slugs
                c.GotoNext(MoveType.After, i => i.MatchStloc(18)); // modifies flag4
                c.Emit(OpCodes.Ldloc, 18); // load flag4 again
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 16);
                c.Emit(OpCodes.Ldloc, 17);
                c.EmitDelegate<Func<bool, UpdatableAndDeletable, int, int, bool>>((val, self, m, n) => {
                    Player player = self.room.physicalObjects[m][n] as Player;
                    return val && !ArtificerDontLaunchKidsEligible(player);
                });
                c.Emit(OpCodes.Stloc, 18); // set the new flag4
            }
            catch (Exception e)
            {
                base.Logger.LogError("ArtificerAbility_ilHook encountered an error: " + e);
                throw;
            }
        }
        private void CreatureUpdate_ilHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchLdstr("{0} Fell out of room!"));
                c.GotoNext(MoveType.Before, i => i.MatchLdsfld<ModManager>("CoopAvailable")); // skip to beginning of code that kills you

                ILCursor placeholder = new ILCursor(c); // label after the code that kills you
                placeholder.GotoNext(MoveType.After, i => i.MatchCallvirt<AbstractWorldEntity>("Destroy"));
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);

                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Creature, bool>>((creature) =>
                {
                    return KillzoneProtectionOptionEligible(creature);
                });
                c.Emit(OpCodes.Brtrue, label); // skip the code that kills you if option is enabled
            }
            catch (Exception e)
            {
                base.Logger.LogError("CreatureUpdate_ilHook encountered an error : " + e);
                throw;
            }
        }
        private void explosionUpdateILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);

                while (c.TryGotoNext(MoveType.Before,
                    x => x.MatchIsinst("Player"),
                    x => x.MatchLdfld<Player>("SlugCatClass"),
                    x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Artificer"),
                    x => x.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality"))))
                {
                    //base.Logger.LogMessage("explosionUpdateILHook found an arti check");
                    Player player = null;
                    c.EmitDelegate<Func<object, object>>((obj) =>
                    {
                        player = (Player)obj;
                        return obj;
                    });
                    c.Index += 4;
                    c.EmitDelegate<Func<bool, bool>>((var) =>
                    {

                        if (OptionsMenu.artiBombProtectionOption.GetValue() && player?.AI != null && player.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                            return true;
                        return var;
                    });
                }
                c = new ILCursor(il);
                while (c.TryGotoNext(MoveType.Before,
                    x => x.MatchIsinst("Player"),
                    x => x.MatchLdfld<Player>("SlugCatClass"),
                    x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Artificer"),
                    x => x.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Inequality"))))
                {
                    //base.Logger.LogMessage("explosionUpdateILHook found an arti check");

                    Player player = null;
                    c.EmitDelegate<Func<object, object>>((obj) =>
                    {
                        player = (Player)obj;
                        return obj;
                    });
                    c.Index += 4;
                    c.EmitDelegate<Func<bool, bool>>((var) =>
                    {
                        if (OptionsMenu.artiBombProtectionOption.GetValue() && player?.AI != null && player.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                            return false;
                        return var;
                    });
                }
            }
            catch (Exception e)
            {
                base.Logger.LogError("explosionUpdateILHook encountered an error : " + e);
                throw;
            }
        }
        #endregion
        #region Misc

        #endregion
    }
}
