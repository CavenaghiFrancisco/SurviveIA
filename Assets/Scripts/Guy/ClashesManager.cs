using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CLASH_RESULT
{
    STAY,
    RUN
}

public class ClashesManager
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
                    tribe.populationGOs.Remove(g);
                    tribe.population.Remove(g.genome);
                }
                else
                {
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
        CLASH_RESULT resultGuy1 = key.Item1.TakeClashDecision(true);
        CLASH_RESULT resultGuy2 = key.Item2.TakeClashDecision(true);

        if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.STAY)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), true);
                key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), true);
            }
        }
        else if (resultGuy1 == CLASH_RESULT.RUN && resultGuy2 == CLASH_RESULT.STAY)
        {
            key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), false);
        }
        else if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.RUN)
        {
            key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item1.transform.position), false);
        }
    }

    private Guy ResolveBadCollision(Tuple<Guy, Guy> key, bool isFoodInBetween)
    {
        CLASH_RESULT resultGuy1 = key.Item1.TakeClashDecision(false);
        CLASH_RESULT resultGuy2 = key.Item2.TakeClashDecision(false);

        if (resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.STAY)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                key.Item2.OnTakeFood(MapCreator.Instance.FoodIn(key.Item2.transform.position), false);
                return key.Item1;
            }
            else
            {
                key.Item1.OnTakeFood(MapCreator.Instance.FoodIn(key.Item1.transform.position), false);
                return key.Item2;
            }
        }
        else if(resultGuy1 == CLASH_RESULT.RUN && resultGuy2 == CLASH_RESULT.STAY)
        {
            if (!isFoodInBetween && UnityEngine.Random.Range(0, 5) > 0)
            {
                key.Item1.transform.position = key.Item1.pastPos;
                return key.Item1;
            }
        }
        else if(resultGuy1 == CLASH_RESULT.STAY && resultGuy2 == CLASH_RESULT.RUN)
        {
            if (!isFoodInBetween && UnityEngine.Random.Range(0, 5) > 0)
            {
                key.Item2.transform.position += key.Item2.pastPos;
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
