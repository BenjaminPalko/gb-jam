using System.Collections;
using UnityEngine;

namespace Scripts {
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
	public class PitchforkController : MonoBehaviour {
		private static readonly int Speed = Animator.StringToHash("speed");

		[SerializeField] private float despawnDelay = 2.0f;
		private Animator m_Animator;

		private IEnumerator m_DespawnCoroutine;
		private Vector3 m_InitialPosition;
		private Vector3 m_InitialVelocity;
		private Rigidbody2D m_Rigidbody2D;

		private SpriteRenderer m_SpriteRenderer;
		private Vector3 m_Target;
		private bool m_Throw;

		private void Awake() {
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
			m_Animator = GetComponent<Animator>();
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
		}

		private void Update() {
			if (!m_Throw) return;
			if (transform.position.y < m_InitialPosition.y - 0.5f) {
				Land();
				return;
			}

			m_Animator.SetFloat(Speed, m_Rigidbody2D.velocity.y / m_InitialVelocity.y);
		}

		private void OnDestroy() {
			if (m_DespawnCoroutine != null) StopCoroutine(m_DespawnCoroutine);
		}

		private void OnCollisionEnter2D(Collision2D other) {
			if (!other.gameObject.TryGetComponent<PlayerController>(out var playerController)) return;
			playerController.Stun();
			Destroy(gameObject);
		}

		public void Throw(Vector2 target) {
			m_Throw = true;
			m_Target = target;
			m_InitialPosition = transform.position;
			var diff = m_Target - m_InitialPosition;
			if (diff != Vector3.zero)
				m_SpriteRenderer.flipX = diff.x < 0;
			m_InitialVelocity = CalculateInitialVelocity();
			m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
			m_Rigidbody2D.velocity = m_InitialVelocity;
			return;

			Vector2 CalculateInitialVelocity() {
				const float time = 1.5f;
				var position = transform.position;
				return new Vector2((m_Target.x - position.x) / time, Mathf.Sqrt(2 * -Physics.gravity.y *
					(m_Target.y - position.y)));
			}
		}

		private void Land() {
			m_Throw = false;
			m_InitialVelocity = Vector2.zero;
			m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
			m_Rigidbody2D.velocity = Vector2.zero;
			m_DespawnCoroutine = WaitThenDespawn(despawnDelay);
			StartCoroutine(m_DespawnCoroutine);
		}

		private IEnumerator WaitThenDespawn(float wait = 1.0f) {
			yield return new WaitForSeconds(wait);
			Destroy(gameObject);
		}
	}
}