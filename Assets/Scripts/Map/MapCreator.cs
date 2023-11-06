using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    static MapCreator instance = null;

    public GameObject FoodPrefab;
    public GameObject tilePrefab;

    public int sizeX = 100;
    public int sizeY = 100;
    public int FoodCount = 50;


    public static MapCreator Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MapCreator>();

            return instance;
        }
    }

    void Start()
    {
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                Instantiate(tilePrefab, new Vector3(j, 0, i), Quaternion.identity);
            }
        }
    }

    void CreateFood()
    {
        
    }
}
