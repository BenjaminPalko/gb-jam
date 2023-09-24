using UnityEngine;
using Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public GameObject scoreTextPrefab; // Reference to a prefab of a Text GameObject with "+1" as its initial text
    public GameObject comboPrefab;
    private int previousGameScore;
    private Queue<int> comboQueue = new Queue<int>();
    private IEnumerator scoreCoroutine;

    private float timeRemaining = 5f;
    private GameObject comboTextInstance;


    private void Start()
    {
        // Initialize the combo queue and start the coroutine
        comboQueue = new Queue<int>();
        previousGameScore = SingletonGameData.Instance.playerScore.score;
        scoreCoroutine = ShowComboScore();
        StartCoroutine(scoreCoroutine);
    
    }

    void Update(){
        if(previousGameScore != SingletonGameData.Instance.playerScore.score){
            previousGameScore = SingletonGameData.Instance.playerScore.score;
            if(previousGameScore > 0) comboQueue.Enqueue(1);
        }
        reduceTime();
    }

    
     void reduceTime(){
        if (SingletonGameData.Instance.playerScore.currentCombo != 0)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
               SingletonGameData.Instance.playerScore.currentCombo = 0;
               Destroy(comboTextInstance);
               comboTextInstance = null;
            }
        }
    }

    private IEnumerator ShowComboScore()
    {
        while (true)
        {
            if (comboQueue.Count > 0)
            {
                comboQueue.Dequeue();
                timeRemaining = 5f;
                SingletonGameData.Instance.playerScore.currentCombo += 1;
                SingletonGameData.Instance.playerScore.score -= 1;
                SingletonGameData.Instance.playerScore.score += SingletonGameData.Instance.playerScore.currentCombo;
                previousGameScore = SingletonGameData.Instance.playerScore.score;

                GameObject scoreTextInstance = Instantiate(scoreTextPrefab);
                scoreTextInstance.transform.SetParent(transform, false);
                scoreTextInstance.SetActive(true);
            
                if(comboTextInstance == null){
                    comboTextInstance = Instantiate(comboPrefab);
                    comboTextInstance.transform.SetParent(transform, false);
                    comboTextInstance.SetActive(true);
                }

                var currentComboText = comboTextInstance.GetComponent<TextMeshProUGUI>();
                
                currentComboText.SetText("+" + SingletonGameData.Instance.playerScore.currentCombo.ToString());

                // Destroy the text instance after a delay (adjust this delay as needed)
                Destroy(scoreTextInstance, 1.0f);
                
            }
            yield return null;
        }
    }
}