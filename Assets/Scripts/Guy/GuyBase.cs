﻿using UnityEngine;
using System.Collections;

public class GuyBase : MonoBehaviour
{
    public Genome genome;
	protected NeuralNetwork brain;
    protected GameObject nearFood;
    protected GameObject nearSameGuy;
    protected GameObject nearDifferentGuy;
    public int generationsSurvived = 0;
    public float foodTaken = 0;
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

    public virtual void OnTakeFood(GameObject food, bool isShared)
    {
    }

    protected virtual void OnReset()
    {

    }
}
