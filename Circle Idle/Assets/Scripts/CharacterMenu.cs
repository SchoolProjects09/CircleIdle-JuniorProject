using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleIdleLib;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class CharacterMenu : MonoBehaviour
{
    [Space(10)]
    [Header("Main Character's Panel")]
    
    public GameObject charPanel;
    public GameObject charslotPanel;
    public GameObject charSlot;
    public GameObject charItem;
    [Space(10)]

    [HideInInspector] public List<GameObject> charSlots = new List<GameObject>();
    //[HideInInspector] public List<Character> localCharacters = new List<Character>();
    [HideInInspector] public int SlotAmount;
    [Header("Character's Info Panel")]
    public GameObject HoldingPanel;
    public TextMeshProUGUI CharName;
    public TextMeshProUGUI CharDescription;
    public TextMeshProUGUI CharLevel;
    public TextMeshProUGUI CharExp;
    public ProgressBar CharProgressBar;
    public Image CharImage;

    public TextMeshProUGUI CharHPStats;
    public TextMeshProUGUI CharSPStats;
    public TextMeshProUGUI CharATKStats;
    public TextMeshProUGUI CharDEFStats;
    public TextMeshProUGUI CharMAGStats;
    public TextMeshProUGUI CharRESStats;
    public Text charId;
    public Button Open_Panel_HireNew;
    public Button editNameButton;  
    public Button CharSpecialAbility;
    public Button CharArmor;
    public Button CharWeapon;
    public GameObject CharNukeButton;
    public GameObject CharUpgradeButton;
    [Space(10)]
    /*---------Hire New Character Menu Vars -------------*/
    [Header("Hire New Character Panel")]
    public GameObject HireNewPanel;
    public GameObject HireNewCharGrid;
    public GameObject HireNewCharSlot;
    public Text HireNewHiddenId;
    public Text HireNewHiddenSlotId;
    public TextMeshProUGUI HireNewCharName;
    public TextMeshProUGUI HireNewCharDescription;
    public Image HireNewCharImage;
    public TextMeshProUGUI HireNewCharStats;
    public TextMeshProUGUI HireNewCharCost;
    public Button HireNewCharButton;
    public GameObject HireNewSlotContainer;
    [Space(10)]
    /*-------------------------------------------------*/
    /*----------Name change Dialog---------------------*/
    [Header("Change Name Panel")]
    public GameObject NameChangePanel;
    public TMP_InputField NameChange_New_Name;
    [Space(10)]
    /*-------------------------------------------------*/
    /*----------Upgrade character Dialog---------------------*/
    [Header("Upgrade Character Panel")]
    public GameObject nextCharacterPanel;
    public GameObject nextCharacterShowPanel;
    public GameObject nextCharacterSlot;
    public TextMeshProUGUI nextCharacterStats;
    public TextMeshProUGUI nextCharacterCost;
    public Text nextCharacterID;
    public Button nextCharacterUpgradeButton;

    /*-------------------------------------------------*/
    [Space(10)]

    [Header("Setup Nuke item Menu ")]
    public GameObject NukePanel;
    public Text NukeHiddenClass;
    public Text NukeHiddenID;
    public Image NukeSprite;


    void Start()
    {
        HoldingPanel.SetActive(false);
        SlotAmount = Game.Player.GetMaxWorkers();

        //initialize the list of characters that Player has including empty slots
        //This is done only when game starts
        for (int i = 0; i < SlotAmount; i++)
        {
            if (Game.Player.Characters.Count - 1 < i)
                Game.Player.Characters.Add(null);
        }
        RenderAllCharacters();


        SetNewCharactorMenu();

    }
    public void RenderAllCharacters() {
        //TODO: Rewrite start function here
        foreach (Transform child in charslotPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        charSlots.Clear();
        //Set all items from the beginning
        for (int i = 0; i < SlotAmount; i++)
        { 
            charSlots.Add(Instantiate(charSlot));
            charSlots[i].transform.SetParent(charslotPanel.transform);
            charSlots[i].GetComponent<RectTransform>().localScale = Vector2.one;
            int index = i;
            charSlots[i].transform.GetChild(0).gameObject.GetComponent<Button>().onClick
                                      .AddListener(delegate { ShowHirePanel(index); });
            if (Game.Player.Characters[i] != null)
                AddItem(Game.Player.Characters[i], i);
        }

    }
    private void SetNewCharactorMenu()
    {
        //Initialize the list for Hire characters with all possible characters but display only the once that were unlocked
        //ClearNewCharacterPanel();
        List<Transform> allCharacterSlots = HireNewSlotContainer.GetComponentsInChildren<Transform>().ToList();
            allCharacterSlots.RemoveAt(0);
        int i = 0;
        foreach (Character HireNewCharactor in Game.AllCharachters)
        {
            allCharacterSlots[i].name = $"[{HireNewCharactor.Id}] {HireNewCharactor.Class}";
            if (Game.Player.UnlockedClasses.Contains(HireNewCharactor.Class))
            {
                allCharacterSlots[i].GetComponent<Image>().sprite = HireNewCharactor.Sprite;
                allCharacterSlots[i].GetComponent<Button>().onClick.AddListener(delegate { DisplayCharacterOnTheboard(HireNewCharactor.Id); });
            }
            allCharacterSlots[i].GetComponent<RectTransform>().localScale = Vector2.one;
            i++;

        }

    }

    private void ClearNewCharacterPanel()
    {
        //Remove all characters from the list
        /*foreach (Transform child in HireNewCharGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }*/

    }
    void Update()
    {
        int id = int.Parse(charId.text);
        if (id != -1)
        {
            
            //Game.Player.Characters[id].LevelUp();
            CharExp.text = Game.Player.Characters[id].GetCurrentExpPoints();
            CharProgressBar.current = Game.Player.Characters[id].Experience;
            CharProgressBar.minimum = Game.Player.Characters[id].MinPointsForLevel;
            CharProgressBar.maximum = Game.Player.Characters[id].MaxPointsForLevel;
            CharLevel.text = "Lv" + Game.Player.Characters[id].Level.ToString();
            CharHPStats.text = Game.Player.Characters[id].GetStats("HP");
            CharSPStats.text = Game.Player.Characters[id].GetStats("SP"); 
            CharATKStats.text = Game.Player.Characters[id].GetStats("ATK");
            CharDEFStats.text = Game.Player.Characters[id].GetStats("DEF");
            CharMAGStats.text = Game.Player.Characters[id].GetStats("MAG");
            CharRESStats.text = Game.Player.Characters[id].GetStats("RES");
            //Debug.Log($"[{id}] points {Game.Player.Characters[id].GetCurrentExpPoints()}");
        }
    }
    void Awake()
    {
        Application.targetFrameRate = 10;
    }
    /// <summary>
    /// Add Character to the Main User's Panel 
    /// </summary>
    /// <param name="charactor"></param>
    /// <param name="index"></param>
    public void AddItem(Character charactor, int index)
    {

        GameObject itemObj = Instantiate(charItem);
        itemObj.name = $"[{charactor.Class}] {charactor.Name}";
        itemObj.transform.SetParent(charSlots[index].transform);
        itemObj.GetComponent<Image>().sprite = charactor.Sprite;
        itemObj.GetComponent<Button>().onClick.AddListener(delegate { Show_Panel(index); });
        itemObj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = charactor.Name;
        itemObj.transform.position = Vector2.zero;
        itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        itemObj.GetComponent<RectTransform>().localScale = Vector2.one;

    }
    /// <summary>
    /// Assigned to click event for the character on player's panel. Displays character information
    /// </summary>
    /// <param name="id"></param>
    private void Show_Panel(int id)
    {
        HoldingPanel.SetActive(true);
        Character character = Game.Player.Characters[id];
        Debug.Log(character.Name);
        CharName.text = character.Name;
        CharDescription.text = character.Description;
        CharLevel.text = "Lv" + character.Level.ToString();
        CharImage.GetComponent<Image>().sprite = character.Sprite;
        CharHPStats.text = character.GetStats("HP");
        CharSPStats.text = character.GetStats("SP"); //THERE SHOULD BE ONE SPEED
        CharATKStats.text = character.GetStats("ATK");
        CharDEFStats.text = character.GetStats("DEF");
        CharMAGStats.text = character.GetStats("MAG");
        CharRESStats.text = character.GetStats("RES");
        charId.text = character.Id.ToString();
        CharExp.text = character.GetCurrentExpPoints();
        CharProgressBar.current = character.Experience;
        CharProgressBar.minimum = character.MinPointsForLevel;
        CharProgressBar.maximum = character.MaxPointsForLevel;
        CharSpecialAbility.GetComponent<TooltipTrigger>().header = character.Ability.Name;
        CharSpecialAbility.GetComponent<TooltipTrigger>().content = character.Ability.Description; ;

        editNameButton.GetComponent<Button>().interactable = true;
        if (character.Next[0] == "none" || (character.Level != character.MaxLvl && character.Experience < character.MaxPointsForLevel))
            CharUpgradeButton.SetActive(false);
        else
            CharUpgradeButton.SetActive(true);

        if (character.IsBusy == true)
            CharNukeButton.SetActive(false);
        else
            CharNukeButton.SetActive(true);
    }
    private void Reset_Show_Panel()
    {
        CharName.text = "";
        CharDescription.text = "";
        CharLevel.text = "Lv0";
        CharImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("default" );
        CharHPStats.text = "---";
        CharSPStats.text = "---";
        CharATKStats.text = "---";
        CharDEFStats.text = "---";
        CharMAGStats.text = "---";
        CharRESStats.text = "---";
        charId.text = "-1";
        CharExp.text = "---/---";
        CharProgressBar.current = 0;
        CharProgressBar.minimum = 0;
        CharProgressBar.maximum = 0;
        CharSpecialAbility.GetComponent<TooltipTrigger>().content = "";
        CharArmor.GetComponent<TooltipTrigger>().content = "";
        CharWeapon.GetComponent<TooltipTrigger>().content = "";
        editNameButton.GetComponent<Button>().interactable = false;
        CharUpgradeButton.SetActive(false);
        CharNukeButton.SetActive(false);
    }

    /// <summary>
    /// Get Character by id that is stored in the main List
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Character FetchCharacterByID(int id)
    {
        foreach (var item in Game.AllCharachters)
        {
            if(item.Id == id)
            {
                return item;
            }
        }
        return null;
    }
    public int GetListID_CharacterByID(int id)
    {
        int index = 0;
        foreach (var item in Game.Player.Characters)
        {
            if (item.Id == id)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
    public Character FetchCharacterByClass(string charClass)
    {
        foreach (var item in Game.AllCharachters)
        {
            if (item.Class == charClass)
            {
                return item;
            }
        }
        return null;
    }

    private List<GameObject> UpgradeToChars = new List<GameObject>();
    public void StartClassAdvancement()
    {
        int id = int.Parse(charId.text);
        NotificationManager.Instance.Log($"Request to level up character with id [{id.ToString()}]");
        if(id != -1) { 
            int firstID = -1;
            
            foreach (var nextCharacter in Game.Player.Characters[id].Next)
            {
                Character fromMainList = FetchCharacterByClass(nextCharacter);
                GameObject itemObj = Instantiate(nextCharacterSlot);
                itemObj.name = nextCharacter;
                
                itemObj.transform.SetParent(nextCharacterShowPanel.transform);
                UpgradeToChars.Add(itemObj);
                GameObject itemChild = itemObj.transform.GetChild(0).gameObject;
                itemChild.GetComponent<Image>().sprite = fromMainList.Sprite;
                itemChild.GetComponent<Button>().onClick.AddListener(delegate { AdvanceClassTo(fromMainList.Id, itemObj); });
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
                

                if (firstID == -1)
                {
                    firstID = fromMainList.Id;
                }
            }
            if (firstID != -1)
            {
                AdvanceClassTo(firstID, UpgradeToChars[0]);
            }

            Helper.FadeIn(nextCharacterPanel);
        }
        else
        {
            NotificationManager.Instance.Log($"Load Character first");
        }
    }
    /// <summary>
    /// Bring up the Class Upgrade menu
    /// </summary>
    /// <param name="id"></param>
    public void AdvanceClassTo(int id, GameObject obj)
    {
        foreach (var item in UpgradeToChars)
        {
            item.GetComponent<Image>().enabled = false;
        }
        obj.GetComponent<Image>().enabled = true;

        nextCharacterStats.text = Game.AllCharachters[id].GetStats();
        nextCharacterCost.text = Game.AllCharachters[id].GetUpgradeCost() + " gold";
        nextCharacterID.text = id.ToString();
        nextCharacterUpgradeButton.GetComponent<Button>().interactable = true;

    }
    public void AdvanceClass()
    {
        int toId = int.Parse(nextCharacterID.text);
        int playersListID = int.Parse(charId.text);
        if (Game.Player.isEnoughGold(int.Parse(Game.AllCharachters[toId].GetUpgradeCost())))
        {
            Game.Player.Upgrade(playersListID, toId);
            GameObject ChildGameObject = charSlots[playersListID].transform.GetChild(1).gameObject;
            ChildGameObject.name = $"[{Game.Player.Characters[playersListID].Class}] {Game.Player.Characters[playersListID].Name}";
            ChildGameObject.GetComponent<Image>().sprite = Game.Player.Characters[playersListID].Sprite;

            Game.Player.Resources.Gold -= int.Parse(Game.AllCharachters[toId].GetUpgradeCost());

            Show_Panel(playersListID);
            Game.Player.UnlockedClasses.Add(Game.Player.Characters[playersListID].Class);
        }
        else
        {
            NotificationManager.Instance.Log($"You do not have enough gold to upgrade {Game.Player.Characters[playersListID].Name}");
            
        }
        ClassAdvancementClose();
    }


    public void ClassAdvancementClose()
    {
        UpgradeToChars.Clear();
        Helper.FadeOut(nextCharacterPanel);
        //Remove all characters from the list
        foreach (Transform child in nextCharacterShowPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        nextCharacterCost.text = "------ GOLD";
        nextCharacterID.text = "-1";
        nextCharacterStats.text = "-\n-\n-\n-\n-\n-\n-\n-";
        nextCharacterUpgradeButton.GetComponent<Button>().interactable = false;
    }

    public void ChangeName()
    {
        int id = int.Parse(charId.text);
        Debug.Log($"Request to change the name of the character with id [{id.ToString()}]");
        id = GetListID_CharacterByID(id);
        NameChange_New_Name.text = Game.Player.Characters[id].Name;
        Helper.FadeIn(NameChangePanel);
    }
    /// <summary>
    /// Function that updates the name of the Character 
    /// </summary>
    public void UpdateName()
    {
        int id = GetListID_CharacterByID(int.Parse(charId.text));
        NotificationManager.Instance.Log($"Name of the character was changed");

        Game.Player.Characters[id].Name = NameChange_New_Name.text;
        RenderAllCharacters();
        Show_Panel(int.Parse(charId.text));
        Helper.FadeOut(NameChangePanel);
    }
    public void CloseUpdateName()
    {
        Helper.FadeOut(NameChangePanel);
    }
    public void ShowHirePanel(int i)
    {
        HireNewHiddenSlotId.text = i.ToString();
        SetNewCharactorMenu();  //Reset panel with all currently unlocked items
        DisplayCharacterOnTheboard(-1); //Reset charactor info
        Helper.FadeIn(HireNewPanel);
    }
    public void CloseHirePanel()
    {
        Helper.FadeOut(HireNewPanel);
    }
    /// <summary>
    /// Assigned to click event for the character that is going to be hired
    /// </summary>
    /// <param name="id"></param>
    public void DisplayCharacterOnTheboard(int id)
    {


        if (id != -1)
        {
            HireNewCharButton.interactable = true;
            HireNewHiddenId.text = id.ToString();
            Character character = FetchCharacterByID(id);
            HireNewCharName.text = character.Name;
            HireNewCharDescription.text = character.Description;
            HireNewCharImage.GetComponent<Image>().sprite = character.Sprite;
            HireNewCharStats.text = character.GetStats();
            //new hire gold text color is always red even if can afford, maybe add check to change gold color
            if (Game.Player.isEnoughGold(character.GetCost()))
            {
                HireNewCharCost.text = "<color=green>" + character.GetCost().ToString() + " gold</color>";
            }
            else
            {
                HireNewCharCost.text = "<color=red>" + character.GetCost().ToString() + " gold</color>";
            }
        }
        else
        {
            HireNewCharButton.interactable = false;
            HireNewHiddenId.text = "-1";
            HireNewCharName.text = "-----";
            HireNewCharDescription.text = "-----";
            HireNewCharImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("default");
            HireNewCharStats.text = "-\n-\n-\n-\n-\n-";
            HireNewCharCost.text = "";
        }

    }
    /// <summary>
    /// Process to hire a character
    /// </summary>
    public void HireCharacter()
    {
        int id = int.Parse(HireNewHiddenId.text);
        int slotId = int.Parse(HireNewHiddenSlotId.text);
        Debug.Log($"Player wanted to hire character [{id}] {Game.AllCharachters[id].Name}");

        if (Game.Player.isEnoughGold(Game.AllCharachters[id].GetCost()))    //checking if player has enough gold to purchase the new character
        {
            Game.Player.HireCharacter(Game.AllCharachters[id].DeepCopy(), slotId);   //Hire process add character to the player's list
            Game.Player.Characters[slotId].Id = slotId;      //make sure that id is set
                
            RenderAllCharacters();
            //AddItem(Game.Player.Characters[slotId], slotId); //add it to the canvas list

            Game.AllCharachters[id].Name = Game.GetRandomName();  //change the name on next character

            Game.Player.Resources.Gold -= Game.AllCharachters[id].GetCost();

        }
        else
        {
            NotificationManager.Instance.Log("Player does not have enough gold to hire this character");
        }
        Helper.FadeOut(HireNewPanel);
            ClearNewCharacterPanel();

    }

    public void OpenNukePanel()
    {
        NukeHiddenID.text = charId.text;
        NukeHiddenClass.text = "Char";
        NukeSprite.sprite = Game.Player.Characters[int.Parse(charId.text)].Sprite;
        Helper.FadeIn(NukePanel);
    }

    public void YesResponseNukeMessageMenu()
    {
        int objectID = int.Parse(NukeHiddenID.text);
        string objectClass = NukeHiddenClass.text.ToLower();
        switch (objectClass)
        {
            case "char":
                //Also need to check if charachter is assigned to any building?
                Game.Player.Characters[objectID] = null;
                RenderAllCharacters();
                Reset_Show_Panel();
                Helper.FadeOut(NukePanel);
                break;
            default:
                break;
        }
    }


}
