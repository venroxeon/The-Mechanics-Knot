using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtilityListButtonCallBacks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public event Action onUtilityButtonDragged, onUtilityButtonReleased;
    private bool isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(onUtilityButtonReleased != null)
            onUtilityButtonReleased();
        
        isPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPressed)
        {
            if (onUtilityButtonDragged != null)
                onUtilityButtonDragged();
        }
    }
}