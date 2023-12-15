using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class LeaderBoardManager : MonoBehaviour
{
    public LeaderBoard leaderboard;
    public TMP_InputField playerNameInputField;

    void Start()
    { 
        StartCoroutine(SetUp());
    }

    public void SetPlayerName()
    {
        LootLockerSDKManager.SetPlayerName(playerNameInputField.text, (response) => 
        {
            if(response.success)
            {
                Debug.Log("Player name set");
            }
            else
            {
                //
            }
        });
    }

    IEnumerator SetUp()
    {
        yield return LogIn();
        yield return leaderboard.GetTopHighScore();
    }
    
    IEnumerator LogIn()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if(response.success)
            {
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
