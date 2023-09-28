using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SceneHelper.cs
//This script handles changing menu screens.
//Current implementation is based on the "tab system" implementation this video: https://www.youtube.com/watch?v=BXrrh95XKzA

public class SceneHelper : MonoBehaviour
{
    //Menu screens displayed from MenuCanvas
    public CanvasGroup TownMenu;
    public CanvasGroup CharacterListMenu;
    public CanvasGroup EquipmentMenu;
    public CanvasGroup DungeonMenu;
    public CanvasGroup FightArenaMenu;

    public void CanvasGroupChanger(bool selected, CanvasGroup group)
    {
        //Menu active? Display and allow the user to interact
        if (selected)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
            return;
        }

        //Else, hide and don't allow user to interact
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    // Start is called before the first frame update
    // Update display status for all groups
    // Hide all else, and choose which menu to display first (Behavior currently set to hide all menus)
    void Start()
    {
        ChangeMenu(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMenu(int sel) //Switch active menu to inputted enum
    {
        switch (sel)
        {
            case 0: //Clear menu
                CanvasGroupChanger(false, TownMenu);
                CanvasGroupChanger(false, CharacterListMenu);
                CanvasGroupChanger(false, EquipmentMenu);
                CanvasGroupChanger(false, DungeonMenu);
                CanvasGroupChanger(false, FightArenaMenu);
                break;
            case 1: //Town
                CanvasGroupChanger(true, TownMenu); //active
                CanvasGroupChanger(false, CharacterListMenu);
                CanvasGroupChanger(false, EquipmentMenu);
                CanvasGroupChanger(false, DungeonMenu);
                CanvasGroupChanger(false, FightArenaMenu);
                break;
            case 2: //CharacterList
                CanvasGroupChanger(false, TownMenu);
                CanvasGroupChanger(true, CharacterListMenu); //active
                CanvasGroupChanger(false, EquipmentMenu);
                CanvasGroupChanger(false, DungeonMenu);
                CanvasGroupChanger(false, FightArenaMenu);
                break;
            //case 3: //CharacterInfo
            //    CanvasGroupChanger(false, TownMenu);
            //    CanvasGroupChanger(false, CharacterListMenu);
            //    CanvasGroupChanger(false, EquipmentMenu);
            //    CanvasGroupChanger(false, DungeonMenu);
            //    CanvasGroupChanger(false, FightArenaMenu);
            //    break;
            case 4: //EquipmentMenu
                CanvasGroupChanger(false, TownMenu);
                CanvasGroupChanger(false, CharacterListMenu);
                CanvasGroupChanger(true, EquipmentMenu); //active
                CanvasGroupChanger(false, DungeonMenu);
                CanvasGroupChanger(false, FightArenaMenu);
                break;
            case 5: //DungeonMenu
                CanvasGroupChanger(false, TownMenu);
                CanvasGroupChanger(false, CharacterListMenu);
                CanvasGroupChanger(false, EquipmentMenu);
                CanvasGroupChanger(true, DungeonMenu); //active
                CanvasGroupChanger(false, FightArenaMenu);
                break;
            case 6: //Fight Arena Menu
                CanvasGroupChanger(false, TownMenu);
                CanvasGroupChanger(false, CharacterListMenu);
                CanvasGroupChanger(false, EquipmentMenu);
                CanvasGroupChanger(false, DungeonMenu);
                CanvasGroupChanger(true, FightArenaMenu); //active
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void SaveGame()
    {
        Debug.Log("Manual Game Save Activated");
    }
}
