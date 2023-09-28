using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    
    public static NotificationManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<NotificationManager>();

            if (instance != null)
            {
                return instance;
            }

            CreateNewInstance();

            return instance;
        }
    }

    public static NotificationManager CreateNewInstance()
    {
        NotificationManager notificationManagerPrefab = Resources.Load<NotificationManager>("NotificationManager");
        instance = Instantiate(notificationManagerPrefab);
        return instance;
    }

    private static NotificationManager instance;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private TextMeshProUGUI notificationText;
    public List<string> logItems;
    public List<DateTime> logTimes;

    private IEnumerator notificationCoroutine;

    private void Start()
    {
        logItems = new List<string>();
        logTimes = new List<DateTime>();
    }

    public void Log(string message)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = DisplaytNotification(message);
        StartCoroutine(notificationCoroutine);
    }

    private IEnumerator DisplaytNotification(string message)
    {
        // add time and message to separate lists
        logTimes.Add(DateTime.Now);
        logItems.Add(message);

        // check if log has 50 messages, if so, remove oldest
        if (logItems.Count == 50)
        {
            string tempItem = logItems[0];
            logItems.Remove(tempItem);
        }
        if (logItems.Count == 50)
        {
            DateTime tempTime = logTimes[0];
            logTimes.Remove(tempTime);
        }

        // clear log for next part
        notificationText.text = "";

        // decrement through lists printing out time and log message
        for (int i = logItems.Count - 1; i >= 0; i--)
        {
            notificationText.text += "[" + logTimes[i].ToString("HH:mm:ss") + "] " + logItems[i] + "\n";
        }
        
        yield return null;
    }

    private IEnumerator TempDisplayNotification(string message)
    {
        logItems.Add(message);

        yield return null;
    }
}
