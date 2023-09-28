using CircleIdleLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletedTasks : MonoBehaviour
{
    [Header("Setup Completed Tasks Panel ")]
    public GameObject CompletedTasksPanel;
    public TextMeshProUGUI CompletedTasksText;
    public Button CompletedTasksButton;
    // Start is called before the first frame update
   private bool showOneTime = true;
    void Start()
    {
         
    }
    public void CloseCompletedTasks()
    {
        Helper.FadeOut(CompletedTasksPanel);
    }
    // Update is called once per frame
    void Update()
    {
        if (showOneTime && Game.Player != null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("");
            List<Building> bbb = new List<Building>(Game.Player.Town.Where(t => t != null).Select(t => t.Building).Where(b => b != null && b.CompletedTasks != null));
            foreach (Building building in bbb)
            {
                foreach (var task in building.CompletedTasks)
                {
                    switch (task.TaskType)
                    {
                        case "resource":
                            builder.AppendLine($"[{task.CompletedBy}] gathered {task.Completed} {task.ItemName}");
                            break;
                        case "armor":
                            builder.AppendLine($"[{task.CompletedBy}] crafted {task.Completed} pcs of [{task.ItemName.Replace("_", "")}]");
                            break;
                        case "weapon":
                            builder.AppendLine($"[{task.CompletedBy}] crafted {task.Completed} pcs of [{task.ItemName.Replace("_","")}]");
                            break;
                        case "training":
                            builder.AppendLine($"[{task.CompletedBy}] got extra {task.Completed} points of {task.ItemName}");
                            break;
                        default:
                            break;
                    }
                    //builder.AppendLine("");
                }
            }
            CompletedTasksText.text = builder.ToString();
            if(!String.IsNullOrWhiteSpace(builder.ToString()))
            {
                Helper.FadeIn(CompletedTasksPanel);
            }

            showOneTime = false;
        }
    }
}
