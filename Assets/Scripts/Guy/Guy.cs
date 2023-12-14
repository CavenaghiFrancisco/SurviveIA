using System;
using System.Collections.Generic;
using UnityEngine;

public class Guy : GuyBase
{
    float fitness = 0;
    float timeBetweenEat = 0;
    float[] output;
    public Vector3 pastPos;
    public List<Vector3> posChecked = new List<Vector3>();
    public int lastChoice = -1;
    public int currentChoice = -1;

    protected override void OnReset()
    {
        fitness = 1;
        posChecked = new List<Vector3>();
    }

    protected override void OnThink(float dt)
    {
        if(GameManager.Instance.CurrentState != STATES.FIGHT)
        {
            inputs[0] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[1] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[2] = transform.position.x;
            inputs[3] = transform.position.z;
            inputs[4] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[5] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[6] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[7] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[8] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[9] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[10] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[11] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[12] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[13] = nearFood == null ? 0 : nearFood.transform.position.z;
        }
        
        if(GameManager.Instance.CurrentState == STATES.FIGHT)
        {
            inputs[0] = nearFood == null ? 0 : nearFood.transform.position.x;
            inputs[1] = nearFood == null ? 0 : nearFood.transform.position.z;
            inputs[2] = transform.position.x;
            inputs[3] = transform.position.z;
            inputs[4] = nearDifferentGuy == null ? 0 : nearDifferentGuy.transform.position.x;
            inputs[5] = nearDifferentGuy == null ? 0 : nearDifferentGuy.transform.position.z;
            inputs[6] = nearSameGuy == null ? 0 : nearSameGuy.transform.position.x;
            inputs[7] = nearSameGuy == null ? 0 : nearSameGuy.transform.position.z;
            inputs[8] = nearSameGuy.GetComponent<Guy>().genome.generationsAlive;
            inputs[9] = nearSameGuy.GetComponent<Guy>().genome.generationsAlive;
            inputs[10] = nearDifferentGuy == null ? 0 : nearDifferentGuy.GetComponent<Guy>().genome.generationsAlive;
            inputs[11] = nearSameGuy == null ? 0 : nearSameGuy.GetComponent<Guy>().foodTaken;
            inputs[12] = nearDifferentGuy == null ? 0 : nearDifferentGuy.GetComponent<Guy>().foodTaken;
            inputs[13] = foodTaken;
        }
        


        output = brain.Synapsis(inputs);

        pastPos = transform.position;

        if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[0])
        {
            transform.position -= transform.right;
            currentChoice = 0;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[1])
        {
            transform.position += transform.right;
            currentChoice = 1;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[2])
        {
            transform.position += transform.forward;
            currentChoice = 2;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[3])
        {
            transform.position -= transform.forward;
            currentChoice = 3;
        }
        else
        {
            currentChoice = 4;
        }

        Vector3 pos = transform.position;
        if (pos.x > MapCreator.Instance.sizeX)
            pos.x = 0;
        else if (pos.x < 0)
            pos.x = MapCreator.Instance.sizeX - 1;

        if (pos.z < 0)
            pos.z = 0;
        else if (pos.z > MapCreator.Instance.sizeY - 1)
            pos.z = MapCreator.Instance.sizeY - 1;

        if (lastChoice != -1 && currentChoice != lastChoice)
        {
            int count = 0;
            for(int i = 0; i < Mathf.Min(5,posChecked.Count); i++)
            {
                count++;
                if(posChecked[i] == pos)
                    break;
            }
            if (!posChecked.Contains(pos) && pos.z > MapCreator.Instance.sizeY + 5 && pos.z < MapCreator.Instance.sizeY - 5)
            {
                genome.fitness += 120;
            }
            else
            {
                if (count == Mathf.Min(5, posChecked.Count))
                    genome.fitness += 30;
            }
        }

        lastChoice = currentChoice;
        transform.position = pos;
        posChecked.Add(pos);
    }

    public void SetNearestFood(GameObject food)
    {
        nearFood = food;
    }

    public void SetNearestGuy(GameObject guy)
    {
        nearSameGuy = guy;
    }

    public void SetNearestDifferentGuy(GameObject guy)
    {
        nearDifferentGuy = guy;
    }

    public override void OnTakeFood(GameObject mine, bool isShared)
    {
        if (isShared)
        {
            foodTaken++;
        }
        else
        {
            foodTaken += 0.5f;
        }
        mine.SetActive(false);
    }

    public void CalculateFinalScore()
    {
        if(GameManager.Instance.CurrentState != STATES.SEARCH)
        {
            fitness = foodTaken * 1000;
            genome.fitness += fitness;
        }
        if (genome.fitness <= 0)
        {
            genome.fitness = 1;
        }
    }

    public CLASH_RESULT TakeClashDecision()
    {
        if (output[0] >= 0.5f)
        {
            return CLASH_RESULT.STAY;
        }
        else
        {
            return CLASH_RESULT.RUN;
        }
    }

    public Vector3 GetRandomDirection()
    {
        Vector3 up = Vector3.forward;
        Vector3 down = -Vector3.forward;
        Vector3 right = Vector3.right;
        Vector3 left = Vector3.left;

        Vector3[] directions = { up, down, right, left };

        int randomIndex = UnityEngine.Random.Range(0, directions.Length);

        Vector3 randomDirection = directions[randomIndex];

        return randomDirection;
    }


}
