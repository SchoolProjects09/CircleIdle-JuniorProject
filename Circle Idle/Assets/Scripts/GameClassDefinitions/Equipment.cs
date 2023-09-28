using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace CircleIdleLib
{
    public class Equipment:iInventory
    {
        // EQUIPMENT INFORMATION
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int HP { get; set; }
        public int SP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int MAG { get; set; }
        public int RES { get; set; }
        public double Bonus { get; set; }
        public string Protect { get; set; }
        public bool isAssigned { get; set; }
        public int CharacterId { get; set; }
        public bool isDefault { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public Sprite Sprite { get; set; }
        public Sprite SpriteTransparent { get; set; }
        public Dictionary<string, int> Recipe { get; set; }
        public int FinishedProgress { get; set; }

        public Equipment(JSONNode data)
        {
            Name = data["name"];
            Description = data["description"];
            Icon = data["icon"];
            HP = data["HP"];
            SP = data["SP"];
            ATK = data["ATK"];
            DEF = data["DEF"];
            MAG = data["MAG"];
            RES = data["RES"];
            Class = data["icon"];
            this.Sprite = Resources.Load<Sprite>("Equipment/" + data["icon"]);
            this.SpriteTransparent = Resources.Load<Sprite>("Equipment/t/" + data["icon"]+"_t");
            FinishedProgress = data["finishedProgress"]*10;
            Protect = data["protect"];
            Recipe = new Dictionary<string, int>();
            foreach (var item in data["recipe"])
                Recipe.Add(item.Key.ToString(), item.Value);

            CharacterId = -1;
            isAssigned = false;
            isDefault = true;
        }

        public Equipment(string name)
        {
            Equipment equipment = (Equipment)Game.AllEquipment.First(e => e.Class.ToLower() == name.ToLower());
            Name = equipment.Name;
            Description = equipment.Description;
            Icon = equipment.Icon;
            HP = equipment.HP;
            SP = equipment.SP;
            ATK = equipment.ATK;
            DEF = equipment.DEF;
            MAG = equipment.MAG;
            RES = equipment.RES;
            Class = equipment.Class;
            this.Sprite = equipment.Sprite;
            Protect = equipment.Protect;
            this.SpriteTransparent = equipment.SpriteTransparent;
            FinishedProgress = equipment.FinishedProgress;
            Recipe = new Dictionary<string, int>();
            foreach (var item in equipment.Recipe)
                Recipe.Add(item.Key.ToString(), item.Value);
            isAssigned = false;
            isDefault = true;
            CharacterId = -1;
        }

        public string[] SortCriterias()
        {
            return new string[] { "Name", "Health Points", "Speed Points", "Attack", "Defence", "Magic", "Resistance" };
        }
        public string GetSortProperty(string property)
        {
            switch (property)
            {
                case "Name":
                    return "Name";
                case "Health Points":
                    return "HP";
                case "Speed Points":
                    return "SP";
                case "Attack":
                    return "ATK";
                case "Defence":
                    return "DEF";
                case "Magic":
                    return "MAG";
                case "Resistance":
                    return "RES";
                default:
                    return "";
            }
            
        }

        public string ToJSON(int index)
        {
            string assignedSlot;

            if (isAssigned)
                assignedSlot = Protect;
            else
                assignedSlot = "n";

            return "{" + String.Format("\"name\":\"{0}\"," +
                "\"icon\":\"{1}\"," +
                "\"HP\":{2}," +
                "\"SP\":{3}," +
                "\"ATK\":{4}," +
                "\"DEF\":{5}," +
                "\"MAG\":{6}," +
                "\"RES\":{7}," +
                "\"i\":\"{8}\"," +
                "\"d\":{9}," +
                "\"a\":{10}," +
                "\"s\":\"{11}\"", 
                Name, Icon, HP, SP, ATK, DEF, MAG, RES, Class, isDefault.ToString().ToLower(), CharacterId, assignedSlot) + "}";
        }

        // TODO: MAKE EQUIPMENT AN ABSTRACT CLASS
        // SO WE CAN ADD ABSTRACT METHODS FOR STAT INCREASES
    }
}
