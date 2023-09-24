using Scripts;
using TMPro;
using UnityEngine;
public class Timer : MonoBehaviour
{

    
    private int previousGameScore;
    private TextMeshProUGUI timerText;


    public bool timerIsRunning = false;


    // Start is called before the first frame update
    void Start()
    {
        timerIsRunning = true;
        timerText = GetComponent<TextMeshProUGUI>();
        previousGameScore = SingletonGameData.Instance.playerScore.score;
        DisplayTime(SingletonGameData.Instance.playerScore.timeRemaining);
    }

    // Update is called once per frame
    void Update()
    {
        timerText.SetText(SingletonGameData.Instance.playerScore.score.ToString());
        reduceTime();
        DisplayTime(SingletonGameData.Instance.playerScore.timeRemaining);
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if(previousGameScore != SingletonGameData.Instance.playerScore.score){
            increaseTime();
        }
    }

    void increaseTime(){
        SingletonGameData.Instance.playerScore.timeRemaining += 1;
        previousGameScore = SingletonGameData.Instance.playerScore.score;
    }

    void reduceTime(){
        if (timerIsRunning)
        {
            if (SingletonGameData.Instance.playerScore.timeRemaining > 0)
            {
                SingletonGameData.Instance.playerScore.timeRemaining -= Time.deltaTime;
            }
            else
            {
                SingletonGameData.Instance.playerScore.timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }
}
