using UnityEngine;
using System.Collections.Generic;
using System;

public enum SIDE
{
    UP,
    DOWN
}

public class PopulationManager : MonoBehaviour
{
    public TextAsset genomesData;

    public GameObject GuyPrefab;

    public static Action<int, List<Vector3>> OnPopulationCreated;

    public Action OnPopulationDeleted;

    public SIDE sideOfTribe;

    public int PopulationCount = 40;

    public static int GenerationTurnDuration = 20;

    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;

    public int InputsCount = 4;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;


    GeneticAlgorithm genAlg;

    public List<Guy> populationGOs = new List<Guy>();
    public List<Genome> population = new List<Genome>();
    List<NeuralNetwork> brains = new List<NeuralNetwork>();

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
        genAlg = new GeneticAlgorithm(MutationChance, MutationRate);

        GenerateInitialPopulation(genomesData != null);

        isRunning = false; //TODO: cambiar a true;
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
    }

    // Generate the random initial population
    void GenerateInitialPopulation(bool hasCreatedGenomes)
    {
        if (hasCreatedGenomes)
        {
            List<Genome> genomes = Serializator.DeserializeGenomes(genomesData.ToString());
            genomesData = null;

            generation = 0;

            // Destroy previous tanks (if there are any)
            DestroyGuys();

            for (int i = 0; i < genomes.Count; i++)
            {
                NeuralNetwork brain = CreateBrain();

                brain.SetWeights(genomes[i].genome);
                brains.Add(brain);

                population.Add(genomes[i]);
                populationGOs.Add(CreateGuy(genomes[i], brain, i, sideOfTribe));
            }

            List<Vector3> positionsUsed = new List<Vector3>();

            foreach (Guy guy in populationGOs)
            {
                positionsUsed.Add(guy.gameObject.transform.position);
            }

            OnPopulationCreated?.Invoke(PopulationCount, positionsUsed);

            accumTime = 0.0f;
        }
        else
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

            List<Vector3> positionsUsed = new List<Vector3>();

            foreach (Guy guy in populationGOs)
            {
                positionsUsed.Add(guy.gameObject.transform.position);
            }

            OnPopulationCreated?.Invoke(PopulationCount, positionsUsed);

            accumTime = 0.0f;
        }
        
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
    public bool Epoch()
    {
        // Increment generation counter
        generation++;

        // Calculate best, average and worst fitness
        bestFitness = getBestFitness();
        avgFitness = getAvgFitness();
        worstFitness = getWorstFitness();

        int l = 0;
        foreach (Guy guy in populationGOs)
        {
            population[l].generationsAlive++;

            if (guy.foodTaken >= 1)
            {
                population[l].ableToLive = true;
                if(guy.foodTaken >= 2)
                {
                    population[l].ableToReproduce = true;
                }
                else
                {
                    population[l].ableToReproduce = false;
                }
            }
            else
            {
                population[l].ableToLive = false;
                population[l].ableToReproduce = false;
            }

            if(population[l].generationsAlive >= 3)
            {
                population[l].ableToLive = false;
                population[l].ableToReproduce = false;
            }
            l++;
        }



        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(population.ToArray());

        DestroyGuys();

        // Add new population
        population.AddRange(newGenomes);


        for (int i = 0; i < population.Count; i++)
        {
            NeuralNetwork brain = CreateBrain();

            brain.SetWeights(newGenomes[i].genome);
            brains.Add(brain);

            populationGOs.Add(CreateGuy(newGenomes[i], brain, i, sideOfTribe));
        }

        List<Vector3> positionsUsed = new List<Vector3>();

        foreach (Guy guy in populationGOs)
        {
            positionsUsed.Add(guy.gameObject.transform.position);
        }

        if(populationGOs.Count <= 0)
        {
            return false;
        }

        OnPopulationCreated?.Invoke(PopulationCount, positionsUsed);

        return true;
    }

    public void Respawn(PopulationManager tribe)
    {
        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(tribe.population.ToArray(),true);

        DestroyGuys();

        // Add new population
        population.AddRange(newGenomes);


        for (int i = 0; i < population.Count; i++)
        {
            NeuralNetwork brain = CreateBrain();

            brain.SetWeights(newGenomes[i].genome);
            brains.Add(brain);

            populationGOs.Add(CreateGuy(newGenomes[i], brain, i, sideOfTribe));
        }

        List<Vector3> positionsUsed = new List<Vector3>();

        foreach (Guy guy in populationGOs)
        {
            positionsUsed.Add(guy.gameObject.transform.position);
        }

        OnPopulationCreated?.Invoke(PopulationCount, positionsUsed);
    }

    // Update is called once per frame
    public void PlayTurn()
    {
        //if (!isRunning)
        //    return;

        float dt = 1;


        foreach (Guy t in populationGOs)
        {
            //// Get the nearest food
            GameObject food = GetNearestFood(t.transform.position);

            //// Set the nearest mine to current tank
            t.SetNearestFood(food);


            GameObject guy = GetNearestGuy(t.transform.position);

            t.SetNearestGuy(guy);
            //mine = GetNearestGoodMine(t.transform.position);

            //// Set the nearest mine to current tank
            //t.SetGoodNearestMine(mine);

            //mine = GetNearestBadMine(t.transform.position);

            //// Set the nearest mine to current tank
            //t.SetBadNearestMine(mine);

            // Think!! 
            t.Think(dt);

            // Just adjust tank position when reaching world extents
            
        }

        // Check the time to evolve
        accumTime += dt;
    }

    public bool CheckFinalTurn()
    {
        if (accumTime >= GenerationTurnDuration)
        {
            foreach (Guy t in populationGOs)
            {
                t.CalculateFinalScore();
            }
            accumTime -= GenerationTurnDuration;
            return true;
        }
        return false;
    }

    #region Helpers
    Guy CreateGuy(Genome genome, NeuralNetwork brain, int iteration, SIDE side)
    {
        Vector3 position;
        if (side == SIDE.DOWN)
        {
            position = new Vector3(iteration - MapCreator.Instance.sizeX * (iteration / MapCreator.Instance.sizeX), 1.5f, iteration / MapCreator.Instance.sizeX);
        }
        else
        {
            position = new Vector3(MapCreator.Instance.sizeX - (iteration - MapCreator.Instance.sizeX * (iteration / MapCreator.Instance.sizeX)) - 1, 1.5f, MapCreator.Instance.sizeY - (iteration / MapCreator.Instance.sizeX) - 1);
        }
        GameObject go = Instantiate<GameObject>(GuyPrefab, position, sideOfTribe == SIDE.UP ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity);
        Guy t = go.GetComponent<Guy>();
        t.SetBrain(genome, brain);
        t.generationsSurvived = genome.generationsAlive;
        return t;
    }

    void DestroyGuys()
    {
        foreach (Guy go in populationGOs)
            Destroy(go.gameObject);

        populationGOs.Clear();
        population.Clear();
        brains.Clear();

        OnPopulationDeleted?.Invoke();
    }

    GameObject GetNearestFood(Vector3 pos)
    {
        if(MapCreator.Instance.foods.Count > 0)
        {
            GameObject nearestFood = MapCreator.Instance.foods[0];

            foreach (GameObject food in MapCreator.Instance.foods)
            {
                if (Vector3.Distance(nearestFood.transform.position, pos) < Vector3.Distance(food.transform.position, pos))
                {
                    nearestFood = food;
                }
            }

            return nearestFood;
        }
        else
        {
            return null;
        }
    }

    GameObject GetNearestGuy(Vector3 pos)
    {
        if (populationGOs.Count > 0)
        {
            GameObject nearestGuy = populationGOs[0].gameObject;

            foreach (Guy guy in populationGOs)
            {
                if (Vector3.Distance(nearestGuy.transform.position, pos) < Vector3.Distance(guy.gameObject.transform.position, pos))
                {
                    nearestGuy = guy.gameObject;
                }
            }

            return nearestGuy;
        }
        else
        {
            return null;
        }
    }
    #endregion

}
