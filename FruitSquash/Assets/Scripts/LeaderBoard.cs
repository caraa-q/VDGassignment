using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LeaderBoard : MonoBehaviour
{
    string leaderboardKey = "userhighscore";

    // Singleton instance
    public static LeaderBoard Instance;

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
}
