using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGrid : MonoBehaviour
{
    public int width = 6; // Grid width
    public int height = 8; // Grid height
    public float xSpacing; // Grid spacing
    public float ySpacing;

    public GameObject[] fruitPrefab; // Reference to fruit
    private Node[,] fruitGrid;

    public GameObject fruitGridGO;
    private List<GameObject> fruitToDestroy = new List<GameObject>();

    [SerializeField]
    private bool isProcessingMove;
    [SerializeField]
    private Fruit selectedFruit;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Fruit>())
            {
                if (isProcessingMove)
                    return;

                Fruit fruit = hit.collider.gameObject.GetComponent<Fruit>();
                Debug.Log("I have hit a fruit it is : " + fruit.gameObject);

                SelectFruit(fruit);
            }
        }
    }

    void InitializeGrid()
    {
        DestroyFruit();
        fruitGrid = new Node[width, height];

        xSpacing = (float)(width - 1) / 2;
        ySpacing = (float)((height - 1) / 2) + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - xSpacing, y - ySpacing);
                if (arrayLayout.rows[y].row[x])
                {
                    fruitGrid[x, y] = new Node(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, fruitPrefab.Length);

                    GameObject fruit = Instantiate(fruitPrefab[randomIndex], position, Quaternion.identity);
                    fruit.GetComponent<Fruit>().SetValues(x, y);
                    fruitGrid[x, y] = new Node(true, fruit);
                    fruitToDestroy.Add(fruit);
                }
            }
        }

        if (CheckGrid())
        {
            Debug.Log("We have matches let's re-create the grid");
            InitializeGrid();
        }
        else
        {
            Debug.Log("There are no matches, it's time to start the game!");
        }
    }

    private void DestroyFruit()
    {
        if (fruitToDestroy != null)
        {
            foreach (GameObject fruit in fruitToDestroy)
                Destroy(fruit);
            fruitToDestroy.Clear();
        }
    }

    public bool CheckGrid()
    {
        Debug.Log("Checking Grid");
        bool hasMatched = false;

        List<Fruit> fruitsToRemove = new List<Fruit>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // checking if fruit node is usable
                if (fruitGrid[x, y].isUsable)
                {
                    // then proceed to get fruit class in node
                    Fruit fruit = fruitGrid[x, y].fruit.GetComponent<Fruit>();

                    // ensure it's not matched
                    if (!fruit.isMatched)
                    {
                        // run some matching logic
                        MatchResult matchedFruits = IsConnected(fruit);

                        if (matchedFruits.connectedFruits.Count >= 3)
                        {
                            // complex matching...

                            fruitsToRemove.AddRange(matchedFruits.connectedFruits);

                            foreach (Fruit pot in matchedFruits.connectedFruits)
                                pot.isMatched = true;

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        return hasMatched;
    }

    MatchResult IsConnected(Fruit fruit)
    {
        List<Fruit> connectedFruits = new List<Fruit>();
        FruitType fruitType = fruit.fruitType;

        connectedFruits.Add(fruit);

        // check right
        CheckDirection(fruit, new Vector2Int(1, 0), connectedFruits);
        // check left
        CheckDirection(fruit, new Vector2Int(-1, 0), connectedFruits);

        // have we made a 3 match? (Horizontal Match)
        if (connectedFruits.Count == 3)
        {
            Debug.Log("I have a normal horizontal match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.Horizontal
            };
        }
        // checking for more than 3 (Long horizontal Match)
        else if (connectedFruits.Count > 3)
        {
            Debug.Log("I have a Long horizontal match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.LongHorizontal
            };
        }

        // clear out the connectedfruits
        connectedFruits.Clear();
        // readd our initial fruit
        connectedFruits.Add(fruit);

        // check up
        CheckDirection(fruit, new Vector2Int(0, 1), connectedFruits);
        // check down
        CheckDirection(fruit, new Vector2Int(0, -1), connectedFruits);

        // have we made a 3 match? (Vertical Match)
        if (connectedFruits.Count == 3)
        {
            Debug.Log("I have a normal vertical match, the color of my match is: " + connectedFruits[0].fruitType);

            return new MatchResult
            {
                connectedFruits = connectedFruits,
                direction = MatchDirection.Vertical
            };
        }
        // checking for more than 3 (Long Vertical Match)
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

    void CheckDirection(Fruit pot, Vector2Int direction, List<Fruit> connectedFruits)
    {
        FruitType fruitType = pot.fruitType;
        int x = pot.xAxis + direction.x;
        int y = pot.yAxis + direction.y;

        // check that we're within the boundaries of the grid
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (fruitGrid[x, y].isUsable)
            {
                Fruit neighbourFruit = fruitGrid[x, y].fruit.GetComponent<Fruit>();

                // does our fruit Type Match? it must also not be matched
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

    #region SwappingLogic

    public void SelectFruit(Fruit fruit)
    {
        if (isProcessingMove)
        {
            return;
        }

        if (selectedFruit == null)
        {
            selectedFruit = fruit;
        }
        else if (selectedFruit == fruit)
        {
            selectedFruit = null;
        }
        else if (selectedFruit != fruit)
        {
            SwapFruit(selectedFruit, fruit);
            selectedFruit = null;
        }
    }

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

    private void DoSwap(Fruit _currentFruit, Fruit _targetFruit)
    {
        GameObject temp = fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit;

        fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit = fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit;
        fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit = temp;

        int tempxAxis = _currentFruit.xAxis;
        int tempyAxis = _currentFruit.yAxis;

        _currentFruit.xAxis = _targetFruit.xAxis;
        _currentFruit.yAxis = _targetFruit.yAxis;
        _targetFruit.xAxis = tempxAxis;
        _targetFruit.yAxis = tempyAxis;

        // moves current fruit to target fruit (physically on the screen)
        _currentFruit.MoveToTarget(fruitGrid[_targetFruit.xAxis, _targetFruit.yAxis].fruit.transform.position);

        // moves target fruit to current fruit (physically on the screen)
        _targetFruit.MoveToTarget(fruitGrid[_currentFruit.xAxis, _currentFruit.yAxis].fruit.transform.position);
    }

    private bool IsAdjacent(Fruit _currentFruit, Fruit _targetFruit)
    {
        return Mathf.Abs(_currentFruit.xAxis - _targetFruit.xAxis) + Mathf.Abs(_currentFruit.yAxis - _targetFruit.yAxis) == 1;
    }

    private IEnumerator ProcessMatches(Fruit _currentFruit, Fruit _targetFruit)
    {
        yield return new WaitForSeconds(0.2f);

        bool hasMatch = CheckGrid();

        if (!hasMatch)
            DoSwap(_currentFruit, _targetFruit);

        isProcessingMove = false;
    }

    #endregion

    public class MatchResult
    {
        public List<Fruit> connectedFruits;
        public MatchDirection direction;
    }

    public enum MatchDirection
    {
        Vertical,
        Horizontal,
        LongVertical,
        LongHorizontal,
        Super,
        None
    }
}
