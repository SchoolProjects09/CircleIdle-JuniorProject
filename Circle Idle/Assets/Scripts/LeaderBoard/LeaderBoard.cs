using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    [Header("Setup the LeaderBoard Panel")]
    public GameObject LeaderSlotPanel;
    public GameObject LeaderSlot;
    public TextMeshProUGUI LeaderRank;
    [HideInInspector] public List<Leader> leaders = new List<Leader>();
    public bool gotLeaders;
    // Start is called before the first frame update
    public void RenderLeaderBoard()
    {
        leaders.Clear();
        StartCoroutine(Game.DataBase.GetLeaders(leaders));
    }

    void Start()
    {
    }
    public void SetupTheBoard()
    {
        _startTime = Time.time;
        isSpinnerActive = true;
        gotLeaders = false;
        Spinner.SetActive(true);
        foreach (Transform child in LeaderSlotPanel.transform)
            GameObject.Destroy(child.gameObject);

        RenderLeaderBoard();
    }
    // Update is called once per frame
    void Update()
    {
        if (isSpinnerActive)
        {
            SpinLogo();
        }

        if (leaders.Count > 0 && !gotLeaders)
        {
            isSpinnerActive = false;
            gotLeaders = true;
            Spinner.SetActive(false);
            //Clear the board
            foreach (Transform child in LeaderSlotPanel.transform)
                GameObject.Destroy(child.gameObject);
            int count = 0;

            //Add function to Show the RANK
            LeaderRank.text = $"Your rank: {Game.DataBase.usersRank}/{Game.DataBase.usersCount}";
            //populate the board with new values
            foreach (Leader leader in leaders)
            {
                GameObject itemObj = Instantiate(LeaderSlot);
                
                itemObj.transform.SetParent(LeaderSlotPanel.transform);
                itemObj.name = $"[{count}] {leader.DisplayName}";
                int i = count;
                var allKids = itemObj.GetComponentsInChildren<Transform>(true);
                allKids.First(k => k.name == "Name").GetComponent<TextMeshProUGUI>().text = leader.DisplayName;
                allKids.First(k => k.name == "Avatar").GetComponent<Image>().sprite = Resources.Load<Sprite>("avatar/" + leader.Avatar);
                allKids.First(k => k.name == "Exp").GetComponent<TextMeshProUGUI>().text = String.Format("{0:n0}", leader.Points);
                allKids.First(k => k.name == "Level").GetComponent<TextMeshProUGUI>().text = leader.Level.ToString();
                allKids.First(k => k.name == "Power").GetComponent<TextMeshProUGUI>().text = leader.Power.ToString();
                allKids.First(k => k.name == "Characters").GetComponent<TextMeshProUGUI>().text = leader.Characters.Count().ToString();
                itemObj.transform.position = Vector2.zero;
                itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                itemObj.GetComponent<RectTransform>().localScale = Vector2.one;
            }
        }
    }
    public GameObject Spinner;
    public RectTransform _LoadSpinner;
    private float _timeStep = .05f;
    private float _oneStepAngle = 6;
    private bool isSpinnerActive = true;
    private float _startTime;

    private void SpinLogo()
    {
        /*----https://www.youtube.com/watch?v=ltu27NLeIWc----*/
        if (Time.time - _startTime >= _timeStep)
        {
            Vector3 iconAngle = _LoadSpinner.localEulerAngles;
            iconAngle.z -= _oneStepAngle;
            _LoadSpinner.localEulerAngles = iconAngle;
            _startTime = Time.time;
        }

    }


}
