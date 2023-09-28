using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;
using System.Reflection;

namespace CircleIdleLib
{
    public class Building
    {
        // BUILDING INFORMATION
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Id { get; set; }
        //Type refers to actual builing refers to general class of buildings
        public string Type { get; set; }
        //Class refers to actual builing 
        public string Class { get; set; }
        public int Level { get; set; }
        public List<string> Crafts { get; set; }
        public List<CraftTask> Queue { get; set; }
        public List<CompletedTasks> CompletedTasks { get; set; }
        public Sprite Sprite { get; set; }
        public Dictionary<string, int> UpgradeCost { get; set; }
        public Dictionary<string, int> LevelUp { get; set; }
        // BUILDING FEATURES
        public bool DoesCraft { get; set; }
        public Dictionary<Resource, int> ConstructionCost { get; set; }
        public string CurrentItem  { get; set; }
        public int LimitOfCraftedItems { get; set; }
        public int CurrentCraftedItemCounter { get; set; }
        public float CurrentProgress { get; set; }
        public float FinishedProgress { get; set; } //Get this and BuildProgress from JSON
        public float BuildProgress { get; set; }
        public bool IsConstruction { get; set; }
        //ConsctructionPoints - number of point to construct this building
        public float ConsctructionPoints { get; set; }
        //ConsctructionCurrect - how many points were completed for existing construction
        public float ConsctructionCurrect { get; set; }
        public string Bonus { get; set; } //Equipment type to provide bonus

        public Building(Building building)
        {
            Name = building.Name;
            Description = building.Description;
            Sprite = building.Sprite;
            Type = building.Type;
            ConsctructionPoints = building.ConsctructionPoints;
            Icon = building.Icon;
            Class = building.Class;
            Bonus = building.Bonus;
            IsConstruction = true;

            Crafts = new List<string>(building.Crafts);

            Queue = new List<CraftTask>();
            CompletedTasks = new List<CompletedTasks>();
            Level = 1;
            CurrentItem = "na";
            BuildProgress = 0;
            FinishedProgress = 0;
            CurrentProgress = 0;
            LimitOfCraftedItems = 1000; //this should be set by the player's level, right now hard coding it so we can present in the class
            CurrentCraftedItemCounter = 0;
            UpgradeCost = building.UpgradeCost;
        }
        //Default Constructor
        public Building(JSONNode data, int id)
        {
            Id = id;
            Name = data["name"];
            Description = data["description"];
            Icon = data["icon"];
            Sprite = Resources.Load<Sprite>("building/" + data["icon"]);
            Type = data["type"];
            Class = data["icon"];  // At this moment data["icon"] hold information about the building class
            Bonus = data["bonus"];
            Crafts = new List<string>();
            ConsctructionPoints = data["constructionPoints"];
            if (!string.IsNullOrEmpty(data["craft"].ToString()))
            {
                foreach (var item in data["craft"])
                    Crafts.Add(item.Value.ToString().Replace("\"",""));               
            }
            Queue = new List<CraftTask>();
            CompletedTasks = new List<CompletedTasks>();
            UpgradeCost = new Dictionary<string, int>();

            foreach (var item in data["levelup"])
            {
                UpgradeCost.Add(item.Key.ToString(), item.Value);
            }

            Level = 0;                 //if level is 0
            IsConstruction = true;
            BuildProgress = 0;
            FinishedProgress = 0;
            CurrentProgress = 0;
            LimitOfCraftedItems = 1000; //this should be set by the player's level
            CurrentCraftedItemCounter = 0;
        }
        //Constructor to be used for data passed in from DB
        public Building(string type, int level, string resource)
        {
            Name = Game.building_data["buildings"][type]["name"];
            Description = Game.building_data["buildings"][type]["description"];
            Sprite = Resources.Load<Sprite>("building/" + type);
            Type = Game.AllBuildings.First(b => b.Class == type).Type;
            ConsctructionPoints = Game.AllBuildings.First(b => b.Class == type).ConsctructionPoints;
            Icon = type;
            Class = type;
            if(level == 0)
                IsConstruction = true;
            else
                IsConstruction = false;

            Bonus = Game.building_data["buildings"][type]["bonus"];
            Crafts = new List<string>();

            Crafts = Game.AllBuildings.First(b => b.Class.ToLower() == type.ToLower()).Crafts;
            Queue = new List<CraftTask>();
            CompletedTasks = new List<CompletedTasks>();
            Level = level;
            CurrentItem = resource;
            //TODO figure out the progress
            BuildProgress = 0;
            FinishedProgress = 0;
            CurrentProgress = 0;
            LimitOfCraftedItems = 1000; //this should be set by the player's level, right now hard coding it so we can present in the class
            CurrentCraftedItemCounter = 0;
            UpgradeCost = Game.AllBuildings.First(b => b.Class.ToLower() == type.ToLower()).UpgradeCost;
        }
        // CRAFTING FEATURES
        private DateTime timedelta;
        // BUILDING METHODS

        //Power to set the progress rate (tickProgress^EXPONENT_RATE)
        //Set this to a sub-1 value to give speed bonuses from stats diminishing returns
        //Try not to set this above 1 or things will get really fast, really fast!
        const double EXPONENT_RATE = 0.8;

        //Number to divide speed by
        //Since speed is a powerful stat that greatly increases efficency for all buildings,
        //Set this higher to skew the calculation more towards the other stats and keep speed balanced
        const double SPEED_DIVISOR = 2;
        public float GetTaskProgressRates(Character worker) //Return number to multiply TickProgress by in DoTask, and also for use in passive calculation
        {
            float tickProgress = 1;
            switch (Bonus) //Calculate tickProgress through worker stats depending on the building
            {
                case "wood":
                    tickProgress *= (float)Math.Pow(worker.Attack * (worker.Speed / SPEED_DIVISOR), EXPONENT_RATE);
                    break;
                case "stone":
                    tickProgress *= (float)Math.Pow(worker.Defense * (worker.Speed / SPEED_DIVISOR), EXPONENT_RATE);
                    break;
                case "smith":
                    tickProgress *= (float)Math.Pow(worker.Attack * (worker.Speed / SPEED_DIVISOR), EXPONENT_RATE);
                    break;
                case "training":
                    tickProgress *= (float)Math.Pow(worker.Speed / SPEED_DIVISOR, EXPONENT_RATE); //SLOW AND PAINFULL TRAINING
                    break;
                default:
                    tickProgress *= (float)Math.Pow(worker.Speed, EXPONENT_RATE);
                    break;
            }

            if (worker.CharacterEquipment.Weapon != null) //Does the character have a weapon?
            {
                if (worker.CharacterEquipment.Weapon.Type == Bonus) //Does the weapon provide a bonus for this building?
                {
                    tickProgress *= (float)worker.CharacterEquipment.Weapon.Bonus;
                }
            }

            if (worker.Ability.Type == Bonus)
            {
                tickProgress *= (float)worker.Ability.Bonus;
            }

            return tickProgress;
        }
        public void DoTask(Character worker)
        {
            if(Queue.Count > 0) //make sure to perform a TASK when there is something in the queue
            {
                FinishedProgress = Queue[0].FinishedProgress;  //This is to set up the Bar Max points

                float tickProgress = 1;

                CurrentProgress += GetTaskProgressRates(worker);

                if (CurrentProgress == 0)
                {
                    timedelta = DateTime.Now;
                }
                CurrentProgress += tickProgress;

                if (CurrentProgress >= FinishedProgress ) //if the task is more than 100% -> subtract Quantity
                {
                    //item is done;
                    CurrentCraftedItemCounter++;
                    if (CurrentCraftedItemCounter != LimitOfCraftedItems) //do not do more than allowed
                    {
                        //Add new item to palyer's inventory
                        switch (Queue[0].InventoryType)
                        {
                            case "resource":
                                string property = string.Concat(Queue[0].Class[0].ToString().ToUpper(), Queue[0].Class.Substring(1));
                                int curr = (int)Game.Player.Resources.GetType().GetProperty(property).GetValue(Game.Player.Resources, null);
                                curr++;
                                Game.Player.Resources.GetType().GetProperty(property).SetValue(Game.Player.Resources, curr);
                                break;
                            case "weapon":
                                Game.Player.Weapons.Add(new Weapon(Queue[0].Class));
                                Game.Player.Weapons.Last().ID = Game.Player.Weapons.Count - 1;
                                break;
                            case "armor":
                                Game.Player.Equipment.Add(new Equipment(Queue[0].Class));
                                Game.Player.Equipment.Last().ID = Game.Player.Equipment.Count - 1;
                                break;
                            case "training":
                                worker.AddTrainingPoint(Queue[0].Class);
                                break;
                            default:
                                break;
                        }
                        //Debug.Log($"{(DateTime.Now - timedelta).TotalSeconds.ToString()},{tickProgress},{worker.UnitLevel},{worker.GetSpeed()},{FinishedProgress}");
                        if (!Queue[0].IsUnLimited && Queue[0].Quantity == 1) //check if this is the last item in the queue
                        {
                            Debug.Log($"[{worker.Name}] finished with crafting task TYPE [{Queue[0].InventoryType}]");
                            NotificationManager.Instance.Log($"[{worker.Name}] finished with crafting task [{string.Concat(Queue[0].Class[0].ToString().ToUpper(), Queue[0].Class.Substring(1).Replace("_", " "))}]");
                            Queue.RemoveAt(0); //remove it from the list
                            CurrentCraftedItemCounter = 0; //go to next item
                            CurrentProgress = 0;

                            
                        }
                        else if (!Queue[0].IsUnLimited) //make sure it is not set to infinity
                        {
                            Queue[0].Quantity--; //subtract from the list
                            CurrentProgress = 0;
                        }
                        else //if this is a resource and setup to infinity
                        {
                            CurrentProgress = 0;
                        }
                        //GAIN EXPIRIENCE AFTER EACH TASK 
                        worker.Experience += 1;
                        worker.LevelUp();

                        //PLAYER GETS XP AFTER TASK COMPLETE
                        Game.Player.ExperiencePoints += 5;
                    }
                    else
                    {
                        NotificationManager.Instance.Log("Daily IDLE limit is reached");
                    }
                    
                }

            }
            else
            {
                Debug.Log($"Queue for {Name} is empty");
            }
        }
        public void ConstructBuilding(Character worker)
        {
            
            if (ConsctructionCurrect <= ConsctructionPoints)
            {
                //to be changed  to take worker's abilities, level, and tools
                ConsctructionCurrect += (int)(Time.deltaTime * 100);
            }

        }
        public double GetConstructPercent() //Percent of progress to completion
        {

            if (ConsctructionCurrect == 0)
            {
                return 0;
            }
            return Math.Round(((ConsctructionCurrect / ConsctructionPoints) * 100), 2);
        }


        public double GetPercent() //Percent of progress to completion
        {
            //Debug.Log($"Building {Name}: CurrentProgress = {CurrentProgress}, CompleteProgress = {FinishedProgress}");
            if (CurrentProgress == 0)
            {
                return 0;
            }
            return Math.Round(((CurrentProgress / FinishedProgress) * 100), 2);
        }
        public void CalculateTaskCompletion(int numberOfTicks, int characterID, float speed)
        {
            int possibleCompletionQty = 0;
            if (Queue.Count > 0 && characterID != -1) //if there is a Queue and Someone was assigned to the building
            {
                //Calculate max Ticks permitted for this Building
                //72000 = 2hrs. Set it as minimum. 
                if (CompletedTasks == null)
                    CompletedTasks = new List<CompletedTasks>();
                //this will result in 72000 game ticks will happen in 2 hr period. 9000 is 15 minute period
                int maxTicks = 63000 + (9000 * Level); 
                
                if (numberOfTicks < maxTicks)
                    maxTicks = numberOfTicks;

                List<int> CraftTaskToRemove = new List<int>();
                int index = 0;
                foreach (CraftTask item in Queue)
                {
                    int tempQty = 0;
                    string property = "";
                    int curr = 0;

                    if (maxTicks > 0)
                    {
                        //Calculate Tick value
                        float tickProgress = 1;
                        switch (item.InventoryType) //Calculate tickProgress through worker stats
                        {
                            case "resource":
                                tickProgress *= (float)(2 * (Math.Pow((double)speed, .75)));
                                break;
                            case "training":
                                tickProgress *= (float)((Math.Pow((double)speed, .75)) / 2);
                                break;
                            default:
                                tickProgress *= (float)(Math.Pow((double)speed, .75));
                                break;
                        }
                        //Calculate Possible Items completed during the period
                        possibleCompletionQty = (int)Math.Floor((maxTicks * tickProgress) / item.FinishedProgress);
                        //IF there would be a limit to complete, comparison would be here
                        //{}
                        if (possibleCompletionQty > 0)
                        {
                            if (item.IsUnLimited) //if this task was set up unlimited gathering, we will just add completed Qty to the Resource
                            {
                                property = string.Concat(item.Class[0].ToString().ToUpper(), item.Class.Substring(1));
                                curr = (int)Game.Player.Resources.GetType().GetProperty(property).GetValue(Game.Player.Resources, null);
                                curr += possibleCompletionQty;
                                tempQty = possibleCompletionQty;
                                Game.Player.Resources.GetType().GetProperty(property).SetValue(Game.Player.Resources, curr);
                            }
                            else
                            {
                                //If there is number to qty
                                switch (item.InventoryType) //Process per type
                                {
                                    case "resource":
                                        property = string.Concat(item.Class[0].ToString().ToUpper(), item.Class.Substring(1));
                                        curr = (int)Game.Player.Resources.GetType().GetProperty(property).GetValue(Game.Player.Resources, null);
                                        if (possibleCompletionQty > item.Quantity)
                                        {
                                            curr += item.Quantity;
                                            //This task is complete , mark it for removal
                                            tempQty = item.Quantity;
                                            int i = index;
                                            CraftTaskToRemove.Add(i);
                                        }
                                        else
                                        {
                                            curr += possibleCompletionQty;
                                            tempQty = possibleCompletionQty;
                                            //There is some left to do, adjust quantity
                                            item.Quantity -= possibleCompletionQty;
                                        }
                                        Game.Player.Resources.GetType().GetProperty(property).SetValue(Game.Player.Resources, curr);
                                        break;
                                    case "training":
                                        if (possibleCompletionQty > item.Quantity)
                                        {
                                            Game.Player.Characters[characterID].AddTrainingPoint(item.Class, item.Quantity);
                                            //This task is complete , mark it for removal
                                            tempQty = item.Quantity;
                                            int i = index;
                                            CraftTaskToRemove.Add(i);
                                        }
                                        else
                                        {
                                            Game.Player.Characters[characterID].AddTrainingPoint(item.Class, possibleCompletionQty);
                                            //There is some left to do, adjust quantity
                                            tempQty = possibleCompletionQty;
                                            item.Quantity -= possibleCompletionQty;
                                        }
                                        break;
                                    case "armor":
                                        if (possibleCompletionQty > item.Quantity)
                                        {
                                            // All crafts where completed, add them all to inventory
                                            for (int x = 0; x < item.Quantity; x++)
                                                Game.Player.Equipment.Add(new Equipment(item.Class));

                                            tempQty = item.Quantity;
                                            //This task is complete , mark it for removal
                                            int i = index;
                                            CraftTaskToRemove.Add(i);
                                        }
                                        else
                                        {
                                            // All crafts where completed, add them all to inventory
                                            for (int x = 0; x < possibleCompletionQty; x++)
                                                Game.Player.Equipment.Add(new Equipment(item.Class));

                                            tempQty = possibleCompletionQty;
                                            item.Quantity -= possibleCompletionQty;
                                        }
                                        break;
                                    case "weapon":
                                        if (possibleCompletionQty > item.Quantity)
                                        {
                                            // All crafts where completed, add them all to inventory
                                            for (int x = 0; x < item.Quantity; x++)
                                                Game.Player.Weapons.Add(new Weapon(item.Class));

                                            tempQty = item.Quantity;
                                            //This task is complete , mark it for removal
                                            int i = index;
                                            CraftTaskToRemove.Add(i);
                                        }
                                        else
                                        {
                                            // All crafts where completed, add them all to inventory
                                            for (int x = 0; x < possibleCompletionQty; x++)
                                                Game.Player.Weapons.Add(new Weapon(item.Class));

                                            tempQty = possibleCompletionQty;
                                            item.Quantity -= possibleCompletionQty;
                                        }
                                        break;
                                    default:
                                        tickProgress *= (float)(Math.Pow((double)speed, .75));
                                        break;
                                }
                                //CompletedTasks.Add(new CompletedTasks(tempQty, item.Class, Game.Player.Characters[characterID].Name, item.InventoryType));
                            }
                            CompletedTasks.Add(new CompletedTasks(tempQty, item.Class, Game.Player.Characters[characterID].Name, item.InventoryType));
                        }
                        else
                        {
                            maxTicks = 0;
                            break;
                        }
                    }
                    //Adjust Ticks so for the next item
                    maxTicks = maxTicks - (tempQty * item.FinishedProgress);
                    index++;
                }
                //Need to remove from the rear, otherwise idex will get messed up
                for (int i = CraftTaskToRemove.Count-1; i >= 0; i--)
                {
                    Queue.RemoveAt(CraftTaskToRemove[i]);
                }
            }

        }
        public void UpgradeBuilding() //Level up building if player has enough resources to spend
        {
            //Check for if player has enough materials is already performed in BuildingList, if the player lacks materials the button will be disabled
            foreach (KeyValuePair<string, int> cost in UpgradeCost)
            {
                //Probably a way to refactor this to not calculate it twice, but this shouldn't add too much more load
                Game.Player.Resources.AddResource(cost.Key, -(int)MaterialRate()); //Subtract cost from resource total
            }
            Level++;
        }
        public double MaterialRate() { return Math.Pow(Level, 0.75); } //Rate of grow for upgrade costs

    }
    public class CraftTask { 
    
        public string Class { set; get; }          // "lumber", "iron_pickaxe", etc.
        public string InventoryType { set; get; }  //resource, weapon, armory
        public int Quantity { set; get; }
        public int FinishedProgress { set; get; }  //defined number that tells how many points needed to craft one item
        public bool IsUnLimited { set; get; }        //craft non-stop?
        public Dictionary<string, int> Recipe { get; set; }
        public CraftTask(string type, int qty, bool limit, int finishedProgress, string inventoryType, Dictionary<string, int> recipe)
        {
            Class = type;
            Quantity = qty;
            IsUnLimited = limit;
            FinishedProgress = finishedProgress;
            InventoryType = inventoryType;
            this.Recipe = recipe;
        }
        public string GetName()
        {
            return Class.Replace("_", " ");
        }
    }
}
