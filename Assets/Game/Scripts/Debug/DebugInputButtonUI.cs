using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EditYourNameSpace
{
    public class DebugInputButtonUI : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] TextMeshProUGUI textButton;
        [SerializeField] TMP_InputField inputField;

        public void Init(string buttonName, UnityAction<string> finishEditCallback, UnityAction buttonCallback, TMP_InputField.ContentType contentType)
        {
            inputField.contentType = contentType;
            inputField.onEndEdit.AddListener(finishEditCallback);

            button.name = buttonName;
            button.onClick.AddListener(buttonCallback);
            textButton.SetText(buttonName);
        }
    }

}

