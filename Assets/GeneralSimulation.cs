using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSimulation : MonoBehaviour
{
    public Text timerTxt;
    public Slider timerSlider;
    string timerText;

    private void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, PopulationManager.IterationCount);
    }

    void OnTimerChange(float value)
    {
        PopulationManager.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, PopulationManager.IterationCount);
    }
}
