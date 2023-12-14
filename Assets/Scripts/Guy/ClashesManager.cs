using System;
using System.Collections.Generic;
using UnityEngine;

public enum CLASH_RESULT
{
    STAY,
    RUN
}

public class ClashesManager : MonoBehaviour
{
    static Dictionary<Tuple<Guy, Guy>, bool> collisionsToResolve = new Dictionary<Tuple<Guy, Guy>, bool>();

    public static void RegisterCollision(Guy guy1, Guy guy2)
    {
        Tuple<Guy, Guy> guysCollision = new Tuple<Guy, Guy>(guy1, guy2);
        Tuple<Guy, Guy> guysCollision2 = new Tuple<Guy, Guy>(guy2, guy1);
        if (MapCreator.Instance.IsFoodIn(guy1.transform.position))
        {
            if (!collisionsToResolve.ContainsKey(guysCollision) && !collisionsToResolve.ContainsKey(guysCollision2))
            {
                collisionsToResolve.Add(guysCollision, true);
            }
        }
        else
        {
            if (!collisionsToResolve.ContainsKey(guysCollision) && !collisionsToResolve.ContainsKey(guysCollision2))
            {
                collisionsToResolve.Add(guysCollision, false);
            }
        }
    }

    public void ResolveCollisions(PopulationManager tribe, PopulationManager tribe2)
    {
        List<Guy> guysToDelete = new List<Guy>();
        foreach (KeyValuePair<Tuple<Guy, Guy>, bool> collision in collisionsToResolve)
        {
            if (IsGoodCollision(tribe, collision.Key))
            {
                ResolveGoodCollision(collision.Key, collision.Value);
            }
            else
            {
                guysToDelete.Add(ResolveBadCollision(collision.Key, collision.Value));
            }
        }
        foreach (Guy g in guysToDelete)
        {
            if(g != null)
            {
                if (tribe.populationGOs.Contains(g))
                {
                    Destroy(g.gameObject);
                    tribe.populationGOs.Remove(g);
                    tribe.population.Remove(g.genome);
                }
                else
                {
                    Destroy(g.gameObject);
                    tribe2.populationGOs.Remove(g);
                    tribe2.population.Remove(g.genome);
                }
            }
        }
        guysToDelete.Clear();
        collisionsToResolve.Clear();
    }

    private void ResolveGoodCollision(Tuple<Guy, Guy> key, bool isFoodInBetween)
    {
        CLASH_RESULT resultGuy1 = key.Item1.TakeClashDecision();
        CLASH_RESULT resultGuy2 = key.Item2.TakeClashDecision();

        if (isFoodInBetween)
        {
            if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.STAY)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), true);
                    key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), true);
                }
                if(key.Item1.foodTaken == 0 && key.Item2.foodTaken == 0)
                {
                    key.Item1.genome.fitness += 500;
                    key.Item2.genome.fitness += 500;
                }
            }
            else if (resultGuy1 == CLASH_RESULT.RUN && resultGuy2 == CLASH_RESULT.STAY)
            {
                key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), false);
                if (key.Item1.foodTaken >= 2 && key.Item2.foodTaken <= 1)
                {
                    key.Item1.genome.fitness += 500;
                    key.Item2.genome.fitness += 500;
                }
            }
            else if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.RUN)
            {
                key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item1.transform.position), false);
                if (key.Item2.foodTaken >= 2 && key.Item1.foodTaken <= 1)
                {
                    key.Item1.genome.fitness += 500;
                    key.Item2.genome.fitness += 500;
                }
            }
        }

        
    }

    private Guy ResolveBadCollision(Tuple<Guy, Guy> key, bool isFoodInBetween)
    {
        CLASH_RESULT resultGuy1 = key.Item1.TakeClashDecision();
        CLASH_RESULT resultGuy2 = key.Item2.TakeClashDecision();

        if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.STAY)
        {
            if (isFoodInBetween)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), false);
                    if(key.Item2.genome.generationsAlive <= 2 && key.Item2.foodTaken == 1 || key.Item1.genome.generationsAlive <= 2)
                    {
                        key.Item2.genome.fitness += 500;
                    }
                    return key.Item1;
                }
                else
                {
                    key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item1.transform.position), false);
                    if (key.Item1.genome.generationsAlive <= 2 && key.Item1.foodTaken == 1 || key.Item2.genome.generationsAlive <= 2)
                    {
                        key.Item1.genome.fitness += 500;
                    }
                    return key.Item2;
                }
            }
            else
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    if(key.Item2.genome.generationsAlive < key.Item1.genome.generationsAlive && key.Item2.genome.fitness < key.Item1.genome.fitness)
                    {
                        key.Item2.genome.fitness += 500;
                    }
                    return key.Item1;
                }
                else
                {
                    if (key.Item1.genome.generationsAlive < key.Item2.genome.generationsAlive && key.Item1.genome.fitness < key.Item2.genome.fitness)
                    {
                        key.Item1.genome.fitness += 500;
                    }
                    return key.Item2;
                }
            }
        }
        else if(resultGuy1 == CLASH_RESULT.RUN && resultGuy2 == CLASH_RESULT.STAY)
        {
            key.Item1.transform.position = key.Item1.pastPos;
            if (key.Item1.genome.generationsAlive < key.Item2.genome.generationsAlive && key.Item1.genome.fitness > key.Item2.genome.fitness && key.Item1.foodTaken >= 2)
            {
                key.Item1.genome.fitness += 500;
                key.Item2.genome.fitness += 500;
            }
            if (!isFoodInBetween && UnityEngine.Random.Range(0, 5) > 0)
            {
                return key.Item1;
            }
        }
        else if(resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.RUN)
        {
            key.Item2.transform.position = key.Item2.pastPos;
            if (key.Item2.genome.generationsAlive < key.Item1.genome.generationsAlive && key.Item2.genome.fitness > key.Item1.genome.fitness && key.Item2.foodTaken >= 2)
            {
                key.Item1.genome.fitness += 500;
                key.Item2.genome.fitness += 500;
            }
            if (!isFoodInBetween && UnityEngine.Random.Range(0, 5) > 0)
            {
                return key.Item2;
            }
        }

        return null;
    }

    private bool IsGoodCollision(PopulationManager tribe, Tuple<Guy, Guy> guys)
    {
        bool guy1isInTribe = tribe.populationGOs.Contains(guys.Item1);
        bool guy2isInTribe = tribe.populationGOs.Contains(guys.Item2);
        return ((guy1isInTribe && guy2isInTribe) || (!guy1isInTribe && !guy2isInTribe));

    }
}
