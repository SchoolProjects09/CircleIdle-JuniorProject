using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public iInventory item;
    private CanvasGroup canvasGroup;
    //public string currentTab;
    //private RectTransform rectTransform;
    public Image Highlight;
    public int SlotNumber;
    public int CurrentCharacterID;
    public GameObject OnTopPanel;
    public GameObject CurrentPanel;

    private Vector3 startPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        //rectTransform = GetComponent<RectTransform>();
        Highlight = GetComponent<Image>();
       
    }

    private void Start()
    { 
        CurrentPanel = transform.parent.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .5f;
        startPosition = transform.position;
        transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
        transform.SetParent(OnTopPanel.transform);
        //transform.SetAsLastSibling();
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;
        transform.SetParent(CurrentPanel.transform);
        transform.position = startPosition;
        
        transform.SetSiblingIndex(SlotNumber);

        transform.parent.GetComponent<GridLayoutGroup>().enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight.color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
