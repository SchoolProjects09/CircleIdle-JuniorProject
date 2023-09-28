using CircleIdleLib;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlotDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public iInventory item;
    private Image Icon;
    public Sprite DefaultIcon;
    public string SlotType; //head, body, legs, feet, weapon, loot1, loot2
    public GameObject Unload;
    public int CharacterId;
    private void Awake()
    {
        Icon = GetComponent<Image>();
        Unload = transform.GetChild(0).gameObject;
        //item = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.GetComponent<EquipItem>().item.Protect == Regex.Replace(SlotType, @"[\d-]", string.Empty))
            {
                Debug.Log($"Item [{eventData.pointerDrag.GetComponent<EquipItem>().item.ID.ToString()}]" +
                               $"[{eventData.pointerDrag.GetComponent<EquipItem>().item.Name}] was dropped on [{SlotType}] for " +
                               $"[{eventData.pointerDrag.GetComponent<EquipItem>().CurrentCharacterID.ToString()}]");

                CharacterId = eventData.pointerDrag.GetComponent<EquipItem>().CurrentCharacterID;
                if (item != null)
                {
                    item.CharacterId = -1;
                }
                item = eventData.pointerDrag.GetComponent<EquipItem>().item;
                Icon.sprite = item.Sprite;
                
                switch (SlotType)
                {
                    case "head":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Head = item;
                        break;
                    case "body":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Body = item;
                        break;
                    case "legs":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Legs = item;
                        break;
                    case "feet":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Feet = item;
                        break;
                    case "accessory1":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Accessory1 = item;
                        break;
                    case "accessory2":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Accessory2 = item;
                        break;
                    case "weapon":
                        Game.Player.Characters[CharacterId].CharacterEquipment.Weapon = item;
                        break;
                    default:
                        break;
                }
                item.CharacterId = CharacterId;
                item.isAssigned = true;

                transform.parent.Find("trigger").GetComponent<Text>().text = 1.ToString();
                Unload.SetActive(true);
            }
            else
            {
                eventData.pointerDrag.GetComponent<EquipItem>().Highlight.color = Color.white;
            }
        }      
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.GetComponent<EquipItem>().item.Protect == Regex.Replace(SlotType, @"[\d-]", string.Empty))
            {
                eventData.pointerDrag.GetComponent<EquipItem>().Highlight.color = Color.green;
            }
            else
            {
                eventData.pointerDrag.GetComponent<EquipItem>().Highlight.color = Color.red;
            }
        }
        if (!Icon.sprite.name.Contains("empty"))
        {
            Unload.SetActive(true);
        }else
            Unload.SetActive(false);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<EquipItem>().Highlight.color = Color.white;

        Unload.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        //item = null;
    }
    public void RemoveFromCharacter()
    {
        Debug.Log($"Clicked to remove [{item.Name.ToUpper()}] from [{SlotType}]");
        item.CharacterId = -1;
        item.isAssigned = false;
        switch (SlotType)
        {
            case "head":
                //Icon.sprite = DefaultIcon;
                Game.Player.Characters[CharacterId].CharacterEquipment.Head = null;
                break;
            case "body":
                Game.Player.Characters[CharacterId].CharacterEquipment.Body = null;
                break;
            case "legs":
                Game.Player.Characters[CharacterId].CharacterEquipment.Legs = null;
                break;
            case "feet":
                Game.Player.Characters[CharacterId].CharacterEquipment.Feet = null;
                break;
            case "accessory1":
                Game.Player.Characters[CharacterId].CharacterEquipment.Accessory1 = null;
                break;
            case "accessory2":
                Game.Player.Characters[CharacterId].CharacterEquipment.Accessory2 = null;
                break;
            case "weapon":
                Game.Player.Characters[CharacterId].CharacterEquipment.Weapon = null;
                break;
            default:
                break;
        }
        Icon.sprite = DefaultIcon;

        Unload.SetActive(false);
        transform.parent.Find("trigger").GetComponent<Text>().text = 1.ToString();
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
