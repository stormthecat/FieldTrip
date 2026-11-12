using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldTrip
{
    public partial class FieldTripMain
    {
        public void BindProtectionsOnHooks()
        {
            On.MirosBirdAI.DoIWantToBiteCreature += mirosDoIBiteHook;
            On.Player.LungUpdate += lungUpdateHook;
            On.Watcher.Barnacle.Update += barnacleUpdateHook;

        }
        public void BindProtectionsILHooks()
        {
            IL.Player.TerrainImpact += playerTerrainImpactILHook;
            IL.PlayerGraphics.MSCUpdate += playerGraphicsMSCUpdateHook;
            IL.Leech.ConsiderOtherCreature += leechConsiderationHook;
            IL.Leech.Crawl += leechCrawlHook;
            IL.Spider.ConsiderPrey += spiderConsiderationHook;
            IL.Player.ClassMechanicsArtificer += ArtificerAbilityHook;
            IL.Explosion.Update += explosionUpdateILHook;
            IL.Player.Stun += playerStunILHook;
            IL.Creature.Update += creatureUpdateILHook;

        }
        #region On Hooks

        #endregion
        #region IL Hooks

        #endregion
        #region Misc

        #endregion
    }
}
