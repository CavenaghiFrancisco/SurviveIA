using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PopulationManager tribe1;
    [SerializeField] private PopulationManager tribe2;
    private ClashesManager clashesManager;

    public static int IterationCount = 1;

    private Coroutine coroutine;

    void OnEnable()
    {
        clashesManager = new ClashesManager();
        coroutine = StartCoroutine(PlayTurns());
    }

    void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator PlayTurns()
    {
        while (true)
        {
            tribe1.PlayTurn();
            tribe2.PlayTurn();
            clashesManager.ResolveCollisions(tribe1,tribe2);
            tribe1.CheckFinalTurn();
            tribe2.CheckFinalTurn();
            yield return new WaitForSeconds(1f/ IterationCount);
        }
    }
}
