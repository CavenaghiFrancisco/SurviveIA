using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    static MapCreator instance = null;

    public GameObject FoodPrefab;
    public GameObject tilePrefab;

    private int createTimesCalled = 0;
    private int deleteTimesCalled = 0;

    public PopulationManager tribe1;
    public PopulationManager tribe2;

    public int sizeX = 100;
    public int sizeY = 100;

    private List<Vector3> originalTiles = new List<Vector3>();
    private List<Vector3> tiles = new List<Vector3>();

    public List<GameObject> foods = new List<GameObject>();

    public static MapCreator Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MapCreator>();

            return instance;
        }
    }

    private void Awake()
    {
        tribe1.OnPopulationDeleted += ClearFood;
        tribe2.OnPopulationDeleted += ClearFood;
    }

    void Start()
    {
        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                Vector3 pos = new Vector3(j, 0, i);
                Instantiate(tilePrefab, pos, Quaternion.identity);
                originalTiles.Add(pos + Vector3.up);
            }
        }
        PopulationManager.OnPopulationCreated += CreateFood;
    }

    void CreateFood(int populationCount, List<Vector3> positionsUsed)
    {
        tiles = originalTiles;
        for (int i = 0; i < populationCount; i++)
        {
            if(positionsUsed.Count > 0)
            {
                tiles.Remove(positionsUsed[i]);
            }
        }
        createTimesCalled++;
        if(createTimesCalled == 2)
        {
            for (int i = 0; i < populationCount; i++)
            {
                if (tiles.Count <= 0)
                {
                    break;
                }
                int random = Random.Range(0, tiles.Count - 1);
                foods.Add(Instantiate(FoodPrefab, tiles[random], Quaternion.identity));
                tiles.Remove(tiles[random]);
            }
            createTimesCalled = 0;
        }
    }

    private void ClearFood()
    {
        deleteTimesCalled++;
        if (deleteTimesCalled == 2)
        {
            foreach (GameObject go in foods)
                Destroy(go.gameObject);

            foods.Clear();
            deleteTimesCalled = 0;
        }
            
    }
}
