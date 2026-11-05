using UnityEngine;
using Menu.Remix.MixedUI;


namespace FieldTrip
{
    public partial class OptionsMenu
    {
        public class SlupfariKeyBind
        {
            string title;
            SlupfariOption<KeyCode> keyboard;
            SlupfariOption<KeyCode> controller1;
            SlupfariOption<KeyCode> controller2;
            SlupfariOption<KeyCode> controller3;
            SlupfariOption<KeyCode> controller4;


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
                this.keyboard = keyboard;
                this.controller1 = controller1;
                this.controller2 = controller2;
                this.controller3 = controller3;
                this.controller4 = controller4;
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
                float vertOffset = 0;

                //constant offsets
                float descHorzOffset = horzOffset + KEYBIND_DESC_HORIZONTAL_OFFSET;
                float keyHorzOffset = KEYBIND_HORIZONTAL_OFFSET + horzOffset;
                float descVertOffset = vert_pos + KEYBIND_DESC_VERTICAL_OFFSET;
                elements = 
                [
                    ..new UIelement[] {new OpLabel(HORIZONTAL + horzOffset, vert_pos + (vertOffset -= KEYBIND_PARAGRAPH_VERT_OFFSET), title, true) },

                    ..keyboard.OverrideElement([
                        new OpKeyBinder(keyboard.GetConfig(),new Vector2(keyHorzOffset, vert_pos + (vertOffset -= KEYBIND_VERT_OFFSET)),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT), false, OpKeyBinder.BindController.AnyController),
                        new OpLabel(descHorzOffset, descVertOffset + vertOffset, "Keyboard", false)
                        ]),

                    ..controller1.OverrideElement([
                        new OpKeyBinder(controller1.GetConfig(),new Vector2(keyHorzOffset, vert_pos + (vertOffset -= KEYBIND_VERT_OFFSET)),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT), false, OpKeyBinder.BindController.Controller1),
                        new OpLabel(descHorzOffset, descVertOffset + vertOffset, "Controller 1", false),
                        ]),

                    ..controller2.OverrideElement([
                        new OpKeyBinder(controller2.GetConfig(),new Vector2(keyHorzOffset, vert_pos + (vertOffset -= KEYBIND_VERT_OFFSET)),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT), false, OpKeyBinder.BindController.Controller2),
                        new OpLabel(descHorzOffset, descVertOffset + vertOffset, "Controller 2", false),
                        ]),

                     ..controller3.OverrideElement([
                        new OpKeyBinder(controller3.GetConfig(),new Vector2(keyHorzOffset, vert_pos + (vertOffset -= KEYBIND_VERT_OFFSET)),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT), false, OpKeyBinder.BindController.Controller3),
                        new OpLabel(descHorzOffset, descVertOffset + vertOffset, "Controller 3", false),
                        ]),

                    ..controller4.OverrideElement([
                        new OpKeyBinder(controller4.GetConfig(),new Vector2(keyHorzOffset, vert_pos + (vertOffset -= KEYBIND_VERT_OFFSET)),new Vector2(KEYBIND_LENGTH,KEYBIND_HEIGHT), false, OpKeyBinder.BindController.Controller4),
                        new OpLabel(descHorzOffset, descVertOffset + vertOffset, "Controller 4", false),
                        ]),
                ];
                if (!preserveVertPos) vert_pos += vertOffset;
                return elements;

            }
        }


    }
}
