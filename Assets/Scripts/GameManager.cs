using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PopulationManager tribe1;
    [SerializeField] private PopulationManager tribe2;
    bool alreadyActive;
    private ClashesManager clashesManager;
    private bool isRunning;
    public static int IterationCount = 1;

    private Coroutine coroutine;

    void Start()
    {
        alreadyActive = true;
        isRunning = true;
        clashesManager = new ClashesManager();
        coroutine = StartCoroutine(PlayTurns());
    }

    private void OnEnable()
    {
        if (alreadyActive)
        {
            isRunning = true;
            clashesManager = new ClashesManager();
            coroutine = StartCoroutine(PlayTurns());
        }
    }

    void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator PlayTurns()
    {

        while (isRunning)
        {
            tribe1.PlayTurn();
            tribe2.PlayTurn();
            //CheckGuysCollisions();
            //clashesManager.ResolveCollisions(tribe1,tribe2);
            CheckFoodToEat();
            bool hasToEnd = tribe1.CheckFinalTurn();
            tribe2.CheckFinalTurn();
            if (hasToEnd)
            {
                if (!tribe1.Epoch())
                {
                    tribe1.Respawn(tribe2);
                }
                if (!tribe2.Epoch())
                {
                    tribe2.Respawn(tribe1);
                }
            }
            yield return new WaitForSeconds(1f/ IterationCount);
        }
    }

    private void CheckGuysCollisions()
    {
        List<Guy> totalGuys = new List<Guy>();
        
        foreach(Guy g1 in tribe1.populationGOs)
        {
            totalGuys.Add(g1);
        }
        foreach (Guy g2 in tribe2.populationGOs)
        {
            totalGuys.Add(g2);
        }

        foreach (Guy g1 in totalGuys)
        {
            foreach (Guy g2 in totalGuys)
            {
                if(g1 != g2 && g1.transform.position.x == g2.transform.position.x && g1.transform.position.z == g2.transform.position.z)
                {
                    ClashesManager.RegisterCollision(g1,g2);
                }
            }
        }
    }

    private void CheckFoodToEat()
    {
        List<Guy> totalGuys = new List<Guy>();

        foreach (Guy g1 in tribe1.populationGOs)
        {
            totalGuys.Add(g1);
        }
        foreach (Guy g2 in tribe2.populationGOs)
        {
            totalGuys.Add(g2);
        }

        foreach (Guy guy in totalGuys)
        {
            if (MapCreator.Instance.IsFoodIn(guy.transform.position))
            {
                guy.OnTakeFood(MapCreator.Instance.FoodIn(guy.transform.position), false);
            }
        }
    }
}
