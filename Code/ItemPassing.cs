using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static FieldTrip.OptionCriterion;


namespace FieldTrip
{
    public partial class FieldTripMain
    {
        public void BindItemPassingOnHooks()
        {
            On.Player.CanIPickThisUp += canIPickThisUpdate;
            On.MoreSlugcats.SlugNPCAI.PassingGrab += passingGrabHook;
        }
        public void BindItemPassingILHooks()
        {
            try
            {
                IL.PlayerGraphics.MSCUpdate += playerGraphicsMSCUpdateHook;
                IL.Player.GrabUpdate += grabUpdateILHook;

            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Slugpup Safari: ItemPassing IL ran into an init error", ex));
                base.Logger.LogError(ex);
            }
        }
        #region On Hooks
        private bool canIPickThisUpdate(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            bool defVal = orig(self, obj);
            if (obj != null && OptionsMenu.keepItemEnabledOption.GetValue() && obj.grabbedBy.Any((Creature.Grasp x) => (x != null && x.grabber != null && x.grabber is Player && (x.grabber as Player).isNPC)))
                return false;
            else if (obj != null && AmIASlugpupOnHead(self) && (obj.grabbedBy.Any((Creature.Grasp x) => (x != null && x.grabber != null && x.grabber is Player))))
                return false;
            else
                return defVal;
        }
        #endregion
        #region IL Hooks

        #endregion
        #region Misc

        #endregion
    }
}
