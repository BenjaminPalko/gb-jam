using Scripts;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI scoreText;
	public Score playerScore;

	private void Start() {
		Time.timeScale = 1.0f;
		scoreText.text = "Score: " + playerScore.score;
	}
}