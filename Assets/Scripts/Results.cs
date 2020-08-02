using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results : MonoBehaviour
{
    private Controller controller;
    private MenuManager menu;

    private OptionData[] resultData = new OptionData[]
    {
        // display, name, value
        new OptionData(null, null, "Reintentar", "retry"),
        new OptionData(null, null, "Regresar al Menu", "menu")
    };

    // Start is called before the first frame update
    void Start()
    {
        this.menu = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        this.controller = GetComponent<Controller>();
        this.SetupController();
    }

    void SetupController()
    {
        this.controller.ButtonSetup(resultData);
    }

    public void ActionRequest(string action)
    {
        switch(action)
        {
            case "retry":
                this.menu.LoadAScene("Game");
                break;
            case "menu":
                Destroy(this.menu.gameObject);
                this.menu.LoadAScene("Main Menu");
                break;
        }
        this.controller.ButtonEnd();
    }



}
