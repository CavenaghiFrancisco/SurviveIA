using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{
    public Text populationCountTxt;
    public Slider populationCountSlider;
    public Text FoodCountTxt;
    public Slider FoodCountSlider;
    public Text GenerationTurnDurationTxt;
    public Slider GenerationTurnDurationSlider;
    public Text eliteCountTxt;
    public Slider eliteCountSlider;
    public Text mutationChanceTxt;
    public Slider mutationChanceSlider;
    public Text mutationRateTxt;
    public Slider mutationRateSlider;
    public Text hiddenLayersCountTxt;
    public Slider hiddenLayersCountSlider;
    public Text neuronsPerHLCountTxt;
    public Slider neuronsPerHLSlider;
    public Text biasTxt;
    public Slider biasSlider;
    public Text sigmoidSlopeTxt;
    public Slider sigmoidSlopeSlider;
    public Button startButton;
    public GameObject simulationScreen;

    public PopulationManager tribe;

    string populationText;
    string minesText;
    string GenerationTurnDurationText;
    string elitesText;
    string mutationChanceText;
    string mutationRateText;
    string hiddenLayersCountText;
    string biasText;
    string sigmoidSlopeText;
    string neuronsPerHLCountText;

    void Start()
    {   
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        FoodCountSlider.onValueChanged.AddListener(OnFoodCountChange);
        GenerationTurnDurationSlider.onValueChanged.AddListener(OnGenerationTurnDurationChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        minesText = FoodCountTxt.text;
        GenerationTurnDurationText = GenerationTurnDurationTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value = tribe.PopulationCount;
        FoodCountSlider.value = MapCreator.Instance.FoodCount;
        GenerationTurnDurationSlider.value = tribe.GenerationTurnDuration;
        eliteCountSlider.value = tribe.EliteCount;
        mutationChanceSlider.value = tribe.MutationChance * 100.0f;
        mutationRateSlider.value = tribe.MutationRate * 100.0f;
        hiddenLayersCountSlider.value = tribe.HiddenLayers;
        neuronsPerHLSlider.value = tribe.NeuronsCountPerHL;
        biasSlider.value = -tribe.Bias;
        sigmoidSlopeSlider.value = tribe.P;

        startButton.onClick.AddListener(OnStartButtonClick);        
    }

    void OnPopulationCountChange(float value)
    {
        tribe.PopulationCount = (int)value;
        
        populationCountTxt.text = string.Format(populationText, tribe.PopulationCount);
    }

    void OnFoodCountChange(float value)
    {
        MapCreator.Instance.FoodCount = (int)value;        

        FoodCountTxt.text = string.Format(minesText, MapCreator.Instance.FoodCount);
    }

    void OnGenerationTurnDurationChange(float value)
    {
        tribe.GenerationTurnDuration = (int)value;
        
        GenerationTurnDurationTxt.text = string.Format(GenerationTurnDurationText, tribe.GenerationTurnDuration);
    }

    void OnEliteCountChange(float value)
    {
        tribe.EliteCount = (int)value;

        eliteCountTxt.text = string.Format(elitesText, tribe.EliteCount);
    }

    void OnMutationChanceChange(float value)
    {
        tribe.MutationChance = value / 100.0f;

        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(tribe.MutationChance * 100));
    }

    void OnMutationRateChange(float value)
    {
        tribe.MutationRate = value / 100.0f;

        mutationRateTxt.text = string.Format(mutationRateText, (int)(tribe.MutationRate * 100));
    }

    void OnHiddenLayersCountChange(float value)
    {
        tribe.HiddenLayers = (int)value;
        

        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, tribe.HiddenLayers);
    }

    void OnNeuronsPerHLChange(float value)
    {
        tribe.NeuronsCountPerHL = (int)value;

        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, tribe.NeuronsCountPerHL);
    }

    void OnBiasChange(float value)
    {
        tribe.Bias = -value;

        biasTxt.text = string.Format(biasText, tribe.Bias.ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        tribe.P = value;

        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, tribe.P.ToString("0.00"));
    }


    void OnStartButtonClick()
    {
        tribe.StartSimulation();
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);
    }
    
}
