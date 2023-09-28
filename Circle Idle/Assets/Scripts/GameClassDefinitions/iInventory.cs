using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CircleIdleLib
{
    public interface iInventory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
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
        public Sprite Sprite { get; set; }
        public Sprite SpriteTransparent { get; set; }
        public Dictionary<string, int> Recipe { get; set; }
        public int FinishedProgress { get; set; }
        public string Type { get; set; }
        public string[] SortCriterias();
        public string GetSortProperty(string property);
        public string ToJSON(int index);
    }
}
