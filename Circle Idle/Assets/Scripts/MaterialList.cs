using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialList : MonoBehaviour
{
    public TextMeshProUGUI txtPlayerName;
    public TextMeshProUGUI txtPlayerLevel;

    public TextMeshProUGUI txtPlayerGold;
    public Image imgPlayerGold;

    public TextMeshProUGUI txtPlayerIron;
    public Image imgPlayerIron;

    public TextMeshProUGUI txtPlayerCopper;
    public Image imgPlayerCopper;

    public TextMeshProUGUI txtPlayerLumber;
    public Image imgPlayerLumber;

    public TextMeshProUGUI txtPlayerOak;
    public Image imgPlayerOak;

    public TextMeshProUGUI txtPlayerHickory;
    public Image imgPlayerHickory;

    public TextMeshProUGUI txtPlayerStone;
    public Image imgPlayerStone;

    public TextMeshProUGUI txtPlayerGems;
    public Image imgPlayerGems;

    public TextMeshProUGUI txtPlayerOre;
    public Image imgPlayerOre;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

        if (Game.Player != null)
        {
            txtPlayerName.text = Game.Player.DisplayName;
            txtPlayerLevel.text = "Lv. " + Game.Player.PlayerLevel + " Adventurer";
            txtPlayerGold.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Gold);
            txtPlayerIron.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Iron);
            txtPlayerCopper.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Copper);
            txtPlayerLumber.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Lumber);
            txtPlayerOak.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Oak);
            txtPlayerHickory.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Hickory);
            txtPlayerStone.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Stone);
            txtPlayerGems.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Gems);
            txtPlayerOre.text = ": " + String.Format("{0:n0}", Game.Player.Resources.Ore);
        }else
        {
            txtPlayerName.text = "NO PLAYER";
            txtPlayerLevel.text = "Lv. 0";
            txtPlayerGold.text = ": 0";
            txtPlayerIron.text = ": 0";
            txtPlayerCopper.text = ": 0";
            txtPlayerLumber.text = ": 0";
            txtPlayerOak.text = ": 0";
            txtPlayerHickory.text = ": 0";
            txtPlayerStone.text = ": 0";
            txtPlayerGems.text = ": 0";
            txtPlayerOre.text = ": 0";
        }

    }
}
