using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipCharacter : MonoBehaviour
{
    [Header("Setup the Equipment Panel")]
    public GameObject CharacterEquipPanel;
    public GameObject EquipmentSlotPanel;
    public GameObject EquipmentSlot;
    public GameObject EquipmentSlotEmpty;
    public GameObject EquipmentEquipmentButton;
    public GameObject EquipmentWeaponsButton;
    public GameObject EquipmentAccessoryButton;

    [HideInInspector] private List<GameObject> buttons = new List<GameObject>();
    [HideInInspector] private string currentType = "";
    public Sprite offButton;
    public Sprite onButton;

    [Space (20)]
    [Header("Setup the Character Panel")]

    public TextMeshProUGUI EquipmentTitle;
    public GameObject EquipmentHead;
    public GameObject EquipmentBody;
    public GameObject EquipmentLegs;
    public GameObject EquipmentFeet;
    public GameObject EquipmentAccessory1;
    public GameObject EquipmentAccessory2;
    public GameObject EquipmentWeapon;
    public Sprite EquipmentEmptyHead;
    public Sprite EquipmentEmptyBody;
    public Sprite EquipmentEmptyLegs;
    public Sprite EquipmentEmptyFeet;
    public Sprite EquipmentEmptyAccessory1;
    public Sprite EquipmentEmptyAccessory2;
    public Sprite EquipmentEmptyWeapon;
    public Text Trigger;

    [Space(20)]
    [Header("Setup the Character Stats Panel")]
    public TextMeshProUGUI EquipmentHealth;
    public TextMeshProUGUI EquipmentSpeed;
    public TextMeshProUGUI EquipmentAttack;
    public TextMeshProUGUI EquipmentDefence;
    public TextMeshProUGUI EquipmentMagic;
    public TextMeshProUGUI EquipmentResistance;
    [HideInInspector] private int CurrentCharacterID;
    public Text CharId;

    void Start()
    {
        CurrentCharacterID = int.Parse(CharId.text);
        TabButtonClick(EquipmentEquipmentButton);
    }
    //private CharacterEquipment characterEquipment = null;
    private void SetupSlots()
    {
        CurrentCharacterID = int.Parse(CharId.text);
        EquipmentTitle.text = Game.Player.Characters[CurrentCharacterID].Name + " - Equipment";
        CharacterEquipment characterEquipment = Game.Player.Characters[CurrentCharacterID].CharacterEquipment;
        if (CurrentCharacterID != -1)
        {
            EquipmentHead.GetComponent<Image>().sprite = characterEquipment.Head != null ? characterEquipment.Head.Sprite : EquipmentEmptyHead;
            EquipmentBody.GetComponent<Image>().sprite = characterEquipment.Body != null ? characterEquipment.Body.Sprite : EquipmentEmptyBody;
            EquipmentLegs.GetComponent<Image>().sprite = characterEquipment.Legs != null ? characterEquipment.Legs.Sprite : EquipmentEmptyLegs;
            EquipmentFeet.GetComponent<Image>().sprite = characterEquipment.Feet != null ? characterEquipment.Feet.Sprite : EquipmentEmptyFeet;
            EquipmentAccessory1.GetComponent<Image>().sprite = characterEquipment.Accessory1 != null ? characterEquipment.Accessory1.Sprite : EquipmentEmptyAccessory1;
            EquipmentAccessory2.GetComponent<Image>().sprite = characterEquipment.Accessory2 != null ? characterEquipment.Accessory2.Sprite : EquipmentEmptyAccessory2;
            EquipmentWeapon.GetComponent<Image>().sprite = characterEquipment.Weapon != null ? characterEquipment.Weapon.Sprite : EquipmentEmptyWeapon;

            EquipmentHead.GetComponent<EquipSlotDrop>().item = characterEquipment.Head;
            EquipmentBody.GetComponent<EquipSlotDrop>().item = characterEquipment.Body;
            EquipmentLegs.GetComponent<EquipSlotDrop>().item = characterEquipment.Legs;
            EquipmentFeet.GetComponent<EquipSlotDrop>().item = characterEquipment.Feet;
            EquipmentAccessory1.GetComponent<EquipSlotDrop>().item = characterEquipment.Accessory1;
            EquipmentAccessory2.GetComponent<EquipSlotDrop>().item = characterEquipment.Accessory2;
            EquipmentWeapon.GetComponent<EquipSlotDrop>().item = characterEquipment.Weapon;

            EquipmentHead.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentBody.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentLegs.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentFeet.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentAccessory1.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentAccessory2.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
            EquipmentWeapon.GetComponent<EquipSlotDrop>().CharacterId = CurrentCharacterID;
        }

        //RenderEquipmentInventory();

    }

    // Update is called once per frame
    void Update()
    {
        if(Trigger.text == 1.ToString())
        {
            RenderEquipmentInventory();
            Trigger.text = 0.ToString();
        }
        if (CurrentCharacterID != -1)
        {
            //this does not include Character's stats, only what is being carried by character
            EquipmentHealth.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("HP").ToString();
            EquipmentSpeed.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("SP").ToString();
            EquipmentAttack.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("ATK").ToString();
            EquipmentDefence.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("DEF").ToString();
            EquipmentMagic.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("MAG").ToString();
            EquipmentResistance.text = Game.Player.Characters[CurrentCharacterID].CharacterEquipment.GetPropertyTotal("RES").ToString();
        }

    }
    public void TabButtonClick(GameObject button)
    {
        string type = button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.ToLower();

        foreach (var item in buttons)
        {
            item.GetComponent<Image>().sprite = offButton;
            item.GetComponent<Image>().color = Color.white;
            item.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
        }
        button.GetComponent<Image>().sprite = onButton;
        button.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;

        if (currentType != type)
        {
            currentType = type;
            RenderEquipmentInventory();
        }
    }
    [HideInInspector] List<iInventory> currentListToShow = new List<iInventory>();
    [HideInInspector] public List<GameObject> equipmentSlotList = new List<GameObject>();
    [HideInInspector] private int ItemCounterWatchDog = 0;
    private void RenderEquipmentInventory()
    {
        //clear panel
        equipmentSlotList.Clear();
        foreach (Transform child in EquipmentSlotPanel.transform)
            GameObject.Destroy(child.gameObject);

        switch (currentType)
        {
            case "equipment":
                currentListToShow = Game.Player.Equipment;
                break;
            case "weapons":
                currentListToShow = Game.Player.Weapons;
                break;
            case "accessories":
                currentListToShow = Game.Player.Accessories;
                break;
            default:
                break;
        }
        
        ItemCounterWatchDog = currentListToShow.Count;
        CurrentCharacterID = int.Parse(CharId.text);
        int index = 0;
        foreach (var item in currentListToShow)
        {
            if (item.CharacterId == -1)
            {
                GameObject itemObj = Instantiate(EquipmentSlot);
                //var resource = currentListToShow.First(r => r.Class == item.Class);
                itemObj.transform.SetParent(EquipmentSlotPanel.transform);
                itemObj.name = $"[{item.ID}] -> [{item.Class}]";
                int i = index;
                //itemObj.GetComponent<Button>().onClick.AddListener(delegate { ShowDetails(i); });

                itemObj.GetComponent<EquipItem>().item = item;
                itemObj.GetComponent<EquipItem>().CurrentCharacterID = CurrentCharacterID;
                itemObj.GetComponent<EquipItem>().SlotNumber = i;
                itemObj.GetComponent<EquipItem>().OnTopPanel = EquipmentSlotPanel.transform.parent.parent.gameObject;
                var allKids = itemObj.GetComponentsInChildren<Transform>(true);
                allKids.First(k => k.name == "Icon").GetComponent<Image>().sprite = item.Sprite;
                allKids.First(k => k.name == "Name").GetComponent<TextMeshProUGUI>().text = item.Name;
                allKids.First(k => k.name == "Star").gameObject.SetActive(!item.isDefault);
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
                equipmentSlotList.Add(itemObj);
                index++;
            }
        }
        if (currentListToShow.Count == 0 || equipmentSlotList.Count == 0)
        {
            GameObject itemObj = Instantiate(EquipmentSlotEmpty);
            itemObj.transform.SetParent(EquipmentSlotPanel.transform);
            itemObj.name = "empty";
            itemObj.transform.position = Vector2.zero;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            equipmentSlotList.Add(itemObj);
        }


    }
    public void OpenEquipPanel()
    {
        buttons.Clear();
        buttons.Add(EquipmentEquipmentButton);
        buttons.Add(EquipmentWeaponsButton);
        buttons.Add(EquipmentAccessoryButton);

        TabButtonClick(EquipmentEquipmentButton);
        
        currentType = "equipment";

        SetupSlots();
        Helper.FadeIn(CharacterEquipPanel);
    }
    public void CloseEquipPanel()
    {
        Helper.FadeOut(CharacterEquipPanel);
    }

}
