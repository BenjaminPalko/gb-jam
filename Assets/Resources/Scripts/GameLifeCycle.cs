using ScoreSystem;
using UnityEngine;

public class GameLifeCycle : MonoBehaviour
{
    public Score playerScore;
    
    void Start()
    {
        playerScore.score = 0;
    }
}
