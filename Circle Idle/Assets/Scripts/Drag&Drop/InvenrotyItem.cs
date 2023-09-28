using CircleIdleLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenrotyItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public iInventory item;
    public int id;
    public string currentTab;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image Highlight;

    Vector3 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        Highlight = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;
        startPosition = transform.position;
        transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
        transform.SetAsLastSibling();
        
        //Debug.Log($"Start Drag [{item.Name}]");
    }
    public void OnDrag(PointerEventData eventData)
    {
       
        transform.position = Input.mousePosition;
        //Debug.Log($"Dragging [{item.Name}]");
     }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        transform.position = startPosition;
        transform.SetSiblingIndex(id);
        transform.parent.GetComponent<GridLayoutGroup>().enabled = true;

    }
    //https://www.youtube.com/watch?v=BGr-7GZJNXg
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Item [{eventData.pointerDrag.GetComponent<InvenrotyItem>().id.ToString()}]" +
                       $"[{eventData.pointerDrag.GetComponent<InvenrotyItem>().item.Name}] was dropped on [{id}][{item.Name}]");
        if (eventData.pointerDrag != null)
        {
            if (item.Class == eventData.pointerDrag.GetComponent<InvenrotyItem>().item.Class)
            {    //If both items are assigned should not combine, only one  can be assigned
                if (eventData.pointerDrag.GetComponent<InvenrotyItem>().item.CharacterId == -1 || item.CharacterId == -1)
                {
                    Game.Player.CombineTwoItems(currentTab, eventData.pointerDrag.GetComponent<InvenrotyItem>().id, id);
                }
                else
                    NotificationManager.Instance.Log($"Item [{eventData.pointerDrag.GetComponent<InvenrotyItem>().item.Name}] cannot be combined with [{item.Name}] " +
                        $"because both of them are assigned to characters");
                    
            }
        }
    }
    //https://www.youtube.com/watch?v=Mb2oua3FjZg
    public void OnPointerDown(PointerEventData eventData)
    {
        //rectTransform.SetAsLastSibling();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (item.Class == eventData.pointerDrag.GetComponent<InvenrotyItem>().item.Class)
            {
                Highlight.color = Color.green;
            }
            else
            {
                Highlight.color = Color.red;
            }
        }
        //Debug.Log($"Entering private space of [{item.Name}]");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight.color = Color.white;
        
        //Debug.Log($"Left private space of [{item.Name}]");
    }
}
