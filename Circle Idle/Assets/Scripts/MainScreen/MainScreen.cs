using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using CircleIdleLib;
using UnityEngine.UI;
using SimpleJSON;
using System.Linq;

public class MainScreen : MonoBehaviour
{
    [Header("Setup the Inputs")]
    public TMP_InputField LoginUserName;
    public GameObject LoginUserNameError;
    public TMP_InputField LoginPassword;
    public GameObject LoginPasswordError;
    public GameObject LoginErrorMessage;
    public TMP_InputField RegistrationUserName;
    public GameObject RegistrationUserNameError;
    public TMP_InputField RegistrationEmail;
    public GameObject RegistrationEmailError;
    public TMP_InputField RegistrationPassword;
    public GameObject RegistrationPasswordError;
    public GameObject RegistrationErrorMessage;

    private List<TMP_InputField> InputsForCurrentPanel = new List<TMP_InputField>();
    public int InputSelected = -1;

    [Space(10)]
    [Header("Setup Panels")]
    public GameObject LoginPanel;
    public GameObject RegistrationPanel;
    public GameObject GuestPanel;
    [Space(10)]
    [Header("Setup Buttons")]
    public GameObject LoginButton;
    public GameObject RegisterButton;
    public GameObject GuestButton;
    public GameObject PlayButton;
    public GameObject LogoutButton;
    public GameObject WelcomeMessage;


    public void PlayAsGuest()
    {
        Game.Player = new GamePlayer(); //loads Default Player's data
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    void Awake()
    {
        //this Code loads from locally saved file to allow autologin 
        //It runs only one time when Application starts
        if (Helper.OneTimeTrigger == 0)
        {
            JSONNode savedUser = Helper.ReadLocal();
            if (savedUser.Count > 0)
            {
                if (Game.Player == null)
                {
                    Game.GetStarted();
                }

                Helper.OneTimeTrigger = 1;
                Game.Player = new GamePlayer(savedUser);
                ProcessPassive();

                Debug.Log("User Logged in Successfully!");
            }

        }
    }
    void Start()
    {
        _startTime = Time.time;
        if(Game.Player == null)
            Game.GetStarted();
        else
            WelcomeMessage.GetComponent<TextMeshProUGUI>().text = $"Welcome back, {Game.Player.DisplayName}";
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);//Assuming Player is logged in and GamePlayer object is created;
    }
    public void Logout()
    {
        Game.Player = null;
        //If user logged out than reset locally saved object
        Helper.LocalReset();
    }
    // Update is called once per frame
    void Update()
    {
        //https://www.youtube.com/watch?v=U8Py8nI-azc
        if (InputsForCurrentPanel.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                InputSelected--;
                if (InputSelected < 0)
                    InputSelected = InputsForCurrentPanel.Count - 1;

                InputsForCurrentPanel[InputSelected].Select();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                InputSelected++;
                if (InputSelected > (InputsForCurrentPanel.Count - 1))
                    InputSelected = 0;

                InputsForCurrentPanel[InputSelected].Select();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Return key was pressed.");
                CallLogin();
            }
               

        }
        if (Game.Player != null)
        {
            LoginButton.SetActive(false);
            RegisterButton.SetActive(false);
            GuestButton.SetActive(false);
            PlayButton.SetActive(true);
            LogoutButton.SetActive(true);
            WelcomeMessage.SetActive(true);

        }
        else
        {
            LoginButton.SetActive(true);
            RegisterButton.SetActive(true);
            GuestButton.SetActive(true);
            PlayButton.SetActive(false);
            LogoutButton.SetActive(false);
            WelcomeMessage.SetActive(false);
        }
        if (isSpinnerActive)
        {
            if(LoginPanel.GetComponent<CanvasGroup>().interactable)
                SpinLogo(_LoginSpinner);
            else if(RegistrationPanel.GetComponent<CanvasGroup>().interactable)
                SpinLogo(_RegisterSpinner);
        }
            
    }
    public void OpenLoginForm()
    {
        isSpinnerActive = false;
        if (RegistrationPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(RegistrationPanel);

        if (GuestPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(GuestPanel);

        LoginUserNameError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        LoginPasswordError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        InputsForCurrentPanel.Clear();
        InputsForCurrentPanel.Add(LoginUserName);
        InputsForCurrentPanel.Add(LoginPassword);
        LoginUserName.text = "";
        LoginPassword.text = "";
        InputSelected = -1;
        Helper.FadeIn(LoginPanel);
    }
    public void CloseLoginForm()
    {
        InputsForCurrentPanel.Clear();
        Helper.FadeOut(LoginPanel);
    }
    public void OpenRegisterForm()
    {
        isSpinnerActive = false;

        if (LoginPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(LoginPanel);

        if (GuestPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(GuestPanel);

        RegistrationUserNameError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        RegistrationEmailError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        RegistrationPasswordError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);

        InputsForCurrentPanel.Clear();
        InputsForCurrentPanel.Add(RegistrationUserName);
        InputsForCurrentPanel.Add(RegistrationEmail);
        InputsForCurrentPanel.Add(RegistrationPassword);
        RegistrationUserName.text = "";
        RegistrationEmail.text = "";
        RegistrationPassword.text = "";
        InputSelected = -1;
        Helper.FadeIn(RegistrationPanel);
    }
    public void CloseRegisterForm()
    {
        InputsForCurrentPanel.Clear();
        Helper.FadeOut(RegistrationPanel);
    }
    public void OpenGuestForm()
    {
        if (LoginPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(LoginPanel);

        if (RegistrationPanel.GetComponent<CanvasGroup>().interactable == true)
            Helper.FadeOut(RegistrationPanel);

        Helper.FadeIn(GuestPanel);
    }
    public void CloseGuestForm()
    {

        Helper.FadeOut(GuestPanel);
    }

    public void OnSelect(GameObject placeHolder)
    {
        placeHolder.SetActive(false);
        TMP_InputField input = placeHolder.transform.parent.parent.GetComponent<TMP_InputField>();

        for (int i = 0; i < InputsForCurrentPanel.Count; i++)
        {
            if (input == InputsForCurrentPanel[i])
                InputSelected = i;
        }
        LoginUserNameError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        LoginPasswordError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        Helper.FadeOut(LoginErrorMessage);
        Helper.FadeOut(RegistrationErrorMessage);

    }
    public void OnDeSelect(GameObject placeHolder)
    {
        placeHolder.SetActive(true);
    }

    public void CallLogin()
    {
        if(LoginUserName.text.Length == 0)
            LoginUserNameError.GetComponent<Image>().color = Color.red;
        
        if(LoginPassword.text.Length == 0)
            LoginPasswordError.GetComponent<Image>().color = Color.red;
       
        if(LoginUserName.text.Length != 0 && LoginPassword.text.Length != 0)
            StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        LoginUserNameError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        LoginPasswordError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);

        isSpinnerActive = true;
        CircleIdleDataBase dBConnection = new CircleIdleDataBase();
        Debug.Log($"username: {LoginUserName.text} | password: {LoginPassword.text}");
        
        if(LoginUserName.text.Contains("@"))
            yield return dBConnection.CheckEmail(LoginUserName.text);
        else
           yield return dBConnection.CheckUser(LoginUserName.text);
        
        if (dBConnection.userName != null)//Check for user name first
        {
            string enteredPassword = dBConnection.md5_hash(LoginPassword.text);
            if (dBConnection.userPass == enteredPassword) //check password
            {
                Debug.Log("User Logged in Successfully!");
                Game.Player = new GamePlayer(dBConnection.json);
                ProcessPassive();
                WelcomeMessage.GetComponent<TextMeshProUGUI>().text = $"Welcome back, {dBConnection.displayName}";
                CloseLoginForm();
            }
            else
            {
                LoginPasswordError.GetComponent<Image>().color = Color.red;
                Alert("Password is incorrect");
            }
            if (dBConnection.userName == null)
            {
                LoginUserNameError.GetComponent<Image>().color = Color.red;
                Alert("Username is incorrect");
            }
        }
        else
        {
            LoginUserNameError.GetComponent<Image>().color = Color.red;
            Alert("User does not exist in the System!");
            
        }
        isSpinnerActive = false;

        if (LoginPanel.GetComponent<CanvasGroup>().interactable)
            _LoginSpinner.localEulerAngles = Vector3.zero;
        else if (RegistrationPanel.GetComponent<CanvasGroup>().interactable)
            _RegisterSpinner.localEulerAngles = Vector3.zero;



    }
    public void CallRegister()
    {
        bool error = false;
        if (RegistrationUserName.text.Length < 5)
        {
            RegistrationUserNameError.GetComponent<Image>().color = Color.red;
            Alert("UserName must be at least 5 characters long");
            error = true;
        }
            
        if (RegistrationEmail.text.Length == 0 || !(RegistrationEmail.text.Contains("@")))
        {
            RegistrationEmailError.GetComponent<Image>().color = Color.red;
            Alert("Incorrect Email provided");
            error = true;
        }
            
        if (RegistrationPassword.text.Length < 5 )
        {
            RegistrationPasswordError.GetComponent<Image>().color = Color.red;
            Alert("Password must be at least 5 characters long");

            if (RegistrationPassword.text.Contains(" "))
            {
                Alert("Password cannot have spaces");
            }
            error = true;
        }
           
        if (!error)
            StartCoroutine(RegisterUser());
    }
    IEnumerator RegisterUser()
    {
        RegistrationUserNameError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        RegistrationEmailError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        RegistrationPasswordError.GetComponent<Image>().color = new Color32(79, 46, 6, 255);
        isSpinnerActive = true;
        CircleIdleDataBase dBConnection = new CircleIdleDataBase();
        yield return dBConnection.CheckUser(RegistrationUserName.text);

        if (dBConnection.userName == null)
        {
            yield return dBConnection.CheckEmail(RegistrationEmail.text);
            if (dBConnection.userEmail == null)
            {
                yield return dBConnection.NewRegistation(RegistrationUserName.text, RegistrationEmail.text, RegistrationPassword.text);
                if (dBConnection.code == 200)
                {
                    Debug.Log("User registered Successfully!");
                    Game.Player = new GamePlayer(RegistrationUserName.text, RegistrationEmail.text, RegistrationPassword.text, dBConnection.displayName);
                    WelcomeMessage.GetComponent<TextMeshProUGUI>().text = $"Welcome back, {dBConnection.displayName}";
                    CloseRegisterForm();
                }
                else
                {
                    Alert("Something went wrong!");
                }
            }
            else
            {
                RegistrationEmailError.GetComponent<Image>().color = Color.red;
                Alert("Email address Already Exists in the System!");
            }   
        }
        else
        {
            RegistrationUserNameError.GetComponent<Image>().color = Color.red;
            Alert("User Already Exists in the System!");
        }
        isSpinnerActive = false;

        if (LoginPanel.GetComponent<CanvasGroup>().interactable)
            _LoginSpinner.localEulerAngles = Vector3.zero;
        else if (RegistrationPanel.GetComponent<CanvasGroup>().interactable)
            _RegisterSpinner.localEulerAngles = Vector3.zero;
    }
    public RectTransform _LoginSpinner;
    public RectTransform _RegisterSpinner;
    private float _timeStep = .05f;
    private float _oneStepAngle = 3;
    private bool isSpinnerActive = false;
    private float _startTime;

    private void SpinLogo(RectTransform Spinner)
    {
        /*----https://www.youtube.com/watch?v=ltu27NLeIWc----*/
        if (Time.time - _startTime >= _timeStep)
        {
            Vector3 iconAngle = Spinner.localEulerAngles;
            iconAngle.z -= _oneStepAngle;
            Spinner.localEulerAngles = iconAngle;
            _startTime = Time.time;
        }

    }
    private void Alert(string message)
    {
        if (LoginPanel.GetComponent<CanvasGroup>().interactable)
        {
            LoginErrorMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            Helper.FadeIn(LoginErrorMessage);
        }
        else if (RegistrationPanel.GetComponent<CanvasGroup>().interactable)
        {
            RegistrationErrorMessage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            Helper.FadeIn(RegistrationErrorMessage);
        }
    }
    private void ProcessPassive()
    {
        //Go thru every building that has Queue and Character in it
        foreach (Town item in Game.Player.Town.Where(t => t != null && t.CharacterId != -1 && t.Building.Queue.Count > 0))
        {
            item.Building.CalculateTaskCompletion(Game.Player.TotalTicks, item.CharacterId, Game.Player.Characters[item.CharacterId].GetSpeed());
        }
        /*--------------------------------------------------------------*/
    }

}
