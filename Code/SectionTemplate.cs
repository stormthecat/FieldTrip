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
        public void Bind_OnHooks()
        {

        }
        public void Bind_ILHooks()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Slugpup Safari: _ IL ran into an init error", ex));
                base.Logger.LogError(ex);
            }
        }
        #region On Hooks

        #endregion
        #region IL Hooks

        #endregion
        #region Misc

        #endregion
    }
}
