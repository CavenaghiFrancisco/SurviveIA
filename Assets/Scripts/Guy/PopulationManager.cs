using UnityEngine;
using System.Collections.Generic;

public enum SIDE
{
    UP,
    DOWN
}

public class PopulationManager : MonoBehaviour
{
    public GameObject GuyPrefab;

    public SIDE sideOfTribe;

    public int PopulationCount = 40;

    public int GenerationTurnDuration = 20;
    public int IterationCount = 1;

    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;

    public int InputsCount = 4;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;


    GeneticAlgorithm genAlg;

    List<Guy> populationGOs = new List<Guy>();
    List<Genome> population = new List<Genome>();
    List<NeuralNetwork> brains = new List<NeuralNetwork>();
    List<GameObject> mines = new List<GameObject>();
    List<GameObject> goodMines = new List<GameObject>();
    List<GameObject> badMines = new List<GameObject>();

    float accumTime = 0;
    bool isRunning = false;

    public int generation
    {
        get; private set;
    }

    public float bestFitness
    {
        get; private set;
    }

    public float avgFitness
    {
        get; private set;
    }

    public float worstFitness
    {
        get; private set;
    }

    private float getBestFitness()
    {
        float fitness = 0;
        foreach (Genome g in population)
        {
            if (fitness < g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    private float getAvgFitness()
    {
        float fitness = 0;
        foreach (Genome g in population)
        {
            fitness += g.fitness;
        }

        return fitness / population.Count;
    }

    private float getWorstFitness()
    {
        float fitness = float.MaxValue;
        foreach (Genome g in population)
        {
            if (fitness > g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    public void StartSimulation()
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        GenerateInitialPopulation();

        isRunning = false; //AAAAAAAAA;
    }

    public void PauseSimulation()
    {
        isRunning = !isRunning;
    }

    public void StopSimulation()
    {
        isRunning = false;

        generation = 0;

        // Destroy previous tanks (if there are any)
        DestroyGuys();

        // Destroy all mines
        DestroyMines();
    }

    // Generate the random initial population
    void GenerateInitialPopulation()
    {
        generation = 0;

        // Destroy previous tanks (if there are any)
        DestroyGuys();

        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();

            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            brains.Add(brain);

            population.Add(genome);
            populationGOs.Add(CreateGuy(genome, brain, i, sideOfTribe));
        }

        accumTime = 0.0f;
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain()
    {
        NeuralNetwork brain = new NeuralNetwork();

        // Add first neuron layer that has as many neurons as inputs
        brain.AddFirstNeuronLayer(InputsCount, Bias, P);

        for (int i = 0; i < HiddenLayers; i++)
        {
            // Add each hidden layer with custom neurons count
            brain.AddNeuronLayer(NeuronsCountPerHL, Bias, P);
        }

        // Add the output layer with as many neurons as outputs
        brain.AddNeuronLayer(OutputsCount, Bias, P);

        return brain;
    }

    // Evolve!!!
    void Epoch()
    {
        // Increment generation counter
        generation++;

        // Calculate best, average and worst fitness
        bestFitness = getBestFitness();
        avgFitness = getAvgFitness();
        worstFitness = getWorstFitness();

        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(population.ToArray());

        // Clear current population
        population.Clear();

        // Add new population
        population.AddRange(newGenomes);

        // Set the new genomes as each NeuralNetwork weights
        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = brains[i];

            brain.SetWeights(newGenomes[i].genome);

            populationGOs[i].SetBrain(newGenomes[i], brain);
            //populationGOs[i].transform.position = GetRandomPos();
            //populationGOs[i].transform.rotation = GetRandomRot();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isRunning)
            return;

        float dt = 1;


        foreach (Guy t in populationGOs)
        {
            //// Get the nearest mine
            //GameObject mine = GetNearestMine(t.transform.position);

            //// Set the nearest mine to current tank
            //t.SetNearestMine(mine);

            //mine = GetNearestGoodMine(t.transform.position);

            //// Set the nearest mine to current tank
            //t.SetGoodNearestMine(mine);

            //mine = GetNearestBadMine(t.transform.position);

            //// Set the nearest mine to current tank
            //t.SetBadNearestMine(mine);

            // Think!! 
            t.Think(dt);

            // Just adjust tank position when reaching world extents
            Vector3 pos = t.transform.position;
            if (pos.x > MapCreator.Instance.sizeX)
                pos.x = 0;
            else if (pos.x < 0)
                pos.x = MapCreator.Instance.sizeX;

            // Set tank position
            t.transform.position = pos;
        }

        // Check the time to evolve
        accumTime += dt;
        if (accumTime >= GenerationTurnDuration)
        {
            accumTime -= GenerationTurnDuration;
            Epoch();
        }

    }

    #region Helpers
    Guy CreateGuy(Genome genome, NeuralNetwork brain, int iteration, SIDE side)
    {
        Vector3 position;
        if (side == SIDE.DOWN)
        {
            position = new Vector3(iteration - MapCreator.Instance.sizeX * (iteration / MapCreator.Instance.sizeX), 0, iteration / MapCreator.Instance.sizeX);
        }
        else
        {
            position = new Vector3(MapCreator.Instance.sizeX - (iteration - MapCreator.Instance.sizeX * (iteration / MapCreator.Instance.sizeX)), 0.5f, MapCreator.Instance.sizeY - (iteration / MapCreator.Instance.sizeX));
        }
        GameObject go = Instantiate<GameObject>(GuyPrefab, position, Quaternion.identity);
        Guy t = go.GetComponent<Guy>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyMines()
    {
        foreach (GameObject go in mines)
            Destroy(go);

        mines.Clear();
        goodMines.Clear();
        badMines.Clear();
    }

    void DestroyGuys()
    {
        foreach (Guy go in populationGOs)
            Destroy(go.gameObject);

        populationGOs.Clear();
        population.Clear();
        brains.Clear();
    }

    

    void SetMineGood(bool good, GameObject go)
    {
        if (good)
        {
            go.GetComponent<Renderer>().material.color = Color.green;
            goodMines.Add(go);
        }
        else
        {
            go.GetComponent<Renderer>().material.color = Color.red;
            badMines.Add(go);
        }

    }

    //Vector3 GetRandomPos()
    //{
    //    return new Vector3(Random.value * SceneHalfExtents.x * 2.0f - SceneHalfExtents.x, 0.0f, Random.value * SceneHalfExtents.z * 2.0f - SceneHalfExtents.z);
    //}

    GameObject GetNearestMine(Vector3 pos)
    {
        GameObject nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (GameObject go in mines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

    

    #endregion

}
