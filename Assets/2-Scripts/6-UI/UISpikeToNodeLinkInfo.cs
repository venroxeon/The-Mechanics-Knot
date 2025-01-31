using UnityEngine;
using UnityEngine.EventSystems;

public class UISpikeToNodeLinkInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject infoObj;

    public void Start()
    {
        infoObj.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        infoObj.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        infoObj.SetActive(false);
    }
}