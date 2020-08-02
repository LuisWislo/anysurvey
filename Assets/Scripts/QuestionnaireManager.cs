using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class QuestionnaireManager : MonoBehaviour
{
    // Audio
    private AudioManager gameAudio;

    private string file;
    private int noQuestions;
    private int noOptionsPerQuestion;
    private Controller controller;
    private int currentQuestionIndex = 0;
    public TMP_Text score;
    private int currentScore = -1;
    private MenuManager menu;
    private OptionData[] testData;
    public RectTransform hintPanel;

    //Prefabs
    public GameObject imgDisplay;
    public GameObject txtDisplay;

    //Objects from prefabs
    private RawImage currentImgDisplay;
    private TMP_Text currentTxtDisplay;

    void Awake()
    {
        this.gameAudio = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        this.menu = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        LoadSources(this.menu.settings.theme);
        Setup();
        UpdateScore();
        this.controller = GetComponent<Controller>();
        this.NewQuestion();
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            Destroy(this.menu.gameObject);
            this.menu.LoadAScene("Main Menu");
        }
    }

    void Setup()
    {
        this.noQuestions = this.menu.settings.noQuestions;
        this.noOptionsPerQuestion = this.menu.settings.noOptions;
        this.file = this.menu.settings.theme + ".txt";
        Randomize(this.testData);
    }

    public void NewQuestion()
    {
        if (currentQuestionIndex < noQuestions)
        {
            // Testing purposes, in reality, will set picture or sound or whatever
            DisplayHint();

            // ToDo: Randomize array of options and send them to controller
            OptionData[] questionOps = GetRandomOptions(currentQuestionIndex);
            Randomize(questionOps);
            this.controller.ButtonSetup(questionOps);
        }
        else
        {
            this.gameAudio.Play("Win");
            this.menu.LoadAScene("Results Screen");
            this.controller.ButtonEnd();
        }
    }

    private void DisplayHint()
    {
        DestroyPreviousHint();
        string path = "Themes/" + this.menu.settings.theme;
        string newPath = Application.streamingAssetsPath + "/Resources/Themes/" + this.menu.settings.theme;
        switch (testData[currentQuestionIndex].type)
        {
            case "img":
                GameObject imgObj = Instantiate(this.imgDisplay);
                imgObj.transform.SetParent(this.hintPanel, false);
                RectTransform rect = imgObj.GetComponent<RectTransform>();
                rect.localPosition = new Vector2(0f, 0f);
                rect.sizeDelta = new Vector2(this.hintPanel.rect.height, this.hintPanel.rect.height);
                this.currentImgDisplay = imgObj.GetComponent<RawImage>();

                byte[] imageBytes = System.IO.File.ReadAllBytes(newPath + "/" + testData[currentQuestionIndex].display);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                this.currentImgDisplay.texture = tex;

                //this.currentImgDisplay.texture = Resources.Load<Texture>(path + "/" + testData[currentQuestionIndex].display);
                break;

            case "txt":
                GameObject txtObj = Instantiate(this.txtDisplay);
                txtObj.transform.SetParent(this.hintPanel, false);
                RectTransform rectTxt = txtObj.GetComponent<RectTransform>();
                //Debug.Log(rectTxt);
                rectTxt.localPosition = new Vector2(0f, 0f);
                this.currentTxtDisplay = txtObj.GetComponent<TMP_Text>();
                //Debug.Log(this.currentTxtDisplay);
                RectTransform textArea = this.currentTxtDisplay.GetComponent<RectTransform>();
                textArea.sizeDelta = new Vector2(this.hintPanel.rect.width * .9f, this.hintPanel.rect.height * .9f);
                this.currentTxtDisplay.text = testData[currentQuestionIndex].display;
                break;
        }
    }

    private void DestroyPreviousHint()
    {
        if(this.currentImgDisplay != null)
        {
            Destroy(this.currentImgDisplay.gameObject); //does this make the child object null?? It doesnt.
        }

        if(this.currentTxtDisplay != null)
        {
            Destroy(this.currentTxtDisplay.gameObject);
        }

        //if(this.currentWhateverDisplay != null) { Destroy(this.currentWhateverDisplay.gameObject); }
    }


    private OptionData[] GetRandomOptions(int ignoreIndex)
    {
        // Needs improvement to ignore previous questions (maybe not)
        // list/whatever of indexes to ignore
        List<int> toIgnore = new List<int>();
        toIgnore.Add(ignoreIndex);

        while(toIgnore.Count < noOptionsPerQuestion)
        {
            int random = Random.Range(0, testData.Length);
            if (!IndexIsInCollection(toIgnore, random))
                toIgnore.Add(random);
        }

        OptionData[] output = new OptionData[toIgnore.Count];

        int j = 0;

        foreach(int index in toIgnore){
            output[j++] = testData[index];
        }

        return output;
    }

    private bool IndexIsInCollection(List<int> collection, int index)
    {
        foreach(int n in collection)
        {
            if (index == n)
                return true;
        }
        return false;
    }


    private void LogArray(OptionData[] collection)
    {
        foreach(OptionData op in collection)
        {
            Debug.Log(op.ToString());
        }
    }

    public void Evaluate(string selectedValue)
    {
        if(selectedValue == testData[currentQuestionIndex].value)
        {
            this.gameAudio.Play("Menu Start");
            this.currentQuestionIndex++;
            UpdateScore();
            NewQuestion();
        }
        else
        {
            this.gameAudio.Play("Incorrect");
            //Signal incorrect answer, 
        }
    }

    private void UpdateScore()
    {
        this.currentScore++;
        this.score.text = currentScore.ToString() + "/" + noQuestions;
    }

    public void RequestExit()
    {
        this.menu.ExitGame();
    }

    private void LoadSources(string theme)
    {
        string newPath = Application.streamingAssetsPath + "/Resources/Themes/" + theme + "/" + theme + ".txt";
        StreamReader reader = new StreamReader(newPath);
        string[] data = reader.ReadToEnd().Split('\n');
        reader.Close();

        List<OptionData> tmp = new List<OptionData>();
        foreach(string s in data)
        {
            OptionData od = getOptionDataObject(s);
            if (od != null)
                tmp.Add(od);
        }
        this.testData = new OptionData[tmp.Count];
        int i = 0;
        foreach(OptionData opd in tmp)
        {
            this.testData[i++] = opd;
        }

    }

    private OptionData getOptionDataObject(string commaSeparated)
    {
        if(commaSeparated != "")
        {
            string[] data = commaSeparated.Split(',');
            return new OptionData(data[0], data[1], data[2], data[3]);
        }

        return null; 
    }

    public static void Randomize<T>(T[] items)
    {
        System.Random rand = new System.Random();

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = rand.Next(i, items.Length);
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }

}
