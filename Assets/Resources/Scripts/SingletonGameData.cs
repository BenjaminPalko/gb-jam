using System;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts {
	public class SingletonGameData : MonoBehaviour {
		public Score playerScore;
		private readonly List<Action<Score>> m_Callbacks = new();
		private float m_ComboCountdown;


		public static SingletonGameData Instance { get; private set; }

		private void Awake() {
			if (Instance) {
				Debug.LogError("Instance already exists!");
				return;
			}

			Instance = this;
		}

		public void Reset() {
			playerScore.ResetScore();
			TriggerCallbacks();
		}

		private void Start() {
			Time.timeScale = 1.0f;
			playerScore.ResetScore();
			;
			TriggerCallbacks();
		}

		private void Update() {
			if (playerScore.timeRemaining <= 0.0f) {
				Time.timeScale = 0.0f;
				Spawner.instance.DespawnAll();
				SceneManager.LoadScene("GameOverScene");
			}

			playerScore.timeRemaining -= Time.deltaTime;
			m_ComboCountdown -= Time.deltaTime;
			if (playerScore.currentCombo > 1 && m_ComboCountdown <= 0.0f) {
				if (playerScore.currentCombo <= 128) playerScore.currentCombo /= 2;
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
			if (playerScore.currentCombo <= 128) playerScore.currentCombo *= 2;
			playerScore.timeRemaining += playerScore.timeBonus;
			m_ComboCountdown = playerScore.comboCountdown;
		}

		public void ResetCombo() {
			playerScore.currentCombo = 1;
		}

		private void TriggerCallbacks() {
			foreach (var callback in m_Callbacks) {
				callback(playerScore);
			}
		}
	}
}