using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleIdleLib;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;
using Assets.Scripts.GameClassDefinitions;
using UnityEngine.EventSystems;

public class PlayerMenu : MonoBehaviour
{
    [Space(10)]
    [Header("Player Menu Panel")]
    public GameObject playerMenuPanel;
    public Image imgPlayerProfilePicture;
    public Image imgPlayerAvatar;
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtPlayerInformation;
    public TextMeshProUGUI txtPlayerLevel;
    public TextMeshProUGUI txtPlayerEXP;
    public ProgressBar pgbarPlayerLevelProgress;
    //public TextMeshProUGUI txtCombatInformation;

    [Space(10)]
    [Header("Info Change Panel")]
    public GameObject infoChangePanel;
    public GameObject changeAvatarGrid;
    public GameObject changeAvatarPanel;
    [Space(10)]
    public GameObject displayAvatar;
    public TMP_InputField displayNameInputField;
    public GameObject LeaderBoardPanel;
    public LeaderBoard LeaderBoard;
    [HideInInspector] public int numAvatars = 9;
    [HideInInspector] public List<GameObject> avatarSlots = new List<GameObject>();
    [HideInInspector] public List<Leader> leaders = new List<Leader>();
    // Start is called before the first frame update
    void Start()
    {
        displayAvatar.GetComponent<Image>().sprite = Game.Player.Avatar;
        displayNameInputField.text = Game.Player.DisplayName;
        //Game.avatars[0].FileName = "avatar/default";
        //Game.avatars[1].FileName = "avatar/maleavatar00";
        //Game.avatars[2].FileName = "avatar/femaleavatar00";
        //Game.avatars[3].FileName = "avatar/maleavatar01";
        //Game.avatars[4].FileName = "avatar/femaleavatar01";
        //Game.avatars[5].FileName = "avatar/maleavatar02";
        //Game.avatars[6].FileName = "avatar/femaleavatar02";
        //Game.avatars[7].FileName = "avatar/maleavatar03";
        //Game.avatars[8].FileName = "avatar/femaleavatar03";
        
        //RenderAllAvatars();
    }
    public Image Volume;
    public Sprite VolumeON;
    public Sprite VolumeOFF;
    public void ChangeVolume()
    {
        Game.Player.GameBackgroundMusic = (Game.Player.GameBackgroundMusic == 0 ? 1 : 0);
    }

    private bool isBoardLoaded = false;
    // Update is called once per frame
    void Update()
    {
        if (Game.Player != null)
        {
            txtPlayerName.text = Game.Player.DisplayName;
            txtPlayerLevel.text = Game.Player.PlayerLevel.ToString();
            //imgPlayerProfilePicture.GetComponent<Image>().sprite = Resources.Load<Sprite>("default");
            imgPlayerAvatar.GetComponent<Image>().sprite = Game.Player.Avatar;
            SetPlayerInformation();
            //SetCombatInformation();
            txtPlayerEXP.text = Game.Player.ExperiencePoints.ToString() + " / " + Game.Player.ExpNext.ToString();
            pgbarPlayerLevelProgress.maximum = Game.Player.ExpNext;
            pgbarPlayerLevelProgress.current = Game.Player.ExperiencePoints;

            if (LeaderBoardPanel.activeInHierarchy && isBoardLoaded != true)
            {
                LeaderBoard.SetupTheBoard();
                Debug.Log("Leader Board Panel is Active!!!");
                isBoardLoaded = true;
            }

            if (Game.Player.GameBackgroundMusic == 0)
                Volume.sprite = VolumeOFF;
            else
                Volume.sprite = VolumeON;

             
            
        }
    }

    public void SetPlayerInformation()
    {
        txtPlayerInformation.text =
            "Level:\t\t\t\t" + Game.Player.PlayerLevel + "\n" +
            "\n" +
            "Gold:\t\t\t\t" + Game.Player.Resources.Gold + "\n" +
            "\n" +
            "Character Slots:\t\t" + Game.Player.MaxCharacters + "\n" +
            "Building Slots:\t\t" + Game.Player.MaxBuildings + "\n" +
            "Inventory Slots:\t\t" + Game.Player.MaxInventory;
    }

    public void UpdateDisplayAvatar_OnClick()
    {
        displayAvatar.GetComponent<Image>().sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
    }

    public void AcceptDisplayInfo_OnClick()
    {
        Game.Player.DisplayName = displayNameInputField.text;
        Game.Player.Avatar = displayAvatar.GetComponent<Image>().sprite;
        Helper.FadeOut(infoChangePanel);
    }

    public void OpenPlayerMenu()
    {
        leaders.Clear();
        isBoardLoaded = false;
        Helper.FadeIn(playerMenuPanel);
    }
    public void ClosePlayerMenu()
    {
        Helper.FadeOut(playerMenuPanel);
        
    }

    public void OpenInfoChangeMenu()
    {
        displayAvatar.GetComponent<Image>().sprite = Game.Player.Avatar;
        displayNameInputField.text = Game.Player.DisplayName;
        Helper.FadeIn(infoChangePanel);
    }

    public void CloseInfoChangeMenu()
    {
        displayAvatar.GetComponent<Image>().sprite = Game.Player.Avatar;
        displayNameInputField.text = Game.Player.DisplayName;
        Helper.FadeOut(infoChangePanel);
    }


}
