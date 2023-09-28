using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CircleIdleLib
{
    public static class Alert
    {
        private static List<string> Messages = new List<string>();
        //public static TextMeshProUGUI LogField;
        public static GameObject LogField;
        public static void Show(string message)
        {
            StringBuilder str = new StringBuilder();
            Messages.Add("<color=red>[" + DateTime.Now.ToString("G") + "]</color>\t<b>" + message + "</b>");
            for (int i = Messages.Count; i > 0; i--)
            {
                str.AppendLine(Messages[i - 1]);
            }

            LogField = GameObject.Find("Log");
            LogField.GetComponent<TextMeshProUGUI>().text = str.ToString();
        }
    }
}
