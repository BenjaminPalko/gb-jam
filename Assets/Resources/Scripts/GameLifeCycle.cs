using Scripts;
using UnityEngine;

public class GameLifeCycle : MonoBehaviour
{

    public float timer = 200f;
    void Start()
    {
        SingletonGameData.Instance.playerScore.score = 0;
        SingletonGameData.Instance.playerScore.timeRemaining = timer;
    }
}
