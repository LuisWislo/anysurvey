using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public string theme;
    public int noOptions;
    public int speed;
    public int noQuestions;

    public Settings(string theme, int noOptions, int speed, int noQuestions)
    {
        this.theme = theme;
        this.noOptions = noOptions;
        this.speed = speed;
        this.noQuestions = noQuestions;
    }

    public override string ToString()
    {
        return "{theme: " + theme + ", noOptions: " + noOptions + ", speed: " + speed + ", noQuestions: " + noQuestions + "}";
    }


}
