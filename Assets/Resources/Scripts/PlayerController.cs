using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts {
	[RequireComponent(typeof(PlayerInput), typeof(AudioSource))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] private float movementSpeed = 2.0f;
		[SerializeField] private Vector3 cargoOffset;
		private Abductable m_Abductable;
		private AudioSource m_AudioSource;
		private Vector3 m_Movement;

		private TargetReticle m_TargetReticle;

		public bool immobilize { get; private set; }

		public Canvas pauseMenu;
		private bool m_Paused;

		private void Awake() {
			m_TargetReticle = GetComponentInChildren<TargetReticle>();
			if (!m_TargetReticle) Debug.LogError("TargetReticle not found on child component!");
			m_AudioSource = GetComponent<AudioSource>();
		}

		private void Update() {
			if (m_Movement != Vector3.zero && !immobilize) Movement();
		}

		private void Resume()
		{
			Time.timeScale = 1.0f;
			pauseMenu.gameObject.SetActive(false);
			m_Paused = false;
		}

    	private void Pause()
    	{
			Time.timeScale = 0.0f;
			pauseMenu.gameObject.SetActive(true);
			m_Paused = true;
    	}

		private void OnDrawGizmos() {
			var position = transform.position + cargoOffset;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position - Vector3.left * 0.2f, position - Vector3.right * 0.2f);
		}

		public void OnMove(InputValue inputValue) {
			m_Movement = inputValue.Get<Vector2>();
			m_AudioSource.pitch = m_Movement != Vector3.zero ? 1.1f : 1.0f;
		}

		public void OnA(InputValue inputValue) {
			immobilize = inputValue.isPressed;
			m_AudioSource.pitch = immobilize ? 0.85f : 1.0f;
			if (immobilize && !m_Paused) {
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
			} else if (m_Abductable && !immobilize) {
				m_Abductable.StopAbduction();
				m_Abductable = null;
			}
		}

		public void OnB() {
			Debug.Log("B Pressed");
		}

		public void OnSelect() {
			if(m_Paused){
				Resume();
			}else{
				Pause();
			}
		}

		public void OnStart() {
			if(m_Paused){
				Resume();
			}
		}

		private void Movement() {
			var position = transform.position;
			position = Vector3.MoveTowards(position, position + new Vector3(m_Movement.x, m_Movement.y, position.z),
				Time.deltaTime * movementSpeed);
			transform.position = position;
		}
	}
}