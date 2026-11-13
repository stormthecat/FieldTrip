using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Watcher;
using static FieldTrip.OptionCriterion;


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
            On.RainWorldGame.ExitToVoidSeaSlideShow += VoidEnd_onHook;
            On.SaveState.GrabSavedCreatures += GrabSavedCreatures_onHook;
            On.SaveState.GrabSavedObjects += GrabSavedObjects_onHook;
            On.Watcher.WarpPoint.NewWorldLoaded_Room += NewWorldLoaded_onHook;
            On.Watcher.WarpPoint.SpawnPendingObject += WatcherSpawnPendingObject_onHook;
        }
        public void BindWarpingILHooks()
        {
            IL.Watcher.WarpPoint.Update += WatcherWarpPointUpdate_ilHook;

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
                placeMyFriends(self.room, new Vector2(1530f, 155f));

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
                MoveToBitterStart(self.room);
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
        private void VoidEnd_onHook(On.RainWorldGame.orig_ExitToVoidSeaSlideShow orig, RainWorldGame self)
        {
            // TODO: double check this
            if (self.manager.upcomingProcess == null && self.IsStorySession)
            {
                if (self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak != null)
                    self.GetStorySession.saveState.pendingObjects = self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak;
                if (self.GetStorySession.saveState.getFieldtripSaveVals().pendingFriendsBak != null)
                    self.GetStorySession.saveState.pendingFriendCreatures = self.GetStorySession.saveState.getFieldtripSaveVals().pendingFriendsBak;
            }
            orig(self);
        }
        private List<string> GrabSavedCreatures_onHook(On.SaveState.orig_GrabSavedCreatures orig, SaveState self, AbstractCreature player, WorldCoordinate atPos)
        {
            self.getFieldtripSaveVals().pendingFriendsBak = new List<string>(self.pendingFriendCreatures);
            return orig(self, player, atPos);
        }
        private List<AbstractPhysicalObject> GrabSavedObjects_onHook(On.SaveState.orig_GrabSavedObjects orig, SaveState self, AbstractCreature player, WorldCoordinate atPos)
        {
            self.getFieldtripSaveVals().pendingObjectsBak = new List<string>(self.pendingObjects);
            return orig(self, player, atPos);
        }
        private void NewWorldLoaded_onHook(On.Watcher.WarpPoint.orig_NewWorldLoaded_Room orig, WarpPoint self, Room newRoom)
        {
            StoryGameSession session = self?.room?.game?.GetStorySession;
            if (session != null)
            {
                if (session.getFieldtripstoryGameSessionVals().pupStack != null)
                {
                    Debug.Log("SLUGPUP SAFARI: clear saved stack ");
                    session.getFieldtripstoryGameSessionVals().pupStack.Clear();
                }
                for (int i = 0; i < session.Players.Count; i++)
                {
                    Debug.Log("SLUGPUP SAFARI: saving stack of Player " + (i + 1) + "/" + session.Players.Count);
                    grabStackForWarp(self.room.game.Players[i].realizedCreature as Player);
                    SlugpupTumble(self.room.game.Players[i].realizedCreature as Player);
                }
            }
            orig(self, newRoom);
        }
        private bool WatcherSpawnPendingObject_onHook(On.Watcher.WarpPoint.orig_SpawnPendingObject orig, WarpPoint self, AbstractPhysicalObject nextObject, bool immediateSpawn)
        {
            Player player = nextObject.realizedObject as Player;
            bool output = orig(self, nextObject, immediateSpawn);
            if (player != null && output)
            {
                ConsumeStackForWarp(player); // yummers
            }
            return output;
        }
        #endregion
        #region IL Hooks
        private void WatcherWarpPointUpdate_ilHook(ILContext il)
        {
            // TODO: figure out why i put this here specifically
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.Before, i => i.MatchLdarg(0),
                    i => i.MatchLdfld<UpdatableAndDeletable>("room"),
                    i => i.MatchLdfld<Room>("game"),
                    i => i.Match(OpCodes.Callvirt));
                c.Index++;
                c.EmitDelegate((WarpPoint self) =>
                {
                    List<AbstractPhysicalObject> pending = self.room.game.GetStorySession.pendingWarpPointTransferObjects;
                    if (pending.Count == 0 || self.MyIdentifyingString() != self.room.game.GetStorySession.pendingWarpPointTransferId || !self.startedPendingSpawn)
                        return;

                    bool noScugs;
                    do
                    {
                        noScugs = true;
                        for (int i = 0; i < pending.Count; i++)
                        {
                            AbstractCreature creature = pending[i] as AbstractCreature;
                            if (creature != null && creature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                            {
                                if (self.SpawnPendingObject(creature, false))
                                {
                                    base.Logger.LogMessage("Spawning " + creature.ToString() + " early");
                                    noScugs = false;
                                    pending.RemoveAt(i);
                                    break;
                                }

                            }
                        }
                    }
                    while (!noScugs);
                });
                c.Emit(OpCodes.Ldarg_0);


            }
            catch (Exception e)
            {
                base.Logger.LogError("WatcherWarpPointUpdate_ilHook encountered an error: " + e);
                throw;
            }
        }
        #endregion
        #region Misc
        private void ConsumeStackForWarp(Player basePup)
        {
            StoryGameSession session = basePup.room.world.game.GetStorySession;
            if (basePup != null && session != null)
            {
                Debug.Log("SLUGPUP SAFARI: trying to stack -> " + basePup.ToString());
                List<Player> pupStack = session.getFieldtripstoryGameSessionVals().pupStack;
                if (DoIHaveASlugOnMyBack(basePup))
                    ConsumeStackForWarp(basePup.slugOnBack.slugcat);
                if (pupStack is null)
                    pupStack = new List<Player>();
                Debug.Log("\tpupStack queue length -> " + pupStack.Count());
                int index = pupStack.IndexOf(basePup);
                if (index <= 0)
                    Debug.Log("\tcannot stack -> " + basePup.ToString() + ", no base slug found (index:" + index + ")");
                if (index > 0 && pupStack[index - 1] != null && pupStack[index - 1].slugOnBack != null)
                {
                    Debug.Log("\tstacking -> " + basePup.ToString() + " onto " + pupStack[index - 1].ToString());
                    pupStack[index - 1].slugOnBack.SlugToBack(basePup);
                    pupStack.RemoveAt(index - 1);
                }
            }
        }
        void MoveToBitterStart(Room room)
        {
            //offset ensures coords are different
            int offset = 0;
            offset = moveFromHeart(room.world.GetAbstractRoom("MS_HEART").realizedRoom, offset);
            offset = moveFromHeart(room.world.GetAbstractRoom("MS_CORE").realizedRoom, offset);
        }
        #endregion
    }
}
