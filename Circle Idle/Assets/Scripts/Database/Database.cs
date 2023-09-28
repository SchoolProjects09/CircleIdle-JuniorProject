using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using Compression;

//namespace DBConnection { 
//https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html

public class CircleIdleDataBase
{

    private readonly string ACCESS_TOKEN = "9D2C1750509C6963BD4B108AA8DB2B54BEF10E5F";

    private readonly string url = "https://circleidle.fun/api/data.php";
    public int code = -1;
    public string message = "";
    public string userName = "";
    public string userEmail = "";
    public string userPass = "";
    public string displayName = "";
    public string data = "";
    public int returnedRows = 0;
    public int usersCount = 0;
    public int usersRank = 0;
    public JSONNode json;
    /// <summary>
    /// Function to save New Player's credentials to the Database
    /// </summary>
    /// <param name="uName">User Name</param>
    /// <param name="uEmail">User Email</param>
    /// <param name="uPass">User Password</param>
    /// <returns></returns>
    public IEnumerator NewRegistation(string uName, string uEmail, string uPass)
    {
        //load defaults: Level:1
        //               Points: 0
        //               All resources: 100
        //               One building: forest
        //               One character: peasant
        //               Empty inventory
        //string dataStr = "{\"timestamp\":\"" + DateTime.Now.ToString("G") + "\",\"level\":1,\"points\":0," +
        //    "\"resources\":{\"lumber\":100,\"oak\":100,\"hickory\":100,\"gold\":100,\"iron\":100,\"copper\":100,\"stone\":100,\"ore\":100,\"gems\":100},\"unlocked\":\"peasant\"," +
        //    "\"characters\":[{\"name\":\"Worker\",\"l\":1,\"t\":\"peasant\",\"point\":0}]," +
        //    "\"buildings\":[{\"l\":1,\"t\":\"forest\",\"a\":-1,\"q\":[]}]," +
        //    "\"weapons\":[],\\\"equipment\\\":[],\\\"accessories\\\":[]}\\";

        string dataStr = "TVBNT4QwEP0rZM41CxjXpLdNXD2YTYyaeCAcShnZBuh0W9AYwn93yhrw0jdvPt6b6QQdfmEHMslEAoPpMQyqd8xhv8t3eZrnyV5m9/L2Lnk5geAW3eIQQKYCqkCjrZfQI8deIxcm6Ma+Qh8105RVSbVrfOZx8j8rb6irV2I82ZVocu6fSBjI4ibpt7jBPlzJzGy0HfGGUbQAhyooO0ApAC+jcT0ykUVScuM3KkeWJwuuKs2rB/IG/xL6rLzSA/rIJ7CqZ0P4IN/yTgIcmajEh/O72gjgj8w455WxcYUJDu/PS9vD8XHB0+FpwdfjG2Myz+xVjaarjW2uVlHwk+8Lm54CecNwiTeVczn/Ag==";
        returnedRows = 0;
        WWWForm form = new WWWForm();
        form.AddField("ACCESS_TOKEN", ACCESS_TOKEN);
        form.AddField("action", "insert");
        form.AddField("table", "datagame");
        form.AddField("name", uName);
        form.AddField("password", uPass);
        form.AddField("email", uEmail);
        form.AddField("data", dataStr);
        //string s = StringCompression.Compress(dataStr);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                /* Recieved data
                 {
                    "id": 6,
                    "code": 200,
                    "message": "Success",
                    "data": []
                    } 
                 */
                json = JSON.Parse(www.downloadHandler.text);
                code = json["code"];
                message = json["message"];
                displayName = "User";

                userName = uName;
                userEmail = uEmail;
                userPass = md5_hash(uPass);
                //string sql = json["sql"];
                //string action = json["action"];
                Debug.Log(message);
            }
            else
            {
                code = -1;
                Debug.Log("Registration Failed!");
            }
        }
    }
    /// <summary>
    ///  Function checks if User exists in the database. 
    /// </summary>
    /// <param name="isLogin">Logic reverse if Login vs Register</param>
    /// <param name="uName">User name</param>
    /// <param name="uPass">Password, Optional</param>
    /// <returns></returns>
    public IEnumerator CheckUser(string uName, string uPass = "none")
    {
        returnedRows = 0;
        WWWForm form = new WWWForm();
        form.AddField("ACCESS_TOKEN", ACCESS_TOKEN);
        form.AddField("action", "checkuser");
        form.AddField("table", "datagame");
        form.AddField("name", uName);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                /* Recieved data
                 {
                    "code": 200,
                    "message": "Success",
                    "user": "pasha",
                    "pass": "ec8bc8e2b120d143e7274de2508f3f6f",
                    "email": "pasha@oit.edu",
                    "points": "0",
                    "avatar": "default",
                    "data": {}
                 */
                json = JSON.Parse(www.downloadHandler.text);

                //string sql = json["sql"];
                //string action = json["action"];

                code = json["code"];
                message = json["message"];
                returnedRows = json.Count;

                userName = json["user"];
                userEmail = json["email"];
                userPass = json["pass"];
                displayName = json["displayname"];

                Debug.Log(message);
            }
            else
            {
                code = -1;
                Debug.Log("Form upload complete!");
            }
        }


    }
    public IEnumerator CheckEmail(string uEmail)
    {
        returnedRows = 0;
        WWWForm form = new WWWForm();
        form.AddField("ACCESS_TOKEN", ACCESS_TOKEN);
        form.AddField("action", "checkEmail");
        form.AddField("table", "datagame");
        form.AddField("email", uEmail);
        //form.AddField("password", uPass);
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                json = JSON.Parse(www.downloadHandler.text);

                //string sql = json["sql"];
                //string action = json["action"];

                code = json["code"];
                message = json["message"];
                returnedRows = json.Count;

                userName = json["user"];
                userEmail = json["email"];
                userPass = json["pass"];
                displayName = json["displayname"];

                Debug.Log(message);
            }
            else
            {
                code = -1;
                Debug.Log("Form upload complete!");
            }
        }


    }
    public IEnumerator SaveGame(string convertedObject)
    {
        returnedRows = 0;
        WWWForm form = new WWWForm();
        form.AddField("ACCESS_TOKEN", ACCESS_TOKEN);
        form.AddField("action", "update");
        form.AddField("table", "datagame");
        form.AddField("name", Game.Player.Username);
        form.AddField("displayname", Game.Player.DisplayName);
        form.AddField("points", Game.Player.ExperiencePoints);
        form.AddField("level", Game.Player.PlayerLevel);
        form.AddField("power", Game.Player.GetPower());
        form.AddField("avatar", Game.Player.Avatar.name);
        string s = String.Join(",", Game.Player.Characters.Where(c => c != null).Select(c => c.Class).ToArray());
        form.AddField("characters", s);
        s = StringCompression.Compress(convertedObject);
        //Debug.Log(s);
        form.AddField("data", s);
        //Debug.Log("Ready To save " + convertedObject);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                /* Recieved data
                 {
                    "code": 200,
                    "message": "Success",
                    "data": []
                  }
                 */
                JSONNode json = JSON.Parse(www.downloadHandler.text);

                code = json["code"];
                message = json["message"];
                Debug.Log("Game Save result message: " + message);
            }
            else
            {
                code = -1;
                Debug.Log("Save Game - something went wrong.");
            }
        }


    }

    public IEnumerator GetLeaders(List<Leader> leaders, string orderby = "points", int limit = 10)
    {
        returnedRows = 0;
        string url = "https://circleidle.fun/api/board.php";

        WWWForm form = new WWWForm();
        form.AddField("ACCESS_TOKEN", ACCESS_TOKEN);
        form.AddField("orderby", orderby);
        form.AddField("limit", limit);
        form.AddField("username", Game.Player.Username);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                JSONNode json = JSON.Parse(www.downloadHandler.text);
                usersCount = json["count"];
                usersRank = json["rank"];
                foreach (var item in json["data"])
                {
                    leaders.Add(new Leader(item));
                }
                yield return leaders;
                Debug.Log("Got list of leaders");
            }
            else
            {
                code = -1;
                Debug.Log("Getting leaders - something went wrong.");
            }
        }
    }
        /// <summary>
        /// Function to provide Encryption Equivalento to php md5
        /// </summary>
        /// <param name="value">String to Encrypt</param>
        /// <returns></returns>
        public string md5_hash(string value)
    {
        byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(value);
        byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
public class Leader {
    public DateTime Timestamp { get; set; }
    public string DisplayName { get; set; }
    public int Points { get; set; }
    public int Level { get; set; }
    public int Power { get; set; }
    public string Avatar { get; set; }
    public List<string> Characters { get; set; }

    public Leader(JSONNode data)
    {
        Timestamp = DateTime.Parse(data["timestamp"]);
        DisplayName = data["displayname"];
        Points = int.Parse(data["points"]);
        Power = int.Parse(data["power"]);
        Level = int.Parse(data["level"]);
        Avatar = data["avatar"];
        Characters = ((string)data["characters"]).Split(',').ToList();
    }
}

