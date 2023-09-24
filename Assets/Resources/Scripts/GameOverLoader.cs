using Scripts;

namespace Resources.Scripts {
	public class GameOverLoader : SceneLoader {
		public void OnStart() {
			LoadDebug();
			UnityEngine.Resources.Load<Score>("Objects/PlayerScore").ResetScore();
		}
	}
}