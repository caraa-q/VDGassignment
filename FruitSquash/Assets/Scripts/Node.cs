using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable; // Detertimes if the space can be filled
    public GameObject fruit;

    public Node(bool _isUsable, GameObject _fruit)
    {
        isUsable = _isUsable;
        fruit = _fruit;
    }
}