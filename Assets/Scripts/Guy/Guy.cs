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
        

        //inputs[0] = dirToMine.x;
        //inputs[1] = dirToMine.z;
        inputs[2] = transform.forward.x;
        inputs[3] = transform.forward.z;

        float[] output = brain.Synapsis(inputs);

        
    }

    protected override void OnTakeMine(GameObject mine)
    {
        fitness *= 2;
        genome.fitness = fitness;
    }
}
