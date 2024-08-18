using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

namespace snorri
{
    public class LayoutInputFieldModule : Module
    {
        TMP_InputField inputField;
        TextMeshProUGUI placeholderText;
        TextMeshProUGUI textComponent;

        bool isSetGameVariable;
        bool isCharLimit;
        int charLimit;

        int index;
        public int ID {
            get {return index;}
        }
        
        
        string gameVariableName;

        public string Text {
            get { return inputField.text; }
            set { inputField.text = value; }
        }

        public Color TextColor {
            get { return textComponent.color; }
            set { textComponent.color = value; }
        }

        public Color PlaceholderColor {
            get { return placeholderText.color; }
            set { placeholderText.color = value; }
        }

        public string PlaceholderText {
            get { return placeholderText.text; }
            set { placeholderText.text = value; }
        }

        public TMP_FontAsset Font {
            get { return textComponent.font; }
            set {
                textComponent.font = value;
                placeholderText.font = value;
            }
        }

        protected override void AddClasses()
        {
            base.AddClasses();
            inputField = ComponentCheck<TMP_InputField>();
            textComponent = inputField.textComponent as TextMeshProUGUI;
            placeholderText = inputField.placeholder as TextMeshProUGUI;
        }

        protected override void Setup()
        {
            base.Setup();

            index = Vars.Get<int>("index", 0);

            isCharLimit = Vars.Get<bool>("is_char_limit", false);
            charLimit = Vars.Get<int>("char_limit", 1000);

            isSetGameVariable = Vars.Get<bool>("is_game_var", false);
            gameVariableName = Vars.Get<string>("game_var", "");

            TMP_FontAsset fontAsset = RESOURCES.Load<TMP_FontAsset>("fonts/" + Vars.Get<string>("font_name", "comic_land"));
            Font = fontAsset;

            placeholderText.text = Vars.Get<string>("placeholder_text", "Enter text...");
            inputField.text = Vars.Get<string>("text", "");
            
            textComponent.fontSize = Vars.Get<int>("size", 12);
            placeholderText.fontSize = Vars.Get<int>("size", 12);

            switch (Vars.Get<string>("alignment", "center"))
            {
                case "center":
                    textComponent.alignment = TextAlignmentOptions.Center;
                    placeholderText.alignment = TextAlignmentOptions.Center;
                    break;
                case "left":
                    textComponent.alignment = TextAlignmentOptions.Left;
                    placeholderText.alignment = TextAlignmentOptions.Left;
                    break;
                case "right":
                    textComponent.alignment = TextAlignmentOptions.Right;
                    placeholderText.alignment = TextAlignmentOptions.Right;
                    break;
                case "justified":
                    textComponent.alignment = TextAlignmentOptions.Justified;
                    placeholderText.alignment = TextAlignmentOptions.Justified;
                    break;
                default:
                    textComponent.alignment = TextAlignmentOptions.Left;
                    placeholderText.alignment = TextAlignmentOptions.Left;
                    break;
            }

            inputField.readOnly = Vars.Get<bool>("read_only", false);

            if (Vars.Has("text_color")) {
                TextColor = UTIL.GetColorFromHex(Vars.Get<string>("text_color"));
            }

            if (Vars.Has("placeholder_color")) {
                PlaceholderColor = UTIL.GetColorFromHex(Vars.Get<string>("placeholder_color"));
            }

            inputField.characterLimit = Vars.Get<int>("character_limit", 0);

            inputField.contentType = (TMP_InputField.ContentType)System.Enum.Parse(typeof(TMP_InputField.ContentType), Vars.Get<string>("content_type", "Standard"));

            inputField.lineType = (TMP_InputField.LineType)System.Enum.Parse(typeof(TMP_InputField.LineType), Vars.Get<string>("line_type", "SingleLine"));

            inputField.keyboardType = (TouchScreenKeyboardType)System.Enum.Parse(typeof(TouchScreenKeyboardType), Vars.Get<string>("keyboard_type", "Default"));

            inputField.characterValidation = (TMP_InputField.CharacterValidation)System.Enum.Parse(typeof(TMP_InputField.CharacterValidation), Vars.Get<string>("character_validation", "None"));

            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = Vars.Get<string>("placeholder_text", "Enter text...");

            // Register the callback for value change
            inputField.onValueChanged.AddListener(OnInputFieldChanged);
        }

        protected override void Launch()
        {
            base.Launch();

            //AccessCaretObject();
        }
        protected override void WhenFirstFrame() {
            base.WhenFirstFrame();

            AccessCaretObject();

            if (isSetGameVariable) {
                GAME.Vars.Set<string>(gameVariableName, inputField.text);
            }
        }
        private void OnInputFieldChanged(string newValue)
        {
            // Handle the value change here
            LOG.Console("Input field value changed: " + newValue);

            Map args = new Map();
            args.Set<string>("text", newValue);
            this.InvokeTrigger("when_input_change", args);

            char[] chars = newValue.ToCharArray();
            string s = "";
            if (isCharLimit && chars.Length > charLimit) {
                int k = 0;
                foreach (char c in chars) {
                    if (k >= charLimit) {
                        break;
                    }

                    s += c;
                    
                    k++;
                }
                newValue = s;

                this.inputField.text = newValue;
            }

            if (isSetGameVariable) {
                GAME.Vars.Set<string>(gameVariableName, newValue);
            }
        }
        public void ActivateInputField(bool isActive = true)
        {
            LOG.Console("input field activate: " + isActive.ToString());

            // Enable interactivity
            inputField.interactable = isActive;
            inputField.enabled = isActive;

            // Activate the input field
            if (isActive) {
                inputField.Select();
                inputField.ActivateInputField();
            }   
        }
        private void AccessCaretObject()
        {
            Transform caret = Node.Entity.FindDeepChild(Node.transform, "Caret");
            if (caret != null) {
                LOG.Console("layout input field module found caret! ");

                TMP_SelectionCaret selectionCaret = caret.gameObject.GetComponent<TMP_SelectionCaret>();
                if (selectionCaret != null)
                    selectionCaret.raycastTarget = false;
            } else {
                LOG.Console("layout input field module did not find caret!");
            }
        }


    }
}
