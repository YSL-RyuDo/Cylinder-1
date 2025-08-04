using UnityEngine;
using UnityEngine.EventSystems;

public class HoverActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("마우스 올렸을 때 활성화할 오브젝트")]
    public GameObject targetObject;
    public GameObject offTargetObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(true);
        if (offTargetObject != null)
            offTargetObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(false);
        if (offTargetObject != null)
            offTargetObject.SetActive(true);
    }
}
