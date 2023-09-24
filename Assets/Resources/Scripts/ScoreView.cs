using Scripts;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public Score playerScore;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text ="Score: " + playerScore.score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
