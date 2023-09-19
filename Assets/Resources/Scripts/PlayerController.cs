using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts {
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] private float movementSpeed = 1.0f;
		[SerializeField] private float abductionSpeed = 1.0f;
		[SerializeField] private GameObject beam;
		[SerializeField] private Vector3 cargoOffset;
		private readonly List<Abductable> m_Abductables = new();
		private Abductable m_Abductable;

		private Collider2D m_Collider;
		private bool m_Immobilize;
		private Vector3 m_Movement;

		private void Awake() {
			m_Collider = GetComponent<Collider2D>();
		}

		private void Update() {
			if (m_Movement != Vector3.zero && !m_Immobilize) Movement();
		}

		private void OnDrawGizmos() {
			var position = transform.position + cargoOffset;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position - Vector3.left * 0.2f, position - Vector3.right * 0.2f);
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			m_Abductables.Add(abductable);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			m_Abductables.Remove(abductable);
		}

		public void OnMove(InputValue inputValue) {
			m_Movement = inputValue.Get<Vector2>();
		}

		public void OnA(InputValue inputValue) {
			m_Immobilize = inputValue.isPressed;
			if (m_Immobilize) {
				var minDistance = Mathf.Infinity;
				var position = m_Collider.transform.position;
				foreach (var abductable in m_Abductables) {
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

			beam.SetActive(m_Immobilize);
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