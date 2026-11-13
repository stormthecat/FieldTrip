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
            IL.Player.ClassMechanicsArtificer += ArtificerAbilityHook;
            IL.Creature.Update += creatureUpdateILHook;
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
        private void ArtificerAbilityHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchLdfld<Options>("friendlyFire"));
                c.GotoNext(i => i.MatchCallvirt<Player>("get_isNPC"));
                c.Remove();
                //prevent arti from launching pups with coop on
                c.EmitDelegate<Func<Player, bool>>((slugcat) =>
                {
                    if (ArtificerDontLaunchKidsEligible(slugcat))
                    {
                        return false;
                    }
                    return true;
                });
                c.GotoNext(i => i.MatchLdcI4(1));
                c.GotoNext(i => i.MatchLdcI4(1));
                c.Index++;
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 16);
                c.Emit(OpCodes.Ldloc, 17);
                //prevent arti from launching pups when coop is off
                c.EmitDelegate<Func<UpdatableAndDeletable, int, int, bool>>((self, m, n) => {
                    Player player = self.room.physicalObjects[m][n] as Player;
                    if (player == null || !player.isNPC || player.AI == null || player.AI.behaviorType != SlugNPCAI.BehaviorType.OnHead || !OptionsMenu.artificerDontLaunchKidsOption.GetValue())
                    {
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("ArtificerAbilityHook encountered an error: " + e);
                throw;
            }
        }
        #endregion
        #region Misc

        #endregion
    }
}
