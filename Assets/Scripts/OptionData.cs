using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionData
{
    public string type = "";
    public string display = "";
    public string name = "";
    public string value = "";

    public OptionData(string type, string display, string name, string value)
    {
        this.type = type;
        this.display = display;
        this.name = name;
        this.value = value;
    }

    override public string ToString()
    {
        return "{ type: " + type + ", display: " + display + ", name: " + name + ", value: " + value + " }";
    }

}
