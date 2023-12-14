using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Canvas CreditCanvas;

    public void loadGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void showCredit()
    {
        CreditCanvas.GetComponent<Canvas> ().enabled = true;
    }
    public void hideCredit()
    {
        CreditCanvas.GetComponent<Canvas> ().enabled = false;
    }

}
