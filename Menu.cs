using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.CodeDom;

namespace FieldTrip
{
    public class OptionsMenu : OptionInterface
    {
        #region Constants
        //horizontal
        const float HORIZONTAL = 30f;
        const float TEXTBOX_LABEL_HORIZONTAL = HORIZONTAL + 10f;
        const float HORIZONTAL_INDENT = 30f;
        const float CHECKBOX_LABEL_HORIZONTAL = HORIZONTAL + 35f;
        const float KEYBIND_HORIZONTAL_2ND_ROW_OFFSET = 250f;
        const float KEYBIND_HORIZONTAL_OFFSET = HORIZONTAL + 30f;
        const float NUM_INPUT_OFFSET = 60f;
        //horizontal (scroll box)
        const float SCROLL_HORIZONTAL_MODIFIER = -35f;

        //vertical
        const float CHECKBOX_VERT_OFFSET = 80f;

        const float KEYBIND_PARAGRAPH_VERT_OFFSET = 100f;

        const float MINI_CHECKBOX_VERT_OFFSET = 50f;
        const float KEYBIND_VERT_OFFSET = 50f;
        const float SCROLL_BOTTOM_PADDING = 60f;

        const float KEYBIND_HEIGHT = (KEYBIND_VERT_OFFSET) - 20f;
        const float KEYBIND_LENGTH = KEYBIND_HORIZONTAL_2ND_ROW_OFFSET - 50f;
        const float KEYBIND_DESC_HORIZONTAL_OFFSET = KEYBIND_HORIZONTAL_OFFSET - 10f;
        const float KEYBIND_DESC_VERTICAL_OFFSET = KEYBIND_HEIGHT;
        const float IMAGE_TEXT_OFFSET = 30f;
        const float CHECKBOX_IMAGE_OFFSET = -23f;
        const float DESC_OFFSET = 20f;
        const float VERT_POS_INIT = 550f;
        const float SCROLL_POS_INIT = 505f;
        const float TITLE_HORIZONTAL = 40f;


        const float NUM_INPUT_SIZE = 50f;
        const float SCROLL_CONTENT_SIZE = 0f;

        Vector2 SCROLL_POS = new(10f, 10f);
        Vector2 SCROLL_SIZE = new(580f, 500f);
        #endregion

        public class SlupfariKeyBind
        {
            string title;
            Configurable<KeyCode> keyboard;
            Configurable<KeyCode> controller1;
            Configurable<KeyCode> controller2;
            Configurable<KeyCode> controller3;
            Configurable<KeyCode> controller4;
            bool leftSide;
            bool preserveVertPos;
            UIelement[] elements;
            float horzOffset;

            public SlupfariKeyBind(
                SlupfariOption<KeyCode> keyboard,
                SlupfariOption<KeyCode> controller1,
                SlupfariOption<KeyCode> controller2,
                SlupfariOption<KeyCode> controller3,
                SlupfariOption<KeyCode> controller4,
                bool leftSide, bool preserveVertPos)
            {
                this.keyboard = keyboard.Get_Config();
                this.controller1 = controller1.Get_Config();
                this.controller2 = controller2.Get_Config();
                this.controller3 = controller3.Get_Config();
                this.controller4 = controller4.Get_Config();
                this.leftSide = leftSide; // has the actual keyboard/controller label
                this.preserveVertPos = preserveVertPos;
                this.horzOffset = (leftSide) ? 0 : KEYBIND_HORIZONTAL_2ND_ROW_OFFSET;
            }
            public void AddText(string title = "")
            {
                this.title = title;
            }
            public UIelement[] MakeElement(ref float vert_pos)
            {
                //if(!leftSide) return [];
                float vertOffset = 0;
                elements = new UIelement[]
                {
                    //TODO: Consolidate some of these for readability
                    new OpLabel(HORIZONTAL + horzOffset, vert_pos + (vertOffset -= KEYBIND_PARAGRAPH_VERT_OFFSET), title, true),

                    new OpLabel(horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET, vert_pos + KEYBIND_DESC_VERTICAL_OFFSET + (vertOffset -= KEYBIND_VERT_OFFSET), "Keyboard", false),
                    new OpKeyBinder(keyboard,new Vector2(KEYBIND_HORIZONTAL_OFFSET + horzOffset, vert_pos + vertOffset),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.AnyController),

                    new OpLabel(horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET, vert_pos  + KEYBIND_DESC_VERTICAL_OFFSET +  (vertOffset -= KEYBIND_VERT_OFFSET), "Controller 1", false),
                    new OpKeyBinder(controller1,new Vector2(KEYBIND_HORIZONTAL_OFFSET + horzOffset, vert_pos + vertOffset),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller1),

                    new OpLabel(horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET, vert_pos  + KEYBIND_DESC_VERTICAL_OFFSET +  (vertOffset -= KEYBIND_VERT_OFFSET), "Controller 2", false),
                    new OpKeyBinder(controller2,new Vector2(KEYBIND_HORIZONTAL_OFFSET + horzOffset, vert_pos + vertOffset),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller2),

                    new OpLabel(horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET, vert_pos  + KEYBIND_DESC_VERTICAL_OFFSET +  (vertOffset -= KEYBIND_VERT_OFFSET), "Controller 3", false),
                    new OpKeyBinder(controller3,new Vector2(KEYBIND_HORIZONTAL_OFFSET + horzOffset, vert_pos + vertOffset),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller3),

                    new OpLabel(horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET, vert_pos  + KEYBIND_DESC_VERTICAL_OFFSET +  (vertOffset -= KEYBIND_VERT_OFFSET), "Controller 4", false),
                    new OpKeyBinder(controller4,new Vector2(KEYBIND_HORIZONTAL_OFFSET + horzOffset, vert_pos + vertOffset),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller4),
                };
                if (!preserveVertPos) vert_pos += vertOffset;
                return elements;

            }
        }
        public class SlupfariOption<T>
        {
            Configurable<T> config;
            Type type;
            string key;
            string title;
            string desc;
            bool disableChildren;
            bool greyedOut;

            float vertOffset;
            float horiOffset;
            SlupfariOption<bool> parent;

            UIelement[] elements;

            public SlupfariOption(ref Configurable<T> config, string key, T defaultVal, ConfigHolder configHolder, SlupfariOption<bool> parent=null)
            {
                this.type = typeof(T);
                this.key = key;
                config = configHolder.Bind<T>(key, defaultVal);
                this.config = config;
                this.parent = parent;
                
                this.vertOffset = SCROLL_POS_INIT;
                this.horiOffset = (parent != null) ? parent.horiOffset + HORIZONTAL_INDENT : 0;
            }

            public Configurable<T> Get_Config()
            {
                return this.config;
            }
            public void AddText(string title, string desc = "")
            {
                this.title = title;
                this.desc = desc;
            }
            public UIelement[] MakeElement(ref float vert_pos)
            {
                if (this.title is null) this.title = "";
                if (this.desc is null) this.desc = "";

                this.vertOffset = vert_pos;
                if (type == typeof(bool))
                {
                    Configurable<bool> option = config as Configurable<bool>;
                    elements = new UIelement[]
                    {
                        new OpCheckBox(option, HORIZONTAL + horiOffset, vert_pos -= CHECKBOX_VERT_OFFSET),
                        new OpLabel(CHECKBOX_LABEL_HORIZONTAL + horiOffset, vert_pos, this.title, true),
                        new OpLabel(CHECKBOX_LABEL_HORIZONTAL + horiOffset, vert_pos - DESC_OFFSET, this.desc, false),
                    };
                }
                else if (type == typeof(int))
                {
                    Configurable<int> option = config as Configurable<int>;
                    elements = new UIelement[]
                    {
                        // TODO: Standardize the offsets for children and text boxes
                        // rn we need an offset for text boxes, both vertical and horizontal
                        new OpTextBox(option, new Vector2(HORIZONTAL + horiOffset, vert_pos -= MINI_CHECKBOX_VERT_OFFSET), NUM_INPUT_SIZE),
                        new OpLabel(TEXTBOX_LABEL_HORIZONTAL + NUM_INPUT_SIZE + horiOffset, vert_pos, this.title, true),
                        new OpLabel(TEXTBOX_LABEL_HORIZONTAL + NUM_INPUT_SIZE + horiOffset, vert_pos - DESC_OFFSET, this.desc, false),
                    };
                }
                return elements;
            }

            public void Update(bool disableChildren = false)
            {
                if (this.parent != null) this.greyedOut = this.parent.disableChildren;
                this.disableChildren = disableChildren;
                foreach(UIelement element in elements)
                {
                    UIconfig uiConfig = element as UIconfig;
                    if(uiConfig != null) uiConfig.greyedOut = this.greyedOut;

                }
            }

        }

        #region OptionDeclarations

        SlupfariOption<bool> keepItemEnabledOption;
        SlupfariOption<bool> artificerDontLaunchKidsOption;
        SlupfariOption<bool> noThrowPupsOption;
        SlupfariOption<bool> pupImpactCushionOption;
        SlupfariOption<bool> allowGrabbingFoodOption;
        SlupfariOption<bool> allowGrabbingNoodlefliesOption;
        SlupfariOption<bool> allowGrabbingCentipedesOption;
        SlupfariOption<bool> allowGrabbing5PNeuronsOption;
        SlupfariOption<bool> allowGrabbingShroomsOption;
        SlupfariOption<bool> allowGrabbingKarmaFlowersOption;
        SlupfariOption<bool> allowGrabbingTardigradesOption;
        SlupfariOption<bool> preventDraggingVinesOption;
        SlupfariOption<bool> allowBackspearOption;
        SlupfariOption<bool> leechProtectionOption;
        SlupfariOption<bool> coalescipedeProtectionOption;
        SlupfariOption<bool> canDropForFoodOption;
        SlupfariOption<int> maxPupSliderOption;
        SlupfariOption<bool> infinitePupStackOption;
        SlupfariOption<KeyCode> upInputController1Option;
        SlupfariOption<KeyCode> downInputController1Option;
        SlupfariOption<KeyCode> upInputController2Option;
        SlupfariOption<KeyCode> downInputController2Option;
        SlupfariOption<KeyCode> upInputController3Option;
        SlupfariOption<KeyCode> downInputController3Option;
        SlupfariOption<KeyCode> upInputController4Option;
        SlupfariOption<KeyCode> downInputController4Option;
        SlupfariOption<KeyCode> upInputKBOption;
        SlupfariOption<KeyCode> downInputKBOption;
        SlupfariOption<bool> itemPassingOption;
        SlupfariOption<bool> playerItemMovingOption;
        SlupfariOption<bool> rivDrownProtectionOption;
        SlupfariOption<bool> artiBombProtectionOption;
        SlupfariOption<bool> bigHeadModeOption;
        SlupfariOption<bool> stunProtectionOption;
        SlupfariOption<int> maxStackOption;
        SlupfariOption<bool> mirosBirdProtectionOption;
        SlupfariOption<bool> killzoneProtectionOption;
        SlupfariKeyBind passUpKeyBind;
        SlupfariKeyBind passDownKeyBind;

        #endregion
        public OptionsMenu(FieldTripMain plugin)
        {
            keepItemEnabledOption = new SlupfariOption<bool>(ref keepItemEnabled, "slugpupsafari_keepItemOnPickup_checkbox", true, this.config);
            artificerDontLaunchKidsOption = new SlupfariOption<bool>(ref artificerDontLaunchKids, "slugpupsafari_artificerdontlaunchkids_checkbox", true, this.config);
            noThrowPupsOption = new SlupfariOption<bool>(ref noThrowPups, "slugpupsafari_noThrowPups_checkbox", true, this.config);
            pupImpactCushionOption = new SlupfariOption<bool>(ref pupImpactCushion, "slugpupsafari_pupimpactcushion_checkbox", true, this.config);
            allowGrabbingFoodOption = new SlupfariOption<bool>(ref allowGrabbingFood, "slugpupsafari_allowgrabbingfood_checkbox", true, this.config, keepItemEnabledOption);
            allowGrabbingNoodlefliesOption = new SlupfariOption<bool>(ref allowGrabbingNoodleflies, "slugpupsafari_allowgrabbingnoodleflies_checkbox", false, this.config, allowGrabbingFoodOption);
            allowGrabbingCentipedesOption = new SlupfariOption<bool>(ref allowGrabbingCentipedes, "slugpupsafari_allowgrabbingcentipedes_checkbox", true, this.config, allowGrabbingFoodOption);
            allowGrabbing5PNeuronsOption = new SlupfariOption<bool>(ref allowGrabbing5PNeurons, "slugpupsafari_allowgrabbing5Pneurons_checkbox", true, this.config, allowGrabbingFoodOption);
            allowGrabbingShroomsOption = new SlupfariOption<bool>(ref allowGrabbingShrooms, "slugpupsafari_allowgrabbingShrooms_checkbox", false, this.config, allowGrabbingFoodOption);
            allowGrabbingKarmaFlowersOption = new SlupfariOption<bool>(ref allowGrabbingKarmaFlowers, "slugpupsafari_allowgrabbingKarmaFlowers_checkbox", true, this.config, allowGrabbingFoodOption);
            allowGrabbingTardigradesOption = new SlupfariOption<bool>(ref allowGrabbingTardigrades, "slugpupsafari_allowgrabbingTardigrades_checkbox", false, this.config, allowGrabbingFoodOption);
            preventDraggingVinesOption = new SlupfariOption<bool>(ref preventDraggingVines, "slugpupsafari_preventDraggingVines_checkbox", false, this.config);
            allowBackspearOption = new SlupfariOption<bool>(ref allowBackspear, "slugpupsafari_allowBackspear_checkbox", true, this.config);
            leechProtectionOption = new SlupfariOption<bool>(ref leechProtection, "slugpupsafari_leechProtection_checkbox", false, this.config);
            coalescipedeProtectionOption = new SlupfariOption<bool>(ref coalescipedeProtection, "slugpupsafari_coalescipedeProtection_checkbox", true, this.config);
            canDropForFoodOption = new SlupfariOption<bool>(ref canDropForFood, "slugpupsafari_canDropForFood_checkbox", true, this.config, allowGrabbingFoodOption);
            maxPupSliderOption = new SlupfariOption<int>(ref maxPupSlider, "slugpupsafari_maxPupSlider_slider", 2, this.config);
            infinitePupStackOption = new SlupfariOption<bool>(ref infinitePupStack, "slugpupsafari_infinitePupStack_checkbox", true, this.config);
            upInputController1Option = new SlupfariOption<KeyCode>(ref upInputController1, "slugpupsafari_upInputCON1_keybind", KeyCode.Joystick1Button8, this.config);
            downInputController1Option = new SlupfariOption<KeyCode>(ref downInputController1, "slugpupsafari_downInputCON1_keybind", KeyCode.Joystick1Button9, this.config);
            upInputController2Option = new SlupfariOption<KeyCode>(ref upInputController2, "slugpupsafari_upInputCON2_keybind", KeyCode.Joystick2Button8, this.config);
            downInputController2Option = new SlupfariOption<KeyCode>(ref downInputController2, "slugpupsafari_downInputCON2_keybind", KeyCode.Joystick2Button9, this.config);
            upInputController3Option = new SlupfariOption<KeyCode>(ref upInputController3, "slugpupsafari_upInputCON3_keybind", KeyCode.Joystick3Button8, this.config);
            downInputController3Option = new SlupfariOption<KeyCode>(ref downInputController3, "slugpupsafari_downInputCON3_keybind", KeyCode.Joystick3Button9, this.config);
            upInputController4Option = new SlupfariOption<KeyCode>(ref upInputController4, "slugpupsafari_upInputCON4_keybind", KeyCode.Joystick4Button8, this.config);
            downInputController4Option = new SlupfariOption<KeyCode>(ref downInputController4, "slugpupsafari_downInputCON4_keybind", KeyCode.Joystick4Button9, this.config);
            upInputKBOption = new SlupfariOption<KeyCode>(ref upInputKB, "slugpupsafari_upInputKB_keybind", KeyCode.LeftBracket, this.config);
            downInputKBOption = new SlupfariOption<KeyCode>(ref downInputKB, "slugpupsafari_dowbInputKB_keybind", KeyCode.RightBracket, this.config);
            itemPassingOption = new SlupfariOption<bool>(ref itemPassing, "slugpupsafari_itemPassing_checkbox", true, this.config);
            playerItemMovingOption = new SlupfariOption<bool>(ref playerItemMoving, "slugpupsafari_playerItemMoving_checkbox", false, this.config);
            rivDrownProtectionOption = new SlupfariOption<bool>(ref rivDrownProtection, "slugpupsafari_rivDrownProtection_checkbox", true, this.config);
            artiBombProtectionOption = new SlupfariOption<bool>(ref artiBombProtection, "slugpupsafari_artiBombProtection_checkbox", true, this.config);
            bigHeadModeOption = new SlupfariOption<bool>(ref bigHeadMode, "slugpupsafari_bigHeadMode_checkbox", false, this.config);
            stunProtectionOption = new SlupfariOption<bool>(ref stunProtection, "slugpupsafari_stunProtection_checkbox", true, this.config);
            maxStackOption = new SlupfariOption<int>(ref maxStack, "slugpupsafari_maxStack_textBox", 1, this.config, infinitePupStackOption);
            mirosBirdProtectionOption = new SlupfariOption<bool>(ref mirosBirdProtection, "slugpupsafari_mirosBirdProtection_checkbox", true, this.config);
            killzoneProtectionOption = new SlupfariOption<bool>(ref killzoneProtection, "slugpupsafari_killzone_checkbox", true, this.config);
            passUpKeyBind = new SlupfariKeyBind(upInputKBOption, upInputController1Option, upInputController2Option, upInputController3Option, upInputController4Option, true, true);
            passDownKeyBind = new SlupfariKeyBind(downInputKBOption, downInputController1Option, downInputController2Option, downInputController3Option, downInputController4Option, false, false);
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


            /*vert_pos = VERT_POS_INIT;
            UIelement[] element4 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Controls", true),
                new OpCheckBox(itemPassing, HORIZONTAL, vert_pos -= MINI_CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Item Passing", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Allow passing items up and down the stack)", false),
                new OpCheckBox(playerItemMoving, HORIZONTAL, vert_pos -= MINI_CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Move Player Items", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(When disabled piggybacked players will be skipped)", false),
                new OpLabel(HORIZONTAL, vert_pos-= 2* MINI_CHECKBOX_VERT_OFFSET, "Keyboard", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos-=LABEL_VERT_OFFSET, "Pass Up", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+ KEYBIND_HORIZONTAL_OFFSET, vert_pos, "Pass Down", true),
                new OpKeyBinder(upInputKB,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.AnyController),
                new OpKeyBinder(downInputKB,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.AnyController),
                new OpLabel(HORIZONTAL, vert_pos-= MINI_CHECKBOX_VERT_OFFSET, "Controller", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos-=LABEL_VERT_OFFSET, "Pass Up", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+ KEYBIND_HORIZONTAL_OFFSET, vert_pos, "Pass Down", true),
                new OpKeyBinder(upInputController1,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller1),
                new OpKeyBinder(downInputController1,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller1),
                new OpKeyBinder(upInputController2,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller2),
                new OpKeyBinder(downInputController2,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller2),
                new OpKeyBinder(upInputController3,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller3),
                new OpKeyBinder(downInputController3,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller3),
                new OpKeyBinder(upInputController4,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller4),
                new OpKeyBinder(downInputController4,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller4),
            };
            itemPassingTab.AddItems(element4);*/

            vert_pos = VERT_POS_INIT;
            itemPassingTab.AddItems(new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Controls", true),
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

        }

        public override void Update()
        {
            base.Update();
/*            bool enableitems
            foreach (UIelement uielement in this.Tabs[1].items.ToArray<UIelement>())
            {
                if (uielement.GetType().IsSubclassOf(typeof(UIconfig)) && !(uielement as UIconfig).cosmetic)
                {
                    UIconfig elem = uielement as UIconfig;
                    if(elem.cfgEntry == keepItemEnabled)

                }
            }*/

        }

        bool GetKeepItemEnabled()
        {
            return keepItemEnabled.Value;
        }
        public static Configurable<bool> keepItemEnabled;
        public static Configurable<bool> artificerDontLaunchKids;
        public static Configurable<bool> noThrowPups;
        public static Configurable<bool> pupImpactCushion;
        public static Configurable<bool> allowGrabbingFood;
        public static Configurable<bool> allowGrabbingNoodleflies;
        public static Configurable<bool> allowGrabbingCentipedes;
        public static Configurable<bool> allowGrabbing5PNeurons;
        public static Configurable<bool> allowGrabbingShrooms;
        public static Configurable<bool> allowGrabbingKarmaFlowers;
        public static Configurable<bool> allowGrabbingTardigrades;
        public static Configurable<bool> preventDraggingVines;
        public static Configurable<bool> allowBackspear;
        public static Configurable<bool> leechProtection;
        public static Configurable<bool> coalescipedeProtection;
        public static Configurable<bool> canDropForFood;
        public static Configurable<int> maxPupSlider;
        public static Configurable<bool> infinitePupStack;
        public static Configurable<KeyCode> upInputController1;
        public static Configurable<KeyCode> downInputController1;
        public static Configurable<KeyCode> upInputController2;
        public static Configurable<KeyCode> downInputController2;
        public static Configurable<KeyCode> upInputController3;
        public static Configurable<KeyCode> downInputController3;
        public static Configurable<KeyCode> upInputController4;
        public static Configurable<KeyCode> downInputController4;
        public static Configurable<KeyCode> upInputKB;
        public static Configurable<KeyCode> downInputKB;
        public static Configurable<bool> itemPassing;
        public static Configurable<bool> playerItemMoving;
        public static Configurable<bool> rivDrownProtection;
        public static Configurable<bool> artiBombProtection;
        public static Configurable<bool> bigHeadMode;
        public static Configurable<bool> stunProtection;
        public static Configurable<int> maxStack;
        public static Configurable<bool> mirosBirdProtection;
        public static Configurable<bool> killzoneProtection;


    }
}
