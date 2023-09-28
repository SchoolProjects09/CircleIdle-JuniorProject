using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleIdleLib
{
    public class Town
    {
        public Building Building { get; set; }
        private int _CharacterId;
        public int CharacterId
        {
            get { return _CharacterId; }
            set
            {
                _CharacterId = value;
                SetupAssignedCharacter();
            }
        }
        public AssignedCharacter Assigned { get; set; }

        public Town(Building building, int charId)
        {
            Building = building;
            CharacterId = charId;
            Assigned = new AssignedCharacter();

            SetupAssignedCharacter();

            //if(charId != -1)
            //Game.Player.Characters[charId].IsBusy = true;
        }
        public void SetupAssignedCharacter()
        {
            if (CharacterId != -1 && Game.Player != null)
            {
                Game.Player.Characters[CharacterId].IsBusy = true;
                Assigned.Class = Game.Player.Characters[CharacterId].Class;
                Assigned.Name = Game.Player.Characters[CharacterId].Name;
                Assigned.Level = Game.Player.Characters[CharacterId].Level;
            }
        }

        public class AssignedCharacter
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public int Level { get; set; }
        }

        public string ToJSON(int index)
        {
            StringBuilder builder = new StringBuilder();
            //This is done to assign proper Character to the building.
            int nullCounter = 0;
            for (int i = 0; i < CharacterId; i++)
            {
                if (Game.Player.Characters[i] == null)
                    nullCounter++;
            }


            builder.Append(String.Format("{{\"t\":\"{1}\",\"l\":{2},\"a\":{3},\"q\": [",
                index, Building.Class, Building.Level, CharacterId-nullCounter));

            int quantity;
            for (int i = 0; i < Building.Queue.Count - 1; i++)
            {
                if (Building.Queue[i].IsUnLimited)
                    quantity = 0;
                else
                    quantity = Building.Queue[i].Quantity;

                builder.Append(String.Format("{{\"t\":\"{1}\",\"item\":\"{2}\",\"q\":{3} }},",
                    i, Building.Queue[i].InventoryType, Building.Queue[i].Class, quantity));
            }
            if (Building.Queue.Count > 0)
            {
                int queueIndex = Building.Queue.Count - 1;

                if (Building.Queue[queueIndex].IsUnLimited)
                    quantity = 0;
                else
                    quantity = Building.Queue[queueIndex].Quantity;

                builder.Append(String.Format("{{\"t\":\"{1}\",\"item\":\"{2}\",\"q\":{3} }}",
                        queueIndex, Building.Queue[queueIndex].InventoryType, Building.Queue[queueIndex].Class, quantity));
            }
            builder.Append("]}");

            return builder.ToString();
        }
    }
}
