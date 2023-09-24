using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts {
	public class SingletonGameData : MonoBehaviour {
		public Score playerScore;
		private readonly List<Action<Score>> m_Callbacks = new();
		private float m_ComboCountdown;

		private Score m_DefaultScore;

		public static SingletonGameData Instance { get; private set; }

		private void Awake() {
			if (Instance) {
				Debug.LogError("Instance already exists!");
				return;
			}

			Instance = this;
			m_DefaultScore = UnityEngine.Resources.Load<Score>("Objects/PlayerScore");
		}

		public void Reset() {
			playerScore = Instantiate(m_DefaultScore);
			TriggerCallbacks();
		}

		private void Start() {
			playerScore = Instantiate(m_DefaultScore);
			TriggerCallbacks();
		}

		private void Update() {
			if (playerScore.timeRemaining <= 0.0f) Time.timeScale = 0.0f;
			playerScore.timeRemaining -= Time.deltaTime;
			m_ComboCountdown -= Time.deltaTime;
			if (playerScore.currentCombo > 1 && m_ComboCountdown <= 0.0f) {
				playerScore.currentCombo /= 2;
				m_ComboCountdown = playerScore.comboCountdown;
			}

			TriggerCallbacks();
		}

		public void AddCallback(Action<Score> cb) {
			m_Callbacks.Add(cb);
		}

		public void RemoveCallback(Action<Score> cb) {
			m_Callbacks.Remove(cb);
		}

		public void IncreaseScore() {
			playerScore.score += playerScore.currentCombo;
			playerScore.currentCombo *= 2;
			playerScore.timeRemaining += playerScore.timeBonus;
			m_ComboCountdown = playerScore.comboCountdown;
		}

		private void TriggerCallbacks() {
			foreach (var callback in m_Callbacks) {
				callback(playerScore);
			}
		}
	}
}