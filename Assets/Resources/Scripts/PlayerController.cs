using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts {
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {

		[SerializeField] private float movementSpeed = 1.0f;
		[SerializeField] private float abductionSpeed = 1.0f;
		[SerializeField] private GameObject beam;

		private Collider2D m_Collider;
		private readonly List<Abductable> m_Abductables = new();
		private Abductable m_Abductable;
		private Vector3 m_Movement;
		private bool m_Abduction;

		private void Awake() {
			m_Collider = GetComponent<Collider2D>();
		}

		private void Update() {
			if (m_Movement != Vector3.zero && !m_Abduction) Movement();
			else if (m_Abduction && m_Abductable) Abduct();
		}

		public void OnMove(InputValue inputValue) {
			m_Movement = inputValue.Get<Vector2>();
		}

		public void OnA(InputValue inputValue) {
			m_Abduction = inputValue.isPressed;
			if (m_Abduction) {
				var minDistance = Mathf.Infinity;
				var position = m_Collider.transform.position;
				foreach (var abductable in m_Abductables) {
					var abductablePosition = abductable.transform.position;
					var distance = (position - abductablePosition).magnitude;
					if (!(distance < minDistance)) continue;
					minDistance = distance;
					m_Abductable = abductable;
				}
				if (m_Abductable) m_Abductable.immobilize = true;
			} else {
				if (m_Abductable) m_Abductable.immobilize = false;
				m_Abductable = null;
			}
			beam.SetActive(m_Abduction);
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

		private void Abduct() {
			var position = transform.position;
			var abductablePosition = m_Abductable.transform.position;
			abductablePosition = Vector3.MoveTowards(abductablePosition, position, Time.deltaTime * abductionSpeed);
			var distance = (abductablePosition - position).magnitude;
			if (distance < 0.10f) Destroy(m_Abductable);
			else m_Abductable.transform.position = abductablePosition;
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			Debug.Log("Enter: " + abductable.name);
			m_Abductables.Add(abductable);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			Debug.Log("Leave: " + abductable.name);
			m_Abductables.Remove(abductable);
		}
	}

}