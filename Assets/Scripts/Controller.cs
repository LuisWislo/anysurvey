using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    // Audio
    private AudioManager gameAudio;

    // Buttons
    public GameObject buttonPrefab;
    private Button[] selectables;
    private Button selectedEntity;
    public RectTransform buttonPanel;
    private Results results;
    private int currentSelectableIndex = 0;

    // Data
    private QuestionnaireManager questionnaire;
    private OptionData[] options;
    
    private Coroutine lastCoroutine = null;

    private MenuManager menu;

    private float SELECT_TRANSITION_WAIT;
    private bool paused = false;

    void Awake()
    {
        this.gameAudio = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        this.menu = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        SELECT_TRANSITION_WAIT = this.menu.settings.speed;
        this.questionnaire = GetComponent<QuestionnaireManager>();
        this.results = GetComponent<Results>();
    }

    // QuestionnaireManager sends Button info (no. buttons, names, pictures, etc.)
    // Type questionnaire/menu
    public void ButtonSetup(OptionData[] options)
    {
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        DestroyButtons();
        SpawnButtons(options);
        this.options = options;
        DecorateButtons();
        this.currentSelectableIndex = 0;
        lastCoroutine = StartCoroutine(ButtonLoop(options[0].value));
    }

    private void DestroyButtons()
    {
        if(selectables != null)
            foreach(Button b in selectables)
            {
                Destroy(b.gameObject);
            }
    }

    private void SpawnButtons(OptionData[] options)
    {
        List<GameObject> gos = new List<GameObject>();
        float buttonWidth = 0f;

        foreach (OptionData op in options)
        {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(buttonPanel, false);
            buttonWidth = newButton.GetComponent<RectTransform>().rect.width;
            gos.Add(newButton);
        }

        float panelWidth = buttonPanel.rect.width;
        float spaceWidth = (panelWidth - (buttonWidth * options.Length)) / options.Length;
        float startX = (spaceWidth / 2f) - (panelWidth/2f);

        this.selectables = new Button[gos.Count];
        int i = 0;

        foreach (GameObject gObj in gos) 
        {
            gObj.GetComponent<RectTransform>().localPosition = new Vector2(startX + (buttonWidth/2f), 0f);
            startX += buttonWidth + spaceWidth;
            this.selectables[i++] = gObj.GetComponent<Button>();
        }

    }
    
    public void ButtonEnd()
    {
        foreach(Button b in selectables)
        {
            TurnNormalButton(b);
        }
        StopCoroutine(lastCoroutine);
    }

    // Set buttons' text to match options passed by questionnaire.
    private void DecorateButtons()
    {
        for (int i = 0; i < options.Length; i++)
        {
            selectables[i].transform.Find("Name").GetComponent<TMP_Text>().text = options[i].name;
            selectables[i].transform.Find("Value").GetComponent<Text>().text = options[i].value;
            //also set image or resource
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ButtonSelect()); 
        }   
    }

    IEnumerator ButtonSelect()
    {
        this.gameAudio.Play("Menu Select");
        SelectButton(this.selectedEntity);
        string selected = this.selectedEntity.transform.Find("Value").GetComponent<Text>().text;
        paused = true;

        yield return new WaitForSeconds(1f);
        if (this.questionnaire != null)
            this.questionnaire.Evaluate(selected);
        else
            this.results.ActionRequest(selected);
        TurnNormalButton(this.selectedEntity);
        paused = false;
    }

    IEnumerator ButtonLoop(string id)
    {
        while(true)
        {
            while (paused)
            {
                //Debug.Log("paused");
                yield return null;
            }
            
            this.selectedEntity = this.selectables[currentSelectableIndex];
            Debug.Log("current selection: " + this.selectedEntity.transform.Find("Value").GetComponent<Text>().text + ", index: " + currentSelectableIndex);
            Debug.Log(SELECT_TRANSITION_WAIT);
            HighlightButton(this.selectedEntity);
            yield return new WaitForSeconds(SELECT_TRANSITION_WAIT);
            TurnNormalButton(this.selectedEntity);
            currentSelectableIndex = (currentSelectableIndex + 1) % this.selectables.Length;
        }
    }

    // Change current button's color to match highlighted state.
    private void HighlightButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(.9811321f, .745105f, .745105f, 1);
        button.colors = colors;
    }

    // Change current button's color to match normal state.
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
