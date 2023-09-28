using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryList : MonoBehaviour
{
    [HideInInspector] public List<GameObject> inventorySlotList = new List<GameObject>(); 
    //list of panel active building slots
    //[HideInInspector] public List<GameObject> CharacterSlotList = new List<GameObject>(); 
    //list of panel active building slots
    //Slots and panel
    [Header("Setup the Inventory Panel")]
    public GameObject inventorySlotPanel;
    public GameObject inventorySlot;
    public GameObject inventorySlotEmpty;
    public GameObject inventoryResourcesButton;
    public GameObject inventoryEquipmentButton;
    public GameObject inventoryWeaponsButton;
    public GameObject inventoryAccessoryButton;


    [HideInInspector] private List<GameObject> buttons = new List<GameObject>();
    [Space(10)]
    [Header("Setup the Sort Panel")]
    public GameObject inventorySortPanel;
    public TMP_Dropdown inventnorySortDropdown;
    public TMP_Text inventnorySortText;
    public GameObject inventorySortAscButton;
    public GameObject inventorySortDescButton;
    public Sprite offButton;
    public Sprite onButton;
    public Toggle showAssigned;
    [HideInInspector] private string currentType = "";

    [Space(10)]
    [Header("Item Panel")]
    public GameObject ItemPanel;
    public Image ItemIcon;
    public GameObject AssignedToIcon;
    public TextMeshProUGUI ItemHealth;
    public TextMeshProUGUI ItemSpeed;
    public TextMeshProUGUI ItemAttack;
    public TextMeshProUGUI ItemDefence;
    public TextMeshProUGUI ItemMagic;
    public TextMeshProUGUI ItemResistance;
    public GameObject ItemStar;
    public GameObject ItemSell;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    [HideInInspector] iInventory currentItemDisplayed;
    [Space(10)]
    [Header("Item Change Name Panel")]
    public GameObject ItemNameChangePanel;
    public TMP_InputField ItemNameChangeInput;
    [Space(10)]
    [Header("Sell Item Panel")]
    public GameObject SellItemPanel;
    public TextMeshProUGUI SellItemTitle;
    public TextMeshProUGUI SellItemOffer;


    [HideInInspector] private int ItemCounterWatchDog = 0;
    public void Sort_ASC_DESC(GameObject button)
    {
        string type = button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.ToLower();
        if (type.ToLower() == "asc")
        {
            //deselect DESC
            inventorySortDescButton.GetComponent<Image>().sprite = offButton;
            inventorySortDescButton.GetComponent<Image>().color = Color.white;
            inventorySortDescButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
        }
        else
        {
            inventorySortAscButton.GetComponent<Image>().sprite = offButton;
            inventorySortAscButton.GetComponent<Image>().color = Color.white;
            inventorySortAscButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
        }
        button.GetComponent<Image>().sprite = onButton;
        button.GetComponent<Image>().color = new Color32(135, 135, 135, 255);
        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        ExecuteSort();
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

        ItemPanel.SetActive(false);

        if (currentType != type)
        {
            currentType = type;
            isSortRequest = false; //reset
            DropdownText = "--------";
            RenderInventory();
        }
        //Debug.Log(currentType);


    }
    
    [HideInInspector] List<iInventory> currentListToShow = new List<iInventory>();
    [HideInInspector] List<iInventory> privateCurrentList = new List<iInventory>();
    [HideInInspector] bool isSortRequest = false;
    private void RenderInventory()
    {
        inventnorySortDropdown.options.Clear();

        //clear panel
        inventorySlotList.Clear();
        foreach (Transform child in inventorySlotPanel.transform)
            GameObject.Destroy(child.gameObject);

        if (!isSortRequest) //do this only when not sorted list || initial request to show the inventory
        {
            switch (currentType)
            {
                case "resources":
                    inventorySortPanel.SetActive(false);
                    RenderResources();
                    break;
                case "equipment":
                    inventorySortPanel.SetActive(true); 
                    currentListToShow = Game.Player.Equipment;
                    privateCurrentList = Game.Player.Equipment;
                    break;
                case "weapons":
                    inventorySortPanel.SetActive(true);
                    currentListToShow = Game.Player.Weapons;
                    privateCurrentList = Game.Player.Weapons;
                    break;
                case "accessories":
                    inventorySortPanel.SetActive(true);
                    currentListToShow = Game.Player.Accessories;
                    privateCurrentList = Game.Player.Accessories;
                    break;
                default:
                    break;
            }
        }
        else
        {
            GetSortedList();
        }


        if (currentType != "resources")
        {
            inventnorySortDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "--------" });


            ItemCounterWatchDog = privateCurrentList.Count;

            if (currentListToShow.Count > 0)
            {
                string[] curr = currentListToShow[0].SortCriterias();
                foreach (string item in curr)
                {
                    inventnorySortDropdown.options.Add(new TMP_Dropdown.OptionData() { text = item });
                }
            }

            inventnorySortDropdown.value = inventnorySortDropdown.options.FindIndex(x => x.text == DropdownText);

            int index = 0;
            foreach (var item in currentListToShow)
            { 
                if(item.CharacterId == -1 || showAssigned.isOn)
                {
                    GameObject itemObj = Instantiate(inventorySlot);
                    //var resource = currentListToShow.First(r => r.Class == item.Class);
                    itemObj.transform.SetParent(inventorySlotPanel.transform);
                    itemObj.name = $"[{item.ID}] -> [{item.Class}]";
                    int i = index;
                    itemObj.GetComponent<Button>().onClick.AddListener(delegate { ShowDetails(item.ID); });

                    itemObj.GetComponent<InvenrotyItem>().item = item;
                    itemObj.GetComponent<InvenrotyItem>().id = item.ID; //has to match index in Player's iInventory Item after sort is complete

                    itemObj.GetComponent<InvenrotyItem>().currentTab = currentType;

                    //Cannot Combine Accessories
                    if (currentType == "accessories")
                        itemObj.GetComponent<InvenrotyItem>().enabled = false;

                    var allKids = itemObj.GetComponentsInChildren<Transform>(true);
                    allKids.First(k => k.name == "Icon").GetComponent<Image>().sprite = item.Sprite;
                    allKids.First(k => k.name == "Name").GetComponent<TextMeshProUGUI>().text = item.Name;
                    allKids.First(k => k.name == "Qty").GetComponent<TextMeshProUGUI>().text = "";
                    allKids.First(k => k.name == "Star").gameObject.SetActive(!item.isDefault);
                    itemObj.transform.position = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
                    inventorySlotList.Add(itemObj);
                }
                index++;
            }
            if (inventorySlotList.Count == 0)
            {
                GameObject itemObj = Instantiate(inventorySlotEmpty);
                itemObj.transform.SetParent(inventorySlotPanel.transform);
                itemObj.name = "empty";
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
                inventorySlotList.Add(itemObj);

            }
        } 
    }
    private PropertyInfo[] props;
    private void RenderResources()
    {
        foreach (PropertyInfo item in props)
        {
            GameObject itemObj = Instantiate(inventorySlot);
            var resource = Game.AllResources.First(r => r.Class == item.Name.ToLower());
            itemObj.transform.SetParent(inventorySlotPanel.transform);
            itemObj.name = item.Name; //  resource.Class;
            itemObj.GetComponent<InvenrotyItem>().enabled = false;
            var allKids = itemObj.GetComponentsInChildren<Transform>();
            allKids.First(k => k.name == "Icon").GetComponent<Image>().sprite = resource.Sprite;
            allKids.First(k => k.name == "Name").GetComponent<TextMeshProUGUI>().text = resource.Name;
            allKids.First(k => k.name == "Qty").GetComponent<TextMeshProUGUI>().text = item.GetValue(Game.Player.Resources, null).ToString();
            itemObj.transform.position = Vector2.zero;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            inventorySlotList.Add(itemObj);
        }
    }
    private void ShowDetails(int i)
    {
        Debug.Log($"Show me [{i.ToString()}] item");
        currentItemDisplayed = inventorySlotList.First(d => d.GetComponent<InvenrotyItem>().item.ID == i).GetComponent<InvenrotyItem>().item;

        if (currentItemDisplayed.CharacterId != -1)
        {
            AssignedToIcon.GetComponent<Image>().sprite = Game.Player.Characters[currentItemDisplayed.CharacterId].Sprite;
            AssignedToIcon.SetActive(true);
            ItemSell.SetActive(false);
        }
        else
        {
            ItemSell.SetActive(true);
            AssignedToIcon.SetActive(false);
        }
            
        ItemIcon.sprite = currentItemDisplayed.Sprite;
        ItemHealth.text = currentItemDisplayed.HP.ToString();
        ItemSpeed.text = currentItemDisplayed.SP.ToString();
        ItemAttack.text = currentItemDisplayed.ATK.ToString();
        ItemDefence.text = currentItemDisplayed.DEF.ToString();
        ItemMagic.text = currentItemDisplayed.MAG.ToString();
        ItemResistance.text = currentItemDisplayed.RES.ToString();
        ItemName.text = currentItemDisplayed.Name;
        ItemStar.SetActive(!currentItemDisplayed.isDefault);
        ItemDescription.text = currentItemDisplayed.Description;

        ItemPanel.SetActive(true);

    }
    private int calcOffer = 0;
    public void OpenSellItemPanel()
    {
        Helper.FadeIn(SellItemPanel);
        SellItemTitle.text = currentItemDisplayed.Name;
        calcOffer = (currentItemDisplayed.HP + currentItemDisplayed.ATK + currentItemDisplayed.SP + currentItemDisplayed.DEF + currentItemDisplayed.MAG + currentItemDisplayed.RES);
        SellItemOffer.text = $"{calcOffer} GOLD";
    }
    public void SellItem()
    {
        Game.Player.Resources.Gold += calcOffer;
        int x = -1;
        switch (currentType)
        {
            case "equipment":
                x = Game.Player.Equipment.FindLastIndex(i => i.ID == currentItemDisplayed.ID);
                Game.Player.Equipment.RemoveAt(currentItemDisplayed.ID);
                break;
            case "weapons":
                x = Game.Player.Weapons.FindLastIndex(i => i.ID == currentItemDisplayed.ID);
                Game.Player.Weapons.RemoveAt(currentItemDisplayed.ID);
                break;
            case "accessories":
                x = Game.Player.Accessories.FindLastIndex(i => i.ID == currentItemDisplayed.ID);
                Game.Player.Accessories.RemoveAt(x);
                break;
            default:
                break;
        }
        ItemPanel.SetActive(false);
        RenderInventory();
    }
    public void AcceptSellItemPanel_Response()
    {
        SellItem();
        CloseSellItemPanel();
    }
    public void DeclineSellItemPanel_Response()
    {
        calcOffer = 0;
        CloseSellItemPanel();
    }
    public void CloseSellItemPanel()
    {
        Helper.FadeOut(SellItemPanel);
    }
    void Start()
    {
        buttons.Add(inventoryResourcesButton);
        buttons.Add(inventoryEquipmentButton);
        buttons.Add(inventoryWeaponsButton);
        buttons.Add(inventoryAccessoryButton);

        currentType = "resources";
        props = Game.Player?.Resources.GetType().GetProperties();
        RenderInventory();
    }
    public void ItemChangeNameShowPanel()
    {
        ItemNameChangeInput.text = currentItemDisplayed.Name;
        Helper.FadeIn(ItemNameChangePanel);
    }
    public void ItemChangeNameClosePanel()
    {
        Helper.FadeOut(ItemNameChangePanel);

    }
    public void ItemChangeNameButtonClick()
    {
        currentItemDisplayed.Name = ItemNameChangeInput.text;
        ItemName.text = currentItemDisplayed.Name;
        RenderInventory();
        ItemChangeNameClosePanel();

    }
    public void CheckboxChanged()
    {
        ItemPanel.SetActive(false);
        RenderInventory(); 
        ExecuteSort();
    }

    public void DropdownSort_Changed()
    {
        ExecuteSort();

    }
    [HideInInspector] private string DropdownText = "";
    private void ExecuteSort() {

        DropdownText = inventnorySortDropdown.options[inventnorySortDropdown.value].text;
        if (!string.IsNullOrEmpty(DropdownText.Replace("-", "")))
        {
            //GetSortedList();
            isSortRequest = true;
            RenderInventory();
        }
        else
        {
            Debug.Log($"Sort by Criteria for [{currentType}] is not set.");
        }

    }

    private void GetSortedList() {

        bool isASC = (inventorySortAscButton.GetComponent<Image>().sprite == onButton);
        bool isAssigned = showAssigned.isOn;
        //Debug.Log($"Ready to sort [{currentType}] list by [{DropdownText}] in [{(isASC ? "ASC" : "DESC")}] order. Also make sure to {(isAssigned ? "" : "[not]")} show Assigned.");
        
        currentListToShow = Game.Player.SortList(currentType, privateCurrentList[0].GetSortProperty(DropdownText), isASC);
        isSortRequest = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Game.Player != null)
        {
            if (currentType == "resources")
            {
                //Debug.Log(inventorySlotList.Count);
                foreach (PropertyInfo item in props)
                {
                    inventorySlotList.First(g => g.name == item.Name).transform.Find("Qty").GetComponent<TextMeshProUGUI>().text = item.GetValue(Game.Player.Resources, null).ToString();
                }
            }
            else
            {
                if (ItemCounterWatchDog != privateCurrentList.Count)
                {//privateCurrentList reads directly from Player's lists

                    RenderInventory();
                }
            }
        }
    }
}

