using UnityEngine;

namespace Scripts {
	[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator))]
	public class Abductable : MonoBehaviour {
		public bool immobilize;

		private Animator m_Animator;

		private void Awake() {
			m_Animator = GetComponent<Animator>();
		}

		private void Start() {
			m_Animator.speed = 0.25f;
		}

		private void OnDestroy() {
			Destroy(gameObject);
		}
	}
}