using System;
using UnityEngine;

public class Guy : GuyBase
{
    float fitness = 0;
    float timeBetweenEat = 0;
    float[] output;
    public Vector3 pastPos;

    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt)
    {
        inputs[0] = nearFood == null ? 0 : nearFood.transform.position.x;
        inputs[1] = nearFood == null ? 0 : nearFood.transform.position.z;
        inputs[2] = transform.position.x;
        inputs[3] = transform.position.z;


        output = brain.Synapsis(inputs);

        pastPos = transform.position;

        if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[0])
        {
            transform.position -= Vector3.right;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[1])
        {
            transform.position += Vector3.right;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[2])
        {
            transform.position += Vector3.forward;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[3])
        {
            transform.position -= Vector3.forward;
        }
        else
        {
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

        transform.position = pos;

        timeBetweenEat += Time.deltaTime;
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
        fitness += 1f / timeBetweenEat * 500f;
        timeBetweenEat = 0;
        genome.fitness = fitness;
        mine.SetActive(false);
    }

    public void CalculateFinalScore()
    {
        fitness = foodTaken * 500;
        genome.fitness = fitness;
    }

    public CLASH_RESULT TakeClashDecision(bool isGoodCollision)
    {
        if (output[0] >= 0.5f)
        {
            transform.position = isGoodCollision ? transform.position + GetRandomDirection() : pastPos;
            return CLASH_RESULT.STAY;
        }
        else
        {
            return CLASH_RESULT.RUN;
        }
    }

    public Vector3 GetRandomDirection()
    {
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 right = Vector3.right;
        Vector3 left = Vector3.left;

        Vector3[] directions = { up, down, right, left };

        int randomIndex = UnityEngine.Random.Range(0, directions.Length);

        Vector3 randomDirection = directions[randomIndex];

        return randomDirection;
    }


}
