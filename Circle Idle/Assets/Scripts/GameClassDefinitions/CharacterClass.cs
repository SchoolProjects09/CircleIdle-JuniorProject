using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleIdleLib
{
    public class CharacterClass
    {
        // CHARACTER CLASS INFORMATION
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxLevel { get; set; }
        public string Icon { get; set; }

        // CHARACTER CLASS LISTS
        public List<CharacterClass> Promotions { get; set; }
        public List<Ability> Abilities { get; set; }

        // CHARACTER CLASS METHODS
        public void Advance(int select)
        {

        }

        public void GetArmor()
        {

        }
    }
}
