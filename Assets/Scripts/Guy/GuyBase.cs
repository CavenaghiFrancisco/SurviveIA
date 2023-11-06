using UnityEngine;
using System.Collections;

public class GuyBase : MonoBehaviour
{
    protected Genome genome;
	protected NeuralNetwork brain;
    protected GameObject nearFood;
    protected GameObject nearSameGuy;
    protected GameObject nearDifferentGuy;
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

        //if(IsCloseToMine(nearMine))
        //{
        //    OnTakeMine(nearMine);
        //    PopulationManager.Instance.RelocateMine(nearMine);
        //}
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(GameObject mine)
    {
    }

    protected virtual void OnReset()
    {

    }
}
