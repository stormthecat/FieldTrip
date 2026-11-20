using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IL.PlayerGraphics.MSCUpdate += playerGraphicsMSCUpdateHook;
            IL.Player.GrabUpdate += grabUpdateILHook;

        }
        #region On Hooks

        #endregion
        #region IL Hooks

        #endregion
        #region Misc

        #endregion
    }
}
