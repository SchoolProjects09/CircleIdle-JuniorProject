using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI PlayerStats;
    public AudioSource BackgroundMusic;
    void Start()
    {
        //Game.GetStarted();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Player != null)
        {
            Game.UpdateStats(PlayerStats);
            Game.Player.LevelUp();

            if (Game.Player.GameBackgroundMusic == 0)
                BackgroundMusic.Stop();
            else
                if (!BackgroundMusic.isPlaying)
                    BackgroundMusic.Play();
        }

    }

    public void LoadMainScreen()
    {
        SceneManager.LoadScene(0);
    }

}
