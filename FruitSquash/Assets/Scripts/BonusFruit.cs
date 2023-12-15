using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

    public class BonusFruit : MonoBehaviour
    {
        // Event for handling bonus fruit click
        public event Action<int, int> OnBonusFruitClicked;

        // Fruit position in the grid
        private int xAxis;
        private int yAxis;

        // Set the fruit position in the grid
        public void SetValues(int x, int y)
        {
            xAxis = x;
            yAxis = y;
        }

        // Handle click on the bonus fruit
        private void OnMouseDown()
        {
            // Invoke the bonus fruit click event
            OnBonusFruitClicked?.Invoke(xAxis, yAxis);

            // Optionally, you can play a sound or perform other actions when the bonus fruit is clicked.
        }
    }