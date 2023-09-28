using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingList : MonoBehaviour
{

    [HideInInspector] public int SlotAmount;   //how many buildings player is allowed to have
    //[HideInInspector] public List<Building> localBuildings = new List<Building>(); //not used
    [HideInInspector] public List<GameObject> buildingSlotList = new List<GameObject>(); //list of panel active building slots
    [HideInInspector] public List<GameObject> CharacterSlotList = new List<GameObject>(); //list of panel active building slots

    //Slots and panel
    [Header("Setup the Building Panel")]
    public GameObject buildingSlotPanel;
    public GameObject buildingSlot;
    public GameObject buildingEMPTYSlot;
    public GameObject BuildingConstructionSlot;
    public GameObject buildingAssignedCharactorSlot;
    [Space(10)]

    [Header("Setup the Assign Character Menu ")]
    public GameObject AssignCharacterPanel;
    public GameObject AssignCharacterScrollPanel;
    public GameObject AssignCharacterSlot;
    public TextMeshProUGUI AssignCharacterName;
    public TextMeshProUGUI AssignCharacterStats;
    public Text AssignCharacterHiddenID;
    public Text AssignBuildingHiddenID;
    public Button AssignCharacterSelect;
    public Button AssignCharacterClosePanel;
    [Space(10)]

    [Header("Setup New Building Menu ")]
    public GameObject NewBuildingPanelMain;
    public GameObject NewBuildingPanel;
    public GameObject NewBuildingSlot;
    public Button NewBuildingSelectButton;
    public Text NewBuildingSlotHiddenID;
    public Text NewBuildingSelectedHiddenID;
    [HideInInspector] private List<GameObject> NewBuildingList = new List<GameObject>();

    [Space(10)]

    [Header("Setup Nuke item Menu ")]
    public GameObject NukePanel;
    public Text NukeHiddenClass;
    public Text NukeHiddenID;
    public Image NukeSprite;
    [Space(10)]

    [Header("Setup Nuke item Menu ")]
    public GameObject SelectCraftMainPanel;
    public Text SelectCraftHiddenID;
    public Text SelectCraftHiddenType;
    public GameObject SelectCraftPanel;
    public GameObject SelectCraftSlot;
    public GameObject SelectCraftCostPanel;
    public GameObject SelectCraftItemCostSlot;
    public GameObject SelectCraftQueuePanel;
    public GameObject SelectCraftQueueSlot;
    public TextMeshProUGUI SelectCraftName;
    public TMP_InputField SelectCraftQuantity;
    public Toggle SelectCraftLimit;
    public GameObject SelectCraftTogglePanel;
    public Button SelectCraftAddQueue;
    public Button SelectCraftDone;
    [Space(10)]

    [Header("Setup Upgrade Building Menu")]
    public GameObject UpgradeBuildingPanel;
    public Image Material1;
    public Image Material2;
    public Image Material3;
    public Image Material4;
    public TextMeshProUGUI Material1Name;
    public TextMeshProUGUI Material2Name;
    public TextMeshProUGUI Material3Name;
    public TextMeshProUGUI Material4Name;
    public TextMeshProUGUI Cost1;
    public TextMeshProUGUI Cost2;
    public TextMeshProUGUI Cost3;
    public TextMeshProUGUI Cost4;
    public TextMeshProUGUI Current1;
    public TextMeshProUGUI Current2;
    public TextMeshProUGUI Current3;
    public TextMeshProUGUI Current4;
    public Button UpgradeButton;
    [HideInInspector] public int UpgradeBuildingID; //Keep track of the building ID so current resources can update each frame
    [HideInInspector] public Dictionary<string, int> UpgradeCost; //Copy of upgrade cost dictionary
    // Start is called before the first frame update
    void Start()
    {
        SlotAmount = Game.Player.GetMaxBuildings();
        //this is where Player's town gets pre-filled
        for (int i = 0; i < SlotAmount; i++)
        {
            if (Game.Player.Town.Count - 1 < i)
                Game.Player.Town.Add(null);
        }

        RenderAllBuildings();

        //Initialize the list for New Building menu
        foreach (Building building in Game.AllBuildings)
        {
            GameObject buildingObj = Instantiate(NewBuildingSlot);
            buildingObj.transform.SetParent(NewBuildingPanel.transform);
            NewBuildingList.Add(buildingObj);
            buildingObj.name = $"[{building.Id}] {building.Name}";
            buildingObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = building.Sprite;
            buildingObj.transform.GetChild(0).gameObject.GetComponent<Button>().onClick
                                             .AddListener(delegate { SelectBuilding(building.Id, buildingObj); });

            buildingObj.transform.position = Vector2.zero;
            buildingObj.GetComponent<RectTransform>().localScale = Vector2.one;
        }

        //Initialize UpgradeCost to make sure ArgumentOutOfRangeException doesn't throw a fit
        UpgradeCost = new Dictionary<string, int>();
    }

    private void RenderAllBuildings()
    {
        foreach (Transform child in buildingSlotPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        buildingSlotList.Clear();
        CharacterSlotList.Clear();


        int index = 0;
        foreach (Town town in Game.Player.Town)
        {

            bool isEmpty = true;

            if (town == null) //Slot is not assigned to any building
            {
                buildingSlotList.Add(Instantiate(buildingEMPTYSlot));
                int position = index;
                buildingSlotList[index].transform.GetChild(0).gameObject.GetComponent<Button>().onClick
                                      .AddListener(delegate { OpenNewBuildingMenu(position); }); ;
                buildingSlotList[index].name = "empty";
            }
            else if (town.Building.IsConstruction) // slot is under construction
            {
                buildingSlotList.Add(Instantiate(BuildingConstructionSlot));
                buildingSlotList[index].name = $"[{town.Building.Name}] Under Construction";
                isEmpty = false;
            }
            else  //not empty and not under construction
            {
                buildingSlotList.Add(Instantiate(buildingSlot));
                buildingSlotList[index].name = town.Building.Name;   //Access name of the building
                isEmpty = false;
            }
            buildingSlotList[index].transform.SetParent(buildingSlotPanel.transform);
            buildingSlotList[index].transform.position = Vector2.zero;
            buildingSlotList[index].GetComponent<RectTransform>().localScale = Vector2.one;
            CharacterSlotList.Add(null);

            if (!isEmpty) // if empty slot no need to set anything
            {
                SetBuilding(index);
            }

            index++;

        }
    }

    private void SelectBuilding(int id, GameObject buildingObj)
    {
        foreach (var item in NewBuildingList)
        {
            item.GetComponent<Image>().enabled = false;
        }
        buildingObj.GetComponent<Image>().enabled = true;
        NewBuildingSelectedHiddenID.text = id.ToString();
        NewBuildingSelectButton.GetComponent<Button>().interactable = true;
    }

    public void SetBuilding(int buildingID)
    {

        GameObject buildingSlot = buildingSlotList[buildingID];
        Building building = Game.Player.Town[buildingID].Building;
        var allKids = buildingSlot.GetComponentsInChildren<Transform>();
        allKids.First(k => k.name == "Building Hidden ID").GetComponent<Text>().text = buildingID.ToString();
        allKids.First(k => k.name == "Building Icon").GetComponent<Image>().sprite = building.Sprite;
        allKids.First(k => k.name == "Building Description").GetComponent<TextMeshProUGUI>().text = building.Description;

        if(building.Type == "training")
        {
            allKids.First(k => k.name == "Building CurrentItem header").GetComponent<TextMeshProUGUI>().text = "Currently Traning For";
        }
        if (!Game.Player.Town[buildingID].Building.IsConstruction) //appies only to NON Construction Building
        {
            string currentItem = " + ";
            if (building.Queue.Count != 0)
            {
                currentItem = building.Queue[0].GetName();
            }
            allKids.First(k => k.name == "Current Item Name").GetComponent<TextMeshProUGUI>().text = currentItem;
            allKids.First(k => k.name == "Building Current Item Button").GetComponent<Button>().onClick
                                      .AddListener(delegate { OpenCraftMenu(buildingID, building.Type); });
            allKids.First(k => k.name == "Building LevelUP").GetComponent<Button>().onClick
                                      .AddListener(delegate { OpenLevelUpMenu(buildingID); });

        }
        allKids.First(k => k.name == "Nuke Building").GetComponent<Button>().onClick
                                      .AddListener(delegate { OpenNukeMessageMenu(buildingID); });
        allKids.First(k => k.name == "Building Level").GetComponent<TextMeshProUGUI>().text = $"Lv{building.Level}";
        allKids.First(k => k.name == "Assign Char Button").GetComponent<Button>().onClick
                                       .AddListener(delegate { OpenCharacterSelectionMenu(buildingID); });

        if (Game.Player.Town[buildingID].CharacterId == -1) //new or empty building
        {
            allKids.First(k => k.name == "Assign Char Button").gameObject.SetActive(true);
        }
        else if (Game.Player.Town[buildingID].CharacterId != -1) //check if character is assigned already from Player's data
        {
            AssignCharacter(buildingID, Game.Player.Town[buildingID].CharacterId);

        }
        
        allKids.First(k => k.name == "Building Linear Progress Bar").GetComponent<ProgressBar>().current = building.BuildProgress;   // building.BuildProgress;
        allKids.First(k => k.name == "Building Progress").GetComponent<TextMeshProUGUI>().text = String.Format("{0:0.##}%", building.BuildProgress);//building.BuildProgress


    }

    private void OpenNukeMessageMenu(int buildingID)
    {

        NukeHiddenID.text = buildingID.ToString();
        NukeHiddenClass.text = "building";
        NukeSprite.sprite = Game.Player.Town[buildingID].Building.Sprite;
        Helper.FadeIn(NukePanel);

    }

    public void YesResponseNukeMessageMenu()
    {
        int objectID = int.Parse(NukeHiddenID.text);
        string objectClass = NukeHiddenClass.text.ToLower();
        switch (objectClass)
        {
            case "building":
                //free up the character
                NotificationManager.Instance.Log($"[{Game.Player.Town[objectID].Building.Name}] was demolished.");

                if (Game.Player.Town[objectID].CharacterId != -1)
                {
                    NotificationManager.Instance.Log($"[{Game.Player.Characters[Game.Player.Town[objectID].CharacterId].Name}] was released.");
                    Game.Player.Characters[Game.Player.Town[objectID].CharacterId].IsBusy = false;
                }


                Game.Player.Town[objectID] = null;
                RenderAllBuildings();
                Helper.FadeOut(NukePanel);

                break;
            default:
                break;
        }

    }

    public void NoResponseNukeMessageMenu()
    {
        Helper.FadeOut(NukePanel);
    }

    //pass building ID to assign, and Charactor ID from Player's charactors
    private void AssignCharacter(int buildingID, int charID)
    {
        Building building = Game.Player.Town[buildingID].Building;
        Game.Player.Town[buildingID].CharacterId = charID;
        var allKids = buildingSlotList[buildingID].GetComponentsInChildren<Transform>();
        allKids.First(k => k.name == "Assign Char Button").gameObject.SetActive(false);
        CharacterSlotList[buildingID] = Instantiate(buildingAssignedCharactorSlot);
        GameObject characterObj = CharacterSlotList[buildingID];
        Character character = Game.Player.Characters[charID];

        characterObj.name = $"[{character.Class}] {character.Name}";
        characterObj.transform.SetParent(allKids.First(k => k.name == "Building Assigned Char"));
        var allCharacterKids = characterObj.GetComponentsInChildren<Transform>();

        allCharacterKids.First(k => k.name == "Assigned Char Icon").GetComponent<Image>().sprite = character.Sprite;
        allCharacterKids.First(k => k.name == "Assigned Char Name").GetComponent<TextMeshProUGUI>().text = character.Name;
        allCharacterKids.First(k => k.name == "Assigned Char Level").GetComponent<TextMeshProUGUI>().text = $"Lv{character.Level}";
        allCharacterKids.First(k => k.name == "Assigned Char Remove btn").GetComponent<Button>().onClick
                                              .AddListener(delegate { RemoveAssigned(buildingID); });

        allCharacterKids.First(k => k.name == "Assigned Char Rate").GetComponent<TextMeshProUGUI>().text = "";// $"Rate: [TODO] per hour";

        characterObj.transform.position = Vector2.zero;
        characterObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        characterObj.GetComponent<RectTransform>().localScale = Vector2.one;

    }

    private void RemoveAssigned(int id)
    {
        GameObject.Destroy(CharacterSlotList[id]);
        Game.Player.Characters[Game.Player.Town[id].CharacterId].IsBusy = false;
        Game.Player.Town[id].Building.CurrentProgress = 0;
        UpdateProgressBar(id);
        //NotificationManager.Instance.Log($"[{Game.Player.Characters[Game.Player.Town[id].CharacterId].Name}] was released.");
        Game.Player.Town[id].CharacterId = -1;
        var allKids = buildingSlotList[id].GetComponentsInChildren<Transform>(true);

        allKids.First(k => k.name == "Assign Char Button").gameObject.SetActive(true);

        if (Game.Player.Town[id].Building.Type == "training")
        {
            Game.Player.Town[id].Building.Queue.Clear();
        }
    }
    [HideInInspector] private List<GameObject> listOfAvailableChars = new List<GameObject>();
    public void OpenCharacterSelectionMenu(int id)
    {
        Debug.Log($"Building ID [{id}] wants to Assign Character");
        CurrentBuilding = Game.Player.Town[id];
        AssignBuildingHiddenID.text = id.ToString();
        foreach (var item in Game.Player.Characters)
        {
            if (item != null && !item.IsBusy)
            {
                GameObject itemObj = Instantiate(AssignCharacterSlot);
                itemObj.name = item.Class;

                itemObj.transform.SetParent(AssignCharacterScrollPanel.transform);
                listOfAvailableChars.Add(itemObj);
                GameObject itemChild = itemObj.transform.GetChild(0).gameObject;
                itemChild.GetComponent<Image>().sprite = item.Sprite;
                itemChild.GetComponent<Button>().onClick.AddListener(delegate { ShowOnPanel(item.Id, itemObj); });
                itemObj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = $"Lv{item.Level}";
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            }
        }

        Helper.FadeIn(AssignCharacterPanel);

    }

    private void ShowOnPanel(int id, GameObject itemObj)
    {
        foreach (var item in listOfAvailableChars)
        {
            item.GetComponent<Image>().enabled = false;
        }
        itemObj.GetComponent<Image>().enabled = true;

        AssignCharacterName.text = Game.Player.Characters[id].Name;
        AssignCharacterStats.text = Game.Player.Characters[id].GetStats();
        AssignCharacterHiddenID.text = id.ToString();
        AssignCharacterSelect.GetComponent<Button>().interactable = true;
    }

    public void CloseCharacterSelectionMenu()
    {
        listOfAvailableChars.Clear();
        Helper.FadeOut(AssignCharacterPanel);
        //Remove all characters from the list
        foreach (Transform child in AssignCharacterScrollPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        AssignCharacterName.text = "";
        AssignCharacterStats.text = "-\n-\n-\n-\n-\n-\n-\n-";
        AssignCharacterHiddenID.text = "-1";
        AssignCharacterSelect.GetComponent<Button>().interactable = false;
    }
    public void SelectCharacter()
    {
        int characterID = int.Parse(AssignCharacterHiddenID.text);
        int buildingID = int.Parse(AssignBuildingHiddenID.text);
        Game.Player.Characters[characterID].IsBusy = true;
        AssignCharacter(buildingID, characterID);
        Game.Player.Town[buildingID].Building.CurrentProgress = 0;
        UpdateProgressBar(buildingID);
        string task = "work on";
        if (Game.Player.Town[buildingID].Building.IsConstruction)
            task = "construct";
        if(Game.Player.Town[buildingID].Building.Type == "training")
            NotificationManager.Instance.Log($"[{Game.Player.Characters[characterID].Name}] was Assigned to train his combat skills.");
        else
            NotificationManager.Instance.Log($"[{Game.Player.Characters[characterID].Name}] was Assigned to {task} [{Game.Player.Town[buildingID].Building.Name}].");
        CloseCharacterSelectionMenu();
    }
    private bool isLevelUpMenuOpen = false;
    public void OpenLevelUpMenu(int id)
    {
        Debug.Log($"Building ID [{id}] wants to Level Up");
        //Set building ID here
        //Current player values will be rendered in Update() so they stay current
        UpgradeBuildingID = id;
        UpgradeCost = Game.Player.Town[id].Building.UpgradeCost;
        string materialName;

        UpdateCosts();
        if (UpgradeCost.Count > 0) //Material 1
        {
            materialName = UpgradeCost.ElementAt(0).Key.ToString();
            Material1.enabled = true;
            Material1.sprite = Resources.Load<Sprite>("building/" + materialName);
            Material1Name.text = materialName;
            Current1.enabled = true;
        }
        else
        {
            Material1.enabled = false;
            Material1Name.text = "";
            Cost1.text = "";
            Current1.enabled = false;
        }
        if (UpgradeCost.Count > 1) //Material 2
        {
            materialName = UpgradeCost.ElementAt(1).Key.ToString();
            Material2.enabled = true;
            Material2.sprite = Resources.Load<Sprite>("building/" + materialName);
            Material2Name.text = materialName;
            Current2.enabled = true;
        }
        else
        {
            Material2.enabled = false;
            Material2Name.text = "";
            Cost2.text = "";
            Current2.enabled = false;
        }
        if (UpgradeCost.Count > 2) //Material 3
        {
            materialName = UpgradeCost.ElementAt(3).Key.ToString();
            Material3.enabled = true;
            Material3.sprite = Resources.Load<Sprite>("building/" + materialName);
            Material3Name.text = materialName;
            Current3.enabled = true;
        }
        else
        {
            Material3.enabled = false;
            Material3Name.text = "";
            Cost3.text = "";
            Current3.enabled = false;
        }
        if (UpgradeCost.Count > 3) //Material 4
        {
            materialName = UpgradeCost.ElementAt(4).Key.ToString();
            Material4.enabled = true;
            Material4.sprite = Resources.Load<Sprite>("building/" + materialName);
            Material4Name.text = materialName;
            Current4.enabled = true;
        }
        else
        {
            Material4.enabled = false;
            Material4Name.text = "";
            Cost4.text = "";
            Current4.enabled = false;
        }


        Helper.FadeIn(UpgradeBuildingPanel);
        isLevelUpMenuOpen = true;
    }
    public void CloseLevelUpMenu()
    {
        isLevelUpMenuOpen = false;
        Helper.FadeOut(UpgradeBuildingPanel);
    }
    public void LevelUpBuilding()
    {
        Debug.Log($"Performing level up on building ID [{UpgradeBuildingID}]");
        Game.Player.Town[UpgradeBuildingID].Building.UpgradeBuilding();
      
        GameObject buildingSlot = buildingSlotList[UpgradeBuildingID];
        var allKids = buildingSlot.GetComponentsInChildren<Transform>();
        allKids.First(k => k.name == "Building Level").GetComponent<TextMeshProUGUI>().text = $"Lv{Game.Player.Town[UpgradeBuildingID].Building.Level}";
        CloseLevelUpMenu();  
        UpdateCosts();
        isLevelUpMenuOpen = false;
    }
    public void UpdateCosts() //Update building upgrade costs on window open and level up
    {
        double rate = Game.Player.Town[UpgradeBuildingID].Building.MaterialRate();
        if (UpgradeCost.Count > 0)
        {
            Cost1.text = ((int)(UpgradeCost.ElementAt(0).Value * rate)).ToString();
        }
        if (UpgradeCost.Count > 1)
        {
            Cost2.text = ((int)(UpgradeCost.ElementAt(1).Value * rate)).ToString();
        }
        if (UpgradeCost.Count > 2)
        {
            Cost3.text = ((int)(UpgradeCost.ElementAt(2).Value * rate)).ToString();
        }
        if (UpgradeCost.Count > 3)
        {
            Cost4.text = ((int)(UpgradeCost.ElementAt(3).Value * rate)).ToString();
        }
    }

    [HideInInspector] private List<GameObject> ListOfAllCrafts = new List<GameObject>();
    private string CurrentResorceType = "";
    private Town CurrentBuilding = null;
    public void OpenCraftMenu(int id, string resourceType)
    {
        Debug.Log($"Building ID [{id}] wants to craft new item");
        CurrentBuilding = Game.Player.Town[id];
        if (resourceType == "training" && CurrentBuilding.CharacterId == -1) 
        { 
            OpenCausionMessage();
        } else
        {
            SelectCraftHiddenID.text = id.ToString();
            SelectCraftHiddenType.text = resourceType;
            CurrentResorceType = resourceType;
            ListOfAllCrafts.Clear();
            CurrentQueue.Clear();
            //Remove all crafts from the list
            foreach (Transform child in SelectCraftPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            //Remove all crafts from the Queue list
            foreach (Transform child in SelectCraftQueuePanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            CurrentRecipeList.Clear();
            //Remove previous costs
            foreach (Transform child in SelectCraftCostPanel.transform)
                GameObject.Destroy(child.gameObject);
            //fill in the available crafts
            int index = 0;

            SelectCraftName.text = "---";
            SelectCraftLimit.isOn = false;
            SelectCraftQuantity.text = "1";
            
            //Allow to pick infinity only for resources
            if (resourceType == "resource" || resourceType == "training")
            {
                SelectCraftTogglePanel.SetActive(true);
                var labels = SelectCraftTogglePanel.GetComponentsInChildren<Transform>();
                if (resourceType == "resource")
                    labels.First(k => k.name == "MaxLabel").GetComponent<Text>().text = $"Infinity";
                else
                    labels.First(k => k.name == "MaxLabel").GetComponent<Text>().text = $"Max ()";
            }
            else
            {
                SelectCraftTogglePanel.SetActive(false);
            }

            //*******************************************
            //Select proper list
            List<string> listToProcess = new List<string>();
            List<iInventory> currentListToCheck = new List<iInventory>();
            switch (resourceType)
            {
                case "resource":
                    listToProcess = Game.Player.Town[id].Building.Crafts;
                    currentListToCheck = Game.AllResources;
                    break;
                case "armor":
                    listToProcess = Game.AllEquipment.Select(x => x.Icon).ToList();
                    currentListToCheck = Game.AllEquipment;
                    break;
                case "weapon":
                    listToProcess = Game.AllWeapons.Select(x => x.Icon).ToList();
                    currentListToCheck = Game.AllWeapons;
                    break;
                case "training":
                    listToProcess = Game.AllTrainingAttributes.Select(x => x.Icon).ToList();
                    currentListToCheck = Game.AllTrainingAttributes;
                    break;
                default:
                    break;
            }


            foreach (string item in listToProcess)
            {
                if (true) //TODO:check is craft is available at this level
                {
                    GameObject itemObj = Instantiate(SelectCraftSlot);
                    var resource = currentListToCheck.First(r => r.Class == item);
                    itemObj.name = resource.Icon;

                    itemObj.transform.SetParent(SelectCraftPanel.transform);
                    GameObject itemChild = itemObj.transform.GetChild(0).gameObject;
                    itemChild.GetComponent<Image>().sprite = resource.Sprite;
                    int i = index;
                
                    if(resourceType != "training")
                        itemChild.GetComponent<Button>().onClick.AddListener(delegate { SelectCraft(resource, i); });
                    else
                        itemChild.GetComponent<Button>().onClick.AddListener(delegate { SelectAttribute(resource, i); });

                    itemObj.transform.position = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    itemObj.GetComponent<RectTransform>().localScale = Vector2.one;


                    ListOfAllCrafts.Add(itemObj);
                    index++;
                }
            }

            foreach (var item in Game.Player.Town[id].Building.Queue)
                CurrentQueue.Add(new CraftTask(item.Class, item.Quantity, item.IsUnLimited, item.FinishedProgress, resourceType, null));

            RenderAllQueue();
            Helper.FadeIn(SelectCraftMainPanel);
        }
    }
    private float MaxAttributePoints = 0;
    private void SelectAttribute(iInventory resource, int index) {
        foreach (var item in ListOfAllCrafts)
        {
            item.GetComponent<Image>().enabled = false;
        }
        ListOfAllCrafts[index].GetComponent<Image>().enabled = true;
        SelectCraftName.text = resource.Name;

        SelectCraftLimit.isOn = true;
        //Calculate how many points for specific attribute can Character get for this level
        Character c = Game.Player.Characters[CurrentBuilding.CharacterId];
        float currentPoints = c.GetAttribute(resource.Class);
        float maxForLevel = c.UnitLevel * 25;
        MaxAttributePoints = maxForLevel - currentPoints;

        var allKids = SelectCraftTogglePanel.GetComponentsInChildren<Transform>();
        allKids.First(k => k.name == "MaxLabel").GetComponent<Text>().text = $"Max ({String.Format("{0:n0}", MaxAttributePoints)})";

        SelectCraftQuantity.text = "1";

    }


    [HideInInspector] private Dictionary<string, int> CurrentRecipe = new Dictionary<string, int>();

    [HideInInspector] private List<GameObject> CurrentRecipeList = new List<GameObject>();
    public Button AddCraftButton;
    private void SelectCraft(iInventory resource, int index)
    {
        foreach (var item in ListOfAllCrafts)
        {
            item.GetComponent<Image>().enabled = false;
        }
        ListOfAllCrafts[index].GetComponent<Image>().enabled = true;

        CurrentRecipe.Clear();
        foreach (var item in resource.Recipe)
            CurrentRecipe.Add(item.Key, item.Value);

        CurrentRecipeList.Clear();
        //Remove previous costs
        foreach (Transform child in SelectCraftCostPanel.transform)
                GameObject.Destroy(child.gameObject);
        int countOfNotEnoughResoures = 0;
        //create new cost
        foreach (var item in CurrentRecipe)
        {
            GameObject itemObj = Instantiate(SelectCraftItemCostSlot);
            itemObj.transform.SetParent(SelectCraftCostPanel.transform);
            itemObj.name = $"{item.Key} [{item.Value}]";
            var allKids = itemObj.GetComponentsInChildren<Transform>();
            allKids.First(k => k.name == "Icon").GetComponent<Image>().sprite = Game.AllResources.First(r => r.Class == item.Key).Sprite;
            if (Game.Player.CheckResource(item.Key, item.Value)) //of there is enough resources for this item show as green, otherwise red, also disable "Add" button
            {
                allKids.First(k => k.name == "Cost").GetComponent<TextMeshProUGUI>().text = $"<color=#164406>{item.Value}</color>";
            }
            else
            {
                allKids.First(k => k.name == "Cost").GetComponent<TextMeshProUGUI>().text = $"<color=#D9280E>{item.Value}</color>";
                countOfNotEnoughResoures++;
            }
            
            itemObj.transform.position = Vector2.zero;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            CurrentRecipeList.Add(itemObj);
        }

        AddCraftButton.interactable = (countOfNotEnoughResoures == 0);
        SelectCraftName.text = resource.Name;
        SelectCraftLimit.isOn = false;
        SelectCraftQuantity.text = "1";

    }

    public void SelectCraftQuantityUp() {


        int q = int.Parse(SelectCraftQuantity.text) + 1;
        if ((int)MaxAttributePoints <= q && CurrentResorceType == "training")
            SelectCraftQuantity.text = MaxAttributePoints.ToString();
        else
            SelectCraftQuantity.text = q.ToString();

        //TODO: CHECK IF THERE IS ENOUGH RESOURCES

    }
    public void SelectCraftQuantityDown()
    {

        int currQty = int.Parse(SelectCraftQuantity.text);

        if (currQty < 1)
        {
            SelectCraftQuantity.text = 1.ToString();
        }
        else
        {
            SelectCraftQuantity.text = (int.Parse(SelectCraftQuantity.text) - 1).ToString();
        }

    }
    [HideInInspector] private List<CraftTask> CurrentQueue = new List<CraftTask>();
    public GameObject CausionMessagePanel;
    [HideInInspector] private iInventory currentItemSelected = null;

    public void AddCraftToQueue()
    {

        if (!string.IsNullOrEmpty(SelectCraftName.text))
        {
            List<iInventory> currentListToCheck = new List<iInventory>();
            string resourceType = SelectCraftHiddenType.text;
            switch (resourceType)
            {
                case "resource":
                    currentListToCheck = Game.AllResources;
                    break;
                case "armor":
                    currentListToCheck = Game.AllEquipment;
                    break;
                case "weapon":
                    currentListToCheck = Game.AllWeapons;
                    break;
                case "training":
                    currentListToCheck = Game.AllTrainingAttributes;
                    break;
                default:
                    break;
            }
            string name = SelectCraftName.text;
            var resource = currentListToCheck.First(r => r.Name == name);

            if(resourceType == "training" && SelectCraftLimit.isOn)
                    CurrentQueue.Add(new CraftTask(resource.Class, (int)MaxAttributePoints, false, resource.FinishedProgress, resourceType, null));
            else
                CurrentQueue.Add(new CraftTask(resource.Class, int.Parse(SelectCraftQuantity.text), bool.Parse(SelectCraftLimit.isOn.ToString()), resource.FinishedProgress, resourceType, resource.Recipe));
            SelectCraftName.text = "";
            SelectCraftQuantity.text = 1.ToString();
            SelectCraftLimit.isOn = false;
            foreach (var item in ListOfAllCrafts)
                item.GetComponent<Image>().enabled = false;
            

            RenderAllQueue();
        }
    }
    
    private void OpenCausionMessage()
    {
        Helper.FadeIn(CausionMessagePanel);
    }
    public void CloseCausionMessage()
    {
        Helper.FadeOut(CausionMessagePanel);
    }
    private void RenderAllQueue()
    {
        //remove all first
        foreach (Transform child in SelectCraftQueuePanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<iInventory> currentListToCheck = new List<iInventory>();
        string resourceType = SelectCraftHiddenType.text;
        switch (resourceType)
        {
            case "resource":
                currentListToCheck = Game.AllResources;
                break;
            case "armor":
                currentListToCheck = Game.AllEquipment;
                break;
            case "weapon":
                currentListToCheck = Game.AllWeapons;
                break;
            case "training":
                currentListToCheck = Game.AllTrainingAttributes;
                break;
            default:
                break;
        }
        //re-render all
        int index = 0;
        foreach (CraftTask item in CurrentQueue)
        {
            GameObject itemObj = Instantiate(SelectCraftQueueSlot);
            var resource = currentListToCheck.First(r => r.Class == item.Class);
            itemObj.transform.SetParent(SelectCraftQueuePanel.transform);
            itemObj.name = resource.Class;
            itemObj.GetComponent<Image>().sprite = resource.Sprite;
            var allKids = itemObj.GetComponentsInChildren<Transform>();

            int i = index;
            allKids.First(k => k.name == "Close Button").GetComponent<Button>().onClick.AddListener(delegate { RemoveFromQueue(i); });

            if (item.IsUnLimited)
               allKids.First(k => k.name == "Qty background").gameObject.SetActive(false);
            else
               allKids.First(k => k.name == "Qty").GetComponent<TextMeshProUGUI>().text = item.Quantity.ToString();
            
            itemObj.transform.position = Vector2.zero;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            
            index++;
        }
    }

    private void RemoveFromQueue(int i)
    {
        CurrentQueue.RemoveAt(i);
        RenderAllQueue();
    }

    public void DoneCraftMenu()
    {
        int buildingID = int.Parse(SelectCraftHiddenID.text);
        //List<CraftTask> queue = new List<CraftTask>();
        Game.Player.Town[buildingID].Building.Queue.Clear();
        foreach (var item in CurrentQueue)
        {
            if (item.Recipe != null && item.Recipe.Count > 0)
            {
                foreach (var costItem in item.Recipe)
                {
                    string property = string.Concat(costItem.Key[0].ToString().ToUpper(), costItem.Key.Substring(1));
                    int curr = (int)Game.Player.Resources.GetType().GetProperty(property).GetValue(Game.Player.Resources, null);
                    curr -= costItem.Value;
                    Game.Player.Resources.GetType().GetProperty(property).SetValue(Game.Player.Resources, curr);
                }
            }

            Game.Player.Town[buildingID].Building.Queue.Add(new CraftTask(item.Class, item.Quantity, item.IsUnLimited, item.FinishedProgress, SelectCraftHiddenType.text, item.Recipe));
        }
            

        //Game.Player.Town[buildingID].Building.Queue = queue;
        RenderAllBuildings();

        CloseCraftMenu();
    }

    public void CloseCraftMenu()
    {
        Helper.FadeOut(SelectCraftMainPanel);
    }
    public void OpenNewBuildingMenu(int id)
    {
        
        NewBuildingSlotHiddenID.text = id.ToString();
        Helper.FadeIn(NewBuildingPanelMain);
    }
    public void CloseNewBuildingMenu()
    {
        Helper.FadeOut(NewBuildingPanelMain);
    }
    public void BuildNewBuildingMenu()
    {
        //get correct positioning for the building
        int buildingSlotID = int.Parse(NewBuildingSlotHiddenID.text);
        //get correct builging to build
        int buildingID = int.Parse(NewBuildingSelectedHiddenID.text);
        
        //reset values
        NewBuildingSlotHiddenID.text = "-1";
        NewBuildingSelectedHiddenID.text = "-1";
        foreach (var item in NewBuildingList)
        {
            item.GetComponent<Image>().enabled = false;
        }
        //Add new building to town
        Building newBuilding = new Building(Game.AllBuildings[buildingID]);
        Game.Player.Town[buildingSlotID] = new Town(newBuilding, -1);
        NotificationManager.Instance.Log($"[{newBuilding.Name}] is ready for worker to finish construction.");
        //Re- render entire town
        
        RenderAllBuildings();

        Helper.FadeOut(NewBuildingPanelMain);
    }

    void Awake()
    {
       Application.targetFrameRate = 10;
    }

    // Update is called once per frame
    void Update()
    {
        int index = 0;
        if (Game.Player != null)
        {

            foreach (Town building in Game.Player.Town.Where(t => t != null))
            {
                if (building != null && building.CharacterId != -1) //check if character is assigned to Building
                {
                    //-----------------Check if Character status---------------------
                    if (Game.Player.Characters[building.CharacterId].Class != building.Assigned.Class ||
                        Game.Player.Characters[building.CharacterId].Name != building.Assigned.Name ||
                        Game.Player.Characters[building.CharacterId].Level != building.Assigned.Level)
                    {
                        int tempID = building.CharacterId;
                        Debug.Log($"[{building.CharacterId}] with name [{Game.Player.Characters[building.CharacterId].Name}] changed Class");
                        RemoveAssigned(index); //remove old character stuff
                        AssignCharacter(index, tempID); //re-publich character with new things
                        building.Assigned.Class = Game.Player.Characters[tempID].Class;
                        building.Assigned.Name = Game.Player.Characters[tempID].Name;
                        building.Assigned.Level = Game.Player.Characters[tempID].Level;
                    }
                    //----------------------------------------------------------------
                    if (building.Building.IsConstruction) //Check if it is under construction
                    {

                        if (building.Building.GetConstructPercent() <= 99.8)
                        {
                            building.Building.ConstructBuilding(Game.Player.Characters[building.CharacterId]);
                            Debug.Log($"[{building.Building.Name}] construction is at: {building.Building.GetConstructPercent()}%");
                        }
                        else
                        {
                            //Finish Construction
                            Game.Player.Town[index].Building.ConsctructionCurrect = 100;
                            Game.Player.Town[index].Building.IsConstruction = false;
                            Game.Player.Town[index].Building.BuildProgress = 0;
                            Game.Player.Town[index].Building.CurrentProgress = 0;
                            Game.Player.Town[index].Building.Level = 1;
                            NotificationManager.Instance.Log($"[{Game.Player.Characters[building.CharacterId].Name}] Finished Construction of [{building.Building.Name}]");
                            NotificationManager.Instance.Log($"[{Game.Player.Characters[building.CharacterId].Name}] is released. Assign Charactor to newly constructed building");
                            Game.Player.Characters[building.CharacterId].IsBusy = false;
                            Game.Player.Town[index].CharacterId = -1;

                            RenderAllBuildings();
                        }

                    }
                    else if (building.Building.Queue.Count > 0) //builing is built and has Something to Work On
                    {
                        //I think DoTask needs to take the crafting object in it
                        building.Building.DoTask(Game.Player.Characters[building.CharacterId]);
                        //Debug.Log($"{building.Building.Name} performed task, now at {building.Building.GetPercent()}");
                    }
                    UpdateProgressBar(index);
                }
                if (building != null && building.Building.Queue.Count() == 0)
                {
                    var allKids = buildingSlotList[index].GetComponentsInChildren<Transform>();
                    allKids.First(k => k.name == "Current Item Name").GetComponent<TextMeshProUGUI>().text = "+";
                }
                index++;
            }
        }
        if (isLevelUpMenuOpen)
        {
            //-----------------------------------------------------------------------------------------------------
            //Rendering player material count in upgrade building screen
            int[] current = new int[4];
            if (UpgradeCost.Count > 0)
            {
                current[0] = Game.Player.Resources.GetResource(UpgradeCost.ElementAt(0).Key);

                Current1.text = current[0].ToString();
                if (current[0] < UpgradeCost.ElementAt(0).Value)
                {
                    Current1.color = Color.red;
                }
                else
                {
                    Current1.color = Color.black;
                }
            }
            if (UpgradeCost.Count > 1)
            {
                current[1] = Game.Player.Resources.GetResource(UpgradeCost.ElementAt(1).Key);

                Current2.text = current[1].ToString();
                if (current[1] < UpgradeCost.ElementAt(1).Value)
                {
                    Current2.color = Color.red;
                }
                else
                {
                    Current2.color = Color.black;
                }
            }
            if (UpgradeCost.Count > 2)
            {
                current[2] = Game.Player.Resources.GetResource(UpgradeCost.ElementAt(2).Key);

                Current3.text = current[2].ToString();
                if (current[2] < UpgradeCost.ElementAt(3).Value)
                {
                    Current3.color = Color.red;
                }
                else
                {
                    Current3.color = Color.black;
                }
            }
            if (UpgradeCost.Count > 3)
            {
                current[3] = Game.Player.Resources.GetResource(UpgradeCost.ElementAt(3).Key);

                Current4.text = current[3].ToString();
                if (current[3] < UpgradeCost.ElementAt(3).Value)
                {
                    Current4.color = Color.red;
                }
                else
                {
                    Current4.color = Color.black;
                }
            }

            //Does the player have enough to upgrade the building?
            bool enough = true;
            for (int i = 0; i < UpgradeCost.Count; i++)
            {
                if (current[i] < UpgradeCost.ElementAt(i).Value)
                {
                    enough = false;
                }
            }
            if (enough) //Enable if player has enough, disable if not
            {
                UpgradeButton.interactable = true;
            }
            else
            {
                UpgradeButton.interactable = false;
            }
        }
    }

    private void UpdateProgressBar(int buildingID)
    {
        GameObject buildingSlot = buildingSlotList[buildingID];
        Building building = Game.Player.Town[buildingID].Building;
        float buildProgress = 0;
        float currentProgress = 0;
        float percent = 0;

        if (building.IsConstruction)
        {
            buildProgress = building.ConsctructionPoints;
            currentProgress = building.ConsctructionCurrect;
            percent = (float)building.GetConstructPercent();
        }
        else
        {
            buildProgress = building.FinishedProgress;
            currentProgress = building.CurrentProgress;
            percent = (float)building.GetPercent();
        }

        var allKids = buildingSlot.GetComponentsInChildren<Transform>();

        if (building.IsConstruction == false)
            allKids.First(k => k.name == "Current Item Name").GetComponent<TextMeshProUGUI>().text = "+";
        
        if(building.Queue.Count > 0)
            allKids.First(k => k.name == "Current Item Name").GetComponent<TextMeshProUGUI>().text = building.Queue[0].GetName();

        
        allKids.First(k => k.name == "Building Linear Progress Bar").GetComponent<ProgressBar>().maximum = buildProgress;
        allKids.First(k => k.name == "Building Linear Progress Bar").GetComponent<ProgressBar>().current = currentProgress;
        allKids.First(k => k.name == "Building Progress").GetComponent<TextMeshProUGUI>().text = percent.ToString("F2") + "%";

        
    }


}
