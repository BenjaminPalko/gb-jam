using UnityEngine;

namespace Scripts {
	[RequireComponent(typeof(Collider2D))]
	public class FarmerTrigger : MonoBehaviour {
		public PlayerController playerController { get; private set; }

		private void OnTriggerEnter2D(Collider2D other) {
			if (other.TryGetComponent<PlayerController>(out var temp)) playerController = temp;
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (other.TryGetComponent<PlayerController>(out _)) playerController = null;
		}
	}
}