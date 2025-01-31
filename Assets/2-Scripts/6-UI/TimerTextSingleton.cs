using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerTextSingleton : MonoBehaviour
{
    public static TimerTextSingleton Instance;

    [HideInInspector] public TextMeshProUGUI textMesh;

    void Awake()
    {
        if(Instance == null)
            Instance = this;

        textMesh = GetComponent<TextMeshProUGUI>(); 
    }
}
