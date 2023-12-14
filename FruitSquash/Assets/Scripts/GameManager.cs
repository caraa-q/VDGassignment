using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // static reference

    public GameObject backgroundPanel; // grey background 
    public GameObject winPanel;
    public GameObject losePanel;

    public int mainGoal; // the amount of points you need to get to to win.
    public int numMoves; //the number of turns you can take
    public int numPoints; // the current points you have earned.

    public bool isGameEnded;

    public TMP_Text numPointsTxt;
    public TMP_Text numMovesTxt;
    public TMP_Text mainGoalTxt;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int _numMoves, int _mainGoal)
    {
        numMoves = _numMoves;
        mainGoal = _mainGoal;
    }

    // Update is called once per frame
    void Update()
    {
        numPointsTxt.text = "Points: " + numPoints.ToString();
        numMovesTxt.text = "Moves: " + numMoves.ToString();
        mainGoalTxt.text = "Goal: " + mainGoal.ToString();
    }

    public void ProcessTurn(int _gainPoints, bool _subtractMoves)
    {
        numPoints += _gainPoints;
        if (_subtractMoves)
            numMoves--;

        if (numPoints >= mainGoal)
        {
            //you've won the game
            isGameEnded = true;
            //Display a victory screen
            backgroundPanel.SetActive(true);
            winPanel.SetActive(true);
            FruitGrid.Instance.fruitParent.SetActive(false);
            return;
        }
        if (numMoves == 0)
        {
            //lose the game
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            FruitGrid.Instance.fruitParent.SetActive(false);
            return;
        }
    }

    //attached to a button to change scene when winning
    public void GameWon()
    {
        string playerName = "Player"; // You can use the actual player's name
        int playerScore = numPoints;
        FindObjectOfType<LeaderBoard>().SetLeaderboardEntry(playerName, playerScore);
        
        SceneManager.LoadScene(0);
    }

    //attached to a button to change scene when losing
    public void GameLost()
    {
        SceneManager.LoadScene(0);
    }

    public void loadGame()
    {
        SceneManager.LoadScene("Level1");
    }

     public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
