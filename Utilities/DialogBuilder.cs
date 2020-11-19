namespace Evzer.Utilities
{
    public class DialogBuilder
    {
        public string Result { get; set; } = "";
        public DialogBuilder AddButton(string name, string text)
        {
            Result += "\nadd_button|" + name + "|" + text + "|noflags|0|0|";
            return this;
        }
        public DialogBuilder AddEmbedData(string embedName, object value)
        {
            Result += "\nembed_data|" + embedName + "|" + value;
            return this;
        }
        public DialogBuilder AddTextBox(string text)
        {
            Result += "\nadd_textbox|" + text + "|";
            return this;
        }
        public DialogBuilder AddPlayerPicker(string name, string text)
        {
            Result += "\nadd_player_picker|" + name + "|" + text + "|";
            return this;
        }
        public DialogBuilder AddCheckBox(string name, string text, bool isChecked)
        {
            string s = isChecked ? "1" : "0";
            Result += "\nadd_checkbox|" + name + "|" + text + "|" + s + "|";
            return this;
        }
        public void Clear() => Result = "";
        public DialogBuilder AddTextInput(string name, string text, string textInside, int length)
        {
            Result += "\nadd_text_input|" + name + "|" + text + "|" + textInside + "|" + length + "|";
            return this;
        }

        public DialogBuilder AddPlayerInfo(string name, int level, int xp, int maxXp)
        {
            Result += "\nadd_player_info|" + name + "|" + level + "|" + xp + "|" + maxXp;
            return this;
        }

        public DialogBuilder AddQuickExit()
        {
            Result += "\nadd_quick_exit|";
            return this;
        }

        public DialogBuilder EndDialog(string DialogName, string cancelText, string okText)
        {
            Result += "\nend_dialog|" + DialogName + "|" + cancelText + "|" + okText + "|";
            return this;
        }
        public DialogBuilder AddSmallLabel(string text)
        {
            Result += "\nadd_label|small|" + text + "|left|0|";
            return this;
        }
        public DialogBuilder AddBigLabel(string text)
        {
            Result += "\nadd_label|big|" + text + "|left|0|";
            return this;
        }
        public DialogBuilder AddSmallSpacer()
        {
            Result += "\nadd_spacer|small|";
            return this;
        }
        public DialogBuilder AddBigSpacer()
        {
            Result += "\nadd_spacer|big|";
            return this;
        }
        public DialogBuilder AddLabelWithIcon(string text, int itemsId, bool small)
        {
            if (small)
            {
                Result += "\nadd_label_with_icon|small|" + text + "|left|" + itemsId + "|";
            }
            else
            {
                Result += "\nadd_label_with_icon|big|" + text + "|left|" + itemsId + "|";
            }
            return this;
        }
        public DialogBuilder SetDefaultColor()
        {
            Result += "set_default_color|`o";
            return this;
        }
        public DialogBuilder AddButtonWithIcon(string buttonName, string text, int itemsId)
        {
            Result += "\nadd_button_with_icon|" + buttonName + "|" + text + "|left|" + itemsId + "||";
            return this;
        }

        public DialogBuilder AddItemPicker(string name, string text, string textInFloating)
        {
            Result += "\nadd_item_picker|" + name + "|" + text + "|" + textInFloating + "|";
            return this;
        }

        public DialogBuilder AddLabelWithIconButtonList(string textFormat, string name, string dialogFormat, string list, bool small)
        {
            Result += $"\nadd_label_with_icon_button_list|{(small ? "small" : "big")}|{textFormat}|left|{name}|{dialogFormat}|{list}";
            return this;
        }

        public DialogBuilder AddSmallText(string text)
        {
            Result += "\nadd_smalltext|" + text + "|left|";
            return this;
        }
    }
}
