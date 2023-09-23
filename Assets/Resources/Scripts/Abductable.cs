using UnityEngine;

namespace Scripts {
	[
		RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator)),
		RequireComponent(typeof(CowBehaviour))
	]
	public class Abductable : MonoBehaviour {
		private static readonly int Abducting = Animator.StringToHash("abducting");
		[SerializeField] private float liftSpeed = 1.0f;
		[SerializeField] private float fallSpeed = 1.0f;

		private Animator m_Animator;
		private Vector3 m_AttractorPosition;
		private CowBehaviour m_CowBehaviour;
		private bool m_Immobilize;
		private Vector3 m_OriginalPosition;

		private void Awake() {
			m_Animator = GetComponent<Animator>();
			m_CowBehaviour = GetComponent<CowBehaviour>();
		}

		private void Update() {
			if (m_AttractorPosition != Vector3.zero && m_Immobilize) Attract();
			else if (m_Immobilize) Fall();
		}

		private void OnDestroy() {
			Destroy(gameObject);
		}

		public void StartAbduction(Vector3 saucerPosition) {
			m_Animator.SetBool(Abducting, true);
			m_CowBehaviour.enabled = false;
			m_OriginalPosition = transform.position;
			m_AttractorPosition = saucerPosition;
			m_Immobilize = true;
		}

		public void StopAbduction() {
			m_Animator.SetBool(Abducting, false);
			m_AttractorPosition = Vector3.zero;
			m_OriginalPosition.x = transform.position.x;
		}

		private void Attract() {
			var abductablePosition = transform.position;
			abductablePosition =
				Vector3.MoveTowards(abductablePosition, m_AttractorPosition, Time.deltaTime * liftSpeed);
			var distance = (abductablePosition - m_AttractorPosition).magnitude;
			if (distance < 0.10f) Destroy(this);
			else transform.position = abductablePosition;
		}

		private void Fall() {
			var abductablePosition = transform.position;
			abductablePosition =
				Vector3.MoveTowards(abductablePosition, m_OriginalPosition, Time.deltaTime * fallSpeed * 9.81f);
			var distance = abductablePosition.y - m_OriginalPosition.y;
			if (distance == 0.0f) Ground();
			else transform.position = abductablePosition;
		}

		private void Ground() {
			m_Immobilize = false;
			m_CowBehaviour.enabled = true;
		}
	}
}