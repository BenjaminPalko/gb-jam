using UnityEngine;

namespace Scripts {
	public class Abductable : MonoBehaviour {

		public bool immobilize;
		
		private void OnDestroy() {
			Destroy(gameObject);
		}
	}
}