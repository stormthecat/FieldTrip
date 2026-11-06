using UnityEngine;
using Menu.Remix.MixedUI;


namespace FieldTrip
{
    public partial class OptionsMenu : OptionInterface
    {
        #region Constants
        //horizontal
        const float HORIZONTAL = 30f;
        const float TEXTBOX_LABEL_HORIZONTAL = HORIZONTAL + 10f;
        const float HORIZONTAL_INDENT = 30f;
        const float CHECKBOX_LABEL_HORIZONTAL = HORIZONTAL + 35f;
        const float KEYBIND_HORIZONTAL_2ND_ROW_OFFSET = 250f;
        const float KEYBIND_HORIZONTAL_OFFSET = HORIZONTAL + 30f;

        //vertical
        const float CHECKBOX_VERT_OFFSET_PARENT = 50f;
        const float CHECKBOX_VERT_OFFSET_CHILD = 40f;

        const float KEYBIND_PARAGRAPH_VERT_OFFSET = 100f;

        const float KEYBIND_VERT_OFFSET = 50f;
        const float SCROLL_BOTTOM_PADDING = 60f;

        const float KEYBIND_HEIGHT = (KEYBIND_VERT_OFFSET) - 20f;
        const float KEYBIND_LENGTH = KEYBIND_HORIZONTAL_2ND_ROW_OFFSET - 50f;
        const float KEYBIND_DESC_HORIZONTAL_OFFSET = KEYBIND_HORIZONTAL_OFFSET - 10f;
        const float KEYBIND_DESC_VERTICAL_OFFSET = KEYBIND_HEIGHT;
        const float DESC_OFFSET = 20f;
        const float VERT_POS_INIT = 550f;
        const float SCROLL_POS_INIT = 505f;
        const float TITLE_HORIZONTAL = 40f;


        const float NUM_INPUT_SIZE = 50f;
        const float SCROLL_CONTENT_SIZE = 0f;
        const float SCROLL_WIDTH = 580f;

        Vector2 SCROLL_POS = new(10f, 10f);
        Vector2 SCROLL_SIZE = new(SCROLL_WIDTH, 500f);

        #endregion

        interface SlupfariOptionInterface
        {
            void Update();
        }

        #region OptionDeclarations

        public static SlupfariOption<bool> keepItemEnabledOption;
        static Configurable<bool> keepItemEnabled;
        public static SlupfariOption<bool> artificerDontLaunchKidsOption;
        static Configurable<bool> artificerDontLaunchKids;
        public static SlupfariOption<bool> noThrowPupsOption;
        static Configurable<bool> noThrowPups;
        public static SlupfariOption<bool> pupImpactCushionOption;
        static Configurable<bool> pupImpactCushion;
        public static SlupfariOption<bool> allowGrabbingFoodOption;
        static Configurable<bool> allowGrabbingFood;
        public static SlupfariOption<bool> allowGrabbingNoodlefliesOption;
        static Configurable<bool> allowGrabbingNoodleflies;
        public static SlupfariOption<bool> allowGrabbingCentipedesOption;
        static Configurable<bool> allowGrabbingCentipedes;
        public static SlupfariOption<bool> allowGrabbing5PNeuronsOption;
        static Configurable<bool> allowGrabbing5PNeurons;
        public static SlupfariOption<bool> allowGrabbingShroomsOption;
        static Configurable<bool> allowGrabbingShrooms;
        public static SlupfariOption<bool> allowGrabbingKarmaFlowersOption;
        static Configurable<bool> allowGrabbingKarmaFlowers;
        public static SlupfariOption<bool> allowGrabbingTardigradesOption;
        static Configurable<bool> allowGrabbingTardigrades;
        public static SlupfariOption<bool> preventDraggingVinesOption;
        static Configurable<bool> preventDraggingVines;
        public static SlupfariOption<bool> allowBackspearOption;
        static Configurable<bool> allowBackspear;
        public static SlupfariOption<bool> leechProtectionOption;
        static Configurable<bool> leechProtection;
        public static SlupfariOption<bool> coalescipedeProtectionOption;
        static Configurable<bool> coalescipedeProtection;
        public static SlupfariOption<bool> canDropForFoodOption;
        static Configurable<bool> canDropForFood;
        public static SlupfariOption<int> maxPupSliderOption;
        static Configurable<int> maxPupSlider;
        public static SlupfariOption<bool> infinitePupStackOption;
        static Configurable<bool> infinitePupStack;
        public static SlupfariOption<KeyCode> upInputController1Option;
        static Configurable<KeyCode> upInputController1;
        public static SlupfariOption<KeyCode> downInputController1Option;
        static Configurable<KeyCode> downInputController1;
        public static SlupfariOption<KeyCode> upInputController2Option;
        static Configurable<KeyCode> upInputController2;
        public static SlupfariOption<KeyCode> downInputController2Option;
        static Configurable<KeyCode> downInputController2;
        public static SlupfariOption<KeyCode> upInputController3Option;
        static Configurable<KeyCode> upInputController3;
        public static SlupfariOption<KeyCode> downInputController3Option;
        static Configurable<KeyCode> downInputController3;
        public static SlupfariOption<KeyCode> upInputController4Option;
        static Configurable<KeyCode> upInputController4;
        public static SlupfariOption<KeyCode> downInputController4Option;
        static Configurable<KeyCode> downInputController4;
        public static SlupfariOption<KeyCode> upInputKBOption;
        static Configurable<KeyCode> upInputKB;
        public static SlupfariOption<KeyCode> downInputKBOption;
        static Configurable<KeyCode> downInputKB;
        public static SlupfariOption<bool> itemPassingOption;
        static Configurable<bool> itemPassing;
        public static SlupfariOption<bool> playerItemMovingOption;
        static Configurable<bool> playerItemMoving;
        public static SlupfariOption<bool> rivDrownProtectionOption;
        static Configurable<bool> rivDrownProtection;
        public static SlupfariOption<bool> artiBombProtectionOption;
        static Configurable<bool> artiBombProtection;
        public static SlupfariOption<bool> bigHeadModeOption;
        static Configurable<bool> bigHeadMode;
        public static SlupfariOption<bool> stunProtectionOption;
        static Configurable<bool> stunProtection;
        public static SlupfariOption<int> maxStackOption;
        static Configurable<int> maxStack;
        public static SlupfariOption<bool> mirosBirdProtectionOption;
        static Configurable<bool> mirosBirdProtection;
        public static SlupfariOption<bool> killzoneProtectionOption;
        static Configurable<bool> killzoneProtection;

        public static SlupfariKeyBind passUpKeyBind;
        public static SlupfariKeyBind passDownKeyBind;

        #endregion

        SlupfariOptionInterface[] optionUpdateList;
        public OptionsMenu(FieldTripMain plugin)
        {
            optionUpdateList = new SlupfariOptionInterface[]
            {
                keepItemEnabledOption = new SlupfariOption<bool>(ref keepItemEnabled, "slugpupsafari_keepItemOnPickup_checkbox", true, this.config),
                artificerDontLaunchKidsOption = new SlupfariOption<bool>(ref artificerDontLaunchKids, "slugpupsafari_artificerdontlaunchkids_checkbox", true, this.config),
                noThrowPupsOption = new SlupfariOption<bool>(ref noThrowPups, "slugpupsafari_noThrowPups_checkbox", true, this.config),
                pupImpactCushionOption = new SlupfariOption<bool>(ref pupImpactCushion, "slugpupsafari_pupimpactcushion_checkbox", true, this.config),
                allowGrabbingFoodOption = new SlupfariOption<bool>(ref allowGrabbingFood, "slugpupsafari_allowgrabbingfood_checkbox", true, this.config, keepItemEnabledOption),
                allowGrabbingNoodlefliesOption = new SlupfariOption<bool>(ref allowGrabbingNoodleflies, "slugpupsafari_allowgrabbingnoodleflies_checkbox", false, this.config, allowGrabbingFoodOption),
                allowGrabbingCentipedesOption = new SlupfariOption<bool>(ref allowGrabbingCentipedes, "slugpupsafari_allowgrabbingcentipedes_checkbox", true, this.config, allowGrabbingFoodOption),
                allowGrabbing5PNeuronsOption = new SlupfariOption<bool>(ref allowGrabbing5PNeurons, "slugpupsafari_allowgrabbing5Pneurons_checkbox", true, this.config, allowGrabbingFoodOption),
                allowGrabbingShroomsOption = new SlupfariOption<bool>(ref allowGrabbingShrooms, "slugpupsafari_allowgrabbingShrooms_checkbox", false, this.config, allowGrabbingFoodOption),
                allowGrabbingKarmaFlowersOption = new SlupfariOption<bool>(ref allowGrabbingKarmaFlowers, "slugpupsafari_allowgrabbingKarmaFlowers_checkbox", true, this.config, allowGrabbingFoodOption),
                allowGrabbingTardigradesOption = new SlupfariOption<bool>(ref allowGrabbingTardigrades, "slugpupsafari_allowgrabbingTardigrades_checkbox", false, this.config, allowGrabbingFoodOption),
                preventDraggingVinesOption = new SlupfariOption<bool>(ref preventDraggingVines, "slugpupsafari_preventDraggingVines_checkbox", false, this.config),
                allowBackspearOption = new SlupfariOption<bool>(ref allowBackspear, "slugpupsafari_allowBackspear_checkbox", true, this.config),
                leechProtectionOption = new SlupfariOption<bool>(ref leechProtection, "slugpupsafari_leechProtection_checkbox", false, this.config),
                coalescipedeProtectionOption = new SlupfariOption<bool>(ref coalescipedeProtection, "slugpupsafari_coalescipedeProtection_checkbox", true, this.config),
                canDropForFoodOption = new SlupfariOption<bool>(ref canDropForFood, "slugpupsafari_canDropForFood_checkbox", true, this.config, allowGrabbingFoodOption),
                maxPupSliderOption = new SlupfariOption<int>(ref maxPupSlider, "slugpupsafari_maxPupSlider_slider", 2, this.config),
                infinitePupStackOption = new SlupfariOption<bool>(ref infinitePupStack, "slugpupsafari_infinitePupStack_checkbox", true, this.config),
                upInputController1Option = new SlupfariOption<KeyCode>(ref upInputController1, "slugpupsafari_upInputCON1_keybind", KeyCode.Joystick1Button8, this.config),
                downInputController1Option = new SlupfariOption<KeyCode>(ref downInputController1, "slugpupsafari_downInputCON1_keybind", KeyCode.Joystick1Button9, this.config),
                upInputController2Option = new SlupfariOption<KeyCode>(ref upInputController2, "slugpupsafari_upInputCON2_keybind", KeyCode.Joystick2Button8, this.config),
                downInputController2Option = new SlupfariOption<KeyCode>(ref downInputController2, "slugpupsafari_downInputCON2_keybind", KeyCode.Joystick2Button9, this.config),
                upInputController3Option = new SlupfariOption<KeyCode>(ref upInputController3, "slugpupsafari_upInputCON3_keybind", KeyCode.Joystick3Button8, this.config),
                downInputController3Option = new SlupfariOption<KeyCode>(ref downInputController3, "slugpupsafari_downInputCON3_keybind", KeyCode.Joystick3Button9, this.config),
                upInputController4Option = new SlupfariOption<KeyCode>(ref upInputController4, "slugpupsafari_upInputCON4_keybind", KeyCode.Joystick4Button8, this.config),
                downInputController4Option = new SlupfariOption<KeyCode>(ref downInputController4, "slugpupsafari_downInputCON4_keybind", KeyCode.Joystick4Button9, this.config),
                upInputKBOption = new SlupfariOption<KeyCode>(ref upInputKB, "slugpupsafari_upInputKB_keybind", KeyCode.LeftBracket, this.config),
                downInputKBOption = new SlupfariOption<KeyCode>(ref downInputKB, "slugpupsafari_dowbInputKB_keybind", KeyCode.RightBracket, this.config),
                itemPassingOption = new SlupfariOption<bool>(ref itemPassing, "slugpupsafari_itemPassing_checkbox", true, this.config),
                playerItemMovingOption = new SlupfariOption<bool>(ref playerItemMoving, "slugpupsafari_playerItemMoving_checkbox", false, this.config),
                rivDrownProtectionOption = new SlupfariOption<bool>(ref rivDrownProtection, "slugpupsafari_rivDrownProtection_checkbox", true, this.config),
                artiBombProtectionOption = new SlupfariOption<bool>(ref artiBombProtection, "slugpupsafari_artiBombProtection_checkbox", true, this.config),
                bigHeadModeOption = new SlupfariOption<bool>(ref bigHeadMode, "slugpupsafari_bigHeadMode_checkbox", false, this.config),
                stunProtectionOption = new SlupfariOption<bool>(ref stunProtection, "slugpupsafari_stunProtection_checkbox", true, this.config),
                maxStackOption = new SlupfariOption<int>(ref maxStack, "slugpupsafari_maxStack_textBox", 1, this.config, infinitePupStackOption),
                mirosBirdProtectionOption = new SlupfariOption<bool>(ref mirosBirdProtection, "slugpupsafari_mirosBirdProtection_checkbox", true, this.config),
                killzoneProtectionOption = new SlupfariOption<bool>(ref killzoneProtection, "slugpupsafari_killzone_checkbox", true, this.config),
            };
            passUpKeyBind = new SlupfariKeyBind(upInputKBOption, upInputController1Option, upInputController2Option, upInputController3Option, upInputController4Option, true, true);
            passDownKeyBind = new SlupfariKeyBind(downInputKBOption, downInputController1Option, downInputController2Option, downInputController3Option, downInputController4Option, false, false);

            infinitePupStackOption.SetChildDisableMode(SlupfariOption<bool>.childDisableStates.INVERSE);
        }

        public override void Initialize()
        {
            float vert_pos = VERT_POS_INIT;

            base.Initialize();
            OpTab mainConfigTab = new OpTab(this, "Main Config");
            OpScrollBox mainConfigScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            OpTab keepItemTab = new OpTab(this, "Keep Item Config");
            OpScrollBox keepItemScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            OpTab creatureImmunityTab = new OpTab(this, "Creature Immunities");
            OpScrollBox creatureImmunityScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            OpTab itemPassingTab = new OpTab(this, "Item Passing");
            OpScrollBox itemPassingScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            OpTab sillinessTab = new OpTab(this, "Silliness");
            OpScrollBox sillinessScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            OpTab campaignSpecificTab = new OpTab(this, "Campaign Specific");
            OpScrollBox campaignSpecificScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);

            this.Tabs = new OpTab[]
            {
                mainConfigTab, campaignSpecificTab, keepItemTab, creatureImmunityTab, itemPassingTab, sillinessTab
            };

            AddDescriptions();

            #region mainConfigTab
            vert_pos = VERT_POS_INIT;
            mainConfigTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Slugpup Safari Options", true),
                mainConfigScroll
            });
            vert_pos = SCROLL_POS_INIT;
            mainConfigScroll.AddItems([
                ..noThrowPupsOption.MakeElement(ref vert_pos),
                ..allowBackspearOption.MakeElement(ref vert_pos),
                ..artificerDontLaunchKidsOption.MakeElement(ref vert_pos),
                ..pupImpactCushionOption.MakeElement(ref vert_pos),
                ..preventDraggingVinesOption.MakeElement(ref vert_pos),
                ..stunProtectionOption.MakeElement(ref vert_pos),
                ..killzoneProtectionOption.MakeElement(ref vert_pos),
                ..infinitePupStackOption.MakeElement(ref vert_pos),
                ..maxStackOption.MakeElement(ref vert_pos)
            ]);
            mainConfigScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

            #region keepItemTab
            vert_pos = VERT_POS_INIT;
            keepItemTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Slugpup Grab Options", true),
                keepItemScroll
            });
            vert_pos = SCROLL_POS_INIT;
            keepItemScroll.AddItems([
                ..keepItemEnabledOption.MakeElement(ref vert_pos),
                ..allowGrabbingFoodOption.MakeElement(ref vert_pos),
                ..allowGrabbingNoodlefliesOption.MakeElement(ref vert_pos),
                ..allowGrabbingCentipedesOption.MakeElement(ref vert_pos),
                ..allowGrabbingTardigradesOption.MakeElement(ref vert_pos),
                ..allowGrabbing5PNeuronsOption.MakeElement(ref vert_pos),
                ..allowGrabbingShroomsOption.MakeElement(ref vert_pos),
                ..allowGrabbingKarmaFlowersOption.MakeElement(ref vert_pos),
                ..canDropForFoodOption.MakeElement(ref vert_pos)
            ]);
            keepItemScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

            #region creatureImmunityTab
            vert_pos = VERT_POS_INIT;
            creatureImmunityTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Creature Protection Options", true),
                creatureImmunityScroll
            });
            vert_pos = SCROLL_POS_INIT;
            creatureImmunityScroll.AddItems([
                ..leechProtectionOption.MakeElement(ref vert_pos),
                ..coalescipedeProtectionOption.MakeElement(ref vert_pos),
                ..mirosBirdProtectionOption.MakeElement(ref vert_pos)
            ]);
            creatureImmunityScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

            #region itemPassingTab
            vert_pos = VERT_POS_INIT;
            itemPassingTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Item Passing Controls", true),
                itemPassingScroll
            });
            vert_pos = SCROLL_POS_INIT;
            itemPassingScroll.AddItems([
                ..itemPassingOption.MakeElement(ref vert_pos),
                ..playerItemMovingOption.MakeElement(ref vert_pos),
                ..passUpKeyBind.MakeElement(ref vert_pos),
                ..passDownKeyBind.MakeElement(ref vert_pos)

            ]);
            itemPassingScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

            #region sillinessTab
            vert_pos = VERT_POS_INIT;
            sillinessTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Silliness", true),
                sillinessScroll
            });
            vert_pos = SCROLL_POS_INIT;
            sillinessScroll.AddItems([
                ..bigHeadModeOption.MakeElement(ref vert_pos)
            ]);
            sillinessScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

            #region campaignSpecificTab
            vert_pos = VERT_POS_INIT;
            campaignSpecificTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Campaign Specific", true),
                campaignSpecificScroll
            });
            vert_pos = SCROLL_POS_INIT;
            campaignSpecificScroll.AddItems([
                ..rivDrownProtectionOption.MakeElement(ref vert_pos),
                ..artiBombProtectionOption.MakeElement(ref vert_pos)
            ]);
            campaignSpecificScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            #endregion

        }

        private static void AddDescriptions()
        {
            #region OptionText
            keepItemEnabledOption.AddText(
                "Keep Items on Pickup",
                "(Allow slugpups to keep holding their item when picked up by the player)"
                );
            artificerDontLaunchKidsOption.AddText(
                "Artificer Parry Protection",
                "(Prevent piggybacked slugpups from being launched by artificer's parry)"
                );
            noThrowPupsOption.AddText(
                "Pup Throw Protection",
                "(Piggybacked slugpups must be moved to your hand in order to throw them)"
                );
            pupImpactCushionOption.AddText(
                "Pup Impact Immunity",
                "(Piggybacked slugpups are immune to fall damage)"
                );
            allowGrabbingFoodOption.AddText(
                "Allow Grabbing Food",
                "(Allow slugpups to grab food while piggybacked)"
                );
            allowGrabbingNoodlefliesOption.AddText(
                "Allow Grabbing Noodleflies"
                );
            allowGrabbingCentipedesOption.AddText(
                "Allow Grabbing Centipedes",
                "(Slugpups cannot eat when you are moving, so be careful)"
                );
            allowGrabbing5PNeuronsOption.AddText(
                "Allow Grabbing Five Pebble's Neurons",
                "(Neurons outside of Five Pebbles are always forbidden)"
                );
            allowGrabbingShroomsOption.AddText(
                "Allow Grabbing Mushrooms"
                );
            allowGrabbingKarmaFlowersOption.AddText(
                "Allow Grabbing Karma Flowers"
                );
            allowGrabbingTardigradesOption.AddText(
                "Allow Grabbing Tardigrades",
                "(Only Applicable if Watcher is Enabled)"
                );
            preventDraggingVinesOption.AddText(
                "Release Vines",
                "(Force slugpups to release vines when piggybacked)"
                );
            allowBackspearOption.AddText(
                "Allow Backspear",
                "(Piggybacked slugpups will put their spear on their back)"
                );
            leechProtectionOption.AddText(
                "Leech Protection",
                "(Leeches will not target piggybacked slugpups)"
                );
            coalescipedeProtectionOption.AddText(
                "Coalescipede Protection",
                "(Coalescipedes will not target piggybacked slugpups)"
                );
            canDropForFoodOption.AddText(
                "Allow Dropping Items",
                "(Slugpups will drop items in their hands to pick up food)"
                );
            maxStackOption.AddText(
                "Maximum Stack Limit"
                );
            infinitePupStackOption.AddText(
                "Infinite Stacking"
                );
            itemPassingOption.AddText(
                "Item Passing",
                "(Allow passing items up and down the stack)"
                );
            playerItemMovingOption.AddText(
                "Move Player Items",
                "(When disabled piggybacked players will be skipped)"
                );
            rivDrownProtectionOption.AddText(
                "Rivulet Drowning Immunity",
                "(Slugpups cannot drown in Rivulet's campaign)"
                );
            artiBombProtectionOption.AddText(
                "Artificer Explosion Immunity",
                "(Slugpups resist explosions in Artificer's campaign)"
                );
            bigHeadModeOption.AddText(
                "Big Brain Mode",
                "(Doubles the size of your slugpup's head for maximum brainage)"
                );
            stunProtectionOption.AddText(
                "Stun Protection",
                "(Slugpups won't fall off of stunned players and stunned pups can be piggybacked)"
                );
            mirosBirdProtectionOption.AddText(
                "Miros Bird Protection",
                "(Miros Birds will not bite piggybacked slugpups)"
                );
            killzoneProtectionOption.AddText(
                "Killzone Protection",
                "(Piggybacked slugcats will not die if out of bounds)"
                );
            passUpKeyBind.AddText("Pass Up");
            passDownKeyBind.AddText("Pass Down");

            #endregion
        }

        public override void Update()
        {
            base.Update();
            foreach (SlupfariOptionInterface option in optionUpdateList)
            {
                option.Update();
            }
        }

    }
}
