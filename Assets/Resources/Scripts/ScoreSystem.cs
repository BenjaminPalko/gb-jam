using UnityEngine;

namespace ScoreSystem {
    [CreateAssetMenu]
    public class Score : ScriptableObject {
        public int score;

        public float timeRemaining = 20;
    }



}