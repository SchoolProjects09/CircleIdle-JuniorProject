using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Compression;

namespace CircleIdleLib
{
    public class GamePlayer
    {
        // PLAYER INFORMATION
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PlayerLevel { get; set; }
        public int ExperiencePoints { get; set; }
        public int MaxBuildings { get; set; }
        public int MaxCharacters { get; set; }
        public int MaxInventory { get; set; }
        public int ExpNext { get; set; }
        public int ArenaTickets { get; set; }
        public int GameBackgroundMusic { get; set; }
        public Sprite Avatar { get; set; }
        // PLAYER LISTS
        public Resource Resources { get; set; }
        public List<Character> Characters { get; set; }
        public List<iInventory> Equipment { get; set; }
        public List<iInventory> Weapons { get; set; }
        //Loot is used for rings, or looted items
        public List<iInventory> Accessories { get; set; }

        public List<Town> Town { get; set; }
        public DateTime LastLoginTime { get; set; }
        //public Dictionary<CharacterClass, bool> UnlockedClasses { get; set; }
        public List<string> UnlockedClasses { get; set; }
        public string GetStats()
        {
            //This sesction will need to re changed by Nick to reflect the Panel that he is building

            StringBuilder str = new StringBuilder();
            str.AppendLine(Username);
            str.AppendLine(PlayerLevel.ToString());
            str.AppendLine(ExperiencePoints.ToString());
            str.AppendLine("");
            str.AppendLine(Resources.Lumber.ToString());
            str.AppendLine(Resources.Iron.ToString());
            str.AppendLine(Resources.Gold.ToString());

            return str.ToString();

        }

        public GamePlayer()
        {
            Username = "guest";
            DisplayName = "Guest";
            Email = "";
            SetDefaults();
            SetCharIds();
        }
        public int TotalTicks = 0;
        public GamePlayer(JSONNode player)
        {
            JSONNode data = player["data"];
            //create Player based on DB data
            if (data["timestamp"] == null)
            {
                data = JSON.Parse(StringCompression.Decompress(data));
            }
            LastLoginTime = DateTime.Parse(data["timestamp"]);
            //Calculate Number of ticks during the absence
            TotalTicks = (int)Math.Floor((DateTime.Now - LastLoginTime).TotalMilliseconds / 100);
            
            Debug.Log("Ready to get player's data");
            Username = player["user"];
            Email = player["email"];
            ExperiencePoints = (int)player["points"];
            DisplayName = player["displayname"];
            ArenaTickets = (int)(data["tickets"] ?? 0);
            GameBackgroundMusic = (int)(data["bsound"] ?? 1);  //turn backgrpund music ON by default
            Avatar = UnityEngine.Resources.Load<Sprite>("avatar/"+ player["avatar"]);
            
            PlayerLevel = (int)data["level"];
            int t =  (int)Game.player_data["player"]["maxBuildings"];

            MaxBuildings = ((int)(PlayerLevel / 10) * 2) + t;
            t = (int)Game.player_data["player"]["maxCharachters"];
            MaxCharacters = ((int)(PlayerLevel / 10) * 2) + t;
            MaxInventory = MaxBuildings * PlayerLevel; //not used anywhere for now

            ExpNext = ExpNext + (10 * (int)Math.Pow(PlayerLevel, 2));
            JSONNode resources = data["resources"];
            Resources = new Resource((int)resources["lumber"], (int)resources["oak"], (int)resources["hickory"],
                                     (int)resources["gold"], (int)resources["iron"], (int)resources["copper"],
                                     (int)resources["stone"], (int)resources["ore"], (int)resources["gems"]);
            /*----------CHARCTERS---------*/
            Characters = new List<Character>();
            int index = 0;
            foreach (JSONNode chars in data["characters"])
            {
                int[] trainingData = {chars["trained"]["ATK"], chars["trained"]["DEF"], chars["trained"]["MAG"], chars["trained"]["RES"] };
                Characters.Add(new Character(chars["name"], (int)chars["point"], chars["t"], (int)chars["l"], trainingData));
                Characters.Last().Id = index++;
            }
            //string[] str = data["unlocked"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',') ;
            UnlockedClasses = data["unlocked"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',').ToList();
            /*----------INVENTORY---------*/
            Weapons = new List<iInventory>();
            Equipment = new List<iInventory>();
            Accessories = new List<iInventory>();
            //Rewrite Logic for Assigned items. Need to add "Protect" field to data
            /*----------WEAPONS---------*/
            index = 0;
            foreach (JSONNode weapon in data["weapons"])
            {
                Weapons.Add(new Weapon((string)weapon["i"]));
                Weapons.Last().Name = weapon["name"];
                Weapons.Last().isDefault = weapon["d"];
                if(weapon["a"] != -1)
                {
                    Weapons.Last().isAssigned = true;
                    Weapons.Last().CharacterId = (int)weapon["a"];
                    Characters[(int)weapon["a"]].CharacterEquipment.Weapon = Weapons.Last();
                }
                Weapons.Last().HP = (int)weapon["HP"];
                Weapons.Last().SP = (int)weapon["SP"];
                Weapons.Last().ATK = (int)weapon["ATK"];
                Weapons.Last().DEF = (int)weapon["DEF"];
                Weapons.Last().MAG = (int)weapon["MAG"];
                Weapons.Last().RES = (int)weapon["RES"];
                Weapons.Last().ID = index++;
            }
            /*----------EQUIPMENT---------*/
            index = 0;
            foreach (JSONNode equipment in data["equipment"])
            {
                Equipment.Add(new Equipment((string)equipment["i"]));
                Equipment.Last().Name = equipment["name"];
                Equipment.Last().isDefault = equipment["d"];
                if ((int)equipment["a"] != -1)
                {
                    Equipment.Last().isAssigned = true;
                    Equipment.Last().CharacterId = (int)equipment["a"];

                    string property = equipment["s"];
                    property= string.Concat(property[0].ToString().ToUpper(), property.Substring(1)); //Capitalize the property
                    Characters[(int)equipment["a"]].CharacterEquipment.GetType().GetProperty(property)
                        .SetValue(Characters[(int)equipment["a"]].CharacterEquipment, Equipment.Last());


                }
                Equipment.Last().HP = (int)equipment["HP"];
                Equipment.Last().SP = (int)equipment["SP"];
                Equipment.Last().ATK = (int)equipment["ATK"];
                Equipment.Last().DEF = (int)equipment["DEF"];
                Equipment.Last().MAG = (int)equipment["MAG"];
                Equipment.Last().RES = (int)equipment["RES"];
                Equipment.Last().ID = index++;
            }
            /*----------ACCESSORIES---------*/
            index = 0;
            foreach (JSONNode accessories in data["accessories"])
            {
                Accessories.Add(new Accessories((string)accessories["item"]));
                Accessories.Last().Name = accessories["name"];
                if ((int)accessories["a"] != -1)
                {
                    Accessories.Last().isAssigned = true;
                    Accessories.Last().CharacterId = (int)accessories["a"];

                    string property = accessories["s"];
                    property = string.Concat(property[0].ToString().ToUpper(), property.Substring(1)); //Capitalize the property

                    Characters[(int)accessories["a"]].CharacterEquipment.GetType().GetProperty(property)
                        .SetValue(Characters[(int)accessories["a"]].CharacterEquipment, Accessories.Last());

                }
                Accessories.Last().ID = index++;
            }
            /*----------BUILDINGS---------*/
            Town = new List<Town>();
            foreach (JSONNode building in data["buildings"])
            {
                Town.Add(new Town(new Building(building["t"], (int)building["l"],""), (int)building["a"]));
                int characterID = (int)building["a"];
                if (characterID != -1)
                    Characters[characterID].IsBusy = true;

                foreach (JSONNode queue in building["q"])
                {
                    
                    int finishedProgress = 0;
                    switch ((string)queue["t"])
                    {
                        case "resource":
                            finishedProgress = Game.AllResources.First(r => r.Name.ToLower() == queue["item"]).FinishedProgress;
                            break;
                        case "armor":
                            finishedProgress = Game.AllEquipment.First(r => r.Class.ToLower() == queue["item"]).FinishedProgress;
                        break;
                        case "weapon":
                            finishedProgress = Game.AllWeapons.First(r => r.Class.ToLower() == queue["item"]).FinishedProgress;
                        break;
                        case "training":
                            finishedProgress = Game.AllTrainingAttributes.First(r => r.Class.ToLower() == queue["item"]).FinishedProgress;
                            break;
                        default:
                            break;
                    }

                    Town.Last().Building.Queue.Add(new CraftTask(queue["item"], (int)queue["q"], ((int)queue["q"] == 0 ? true : false), finishedProgress, queue["t"], null));
                }
            }
        }

        public GamePlayer(string name, string email, string password, string dName)
        {
            //ToDO: create Player based on new registration
            Username = name;
            DisplayName = dName;
            Email = email;
            SetDefaults();
            SetCharIds();
        }

        public GamePlayer(string test)
        {
            Username = "guest";
            DisplayName = "Test Player";
            Email = "";
            Avatar = UnityEngine.Resources.Load<Sprite>("avatar/default");
            PlayerLevel = 1;
            ExperiencePoints = 22;
            MaxBuildings = 4;
            MaxCharacters = 6;
            MaxInventory = 20;
            ExpNext = ExpNext + (10 * (int)Math.Pow(PlayerLevel, 2));

            Resources = new Resource(1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000);

            Characters = new List<Character>();
            Characters.Add(new Character("Worker", 0, "peasant", 3, null));
            Characters.Add(new Character("Hodor", 125, "worker", 2, null));
            Characters.Add(new Character("Private Ryan", 750, "warrior", 1, null));

            Town = new List<Town>();
            Town.Add(new Town(new Building("forest", 1, "wood"), -1));
            Town.Add(new Town(new Building("armorsmith", 1, "wood"), 1));
            Town.Add(new Town(new Building("mine", 1, "gold"), 0));
            Characters[0].IsBusy = true;

            LastLoginTime = DateTime.Now;

            UnlockedClasses = new List<string>();
            UnlockedClasses.AddRange(new string[] { "peasant", "worker", "fighter", "soldier", "warrior" });
            UnlockedClasses.AddRange(new string[] { "peasant" });
            Weapons = new List<iInventory>();
            Equipment = new List<iInventory>();
            Accessories = new List<iInventory>();

            Weapons.Add(new Weapon("rusted_sword"));
            Weapons.Add(new Weapon("flint_spear"));
            Weapons.Add(new Weapon("flint_axe"));
            Weapons.Add(new Weapon("rusted_sword"));
            Weapons.Add(new Weapon("rusted_sword"));

            for (int i = 0; i < Weapons.Count(); i++)
                Weapons[i].ID = i;

            Equipment.Add(new Equipment("leather_hood"));
            Equipment.Add(new Equipment("leather_hood"));
            Equipment.Add(new Equipment("leather_chestplate"));
            Equipment.Add(new Equipment("leather_leggings"));
            Equipment.Add(new Equipment("leather_boots"));


            for (int i = 0; i < Equipment.Count(); i++)
                Equipment[i].ID = i;

            Accessories.Add(new Accessories("ring_of_magic"));
            Accessories.Add(new Accessories("solar_pendant"));
            Accessories.Add(new Accessories("ring_of_magic"));

            for (int i = 0; i < Accessories.Count(); i++)
                Accessories[i].ID = i;
        }
        private void SetDefaults()
        {
            PlayerLevel = 1;
            ExperiencePoints = 0;
            MaxBuildings = 4;
            MaxCharacters = 6;
            MaxInventory = 20;
            ExpNext = 10;

            Avatar = UnityEngine.Resources.Load<Sprite>("avatar/default");

            Resources = new Resource(100, 100, 100, 100, 100, 100, 100, 100, 100);
            
            Characters = new List<Character>();
            Characters.Add(new Character("Worker", 0, "peasant", 1, null));

            Town = new List<Town>();
            Town.Add(new Town(new Building("forest", 1, "wood"), -1));

            LastLoginTime = DateTime.Now;

            UnlockedClasses = new List<string>();
            UnlockedClasses.AddRange(new string[] { "peasant"});
            Weapons = new List<iInventory>();
            Equipment = new List<iInventory>();
            Accessories = new List<iInventory>();

            for (int i = 0; i < Weapons.Count(); i++)
                Weapons[i].ID = i;

            for (int i = 0; i < Equipment.Count(); i++)
                Equipment[i].ID = i;

            for (int i = 0; i < Accessories.Count(); i++)
                Accessories[i].ID = i;
        }
        public void SetCharIds()
        {
            int counter = 0;
            foreach (var item in Characters)
            {
                item.Id = counter++;
            }
        }
        // PLAYER METHODS
        public int GetMaxWorkers()
        {
            return MaxCharacters;
        }

        public int GetMaxBuildings()
        {
            return MaxBuildings;
        }

        public void HireCharacter(Character character, int id)
        {
            Characters[id] = character;
            NotificationManager.Instance.Log($"With some magic, [{character.Name}] the [{character.Class}] was hired");
        }

        internal bool isEnoughGold(int requiredGold)
        {
            return Resources.Gold >= requiredGold;
        }

        internal void Upgrade(int playersListID, int toId)
        {
            Characters[playersListID].Class = Game.AllCharachters[toId].Class;
            Characters[playersListID].Attack = Game.AllCharachters[toId].Attack;
            Characters[playersListID].Defense = Game.AllCharachters[toId].Defense;
            Characters[playersListID].Description = Game.AllCharachters[toId].Description;
            Characters[playersListID].Health = Game.AllCharachters[toId].Health;
            Characters[playersListID].Level = 1;
            //Characters[playersListID].Experience = 0; // Should this be reset or kept from previous level or class?
            Characters[playersListID].Magic = Game.AllCharachters[toId].Magic;
            Characters[playersListID].MaxPointsForLevel = Game.AllCharachters[toId].MaxPointsForLevel;
            Characters[playersListID].MinPointsForLevel = Game.AllCharachters[toId].MinPointsForLevel;
            Characters[playersListID].Next = Game.AllCharachters[toId].Next;
            Characters[playersListID].Resistance = Game.AllCharachters[toId].Resistance;
            Characters[playersListID].Speed = Game.AllCharachters[toId].Speed;
            Characters[playersListID].Sprite = Game.AllCharachters[toId].Sprite;
            
        }

        public void LevelUp()
        {
            if (ExperiencePoints >= ExpNext)
            {
                PlayerLevel++;
                if (PlayerLevel % 10 == 0)
                {
                    MaxBuildings += 2;
                    MaxCharacters += 2;
                }
                //Exp for next level starts at 100 and increases exponentially from there
                ExpNext = ExpNext + (10 * (int)Math.Pow(PlayerLevel, 2));

                NotificationManager.Instance.Log("You have reached level " + PlayerLevel + "!");
                //Handle gaining enough experience to level up multiple times at once
                LevelUp();
            }
        }

        public List<Equipment> SortEquipment(string property, bool Desc)
        {
            List<Equipment> sortedEqipment = new List<Equipment>((IEnumerable<Equipment>)Equipment);

            if (property == "Name")
            {
                if (Desc)
                    sortedEqipment = sortedEqipment.OrderByDescending(s => s.Name).ToList();
                else
                    sortedEqipment = sortedEqipment.OrderBy(s => s.Name).ToList();
            }
            else
            {
                //Use reflection to sort by any int property (HP, SP, ATK, DEF, MAG, RES) 
                PropertyInfo pinfo = typeof(Equipment).GetProperty(property);
                if (Desc)
                    sortedEqipment = sortedEqipment.OrderByDescending(s => (int)pinfo.GetValue(s)).ToList();
                else
                    sortedEqipment = sortedEqipment.OrderBy(s => (int)pinfo.GetValue(s)).ToList();
            }
            return sortedEqipment;
        }
        public List<Weapon> SortWeapons(string property, bool Desc)
        {
            List<Weapon> sortedWeapons = new List<Weapon>((IEnumerable<Weapon>)Weapons);

            if (property == "Name" || property == "Type")
            {
                PropertyInfo pinfo = typeof(Weapon).GetProperty(property);
                if (Desc)
                    sortedWeapons = sortedWeapons.OrderByDescending(s => (string)pinfo.GetValue(s)).ToList();
                else
                    sortedWeapons = sortedWeapons.OrderBy(s => (string)pinfo.GetValue(s)).ToList();
            }
            else if (property == "Bonus")
            {
                if (Desc)
                    sortedWeapons = sortedWeapons.OrderByDescending(s => s.Bonus).ToList();
                else
                    sortedWeapons = sortedWeapons.OrderBy(s => s.Bonus).ToList();
            }
            else
            {
                //Use reflection to sort by any int property (HP, SP, ATK, DEF, MAG, RES) 
                PropertyInfo pinfo = typeof(Weapon).GetProperty(property);
                if (Desc)
                    sortedWeapons = sortedWeapons.OrderByDescending(s => (int)pinfo.GetValue(s)).ToList();
                else
                    sortedWeapons = sortedWeapons.OrderBy(s => (int)pinfo.GetValue(s)).ToList();
            }
            return sortedWeapons;
        }

        public List<iInventory> SortList(string list, string property, bool Desc)
        {
            List<iInventory> current = null;
            if (list == "weapons")
            {
                current = new List<iInventory>(Weapons);
            }
            else if (list == "equipment")
            {
                current = new List<iInventory>(Equipment);
            }
            else if (list == "accessories")
            {
                current = new List<iInventory>(Accessories);
            }

            if (property == "Name" || property == "Type")
            {
                PropertyInfo pinfo = typeof(iInventory).GetProperty(property);
                if (Desc)
                    current = current.OrderByDescending(s => (string)pinfo.GetValue(s)).ToList();
                else
                    current = current.OrderBy(s => (string)pinfo.GetValue(s)).ToList();
            }
            else if (property == "Bonus")
            {
                if (Desc)
                    current = current.OrderByDescending(s => s.Bonus).ToList();
                else
                    current = current.OrderBy(s => s.Bonus).ToList();
            }
            else
            {
                //Use reflection to sort by any int property (HP, SP, ATK, DEF, MAG, RES) 
                PropertyInfo pinfo = typeof(iInventory).GetProperty(property);
                if (Desc)
                    current = current.OrderByDescending(s => (int)pinfo.GetValue(s)).ToList();
                else
                    current = current.OrderBy(s => (int)pinfo.GetValue(s)).ToList();
            }
            return current;
        }
        public List<Equipment> SearchEquipmentByName(string name)
        {
            string nameL = name.ToLower();

            //Search all equipment for matching names
            var sortedEqipment = from item in Equipment
                                     //Convert to lowercase to not be dependent on capitalization
                                 where item.Name.ToLower().Contains(nameL)
                                 select item;

            //sortedEqipment is an IEnumerable and needs to be converted into a list
            return (List<Equipment>)(iInventory)sortedEqipment.ToList();
        }
        public List<Weapon> SearchWeaponsByName(string name)
        {
            string nameL = name.ToLower();

            var sortedWeapons = from item in Weapons
                                where item.Name.ToLower().Contains(nameL)
                                select item;

            return (List<Weapon>)(iInventory)sortedWeapons.ToList();
        }
        public void CombineTwoItems(string category, int from, int to)
        {
            bool isSwitched = false;
            float multiplyer = .5f;
            List<iInventory> current = null;
            if(category == "weapons")
            {
                current = Weapons;
            }
            else if(category == "equipment"){
                current = Equipment;
            }

            if (current[to].isDefault && !current[from].isDefault) //if Item to NOT combined YET and Item FROM is alsready combined => Switch them to get max points
            {
                int temp = to;
                to = from;
                from = temp;
                isSwitched = true;
            }

            int idOfAssigned = current[to].CharacterId;

            if (idOfAssigned == -1)
                idOfAssigned = current[from].CharacterId;
            //at the end idOfAssigned will carry the Id of the Character if it is assigned


            current[to].ATK += (int)(current[from].ATK * multiplyer);
            current[to].DEF += (int)(current[from].DEF * multiplyer);
            current[to].HP += (int)(current[from].HP * multiplyer);
            current[to].MAG += (int)(current[from].MAG * multiplyer);
            current[to].RES += (int)(current[from].RES * multiplyer);
            current[to].SP += (int)(current[from].SP * multiplyer);

            if (isSwitched) //Switch back, this is done to make sure that item stays in same slot before the switch
            {
                current[from] = current[to];
                from = to;
            }

            current[to].isDefault = false;
            current[to].CharacterId = idOfAssigned;

            if (idOfAssigned != -1)
                current[to].isAssigned = true;

            NotificationManager.Instance.Log($"[{current[from].Name}] was combined with [{current[to].Name}] Successfully");
            current.RemoveAt(from);
            /*Make sure to reassign IDs of Inventory and referenced charachters, to avoid conflicts when new item is added since we removed one*/
            for (int i = 0; i < current.Count(); i++)
            {
                current[i].ID = i;
                //We do not combine accessories
                if (current[i].isAssigned) {
                    switch (current[i].Protect)
                    {
                        case "head":
                            Characters[current[i].CharacterId].CharacterEquipment.Head = current[i];
                            break;
                        case "body":
                            Characters[current[i].CharacterId].CharacterEquipment.Body = current[i];
                            break;
                        case "legs":
                            Characters[current[i].CharacterId].CharacterEquipment.Legs = current[i];
                            break;
                        case "feet":
                            Characters[current[i].CharacterId].CharacterEquipment.Feet = current[i];
                            break;
                        case "weapon":
                            Characters[current[i].CharacterId].CharacterEquipment.Weapon = current[i];
                            break;
                        default:
                            break;
                    }
                }
            }


        }

        public string ToJSON()
        {
            return String.Format("{{ \"user\": \"{0}\"," +
                "\"email\": \"{1}\"," +
                "\"points\": {2}," +
                "\"displayname\": \"{3}\"," +
                "\"avatar\": \"{4}\"," +
                "\"tickets\": {5}," +
                "\"data\": {{ ",
                Username, Email, ExperiencePoints, DisplayName, Avatar.name, ArenaTickets);
        }
        public int GetPower()
        {
            int ret = 0;
            foreach(Character character in Characters.Where(c => c != null))
            {
                ret += character.Level;
            }
            return ret;
        }

        public bool CheckResource(string key, int value)
        {
            string property = string.Concat(key[0].ToString().ToUpper(), key.Substring(1));
            int curr = (int)Game.Player.Resources.GetType().GetProperty(property).GetValue(Game.Player.Resources, null);

            return curr >= value;
        }
    }



}
