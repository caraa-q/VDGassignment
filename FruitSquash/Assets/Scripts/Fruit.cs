using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int xAxis;
    public int yAxis;
    public int isMatched;
    public bool isMoving;
    public FruitType fruitType;

    private Vector2 currentPosition;
    private Vector2 targetPosition;

    public Fruit(int x, int y)
    {
        xAxis = x;
        yAxis = y;
    }

    public void SetValues(int x, int y)
    {
        xAxis = x;
        yAxis = y;
    }

    public enum FruitType // Drop down in the inspector instead of having to drag assets 
    {
        Banana,
        Cherry,
        Chilli,
        Grape,
        Lemon,
        Orange,
        Pear,
        Tomato
    }

    public bool CheckGrid()
    {
        
    }
}