using UnityEngine;
using System.Collections;

public class GuyBase : MonoBehaviour
{
    protected Genome genome;
	protected NeuralNetwork brain;
    protected GameObject nearFood;
    protected GameObject nearSameGuy;
    protected GameObject nearDifferentGuy;
    public int generationsSurvived = 0;
    public int foodTaken = 0;
    protected float[] inputs;

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeFood(GameObject food)
    {
    }

    protected virtual void OnReset()
    {

    }
}
