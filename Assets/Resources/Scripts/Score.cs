using UnityEngine;

namespace Scripts {
	[CreateAssetMenu]
	public class Score : ScriptableObject {
		public float timeRemaining = 20;
		public float timeBonus = 2.0f;
		public int score;
		public int currentCombo = 0;
		public float comboCountdown = 5.0f;
	}
}