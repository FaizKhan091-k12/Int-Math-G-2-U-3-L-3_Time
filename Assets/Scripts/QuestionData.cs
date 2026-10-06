using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum QuestionVisualMode
{
    AnalogClock,
    DigitalClock,
    SchoolAssembly,
    MovieCinema,
    TimelineJump
}

[System.Serializable]
public class QuestionData
{
    public int questionNumber;
    [TextArea(2, 4)]
    public string prompt;
    public QuestionVisualMode visualMode;
    public int clockHour;
    public int clockMinute;
    public string digitalDisplayString;
    public string[] options = new string[3];
    public int correctOptionIndex; // 0 = A, 1 = B, 2 = C
    [TextArea(2, 4)]
    public string explanation;
    public string detectiveClue;

    public QuestionData(int qNum, string prompt, QuestionVisualMode visualMode, int hour, int minute, 
        string digitalStr, string[] opts, int correctIdx, string explanation, string clue)
    {
        this.questionNumber = qNum;
        this.prompt = prompt;
        this.visualMode = visualMode;
        this.clockHour = hour;
        this.clockMinute = minute;
        this.digitalDisplayString = digitalStr;
        this.options = opts;
        this.correctOptionIndex = correctIdx;
        this.explanation = explanation;
        this.detectiveClue = clue;
    }
}
