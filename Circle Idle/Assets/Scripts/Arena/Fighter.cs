using CircleIdleLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Fighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Character fighter;
    public Sprite defaultSprite;
    public GameObject Unload;
    public Text RenderTrigger;
    public bool ShowRemove;
    public bool isDone;

    private void Awake()
    {
        defaultSprite = GetComponent<Image>().sprite;
        Unload = transform.GetChild(4).gameObject;
        Unload.GetComponent<Button>().onClick.AddListener(delegate { RemoveFromCharacter(); });
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Unload.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (fighter == null || ShowRemove == false)
        {
            Unload.SetActive(false);
        }
        else
            Unload.SetActive(true);
    }
    public void RemoveFromCharacter()
    {
        fighter.IsBusy = false;
        fighter = null;
        RenderTrigger.text = 1.ToString();
        Unload.SetActive(false);
    }
    

    // Update is called once per frame
    void Update()
    {
    }
}
