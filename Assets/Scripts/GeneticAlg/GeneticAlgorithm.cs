using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Genome
{
    public float[] genome;
    public float fitness = 0;
    public int generationsAlive = 0;
    public bool ableToLive;
    public bool ableToReproduce;


    public Genome(float[] genes)
    {
        this.genome = genes;
        fitness = 0;
    }

    public Genome(int genesCount)
    {
        genome = new float[genesCount];

        for (int j = 0; j < genesCount; j++)
            genome[j] = Random.Range(-1.0f, 1.0f);

        fitness = 0;
        ableToLive = false;
        ableToReproduce = false;
    }

    public Genome(float[] genes,int generations)
    {
        this.genome = genes;
        generationsAlive = generations;
        fitness = 0;
        ableToLive = false;
        ableToReproduce = false;
    }

    public Genome()
    {
        fitness = 0;
        ableToLive = false;
        ableToReproduce = false;
    }

}

public class GeneticAlgorithm
{
    List<Genome> population = new List<Genome>();
    List<Genome> populationToReproduce = new List<Genome>();
    List<Genome> newPopulation = new List<Genome>();

    private const float RANDOM_MUTATION_CHANCE = 0.25f;

    float totalFitness;

    float mutationChance = 0.0f;
    float mutationRate = 0.0f;

    public GeneticAlgorithm(float mutationChance, float mutationRate)
    {
        this.mutationChance = mutationChance;
        this.mutationRate = mutationRate;
    }

    public Genome[] GetRandomGenomes(int count, int genesCount)
    {
        Genome[] genomes = new Genome[count];

        for (int i = 0; i < count; i++)
        {
            genomes[i] = new Genome(genesCount);
        }

        return genomes;
    }


    public Genome[] Epoch(Genome[] oldGenomes, bool random = false)
    {
        totalFitness = 0;

        population.Clear();
        populationToReproduce.Clear();
        newPopulation.Clear();

        population.AddRange(oldGenomes);

        SelectReproductiveElite(random);

        while (newPopulation.Count < (populationToReproduce.Count % 2 == 0 ? populationToReproduce.Count : populationToReproduce.Count - 1))
        {
            Crossover(random);
        }

        SelectElite();


        return newPopulation.ToArray();
    }

    void SelectReproductiveElite(bool random = false)
    {
        if (random)
        {
            foreach (Genome genome in population)
            {
                populationToReproduce.Add(genome);
            }
        }
        else
        {
            foreach (Genome genome in population)
            {
                if (genome.ableToReproduce)
                {
                    populationToReproduce.Add(genome);
                }

                totalFitness += genome.fitness;
            }
            populationToReproduce = populationToReproduce.OrderByDescending(elemento => elemento.fitness).ToList();
        }
    }

    void SelectElite()
    {
        foreach (Genome genome in population)
        {
            if (genome.ableToLive)
            {
                newPopulation.Add(genome);
            }
        }
    }

    void Crossover(bool random)
    {
        Genome mom;
        Genome dad;
        if (random)
        {
            mom = populationToReproduce[Random.Range(0, populationToReproduce.Count)];
            dad = populationToReproduce[Random.Range(0, populationToReproduce.Count)];
        }
        else
        {
            do
            {
                mom = RouletteSelection();
                dad = RouletteSelection();
                if (mom == null || dad == null)
                {
                    return;
                }
            }
            while (mom == dad);
        }
        

        Genome child1;
        Genome child2;

        Crossover(mom, dad, out child1, out child2, random);

        newPopulation.Add(child1);
        newPopulation.Add(child2);
    }

    void Crossover(Genome mom, Genome dad, out Genome child1, out Genome child2, bool random = false)
    {
        child1 = new Genome();
        child2 = new Genome();

        child1.genome = new float[mom.genome.Length];
        child2.genome = new float[mom.genome.Length];

        int pivot = Random.Range(0, mom.genome.Length);

        for (int i = 0; i < pivot; i++)
        {
            child1.genome[i] = mom.genome[i];

            if (ShouldMutate(random))
                child1.genome[i] += Random.Range(-mutationRate, mutationRate);

            child2.genome[i] = dad.genome[i];

            if (ShouldMutate(random))
                child2.genome[i] += Random.Range(-mutationRate, mutationRate);
        }

        for (int i = pivot; i < mom.genome.Length; i++)
        {
            child2.genome[i] = mom.genome[i];

            if (ShouldMutate(random))
                child2.genome[i] += Random.Range(-mutationRate, mutationRate);

            child1.genome[i] = dad.genome[i];

            if (ShouldMutate(random))
                child1.genome[i] += Random.Range(-mutationRate, mutationRate);
        }
    }

    bool ShouldMutate(bool random = false)
    {
        return Random.Range(0.0f, 1.0f) < (random ? mutationChance + RANDOM_MUTATION_CHANCE : mutationChance);
    }



    public Genome RouletteSelection()
    {
        Genome g = null;
        float value = Random.Range(0, totalFitness);
        float currentValue = 0;
        int aux = 0;
        foreach (Genome gen in populationToReproduce)
        {
            currentValue += gen.fitness;
            if (value <= currentValue)
            {
                g = populationToReproduce[aux];
                return g;
            }
            aux++;
        }

        return g;
    }

}
