using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int xAxis;
    public int yAxis;

    public bool isMoving;
    public bool isMatched;

    public FruitType fruitType;

    private Vector2 targetPosition;
    private Vector2 currentPos;

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

    public void MoveToTarget( Vector2 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }

    private IEnumerator MoveCoroutine(Vector2 _targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector2.Lerp(startPosition, _targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = _targetPosition;
        isMoving = false;
    }

}

public enum FruitType
    {
        Banana,
        Cherry,
        Chilli,
        Grape,
        Lemon,
    }