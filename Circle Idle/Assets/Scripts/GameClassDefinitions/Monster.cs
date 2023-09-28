using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleIdleLib
{
    public class Monster
    {
        // MONSTER INFORMATION
        public string Name { get; set; }
        public string Icon { get; set; }

        // MONSTER HEALTH AND MANA
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int MaxMP { get; set; }
        public int CurrentMP { get; set; }

        // MONSTER STATS
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Magic { get; set; }
        public int Resistance { get; set; }
        public int Dexterity { get; set; }
        public int Speed { get; set; }

        // MONSTER LOOT
        //public Dictionary<Material, double> DropTable { get; set; }
    }
}
