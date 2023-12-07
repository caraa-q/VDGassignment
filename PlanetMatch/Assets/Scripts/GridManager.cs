using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();   // List of sprites to be used for tiles
    public GameObject TilePrefab;   // Prefab for individual tiles
    public int GridDimension = 12;   // Dimension of the grid 
    public float Distance = 1.0f;   // Distance between tiles
    private GameObject[,] Grid;     // 2D array to represent the grid


    // Singleton instance of the GridManager
    public static GridManager Instance 
    { 
        get; private set; 
    } 
    
    // Awake is called before the Start method
    void Awake() 
    { 
        Instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the grid
        Grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
    }

    // Initialize the grid with tiles and sprites
    void InitGrid()
    {
        // Calculate the position offset to center the grid
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0);

        // Loop through each row and column to create tiles
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                // Create a list of possible sprites for the current tile
                List<Sprite> possibleSprites = new List<Sprite>(Sprites);

                // Check and remove sprites based on matching criteria
                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                // Instantiate a new tile
                GameObject newTile = Instantiate(TilePrefab);
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();

                // Set the sprite for the tile randomly from the possible sprites
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

                // Add a Tile component to the new tile
                Tile tile = newTile.AddComponent<Tile>();
                tile.Position = new Vector2Int(column, row);
                newTile.transform.parent = transform;

                // Set the sprite for the tile again (duplicate line)
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

                // Set the position of the tile in the grid
                newTile.transform.parent = transform;
                newTile.transform.position = new Vector3(column * Distance, row * Distance, 0) + positionOffset;

                // Store the tile in the grid array
                Grid[column, row] = newTile;
            }
        }
    }

    // Get the sprite at a specific grid position
    Sprite GetSpriteAt(int column, int row)
    {
        // Check if the position is outside the grid boundaries
        if (column < 0 || column >= GridDimension || row < 0 || row >= GridDimension)
            return null;

        // Get the tile at the specified position
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

        // Return the sprite if the renderer is not null, otherwise return null
        return renderer != null ? renderer.sprite : null;
    }

    // Swap the sprites of two tiles
    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        // Get the GameObjects and SpriteRenderers of the two tiles
        GameObject tile1 = Grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = Grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        // Swap the sprites
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        // Check for matches
        bool changesOccurs = CheckMatches();

        // If no matches, swap the sprites back
        if(!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        // If matches occurred, fill holes and check for more matches
        else
        {
            do
            {
                FillHoles();
            } while (CheckMatches());
        }
    }

    // Get the SpriteRenderer component at a specific grid position
    SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        // Check if the position is outside the grid boundaries
        if (column < 0 || column >= GridDimension || row < 0 || row >= GridDimension)
            return null;

        // Get the tile at the specified position
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

        // Return the SpriteRenderer component
        return renderer;
    }

    // Check for matching tiles and remove them
    bool CheckMatches()
    {
        // HashSet to store matched tiles
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>();

        // Loop through each row and column
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                // Get the SpriteRenderer of the current tile
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                // Check for horizontal matches
                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }

                // Check for vertical matches
                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }

        // Remove matched tiles
        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }

        // Generate new tiles after removing matches
        GenerateNewTiles();

        // Return true if matches were found
        return matchedTiles.Count > 0;
    }

    // Find horizontally matching tiles for a given tile
    List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        // List to store matching tiles
        List<SpriteRenderer> result = new List<SpriteRenderer>();

        // Loop through columns to the right
        for (int i = col + 1; i < GridDimension; i++)
        {
            // Get the SpriteRenderer of the next column
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);

            // If the sprite is different, break the loop
            if (nextColumn.sprite != sprite)
            {
                break;
            }

            // Add the matching tile to the list
            result.Add(nextColumn);
        }

        // Return the list of matching tiles
        return result;
    }

    // Find vertically matching tiles for a given tile
    List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        // List to store matching tiles
        List<SpriteRenderer> result = new List<SpriteRenderer>();

        // Loop through rows below
        for (int i = row + 1; i < GridDimension; i++)
        {
            // Get the SpriteRenderer of the next row
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);

            // If the sprite is different, break the loop
            if (nextRow.sprite != sprite)
            {
                break;
            }

            // Add the matching tile to the list
            result.Add(nextRow);
        }

        // Return the list of matching tiles
        return result;
    }

    // Fill holes in the grid after matches
   void FillHoles()
{
    for (int column = 0; column < GridDimension; column++)
    {
        for (int row = 0; row < GridDimension; row++)
        {
            // Check if the current cell has a null sprite (indicating a cleared tile)
            if (GetSpriteRendererAt(column, row).sprite == null)
            {
                // Start from the cleared cell and move upward
                for (int filler = row; filler < GridDimension - 1; filler++)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, filler);
                    SpriteRenderer next = GetSpriteRendererAt(column, filler + 1);

                    // Shift the sprite from the cell above down to the current cell
                    current.sprite = next.sprite;
                }

                // Generate a new random sprite for the top cell in the column
                SpriteRenderer last = GetSpriteRendererAt(column, GridDimension - 1);
                last.sprite = Sprites[Random.Range(0, Sprites.Count)];
            }
        }
    }
}


    // Generate new tiles at the top after removing matches
    void GenerateNewTiles()
    {
        // Loop through each column
        for (int column = 0; column < GridDimension; column++)
        {
            // Loop through rows from bottom to top
            for (int row = GridDimension - 1; row >= 0; row--)
            {
                // If the current tile is empty
                if (GetSpriteRendererAt(column, row).sprite == null)
                {
                    // Get the SpriteRenderer of the top tile in the same column
                    SpriteRenderer newTileRenderer = GetSpriteRendererAt(column, 0);

                    // Set a random sprite for the top tile
                    newTileRenderer.sprite = Sprites[Random.Range(0, Sprites.Count)];
                }
            }
        }
    }
}
