using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializator : MonoBehaviour
{
    public struct GenomeSerial
    {
        public float[] gens;
        public int generations;
    }

    public struct PopulationSerial
    {
        public int generation;
        public float mutationChance;
        public float mutationRate;

        public int inputsCount;
        public int hiddenLayers;
        public int outputsCount;
        public int neuronsCountPerHL;
        public float bias;
        public float p;
    }

    public static string SerializeGenomes(List<Genome> genomes, string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "Assets/Resources/GenomesData/GenomesData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        }
        else
        {
            if (!fileName.ToLower().EndsWith(".json"))
            {
                fileName += ".json";
            }

            int counter = 1;
            string originalFileName = "Assets/Resources/GenomesData" + fileName;
            while (System.IO.File.Exists(fileName))
            {
                fileName = originalFileName.Insert(originalFileName.LastIndexOf(".json"), "_" + counter);
                counter++;
            }
        }
        List<GenomeSerial> serializedGenomes = new List<GenomeSerial>();

        foreach (Genome g in genomes)
        {
            GenomeSerial serializedGenome = new GenomeSerial
            {
                gens = g.genome,
                generations = g.generationsAlive
            };

            serializedGenomes.Add(serializedGenome);
        }

        string json = JsonConvert.SerializeObject(serializedGenomes, Formatting.Indented);

        System.IO.File.WriteAllText(fileName, json);

        return json;
    }

    public static List<Genome> DeserializeGenomes(string json)
    {
        List<GenomeSerial> serializedGenomes = JsonConvert.DeserializeObject<List<GenomeSerial>>(json);

        List<Genome> genomes = new List<Genome>();

        foreach (GenomeSerial serializedGenome in serializedGenomes)
        {
            Genome genome = new Genome
            {
                genome = serializedGenome.gens,
                generationsAlive = serializedGenome.generations
            };

            genomes.Add(genome);
        }

        return genomes;
    }


    public static string SerializePopulation(PopulationManager population, string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "Assets/Resources/PopulationsData/PopulationData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        }
        else
        {
            if (!fileName.ToLower().EndsWith(".json"))
            {
                fileName += ".json";
            }

            int counter = 1;
            string originalFileName = "Assets/Resources/PopulationsData" + fileName;
            while (System.IO.File.Exists(fileName))
            {
                fileName = originalFileName.Insert(originalFileName.LastIndexOf(".json"), "_" + counter);
                counter++;
            }
        }

        PopulationSerial serializedPopulation = new PopulationSerial
        {
            generation = population.generation,
            mutationChance = population.MutationChance,
            mutationRate = population.MutationRate,

            inputsCount = population.InputsCount,
            hiddenLayers = population.HiddenLayers,
            outputsCount = population.OutputsCount,
            neuronsCountPerHL = population.NeuronsCountPerHL,
            bias = population.Bias,
            p = population.P
        };

        string json = JsonConvert.SerializeObject(serializedPopulation, Formatting.Indented);

        System.IO.File.WriteAllText(fileName, json);

        return json;
    }

    public static PopulationSerial DeserializePopulation(string json)
    {
        PopulationSerial serializedPopulation = JsonConvert.DeserializeObject<PopulationSerial>(json);

        return serializedPopulation;
    }

}
