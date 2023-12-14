using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Canvas CreditCanvas;
    public Canvas HowToPlay;

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

    public void ShowCredit()
    {
        CreditCanvas.GetComponent<Canvas>().enabled = true;
    }

    public void HideCredit()
    {
        CreditCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void ShowHowToPlay()
    {
        HowToPlay.GetComponent<Canvas>().enabled = true;
    }

    public void HideHowToPlay()
    {
        HowToPlay.GetComponent<Canvas>().enabled = false;
    }
}
