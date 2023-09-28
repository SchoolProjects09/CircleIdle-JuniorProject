using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace CircleIdleLib
{
    public class Weapon : iInventory
    {
        // WEAPON INFORMATION
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
        public string Protect { get; set; }
        public bool isAssigned { get; set; }
        public int CharacterId { get; set; }
        public bool isDefault { get; set; }
        public string Type { get; set; }
        public double Bonus { get; set; }
        public Sprite Sprite { get; set; }
        public Sprite SpriteTransparent { get; set; }
        public string Class { get; set; }
        public Dictionary<string, int> Recipe { get; set; }
        public int FinishedProgress { get; set; }


        public Weapon(JSONNode data)
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
            Type = data["type"];
            Bonus = data["bonus"];
            Class = data["icon"];
            this.Sprite = Resources.Load<Sprite>("Weapons/" + data["icon"]);
            this.SpriteTransparent = Resources.Load<Sprite>("Weapons/t/" + data["icon"]+ "_t");
            FinishedProgress = data["finishedProgress"]*10;
            Recipe = new Dictionary<string, int>();
            foreach (var item in data["recipe"])
                Recipe.Add(item.Key.ToString(), item.Value);
            isAssigned = false;
            Protect = "weapon";
            CharacterId = -1;
            isDefault = true;
        }
        public Weapon(string name)
        {
            Weapon weapon = (Weapon)Game.AllWeapons.First(w => w.Class.ToLower() == name.ToLower());

            Name = weapon.Name;
            Description = weapon.Description;
            Icon = weapon.Icon;
            HP = weapon.HP;
            SP = weapon.SP;
            ATK = weapon.ATK;
            DEF = weapon.DEF;
            MAG = weapon.MAG;
            RES = weapon.RES;
            Type = weapon.Type;
            Bonus = weapon.Bonus;
            Class = weapon.Class;
            this.Sprite = weapon.Sprite;
            Protect = weapon.Protect;
            this.SpriteTransparent = weapon.SpriteTransparent;
            FinishedProgress = weapon.FinishedProgress;
            Recipe = new Dictionary<string, int>();
            foreach (var item in weapon.Recipe)
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

    }
}
