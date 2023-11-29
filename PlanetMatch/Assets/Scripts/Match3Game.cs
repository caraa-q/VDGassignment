using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Game : MonoBehaviour
{
    // Set the width and height of the grid
    public int gridWidth = 8;
    public int gridHeight = 8;

    // Array of planet prefabs to choose from
    public GameObject[] planetPrefabs;

    // 2D array to hold references to the planets in the grid
    private GameObject[,] grid;

    void Start()
    {
        // Initialise the grid
        InitialiseGrid();
        // Fill the grid with planets
        FillGrid();
    }

    void InitialiseGrid()
    {
        // Create an empty 2D array to represent the grid
        grid = new GameObject[gridWidth, gridHeight];
    }

    void FillGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Instantiate a random planet at the grid position (x, y)
                GameObject newPlanet = InstantiateRandomPlanet();

                // Set the grid position of the planet
                newPlanet.GetComponent<Planet>().SetGridPosition(x, y);

                // Assign the planet to the grid array
                grid[x, y] = newPlanet;
            }
        }
    }

    GameObject InstantiateRandomPlanet()
    {
        // Choose a random type of planet
        int randomType = Random.Range(0, planetPrefabs.Length);
        // Instantiate the chosen planet prefab
        return Instantiate(planetPrefabs[randomType], GetPositionFromGrid(), Quaternion.identity);
    }

    Vector3 GetPositionFromGrid()
    {
        // Adjust the spacing and origin based on your requirements
        float spacingX = 1.0f;
        float spacingY = 1.0f;
        Vector3 origin = Vector3.zero;

        // Calculate the position based on grid spacing and origin
        return origin + new Vector3(spacingX, spacingY, 0);
    }

    void Update()
    {
        // For testing purposes, trigger CheckForMatches on space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CheckForMatches())
            {
                // Matches found, handle accordingly
                // You may want to add delay, animations, or additional logic here
            }
        }
    }

    bool CheckForMatches()
    {
        bool matchFound = false;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth - 2; x++)
            {
                if (IsMatch(grid[x, y], grid[x + 1, y], grid[x + 2, y]))
                {
                    DestroyMatchedPlanets(x, y, 3);
                    matchFound = true;
                }
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight - 2; y++)
            {
                if (IsMatch(grid[x, y], grid[x, y + 1], grid[x, y + 2]))
                {
                    DestroyMatchedPlanets(x, y, 3);
                    matchFound = true;
                }
            }
        }

        return matchFound;
    }

    bool IsMatch(GameObject planet1, GameObject planet2, GameObject planet3)
    {
        return planet1 != null && planet2 != null && planet3 != null &&
               planet1.GetComponent<SpriteRenderer>().color == planet2.GetComponent<SpriteRenderer>().color &&
               planet2.GetComponent<SpriteRenderer>().color == planet3.GetComponent<SpriteRenderer>().color;
    }

    void DestroyMatchedPlanets(int startX, int startY, int length)
    {
        for (int i = 0; i < length; i++)
        {
            GameObject planet = grid[startX + i, startY];
            Destroy(planet);
            grid[startX + i, startY] = null;
        }

        ShiftDownAndRefill();
    }

    void ShiftDownAndRefill()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int aboveY = y + 1; aboveY < gridHeight; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            grid[x, y].GetComponent<Planet>().SetGridPosition(x, y);
                            break;
                        }
                    }

                    if (grid[x, y] == null)
                    {
                        GameObject newPlanet = InstantiateRandomPlanet();
                        newPlanet.GetComponent<Planet>().SetGridPosition(x, y);
                        grid[x, y] = newPlanet;
                    }
                }
            }
        }
    }
}

public class Planet : MonoBehaviour
{
    // Add any specific properties or methods for your planets here
    public void SetGridPosition(int x, int y)
    {
        // Set the grid position of the planet
    }
}
