using CircleIdleLib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;
using System;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.GameClassDefinitions;

public static class Game
{
    // Start is called before the first frame update
    public static GamePlayer Player;
    public static JSONNode player_data;
    public static JSONNode char_data;
    public static JSONNode resources_data;
    public static JSONNode equipment_data;
    public static JSONNode weapon_data;
    public static JSONNode accessories_data;
    public static JSONNode building_data;
    public static JSONNode randomNames;
    public static List<Character> AllCharachters = new List<Character>();
    public static List<iInventory> AllResources = new List<iInventory>();
    public static List<iInventory> AllEquipment = new List<iInventory>();
    public static List<iInventory> AllWeapons = new List<iInventory>();
    public static List<iInventory> AllAccessories = new List<iInventory>();
    public static List<iInventory> AllTrainingAttributes = new List<iInventory>();
    public static List<PlayerAvatar> avatars = new List<PlayerAvatar>();

    public static List<Building> AllBuildings = new List<Building>();
    public static System.Random random = new System.Random();
    public const int CHARACTER_MAX_LEVEL = 3;
    public static CircleIdleDataBase DataBase;
    public static void GetStarted()
    {
        randomNames = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Names.json"));
        player_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Player.json"));
        char_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/CharacterClasses.json"));
        resources_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Resources.json"));
        equipment_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Equipment.json"));
        weapon_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapons.json"));
        accessories_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Accessories.json"));
        building_data = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Buildings.json"));
        
        LoadResources(resources_data["resources"]);
        LoadCharacters(char_data["characters"]);
        LoadEquipment(equipment_data["equipment"]);
        LoadWeapons(weapon_data["weapons"]);
        LoadAccessories(accessories_data["accessories"]);
        LoadBuildings(building_data["buildings"]);
        LoadTrainingAttributes();

        DataBase = new CircleIdleDataBase();

        Debug.Log("Json loaded successfully");
        Player = null;

        //To test without login uncomment lines 55-64 and uncomment line #14 in Main.cs

        //Player = new GamePlayer("test"); //Uses default constructor with some resources
        //Player.SetCharIds();

        ////Only needed in the beggining of the game

        //foreach (Town item in Player.Town)
        //{
        //    if (item.CharacterId != -1)
        //        item.SetupAssignedCharacter();
        //}


    }

    private static void LoadTrainingAttributes()
    {
        string[] TrainingAttributes = new string[] { "Attack", "Defence", "Magic", "Resistance" };
        foreach (string attr in TrainingAttributes)
        {
            AllTrainingAttributes.Add(new TraningAttributes(attr));
        }
    }

    private static void LoadResources(JSONNode jSONNode)
    {
        foreach (JSONNode item in jSONNode)
        {
            AllResources.Add(new GameResources(item));
        }
        Debug.Log("Resources loaded");

    }
    private static void LoadAccessories(JSONNode jSONNode)
    {
        foreach (JSONNode item in jSONNode)
        {
            AllAccessories.Add(new Accessories(item));
            //treat Accessory as equipment 
        }
        Debug.Log("Accessories loaded");

    }
    private static void LoadCharacters(JSONNode jSONNode)
    {
        int index = 0;
        foreach (var item in jSONNode)
        {
            AllCharachters.Add(new Character(index, item, item.Key.ToString()));
            AllCharachters[index++].Name = GetRandomName();
        }
        Debug.Log("Characters loaded");

    }

    private static void LoadEquipment(JSONNode jsonNode)
    {
        foreach (var item in jsonNode)
        {
            AllEquipment.Add(new Equipment(item));
        }
        Debug.Log("Equipment loaded");
    }

    private static void LoadWeapons(JSONNode jsonNode)
    {
        foreach (var item in jsonNode)
        {
            AllWeapons.Add(new Weapon(item));
        }
        Debug.Log("Weapons loaded");
    }

    private static void LoadBuildings(JSONNode jsonNode)
    {
        int index = 0;
        foreach (var item in jsonNode)
        {
            AllBuildings.Add(new Building(item, index));
            index++;
        }
        Debug.Log("Buildings Loaded");
    }

    public static string GetRandomName()
    {
        int num = random.Next(0, randomNames.Count);
        return randomNames[num];
    }

    // Update is called once per frame
    public static void UpdateStats(TextMeshProUGUI PlayerStats)
    {
        
        
    }


}
