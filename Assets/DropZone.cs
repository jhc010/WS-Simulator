using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool drop;
    public List<GameObject> list;

    /// <summary>
    /// Parent class for zones with drag and drop functionality
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
        {
            drop = false;
        }
        else
        {
            drop = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
        {
            drop = false;
        }
        else
        {
            drop = true;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (!drop)
        {
            return;
        }
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        list.Add(eventData.pointerDrag.gameObject);
        d.parentToRet = this.transform;
    }
}
