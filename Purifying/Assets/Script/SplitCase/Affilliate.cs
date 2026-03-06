using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Affilliate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FunctionToggle functionToggle;
    bool isOnAffiliate = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnAffiliate = false;
        functionToggle.OnMouseEnterAffiliate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOnAffiliate = true;
        functionToggle.OnMouseEnterAffiliate(false);
    }

    public bool IsOnAffiliate()
    {
        return isOnAffiliate;
    }


}
