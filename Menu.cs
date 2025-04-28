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

namespace FieldTrip
{
    public class OptionsMenu : OptionInterface
    {
        public OptionsMenu(FieldTripMain plugin)
        {
            keepItemEnabled = this.config.Bind<bool>("slugpupsafari_keepItemOnPickup_checkbox", true);
            artificerDontLaunchKids = this.config.Bind<bool>("slugpupsafari_artificerdontlaunchkids_checkbox", true);
            noThrowPups = this.config.Bind<bool>("slugpupsafari_noThrowPups_checkbox", true);
            pupImpactCushion = this.config.Bind<bool>("slugpupsafari_pupimpactcushion_checkbox", true);
            allowGrabbingFood = this.config.Bind<bool>("slugpupsafari_allowgrabbingfood_checkbox", true);
            allowGrabbingNoodleflies = this.config.Bind<bool>("slugpupsafari_allowgrabbingnoodleflies_checkbox", false);
            allowGrabbingCentipedes = this.config.Bind<bool>("slugpupsafari_allowgrabbingcentipedes_checkbox", true);
            allowGrabbing5PNeurons = this.config.Bind<bool>("slugpupsafari_allowgrabbing5Pneurons_checkbox", false);
            allowGrabbingShrooms = this.config.Bind<bool>("slugpupsafari_allowgrabbingShrooms_checkbox", false);
            allowGrabbingKarmaFlowers = this.config.Bind<bool>("slugpupsafari_allowgrabbingKarmaFlowers_checkbox", true);
            allowGrabbingTardigrades = this.config.Bind<bool>("slugpupsafari_allowgrabbingTardigrades_checkbox", false);
            preventDraggingVines = this.config.Bind<bool>("slugpupsafari_preventDraggingVines_checkbox", false);
            allowBackspear = this.config.Bind<bool>("slugpupsafari_allowBackspear_checkbox", true);
            leechProtection = this.config.Bind<bool>("slugpupsafari_leechProtection_checkbox", false);
            coalescipedeProtection = this.config.Bind<bool>("slugpupsafari_coalescipedeProtection_checkbox", true);
            canDropForFood = this.config.Bind<bool>("slugpupsafari_canDropForFood_checkbox", true);
            maxPupSlider = this.config.Bind<int>("slugpupsafari_maxPupSlider_slider", 2);
            infinitePupStack = this.config.Bind<bool>("slugpupsafari_infinitePupStack_checkbox", true);
            upInputController1 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON1_keybind", KeyCode.Joystick1Button8);
            downInputController1 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON1_keybind", KeyCode.Joystick1Button9);
            upInputController2 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON2_keybind", KeyCode.Joystick2Button8);
            downInputController2 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON2_keybind", KeyCode.Joystick2Button9);
            upInputController3 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON3_keybind", KeyCode.Joystick3Button8);
            downInputController3 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON3_keybind", KeyCode.Joystick3Button9);
            upInputController4 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON4_keybind", KeyCode.Joystick4Button8);
            downInputController4 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON4_keybind", KeyCode.Joystick4Button9);
            upInputKB = this.config.Bind<KeyCode>("slugpupsafari_upInputKB_keybind", KeyCode.LeftBracket);
            downInputKB = this.config.Bind<KeyCode>("slugpupsafari_dowbInputKB_keybind", KeyCode.RightBracket);
            itemPassing = this.config.Bind<bool>("slugpupsafari_itemPassing_checkbox", true);
            playerItemMoving = this.config.Bind<bool>("slugpupsafari_playerItemMoving_checkbox", false);
            rivDrownProtection = this.config.Bind<bool>("slugpupsafari_rivDrownProtection_checkbox", true);
            artiBombProtection = this.config.Bind<bool>("slugpupsafari_artiBombProtection_checkbox", true);
            bigHeadMode = this.config.Bind<bool>("slugpupsafari_bigHeadMode_checkbox", false);
            stunProtection = this.config.Bind<bool>("slugpupsafari_stunProtection_checkbox", true);
            maxStack = this.config.Bind<int>("slugpupsafari_maxStack_textBox", 1);
            mirosBirdProtection = this.config.Bind<bool>("slugpupsafari_mirosBirdProtection_checkbox", true);
            killzoneProtection = this.config.Bind<bool>("slugpupsafari_killzone_checkbox", true);


            //myriad Keybinds
            /*upInputController5 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON5_keybind", KeyCode.Joystick5Button8);
            downInputController5 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON5_keybind", KeyCode.Joystick5Button9);
            upInputController6 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON6_keybind", KeyCode.Joystick6Button8);
            downInputController6 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON6_keybind", KeyCode.Joystick6Button9);
            upInputController7 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON7_keybind", KeyCode.Joystick7Button8);
            downInputController7 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON7_keybind", KeyCode.Joystick7Button9);
            upInputController8 = this.config.Bind<KeyCode>("slugpupsafari_upInputCON8_keybind", KeyCode.Joystick8Button8);
            downInputController8 = this.config.Bind<KeyCode>("slugpupsafari_downInputCON8_keybind", KeyCode.Joystick8Button9);*/

        }
        public override void Initialize()
        {
            //horizontal
            float CHECKBOX_HORIZONTAL = 60f;
            float SUB_CHECKBOX_HORIZONTAL = CHECKBOX_HORIZONTAL + 30f;
            float SUB_SUB_CHECKBOX_HORIZONTAL = SUB_CHECKBOX_HORIZONTAL + 50f;
            float CHECKBOX_LABEL_HORIZONTAL = CHECKBOX_HORIZONTAL + 35f;
            float SUB_CHECKBOX_LABEL_HORIZONTAL = SUB_CHECKBOX_HORIZONTAL + 35f;
            float SUB_SUB_CHECKBOX_LABEL_HORIZONTAL = SUB_SUB_CHECKBOX_HORIZONTAL + 35f;
            float KEYBIND_HORIZONTAL_OFFSET = 200f;
            float NUM_INPUT_OFFSET = 60f;
            //horizontal (scroll box)
            float SCROLL_HORIZONTAL_MODIFIER = -35f;
            
            //vertical
            float CHECKBOX_VERT_OFFSET = 70f;
            float MINI_CHECKBOX_VERT_OFFSET = 50f;
            float LABEL_VERT_OFFSET = 30f;
            float SUB_CHECKBOX_VERT_OFFSET= CHECKBOX_VERT_OFFSET - 10;
            float SUB_SUB_CHECKBOX_VERT_OFFSET = CHECKBOX_VERT_OFFSET - 15;
            float KEYBIND_VERT_OFFSET = -40f;
            float SCROLL_BOTTOM_PADDING = 60f;

            float KEYBIND_HEIGHT = (KEYBIND_VERT_OFFSET * (-1)) - 20f;
            float KEYBIND_LENGTH = KEYBIND_HORIZONTAL_OFFSET - 50f;
            float IMAGE_TEXT_OFFSET = 30f;
            float CHECKBOX_IMAGE_OFFSET = -23f;
            float DESC_OFFSET = -20f;
            float VERT_POS_INIT = 550f;
            float SCROLL_POS_INIT = 505f;
            float TITLE_HORIZONTAL = 40f;

            float vert_pos = VERT_POS_INIT;

            float NUM_INPUT_SIZE = 50f;
            float SCROLL_CONTENT_SIZE = 0f;
            Vector2 SCROLL_POS = new(10f, 10f);
            Vector2 SCROLL_SIZE = new(580f, 500f);

            base.Initialize();
            OpTab opTab = new OpTab(this, "Main Config");
            OpTab opTab2 = new OpTab(this, "Keep Item Config");
            OpTab opTab3 = new OpTab(this, "Creature Immunities");
            OpTab opTab4 = new OpTab(this, "Item Passing");
            OpTab opTab5 = new OpTab(this, "Silliness");
            OpTab opTab6 = new OpTab(this, "Campaign Specific");


            //OpTab myriadItemPassing = new OpTab(this, "Myriad Item Passing");
            this.Tabs = new OpTab[]
            {
                opTab, opTab6, opTab2, opTab3, opTab4, opTab5
            };

            OpContainer tab1Container = new OpContainer(new Vector2(0, 0));
            opTab.AddItems(tab1Container);
            OpScrollBox mainConfigScroll = new OpScrollBox(SCROLL_POS, SCROLL_SIZE, SCROLL_CONTENT_SIZE, false, true, true);
            UIelement[] element = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Slugpup Safari Options", true),
                mainConfigScroll 

            };
            opTab.AddItems(element);
            vert_pos = SCROLL_POS_INIT;

            mainConfigScroll.AddItems(
                new OpCheckBox(noThrowPups, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Pup Throw Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Piggybacked slugpups must be moved to your hand in order to throw them)", false),
                new OpCheckBox(allowBackspear, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Allow Backspear", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Piggybacked slugpups will put their spear on their back)", false),
                new OpCheckBox(artificerDontLaunchKids, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Artificer Parry Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Prevent piggybacked slugpups from being launched by artificer's parry)", false),
                new OpCheckBox(pupImpactCushion, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Pup Impact Immunity", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Piggybacked slugpups are immune to fall damage)", false),
                new OpCheckBox(preventDraggingVines, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Release Vines", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Force slugpups to release vines when piggybacked)", false),
                new OpCheckBox(stunProtection, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Stun Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Slugpups won't fall off of stunned players and stunned pups can be piggybacked)", false),
                new OpCheckBox(killzoneProtection, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Killzone Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos + DESC_OFFSET, "(Piggybacked slugcats will not die if out of bounds)", false),
                new OpCheckBox(infinitePupStack, CHECKBOX_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos, "Infinite Stacking", true),
                new OpTextBox(maxStack, new Vector2(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER, vert_pos - LABEL_VERT_OFFSET), NUM_INPUT_SIZE),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL + SCROLL_HORIZONTAL_MODIFIER + NUM_INPUT_OFFSET, vert_pos - LABEL_VERT_OFFSET, "(Max if disabled)", false)
                );
            mainConfigScroll.SetContentSize(SCROLL_POS_INIT - vert_pos + SCROLL_BOTTOM_PADDING, true);
            /*UIelement[] element = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Slugpup Safari Options", true),

                new OpCheckBox(noThrowPups, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Pup Throw Protection", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Piggybacked slugpups must be moved to your hand in order to throw them)", false),
                new OpCheckBox(allowBackspear, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Backspear", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Piggybacked slugpups will put their spear on their back)", false),
                new OpCheckBox(artificerDontLaunchKids, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Artificer Parry Protection", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Prevent piggybacked slugpups from being launched by artificer's parry)", false),
                new OpCheckBox(pupImpactCushion, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Pup Impact Immunity", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Piggybacked slugpups are immune to fall damage)", false),
                new OpCheckBox(preventDraggingVines, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Release Vines", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Force slugpups to release vines when piggybacked)", false),
                new OpCheckBox(stunProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Stun Protection", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Slugpups won't fall off of stunned players and stunned pups can be piggybacked)", false),

            };*/
            /*new OpCheckBox(infinitePupStack, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
            new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Infinite Pup Stack", false),
            new OpSlider(maxPupSlider, new Vector2(CHECKBOX_HORIZONTAL + 150f, vert_pos - 5f), 300) { max = 99, hideLabel = false },
            new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "How many extra pups can be stacked? (Overriden by Infinite Pup Stack)", false),*/

            //reset vertpos
            vert_pos = VERT_POS_INIT;
            UIelement[] element2 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Slugpup Grab Options", true),
                new OpCheckBox(keepItemEnabled, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Keep Items on Pickup", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Allow slugpups to keep holding their item when picked up by the player)", false),
                // Keep items options

                new OpCheckBox(allowGrabbingFood, SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_CHECKBOX_VERT_OFFSET),
                new OpLabel(SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Food", true),
                new OpLabel(SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Allow slugpups to grab food while piggybacked)", false),
                // Grab food options
                new OpCheckBox(allowGrabbingNoodleflies, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(SUB_SUB_CHECKBOX_HORIZONTAL+CHECKBOX_IMAGE_OFFSET, vert_pos), "Kill_SmallNeedleWorm"),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Noodleflies", true),
                new OpCheckBox(allowGrabbingCentipedes, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(SUB_SUB_CHECKBOX_HORIZONTAL+CHECKBOX_IMAGE_OFFSET-5f, vert_pos), "Kill_Centipede1"),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Centipedes", true),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Slugpups cannot eat when you are moving, so be careful)", false),
                new OpCheckBox(allowGrabbingTardigrades, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Tardigrades", true),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Only Applicable if Watcher is Enabled)", false),
                new OpCheckBox(allowGrabbing5PNeurons, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(SUB_SUB_CHECKBOX_HORIZONTAL+CHECKBOX_IMAGE_OFFSET, vert_pos), "Symbol_Neuron"),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Five Pebble's Neurons", true),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Neurons outside of Five Pebbles are always forbidden)", false),
                new OpCheckBox(allowGrabbingShrooms, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(SUB_SUB_CHECKBOX_HORIZONTAL+CHECKBOX_IMAGE_OFFSET, vert_pos), "Symbol_Mushroom"),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Mushrooms", true),
                new OpCheckBox(allowGrabbingKarmaFlowers, SUB_SUB_CHECKBOX_HORIZONTAL, vert_pos -= SUB_SUB_CHECKBOX_VERT_OFFSET),
                new OpLabel(SUB_SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Grabbing Karma Flowers", true),
                // Back to other options
                new OpCheckBox(canDropForFood, SUB_CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Allow Dropping Items", true),
                new OpLabel(SUB_CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Slugpups will drop items in their hands to pick up food)", false),
            };
            opTab2.AddItems(element2);

            vert_pos = VERT_POS_INIT;
            UIelement[] element3 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Creature Protection Options", true),
                new OpCheckBox(leechProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(CHECKBOX_LABEL_HORIZONTAL+5f, vert_pos + 5f), "Kill_Leech"),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+IMAGE_TEXT_OFFSET, vert_pos, "Leech Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Leeches will not target piggybacked slugpups)", false),
                new OpCheckBox(coalescipedeProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos+ 5f), "Kill_SmallSpider"),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+IMAGE_TEXT_OFFSET, vert_pos, "Coalescipede Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Coalescipedes will not target piggybacked slugpups)", false),
                new OpCheckBox(mirosBirdProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpImage(new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos+ 5f), "Kill_MirosBird"),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+IMAGE_TEXT_OFFSET + 15f, vert_pos, "Miros Bird Protection", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Miros Birds will not bite piggybacked slugpups)", false),
            };
            opTab3.AddItems(element3);

            vert_pos = VERT_POS_INIT;
            UIelement[] element4 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Controls", true),
                new OpCheckBox(itemPassing, CHECKBOX_HORIZONTAL, vert_pos -= MINI_CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Item Passing", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(Allow passing items up and down the stack)", false),
                new OpCheckBox(playerItemMoving, CHECKBOX_HORIZONTAL, vert_pos -= MINI_CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Move Player Items", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(When disabled piggybacked players will be skipped)", false),
                new OpLabel(CHECKBOX_HORIZONTAL, vert_pos-= 2* MINI_CHECKBOX_VERT_OFFSET, "Keyboard", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos-=LABEL_VERT_OFFSET, "Pass Up", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+ KEYBIND_HORIZONTAL_OFFSET, vert_pos, "Pass Down", true),
                new OpKeyBinder(upInputKB,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.AnyController),
                new OpKeyBinder(downInputKB,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.AnyController),
                new OpLabel(CHECKBOX_HORIZONTAL, vert_pos-= MINI_CHECKBOX_VERT_OFFSET, "Controller", true),
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
            opTab4.AddItems(element4);
            vert_pos = VERT_POS_INIT;
            UIelement[] element5 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Silliness", true),
                new OpCheckBox(bigHeadMode, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Big Brain Mode", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Double's the size of your slugpup's head for maximum brainage)", false),
            };
            opTab5.AddItems(element5);
            vert_pos = VERT_POS_INIT;
            UIelement[] element6 = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Campaign Specific", true),
                new OpCheckBox(rivDrownProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Rivulet Drowning Immunity", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Slugpups cannot drown in Rivulet's campaign)", false),
                new OpCheckBox(artiBombProtection, CHECKBOX_HORIZONTAL, vert_pos -= CHECKBOX_VERT_OFFSET),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos, "Artificer Explosion Immunity", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos+DESC_OFFSET, "(Slugpups resist explosions in Artificer's campaign)", false),
            };
            opTab6.AddItems(element6);
            /*vert_pos = VERT_POS_INIT;
            UIelement[] myriadStuff = new UIelement[]
            {
                new OpLabel(TITLE_HORIZONTAL, vert_pos, "Extra Myriad Controls", true),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos + DESC_OFFSET, "(For now I am unable to add any more controllers)", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL, vert_pos-=LABEL_VERT_OFFSET, "Pass Up", false),
                new OpLabel(CHECKBOX_LABEL_HORIZONTAL+ KEYBIND_HORIZONTAL_OFFSET, vert_pos, "Pass Down", false),
                new OpKeyBinder(upInputController5,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller5),
                new OpKeyBinder(downInputController5,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller5),
                new OpKeyBinder(upInputController6,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller6),
                new OpKeyBinder(downInputController6,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller6),
                new OpKeyBinder(upInputController7,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller6),
                new OpKeyBinder(downInputController7,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller6),
                new OpKeyBinder(upInputController7,new Vector2(CHECKBOX_LABEL_HORIZONTAL, vert_pos += KEYBIND_VERT_OFFSET),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller7),
                new OpKeyBinder(downInputController7,new Vector2(CHECKBOX_LABEL_HORIZONTAL + KEYBIND_HORIZONTAL_OFFSET, vert_pos),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT),false, OpKeyBinder.BindController.Controller7),

            };
            myriadItemPassing.AddItems(myriadStuff);
*/
            //allowGrabbingFood.OnChange += foodOptionAvaliability;

        }
        /*private void foodOptionAvaliability()
        {
            if (!allowGrabbingFood.Value)
            {
                allowGrabbingNoodleflies.Value = false;
            }
        }*/
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

        /*public static Configurable<KeyCode> upInputController5;
        public static Configurable<KeyCode> downInputController5;
        public static Configurable<KeyCode> upInputController6;
        public static Configurable<KeyCode> downInputController6;
        public static Configurable<KeyCode> upInputController7;
        public static Configurable<KeyCode> downInputController7;
        public static Configurable<KeyCode> upInputController8;
        public static Configurable<KeyCode> downInputController8;*/
    }
}
