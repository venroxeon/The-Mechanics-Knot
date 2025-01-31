using TMPro;
using UnityEngine;

public class TestTextSingleton : MonoBehaviour
{
    public static TestTextSingleton Instance;
    public TextMeshProUGUI text;
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        text = GetComponent<TextMeshProUGUI>();
    }
}
