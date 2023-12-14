using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI inputScore;

    [SerializeField]
    private TMP_InputField inputName;

    public UnityEvent<string, int> submitScoreEvent;

    public void SubmitScore()
    {
        // Reference input score and name. Communicates with leaderboard without direct reference 
        submitScoreEvent.Invoke(inputName.text, int.Parse(inputScore.text));
    }
}
