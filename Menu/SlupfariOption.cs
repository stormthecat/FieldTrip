using System;
using System.Linq;
using UnityEngine;
using Menu.Remix.MixedUI;
using BepInEx;
using System.Collections.Generic;


namespace FieldTrip
{
    public partial class OptionsMenu
    {
        public class SlupfariOption<T> : SlupfariOptionInterface
        {
            Configurable<T> config;
            Type type;
            string key;
            string title;
            string desc;
            bool disableChildrenFlag;
            bool disableChildren;
            bool disabled;

            public enum childDisableStates
            {
                MANUAL,
                INVERSE,
                NONINVERSE
            }
            childDisableStates childDisableMode;
            
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
                this.childDisableMode = childDisableStates.NONINVERSE;
            }
            public T GetValue()
            {
                T value = config.Value;
                if (value is bool)
                {
                    bool val = (bool)(object)value & !this.disabled;
                    return (T)(object)val;
                }
                return value;
            }

            public Configurable<T> GetConfig()
            {
                return this.config;
            }
            public void AddText(string title, string desc = "")
            {
                this.title = title;
                this.desc = desc;
            }
            public UIelement[] OverrideElement(UIelement[] customElements)
            {
                /*****    Uiconfig is expected to go first, if you don't put it first I will tell your mom    *****/
                this.elements = customElements;
                PopulateLabelBumpBehav();
                return customElements;
            }
            public UIelement[] MakeElement(ref float vert_pos)
            {
                if (this.title is null) this.title = "";
                if (this.desc is null) this.desc = "";

                float mainVertOffset = (this.parent == null)? CHECKBOX_VERT_OFFSET_PARENT : CHECKBOX_VERT_OFFSET_CHILD;

                this.vertOffset = vert_pos;

                float horzOffset = HORIZONTAL + horiOffset;
                List<UIelement> listOfElements = new List<UIelement>();

                if (type == typeof(bool))
                {
                    Configurable<bool> option = config as Configurable<bool>;
                    horzOffset = CHECKBOX_LABEL_HORIZONTAL + horiOffset;
                    listOfElements.Add(new OpCheckBox(option, HORIZONTAL + horiOffset, vert_pos -= mainVertOffset));
                }
                else if (type == typeof(int))
                {
                    Configurable<int> option = config as Configurable<int>;
                    horzOffset = TEXTBOX_LABEL_HORIZONTAL + NUM_INPUT_SIZE + horiOffset;
                    listOfElements.Add(new OpTextBox(option, new Vector2(HORIZONTAL + horiOffset, vert_pos -= mainVertOffset), NUM_INPUT_SIZE));
                }
                listOfElements.Add(new OpLabel(horzOffset, vert_pos, this.title, true));
                if (!this.desc.IsNullOrWhiteSpace())
                {
                    OpLabel description = new OpLabel(new Vector2(horzOffset, vert_pos -= DESC_OFFSET), new Vector2(SCROLL_WIDTH - HORIZONTAL * 2 - horzOffset, 0f), this.desc, FLabelAlignment.Left, false);
                    description.autoWrap = true;
                    description.verticalAlignment = OpLabel.LabelVAlignment.Top;
                    listOfElements.Add(description);
                    vert_pos -= DESC_OFFSET * (description.GetLineCount() - 1);
                }

                elements = listOfElements.ToArray();

                PopulateLabelBumpBehav();
                return elements;
            }

            private void PopulateLabelBumpBehav()
            {
                /*****    Uiconfig is expected to go first    *****/
                if (elements == null || elements.Length == 0 || elements[0] is not UIconfig)
                    return;
                BumpBehaviour bumpBehav = (elements[0] as UIconfig).bumpBehav;
                for(int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] is OpLabel) (elements[i] as OpLabel).bumpBehav = bumpBehav;
                    elements[i].description = "OPTION_ID: " + this.key;
                }
            }

            public void SetDisableChildren(bool disableChildren)
            {
                this.disableChildren = disableChildren;
            }
            public void SetChildDisableMode(childDisableStates state)
            {
                this.childDisableMode = state;
            }
            public void Update()
            {
                string value = (elements == null) ? "" : (elements.FirstOrDefault() as UIconfig)?.value; //get boolean switch
                bool booleanVal = value != null && value == "true";

                switch (childDisableMode)
                {
                    case childDisableStates.NONINVERSE:
                        this.disableChildrenFlag = !booleanVal;
                        break;
                    case childDisableStates.INVERSE:
                        this.disableChildrenFlag = booleanVal;
                        break;
                    default:
                        this.disableChildrenFlag = this.disableChildren;
                        break;
                }

                if (this.parent != null) 
                {
                    this.disableChildrenFlag = this.disableChildrenFlag | this.parent.disableChildrenFlag;
                    this.disabled = this.parent.disableChildrenFlag;
                }
                if(elements is not null)
                {
                    foreach (UIelement element in elements)
                    {
                        UIconfig uiConfig = element as UIconfig;
                        OpLabel opLabel = element as OpLabel;
                        if (uiConfig != null) uiConfig.greyedOut = this.disabled;
                        else if (opLabel != null && opLabel.bumpBehav != null) opLabel.label.color = this.disabled ? opLabel.bumpBehav.GetColor(opLabel.color) : opLabel.color;

                    }
                }
            }

        }


    }
}
