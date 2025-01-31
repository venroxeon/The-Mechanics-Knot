using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputSystemManager : MonoBehaviour
{
    public static InputSystemManager Instance;

    [HideInInspector] public bool isFirstTouchPress, isSecondTouchPress;

    bool eventNotNull;
    int UI_Layer;
    TouchInput touchInput;

    public void OnDisable()
    {
        touchInput.Disable();

        touchInput.TouchInputs.FirstTouchPress.started -= ctx => SetTouchPress(0, true);
        touchInput.TouchInputs.FirstTouchPress.canceled -= ctx => SetTouchPress(0, false);

        touchInput.TouchInputs.SecondTouchPress.started -= ctx => SetTouchPress(1, true);
        touchInput.TouchInputs.SecondTouchPress.canceled -= ctx => SetTouchPress(1, false);
    }

    public void OnEnable()
    {
        touchInput = new();
        touchInput.Enable();

        touchInput.TouchInputs.FirstTouchPress.started += ctx => SetTouchPress(0, true);
        touchInput.TouchInputs.FirstTouchPress.canceled += ctx => SetTouchPress(0, false);

        touchInput.TouchInputs.SecondTouchPress.started += ctx => SetTouchPress(1, true);
        touchInput.TouchInputs.SecondTouchPress.canceled += ctx => SetTouchPress(1, false);
    }

    public void SetTouchPress(int touchIndex, bool check)
    {
        if (touchIndex == 0)
            isFirstTouchPress = check;
        else if (touchIndex == 1)
            isSecondTouchPress = check;
    }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        eventNotNull = EventSystem.current;
        UI_Layer = LayerMask.NameToLayer("UI");
    }
    
    public void Update()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            isFirstTouchPress = Input.GetMouseButton(0);
            isSecondTouchPress = Input.GetMouseButton(1) && isFirstTouchPress;
        }

        if (isFirstTouchPress)
        {
            if (eventNotNull && EventSystem.current.currentSelectedGameObject != null)
            {
                GameObject obj;
                obj = EventSystem.current.currentSelectedGameObject;

                if (obj.layer == UI_Layer)
                {
                    isFirstTouchPress = false;
                    isSecondTouchPress = false;
                }
            }
        }
    }

    public float3 GetFirstTouchPos()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }


        Vector3 pos = touchInput.TouchInputs.FirstTouchPos.ReadValue<Vector2>();
        
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 0;
        
        return pos;
    }

    public float3 GetSecondTouchPos()
    {
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }
        
        Vector3 pos = touchInput.TouchInputs.SecondTouchPos.ReadValue<Vector2>();
        
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 0;
        
        return pos;
    }
}