using UnityEngine;
using TMPro;

public class UIHelpTextPlatformBasis : MonoBehaviour
{
    [TextArea]
    [SerializeField] string mobileText, pcText;

    [SerializeField] TextMeshProUGUI helpText;

    public void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            helpText.text = mobileText;
        }
        else
        {
            helpText.text = pcText;
        }
    }
}
