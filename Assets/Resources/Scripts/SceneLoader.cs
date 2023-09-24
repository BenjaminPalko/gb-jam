using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts {
	public class SceneLoader : MonoBehaviour {
		public static void LoadMenu() {
			SceneManager.LoadScene("MenuScene");
		}

		public static void LoadGame() {
			SceneManager.LoadScene("GameScene");
		}

		public static void LoadDebug() {
			SceneManager.LoadScene("DebugScene");
		}

		public void OnStart() {
			LoadDebug();
		}
	}
}