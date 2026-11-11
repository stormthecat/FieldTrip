using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FieldTrip
{
    public partial class FieldTripMain
    {
        public void BindWarpingOnHooks()
        {
            On.MoreSlugcats.MSCRoomSpecificScript.LC_FINAL.Update += ArtiEnding_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.SL_AI_Behavior.Update += RivEnding_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.SpearmasterEnding.Update += SpearEnding_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_bitterstart.Update += Bitterstart_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_CORESTARTUPHEART.Update += CoreStart_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_HEARTWARP.Update += HeartWarp_onHook;
            On.MoreSlugcats.MSCRoomSpecificScript.OE_CAVE03_warp.Update += OE_CAVE03_WarpUpdate_onHook;

            On.RainWorldGame.ExitToVoidSeaSlideShow += voidEndHook;
            On.SaveState.GrabSavedCreatures += grabSavedCreaturesHook;
            On.Watcher.WarpPoint.SpawnPendingObject += watcherSpawnPendingObjectHook;
            On.Watcher.WarpPoint.NewWorldLoaded_Room += newWorldLoadedHook;
            On.SaveState.GrabSavedObjects += grabSavedObjectsHook;
        }
        public void BindWarpingILHooks()
        {

        }

        #region On Hooks
        private void ArtiEnding_onHook(On.MoreSlugcats.MSCRoomSpecificScript.LC_FINAL.orig_Update orig, MSCRoomSpecificScript.LC_FINAL self, bool eu)
        {
            orig(self, eu);
            //TODO: CLEAN THIS UP
            if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding && !self.triggeredBoss && !self.endingTriggered && self.counter == 1)
                spawnMyFriends(self.room, new Vector2(2700f, 500f));

        }
        private void RivEnding_onHook(On.MoreSlugcats.MSCRoomSpecificScript.SL_AI_Behavior.orig_Update orig, MSCRoomSpecificScript.SL_AI_Behavior self, bool eu)
        {
            //TODO: CLEAN THIS UP
            AbstractCreature firstAlivePlayer = self.room.game.FirstAlivePlayer;
            Player player = (self.room.game.Players.Count > 0 && firstAlivePlayer != null) ? (firstAlivePlayer.realizedCreature as Player) : null;
            if (self.counter < 40 && self.fadeIn == null)
            {
                placeMyFriends(self.room, new Vector2(1530f, 155f));
            }
            orig(self, eu);
        }
        private void SpearEnding_onHook(On.MoreSlugcats.MSCRoomSpecificScript.SpearmasterEnding.orig_Update orig, MSCRoomSpecificScript.SpearmasterEnding self, bool eu)
        {
            //TODO: CLEAN THIS UP
            string oldroom = SaveState.forcedEndRoomToAllowwSave;
            SaveState.forcedEndRoomToAllowwSave = self.room.abstractRoom.name;
            orig(self, eu);
            spawnMyFriends(self.room, new Vector2(540f, 147f));
            SaveState.forcedEndRoomToAllowwSave = oldroom;
        }
        private void Bitterstart_onHook(On.MoreSlugcats.MSCRoomSpecificScript.MS_bitterstart.orig_Update orig, MSCRoomSpecificScript.MS_bitterstart self, bool eu)
        {
            //TODO: CLEAN THIS UP
            Vector2 coords = new Vector2(35, 15);
            Vector2 coordsCreature = new Vector2(700, 300);

            IntVector2 intCoords = new IntVector2(35, 15);
            AbstractCreature firstAlivePlayer = self.room.game.FirstAlivePlayer;
            Player player = (self.room.game.Players.Count > 0 && firstAlivePlayer != null) ? (firstAlivePlayer.realizedCreature as Player) : null;
            if (player != null && self.waitCounter > 0 && !player.inShortcut && player.firstChunk.pos.x <= 700f && self.room.game.cameras[0].currentCameraPosition == 0 && self.fadeIn == null)
            {
                self.room.game.SpawnCritters(self.room.game.GetStorySession.saveState.GrabSavedCreatures(firstAlivePlayer, self.room.ToWorldCoordinate(intCoords)), firstAlivePlayer);
                self.room.game.SpawnObjs(self.room.game.GetStorySession.saveState.GrabSavedObjects(firstAlivePlayer, self.room.ToWorldCoordinate(intCoords)));
                placeMyFriends(self.room, coordsCreature);

            }
            orig(self, eu);
        }
        private void CoreStart_onHook(On.MoreSlugcats.MSCRoomSpecificScript.MS_CORESTARTUPHEART.orig_Update orig, MSCRoomSpecificScript.MS_CORESTARTUPHEART self, bool eu)
        {
            //TODO: CLEAN THIS UP
            SaveState save = self.room.game.GetStorySession.saveState;
            Room room = self.room;
            if (self.primed && self.foundCell != null && self.foundCell.room == room && self.foundCell.usingTime > 0f && !save.miscWorldSaveData.moonHeartRestored)
            {
                orig(self, eu);
                save.BringUpToDate(room.game);
                AddItemsToPending(room.abstractRoom, save, false);
                AddCrittersToPending(room.abstractRoom, save, false);
                AddItemsToPending(room.world.GetAbstractRoom("MS_HEART"), save, false);
                AddCrittersToPending(room.world.GetAbstractRoom("MS_HEART"), save, false);
                save.denPosition = "MS_bitterstart";
                save.progression.SaveWorldStateAndProgression(false);
                save.pendingFriendCreatures.Clear();
                save.pendingObjects.Clear();
            }
            else
                orig(self, eu);
        }
        private void HeartWarp_onHook(On.MoreSlugcats.MSCRoomSpecificScript.MS_HEARTWARP.orig_Update orig, MSCRoomSpecificScript.MS_HEARTWARP self, bool eu)
        {
            //TODO: CLEAN THIS UP
            if (self.fadeOut != null && self.fadeOut.IsDoneFading() && !self.triggered && self.afterFadeTime > 119f)
                moveToBitterStart(self.room);
            orig(self, eu);
        }
        private void OE_CAVE03_WarpUpdate_onHook(On.MoreSlugcats.MSCRoomSpecificScript.OE_CAVE03_warp.orig_Update orig, MSCRoomSpecificScript.OE_CAVE03_warp self, bool eu)
        {
            //TODO: CLEAN THIS UP
            // Remove pups before warp, otherwise suffer catastrophic consequences (game no worky)
            if (self.fadeOut != null && self.fadeOut.IsDoneFading() && !self.triggered) // only do it once or game explode (lag)
            {
                for (int l = 0; l < self.room.game.Players.Count; l++)
                {
                    if (self.room.game.Players[l].realizedCreature != null)
                    {
                        SlugpupTumble(self.room.game.Players[l].realizedCreature as Player);
                    }
                }
            }
            orig(self, eu);
        }
        #endregion
        #region IL Hooks

        #endregion
        #region Misc

        #endregion
    }
}
