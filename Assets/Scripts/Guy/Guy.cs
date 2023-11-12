using UnityEngine;

public class Guy : GuyBase
{
    float fitness = 0;
    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt)
    {
        inputs[0] = nearFood == null ? 0 : nearFood.transform.position.x;
        inputs[1] = nearFood == null ? 0 : nearFood.transform.position.z;
        inputs[2] = nearSameGuy == null ? 0 : nearSameGuy.transform.position.x;
        inputs[3] = nearSameGuy == null ? 0 : nearSameGuy.transform.position.z;
        inputs[4] = MapCreator.Instance.foods.Count;
        //inputs[5] = 3.5f;
        //inputs[6] = 1;
        //inputs[7] = -234;


        float[] output = brain.Synapsis(inputs);

        Vector3 pastPos = transform.position;

        if(Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[0])
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

        // Set tank position
        transform.position = pos;

        if (pastPos == transform.position)
        {
            fitness -= 0.02f;
        }

        fitness -= inputs[4]/100000;
        if(fitness < 0)
        {
            fitness = 0;
        }
        genome.fitness = fitness;
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

    protected override void OnTakeFood(GameObject mine)
    {
        fitness += 500;
        if(foodTaken >= 2)
        {
            fitness += 1000;
        }
        genome.fitness = fitness;
        mine.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            foodTaken++;
            OnTakeFood(other.gameObject);
        }
    }

    public void CalculateFinalScore()
    {
        fitness -= MapCreator.Instance.foods.Count * 5;
        if (fitness < 0)
        {
            fitness = 0;
        }
        genome.fitness = fitness;
    }
}
