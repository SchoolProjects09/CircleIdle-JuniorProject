using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CircleIdleLib
{
    public class Character
    {
        // CHARACTER INFORMATION
        public int Id { get; set; }
        public string Name { get; set; }

        public int Experience { get; set; }
        public int MaxPointsForLevel { get; set; }
        public int MinPointsForLevel { get; set; }
        // CHARACTER CLASS
        public string Class { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        private int ClassLevel { get; set; }
        public int UnitLevel { get; set; }
        private const int BASEPOINTS = 300;
        private const double BASEGROW = 10;
        public List<string> Next { get; set; }
        public int MaxLvl { get; set; }
        //Ability
        public Ability Ability { get; set; }

        // CHARACTER EQUIPMENT
        public CharacterEquipment CharacterEquipment { get; set; }
        // CHARACTER STATS
        public float Attack { get; set; }
        public float Health { get; set; }
        public float Defense { get; set; }
        public float Magic { get; set; }
        public float Resistance { get; set; }
        public float Speed { get; set; }
        // CHARACTER Rates
        public float TrainedATK { get; set; }
        public float TrainedDEF { get; set; }
        public float TrainedMAG { get; set; }
        public float TrainedRES { get; set; }
        public float CombatHP { get; set; }
        public float CombatATK { get; set; }
        public float CombatDEF { get; set; }
        public float CombatMAG { get; set; }
        public float CombatRES { get; set; }
        public double GrowRate { get; set; }
        public double GrowAttackRate { get; set; }
        public double GrowHealthRate { get; set; }
        public double GrowDefenseRate { get; set; }
        public double GrowMagicRate { get; set; }
        public double GrowResistanceRate { get; set; }
        public double GrowSpeedRate { get; set; }
        //public int SpeedWood { get; set; }
        //public int SpeedMine { get; set; }
        //public int SpeedSmith { get; set; }
        public Sprite Sprite { get; set; }
        public bool IsBusy { get; set; }
        /// <summary>
        /// This is default empty constructor, It sets only Id to -1
        /// </summary>
        public Character(){
            Id = -1;
        }
        /// <summary>
        /// Character Constructor that is used to populate list from JSON file. It creates all characters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="characterClass"></param>
        public Character(int id, JSONNode data, string characterClass)
        {
            Id = id;
            Name = data["name"];
            
            Class = characterClass;
            Level = 1;
            ClassLevel = data["classLvl"];
            MaxLvl = data["maxLvl"];
            Description = data["description"];

            GrowRate = .75;
            UnitLevel = CalcUnitLevel();
            //Calculate ExpPoint at that Level
            //Experiance = BASEPOINTS *(UnitLevel^Rate)
            if (UnitLevel == 1)
               Experience = 0;
            else
               Experience = CalcExpiriencePoints(-1);

            
            MaxPointsForLevel = CalcExpiriencePoints();
            MinPointsForLevel = CalcExpiriencePoints(-1);
            Next = new List<string>();
            foreach (JSONNode item in data["next"])
                Next.Add(item);
            
            Ability = new Ability();
            Ability.Name = data["ability"]["name"];
            Ability.Description = data["ability"]["description"];
            Ability.Type = data["ability"]["type"];
            Ability.Bonus = data["ability"]["bonus"];


            IsBusy = false;
            
            Health = data["baseHealth"];
            GrowHealthRate = data["growHealth"];
            Attack = data["baseAttack"];
            GrowAttackRate = data["growAttack"];
            Defense = data["baseDefense"];
            GrowDefenseRate = data["growDefense"];
            Magic = data["baseMagic"];
            GrowMagicRate = data["growMagic"];
            Resistance = data["baseResistance"];
            GrowResistanceRate = data["growResistance"];
            Speed = data["baseSpeed"];
            GrowSpeedRate = data["growSpeed"];
            Sprite = Resources.Load<Sprite>("char/" + characterClass);
            CharacterEquipment = new CharacterEquipment();

            /*-----------TRAINING STUFF-------------*/
            TrainedATK = 0;
            TrainedDEF = 0;
            TrainedMAG = 0;
            TrainedRES = 0;
        }
        public int CalcExpiriencePoints(int step = 0)
        {
            //Calculate ExpPoint at that Level
            //Experiance = BASEPOINTS *(UnitLevel^Rate)
            return Convert.ToInt32(BASEPOINTS * Math.Pow((double)(UnitLevel + step), GrowRate));
        }
        private int CalcNextLevelPoints()
        {
            //If Character is at max level of its Class, just return the Current EXP Points
            if (Level == MaxLvl)
            {
                return CalcExpiriencePoints();
            }
            else //Else return points for next level within its class
            {
                return CalcExpiriencePoints(1);
            }
        }

        private int CalcUnitLevel()
        {
            //calculate Unit Level (1-53)
            int addend = 0;
            switch (ClassLevel)
            {
                case 0:
                    addend = 0;
                    break;
                case 1:
                    addend = 3;
                    break;
                case 2:
                    addend = 8;
                    break;
                case 3:
                    addend = 18;
                    break;
                case 4:
                    addend = 33;
                    break;
                default:
                    break;
            }
            return Level + addend;
        }



        /// <summary>
        /// Special Constructor that is used to populate character from Database. 
        /// </summary>
        /// <param name="name"> Name is set by the Player</param>
        /// <param name="experience">Exp points will never expire or get lost</param>
        /// <param name="characterClass">Class of the character at this point of time</param>
        /// <param name="level">Level of the character</param>
        public Character(string name, int experience, string characterClass, int level, int[] training)
        {
            training = training ?? new int[] { 0, 0, 0, 0 };

            Character data = Game.AllCharachters.Where(c => c.Class == characterClass).First(); 
            Name = name;
            Description = data.Description;
            Experience = experience;
            Class = characterClass;
            Level = level;
            MaxLvl = data.MaxLvl;
            ClassLevel = data.ClassLevel;
            GrowRate = data.GrowRate;

            UnitLevel = CalcUnitLevel();
            MaxPointsForLevel = CalcExpiriencePoints();
            MinPointsForLevel = CalcExpiriencePoints(-1);

            Next = new List<string>(data.Next);
            Ability = new Ability();
            Ability = data.Ability;

            IsBusy = false;
            
            Health = data.Health;
            GrowHealthRate = data.GrowRate;
            Attack = data.Attack;
            GrowAttackRate = data.GrowAttackRate;
            Defense = data.Defense;
            GrowDefenseRate = data.GrowDefenseRate;
            Magic = data.Magic;
            GrowMagicRate = data.GrowMagicRate;
            Resistance = data.Resistance;
            GrowResistanceRate = data.GrowResistanceRate;
            Speed = data.Speed;
            GrowSpeedRate = data.GrowSpeedRate;

            for (int i = 0; i < Level; i++)
            {
                Health = (int)(Health + (Math.Pow(BASEGROW, GrowHealthRate)));
                Attack = (int)(Attack + (Math.Pow(BASEGROW, GrowAttackRate)));
                Defense = (int)(Defense + (Math.Pow(BASEGROW, GrowDefenseRate)));
                Magic = (int)(Magic + (Math.Pow(BASEGROW, GrowMagicRate)));
                Resistance = (int)(Resistance + (Math.Pow(BASEGROW, GrowResistanceRate)));
                Speed = (int)(Speed + (Math.Pow(BASEGROW, GrowSpeedRate)));
            }

            Sprite = data.Sprite;
            CharacterEquipment = new CharacterEquipment();

            /*-----------TRAINING STUFF-------------*/
            TrainedATK = training[0];
            TrainedDEF = training[1];
            TrainedMAG = training[2];
            TrainedRES = training[3];
        }

        public void SetLevelStats()
        {
            UnitLevel = CalcUnitLevel();

            for (int i = 0; i < Level; i++)
            {
                Health = (int)(Health + (Math.Pow(BASEGROW, GrowHealthRate)));
                Attack = (int)(Attack + (Math.Pow(BASEGROW, GrowAttackRate)));
                Defense = (int)(Defense + (Math.Pow(BASEGROW, GrowDefenseRate)));
                Magic = (int)(Magic + (Math.Pow(BASEGROW, GrowMagicRate)));
                Resistance = (int)(Resistance + (Math.Pow(BASEGROW, GrowResistanceRate)));
                Speed = (int)(Speed + (Math.Pow(BASEGROW, GrowSpeedRate)));
            }
        }
        // CHARACTER METHODS
        /// <summary>
        /// Function that combines all stats and returns as one-multi-line string
        /// Used for display purposes only
        /// </summary>
        /// <returns></returns>
        public string GetStats(string property = "")
        {
            if (string.IsNullOrEmpty(property))
            {
                StringBuilder str = new StringBuilder();
                //also need to include stats from equipment
                str.AppendLine((Attack + CharacterEquipment.GetPropertyTotal("ATK")).ToString());
                str.AppendLine((Defense + CharacterEquipment.GetPropertyTotal("DEF")).ToString());
                str.AppendLine((Health + CharacterEquipment.GetPropertyTotal("HP")).ToString());
                str.AppendLine((Magic + CharacterEquipment.GetPropertyTotal("MAG")).ToString());
                str.AppendLine((Resistance + CharacterEquipment.GetPropertyTotal("RES")).ToString());
                str.AppendLine((Speed + CharacterEquipment.GetPropertyTotal("SP")).ToString());

                return str.ToString();
            }
            else
            {
                float stat = 0;
                switch (property.ToLower())
                {
                    case "health":
                    case "hp":
                        stat = (Health + CharacterEquipment.GetPropertyTotal("HP"));
                        break;
                    case "attack":
                    case "atk":
                        stat = (TrainedATK +  Attack + CharacterEquipment.GetPropertyTotal("ATK"));
                        break;
                    case "speed":
                    case "sp":
                        stat = (Speed + CharacterEquipment.GetPropertyTotal("SP"));
                        break;
                    case "defence":
                    case "def":
                        stat = (TrainedDEF + Defense + CharacterEquipment.GetPropertyTotal("DEF"));
                        break;
                    case "magic":
                    case "mag":
                        stat = (TrainedMAG + Magic + CharacterEquipment.GetPropertyTotal("MAG"));
                        break;
                    case "resistance":
                    case "res":
                        stat = (TrainedRES + Resistance + CharacterEquipment.GetPropertyTotal("RES"));
                        break;
                    default:
                        break;
                }
                return String.Format("{0:0.##}", stat);
            }
        }

        /// <summary>
        /// Returns Experience points of the Character as a string
        /// Used for display purposes
        /// </summary>
        /// <returns></returns>
        public string GetCurrentExpPoints()
        {
            return $"{Experience} / {MaxPointsForLevel}";
        }
        /// <summary>
        /// Temporary function to check how leveling works
        /// </summary>
        public void LevelUp()
        {

            if (Experience >= MaxPointsForLevel && Level != MaxLvl)
            {
                Level++; //increase level
                UnitLevel++;
                if (Level != MaxLvl)
                {
                    MinPointsForLevel = MaxPointsForLevel;
                }
                
                MaxPointsForLevel = CalcNextLevelPoints();

                Health = (int)(Health + (Math.Pow(BASEGROW, GrowHealthRate)));
                Attack = (int)(Attack + (Math.Pow(BASEGROW, GrowAttackRate)));
                Defense = (int)(Defense + (Math.Pow(BASEGROW, GrowDefenseRate)));
                Magic = (int)(Magic + (Math.Pow(BASEGROW, GrowMagicRate)));
                Resistance = (int)(Resistance + (Math.Pow(BASEGROW, GrowResistanceRate)));
                Speed = (int)(Speed + (Math.Pow(BASEGROW, GrowSpeedRate)));

                NotificationManager.Instance.Log(Name + " has leveled up from level " 
                    + (Level-1) + " to level " + Level);
            }
            else if (Experience >= MaxPointsForLevel && Level == MaxLvl) 
            {

                Experience = MaxPointsForLevel;
                //Do we want to keep gaining expirience points after we get to max? NO
            }
            if (Experience >= MaxPointsForLevel && Level != MaxLvl)
                {
                    LevelUp(); //create a loop to make sure added points are contributiong to correct level
                }
            }

        /// <summary>
        /// Returns a numerical cost of the character
        /// Used in hiring new character function
        /// </summary>
        /// <returns></returns>
        public int GetCost()
        {
            int cost = MaxPointsForLevel;
            return cost;
        }
        //Unity copies things as shallow copy with all references to original item
        public Character DeepCopy()
        {
            Character character = new Character();
            character.Id = this.Id;
            character.Name = this.Name;
            character.Description = this.Description;
            character.Experience = this.Experience;

            character.MaxPointsForLevel = this.MaxPointsForLevel;
            character.MinPointsForLevel = this.MinPointsForLevel;
            character.Class = this.Class;
            character.ClassLevel = this.ClassLevel;
            character.UnitLevel = this.UnitLevel;
            character.Level = this.Level;
            character.MaxLvl = this.MaxLvl;
            character.Next = this.Next;
            character.Ability = new Ability();
            character.Ability = this.Ability;

            character.Health = this.Health;
            character.Attack = this.Attack;
            character.Defense = this.Defense;
            character.Magic = this.Magic;
            character.Resistance = this.Resistance;
            character.Speed = this.Speed;

            character.Sprite = this.Sprite;

            character.CharacterEquipment = new CharacterEquipment();
            character.CharacterEquipment = this.CharacterEquipment;

            character.IsBusy = false;
            character.GrowRate = this.GrowRate;
            character.GrowHealthRate = this.GrowHealthRate;
            character.GrowAttackRate = this.GrowAttackRate;
            character.GrowDefenseRate = this.GrowDefenseRate;
            character.GrowMagicRate = this.GrowMagicRate;
            character.GrowResistanceRate = this.GrowResistanceRate;
            character.GrowSpeedRate = this.GrowSpeedRate;

            character.TrainedATK = this.TrainedATK;
            character.TrainedDEF = this.TrainedDEF;
            character.TrainedMAG = this.TrainedMAG;
            character.TrainedRES = this.TrainedRES;

            return character;
        }

        public string GetUpgradeCost()
        {
            int cost = MaxPointsForLevel / 2;
            return cost.ToString();
        }

        //Checks if the character is max level
        //Also fixes incorrectly set max levels
        public bool CanAdvance()
        {
            if (MaxLvl == 0)
            {
                foreach (var character in Game.AllCharachters)
                {
                    if (character.Class == Class)
                    {
                        MaxLvl = character.MaxLvl;
                    }
                }
            }

            if (Level == MaxLvl)
                return true;
            else
                return false;
        }

        public string ToJSON(int characterIndex)
        {
            return String.Format("{{\"name\":\"{1}\",\"point\":{2},\"t\":\"{3}\",\"l\":{4}," +
                "\"trained\":{{\"ATK\":{5},\"DEF\":{6},\"MAG\":{7},\"RES\":{8} }}}}",
                characterIndex, Name, Experience, Class, Level, TrainedATK, TrainedDEF, TrainedMAG, TrainedRES);
        }
        public float GetAttribute(string prop)
        {
            switch (prop.ToLower())
            {
                case "attack":
                    return TrainedATK;
                case "defence":
                    return TrainedDEF;
                case "magic":
                    return TrainedMAG;
                case "resistance":
                    return TrainedRES;
                default:
                    return 0;
            }
        }
        public void AddTrainingPoint(string trainingType, int points = 1)
        {
            switch (trainingType.ToLower())
            {
                case "attack":
                    TrainedATK += points;
                    break;
                case "defence":
                    TrainedDEF += points;
                    break;
                case "magic":
                    TrainedMAG += points;
                    break;
                case "resistance":
                    TrainedRES += points;
                    break;
                default:
                    break;
            }
        }
        public float GetSpeed()
        {
            return Speed + CharacterEquipment.GetPropertyTotal("SP");
        }
    }
}
