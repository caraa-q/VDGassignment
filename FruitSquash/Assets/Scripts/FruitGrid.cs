using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGrid : MonoBehaviour
{
    // Grid dimensions
    public int width = 6;
    public int height = 8;

    // Spacing between grid elements
    public float xSpacing;
    public float ySpacing;

    // Array of fruit prefabs to instantiate
    public GameObject[] fruitPrefab;

    // 2D array to represent the grid
    private Node[,] fruitGrid;

    // Reference to the GameObject that holds the grid
    public GameObject fruitGridGO;

    // Reference to fruit parent gameobject
    public GameObject fruitParent;

    // List to keep track of fruits that need to be destroyed
    private List<GameObject> fruitToDestroy = new ();

    // Flags to control the game flow
    [SerializeField]
    private bool isProcessingMove;

    [SerializeField]
    private Fruit selectedFruit;

    [SerializeField]
    List<Fruit> fruitsToRemove = new();

    // Audio source for match sound
    public AudioSource matchAudioSource;
    
    // Sound effect for match
    public AudioClip matchSound;

    // Reference to the array layout scriptable object
    public ArrayLayout arrayLayout;

    // Singleton instance
    public static FruitGrid Instance;

    void Awake()
    {
        Instance = this; // Set the singleton instance
    }

    void Start()
    {
        InitializeGrid(); // Called at the beginning of the game to set up the grid
    }

    private void Update()
    {
        // Check for mouse click to select a fruit
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Fruit>())
            {
                if (isProcessingMove)
                    return;

                Fruit fruit = hit.collider.gameObject.GetComponent<Fruit>();
                Debug.Log("I have clicked the fruit" + fruit.gameObject);

                SelectFruit(fruit);
            }
        }
    }

    // Initialize the grid at the start of the game
    void InitializeGrid()
    {
        DestroyFruit(); // Clear any existing fruits
        fruitGrid = new Node[width, height]; // Initialize the grid

        // Calculate spacing between grid elements
        xSpacing = (float)(width - 1) / 2;
        ySpacing = (float)((height - 1) / 2) + 1;

        // Loop through each grid position
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - xSpacing, y - ySpacing);

                // Check if the current position should be empty or have a fruit
                if (arrayLayout.rows[y].row[x])
                {
                    fruitGrid[x, y] = new Node(false, null);
                }
                else
                {
                    // Instantiate a random fruit at the current position
                    int randomIndex = Random.Range(0, fruitPrefab.Length);

                    GameObject fruit = Instantiate(fruitPrefab[randomIndex], position, Quaternion.identity);
                    fruit.transform.SetParent(fruitParent.transform);
                    fruit.GetComponent<Fruit>().SetValues(x, y);
                    fruitGrid[x, y] = new Node(true, fruit);

                    fruitToDestroy.Add(fruit);
                }
            }
        }
        
        // Check if there are initial matches and recreate the grid if needed
        if (CheckGrid())
        {
            Debug.Log("We have matches, let's re-create the grid");
            InitializeGrid();
        }
        else
        {
            Debug.Log("There are no matches, it's time to start the game!");
        }
    }

    // Destroy all fruits in the fruitToDestroy list
    private void DestroyFruit()
    {
        if (fruitToDestroy != null)
        {
            foreach (GameObject fruit in fruitToDestroy)
            {
                Destroy(fruit);
            }
            fruitToDestroy.Clear();
        }
    }

    // Check the entire grid for matches
    public bool CheckGrid()
    {
        if (GameManager.Instance.isGameEnded)
            return false;
        Debug.Log("Checking Grid");
        bool hasMatched = false;

        fruitsToRemove.Clear();

        foreach(Node nodeFruit in fruitGrid)
        {
            if (nodeFruit.fruit != null)
            {
                nodeFruit.fruit.GetComponent<Fruit>().isMatched = false;
            }
        }

        // Loop through each grid position
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Check if the fruit node is usable
                if (fruitGrid[x, y].isUsable)
                {
                    // Get the Fruit component in the current node
                    Fruit fruit = fruitGrid[x, y].fruit.GetComponent<Fruit>();

                    // Check if the fruit is not already matched
                    if (!fruit.isMatched)
                    {
                        // Check for matches and mark matched fruits
                        MatchResult matchedFruits = IsConnected(fruit);

                        if (matchedFruits.connectedFruits.Count >= 3)
                        {
                            MatchResult superMatchedFruit = SuperMatch(matchedFruits);

                            // Perform actions for matching fruits
                            fruitsToRemove.AddRange(superMatchedFruit.connectedFruits);

                            foreach (Fruit pot in superMatchedFruit.connectedFruits)
                            pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        return hasMatched;
    }
    
    public IEnumerator ProcessTurnOnMatchedGrid(bool _subtractMoves)
    {
        foreach (Fruit fruitToRemove in fruitsToRemove)
        {
            fruitToRemove.isMatched = false;
        }

        RemoveAndRefill(fruitsToRemove);
        GameManager.Instance.ProcessTurn(fruitsToRemove.Count, _subtractMoves);
        yield return new WaitForSeconds(0.4f);

        if (CheckGrid())
        {
            // Play match sound here
            if (matchAudioSource != null && matchSound != null)
            {
                matchAudioSource.PlayOneShot(matchSound);
            }

            StartCoroutine(ProcessTurnOnMatchedGrid(false));
        }
    }


    private void RemoveAndRefill(List<Fruit> _fruitsToRemove)
    {
        foreach (Fruit fruit in _fruitsToRemove)
        {
            int _xAxis = fruit.xAxis; 
            int _yAxis = fruit.yAxis;

            Destroy(fruit.gameObject);

            fruitGrid[_xAxis, _yAxis] = new Node(true, null);
        }

        for(int x=0; x < width; x++)
        {
            for (int y=0; y < height; y++)
            {
                if (fruitGrid[x, y].fruit == null)
                {
                    RefillFruit(x, y);
                }
            }
        }
    }

    private void RefillFruit(int x, int y)
    {
        int yOffset = 1;

        // If inside the grid and the cell is null, increment offset
        while (y + yOffset < height && fruitGrid[x, y + yOffset].fruit == null)
        {
            yOffset++;
        }
        
        if (y + yOffset < height && fruitGrid[x, y + yOffset].fruit != null)
        {
            Fruit fruitAbove = fruitGrid[x, y + yOffset].fruit.GetComponent<Fruit>();

            Vector3 targetPosition = new Vector3(x - xSpacing, y - ySpacing, fruitAbove.transform.position.z);
            
            fruitAbove.MoveToTarget(targetPosition);
            fruitAbove.SetValues(x,y);

            fruitGrid[x, y] = fruitGrid[x, y + yOffset];
            fruitGrid[x, y + yOffset] = new Node(true, null);
        }
        
        if (y + yOffset == height)
        {
            SpawnFruitAtTop(x);
        }
    }

    private void SpawnFruitAtTop(int x)
    {
        int index = FindLowestNull(x);
        int moveTo = 8 - index;
        int randomIndex = Random.Range(0, fruitPrefab.Length);
        GameObject newFruit = Instantiate(fruitPrefab[randomIndex], new Vector2(x - xSpacing, height - ySpacing), Quaternion.identity);
        
        newFruit.transform.SetParent(fruitParent.transform);
        newFruit.GetComponent<Fruit>().SetValues(x, index);
        fruitGrid[x, index] = new Node(true, newFruit);

        Vector3 targetPosition = new Vector3(newFruit.transform.position.x, newFruit.transform.position.y - moveTo, newFruit.transform.position.z);
        newFruit.GetComponent<Fruit>().MoveToTarget(targetPosition);

    }

    private int FindLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 7; y>= 0; y--)
        {
            if (fruitGrid[x,y].fruit == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #region Cascading Fruits

    //

    #endregion  

    #region MatchingLogic

    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            foreach (Fruit pot in _matchedResults.connectedFruits)
            {
                List<Fruit> extraConnectedFruits = new ();

                // Up and down
                CheckDirection(pot, new Vector2Int(0, 1), extraConnectedFruits);
                CheckDirection(pot, new Vector2Int(0, -1), extraConnectedFruits);

                if (extraConnectedFruits.Count >= 2)
                {
                    extraConnectedFruits.AddRange(_matchedResults.connectedFruits);

                    return new MatchResult
                    {
                        connectedFruits = extraConnectedFruits,
                        direction = MatchDirection.Super
                    };
                }
            }
            
            return new MatchResult
            {
                connectedFruits = _matchedResults.connectedFruits, 
                direction = _matchedResults.direction
            };
        }
        else if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Fruit pot in _matchedResults.connectedFruits)
            {
                List<Fruit> extraConnectedFruits = new ();

                CheckDirection(pot, new Vector2Int(1, 0), extraConnectedFruits);
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnectedFruits);

                if (extraConnectedFruits.Count >= 2)
                {
                    extraConnectedFruits.AddRange(_matchedResults.connectedFruits);

                    return new MatchResult
                    {
                        connectedFruits = extraConnectedFruits,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedFruits = _matchedResults.connectedFruits, 
                direction = _matchedResults.direction
            }; 
        }
        return null;
    }

    // Check for connected fruits in different directions from a given fruit
    MatchResult IsConnected(Fruit fruit)
    {
        List<Fruit> connectedFruits = new ();
        FruitType fruitType = fruit.fruitType;

        connectedFruits.Add(fruit);

        // Check right and left directions for horizontal matches
        CheckDirection(fruit, new Vector2Int(1, 0), connectedFruits);
        CheckDirection(fruit, new Vector2Int(-1, 0), connectedFruits);

        // Check for horizontal matches
        if (connectedFruits.Count == 3)
        {
            Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.Horizontal
            };
        }
        // Check for long horizontal matches
        else if (connectedFruits.Count > 3)
        {
            Debug.Log("I have a Long horizontal match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.LongHorizontal
            };
        }

        // Clear out the connected fruits list
        connectedFruits.Clear();
        // Re-add the initial fruit to the list
        connectedFruits.Add(fruit);

        // Check up and down directions for vertical matches
        CheckDirection(fruit, new Vector2Int(0, 1), connectedFruits);
        CheckDirection(fruit, new Vector2Int(0, -1), connectedFruits);

        // Check for vertical matches
        if (connectedFruits.Count == 3)
        {
            Debug.Log("I have a normal vertical match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.Vertical
            };
        }
        // Check for long vertical matches
        else if (connectedFruits.Count > 3)
        {
            Debug.Log("I have a Long vertical match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.None
            };
        }
    }

    // Check for connected fruits in a given direction
    void CheckDirection(Fruit pot, Vector2Int direction, List<Fruit> connectedFruits)
    {
        FruitType fruitType = pot.fruitType;
        int x = pot.xAxis + direction.x;
        int y = pot.yAxis + direction.y;

        // Check that we're within the boundaries of the grid
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (fruitGrid[x, y].isUsable)
            {
                Fruit neighbourFruit = fruitGrid[x, y].fruit.GetComponent<Fruit>();

                // Make sure it is not matched
                if (!neighbourFruit.isMatched && neighbourFruit.fruitType == fruitType)
                {
                    connectedFruits.Add(neighbourFruit);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
    #endregion

    #region  Swapping Fruits

    // Select a fruit for swapping
    public void SelectFruit(Fruit _fruit)
    {
        if (selectedFruit == null)
        {
            selectedFruit = _fruit;
        }
        else if (selectedFruit == _fruit)
        {
            selectedFruit = null;
        }
        else if (selectedFruit != _fruit)
        {
            SwapFruit(selectedFruit, _fruit);
            selectedFruit = null;
        }
    }

    // Swap two selected fruits
    private void SwapFruit(Fruit _currentFruit, Fruit _targetFruit)
    {
        if (!IsAdjacent(_currentFruit, _targetFruit))
        {
            return;
        }

        DoSwap(_currentFruit, _targetFruit);

        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentFruit, _targetFruit));
    }

    // Perform the actual swap of fruits
    private void DoSwap(Fruit _currentFruit, Fruit _targetFruit)
    {
        GameObject temp = fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit;

        fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit = fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit;
        fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit = temp;

        int tempXAxis = _currentFruit.xAxis;
        int tempYAxis = _currentFruit.yAxis;

        _currentFruit.xAxis = _targetFruit.xAxis;
        _currentFruit.yAxis = _targetFruit.yAxis;
        _targetFruit.xAxis = tempXAxis;
        _targetFruit.yAxis = tempYAxis;

        // Move current fruit to target fruit (physically on the screen)
        _currentFruit.MoveToTarget(fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit.transform.position);

        // Move target fruit to current fruit (physically on the screen)
        _targetFruit.MoveToTarget(fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit.transform.position);
    }

    // Process matches after a swap
    private IEnumerator ProcessMatches(Fruit _currentFruit, Fruit _targetFruit)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckGrid())
        {
            StartCoroutine(ProcessTurnOnMatchedGrid(true));
        }
        else
        {
            DoSwap(_currentFruit, _targetFruit);
        }
        isProcessingMove = false;
    }

    // Check if two fruits are adjacent
    private bool IsAdjacent(Fruit _currentFruit, Fruit _targetFruit)
    {
        return Mathf.Abs(_currentFruit.xAxis - _targetFruit.xAxis) + Mathf.Abs(_currentFruit.yAxis - _targetFruit.yAxis) == 1;
    }

    #endregion

    // Enumeration for the direction of a match
    public enum MatchDirection
    {
        Vertical,
        Horizontal,
        LongVertical,
        LongHorizontal,
        Super,
        None
    }

    // Class to store information about a match
    public class MatchResult
    {
        public List<Fruit> connectedFruits;
        public MatchDirection direction;
    }
}