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

    private int populationOriginal = 0;

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

    private void OnDestroy()
    {
        tribe1.OnPopulationDeleted -= ClearFood;
        tribe2.OnPopulationDeleted -= ClearFood;
        PopulationManager.OnPopulationCreated -= CreateFood;
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
        populationOriginal += populationCount;
        if(createTimesCalled == 0)
        {
            foreach (Vector3 pos in originalTiles)
            {
                tiles.Add(pos);
            }
        }
        for (int i = 0; i < positionsUsed.Count; i++)
        {
            tiles.Remove(positionsUsed[i] - Vector3.up/2);
        }
        createTimesCalled++;
        if(createTimesCalled == 2)
        {
            for (int i = 0; i < populationOriginal; i++)
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
            populationOriginal = 0;
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

    public GameObject FoodIn(Vector3 pos)
    {
        foreach (GameObject go in foods)
        {
            if (go.transform.position.x == pos.x && go.transform.position.z == pos.z)
            {
                return go;
            }
        }
        return null;
    }

    public bool IsFoodIn(Vector3 pos)
    {
        foreach(GameObject go in foods)
        {
            if(go.transform.position.x == pos.x && go.transform.position.z == pos.z && go.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
