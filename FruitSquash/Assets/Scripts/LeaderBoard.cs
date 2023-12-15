using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    string leaderboardKey = "userhighscore";
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    // Singleton instance
    public static LeaderBoard Instance;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator SubmitScore(int scoreToUpload)
    {
            bool done = false;
            string playerID = PlayerPrefs.GetString("PlayerID");
            
            LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardKey, (response) => 
            {
                if(response.success)
                {
                    done = true;
                }
                else
                {
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
    }

    public IEnumerator GetTopHighScore()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardKey, 10, 0, (response) =>
        {
            if(response.success)
            {
                string tempPlayerNames = "Names\n";
                string tempPlayerScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                for(int i = 0; i < members.Length; i++)
                {
                    tempPlayerNames += members[i].rank + ". ";
                    if(members[i].player.name != "")
                    {
                        tempPlayerNames += members[i].player.name;
                    }
                    else
                    {
                        tempPlayerNames += members[i].player.id;
                    }
                    tempPlayerScores += members[i].score + "\n";
                    tempPlayerNames += "\n";
                }
                done = true;
                playerName.text = tempPlayerNames;
                playerScore.text = tempPlayerScores;
            }
            else
            {
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}