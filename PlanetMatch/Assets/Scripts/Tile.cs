using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Tile selected; // Currently selected tile, is static to keep track across all instances
    private SpriteRenderer Renderer; // SpriteRenderer component of the tile
    public Vector2Int Position; // 2D position of the tile in the grid

    // Start is called before the first frame update
    private void Start() // Initialization method
    {
        Renderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    // Change the color of the tile to grey when selected
    public void Select() // Selection method
    {
        Renderer.color = Color.grey;
    }

    // Change the color of the tile to white when unselected
    public void Unselect() // Unselection method
    {
        Renderer.color = Color.white;
    }

    // Called when the tile is clicked
    private void OnMouseDown()
    {
        // Check if there is a previously selected tile
        if (selected != null)
        {
            // Check if the clicked tile is the same as the selected one
            if (selected == this)
                return;

            // Unselect the previously selected tile
            selected.Unselect();

            // Check if the clicked tile is adjacent to the selected one
            if (Vector2Int.Distance(selected.Position, Position) == 1)
            {
                // Swap the positions of the clicked and selected tiles
                GridManager.Instance.SwapTiles(Position, selected.Position);
                selected = null; // Reset the selected tile
            }
            else
            {
                selected = this; // Set the clicked tile as the new selected tile
                Select(); // Highlight the clicked tile
            }
        }
        else
        {
            selected = this; // Set the clicked tile as the selected tile
            Select(); // Highlight the clicked tile
        }
    }
}
