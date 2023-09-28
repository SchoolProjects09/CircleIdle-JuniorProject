using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleIdleLib
{
    public class GameResources: iInventory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Icon { get; set; }
        public int HP { get; set; }
        public int SP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MAG { get; set; }
        public int RES { get; set; }
        public double Bonus { get; set; }
        public bool isAssigned { get; set; }
        public string Protect { get; set; }
        public int CharacterId { get; set; }
        public bool isDefault { get; set; }
        public string Type { get; set; }
        public Sprite Sprite { get; set; }
        public Sprite SpriteTransparent { get; set; }
        public int UnlockLevel { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> Recipe { get; set; }
        public int FinishedProgress { get; set; }

        //Used during the game 
        public GameResources(string name, string classs, int unlockLevel)
        {
            this.Name = name;
            this.Class = classs;
            this.Icon = classs;
            this.UnlockLevel = unlockLevel;
            this.Sprite = Resources.Load<Sprite>("building/" + classs);
            this.SpriteTransparent = Resources.Load<Sprite>("building/t/" + classs);
            Recipe = new Dictionary<string, int>();
            FinishedProgress = Game.AllResources.First(r => r.Class == classs).FinishedProgress;
        }
        //default Constructor
        public GameResources(JSONNode data)
        {
            this.Name = data["name"];
            this.Class = data["class"];
            this.Icon = data["class"];
            this.UnlockLevel = data["unlockLevel"];
            this.Sprite = Resources.Load<Sprite>("building/" + data["class"]);
            this.SpriteTransparent = Resources.Load<Sprite>("building/t/" + data["icon"]);
            Recipe = new Dictionary<string, int>();
            FinishedProgress = data["finishedProgress"]*5;
        }

        public string[] SortCriterias() {
            return new string[] { };
        }
        public string GetSortProperty(string property)
        {
            return "";
        }
        public string ToJSON(int index)
        {
            return "";
        }
    }
}
