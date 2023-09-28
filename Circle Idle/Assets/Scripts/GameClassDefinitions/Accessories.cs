using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace CircleIdleLib
{
    public class Accessories:iInventory
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

        public Accessories(JSONNode data)
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
            this.Sprite = Resources.Load<Sprite>("Accessories/" + data["icon"]);
            
            FinishedProgress = data["finishedProgress"];
            Protect = data["protect"];
            Recipe = new Dictionary<string, int>();
            foreach (var item in data["recipe"])
                Recipe.Add(item.Key.ToString(), item.Value);

            CharacterId = -1;
            isAssigned = false;
            isDefault = true;
        }

        public Accessories(string name)
        {
            Accessories equipment = (Accessories)Game.AllAccessories.First(w => w.Name.ToLower() == name.ToLower().Replace("_", " "));
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

            FinishedProgress = equipment.FinishedProgress;
            Recipe = new Dictionary<string, int>();
            foreach (var item in equipment.Recipe)
                Recipe.Add(item.Key.ToString(), item.Value);
            isAssigned = false;
            isDefault = true;
            CharacterId = -1;
        }

        public Accessories(iInventory iInventory)
        {
            Accessories equipment = (Accessories)iInventory;
            this.Name = equipment.Name;
            this.Description = equipment.Description;
            this.Icon = equipment.Icon;
            this.HP = equipment.HP;
            this.SP = equipment.SP;
            this.ATK = equipment.ATK;
            this.DEF = equipment.DEF;
            this.MAG = equipment.MAG;
            this.RES = equipment.RES;
            this.Class = equipment.Class;
            this.Sprite = equipment.Sprite;
            this.Protect = equipment.Protect;

            this.FinishedProgress = equipment.FinishedProgress;
            this.Recipe = new Dictionary<string, int>();
            foreach (var item in equipment.Recipe)
                this.Recipe.Add(item.Key.ToString(), item.Value);
            this.isAssigned = false;
            this.isDefault = true;
            this.CharacterId = -1;
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
            //Need to figure out the assignment slot 
            string assignedSlot = "n";

            if (isAssigned && CharacterId != -1) //redundant check
            {
                if (Game.Player.Characters[CharacterId].CharacterEquipment.Accessory1 != null 
                     && Game.Player.Characters[CharacterId].CharacterEquipment.Accessory1.ID == ID)
                    assignedSlot = "accessory1";
                else if (Game.Player.Characters[CharacterId].CharacterEquipment.Accessory2 != null 
                          && Game.Player.Characters[CharacterId].CharacterEquipment.Accessory2.ID == ID)
                    assignedSlot = "accessory2";
            }

            //We don't need many info here, since Accessories are not combinable.
            return "{" + String.Format("\"name\":\"{0}\"," +
                "\"item\":\"{1}\"," +
                "\"a\": {2}," +
                "\"s\":\"{3}\"",
                Name, Class, CharacterId, assignedSlot) +"}"; 
        }

        // TODO: MAKE EQUIPMENT AN ABSTRACT CLASS
        // SO WE CAN ADD ABSTRACT METHODS FOR STAT INCREASES
    }
}
