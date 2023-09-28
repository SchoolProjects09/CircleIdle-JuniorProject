using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleIdleLib
{
    public class Resource
    {
        // RESOURCE INFORMATION
        public int Lumber { get; set; }
        public int Oak { get; set; }
        public int Hickory { get; set; }
        public int Gold { get; set; }
        public int Iron { get; set; }
        public int Copper { get; set; }
        public int Stone { get; set; }
        public int Ore { get; set; }
        public int Gems { get; set; }
        public Resource()
        {

        }
        /// <summary>
        /// This constructor is to use with DB data transfer
        /// </summary>
        /// <param name="lumber">Lumber</param>
        /// <param name="oak"></param>
        /// <param name="hickory"></param>
        /// <param name="gold"></param>
        /// <param name="iron"></param>
        /// <param name="copper"></param>
        /// <param name="stone"></param>
        /// <param name="ore"></param>
        /// <param name="gems"></param>
        public Resource(int lumber, int oak, int hickory, int gold, int iron, int copper, int stone, int ore, int gems)
        {
            Lumber = lumber;
            Oak = oak;
            Hickory = hickory;
            Gold = gold;
            Iron = iron;
            Copper = copper;
            Stone = stone;
            Ore = ore;
            Gems = gems;
        }
        //Initial resources
        public Resource(int lumber, int gold, int stone)
        {
            Lumber = lumber;
            Oak = 0;
            Hickory = 0;
            Gold = gold;
            Iron = 0;
            Copper = 0;
            Stone = stone;
            Ore = 0;
            Gems = 0;
        }
        //Helper functions to access specific resources based on a string input
        //This code is ugly and very likely not an ideal solution
        public int GetResource(string resource)
        {
            switch (resource)
            {
                case "lumber":
                    return Lumber;
                case "oak":
                    return Oak;
                case "hickory":
                    return Hickory;
                case "gold":
                    return Gold;
                case "iron":
                    return Iron;
                case "copper":
                    return Copper;
                case "stone":
                    return Stone;
                case "ore":
                    return Ore;
                case "gems":
                    return Gems;
                default:
                    return 0;
            }
        }
        //Add specified amount to resource
        //If subtracting (for cost), input negative amount
        public void AddResource(string resource, int amount)
        {
            switch (resource)
            {
                case "lumber":
                    Lumber += amount;
                    break;
                case "oak":
                    Oak += amount;
                    break;
                case "hickory":
                    Hickory += amount;
                    break;
                case "gold":
                    Gold += amount;
                    break;
                case "iron":
                    Iron += amount;
                    break;
                case "copper":
                    Copper += amount;
                    break;
                case "stone":
                    Stone += amount;
                    break;
                case "ore":
                    Ore += amount;
                    break;
                case "gems":
                    Gems += amount;
                    break;
                default:
                    break;
            }
        }
        public string ToJSON()
        {
            return String.Format("\"lumber\": {0}, \"oak\": {1}, \"hickory\": {2}, \"gold\": {3}, \"iron\": {4}, \"copper\": {5}, \"stone\": {6}, \"ore\": {7}, \"gems\": {8}",
                Lumber, Oak, Hickory, Gold, Iron, Copper, Stone, Ore, Gems);
        }
    }
}
