using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGrid : MonoBehaviour
{
    public int width = 6; // Grid width
    public int height = 8; // Grid height 
    public float xSpacing;// Grid spacing
    public float ySpacing;
    public GameObject[] fruitPrefab;// Reference to fruit
    private Node[,] fruitGrid;
    public GameObject fruitBoardGO;
    public ArrayLayout arrayLayout;
    public static FruitGrid Instance;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeGrid(); // Called at the beginning of the game
    }

    void InitializeGrid()
    {
        fruitGrid = new Node[width, height];

        xSpacing = (float)(width - 1) / 2;
        ySpacing = (float)((height -1) / 2) + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - xSpacing, y - xSpacing);

                int randomIndex = Random.Range(0, fruitPrefab.Length);

                GameObject fruit = Instantiate(fruitPrefab[randomIndex], position, Quaternion.identity);
                fruit.GetComponent<Fruit>().SetValues(x, y);
                fruitGrid[x,y] = new Node(true, fruit);
            }
        }
    }

}
