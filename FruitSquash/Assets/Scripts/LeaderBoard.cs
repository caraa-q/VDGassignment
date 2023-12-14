using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main; // Package imported

public class LeaderBoard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;

    [SerializeField]
    private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "a4e8b866895abe1b9f3fab3cf3e303b6dc495396ba58c70f88e8bdb85dd100fa";

    void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        // Call back function called for getting the leaderboard is completed. 
        // Will return a value that is accessible via the msg
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) => { 
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            for (int i = 0; i < loopLength; i++) {
                names[i].text = msg[i].Username; 
                scores[i].text = msg[i].Score.ToString();
            }     
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((_) => 
        {
            username.Substring(0,20); // Limit username characters
            GetLeaderboard(); // Called so use can see changes
        }));
    }   
}