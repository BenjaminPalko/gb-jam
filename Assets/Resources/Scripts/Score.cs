using UnityEngine;

namespace Scripts {
	[CreateAssetMenu]
	public class Score : ScriptableObject {
		public float timeRemaining = 120;
		public float timeBonus = 2.0f;
		public int score;
		public int currentCombo = 1;
		public float comboCountdown = 5.0f;

		public override string ToString() {
			return $"Score: {score}, Combo: {currentCombo}, ComboCountdown: {comboCountdown}, Time: {timeRemaining}";
		}

		public void ResetScore() {
			timeRemaining = 120;
			timeBonus = 2.0f;
			score = 0;
			currentCombo = 1;
			comboCountdown = 5.0f;
		}
	}
}