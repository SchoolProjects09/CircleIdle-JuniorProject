using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Compression;

public class SaveGame : MonoBehaviour
{
    public GameObject SaveButton;
    private int SaveGameTrigger = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {/**/
        //if (Game.Player.Email == "")
        //    SaveButton.SetActive(false);
        //else
        //   
        SaveButton.SetActive(true);
        if((DateTime.Now.Minute % 5 == 0) && DateTime.Now.Second == 0 && SaveGameTrigger > 100)    //Time 5 Minutes and 00 Seconds && saveTrigger Count is > 10
        { 
            SaveGameTrigger = 0;
        }
        else
        {
            SaveGameTrigger++;
        }

        if (SaveGameTrigger == 0)
        {
            SaveCurrentGame();  
        }

    }

    public void SaveCurrentGame()
    {
        SaveGameTrigger++;
        Helper.LocalSave();
        if(Game.Player.Username != "guest")
        {
            string convertedObject = Helper.PlayerToJSON();
            StartCoroutine(Game.DataBase.SaveGame(convertedObject));
        }

        

        NotificationManager.Instance.Log("Game saved @" + DateTime.Now.ToString("G"));
    }

    void Awake()
    {
        Application.targetFrameRate = 10;
    }



}
