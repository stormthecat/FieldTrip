using BepInEx;
using DevInterface;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Management.Instrumentation;
using System.Security;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;


/*
 * !!!TODO LIST!!!
 * 
 * 
 *  0 = not done
 *  / = partially done
 *  X = done
 *  ? = on hold
 * 
 * >>>>> 1.5 <<<<<
 * [X]  FIX OVERLAP
 *          - spears are still broken :(
 *          - top spear is fine, lower spears are less fine
 *          - pipe breaks tower AAAAAAAAA
 * [X]  FIX RIV WARP
 *          - Warp creatures and items in MS_HEART as well
 * [X]  FIX ENDING PUP NOT SPAWNING
 * [X]  RIV CAMPAIGN EXTENDED BREATH TOGGLE
 * [X}  ADD FUNCTION TO HANDLE SETING OBJ POSITIONS + FEEDING PUPS
 * [X]  CHANGE VERSION NUMBER
 * [X]  UPDATE DESC
 * [X]  UNBREAK OVERSEERS
 * 
 * >>>>> 1.6.0 <<<<<
 * [X]  CHANGE VERSION AND WRITE PATCH NOTES
 * [X]  INVESTIGATE JUNGLE LEACHES
 * [X]  FIX GOURM ENDING STARVING PUPS (couldnt replicate)
 * [X]  EXCEPTION TO KILLZONE FOR STACKED PUPS
 * [X]  FIX VERSIONING AND TEST UPDATE
 * [X]  ARTI BOMB RESISTANCE
 * [X]  DELETION MEMLEAK FIX (AI TRACKERS? -- NOPE)
 * [X]  WIPE SLEASERS
 * [X]  RIV WARP RETURNS
 *          - Stop pups from spawning in ceiling
 * [X]  MIROS PROTECTION
 * [X]  STACK STUN PROTECTION
 * [X]  CAMPAIGN SPECIFIC OPTIONS
 * [X]  CONFIGURABLE STACK SIZE
 * [X]  FIX AUTO PALETTE
 * 
 * 
 * >>>>> NEED MORE INFO/CONSIDERATION <<<<<
 * [?]  FIX MAST ENDING
 * [?]  DMS TAIL FIX
 * [?]  PUPS STOP EATING LIVE FOOD
 * [?]  THE REFACTORING.

//if ((critter as Player)?.AI?.behaviorType == SlugNPCAI.BehaviorType.OnHead) 

*/

namespace FieldTrip
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    //[BepInPlugin("yeliah.slugpupFieldtrip", "Slugpup Field Trip", "1.0.0")]


    public class FieldTripMain : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "yeliah.slugpupFieldtrip";
        public const string PLUGIN_NAME = "Slugpup Safari";
        public const string PLUGIN_VERSION = "1.6.1";

        private OptionsMenu optionsMenuInstance;
        private bool initialized;
        private int layer = 0;

        public object Playergraphics { get; private set; }

        private void OnEnable()
        {
            On.RainWorld.OnModsInit += this.RainWorld_OnModsInit;
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (this.initialized)
            {
                return;
            }
            this.initialized = true;
            On.Player.SlugOnBack.Update += slugOnBackUpdateHook;
            On.Player.GrabUpdate += grabUpdateHook;
            On.Player.Grabbed += grabbedHook;
            On.Player.SlugOnBack.SlugToHand += slugToHandHook;
            this.optionsMenuInstance = new OptionsMenu(this);
            try
            {
                global::MachineConnector.SetRegisteredOI("yeliah.slugpupFieldtrip", optionsMenuInstance);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Slugpup Safari: OnModsInit options failed init error {0}{1}", this.optionsMenuInstance, ex));
                base.Logger.LogError(ex);
            }
            On.MoreSlugcats.MSCRoomSpecificScript.SL_AI_Behavior.Update += rivEndingHook;
            On.MoreSlugcats.MSCRoomSpecificScript.SpearmasterEnding.Update += spearEndingHook;
            On.MoreSlugcats.MSCRoomSpecificScript.LC_FINAL.Update += artiEndingHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_bitterstart.Update += bitterHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_CORESTARTUPHEART.Update += coreStartHook;
            On.MoreSlugcats.MSCRoomSpecificScript.MS_HEARTWARP.Update += heartWarp;
            On.RainWorldGame.ExitToVoidSeaSlideShow += voidEndHook;
            //On.SaveState.SpawnSavedObjectsAndCreatures += spawnSavedStuffHook;
            On.SaveState.GrabSavedCreatures += grabSavedCreaturesHook;
            On.SaveState.GrabSavedObjects += grabSavedObjectsHook;
            On.MoreSlugcats.SlugNPCAI.Update += SlugNPCUpdateHook_On;
            On.Player.LungUpdate += lungUpdateHook;
            On.Player.CanIPickThisUp += canIPickThisUpdate;
            On.MoreSlugcats.SlugNPCAI.PassingGrab += passingGrabHook;
            On.Player.SlugOnBack.SlugToBack += slugToBackHook;
            On.MirosBirdAI.DoIWantToBiteCreature += mirosDoIBiteHook;
            //IL.MoreSlugcats.SlugNPCAI.PassingGrab += passingGrabILHook;
            try
            {
                IL.MoreSlugcats.SlugNPCAI.Update += SlugNPCUpdateHook;
                IL.Player.ClassMechanicsArtificer += ArtificerAbilityHook;
                IL.Player.GrabUpdate += grabUpdateILHook;
                IL.MoreSlugcats.SlugNPCAI.Move += slugNPCMoveHook;
                IL.Player.SlugOnBack.Update += slugOnBackUpdateILHook;
                IL.Player.TerrainImpact += playerTerrainImpactILHook;
                IL.PlayerGraphics.MSCUpdate += playerGraphicsMSCUpdateHook;
                IL.Leech.ConsiderOtherCreature += leechConsiderationHook;
                IL.Leech.Crawl += leechCrawlHook;
                IL.Spider.ConsiderPrey += spiderConsiderationHook;
                IL.PlayerGraphics.DrawSprites += worldsFunniestILHook;
                IL.Explosion.Update += explosionUpdateILHook;
                IL.Player.Stun += playerStunILHook;
                IL.Player.SlugOnBack.GraphicsModuleUpdated += graphicsModuleUpdatedILHook;
                IL.Creature.Update += creatureUpdateILHook;
                IL.PlayerGraphics.ctor += playerGraphicsCtorILHook;

            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Slugpup Safari: OnModsInit IL failed init error", ex));
                base.Logger.LogError(ex);
            }
        }

        private void playerGraphicsCtorILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<PlayerGraphics>("player"),
                    x => x.MatchCallvirt<Player>("get_playerState"),
                    x => x.MatchLdfld<PlayerState>("playerNumber"),
                    x => x.Match(OpCodes.Brtrue_S));
                c.Index--;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, PlayerGraphics, bool>>((val, graphicsModule) =>
                {
                    Player player = graphicsModule?.player;
                    if (player != null && player.isNPC)
                        return true;
                    return val != 0;
                });

            }
            catch (Exception e)
            {
                base.Logger.LogError("playerGraphicsCtorILHook encountered an error : " + e);
                throw;
            }
        }

        
        private void creatureUpdateILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchLdstr("{0} Fell out of room!"));
                c.GotoNext(MoveType.Before, i => i.MatchLdsfld<ModManager>("CoopAvailable"));
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(MoveType.After, i => i.MatchCallvirt<AbstractWorldEntity>("Destroy"));
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Creature, bool>>((creature) =>
                {
                    Player player = creature as Player;
                    if (player == null)
                        return false;
                    //base.Logger.LogMessage("Prevented killzone death");
                    return (OptionsMenu.killzoneProtection.Value && player.onBack != null);
                });
                c.Emit(OpCodes.Brtrue, label);


            }
            catch (Exception e)
            {
                base.Logger.LogError("creatureUpdateILHook encountered an error : " + e);
                throw;
            }
        }

        private bool mirosDoIBiteHook(On.MirosBirdAI.orig_DoIWantToBiteCreature orig, MirosBirdAI self, AbstractCreature creature)
        {
            if ((creature?.realizedCreature as Player) != null && (creature.realizedCreature as Player).onBack != null)
                return false;
            return orig(self, creature);
        }

        private void slugToBackHook(On.Player.SlugOnBack.orig_SlugToBack orig, Player.SlugOnBack self, Player playerToBack)
        {
            if (OptionsMenu.maxStack.Value <= 0)
                return;
            orig(self, playerToBack);
        }

        private void graphicsModuleUpdatedILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchCallvirt<Creature>("get_Consious"));
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, Player.SlugOnBack, bool>>((val, slugOnBack) =>
                {
                    if (!val && !slugOnBack.slugcat.dead && OptionsMenu.stunProtection.Value)
                        return true;
                    return val;
                });

            }
            catch (Exception e)
            {
                base.Logger.LogError("graphicsModuleUpdatedILHook encountered an error : " + e);
                throw;
            }
        }

        private void playerStunILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,i => i.MatchLdfld<Player.SlugOnBack>("slugcat"));
                c.EmitDelegate<Func<object, object>>((obj) =>
                {
                    if (OptionsMenu.stunProtection.Value)
                        return null;
                    return obj;
                });

            }
            catch (Exception e)
            {
                base.Logger.LogError("playerStunILHook encountered an error : " + e);
                throw;
            }
        }

        private void explosionUpdateILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);

                while (c.TryGotoNext(MoveType.Before,
                    x => x.MatchIsinst("Player"),
                    x => x.MatchLdfld<Player>("SlugCatClass"),
                    x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Artificer"),
                    x => x.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality"))))
                {
                    //base.Logger.LogMessage("explosionUpdateILHook found an arti check");
                    Player player = null;
                    c.EmitDelegate<Func<object, object>>((obj) =>
                    { 
                        player = (Player)obj;
                        return obj;
                    });
                    c.Index += 4;
                    c.EmitDelegate<Func<bool, bool>>((var) =>
                    {
                        
                        if (OptionsMenu.artiBombProtection.Value && player?.AI != null && player.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                            return true;
                        return var;
                    });
                }
                c = new ILCursor(il);
                while (c.TryGotoNext(MoveType.Before,
                    x => x.MatchIsinst("Player"),
                    x => x.MatchLdfld<Player>("SlugCatClass"),
                    x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Artificer"),
                    x => x.MatchCall(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Inequality"))))
                {
                    //base.Logger.LogMessage("explosionUpdateILHook found an arti check");

                    Player player = null;
                    c.EmitDelegate<Func<object, object>>((obj) =>
                    {
                        player = (Player)obj;
                        return obj;
                    });
                    c.Index += 4;
                    c.EmitDelegate<Func<bool, bool>>((var) =>
                    {
                        if (OptionsMenu.artiBombProtection.Value && player?.AI != null && player.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                            return false;
                        return var;
                    });
                }
            }
            catch (Exception e)
            {
                base.Logger.LogError("explosionUpdateILHook encountered an error : " + e);
                throw;
            }
        }

        private void worldsFunniestILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[3];
                array[0] = ((Instruction i) => i.MatchMul());
                array[1] = ((Instruction i) => i.MatchAdd());
                array[2] = ((Instruction i) => i.MatchMul());
                c.GotoNext(MoveType.After, array);
                c.EmitDelegate<Func<float, float>>((val) =>
                {
                    if (OptionsMenu.bigHeadMode.Value)
                        return val * 2;
                    else
                        return val;
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("fun cancelled : " + e);
                throw;
            }
        }

        private List<AbstractPhysicalObject> grabSavedObjectsHook(On.SaveState.orig_GrabSavedObjects orig, SaveState self, AbstractCreature player, WorldCoordinate atPos)
        {
            self.getFieldtripSaveVals().pendingObjectsBak = new List<string>(self.pendingObjects);
            return orig(self, player, atPos);
        }

        private List<string> grabSavedCreaturesHook(On.SaveState.orig_GrabSavedCreatures orig, SaveState self, AbstractCreature player, WorldCoordinate atPos)
        {
            self.getFieldtripSaveVals().pendingFriendsBak = new List<string>(self.pendingFriendCreatures);
            return orig(self, player, atPos);
        }

        private void lungUpdateHook(On.Player.orig_LungUpdate orig, Player self)
        {
            float air = self.airInLungs;
            bool lungsExhausted = self.lungsExhausted;
            float drown = self.drown;
            orig(self);
            if (self.submerged && self.isNPC &&
                OptionsMenu.rivDrownProtection.Value && self.abstractCreature.world.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
            {
                self.airInLungs = air;
                self.lungsExhausted = lungsExhausted;
                self.drown = drown;
                self.airInLungs += 1f / (float)(self.lungsExhausted ? 240 : 60);
                if (self.airInLungs >= 1f)
                {
                    self.airInLungs = 1f;
                    self.lungsExhausted = false;
                    self.drown = 0f;
                }
            }
        }

        /*private void spawnSavedStuffHook(On.SaveState.orig_SpawnSavedObjectsAndCreatures orig, SaveState self, World world, WorldCoordinate atPos)
        {
            self.getFieldtripSaveVals().pendingObjectsBak = new List<string>(self.pendingObjects);
            self.getFieldtripSaveVals().pendingFriendsBak = new List<string>(self.pendingFriendCreatures);
            *//*Debug.Log("SLUGPUP SAFARI: logging pending items: ");
            Debug.Log("SLUGPUP SAFARI: objects \n" + String.Join("\n", self.getFieldtripSaveVals().pendingObjectsBak));
            Debug.Log("SLUGPUP SAFARI: friends \n" + String.Join("\n", self.getFieldtripSaveVals().pendingFriendsBak));*//*

            orig(self, world, atPos);
        }*/

        private void voidEndHook(On.RainWorldGame.orig_ExitToVoidSeaSlideShow orig, RainWorldGame self)
        {
            if(self.manager.upcomingProcess == null && self.IsStorySession)
            {
                //Debug.Log("SLUGPUP SAFARI: loading pending items: ");
                if (self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak != null)
                {
                    self.GetStorySession.saveState.pendingObjects = self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak;
                    //Debug.Log("SLUGPUP SAFARI: objects \n" + String.Join("\n", self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak));
                }
                if (self.GetStorySession.saveState.getFieldtripSaveVals().pendingObjectsBak != null)
                {
                    self.GetStorySession.saveState.pendingFriendCreatures = self.GetStorySession.saveState.getFieldtripSaveVals().pendingFriendsBak;
                    //Debug.Log("SLUGPUP SAFARI: friends \n " + String.Join("\n", self.GetStorySession.saveState.getFieldtripSaveVals().pendingFriendsBak));
                }
            }    
            orig(self);
        }

        private void coreStartHook(On.MoreSlugcats.MSCRoomSpecificScript.MS_CORESTARTUPHEART.orig_Update orig, MSCRoomSpecificScript.MS_CORESTARTUPHEART self, bool eu)
        {
            SaveState save = self.room.game.GetStorySession.saveState;
            Room room = self.room;
            if (self.primed && self.foundCell != null && self.foundCell.room == room && self.foundCell.usingTime > 0f && !save.miscWorldSaveData.moonHeartRestored)
            {
                //save.BringUpToDate(room.game);
                orig(self, eu);
                //Debug.Log("SLUGPUP SAFARI: Saving Heart and Core");
                save.BringUpToDate(room.game);
                AddItemsToPending(room.abstractRoom,save, false);
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

        void AddItemsToPending(AbstractRoom room, SaveState save, bool eCell)
        {
            for (int n = 0; n < room.entities.Count; n++)
            {
                AbstractPhysicalObject obj = room.entities[n] as AbstractPhysicalObject;
                if (obj != null && obj.type != AbstractPhysicalObject.AbstractObjectType.Creature && (eCell || obj.type != MoreSlugcatsEnums.AbstractObjectType.EnergyCell))
                {
                    save.pendingObjects.Add((obj as AbstractPhysicalObject).ToString());
                    Debug.Log("SLUGPUP SAFARI: Saved item manually - " + obj.ToString());
                }
                
            }
        }
        void AddCrittersToPending(AbstractRoom room, SaveState save, bool eCell)
        {
            for (int n = 0; n < room.entities.Count; n++)
            {
                AbstractCreature obj = room.entities[n] as AbstractCreature;
                if (obj != null && obj.creatureTemplate.type != CreatureTemplate.Type.Slugcat)
                {
                    save.pendingFriendCreatures.Add(SaveState.AbstractCreatureToStringStoryWorld(obj));
                    Debug.Log("SLUGPUP SAFARI: Saved critter manually - " + obj.ToString());
                }
                
            }
        }
        private void heartWarp(On.MoreSlugcats.MSCRoomSpecificScript.MS_HEARTWARP.orig_Update orig, MSCRoomSpecificScript.MS_HEARTWARP self, bool eu)
        {
            if (self.fadeOut != null && self.fadeOut.IsDoneFading() && !self.triggered && self.afterFadeTime > 119f)
                moveToBitterStart(self.room);
            orig(self, eu);
        }

        private void bitterHook(On.MoreSlugcats.MSCRoomSpecificScript.MS_bitterstart.orig_Update orig, MSCRoomSpecificScript.MS_bitterstart self, bool eu)
        {
            Vector2 coords = new Vector2(35, 15);
            Vector2 coordsCreature = new Vector2(700, 300);

            IntVector2 intCoords = new IntVector2(35, 15);
            AbstractCreature firstAlivePlayer = self.room.game.FirstAlivePlayer;
            Player player = (self.room.game.Players.Count > 0 && firstAlivePlayer != null) ? (firstAlivePlayer.realizedCreature as Player) : null;
            if (player != null && self.waitCounter > 0 && !player.inShortcut && player.firstChunk.pos.x <= 700f && self.room.game.cameras[0].currentCameraPosition == 0 && self.fadeIn == null)
            {
                self.room.game.SpawnCritters(self.room.game.GetStorySession.saveState.GrabSavedCreatures(firstAlivePlayer, self.room.ToWorldCoordinate(intCoords)), firstAlivePlayer);
                self.room.game.SpawnObjs(self.room.game.GetStorySession.saveState.GrabSavedObjects(firstAlivePlayer, self.room.ToWorldCoordinate(intCoords)));
                placeMyFriends(self.room,coordsCreature);

            }
            orig(self, eu);
        }
        void PrintRoomContents(Room room)
        {
            for (int m = 0; m < room.physicalObjects.Length; m++)
            {
                for (int n = 0; n < room.physicalObjects[m].Count; n++)
                {
                    AbstractPhysicalObject obj = room.physicalObjects[m][n].abstractPhysicalObject;
                    AbstractCreature creature = obj as AbstractCreature;
                    String creachur = "";
                    if (creature != null)
                        creachur = creature.ToString();
                    if(obj != null)
                        Debug.Log("SLUGPUP SAFARI: In room " + room.ToString() + " at " + obj.pos.ToString() + " is "+ obj.type.ToString() + "  " + creachur);

                }
            }
        }
        private void artiEndingHook(On.MoreSlugcats.MSCRoomSpecificScript.LC_FINAL.orig_Update orig, MSCRoomSpecificScript.LC_FINAL self, bool eu)
        {
            orig(self, eu);
            if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding && !self.triggeredBoss && !self.endingTriggered && self.player != null && self.counter == 1)
                spawnMyFriends(self.room, new Vector2(2700f, 500f));

        }

        private void spearEndingHook(On.MoreSlugcats.MSCRoomSpecificScript.SpearmasterEnding.orig_Update orig, MSCRoomSpecificScript.SpearmasterEnding self, bool eu)
        {
            string oldroom = SaveState.forcedEndRoomToAllowwSave;
            SaveState.forcedEndRoomToAllowwSave = self.room.abstractRoom.name;
            orig(self, eu);
            spawnMyFriends(self.room, new Vector2(540f, 147f));
            SaveState.forcedEndRoomToAllowwSave = oldroom;

        }

        private void rivEndingHook(On.MoreSlugcats.MSCRoomSpecificScript.SL_AI_Behavior.orig_Update orig, MSCRoomSpecificScript.SL_AI_Behavior self, bool eu)
        {
            AbstractCreature firstAlivePlayer = self.room.game.FirstAlivePlayer;
            Player player = (self.room.game.Players.Count > 0 && firstAlivePlayer != null) ? (firstAlivePlayer.realizedCreature as Player) : null;
            if(self.counter < 40 && self.fadeIn == null)
            {
                placeMyFriends(self.room, new Vector2(1530f, 155f));
            }
            orig(self, eu);
        }

        
        void moveToBitterStart(Room room)
        {

            //offset ensures coords are different
            int offset = 0;

            offset = moveFromHeart(room.world.GetAbstractRoom("MS_HEART").realizedRoom, offset);
            offset = moveFromHeart(room.world.GetAbstractRoom("MS_CORE").realizedRoom, offset);

            SaveState save = room.game.GetStorySession.saveState;

            //save pups
            Debug.Log("SLUGPUP SAFARI: Saving den objects in den "+ room.game.FirstAlivePlayer.Room.name);
            SaveState.forcedEndRoomToAllowwSave = "MS_bitterstart";
            save.BringUpToDate(room.game);
            SaveState.forcedEndRoomToAllowwSave = "";

            //Save items
            AddItemsToPending(room.world.GetAbstractRoom("MS_bitterstart"), save, false);

            save.denPosition = "MS_bitterstart";
            save.progression.SaveWorldStateAndProgression(false);
            Debug.Log("SLUGPUP SAFARI: Saved " + save.pendingFriendCreatures.Count + " friend(s)");
            Debug.Log("SLUGPUP SAFARI: Saved " + save.pendingObjects.Count + " items(s)");
            save.pendingFriendCreatures.Clear();
            save.pendingObjects.Clear();

        }
        int moveFromHeart(Room room, int offset)
        {
            
            for (int i = 0; i < room.abstractRoom.entities.Count; i++)
            {
                AbstractPhysicalObject obj = room.abstractRoom.entities[i] as AbstractPhysicalObject;
                obj.LoseAllStuckObjects();
                //skip player and energy cell
                if (obj.realizedObject == null || obj.realizedObject is EnergyCell || obj is AbstractCreature)
                {
                    continue;
                }
                AbstractPhysicalObject objCopy = null;
                objCopy = SaveState.AbstractPhysicalObjectFromString(obj.world, obj.ToString());
                //skip if copy failed :(
                if (objCopy == null)
                {
                    Debug.Log("SLUGPUP SAFARI (entities): " + obj.realizedObject.ToString() + " failed to copy and is null");
                    continue;
                }
                objCopy.Abstractize(objCopy.pos);
                objCopy.Move(new WorldCoordinate(room.world.GetAbstractRoom("MS_bitterstart").index, 35 + offset, 13, -1));
                Debug.Log("SLUGPUP SAFARI: Moving Obj - " + objCopy.ToString() + ", New Coord is " + objCopy.pos.ToString());
                offset++;
            }
            for (int i = 0; i < room.abstractRoom.creatures.Count; i++)
            {
                AbstractCreature obj = room.abstractRoom.creatures[i];
                obj.LoseAllStuckObjects();
                //skip player and energy cell
                if (obj.realizedObject == null || obj.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                {
                    continue;
                }
                AbstractCreature objCopy = SaveState.AbstractCreatureFromString(obj.world, SaveState.AbstractCreatureToStringSingleRoomWorld(obj), true);
                
                //skip if copy failed :(
                if (objCopy == null)
                {
                    Debug.Log("SLUGPUP SAFARI (creatures): " + obj.realizedObject.ToString() + " failed to copy and is null");
                    continue;
                }
                objCopy.Abstractize(objCopy.pos);
                objCopy.Move(new WorldCoordinate(room.world.GetAbstractRoom("MS_bitterstart").index, 35 + offset, 13, -1));
                //force pup positions
                if (objCopy.realizedObject is Player)
                {
                    int foodToAdd = (objCopy.realizedObject as Player).MaxFoodInStomach - (objCopy.realizedObject as Player).CurrentFood;

                    (objCopy.realizedObject as Player).SuperHardSetPosition(new Vector2(35, 13) + Custom.RNV());

                }

                Debug.Log("SLUGPUP SAFARI: Moving Creature - " + objCopy.ToString() + ", New Coord is " + objCopy.pos.ToString());
                offset++;

            }
            for (int i = 0; i < room.abstractRoom.entitiesInDens.Count; i++)
            {
                AbstractCreature obj = room.abstractRoom.entitiesInDens[i] as AbstractCreature;
                obj.LoseAllStuckObjects();
                //skip player and energy cell
                if (obj.realizedObject == null || obj.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                {
                    continue;
                }
                AbstractCreature objCopy = SaveState.AbstractCreatureFromString(obj.world, SaveState.AbstractCreatureToStringSingleRoomWorld(obj), true);

                //skip if copy failed :(
                if (objCopy == null)
                {
                    Debug.Log("SLUGPUP SAFARI (entities in dens): " + obj.realizedObject.ToString() + " failed to copy and is null");
                    continue;
                }
                objCopy.Abstractize(objCopy.pos);
                objCopy.Move(new WorldCoordinate(room.world.GetAbstractRoom("MS_bitterstart").index, 35 + offset, 13, -1));
                //force pup positions
                if (objCopy.realizedObject is Player)
                {
                    int foodToAdd = (objCopy.realizedObject as Player).MaxFoodInStomach - (objCopy.realizedObject as Player).CurrentFood;

                    (objCopy.realizedObject as Player).SuperHardSetPosition(new Vector2(35, 13) + Custom.RNV());

                }

                Debug.Log("SLUGPUP SAFARI: Moving Den Creature - " + objCopy.ToString() + ", New Coord is " + objCopy.pos.ToString());
                offset++;

            }
            return offset;
            /*

                        for (int m = 0; m < room.physicalObjects.Length; m++)
                        {
                            for (int n = 0; n < room.physicalObjects[m].Count; n++)
                            {
                                AbstractPhysicalObject obj = room.physicalObjects[m][n].abstractPhysicalObject;
                                obj.LoseAllStuckObjects();
                                //skip player and energy cell
                                if (obj.realizedObject == null || obj.realizedObject is EnergyCell || (obj.realizedObject is Player && !(obj.realizedObject as Player).isNPC))
                                {
                                    continue;
                                }
                                AbstractPhysicalObject objCopy = null;
                                if (obj is AbstractCreature)
                                    objCopy = SaveState.AbstractCreatureFromString(obj.world, SaveState.AbstractCreatureToStringSingleRoomWorld(obj as AbstractCreature), true);
                                else
                                    objCopy = SaveState.AbstractPhysicalObjectFromString(obj.world, obj.ToString());
                                //skip if copy failed :(
                                if (objCopy == null)
                                {
                                    Debug.Log("SLUGPUP SAFARI: " + obj.realizedObject.ToString() + " failed to copy and is null");
                                    continue;
                                }
                                objCopy.Abstractize(objCopy.pos);
                                objCopy.Move(new WorldCoordinate(room.world.GetAbstractRoom("MS_bitterstart").index, 35 + offset, 13, -1));
                                //force pup positions
                                if (objCopy.realizedObject is Player)
                                {
                                    int foodToAdd = (objCopy.realizedObject as Player).MaxFoodInStomach - (objCopy.realizedObject as Player).CurrentFood;

                                    (objCopy.realizedObject as Player).SuperHardSetPosition(new Vector2(35, 13) + Custom.RNV());

                                }

                                Debug.Log("SLUGPUP SAFARI: Moving Obj - " + objCopy.ToString()+ ", New Coord is "+ objCopy.pos.ToString());
                                offset++;

                            }
                        }
                        return offset;*/
        }

        void slugNPCInputPrinter(SlugNPCAI ai)
        {
            Debug.Log("CURRENT SLUGNPC: " + ai.cat.ToString());

            //Debug.Log("CURRENT SLUGNPC STUCK OBJS COUNT: " + ai.cat.abstractCreature.stuckObjects.Count);
            //Debug.Log(">>> X: " + ai.cat.input[0].x);
            //Debug.Log(">>> Y: " + ai.cat.input[0].y);
            //Debug.Log(">>> JMP: " + ai.cat.input[0].jmp);
            //Debug.Log(">>> PCKP: " + ai.cat.input[0].pckp);
            //Debug.Log(">>> BURST VEL X: " + ai.cat.burstVelX);
            //Debug.Log(">>> BURST VEL Y: " + ai.cat.burstVelY);
            //Debug.Log(">>> BURST X: " + ai.cat.burstX);
            //Debug.Log(">>> BURST Y: " + ai.cat.burstY);
            //Debug.Log(">>> PATHER?: " + ai.pathFinder.ToString());
            //Debug.Log(">>> PATHER WORLD?: " + ai.pathFinder.world.ToString());
            //Debug.Log(">>> MAIN CHUNK VEL (X,Y): " + ai.cat.mainBodyChunk.vel.ToString());
            //Debug.Log(">>> SECOND CHUNK VEL (X,Y): " + ai.cat.bodyChunks[1].vel.ToString());
            //Debug.Log(">>> BEHAVIOR: " + ai.behaviorType.ToString());
            //Debug.Log(">>> ANIMATION: " + ai.cat.animation.ToString());
            //Debug.Log(">>> BODY MODE: " + ai.cat.bodyMode.ToString());
            //Debug.Log(">>> STANDING: " + ai.cat.standing.ToString());
            //Debug.Log(">>> FEET STUCK POS: " + ai.cat.feetStuckPos.ToString());
            //Debug.Log(">>> AEROBIC LEVEL: " + ai.cat.aerobicLevel.ToString());
            //Debug.Log(">>> ROOM: " + ai.cat.room.ToString());
            //Debug.Log(">>> LAVA CONTACT COUNT: " + ai.cat.lavaContactCount);
            //Debug.Log(">>> MASS: " + ai.cat.TotalMass);
            //Debug.Log(">>> AIMAP: " + ai.cat.room.ToString());
            //Debug.Log(">>> PLAYER GRAVITY: " + ai.cat.gravity.ToString());
            //Debug.Log(">>> CUSTOM PLAYER GRAVITY: " + ai.cat.customPlayerGravity.ToString());
            //Debug.Log(">>> IN SHORTCUT: " + ai.cat.inShortcut.ToString());
            //Debug.Log(">>> SHOOTUPCOUNTER: " + ai.cat.shootUpCounter.ToString());
            //Debug.Log(">>> SUBMERGED: " + ai.cat.submerged.ToString());
            //Debug.Log(">>> COLLISION LAYER: " + ai.cat.collisionLayer.ToString());
            //Debug.Log(">>> COLLISION RANGE: " + ai.cat.collisionRange.ToString());





        }
        void slugOnBackUpdateHook(On.Player.SlugOnBack.orig_Update orig, Player.SlugOnBack self, bool eu)
        {
            if (self.slugcat != null)
            {
                //iterate over grasps
                for (int i = 0; i < 2; i++)
                {
                    if (self.owner.grasps[i] != null && self.owner.grasps[i].grabbed is Player && self.counter > 10)
                    {
                        int pupTowerSize = 0;
                        //reset grab counter
                        self.counter = 0;
                        self.increment = false;
                        Player slugInHand = (Player) self.owner.grasps[i].grabbed;
                        Player nextSlug = self.slugcat;
                        //find the topmost slug
                        if (!nextSlug.dead||self.owner.CanIPutDeadSlugOnBack(nextSlug))
                        {
                            //ensure Player.SlugOnBack != null
                            if (nextSlug.slugOnBack == null)
                            {
                                nextSlug.slugOnBack = new Player.SlugOnBack(nextSlug);
                                nextSlug.slugOnBack = new Player.SlugOnBack(nextSlug);
                                pupTowerSize++;

                            }
                            while (nextSlug.slugOnBack.slugcat != null && nextSlug.slugOnBack.HasASlug)
                            {
                                pupTowerSize++;
                                nextSlug = nextSlug.slugOnBack.slugcat;
                                //pupTowerSize++;
                                if (nextSlug.slugOnBack == null)
                                {
                                    nextSlug.slugOnBack = new Player.SlugOnBack(nextSlug);
                                    nextSlug.slugOnBack = new Player.SlugOnBack(nextSlug);
                                    pupTowerSize++;

                                }
                            }
                        }
                        //Debug.Log("SLUGUP SAFARI:\n\tStack size= " + pupTowerSize + "\n\tMax stack is infinite= " + OptionsMenu.infinitePupStack.Value + "\n\tMax stack value= " + OptionsMenu.maxStack.Value);
                        //remove slug from hand and place it at the top of the stack
                       if (OptionsMenu.infinitePupStack.Value || pupTowerSize + 1 < OptionsMenu.maxStack.Value)
                        {
                            nextSlug.slugOnBack.SlugToBack(slugInHand);
                            self.owner.ReleaseGrasp(i);
                            adjustTower(self.owner);
                        }

                        
                        
                    }
                }
            }
            orig(self, eu);
        }
        void adjustTower(Player player)
        {
            
            //trying to simulate a shortcut
            List<AbstractPhysicalObject> allConnectedObjects = player.abstractCreature.GetAllConnectedObjects();
            Room room = player.room;
            //remove all scugs and delete them from the update list
            for (int i = 0; i < allConnectedObjects.Count; i++)
            {
                if (allConnectedObjects[i].realizedObject != null && allConnectedObjects[i].realizedObject is Player)
                {
                    //Debug.Log("SLUGPUP SAFARI: Removing " + allConnectedObjects[i].realizedObject.ToString());
                    
                    room.RemoveObject(allConnectedObjects[i].realizedObject);
                    for (int j = 0; j < room.game.cameras[0].spriteLeasers.Count; j++)
                    {
                        if (room.game.cameras[0].spriteLeasers[j].drawableObject.Equals(allConnectedObjects[i].realizedObject.graphicsModule))
                        {
                            room.game.cameras[0].spriteLeasers[j].RemoveAllSpritesFromContainer();
                            room.game.cameras[0].spriteLeasers.RemoveAt(j);
                        }
                        
                    }
                    room.CleanOutObjectNotInThisRoom(allConnectedObjects[i].realizedObject);
                    

                }
                /*if(player.room.game.rainWorld.options.JollyPlayerCount > 1)
                    PlayerGraphics.PopulateJollyColorArray((player.room.game.FirstAlivePlayer.realizedCreature as Player).slugcatStats.name);*/
            }

            //add them in order
            for (int i = 0; i < allConnectedObjects.Count; i++)
            {
                if (allConnectedObjects[i].realizedObject != null && allConnectedObjects[i].realizedObject is Player)
                {
                    room.AddObject(allConnectedObjects[i].realizedObject);
                    
                    //Debug.Log("SLUGPUP SAFARI: ADDED " + allConnectedObjects[i].realizedObject.ToString());
                }
            }
            /*Debug.Log("SLUGPUP SAFARI: Drawables: " + room.drawableObjects.Count);
            Debug.Log("SLUGPUP SAFARI: Update List: " + room.updateList.Count);
            Debug.Log("SLUGPUP SAFARI: Entities: " + room.abstractRoom.entities.Count);
            Debug.Log("SLUGPUP SAFARI: Creatures: " + room.abstractRoom.creatures.Count);*/
            //Debug.Log("SLUGPUP SAFARI: Sleasers: " + room.game.cameras[0].spriteLeasers.Count); 

        }

        private void grabUpdateHook(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            bool shouldIIncrement = false;
            for (int n = 0; n < 2; n++)
            {
                if (self.grasps[n] != null && self.grasps[n].grabbed != null && self.grasps[n].grabbed is Player && (self.grasps[n].grabbed as Player).isNPC && (!(self.grasps[n].grabbed as Player).dead || self.CanIPutDeadSlugOnBack((self.grasps[n].grabbed as Player))))
                {
                    if (self.input[0].pckp)
                    {
                        //confiscate item from slug in hand with grab + up
                        shouldIIncrement = true;
                        if (self.input[0].y == 1 && (self.grasps[n].grabbed as Player).grasps[0] != null)
                        {
                            Debug.Log("Confiscating " + (self.grasps[n].grabbed as Player).grasps[0].grabbed.ToString() + " from " + self.grasps[n].grabbed.ToString());
                            (self.grasps[n].grabbed as Player).ReleaseGrasp(0);
                            //(self.grasps[n].grabbed as Player).grasps[0].Release();
                        }
                    }
                    break;
                }
            }
            orig(self, eu);
            //grab counter stuff
            if (self.slugOnBack != null && !self.slugOnBack.increment && shouldIIncrement)
            {
                self.slugOnBack.increment = true;
            }
            if(self.spearOnBack != null && self.isNPC && self.AI != null && self.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead)
            {
                self.spearOnBack.increment = false;
            }
            //Debug.Log("PLAYER NUMBER: " + self.playerState.playerNumber + "'s GRAB UPDATE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            //THAT NPC CHECK MUST NOT BE MOVED EVER OR YOU WILL FEEL TRUE PAIN
            if (OptionsMenu.keepItemEnabled.Value&&OptionsMenu.itemPassing.Value&&!self.isNPC&&self.onBack==null)
            {
                if (isUpPressed(self))
                {
                    //Debug.Log("-----PLAYER NUMBER: " + self.playerState.playerNumber + "'s Pass up counter: " + self.getFieldtripPlayerVals().moveItemCounter);
                    //recursively move items upwards, starting at the top
                    //must only affect player and npcs, not other players
                    //only pass if next slug has a free hand
                    //check main hand and then backspear
                    //Debug.Log("Up Pressed!");
                    if(self.getFieldtripPlayerVals().moveItemCounter>5)
                    {
                        self.getFieldtripPlayerVals().moveItemCounter = 0;
                        moveItemsUp(self);
                    }
                    self.getFieldtripPlayerVals().moveItemCounter++;
                }
                else if (isDownPressed(self))
                {
                    //move items downwards, starting at the bottom
                    //must only affect player and npcs, not other players
                    //only pass if next slug has a free hand
                    //check main hand and then backspear
                    if (self.getFieldtripPlayerVals().moveItemCounter > 5)
                    {
                        self.getFieldtripPlayerVals().moveItemCounter = 0;
                        Player curr = getPassCandidate(self);
                        Player next = self;
                        while (curr!=null)
                        {
                            if (next.grasps[0] == null && (!next.isNPC || (next.spearOnBack == null || !next.spearOnBack.HasASpear)))
                            {
                                if (curr.grasps[0] != null)
                                {
                                    PhysicalObject obj = curr.grasps[0].grabbed;
                                    curr.ReleaseGrasp(0);
                                    if(next.CanIPickThisUp(obj))
                                        next.SlugcatGrab(obj, 0);
                                    else
                                        curr.SlugcatGrab(obj, 0);
                                }
                                else if (curr.isNPC && (curr.spearOnBack != null && curr.spearOnBack.HasASpear))
                                {
                                    PhysicalObject obj = curr.spearOnBack.spear;
                                    curr.spearOnBack.DropSpear();
                                    if (next.CanIPickThisUp(obj))
                                        next.SlugcatGrab(obj, 0);
                                    else
                                        curr.spearOnBack.SpearToBack(obj as Spear);
                                }
                            }
                            next = curr;
                            curr = getPassCandidate(curr);
                        }
                    }
                    self.getFieldtripPlayerVals().moveItemCounter++;
                }
                else
                    self.getFieldtripPlayerVals().moveItemCounter = 0;

            }

        }
        void moveItemsUp(Player curr)
        {
            Player next = getPassCandidate(curr);
            //Debug.Log("Current slug: " + curr.ToString() + " Next slug: " + next.ToString());
            if (next == null)
                return;
            moveItemsUp(next);
            if (next.grasps[0] == null && (!next.isNPC || (next.spearOnBack == null || !next.spearOnBack.HasASpear)))
            {
                //Debug.Log("Current slug: " + curr.ToString() + " Next slug: " + next.ToString());
                if (curr.grasps[0] != null)
                {
                    PhysicalObject obj = curr.grasps[0].grabbed;
                    curr.ReleaseGrasp(0);
                    if (next.CanIPickThisUp(obj))
                        next.SlugcatGrab(obj, 0);
                    else
                        curr.SlugcatGrab(obj, 0);
                }
                else if (curr.isNPC && (curr.spearOnBack != null && curr.spearOnBack.HasASpear))
                {
                    PhysicalObject obj = curr.spearOnBack.spear;
                    curr.spearOnBack.DropSpear();
                    if (next.CanIPickThisUp(obj))
                        next.SlugcatGrab(obj, 0);
                    else
                        curr.SlugcatGrab(obj, 0);
                }
            }
            
        }
        Player getPassCandidate(Player slug)
        {
            if (slug.slugOnBack == null || !slug.slugOnBack.HasASlug)
                return null;
            else if ((!slug.slugOnBack.slugcat.isNPC && !OptionsMenu.playerItemMoving.Value))
                return getPassCandidate(slug.slugOnBack.slugcat);
            else
                return slug.slugOnBack.slugcat;
        }
        bool isUpPressed(Player player)
        {
            int playerNumber = player.playerState.playerNumber;
            bool isPressed = false;
            if (player.input[0].controllerType == Options.ControlSetup.Preset.KeyboardSinglePlayer || player.input[0].controllerType == Options.ControlSetup.Preset.None)
            {
                isPressed = Input.GetKey(OptionsMenu.upInputKB.Value);
            }
            int gamePadNumber = Custom.rainWorld.options.controls[playerNumber].gamePadNumber;
            switch (gamePadNumber)
            {
                case(0):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController1.Value));
                    return isPressed || Input.GetKey(OptionsMenu.upInputController1.Value);
                case (1):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController2.Value));
                    return (isPressed || Input.GetKey(OptionsMenu.upInputController2.Value));
                case (2):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController3.Value));
                    return isPressed || Input.GetKey(OptionsMenu.upInputController3.Value);
                case (3):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController4.Value));
                    return isPressed || Input.GetKey(OptionsMenu.upInputController4.Value);
                default:
                    return isPressed;
            }
        }
        bool isDownPressed(Player player)
        {
            int playerNumber = player.playerState.playerNumber;
            bool isPressed = false;
            if (player.input[0].controllerType == Options.ControlSetup.Preset.KeyboardSinglePlayer || player.input[0].controllerType == Options.ControlSetup.Preset.None)
            {
                isPressed = Input.GetKey(OptionsMenu.downInputKB.Value);
            }
            int gamePadNumber = Custom.rainWorld.options.controls[playerNumber].gamePadNumber;
            switch (gamePadNumber)
            {
                case (0):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController1.Value));
                    return isPressed || Input.GetKey(OptionsMenu.downInputController1.Value);
                case (1):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController2.Value));
                    return (isPressed || Input.GetKey(OptionsMenu.downInputController2.Value));
                case (2):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController3.Value));
                    return isPressed || Input.GetKey(OptionsMenu.downInputController3.Value);
                case (3):
                    //Debug.Log(Input.GetKey(OptionsMenu.upInputController4.Value));
                    return isPressed || Input.GetKey(OptionsMenu.downInputController4.Value);
                default:
                    return isPressed;
            }
        }
        private void grabbedHook(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            //slugpups tumble down if a lower slug is grabbed by an enemy
            if (grasp.grabber is Lizard || grasp.grabber is Vulture || grasp.grabber is BigSpider || grasp.grabber is DropBug)
            {
                if(self.slugOnBack != null && self.slugOnBack.HasASlug && self.slugOnBack.slugcat.isNPC)
                {
                    slugpupTumble(self.slugOnBack.slugcat);
                }
            }
                orig(self, grasp);
        }
        void slugpupTumble(Player basePup)
        {

            //recursivly destroy the tower above this slug
            if (basePup != null)
            {
                if (basePup.slugOnBack != null && basePup.slugOnBack.HasASlug)
                {
                    slugpupTumble(basePup.slugOnBack.slugcat);
                    basePup.slugOnBack.DropSlug();
                }
            }
        }
        private void slugToHandHook(On.Player.SlugOnBack.orig_SlugToHand orig, Player.SlugOnBack self, bool eu)
        {
            Player slugToHand = self.slugcat;
            orig(self, eu);
            if(slugToHand != null && !self.HasASlug && slugToHand.slugOnBack != null && slugToHand.slugOnBack.HasASlug)
            {
                self.SlugToBack(slugToHand.slugOnBack.slugcat);
                slugToHand.slugOnBack.DropSlug();
            }
        }
        private void SlugNPCUpdateHook_On(On.MoreSlugcats.SlugNPCAI.orig_Update orig, SlugNPCAI self)
        {
            if (self.behaviorType != null && self.behaviorType == SlugNPCAI.BehaviorType.OnHead && self.cat != null)
            {
                if (self.cat.animation == Player.AnimationIndex.VineGrab && OptionsMenu.preventDraggingVines.Value)
                    self.cat.animation = Player.AnimationIndex.None;
                if (self.cat.grasps[0] != null && self.cat.grasps[0].grabbed != null)
                {
                    if (self.cat.grasps[0].grabbed is Spear && OptionsMenu.allowBackspear.Value)
                    {
                        if (self.cat.spearOnBack == null)
                        {
                            self.cat.spearOnBack = new Player.SpearOnBack(self.cat);
                        }
                        self.cat.spearOnBack.SpearToBack(self.cat.grasps[0].grabbed as Spear);
                    }
                    else if (self.cat.grasps[0].grabbed is Creature && !(self.cat.Grabability(self.cat.grasps[0].grabbed) == Player.ObjectGrabability.OneHand))
                    {
                        self.cat.ReleaseGrasp(0);
                        Debug.Log("Pup releasing invalid creature");
                    }
                }
            }
            else if (self.behaviorType!=null && self.behaviorType != SlugNPCAI.BehaviorType.OnHead)
            {
                if (self.cat.spearOnBack != null && self.cat.spearOnBack.HasASpear)
                    self.cat.spearOnBack.SpearToHand(true);
                if (self.cat.slugOnBack != null && self.cat.slugOnBack.HasASlug)
                    slugpupTumble(self.cat);
            }
            //Debug.Log("ANIMINDEX: "+self.cat.animation.ToString());

            orig(self);
            //slugNPCInputPrinter(self);
        }
        private bool canIPickThisUpdate(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
        {
            //Debug.Log(self.ToString() + " wants to grab " + obj.ToString());

            if (obj != null && OptionsMenu.keepItemEnabled.Value && obj.grabbedBy.Any((Creature.Grasp x) => (x != null && x.grabber != null && x.grabber is Player && (x.grabber as Player).isNPC)))
                return false;
            else if (obj != null && self.isNPC && self.AI != null && self.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead && (obj.grabbedBy.Any((Creature.Grasp x) => (x != null && x.grabber != null && x.grabber is Player))))
            {
                return false;
            }
            else
                return orig(self, obj);
        }
        private void passingGrabHook(On.MoreSlugcats.SlugNPCAI.orig_PassingGrab orig, SlugNPCAI self)
        {
 
            if (OptionsMenu.allowGrabbingFood.Value && self.behaviorType == SlugNPCAI.BehaviorType.OnHead && (self.cat.grasps[0] == null || OptionsMenu.canDropForFood.Value))
            {
                //TODO make this not awful
                if (self.itemTracker.ItemCount > 0)
                {
                    for (int j = 0; j < self.itemTracker.ItemCount; j++)
                    {
                        PhysicalObject realizedObject = self.itemTracker.GetRep(j).representedItem.realizedObject;
                        if (self.CanGrabItem(realizedObject) && realizedObject.grabbedBy.Count == 0 && realizedObject is IPlayerEdible)
                        {
                            if (self.WantsToEatThis(realizedObject) && (self.cat.grasps[0] == null || !self.WantsToEatThis(self.cat.grasps[0].grabbed)) && (!(realizedObject is OracleSwarmer) || self.NeuronsLegal()) && (!(realizedObject is SSOracleSwarmer) || OptionsMenu.allowGrabbing5PNeurons.Value) && (!(realizedObject is Mushroom) || OptionsMenu.allowGrabbingShrooms.Value) && (!(realizedObject is KarmaFlower) || OptionsMenu.allowGrabbingKarmaFlowers.Value))
                            {
                                //Debug.Log("Slugpup_Safari: " + self.cat.ToString() + " grabbing " + realizedObject.ToString());
                                self.cat.NPCForceGrab(realizedObject);
                                break;
                            }
                        }
                    }
                }
                if (self.tracker.CreaturesCount > 0)
                {
                    for (int k = 0; k < self.tracker.CreaturesCount; k++)
                    {
                        Creature realizedCreature = self.tracker.GetRep(k).representedCreature.realizedCreature;
                        if (self.CanGrabItem(realizedCreature) && realizedCreature.grabbedBy.Count == 0)
                        {
                            if (self.WantsToEatThis(realizedCreature) && (self.cat.grasps[0] == null || !self.WantsToEatThis(self.cat.grasps[0].grabbed)))
                            {
                                if ((realizedCreature == null || realizedCreature.dead || self.creature.personality.sympathy <= 0.8f) && self.cat.Grabability(realizedCreature) == Player.ObjectGrabability.OneHand && (OptionsMenu.allowGrabbingNoodleflies.Value || !(realizedCreature is SmallNeedleWorm)) && (OptionsMenu.allowGrabbingCentipedes.Value || !(self.preyTracker.MostAttractivePrey.representedCreature.realizedCreature is Centipede)))
                                {
                                    self.cat.NPCForceGrab(realizedCreature);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        private void SlugNPCUpdateHook(ILContext il)
        {
            try
            {
                //put cursor after releasegrasp and get_cat is called
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchLdsfld<SlugNPCAI.BehaviorType>("OnHead"));
                c.GotoNext(i => i.MatchCall<SlugNPCAI>("get_cat"));

                //hope this doesn't break
                c.Emit(OpCodes.Pop);

                //put place holder after release grasp
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(i => i.MatchCallvirt<Creature>("ReleaseGrasp"));
                placeholder.Index++;

                //make label after releasegrasp
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);                
                c.EmitDelegate<Func<bool>>(() => OptionsMenu.keepItemEnabled.Value);
                c.Emit(OpCodes.Brtrue_S, label);
                c.Emit(OpCodes.Ldarg_0);
            }
            catch (Exception e)
            {
                base.Logger.LogError("SlugNPCUpdateHook encountered an error: "+e);
                throw;
            }
        }
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
                    if (!slugcat.isNPC || (OptionsMenu.artificerDontLaunchKids.Value && slugcat.AI != null && slugcat.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead))
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
                c.EmitDelegate<Func<UpdatableAndDeletable,int,int,bool>>((self, m, n) =>{
                    Player player = self.room.physicalObjects[m][n] as Player;
                    if (player == null||!player.isNPC||player.AI == null||player.AI.behaviorType!=SlugNPCAI.BehaviorType.OnHead||!OptionsMenu.artificerDontLaunchKids.Value)
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
        private void grabUpdateILHook(ILContext il)
        {
            //this prevents throwing pups
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchCallvirt<Player.SlugOnBack>("SlugToHand"));
                ILCursor placeholder = new ILCursor(c);
                c.GotoPrev(i => i.MatchLdfld<Player.SlugOnBack>("slugcat"));
                c.Index -= 2;
                placeholder.GotoNext(i => i.MatchLdfld<Player>("wantToPickUp"));
                placeholder.Index--;
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.EmitDelegate<Func<bool>>(() => OptionsMenu.noThrowPups.Value);
                c.Emit(OpCodes.Brtrue_S, label);
                var label2 = il.DefineLabel();
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[3];
                array[0] = ((Instruction i) => i.MatchRet());
                array[1] = ((Instruction i) => i.MatchLdarg(0));
                array[2] = ((Instruction i) => i.MatchLdfld<Player>("pickUpCandidate"));
                c.GotoNext(MoveType.After, array);
                c.Index++;
                placeholder.Index = c.Index;
                placeholder.GotoNext(i => i.MatchRet());
                placeholder.MarkLabel(label2);
                c.Emit(OpCodes.Ldarg_0);
                //check this for player shenanigans
                c.EmitDelegate<Func<Player, bool>>((slugcat) =>
                {
                    if (slugcat.isNPC && !slugcat.safariControlled)
                    {
                        return true;
                    }
                    return false;
                });
                c.Emit(OpCodes.Brtrue_S, label2);


            }
            catch (Exception e)
            {
                base.Logger.LogError("grabUpdateILHook encountered an error: " + e);
                throw;
            }
        }
        
        private void slugNPCMoveHook(ILContext il)
        {
            
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchLdsfld<SlugNPCAI.BehaviorType>("OnHead"));
                c.GotoNext(i => i.MatchRet());
                c.Index--;
                //pop the Inputpackage so we can insert our own
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<SlugNPCAI, Player.InputPackage>>((ai) => 
                {
                    Player.InputPackage input = default(Player.InputPackage);
                    input.x = 0;
                    input.pckp = false;
                    if (ai.HasEdible() && !ai.IsFull)
                    {
                        input.pckp = true;
                    }
                    return input;
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("slugNPCMoveHook encountered an error: " + e);
                throw;
            }
        }
        
        private void slugOnBackUpdateILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[3];
                array[0] = ((Instruction i) => i.MatchLdarg(0));
                array[1] = ((Instruction i) => i.MatchLdfld<Player.SlugOnBack>("counter"));
                array[2] = ((Instruction i) => i.MatchLdcI4(20));
                c.GotoNext(MoveType.Before, array);
                c.Emit(OpCodes.Ldarg_0);
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(i => i.MatchLdfld<Player.SlugOnBack>("slugcat"));
                placeholder.Index--;
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.EmitDelegate<Func<Player.SlugOnBack, bool>>((slugcat) =>
                {
                    if (slugcat.owner.isNPC && slugcat.owner.AI != null && slugcat.owner.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead)
                    {
                        return true;
                    }
                    return false;
                });
                c.Emit(OpCodes.Brtrue_S, label);
                /*c.GotoNext(MoveType.After,i => i.MatchIsinst("Player"));
                c.EmitDelegate<Func<object, object>>((val) =>
                {
                    if (OptionsMenu.maxStack.Value <= 0)
                        return null;
                    return val;
                });*/
            }
            catch (Exception e)
            {
                base.Logger.LogError("slugOnBackUpdateILHook encountered an error: " + e);
                throw;
            }
        }
        private void playerTerrainImpactILHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchLdfld<Player>("playerInAntlers"));
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(i => i.MatchLdarg(0));
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.EmitDelegate<Func<Player, bool>>((slugcat) =>
                {
                    if (slugcat.isNPC && (OptionsMenu.pupImpactCushion.Value && slugcat.AI != null && slugcat.AI.behaviorType == SlugNPCAI.BehaviorType.OnHead))
                    {
                        return true;
                    }
                    return false;
                });
                c.Emit(OpCodes.Brtrue_S, label);
                c.Emit(OpCodes.Ldarg_0);
            }
            catch (Exception e)
            {
                base.Logger.LogError("playerTerrainImpactILHook encountered an error: " + e);
                throw;
            }

        }
        private void playerGraphicsMSCUpdateHook(ILContext il)
        {
            //prevents pups from retracting arm when holding item
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.MatchRet());
                c.GotoPrev(i => i.MatchLdarg(0));
                c.Index++;
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(i => i.MatchAdd());
                placeholder.Index-=2;
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.Emit(OpCodes.Ldloc,10);
                //base.Logger.LogDebug(c);
                //base.Logger.LogDebug(placeholder);
                c.EmitDelegate<Func<PlayerGraphics, int, bool>>((slugcatGraphics, index) =>
                {
                    //slugcatGraphics.player.PyroDeath();
                    if (slugcatGraphics.player.grasps[index]!= null)
                    {
                        if (slugcatGraphics.player.isNPC)
                        {
                            slugcatGraphics.hands[index].mode = Limb.Mode.HuntRelativePosition;
                            slugcatGraphics.hands[index].relativeHuntPos = new Vector2(5f * ((index == 0) ? -1f : 1f), -10f);
                            return true;
                        }
                        else
                        {
                            slugcatGraphics.hands[index].mode = Limb.Mode.HuntRelativePosition;
                            slugcatGraphics.hands[index].relativeHuntPos = new Vector2(5f * ((index == 0) ? -5f : 5f), -10f);
                            return true;
                        }
                    }
                    return false;
                });
                c.Emit(OpCodes.Brtrue_S, label);
                c.Emit(OpCodes.Ldarg_0);
            }
            catch (Exception e)
            {
                base.Logger.LogError("playerGraphicsMSCUpdateHook encountered an error: " + e);
                throw;
            }
        }
        private void leechConsiderationHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[3];
                array[0] = ((Instruction i) => i.MatchLdarg(1));
                array[1] = (i => i.MatchIsinst("Leech"));
                array[2] = ((Instruction i) => i.Match(OpCodes.Brfalse));
                c.GotoNext(MoveType.After, array);
                Func<Instruction, bool>[] array2 = new Func<Instruction, bool>[3];
                array[0] = ((Instruction i) => i.MatchLdarg(0));
                array[1] = (i => i.MatchCall<Creature>("get_Template"));
                array[2] = ((Instruction i) => i.MatchLdarg(1));
                c.GotoNext(MoveType.Before, array);
                c.Index++;
                c.Emit(OpCodes.Pop);
                ILCursor placeholder = new ILCursor(c);
                placeholder.GotoNext(i => i.MatchRet());
                var label = il.DefineLabel();
                placeholder.MarkLabel(label);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<Creature, bool>>((critter) =>
                {
                    if (OptionsMenu.leechProtection.Value && (critter is Player) && (critter as Player).AI != null && (critter as Player).AI.behaviorType == SlugNPCAI.BehaviorType.OnHead)
                    {
                        //base.Logger.LogDebug("BBBBBBBBBBBB");
                        return true;
                    }
                    return false;
                });
                c.Emit(OpCodes.Brtrue_S, label);
                c.Emit(OpCodes.Ldarg_0);
            }
            catch (Exception e)
            {
                base.Logger.LogError("leechConsiderationHook encountered an error: " + e);
                throw;
            }
        }
        private void leechCrawlHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                Func<Instruction, bool>[] array = new Func<Instruction, bool>[2];
                array[0] = ((Instruction i) => i.MatchLdsfld<CreatureTemplate.Relationship.Type>("Eats"));
                array[1] = ((Instruction i) => i.Match(OpCodes.Call));

                c.GotoNext(MoveType.After, array);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 4);
                //get creature to check
                c.EmitDelegate<Func<Leech, int, Creature>>((leech, val) =>
                {
                    return leech.room.abstractRoom.creatures[val].realizedCreature;
                });
                //prevent grabbing if conditions are met
                c.EmitDelegate<Func<bool, Creature, bool>>((val, critter) =>
                {
                    if (OptionsMenu.leechProtection.Value && (critter is Player) && (critter as Player).AI != null && (critter as Player).AI.behaviorType == SlugNPCAI.BehaviorType.OnHead)
                    {
                        return false;
                    }
                    return val;
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("leechCrawlHook encountered an error: " + e);
                throw;
            }
        }

        private void spiderConsiderationHook(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.Match(OpCodes.Brfalse_S));
                c.Emit(OpCodes.Ldarg, 1);
                c.EmitDelegate<Func<bool, Creature, bool>>((origCheck, critter) =>
                {
                    if(origCheck == true)
                    {
                        return true;
                    }
                    if (OptionsMenu.coalescipedeProtection.Value && (critter is Player) && (critter as Player).AI != null && (critter as Player).AI.behaviorType == SlugNPCAI.BehaviorType.OnHead)
                    {
                        //base.Logger.LogDebug("BBBBBBBBBBBB");
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception e)
            {
                base.Logger.LogError("spiderConsiderationHook encountered an error: " + e);
                throw;
            }
        }

        void spawnMyFriends(Room room, Vector2 coord)
        {
            //Debug.Log("SLUGPUP SAFARI: Spawn my friends coords: " + coord.ToString());
            bool debug = RainWorld.ShowLogs;
            AbstractCreature firstAlivePlayer = room.game.FirstAlivePlayer;
            if (room.game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == room)
            {
                Player player = firstAlivePlayer.realizedCreature as Player;
                if (room.game.GetStorySession.saveState.denPosition == room.abstractRoom.name && room.world.rainCycle.timer < 400)
                {
                    if(debug)
                        Debug.Log("Slugpup Safari: Handling Saved Creatures");
                    
                    for (int i = 0; i < room.abstractRoom.creatures.Count; i++)
                    {
                        //skip player
                        if (room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                            continue;

                        Vector2 temp = coord + Custom.RNV();
                        IntVector2  intCoord = new IntVector2(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));

                        //set location
                        if (debug)
                            Debug.Log("Slugpup Safari: >>>>> Moving " + room.abstractRoom.creatures[i].ToString() + " to " + room.ToWorldCoordinate(intCoord).ToString());
                        room.abstractRoom.creatures[i].pos = room.ToWorldCoordinate(intCoord);
                        coord[1] += 1f;
                        //realize if not realized
                        if (room.abstractRoom.creatures[i].realizedCreature == null)
                        {
                            if (debug)
                                Debug.Log("Slugpup Safari: >>>>> SPAWNING " + room.abstractRoom.creatures[i].ToString());
                            room.abstractRoom.creatures[i].Realize();
                        }

                        if ((room.abstractRoom.creatures[i].realizedCreature is Player) && (room.abstractRoom.creatures[i].realizedCreature as Player).isNPC)
                        {
                            int foodToAdd = (room.abstractRoom.creatures[i].realizedCreature as Player).MaxFoodInStomach - (room.abstractRoom.creatures[i].realizedCreature as Player).CurrentFood;
                            (room.abstractRoom.creatures[i].realizedCreature as Player).SetMalnourished(false);
                            (room.abstractRoom.creatures[i].realizedCreature as Player).AddFood((int)Mathf.Max(0, foodToAdd));
                            (room.abstractRoom.creatures[i].realizedCreature as Player).SuperHardSetPosition(coord + Custom.RNV());
                        }
                        else
                            placeFriend(room.abstractRoom.creatures[i].realizedCreature, coord + Custom.RNV());
                    }
                    if (debug)
                        Debug.Log("Slugpup Safari: Handled All Creatures");

                }
            }
        }
        void placeMyFriends(Room room, Vector2 pos)
        {
            int offset = 0;

            for (int m = 0; m < room.physicalObjects.Length; m++)
            {
                for (int n = 0; n < room.physicalObjects[m].Count; n++)
                {
                    AbstractPhysicalObject obj = room.physicalObjects[m][n].abstractPhysicalObject;
                    if (obj.realizedObject == null || (obj.realizedObject is Player && !(obj.realizedObject as Player).isNPC))
                    {
                        continue;
                    }
                    if(obj.realizedObject is Player && (obj.realizedObject as Player).isNPC)
                    {
                        int foodToAdd = (obj.realizedObject as Player).MaxFoodInStomach - (obj.realizedObject as Player).CurrentFood;
                        (obj.realizedObject as Player).SetMalnourished(false);
                        (obj.realizedObject as Player).AddFood((int)Mathf.Max(0, foodToAdd));
                        (obj.realizedObject as Player).SuperHardSetPosition(pos+Custom.RNV());
                    }
                    else
                    {
                        offset++;
                        obj.Move(new WorldCoordinate(room.abstractRoom.index, Mathf.RoundToInt(pos.x) + offset, Mathf.RoundToInt(pos.y), -1));
                    }

                }

            }
        }
        void feedMyFriends(Room room)
        {
            Debug.Log("SLUGPUP SAFARI: feeding friends");
            for (int m = 0; m < room.physicalObjects.Length; m++)
            {
                for (int n = 0; n < room.physicalObjects[m].Count; n++)
                {
                    AbstractPhysicalObject obj = room.physicalObjects[m][n].abstractPhysicalObject;
                    if (obj.realizedObject == null || (obj.realizedObject is Player && !(obj.realizedObject as Player).isNPC))
                    {
                        continue;
                    }
                    if (obj.realizedObject is Player && (obj.realizedObject as Player).isNPC)
                    {
                        int foodToAdd = (obj.realizedObject as Player).MaxFoodInStomach - (obj.realizedObject as Player).CurrentFood;
                        (obj.realizedObject as Player).SetMalnourished(false);
                        (obj.realizedObject as Player).AddFood((int)Mathf.Max(0, foodToAdd));
                    }
                    
                }

            }
        }
        void placeFriend(Creature friend, Vector2 pos)
        {
            for(int i = 0; i < friend.bodyChunks.Count(); i++)
            {
                friend.bodyChunks[i].HardSetPosition(pos + Custom.RNV());
            }
        }

    }
}
