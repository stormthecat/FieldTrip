using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watcher;

namespace FieldTrip
{
    static public class OptionCriterion
    {
        // General
        public static bool AmIASlugpupOnHead(Player self)
        {
            return self != null && self.isNPC && self.AI != null && self.AI.behaviorType != null && self.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead;
        }
        public static bool AmIAnAIOnHead(SlugNPCAI self)
        {
            return self != null && self.behaviorType != null && self.behaviorType == SlugNPCAI.BehaviorType.OnHead;
        }
        public static bool AmIOnTheWatcher(Player pup)
        {
            if (ModManager.Watcher && pup?.onBack != null)
            {
                if (pup.onBack.SlugCatClass == WatcherEnums.SlugcatStatsName.Watcher)
                    return true;
                else
                    return AmIOnTheWatcher(pup.onBack);
            }
            return false;
        }

        // Specific Options
        public static bool ArtificerDontLaunchKidsEligible(Player player)
        {
            return OptionsMenu.artificerDontLaunchKidsOption.GetValue() && AmIASlugpupOnHead(player);
        }
        public static bool PupImpactCushionOptionEligible(Player player)
        {
            return OptionsMenu.pupImpactCushionOption.GetValue() && AmIASlugpupOnHead(player);
        }
        public static bool LeechProtectionOptionEligible(Player player)
        {
            return OptionsMenu.leechProtectionOption.GetValue() && AmIASlugpupOnHead(player);
        }
        public static bool CoalescipedeProtectionOptionEligible(Player player)
        {
            return OptionsMenu.coalescipedeProtectionOption.GetValue() && AmIASlugpupOnHead(player);
        }
    }
}
