using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CircleIdleLib
{
    public class CharacterEquipment
    {
        public iInventory Head { get; set; }
        public iInventory Body { get; set; }
        public iInventory Legs { get; set; }
        public iInventory Feet { get; set; }
        public iInventory Accessory1 { get; set; }
        public iInventory Accessory2 { get; set; }
        public iInventory Weapon { get; set; }
        public CharacterEquipment()
        {
            Head = null;
            Body = null;
            Legs = null;
            Feet = null;
            Accessory1 = null;
            Accessory2 = null;
            Weapon = null;
        }
        public int GetPropertyTotal(string property)
        {
            PropertyInfo pinfo = typeof(iInventory).GetProperty(property);
            int temp = 0;

            temp += Head != null ? (int)pinfo.GetValue(Head, null) : 0;
            temp += Body != null ? (int)pinfo.GetValue(Body, null) : 0;
            temp += Legs != null ? (int)pinfo.GetValue(Legs, null) : 0;
            temp += Feet != null ? (int)pinfo.GetValue(Feet, null) : 0;
            temp += Accessory1 != null ? (int)pinfo.GetValue(Accessory1, null) : 0;
            temp += Accessory2 != null ? (int)pinfo.GetValue(Accessory2, null) : 0;
            temp += Weapon != null ? (int)pinfo.GetValue(Weapon, null) : 0;
            return temp;
        }
    }
}
