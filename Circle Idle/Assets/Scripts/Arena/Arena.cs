using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    [Header("Setup Main Sections Panel")]
    public GameObject ArenaPanel;
    public GameObject TicketPanel;
    
    [Space(10)]
    [Header("Setup Arena objects")]
    public GameObject PlayerTeamPanel;
    public TextMeshProUGUI PlayerTeamHPStats;
    public TextMeshProUGUI PlayerTeamSPStats;
    public TextMeshProUGUI PlayerTeamATKStats;
    public TextMeshProUGUI PlayerTeamDEFStats;
    public TextMeshProUGUI PlayerTeamMAGStats;
    public TextMeshProUGUI PlayerTeamRESStats;
    public GameObject PlayerFigherSpace;

    public GameObject OpponentTeamPanel;
    public TextMeshProUGUI OpponentTeamHPStats;
    public TextMeshProUGUI OpponentTeamSPStats;
    public TextMeshProUGUI OpponentTeamATKStats;
    public TextMeshProUGUI OpponentTeamDEFStats;
    public TextMeshProUGUI OpponentTeamMAGStats;
    public TextMeshProUGUI OpponentTeamRESStats;
    public GameObject OpponentFigherSpace;

    //public GameObject CharacterSpaceEmptyPREFAB;
    public GameObject CharacterSpacePREFAB;
    [Space(10)]
    [Header("Setup Character Assignment Panel")]
    public GameObject AssignCharacterPanel;
    public GameObject AssignCharacterSlotPanel;
    public GameObject AssignCharacterPREFAB;
    public GameObject CharacterGrayedOut;
    public TextMeshProUGUI AssignCharacterHPStats;
    public TextMeshProUGUI AssignCharacterSPStats;
    public TextMeshProUGUI AssignCharacterATKStats;
    public TextMeshProUGUI AssignCharacterDEFStats;
    public TextMeshProUGUI AssignCharacterMAGStats;
    public TextMeshProUGUI AssignCharacterRESStats;

    public Button AssignCharacterSelectButton;
    public Text RenderTrigger;
    [Space(10)]
    [Header("Setup Fight Stats")]
    public TextMeshProUGUI Round;
    public TextMeshProUGUI Score;
    [Space(10)]
    [Header("Setup Ticket objects")]
    public Button BuyTicket;
    public GameObject RewardsPanel;
    public GameObject RewardPREFAB;
    public TextMeshProUGUI TicketCost;
    public GameObject HelpPanel;

    [HideInInspector]
    private List<int> PlayerTeam = new List<int>();
    //private List<Character> PlayerTeamStats = new List<Character>();
    //private List<Character> OpponentTeam = new List<Character>();
    //private List<Character> OpponentTeamStats = new List<Character>();

    private int round = 1;
    private int PlayerPoints = 0;
    private int OpponentPoints = 0;
    private List<GameObject> PlayerTeamList = new List<GameObject>();
    private List<GameObject> OpponentTeamList = new List<GameObject>();
    private int CurrentSlotId = -1;
    private List<GameObject> ListOfAllFighers = new List<GameObject>();//used for Character Selection
    private int SelectedCharacterID = -1;
    public Sprite DefaultSprite;
    [Space(10)]
    [Header("Final Message")]
    public GameObject FinalMessagePanel;
    public GameObject FinalRewardsPanel;
    public TextMeshProUGUI Message;
    public TextMeshProUGUI ButtonMessage;
    public GameObject FinalRewards;
    public GameObject StartButton;
    public GameObject RefreshButton;

    public GameObject StepMessage;

    public Button[] TabButtons;
    private void RenderPlayerTeam()
    {
        //foreach (Transform child in PlayerTeamPanel.transform)
        //{
        //    GameObject.Destroy(child.gameObject);
        //}
        PlayerTeam.Clear();
        foreach (GameObject fighter in PlayerTeamList)
        {
            if(fighter.GetComponent<Fighter>().fighter != null)
            {
                Character c = fighter.GetComponent<Fighter>().fighter;
                fighter.name = $"[{c.Class}] {c.Name}";
                fighter.GetComponent<Image>().sprite = c.Sprite;
                fighter.transform.GetChild(0).gameObject.SetActive(false);
                fighter.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = c.Sprite;
                fighter.GetComponent<Button>().enabled = false;
                fighter.GetComponent<Fighter>().ShowRemove = true;
                PlayerTeam.Add(c.Id);
            }
            else
            {
                fighter.name = $"[EMPTY]";
                fighter.GetComponent<Image>().sprite = DefaultSprite;
                fighter.transform.GetChild(0).gameObject.SetActive(true);
                fighter.GetComponent<Button>().enabled = true;
                fighter.GetComponent<Fighter>().ShowRemove = false;
            }

        }
        PlayerTeamHPStats.text = 0.ToString();
        PlayerTeamSPStats.text = 0.ToString();
        PlayerTeamATKStats.text = 0.ToString();
        PlayerTeamDEFStats.text = 0.ToString();
        PlayerTeamMAGStats.text = 0.ToString();
        PlayerTeamRESStats.text = 0.ToString();

        Character[] characters = PlayerTeamList.Select(f => f.GetComponent<Fighter>().fighter).Where(f => f != null).ToArray();
        foreach (Character fighter in characters)
        {
            PlayerTeamHPStats.text = (int.Parse(PlayerTeamHPStats.text) + ConvertToInt(fighter.GetStats("HP"))).ToString();
            PlayerTeamSPStats.text = (int.Parse(PlayerTeamSPStats.text) + ConvertToInt(fighter.GetStats("SP"))).ToString();
            PlayerTeamATKStats.text = (int.Parse(PlayerTeamATKStats.text) + ConvertToInt(fighter.GetStats("ATK"))).ToString();
            PlayerTeamDEFStats.text = (int.Parse(PlayerTeamDEFStats.text) + ConvertToInt(fighter.GetStats("DEF"))).ToString();
            PlayerTeamMAGStats.text = (int.Parse(PlayerTeamMAGStats.text) + ConvertToInt(fighter.GetStats("MAG"))).ToString();
            PlayerTeamRESStats.text = (int.Parse(PlayerTeamRESStats.text) + ConvertToInt(fighter.GetStats("RES"))).ToString();
        }
    }
    private int ConvertToInt(string str)
    {
        return int.Parse(Math.Floor(double.Parse(str)).ToString());
    }
    private void RenderInitialPlayerTeam()
    {
        if(PlayerTeamList.Count > 0)
        {
            foreach (Character f in PlayerTeamList.Select(f => f.GetComponent<Fighter>().fighter))
            {
                f.IsBusy = false;
            }
        }
        PlayerTeamList.Clear();

        PlayerTeamHPStats.text = 0.ToString();
        PlayerTeamSPStats.text = 0.ToString();
        PlayerTeamATKStats.text = 0.ToString();
        PlayerTeamDEFStats.text = 0.ToString();
        PlayerTeamMAGStats.text = 0.ToString();
        PlayerTeamRESStats.text = 0.ToString();
        foreach (Transform child in PlayerTeamPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 5; i++)
        {
            GameObject fighter = Instantiate(CharacterSpacePREFAB);
            fighter.transform.SetParent(PlayerTeamPanel.transform);
            //Might need to have Selection
            fighter.name = "[EMPTY] Not Assigned";
            int index = i;
            fighter.GetComponent<Fighter>().fighter = null;
            fighter.GetComponent<Fighter>().RenderTrigger = RenderTrigger;
            fighter.GetComponent<Button>().onClick.AddListener(delegate { OpenAssignCharacterPanel(index); });
            fighter.transform.position = Vector2.zero;
            fighter.GetComponent<RectTransform>().localScale = Vector2.one;
            PlayerTeamList.Add(fighter);
        }
    }

    private void ClearFightersPlace() {
        PlayerFigherSpace.GetComponent<Image>().sprite = DefaultSprite;
        OpponentFigherSpace.GetComponent<Image>().sprite = DefaultSprite;
        PlayerTurn.GetComponent<Image>().sprite = DefaultSprite;
        OpponentTurn.GetComponent<Image>().sprite = DefaultSprite;
        PlayerTurn.SetActive(false);
        OpponentTurn.SetActive(false);
        PlayerResult.SetActive(false);
        OpponentResult.SetActive(false);
        Round.text = "Round 1";
        PlayerProgressBar.current = 0;
        PlayerProgressBar.maximum = 0;
        OpponentProgressBar.current = 0;
        OpponentProgressBar.maximum = 0;
        Score.text = "0:0";
        StartButton.SetActive(false);
        RefreshButton.SetActive(true);
    }
    private void OpenAssignCharacterPanel(int index)
    {
        CurrentSlotId = index;
        //TODO Open Panel
        foreach (Transform child in AssignCharacterSlotPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        ListOfAllFighers.Clear();
        AssignCharacterSelectButton.interactable = false;
        int i = 0;
        foreach (Character character in Game.Player.Characters.Where(c => c!= null && c.IsBusy == false))
        {
            GameObject fighter = Instantiate(AssignCharacterPREFAB);
            fighter.transform.SetParent(AssignCharacterSlotPanel.transform);
            fighter.name = $"[{character.Class}] {character.Name}";
            fighter.GetComponent<Image>().enabled = false;  //Turn Off selection background
            var allKids = fighter.GetComponentsInChildren<Transform>(true);
            int slotId = i;
            allKids.First(k => k.name == "Character").gameObject.GetComponent<Image>().sprite = character.Sprite;
            allKids.First(k => k.name == "Character").gameObject.GetComponent<Button>().onClick.AddListener(delegate { ShowFighterStats(slotId, character.Id); });
            allKids.First(k => k.name == "Level").gameObject.GetComponent<TextMeshProUGUI>().text = $"{character.Level}";
            allKids.First(k => k.name == "Name").gameObject.GetComponent<TextMeshProUGUI>().text = character.Name;

            fighter.transform.position = Vector2.zero;
            fighter.GetComponent<RectTransform>().localScale = Vector2.one;
            ListOfAllFighers.Add(fighter);
            i++;
        }
        AssignCharacterHPStats.text = 0.ToString();
        AssignCharacterSPStats.text = 0.ToString();
        AssignCharacterATKStats.text = 0.ToString();
        AssignCharacterDEFStats.text = 0.ToString();
        AssignCharacterMAGStats.text = 0.ToString();
        AssignCharacterRESStats.text = 0.ToString();

        Helper.FadeIn(AssignCharacterPanel);
    }

    private void ShowFighterStats(int slotId, int id)
    {
        SelectedCharacterID = id;

        foreach (var item in ListOfAllFighers)
        {
            item.GetComponent<Image>().enabled = false;
        }
        ListOfAllFighers[slotId].GetComponent<Image>().enabled = true;

        Character c = Game.Player.Characters.First(c => c.Id == id);

        AssignCharacterHPStats.text = c.GetStats("HP");
        AssignCharacterSPStats.text = c.GetStats("SP");
        AssignCharacterATKStats.text = c.GetStats("ATK");
        AssignCharacterDEFStats.text = c.GetStats("DEF");
        AssignCharacterMAGStats.text = c.GetStats("MAG");
        AssignCharacterRESStats.text = c.GetStats("RES");

        AssignCharacterSelectButton.interactable = true;

    }

    public void SelectFighter()
    {
        Game.Player.Characters[SelectedCharacterID].IsBusy = true;
        PlayerTeamList[CurrentSlotId].GetComponent<Fighter>().fighter = Game.Player.Characters[SelectedCharacterID];
        RenderPlayerTeam();
        CloseSelectFighter();

    }
    public void CloseSelectFighter()
    {
        Helper.FadeOut(AssignCharacterPanel);
    }
    System.Random random = new System.Random();
    public void GenerateOpponentTeam()
    {
        //Select characters from All Character's list
        //Assign Stats to that Character
        OpponentTeamList.Clear();
        OpponentTeamHPStats.text = 0.ToString();
        OpponentTeamSPStats.text = 0.ToString();
        OpponentTeamATKStats.text = 0.ToString();
        OpponentTeamDEFStats.text = 0.ToString();
        OpponentTeamMAGStats.text = 0.ToString();
        OpponentTeamRESStats.text = 0.ToString();
        foreach (Transform child in OpponentTeamPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int c_count = Game.AllCharachters.Count - 1;
        for (int i = 0; i < 5; i++)
        {
            int x = random.Next(0, c_count);
            Character c = Game.AllCharachters[x].DeepCopy(); //Return New not attached character
            GameObject fighter = Instantiate(CharacterSpacePREFAB);
            fighter.transform.SetParent(OpponentTeamPanel.transform);
            //Might need to have Selection
            c.Level = random.Next(1, c.MaxLvl);
            c.SetLevelStats();
            fighter.name = $"[{c.Class}] {c.Name}";
            fighter.GetComponent<Fighter>().fighter = c;
            fighter.GetComponent<Image>().sprite = c.Sprite;
            fighter.transform.GetChild(0).gameObject.SetActive(false);
            fighter.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = c.Sprite;
            fighter.GetComponent<Button>().enabled = false;
            fighter.GetComponent<Fighter>().ShowRemove = false;
            fighter.transform.position = Vector2.zero;
            fighter.GetComponent<RectTransform>().localScale = Vector2.one;
            OpponentTeamList.Add(fighter);

            //-------Update Stats----------------UnitLevel * 25
            c.TrainedATK = random.Next(0, (c.UnitLevel * 25));
            c.TrainedDEF = random.Next(0, (c.UnitLevel * 25));
            c.TrainedMAG = random.Next(0, (c.UnitLevel * 25));
            c.TrainedRES = random.Next(0, (c.UnitLevel * 25));
            c.Speed = random.Next(0, (c.UnitLevel * 25));
            c.Health = random.Next(0, (c.UnitLevel * 25));
            OpponentTeamHPStats.text = (int.Parse(OpponentTeamHPStats.text) + (int)Math.Floor(double.Parse(c.GetStats("HP")))).ToString();
            OpponentTeamSPStats.text = (int.Parse(OpponentTeamSPStats.text) + (int)Math.Floor(double.Parse(c.GetStats("SP")))).ToString();
            OpponentTeamATKStats.text = (int.Parse(OpponentTeamATKStats.text) + (int)Math.Floor(double.Parse(c.GetStats("ATK")))).ToString();
            OpponentTeamDEFStats.text = (int.Parse(OpponentTeamDEFStats.text) + (int)Math.Floor(double.Parse(c.GetStats("DEF")))).ToString();
            OpponentTeamMAGStats.text = (int.Parse(OpponentTeamMAGStats.text) + (int)Math.Floor(double.Parse(c.GetStats("MAG")))).ToString();
            OpponentTeamRESStats.text = (int.Parse(OpponentTeamRESStats.text) + (int)Math.Floor(double.Parse(c.GetStats("RES")))).ToString();
        }


    }
    // Start is called before the first frame update
    public void GetTheTicket()
    {
        if (Game.Player.ArenaTickets == 0)
        {
            if (Game.Player.isEnoughGold(1000) && Game.Player.Characters.Where(c => c != null).Count() >= 5)
            {
                Game.Player.Resources.Gold -= 1000;
                NotificationManager.Instance.Log("Player Purchased a ticket!");
                Game.Player.ArenaTickets++;
                PreSetArena();
            }
            else
            {
                NotificationManager.Instance.Log("Player do not meet minimum requirements to play in Arena");
            }
        }
        else
        {
            NotificationManager.Instance.Log("Player Used Previously purchased ticket!");
            PreSetArena();
        }

    }
    private void PreSetArena()
    {
        round = 1;
        PlayerPoints = 0;
        OpponentPoints = 0;
        isSpinnerActive = false;
        roundTrigger = 0;
        PlayerFigherSpace.SetActive(false);
        OpponentFigherSpace.SetActive(false);
        _ArenaSpinner.gameObject.SetActive(false);
        StartButton.SetActive(false);
        RenderInitialPlayerTeam();
        GenerateOpponentTeam();
        ClearFightersPlace();
        Helper.FadeIn(ArenaPanel);
        Helper.FadeOut(TicketPanel);
        isTicketBeingUsed = true;
    }
    public void OpenHelp()
    {
        Helper.FadeIn(HelpPanel);
    }
    public void CloseHelp()
    {
        Helper.FadeOut(HelpPanel);
    }
    void Start()
    {




    }

    // Update is called once per frame
    private int prevPlayerIndex = 0;
    private int prevOpponentIndex = 0;
    private static LTDescr delay;
    List<GameObject> AlreadyPlayed = new List<GameObject>();

    public Sprite ATK;
    public Sprite DEF;
    public Sprite MAG;
    public Sprite RES;
    public GameObject PlayerTurn;
    public GameObject OpponentTurn;
    public AudioSource Tick;
    public AudioSource GongSound;
    public GameObject PlayerResult;
    public GameObject OpponentResult;
    public Sprite Check;
    public Sprite Cross;
    void Update()
    {
        if (ArenaPanel!= null && ArenaPanel.activeInHierarchy)
        {
            if (PlayerTeam.Count == 5)
            {
                StepMessage.SetActive(false);

                if (RefreshButton.activeInHierarchy)
                {
                    StartButton.SetActive(true);
                }

            }
            else
            {
                StepMessage.SetActive(true);
                StartButton.SetActive(false);
                PlayerFigherSpace.SetActive(false);
                OpponentFigherSpace.SetActive(false);
                _ArenaSpinner.gameObject.SetActive(false);
            }

            if (RenderTrigger != null && RenderTrigger.text != "0")
            {
                RenderPlayerTeam();
                RenderTrigger.text = 0.ToString();
            }

            if (isSpinnerActive)
            {
                SpinLogo();
            }
        }
    }
    private void RandomPick()
    {
        _ArenaSpinner.gameObject.SetActive(true);
        isSpinnerActive = true;
        PlayerFigherSpace.SetActive(false);
        OpponentFigherSpace.SetActive(false);
        PlayerTurn.SetActive(false);
        OpponentTurn.SetActive(false);
        PlayerResult.SetActive(false);
        OpponentResult.SetActive(false);
        Round.text = $"Round {round.ToString()}";

        DelayedCall(10);


    }

    private void DelayedCall(int call)
    {
        int r = 0, l = 0;
        r = random.Next(0, PlayerTeamList.Count);

        if (r == PlayerTeamList.Count) //make sure that Index is not out of range
            r--;
        if (prevPlayerIndex != PlayerTeamList.Count)
            PlayerTeamList[prevPlayerIndex].transform.GetChild(2).gameObject.SetActive(false);
        
        prevPlayerIndex = r;

        l = random.Next(0, OpponentTeamList.Count);

        if (l == OpponentTeamList.Count) //make sure that Index is not out of range
            l--;
        if (prevOpponentIndex != OpponentTeamList.Count)
            OpponentTeamList[prevOpponentIndex].transform.GetChild(3).gameObject.SetActive(false);

        prevOpponentIndex = l;

        PlayerTeamList[prevPlayerIndex].transform.GetChild(2).gameObject.SetActive(true);
        OpponentTeamList[prevOpponentIndex].transform.GetChild(3).gameObject.SetActive(true);

        delay = LeanTween.delayedCall(.1f, () =>
        {
            //Debug.Log($"[{call}][{Time.deltaTime}] PlayerTeam:[{PlayerTeamList.Count}][{r}] | OpponentTeam:[{OpponentTeamList.Count}][{l}]");
            Tick.Play();

            if(call > 0)
            {
                DelayedCall(--call);
            }
            else
            {
                PlayerFighter = PlayerTeamList[prevPlayerIndex].GetComponent<Fighter>().fighter;
                OpponentFighter = OpponentTeamList[prevOpponentIndex].GetComponent<Fighter>().fighter;
                AlreadyPlayed.Add(PlayerTeamList[prevPlayerIndex]);
                PlayerTeamList[prevPlayerIndex].transform.GetChild(1).gameObject.SetActive(true);   //activate GrayScale
                OpponentTeamList[prevOpponentIndex].transform.GetChild(1).gameObject.SetActive(true);   //activate GrayScale
                PlayerTeamList[prevPlayerIndex].GetComponent<Fighter>().isDone = true;

                PlayerTeamList.RemoveAt(prevPlayerIndex);
                AlreadyPlayed.Add(OpponentTeamList[prevOpponentIndex]);
                OpponentTeamList[prevOpponentIndex].GetComponent<Fighter>().isDone = true;

                OpponentTeamList.RemoveAt(prevOpponentIndex);
                roundTrigger = 0;

                delay = LeanTween.delayedCall(.5f, () =>
                {
                    MoveFighters();
                });
            }

        }); //Just wait
    }
    private Character PlayerFighter = null;
    private Character OpponentFighter = null;

    public ProgressBar PlayerProgressBar;
    public ProgressBar OpponentProgressBar;

    
    private void MoveFighters()
    {

        PlayerFigherSpace.GetComponent<Image>().sprite = PlayerFighter.Sprite;
        PlayerFighter.CombatHP = float.Parse(PlayerFighter.GetStats("HP"));
        PlayerFighter.CombatATK = float.Parse(PlayerFighter.GetStats("ATK"));
        PlayerFighter.CombatDEF = float.Parse(PlayerFighter.GetStats("DEF"));
        PlayerFighter.CombatMAG = float.Parse(PlayerFighter.GetStats("MAG"));
        PlayerFighter.CombatRES = float.Parse(PlayerFighter.GetStats("RES"));
        PlayerProgressBar.current = PlayerFighter.CombatHP;
        PlayerProgressBar.maximum = PlayerFighter.CombatHP;

        OpponentFigherSpace.GetComponent<Image>().sprite = OpponentFighter.Sprite;
        OpponentFighter.CombatHP = float.Parse(OpponentFighter.GetStats("HP"));
        OpponentFighter.CombatATK = float.Parse(OpponentFighter.GetStats("ATK"));
        OpponentFighter.CombatDEF = float.Parse(OpponentFighter.GetStats("DEF"));
        OpponentFighter.CombatMAG = float.Parse(OpponentFighter.GetStats("MAG"));
        OpponentFighter.CombatRES = float.Parse(OpponentFighter.GetStats("RES"));
        OpponentProgressBar.current = OpponentFighter.CombatHP;
        OpponentProgressBar.maximum = OpponentFighter.CombatHP;

        PlayerFigherSpace.SetActive(true);
        OpponentFigherSpace.SetActive(true);
        delay = LeanTween.delayedCall(1f, () => {
            StartRound();
        });
    }
    public void StartFirstRound()
    {
        if (PlayerTeam.Count == 5)
        {
            Game.Player.ArenaTickets--;
            AlreadyPlayed.Clear();

            foreach (var button in TabButtons)
            {
                button.interactable = false;
            }
            TriggerStart();
        }
        else
        {
            NotificationManager.Instance.Log($"Your team still needs {5 - PlayerTeam.Count} Fighters. Please select them.");
        }
    }
    private void TriggerStart()
    {
        PlayerPoints = (PlayerPoints == 0 ? 0 : PlayerPoints++);
        OpponentPoints = (OpponentPoints == 0 ? 0 : OpponentPoints++);
        foreach (var item in AlreadyPlayed)
        {
            item.transform.GetChild(2).gameObject.SetActive(false);
            item.transform.GetChild(3).gameObject.SetActive(false);
        }
        PlayerFigherSpace.SetActive(false);
        OpponentFigherSpace.SetActive(false);
        PlayerFigherSpace.GetComponent<Image>().sprite = DefaultSprite;
        OpponentFigherSpace.GetComponent<Image>().sprite = DefaultSprite;
        PlayerProgressBar.current = 0;
        PlayerProgressBar.maximum = 0;
        OpponentProgressBar.current = 0;
        OpponentProgressBar.maximum = 0;
        PlayerTurn.SetActive(false);
        OpponentTurn.SetActive(false);
        PlayerResult.SetActive(false);
        OpponentResult.SetActive(false);
        Round.text = $"Round {round.ToString()}";
        StartButton.SetActive(false);
        RefreshButton.SetActive(false);
        GongSound.Play();
        delay = LeanTween.delayedCall(3f, () => { RandomPick(); });

    }
    private int roundTrigger = 0;
    private bool isPlayerFirst = false;
    private bool setOnce = false;
    bool isEnd = false;
    string player = "attack";
    string opponet = "attack";
    public void StartRound()
    {
        Round.text = $"Round {round.ToString()}";
        Score.text = $"{PlayerPoints}:{OpponentPoints}";

        isSpinnerActive = true;
        //
        //Determaine who starts first by comparins speeds:
        if (PlayerFighter.GetSpeed() >= OpponentFighter.GetSpeed() && !setOnce)
        {
            isPlayerFirst = true;
        }
        if (!setOnce)
            setOnce = true;

        
        List<string> useMagic = new List<string>{ "apprentice", "mage", "healer" };
        player = "attack";
        opponet = "attack";
        if (useMagic.Any(PlayerFighter.Class.Contains))
            player = "mag";
          
        if (useMagic.Any(OpponentFighter.Class.Contains))
            opponet = "mag";



        delay = LeanTween.delayedCall(1f, () => {
            TakeTurns();
        });
    }

    private void TakeTurns()
    {

       if (isPlayerFirst) //Player Attacks
        {
            if (player == "attack" && opponet == "attack") //if both use normal ATTACK POINTS
            {
                PlayerTurn.GetComponent<Image>().sprite = ATK;
                OpponentTurn.GetComponent<Image>().sprite = DEF;

                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);

                OpponentTurn.GetComponent<AudioSource>().Play();

                isEnd = Fight(PlayerFighter, OpponentFighter, "a&a", OpponentProgressBar);

            }
            else if (player == "attack" && opponet == "mag")
            {

                PlayerTurn.GetComponent<Image>().sprite = ATK;
                OpponentTurn.GetComponent<Image>().sprite = DEF;

                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);

                OpponentTurn.GetComponent<AudioSource>().Play();

                isEnd = Fight(PlayerFighter, OpponentFighter, "a&m", OpponentProgressBar);
            }
            else if (player == "mag" && opponet == "attack")
            {
                PlayerTurn.GetComponent<Image>().sprite = MAG;
                OpponentTurn.GetComponent<Image>().sprite = RES;

                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);

                PlayerTurn.GetComponent<AudioSource>().Play();

                isEnd = Fight(PlayerFighter, OpponentFighter, "m&a", OpponentProgressBar);
            }
            else if (player == "mag" && opponet == "mag")
            {
                PlayerTurn.GetComponent<Image>().sprite = MAG;
                OpponentTurn.GetComponent<Image>().sprite = RES;
                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);
                PlayerTurn.GetComponent<AudioSource>().Play();
                isEnd = Fight(PlayerFighter, OpponentFighter, "m&m", OpponentProgressBar);
            }
        } else //Opponent Attacks
        {
            if (opponet == "attack" && player == "attack") //if both use normal ATTACK POINTS
            {
                OpponentTurn.GetComponent<Image>().sprite = ATK;

                PlayerTurn.GetComponent<Image>().sprite = DEF;
                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);

                OpponentTurn.GetComponent<AudioSource>().Play();

                isEnd = Fight(OpponentFighter, PlayerFighter, "a&a", PlayerProgressBar);
            }
            else if (opponet == "mag" && player == "attack")
            {
                OpponentTurn.GetComponent<Image>().sprite = MAG;
                PlayerTurn.GetComponent<Image>().sprite = RES;
                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);
                PlayerTurn.GetComponent<AudioSource>().Play();
                isEnd = Fight(OpponentFighter, PlayerFighter, "m&a", PlayerProgressBar);
            }
            else if (opponet == "attack" && player == "mag")
            {
                OpponentTurn.GetComponent<Image>().sprite = ATK;

                PlayerTurn.GetComponent<Image>().sprite = DEF;
                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);

                OpponentTurn.GetComponent<AudioSource>().Play();

                isEnd = Fight(OpponentFighter, PlayerFighter, "a&m", PlayerProgressBar);
            }
            else if (opponet == "mag" && player == "mag")
            {
                OpponentTurn.GetComponent<Image>().sprite = MAG;
                PlayerTurn.GetComponent<Image>().sprite = RES;
                PlayerTurn.SetActive(true);
                OpponentTurn.SetActive(true);
                PlayerTurn.GetComponent<AudioSource>().Play();
                isEnd = Fight(OpponentFighter, PlayerFighter, "m&m", PlayerProgressBar);
            }
        }

        isPlayerFirst = !isPlayerFirst; //toggle   

        if (isEnd)
        {
            //TODO: STOP Figure out who WON
            //UPDATE SCORES
            round++;
            if (OpponentFighter.CombatHP <= 0)
            {
                PlayerPoints++;
                PlayerResult.GetComponent<Image>().sprite = Check;
                OpponentResult.GetComponent<Image>().sprite = Cross;
            }
                
            else if (PlayerFighter.CombatHP <= 0)
            {
                OpponentPoints++;
                PlayerResult.GetComponent<Image>().sprite = Cross;
                OpponentResult.GetComponent<Image>().sprite = Check;
            }
                

            PlayerResult.SetActive(true);
            OpponentResult.SetActive(true);

            //StartRound();
            Score.text = $"{PlayerPoints.ToString()}:{OpponentPoints.ToString()}";
            if(round != 4) //max rounds 3
            {
                delay = LeanTween.delayedCall(2f, () => {
                    PlayerFigherSpace.SetActive(false);
                    OpponentFigherSpace.SetActive(false);
                    TriggerStart();
                });
            }
            else
            {
                delay = LeanTween.delayedCall(2f, () => {
                    PlayerFigherSpace.SetActive(false);
                    OpponentFigherSpace.SetActive(false);
                    ShowFinaleResult();
                });
            }

        }
        else
        { //Delay between attacks
            delay = LeanTween.delayedCall(1f, () => {
                TakeTurns();
            });
        }
 
    }
    private int XPrewards = 0;
    private List<Accessories> rewards = new List<Accessories>();
    public TextMeshProUGUI PurchaseButtonText;
    bool isTicketBeingUsed = false;
    public void SetupEnvironment()
    {
        if (!isTicketBeingUsed)
        {
            if (Game.Player.ArenaTickets > 0)
            {
                PurchaseButtonText.text = "User Your Ticket";
            }
            else
            {
                PurchaseButtonText.text = "Purchase Ticket";
            }
            Helper.FadeOut(ArenaPanel);
            Helper.FadeIn(TicketPanel);
            GenerateRewards();
            //isTicketBeingUsed = true;
        }


    }
    public void GenerateRewards()
    {
        //Trun OFF all Panels
        XPrewards = random.Next(100, 1000);
        rewards.Clear();
        for (int i = 0; i < 4; i++)
        {
            int r = random.Next(0, Game.AllAccessories.Count - 1);
            rewards.Add(new Accessories(Game.AllAccessories[r]));
        }

        //clear rewards holder
        foreach (Transform child in RewardsPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //create new list

        GameObject XPitemObj = Instantiate(RewardPREFAB);
        XPitemObj.name = "XP_reward";

        XPitemObj.transform.SetParent(RewardsPanel.transform);
        XPitemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("xp");
        XPitemObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = String.Format("{0:n0}", XPrewards);
        XPitemObj.transform.position = Vector2.zero;
        XPitemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        XPitemObj.GetComponent<RectTransform>().localScale = Vector2.one;

        foreach (var reward in rewards)
        {
            GameObject itemObj = Instantiate(RewardPREFAB);
            itemObj.name = reward.Name;

            itemObj.transform.SetParent(RewardsPanel.transform);
            itemObj.GetComponent<Image>().sprite = reward.Sprite;
            itemObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = String.Format("{0:n0}", 1);
            itemObj.transform.position = Vector2.zero;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
        }


    }
    private bool Fight(Character Attack, Character Opponent, string strategy, ProgressBar progressBar)
    {
        bool isTheEndYet = false;
        float ratio = 0;
        switch (strategy)
        {
            case "a&a":  //use attack and defence
                ratio = (Attack.CombatATK / 4) / Opponent.CombatDEF;
                Opponent.CombatHP -= (Opponent.CombatHP * ((ratio > 1) ? 1 : ratio));
                progressBar.current = Opponent.CombatHP;
                if (Opponent.CombatHP > 0)
                    Opponent.CombatDEF -= Attack.CombatATK / 5;// () >= 0 ? (Attack.CombatATK / 2) : 1;
                else
                    isTheEndYet = true;
                break;
            case "a&m":  //use attack and defence && use magic and resistance
                ratio = (Attack.CombatATK / 4) / Opponent.CombatRES;
                Opponent.CombatHP -= (Opponent.CombatHP * ((ratio > 1) ? 1 : ratio));
                progressBar.current = Opponent.CombatHP;
                if (Opponent.CombatHP > 0)
                    Opponent.CombatRES -= Attack.CombatATK / 5;// () >= 0 ? (Attack.CombatATK / 2) : 1;
                else
                    isTheEndYet = true;
                break;
            case "m&a":  //use magic and resistance && use attack and defence
                ratio = (Attack.CombatMAG / 4) / Opponent.CombatDEF;
                Opponent.CombatHP -= (Opponent.CombatHP * ((ratio > 1) ? 1 : ratio));
                progressBar.current = Opponent.CombatHP;
                if (Opponent.CombatHP > 0)
                    Opponent.CombatDEF -= Attack.CombatMAG / 5;// () >= 0 ? (Attack.CombatMAG / 2) : 1;
                else
                    isTheEndYet = true;
                break;
            case "m&m":  //use magic and resistance
                ratio = (Attack.CombatMAG / 4) / Opponent.CombatRES;
                Opponent.CombatHP -= (Opponent.CombatHP * ((ratio > 1) ? 1 : ratio));
                progressBar.current = Opponent.CombatHP;
                if (Opponent.CombatHP > 0)
                    Opponent.CombatRES -= Attack.CombatMAG / 5;// () >= 0 ? (Attack.CombatMAG / 2) : 1;
                else
                    isTheEndYet = true;
                Debug.Log($"Ratio:{ratio} | Opponent.CombatHP:{Opponent.CombatHP} | Opponent.CombatDEF:{Opponent.CombatDEF}");
                break;
            default:
                break;
        }

        return isTheEndYet;
    } 
    private void ShowFinaleResult()
    {
        if(PlayerPoints >= OpponentPoints)
        {
            Message.text = "Congradulations!\nYour Team WON!";
             //clear rewards holder
            foreach (Transform child in FinalRewards.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            //create new list

            GameObject XPitemObj = Instantiate(RewardPREFAB);
            XPitemObj.name = "XP_reward";

            XPitemObj.transform.SetParent(FinalRewards.transform);
            XPitemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("xp");
            XPitemObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = String.Format("{0:n0}", XPrewards);
            XPitemObj.transform.position = Vector2.zero;
            XPitemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            XPitemObj.GetComponent<RectTransform>().localScale = Vector2.one;

            foreach (var reward in rewards)
            {
                GameObject itemObj = Instantiate(RewardPREFAB);
                itemObj.name = reward.Name;

                itemObj.transform.SetParent(FinalRewards.transform);
                itemObj.GetComponent<Image>().sprite = reward.Sprite;
                itemObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = String.Format("{0:n0}", 1);
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            }

            FinalRewardsPanel.SetActive(true);
            ButtonMessage.text = "Close";
            //Something to add New items to the Player
            foreach (int id in PlayerTeam)
            {
                Game.Player.Characters[id].Experience += XPrewards;
                Game.Player.Characters[id].LevelUp();
                Game.Player.Characters[id].IsBusy = false;
            }
            NotificationManager.Instance.Log($"Each player that went to Fight Arena was rewarded with [{XPrewards}] points");
            int startID = Game.Player.Accessories.Last().ID + 1;
            foreach (var reward in rewards)
            {
                Game.Player.Accessories.Add(new Accessories(reward));
                Game.Player.Accessories.Last().ID = startID++;
                NotificationManager.Instance.Log($"Player was rewarded with [{reward.Name}] accessory");
            }
            


        }
        else
        {
            FinalRewardsPanel.SetActive(false);
            Message.text = "Sorry!\nYour Team did not WIN!";
            ButtonMessage.text = "Close";
        }
        foreach (int x in PlayerTeam)
        {
            Game.Player.Characters[x].IsBusy = false;
        }
        PlayerTeam.Clear();

        Helper.FadeIn(FinalMessagePanel);
    }
    public void CloseFinalMessagePanel()
    {
        foreach (var button in TabButtons)
        {
            button.interactable = true;
        }
        Helper.FadeOut(FinalMessagePanel);
        isTicketBeingUsed = false;
        SetupEnvironment();

    }
    public RectTransform _ArenaSpinner;
    private float _timeStep = .05f;
    private float _oneStepAngle = 3;
    private bool isSpinnerActive = false;
    private float _startTime;


    private void SpinLogo()
    {
        /*----https://www.youtube.com/watch?v=ltu27NLeIWc----*/
        if (Time.time - _startTime >= _timeStep)
        {
            Vector3 iconAngle = _ArenaSpinner.localEulerAngles;
            iconAngle.z -= _oneStepAngle;
            _ArenaSpinner.localEulerAngles = iconAngle;
            _startTime = Time.time;
        }

    }
}
