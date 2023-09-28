using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleIdleLib;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using SimpleJSON;
using System.Linq;

public static class Helper
{
    public class MyHelper : MonoBehaviour { }
    private static MyHelper myHelper;
    public static int OneTimeTrigger = 0;
    private static void Init()
    {
        if (myHelper == null)
        {
            GameObject gameObject = new GameObject("Helper");
            myHelper = gameObject.AddComponent<MyHelper>();
        }
    }

    /*-------------CODE FOR FADING IN/OUT ------------*/
    public static void FadeIn(GameObject panel)
    {
        Init();
        myHelper.StartCoroutine(DoFadeIn(panel));
    }
    public static void FadeOut(GameObject panel)
    {
        Init();
        myHelper.StartCoroutine(DoFadeOut(panel));
    }
    static private IEnumerator DoFadeIn(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 3;
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        panel.SetActive(true);
        yield return null;
    }
    static private IEnumerator DoFadeOut(GameObject panel)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 5;
            yield return null;
        }
        canvasGroup.alpha = 0;
        panel.SetActive(false);
        yield return null;
    }

    static public string PlayerToJSON()
    {
        StringBuilder builder = new StringBuilder();
        //builder.Append(Game.Player.ToJSON());
        builder.Append("{");
        builder.Append(String.Format("\"level\": {0}, \"timestamp\": \"{1}\",",
            Game.Player.PlayerLevel, DateTime.Now.ToString("G")));
        builder.Append($"\"tickets\":{Game.Player.ArenaTickets},");
        builder.Append($"\"bsound\":{Game.Player.GameBackgroundMusic},"); 
        int index = -1;

        //Add Resources
        builder.Append("\"resources\":{");
        builder.Append(Game.Player.Resources.ToJSON());
        builder.Append("}, ");
        builder.Append("\"unlocked\": [\"" + string.Join(",", Game.Player.UnlockedClasses.Distinct()) + "\"],");
        //Add Equipment 
        builder.Append("\"equipment\":[ ");
        for (int i = 0; i < Game.Player.Equipment.Count - 1; i++)
        {
            //Add a comma up until the last item
            builder.Append(Game.Player.Equipment[i].ToJSON(i) + ", ");
        }
        if (Game.Player.Equipment.Count > 0)
        {
            index = Game.Player.Equipment.Count - 1;
            builder.Append(Game.Player.Equipment[index].ToJSON(index));
        }
        builder.Append("], "); //Closing bracket of equipment section

        //Add Weapons 
        builder.Append("\"weapons\":[");
        for (int i = 0; i < Game.Player.Weapons.Count - 1; i++)
        {
            builder.Append(Game.Player.Weapons[i].ToJSON(i) + ", ");
        }
        if (Game.Player.Weapons.Count > 0)
        {
            index = Game.Player.Weapons.Count - 1;
            builder.Append(Game.Player.Weapons[index].ToJSON(index));
        }
        builder.Append("],");

        //Add Accessories 
        builder.Append("\"accessories\":[");
        for (int i = 0; i < Game.Player.Accessories.Count - 1; i++)
        {
            builder.Append(Game.Player.Accessories[i].ToJSON(i) + ", ");
        }
        if (Game.Player.Accessories.Count > 0)
        {
            index = Game.Player.Accessories.Count - 1;
            builder.Append(Game.Player.Accessories[index].ToJSON(index));
        }
        builder.Append("],");



        int num = 0;
        List<int> indexes = new List<int>();
        for (int i = 0; i < Game.Player.Characters.Count; i++)
        {
            if (Game.Player.Characters[i] != null)
            {
                num++;
                indexes.Add(i);
            }
        }

        //Add Characters 
        builder.Append("\"characters\":[");
        foreach (int i in indexes)
        {
            builder.Append(Game.Player.Characters[i].ToJSON(i));
            if (num-- > 1)
            {
                builder.Append(",");
            }
        }
        builder.Append("],");

        //Add buildings
        num = 0;
        indexes.Clear();
        for (int i = 0; i < Game.Player.Town.Count; i++)
        {
            if (Game.Player.Town[i] != null)
            {
                num++;
                indexes.Add(i);
            }
        }

        builder.Append("\"buildings\":[");
        foreach (int i in indexes)
        {
            builder.Append(Game.Player.Town[i].ToJSON(i));
            if (num-- > 1)
            {
                builder.Append(", ");
            }
        }

        builder.Append("]}"); //Final closing brackets
        return builder.ToString();
    }

    static public string LocalSave()
    {

        StringBuilder str = new StringBuilder();
        str.Append($"{{\"user\":\"{Game.Player.Username}\",");
        str.Append($"\"email\":\"{Game.Player.Email}\",");
        str.Append($"\"displayname\":\"{Game.Player.DisplayName}\",");
        str.Append($"\"points\":\"{Game.Player.ExperiencePoints}\",");
        str.Append($"\"avatar\":\"{Game.Player.Avatar.name}\",");
        str.Append($"\"data\":{PlayerToJSON()}}}");
        // Write to disk
        StreamWriter writer = new StreamWriter("data.circle", false);
        writer.AutoFlush = true;
        writer.Write(str);


        return str.ToString();

    }
    static public void LocalReset()
    {
        using (StreamWriter writer = new StreamWriter("data.circle", false))
        {
            writer.AutoFlush = true;
            writer.Write("");
        }
    }
    static public JSONNode ReadLocal()
    {
        string str = "";
        JSONNode json;
        if (File.Exists("data.circle"))
        {
            using (StreamReader reader = new StreamReader("data.circle"))
            {
                str = reader.ReadToEnd();
            }  
        }
        json = JSON.Parse(str);

        return json;
    }
}
