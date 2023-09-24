using UnityEngine;

namespace Scripts {
	[CreateAssetMenu]
	public class Score : ScriptableObject {
		public float timeRemaining = 20;
		public float timeBonus = 2.0f;
		public int score;
		public int currentCombo = 1;
		public float comboCountdown = 5.0f;

		public override string ToString() {
			return $"Score: {score}, Combo: {currentCombo}, ComboCountdown: {comboCountdown}, Time: {timeRemaining}";
		}
	}
}