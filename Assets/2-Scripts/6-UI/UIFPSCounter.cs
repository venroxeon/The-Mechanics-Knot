using System.Collections;
using TMPro;
using UnityEngine;

public class UIFPSCounter : MonoBehaviour
{
    [SerializeField] float updateFreq = 0.2f;
    [SerializeField] TextMeshProUGUI counter;

    private float updateTimer;
    
    void Start()
    {
        updateTimer = updateFreq;
    }

    void Update()
    {
        updateTimer -= Time.deltaTime;

        if(updateTimer <= 0f)
        {
            var fps = 1f / Time.unscaledDeltaTime;
            counter.text = "FPS: " + Mathf.Round(fps);
            updateTimer = updateFreq;
        }
    }
}