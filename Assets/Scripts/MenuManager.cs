using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;

public class MenuManager : MonoBehaviour
{
    Hashtable themeOptions;

    // Audio
    private AudioManager gameAudio;

    // No. questions
    private Button[] noQuestionsButtons;
    public GameObject noQuestionsButtonPrefab;
    public RectTransform noQuestionButtonPanel;
    private int noQuestions;

    // Speed
    private int speed;
    public Slider speedSlider;
    public TMP_Text speedText;

    // No. options
    private int noOptions = 2;
    public Button[] noOptionsButtons;

    // Theme
    public GameObject themeButtonPrefab;
    public RectTransform themeButtonPanel;
    private Button[] themeButtons;
    private string theme = "colores";

    // Scene Object (QuestionnaireManager will request it)
    public Settings settings;

    void Awake()
    {
        this.gameAudio = FindObjectOfType<AudioManager>();
        this.gameAudio.Play("Music");
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //this.title.text = Resources.Load<TextAsset>("uwu").ToString();
        // Load all possible theme option values from source file.
        LoadThemeOptions();

        // Default slider value
        this.SetSpeed(4);
        speedSlider.onValueChanged.AddListener(delegate { SetSpeed((int)speedSlider.value); });

        // Default noOptions value
        this.KeepHighlighted(noOptionsButtons[0], noOptionsButtons, "noOptions");
        foreach(Button button in noOptionsButtons)
        {
            button.onClick.AddListener(delegate { KeepHighlighted(button, noOptionsButtons, "noOptions"); PlayButtonSFX(); });
        }

        // Default theme value
        this.KeepHighlighted(themeButtons[0], themeButtons, "theme");
        foreach (Button button in themeButtons)
        {
            button.onClick.AddListener(delegate { KeepHighlighted(button, themeButtons, "theme"); PlayButtonSFX(); });
        }

        //Default noQuestions value
        SetTheme(this.theme);
    }

    void PlayButtonSFX()
    {
        this.gameAudio.Play("Menu Select");
    }

    private void LoadThemeOptions()
    {
        this.themeOptions = new Hashtable();
        
        string newPath = Application.streamingAssetsPath + "/Resources/Themes/catalog";
        StreamReader reader = new StreamReader(newPath + "/catalog.txt");
        string[] lines = reader.ReadToEnd().Split('\n');
        reader.Close();

        float buttonWidth = 0f;
        int i = 0;
        this.themeButtons = new Button[lines.Length];

        /*
        byte[] imageBytes = System.IO.File.ReadAllBytes(newPath + "/" + testData[currentQuestionIndex].display);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        this.currentImgDisplay.texture = tex;
         */

        foreach (string s in lines)
        {
            // Add items to hashtable
            string[] themeAndQuestions = s.Split(':');
            string[] leftSide = themeAndQuestions[0].Split(',');
            this.themeOptions.Add(leftSide[2], themeAndQuestions[1]);

            // Instantiate buttons and set name and icon
            GameObject themeButton = Instantiate(this.themeButtonPrefab);
            themeButton.transform.SetParent(this.themeButtonPanel, false);
            buttonWidth = themeButton.GetComponent<RectTransform>().rect.width;

            themeButton.transform.Find("Name").GetComponent<TMP_Text>().text = leftSide[1];
            themeButton.transform.Find("Value").GetComponent<Text>().text = leftSide[2];

            byte[] imageBytes = System.IO.File.ReadAllBytes(newPath + "/" + leftSide[0]);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            themeButton.transform.Find("Icon").GetComponent<RawImage>().texture = tex;

            this.themeButtons[i++] = themeButton.GetComponent<Button>();
        }

        float panelWidth = this.themeButtonPanel.rect.width;
        float spaceWidth = (panelWidth - (buttonWidth * lines.Length)) / lines.Length;
        float startX = (spaceWidth / 2f) - (panelWidth / 2f);

        foreach(Button b in this.themeButtons)
        {
            b.gameObject.GetComponent<RectTransform>().localPosition = new Vector2(startX + (buttonWidth/2f), 0f);
            startX += buttonWidth + spaceWidth;
        }


    }

    private void HighlightNoQuestions()
    {
        this.KeepHighlighted(this.noQuestionsButtons[0], this.noQuestionsButtons, "noQuestions");
        foreach (Button button in this.noQuestionsButtons)
        {
            button.onClick.AddListener(delegate { KeepHighlighted(button, this.noQuestionsButtons, "noQuestions"); PlayButtonSFX(); });
        }
    }

    public void LoadScene() 
    {
        this.gameAudio.Play("Menu Start");
        this.settings = new Settings(this.theme, this.noOptions, this.speed, this.noQuestions);
        Debug.Log(this.settings);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void LoadAScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }


    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetSpeed(int speed)
    {
        //Debug.Log("Set speed: " + speed);
        this.speed = speed;
        this.speedText.text = "(" + speed + " s)";
    }

    public void SetNoOptions(int noOptions)
    {
        //Debug.Log("Set noOptions: " + noOptions);
        this.noOptions = noOptions;
    }

    public void SetTheme(string theme)
    {
        this.theme = theme;
        SetThemeOptions(theme);
        HighlightNoQuestions();
    }

    private void SetNoQuestions(int noQuestions)
    {
        this.noQuestions = noQuestions;
    }

    private void SetThemeOptions(string theme)
    {
        DestroyPreviousThemes();
        string[] themeOps = this.themeOptions[theme].ToString().Split(',');
        this.noQuestionsButtons = new Button[themeOps.Length];
        float startX = -(this.noQuestionButtonPanel.rect.width/2f * 1.1f);
        int i = 0;
        foreach(string s in themeOps)
        {
            GameObject btn = Instantiate(this.noQuestionsButtonPrefab);
            btn.transform.SetParent(this.noQuestionButtonPanel, false);
            float buttonWidth = btn.GetComponent<RectTransform>().rect.width;
            startX += buttonWidth;
            btn.GetComponent<RectTransform>().localPosition = new Vector2(startX, 0f);
            Button theButton = btn.GetComponent<Button>();
            this.noQuestionsButtons[i] = theButton;
            theButton.transform.Find("Value").GetComponent<Text>().text = s;
            theButton.transform.Find("Name").GetComponent<TMP_Text>().text = s;
            i++;
        }

    }

    private void DestroyPreviousThemes()
    {
        if (this.noQuestionsButtons != null)
        {
            foreach(Button b in this.noQuestionsButtons)
            {
                Destroy(b.gameObject);
            }
        }
    }


    void KeepHighlighted(Button button, Button[] group, string type)
    {
        foreach(Button b in group)
        {
            if(b != button)
                TurnNormalButton(b);
        }

        SelectButton(button);

        switch(type)
        {
            case "theme":
                SetTheme(button.transform.Find("Value").GetComponent<Text>().text);
                break;
            case "noOptions":
                SetNoOptions(Int32.Parse(button.transform.Find("Value").GetComponent<Text>().text));
                break;
            case "noQuestions":
                SetNoQuestions(Int32.Parse(button.transform.Find("Value").GetComponent<Text>().text));
                break;
        }
            
    }

    private void TurnNormalButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1, 1, 1, 1);
        button.colors = colors;
    }

    private void SelectButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.1462264f, 0.5601634f, 1, 1);
        button.colors = colors;
    }
}
