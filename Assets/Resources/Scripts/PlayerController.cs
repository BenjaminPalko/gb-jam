using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts {
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] private float movementSpeed = 1.0f;
		[SerializeField] private Vector3 cargoOffset;
		private Abductable m_Abductable;

		private bool m_Immobilize;
		private Vector3 m_Movement;
		private TargetReticle m_TargetReticle;

		private void Awake() {
			m_TargetReticle = GetComponentInChildren<TargetReticle>();
			if (!m_TargetReticle) Debug.LogError("TargetReticle not found on child component!");
		}

		private void Update() {
			if (m_Movement != Vector3.zero && !m_Immobilize) Movement();
		}

		private void OnDrawGizmos() {
			var position = transform.position + cargoOffset;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position - Vector3.left * 0.2f, position - Vector3.right * 0.2f);
		}

		public void OnMove(InputValue inputValue) {
			m_Movement = inputValue.Get<Vector2>();
		}

		public void OnA(InputValue inputValue) {
			m_Immobilize = inputValue.isPressed;
			if (m_Immobilize) {
				var minDistance = Mathf.Infinity;
				var position = m_TargetReticle.transform.position;
				foreach (var abductable in m_TargetReticle.Abductables) {
					var abductablePosition = abductable.transform.position;
					var distance = (position - abductablePosition).magnitude;
					if (!(distance < minDistance)) continue;
					minDistance = distance;
					m_Abductable = abductable;
				}

				if (m_Abductable) m_Abductable.StartAbduction(transform.position + cargoOffset);
			} else if (m_Abductable && !m_Immobilize) {
				m_Abductable.StopAbduction();
				m_Abductable = null;
			}
		}

		public void OnB() {
			Debug.Log("B Pressed");
		}

		public void OnSelect() {
			Debug.Log("Select Pressed");
		}

		public void OnStart() {
			Debug.Log("Start Pressed");
		}

		private void Movement() {
			var position = transform.position;
			position = Vector3.MoveTowards(position, position + new Vector3(m_Movement.x, m_Movement.y, position.z),
				Time.deltaTime * movementSpeed);
			transform.position = position;
		}
	}
}