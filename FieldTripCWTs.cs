using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Watcher;

namespace FieldTrip
{
    public static class FieldTripCWTs
    {
        public class PlayerCWT
        {
            public int moveItemCounter;
            public RoomCamera.SpriteLeaser sLeaser;
            public RoomCamera rCam;
            public RoomPalette palette;
            public PlayerCWT()
            {
                this.moveItemCounter = 0;
                this.sLeaser = null;
                this.rCam = null;

            }
        }
        private static readonly ConditionalWeakTable<Player, PlayerCWT> ThePlayerCWT = new ConditionalWeakTable<Player, PlayerCWT>();
        public static PlayerCWT getFieldtripPlayerVals(this Player player) => ThePlayerCWT.GetValue(player, _ => new PlayerCWT());

        public class SaveCWT
        {
            public List<string> pendingObjectsBak;
            public List<string> pendingFriendsBak;
            public SaveCWT()
            {
                this.pendingObjectsBak = null;
                this.pendingFriendsBak = null;
            }
        }
        private static readonly ConditionalWeakTable<SaveState, SaveCWT> TheSaveCWT = new ConditionalWeakTable<SaveState, SaveCWT>();
        public static SaveCWT getFieldtripSaveVals(this SaveState save) => TheSaveCWT.GetValue(save, _ => new SaveCWT());

        public class storyGameSessionCWT
        {
            public List<Player> pupStack;

            public storyGameSessionCWT()
            {
                this.pupStack = null;
            }
        }
        private static readonly ConditionalWeakTable<StoryGameSession, storyGameSessionCWT> TheStoryGameSessionCWT = new ConditionalWeakTable<StoryGameSession, storyGameSessionCWT>();
        public static storyGameSessionCWT getFieldtripstoryGameSessionVals(this StoryGameSession session) => TheStoryGameSessionCWT.GetValue(session, _ => new storyGameSessionCWT());

        public class playerGraphicsCWT
        {
            public RoomCamera.SpriteLeaser sLeaser;

            public playerGraphicsCWT()
            {
            }
        }
        private static readonly ConditionalWeakTable<PlayerGraphics, playerGraphicsCWT> ThePlayerGraphicsCWT = new ConditionalWeakTable<PlayerGraphics, playerGraphicsCWT>();

        public static playerGraphicsCWT getFieldtripPlayerGraphicsVals(this PlayerGraphics graphics) => ThePlayerGraphicsCWT.GetValue(graphics, _ => new playerGraphicsCWT());

        public class spriteLeaserCWT
        {
            public int sluppyContainerIndex;

            public spriteLeaserCWT()
            {
                this.sluppyContainerIndex = -1;
            }
        }
        private static readonly ConditionalWeakTable<RoomCamera.SpriteLeaser, spriteLeaserCWT> TheSpriteLeaserCWT = new ConditionalWeakTable<RoomCamera.SpriteLeaser, spriteLeaserCWT>();
        public static spriteLeaserCWT getFieldtripSpriteLeaserVals(this RoomCamera.SpriteLeaser graphics) => TheSpriteLeaserCWT.GetValue(graphics, _ => new spriteLeaserCWT());
    }
}

