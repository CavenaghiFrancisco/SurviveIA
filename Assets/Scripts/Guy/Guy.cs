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
        inputs[2] = 3;
        inputs[3] = 41;
        inputs[4] = 68;
        inputs[5] = 3.5f;
        inputs[6] = 1;
        inputs[7] = -234;


        float[] output = brain.Synapsis(inputs);

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
            transform.position = Vector3.forward;
        }
        else if (Mathf.Max(output[0], output[1], output[2], output[3], output[4]) == output[3])
        {
            transform.position -= Vector3.forward;
        }
        else
        {
        }
    }

    public void SetNearestFood(GameObject food)
    {
        nearFood = food;
    }

    protected override void OnTakeMine(GameObject mine)
    {
       
    }
}
