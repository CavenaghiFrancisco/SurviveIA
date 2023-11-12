using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSimulation : MonoBehaviour
{
    public Text timerTxt;
    public Slider timerSlider;
    public Text GenerationTurnDurationTxt;
    public Slider GenerationTurnDurationSlider;
    string timerText;
    string GenerationTurnDurationText;

    private void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        GenerationTurnDurationSlider.onValueChanged.AddListener(OnGenerationTurnDurationChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, PopulationManager.IterationCount);

        GenerationTurnDurationText = GenerationTurnDurationTxt.text;
        GenerationTurnDurationSlider.value = PopulationManager.GenerationTurnDuration;
    }

    void OnTimerChange(float value)
    {
        PopulationManager.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, PopulationManager.IterationCount);
    }

    void OnGenerationTurnDurationChange(float value)
    {
        PopulationManager.GenerationTurnDuration = (int)value;

        GenerationTurnDurationTxt.text = string.Format(GenerationTurnDurationText, PopulationManager.GenerationTurnDuration);
    }
}
