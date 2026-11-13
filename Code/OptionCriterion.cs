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
            return self != null && self.isNPC && AmIAnAIOnHead(self.AI);
        }
        public static bool AmIAnAIOnHead(SlugNPCAI self)
        {
            return self != null && self.behaviorType != null && self.behaviorType == SlugNPCAI.BehaviorType.OnHead;
        }
        public static bool DoIHaveASlugOnMyBack(Player self)
        {
            return self?.slugOnBack != null && self.slugOnBack.HasASlug;
        }
        public static bool DoIHaveASpearOnMyBack(Player self)
        {
            return self?.spearOnBack != null && self.spearOnBack.HasASpear;
        }
        public static bool DoIHaveASlugNPCInThisGrasp(Player player, int graspIndex)
        {
            return DoIHaveASlugInThisGrasp(player, graspIndex) && (player.grasps[graspIndex].grabbed as Player).isNPC;
        }
        public static bool DoIHaveASlugInThisGrasp(Player player, int graspIndex)
        {
            return DoIHaveSomethingInThisGrasp(player, graspIndex) && player.grasps[graspIndex].grabbed is Player;
        }
        public static bool DoIHaveSomethingInThisGrasp(Player player, int graspIndex)
        {
            return player.grasps[graspIndex] != null && player.grasps[graspIndex].grabbed != null;
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
        public static bool AmIOnTheRivulet(Player pup)
        {
            if (ModManager.MSC && pup?.onBack != null)
            {
                if (pup.onBack.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                    return true;
                else
                    return AmIOnTheRivulet(pup.onBack);
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
        public static bool MirosBirdProtectionOptionEligible(AbstractCreature creature)
        {
            Player player = creature?.realizedCreature as Player;
            return OptionsMenu.mirosBirdProtectionOption.GetValue() && player?.onBack != null;
        }
        public static bool RivDrownProtectionOptionEligible(Player player)
        {
            if (!player.submerged || !OptionsMenu.rivDrownProtectionOption.GetValue())
                return false;
            bool correctCampaign = player.isNPC && player.abstractCreature.world.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Rivulet;
            return  AmIOnTheRivulet(player) || correctCampaign;
        }
    }
}
