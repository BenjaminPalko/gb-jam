using System.Collections;
using Scripts;
using TMPro;
using UnityEngine;

namespace Resources.Scripts {
	public class UIManager : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI addTimeText;
		[SerializeField] private TextMeshProUGUI multiplierText;
		[SerializeField] private TextMeshProUGUI timerText;
		[SerializeField] private TextMeshProUGUI scoreText;

		private int m_PrevScore;

		private void Start() {
			SingletonGameData.Instance.AddCallback(Callback);
			m_PrevScore = SingletonGameData.Instance.playerScore.score;
		}

		private void OnDestroy() {
			SingletonGameData.Instance.RemoveCallback(Callback);
		}

		private void Callback(Score score) {
			SetTimer(score.timeRemaining);
			if (m_PrevScore != score.score) StartCoroutine(ShowAddTime(score.timeBonus));
			SetScore(score.score);
			ShowMultiplier(score.currentCombo);
		}

		private void SetTimer(float value) {
			float minutes = Mathf.FloorToInt(value / 60);
			float seconds = Mathf.FloorToInt(value % 60);
			if(seconds >=0 && minutes >= 0) timerText.text = $"{minutes:00}:{seconds:00}";
		}

		private void SetScore(int value) {
			scoreText.text = $"{value}";
			m_PrevScore = value;
		}

		private IEnumerator ShowAddTime(float value) {
			addTimeText.gameObject.SetActive(true);
			addTimeText.text = $"+{value}";
			yield return new WaitForSeconds(1);
			addTimeText.gameObject.SetActive(false);
		}

		private void ShowMultiplier(int value) {
			if (value > 1) {
				multiplierText.gameObject.SetActive(true);
				multiplierText.text = $"x{value}";
			} else {
				multiplierText.gameObject.SetActive(false);
			}
		}
	}
}