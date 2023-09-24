using UnityEngine;
using ScoreSystem;

namespace Scripts {
    public class SingletonGameData : MonoBehaviour {
        
        public static SingletonGameData Instance { get; private set; }

        public Score playerScore;

        private void Awake() {
            if (Instance) {
                Debug.LogError("Instance already exists!");
                return;
            }

            Instance = this;
        }
    }
}