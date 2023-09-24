using UnityEngine;
using Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public GameObject scoreTextPrefab; // Reference to a prefab of a Text GameObject with "+1" as its initial text
    private int previousGameScore;
    private Queue<int> comboQueue = new Queue<int>();
    private IEnumerator scoreCoroutine;

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
    }

    void Destroy(){
        StopCoroutine(scoreCoroutine);
    }

    private IEnumerator ShowComboScore()
    {
        while (true)
        {
            if (comboQueue.Count > 0)
            {
                int comboValue = comboQueue.Dequeue();
                // Create a new instance of the scoreTextPrefab
                GameObject scoreTextInstance = Instantiate(scoreTextPrefab);
                scoreTextInstance.transform.SetParent(transform, false);
                scoreTextInstance.SetActive(true);
                var scoreText = scoreTextInstance.GetComponent<TextMeshProUGUI>();
                scoreText.text = "+" + comboValue;

                // Destroy the text instance after a delay (adjust this delay as needed)
                Destroy(scoreTextInstance, 1.0f);
            }
            yield return null;
        }
    }
}