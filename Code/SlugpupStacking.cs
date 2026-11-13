using MonoMod.Cil;
using RWCustom;
using System;
using System.Collections.Generic;
using UnityEngine;
using Watcher;
using static FieldTrip.OptionCriterion;
using Mono.Cecil.Cil;



namespace FieldTrip
{
    public partial class FieldTripMain
    {
        public void BindStackingOnHooks()
        {
            On.Player.Grabbed += Grabbed_onHook;
            On.Player.SlugOnBack.DropSlug += PlayerDropSlug_onHook;
            On.PlayerGraphics.DrawSprites += PlayerGraphicsDrawSprite_onHook;
            On.Player.SlugOnBack.SlugToBack += SlugToBack_onHook;
        }
        public void BindStackingILHooks()
        {
            try
            {
                IL.Player.SlugOnBack.ChangeOverlap += ChangeOverlap_ilHook;

            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Slugpup Safari: SlugpupStacking IL ran into an init error", ex));
                base.Logger.LogError(ex);
            }
        }

        #region On Hooks
        private void Grabbed_onHook(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            //slugpups tumble down if a lower slug is grabbed by an enemy
            if(self.dangerGrasp != null && DoIHaveASlugOnMyBack(self))
                SlugpupTumble(self.slugOnBack.slugcat);
        }
        private void PlayerDropSlug_onHook(On.Player.SlugOnBack.orig_DropSlug orig, Player.SlugOnBack self)
        {
            // Toggle camo on drop
            if (self?.slugcat != null && self.slugcat.isCamo)
                self.slugcat.ToggleCamo();
            orig(self);
        }
        private void PlayerGraphicsDrawSprite_onHook(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            bool amIOnTheWatcher = AmIOnTheWatcher(self.player);
            // handle watcher camo on piggybacked scugs
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if ((i < 9 || i > 11)) //skip these sprites
                {
                    if (amIOnTheWatcher)
                    {
                        sLeaser.sprites[i].shader = Custom.rainWorld.Shaders["PlayerSimpleCamo"];
                        self.useSimpleCamo = true;
                    }
                    else if (self.player.SlugCatClass != WatcherEnums.SlugcatStatsName.Watcher && self.useSimpleCamo)
                    {
                        //emulate watcher progress decay
                        if (self.player.isCamo && self.player.camoProgress < 1f)
                            self.player.camoProgress += 0.01f;
                        else if (!self.player.isCamo && self.player.camoProgress > 0f)
                            self.player.camoProgress -= 0.01f;
                    }
                    if (self.useSimpleCamo)
                        sLeaser.sprites[i].alpha = 1f - self.player.camoProgress;
                }
            }
        }
        private void SlugToBack_onHook(On.Player.SlugOnBack.orig_SlugToBack orig, Player.SlugOnBack self, Player playerToBack)
        {
            // early return if stacking is disabled
            if (OptionsMenu.maxStackOption.GetValue() <= 0)
                return;
            orig(self, playerToBack);
        }
        #endregion
        #region IL Hooks
        private void ChangeOverlap_ilHook(ILContext il)
        {
            try
            {
                // inject an early return if newOverlap is false
                // this gives us free reign to mess with the overlap if it isn't being set

                // if the overlap is updated the regular code will run and put it in
                // midground as normal
                ILCursor c = new ILCursor(il);
                var label = il.DefineLabel();
                c.GotoNext(MoveType.After, i => i.MatchRet(),
                    i => i.MatchLdcI4(0));
                c.Emit(OpCodes.Pop); // remove very beginning of following loop
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Brtrue, label);
                c.Emit(OpCodes.Ret);
                c.MarkLabel(label);
                c.Emit(OpCodes.Ldc_I4_0); // put beginning of loop back where we found it
            }
            catch (Exception e)
            {
                base.Logger.LogError("ChangeOverlap_ilHook encountered an error: " + e);
                throw;
            }
        }
        #endregion
        #region Misc
        void SlugpupTumble(Player basePup)
        {
            //recursivly destroy the tower above this slug
            if (DoIHaveASlugOnMyBack(basePup))
            {
                SlugpupTumble(basePup.slugOnBack.slugcat);
                basePup.slugOnBack.DropSlug();
            }
        }
        void AdjustTower(Player player)
        {
            //trying to simulate a shortcut
            List<AbstractPhysicalObject> allConnectedObjects = player.abstractCreature.GetAllConnectedObjects();
            Room room = player.room;

            //remove all scugs and delete them from the update list
            for (int i = 0; i < allConnectedObjects.Count; i++)
            {
                PhysicalObject currObj = allConnectedObjects[i].realizedObject;
                if (currObj != null && currObj is Player)
                {
                    //Debug.Log("SLUGPUP SAFARI: Removing " + allConnectedObjects[i].realizedObject.ToString());
                    room.RemoveObject(currObj);
                    List<RoomCamera.SpriteLeaser> spriteLeasers = room.game.cameras[0].spriteLeasers;
                    for (int j = 0; j < spriteLeasers.Count; j++)
                    {
                        if (spriteLeasers[j].drawableObject.Equals(currObj.graphicsModule))
                        {
                            spriteLeasers[j].RemoveAllSpritesFromContainer();
                            spriteLeasers.RemoveAt(j);
                        }
                    }
                    room.CleanOutObjectNotInThisRoom(currObj);
                }
            }

            //add them in order
            for (int i = 0; i < allConnectedObjects.Count; i++)
            {
                PhysicalObject currObj = allConnectedObjects[i].realizedObject;
                if (currObj != null && currObj is Player)
                    room.AddObject(currObj);
            }
        }
        #endregion
    }
}
