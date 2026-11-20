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
            IL.Explosion.Update += ExplosionUpdate_ilHook;
            IL.Leech.Swim += LeechSwim_ilHook;
            IL.Leech.Crawl += LeechCrawl_ilHook;
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
                c.EmitDelegate((bool val, UpdatableAndDeletable self, int m, int n) => {
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
                c.EmitDelegate((Creature creature) =>
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
        private void ExplosionUpdate_ilHook(ILContext il)
        {
            // treat scug as arti when under the correct conditions
            try
            {
                ILCursor c = new ILCursor(il);
                while (c.TryGotoNext(MoveType.After,
                    x => x.MatchIsinst("Player"),
                    x => x.MatchLdfld<Player>("SlugCatClass"),
                    x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Artificer")))
                {
                    Instruction nextInstruction = c.Next; // get instruction after ldsfld match
                    c.Index -= 3; // go to beginning of match

                    bool? equality = null;
                    if (nextInstruction.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality")))
                        equality = true;
                    else if (nextInstruction.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Inequality")))
                        equality = false;
                    if(equality != null)
                    {
                        Player player = null;
                        c.EmitDelegate((object obj) =>
                        {
                            player = (Player)obj;
                            return obj;
                        });
                        c.Index += 4; // go back to the end of match
                        c.EmitDelegate((bool var) =>
                        {
                            return (equality == ArtiBombProtectionOptionEligible(player)) || var;
                        });
                    }
                }
            }
            catch (Exception e)
            {
                base.Logger.LogError("ExplosionUpdate_ilHook encountered an error : " + e);
                throw;
            }
        }
        private void LeechSwim_ilHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.Before, i => i.MatchLdfld<Creature>("leechedOut"));
                Creature critter = null;
                c.EmitDelegate((Creature crit) => // grab creature for upcoming check
                {
                    critter = crit;
                    return crit;
                });
                c.Index++; // jump over leeched out so we can use its output
                c.EmitDelegate((bool leechedOut) =>
                {
                    return leechedOut || LeechProtectionOptionEligible(critter as Player);
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("LeechSwim_ilHook encountered an error: " + e);
                throw;
            }
        }
        private void LeechCrawl_ilHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchLdsfld<CreatureTemplate.Relationship.Type>("Eats"),
                    i => i.Match(OpCodes.Call));

                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 4);
                c.EmitDelegate((Leech leech, int index) =>  //get creature to check
                {
                    return leech.room.abstractRoom.creatures[index].realizedCreature;
                });
                c.EmitDelegate((bool val, Creature critter) =>  //prevent grabbing if conditions are met
                {
                    return val && (critter == null || !LeechProtectionOptionEligible(critter as Player));
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("LeechCrawl_ilHook encountered an error: " + e);
                throw;
            }
        }
        #endregion
        #region Misc

        #endregion
    }
}
