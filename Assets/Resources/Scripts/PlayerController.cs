using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts {
	[RequireComponent(typeof(Animator), typeof(PlayerInput), typeof(AudioSource))]
	public class PlayerController : MonoBehaviour {
		private static readonly int Hurt = Animator.StringToHash("hurt");
		[SerializeField] private float movementSpeed = 2.0f;
		[SerializeField] private Vector3 cargoOffset;
		[SerializeField] private List<AudioClip> m_StunClips = new();
		
		private Abductable m_Abductable;

		private Animator m_Animator;
		private AudioSource m_AudioSource;
		private Vector3 m_Movement;
		private IEnumerator m_Stunned;

		private TargetReticle m_TargetReticle;

		public bool immobilize { get; private set; }

		public GameObject pauseMenu;
		private bool m_Paused;

		private void Awake() {
			m_Animator = GetComponent<Animator>();
			m_TargetReticle = GetComponentInChildren<TargetReticle>();
			if (!m_TargetReticle) Debug.LogError("TargetReticle not found on child component!");
			m_AudioSource = GetComponent<AudioSource>();
		}

		private void Update() {
			if (m_Movement != Vector3.zero && !immobilize) Movement();
		}

		private void OnDestroy() {
			if (m_Stunned == null) return;
			StopCoroutine(m_Stunned);
			m_Stunned = null;
		}

		private void OnDrawGizmos() {
			var position = transform.position + cargoOffset;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position - Vector3.left * 0.2f, position - Vector3.right * 0.2f);
		}

		public void Stun() {
			if (m_Stunned != null) return;
			m_Stunned = StunPlayer();
			StartCoroutine(m_Stunned);
		}

		private IEnumerator StunPlayer() {
			m_Animator.SetBool(Hurt, true);
			m_Movement = Vector3.zero;
			if (!m_AudioSource.isPlaying) m_AudioSource.PlayOneShot(m_StunClips[Random.Range(0, m_StunClips.Count)]);
			if (m_Abductable) {
				m_Abductable.StopAbduction();
				m_Abductable = null;
			}

			yield return new WaitForSeconds(0.5f);
			m_Stunned = null;
			m_Animator.SetBool(Hurt, false);
		}

		private void Resume() {
			Time.timeScale = 1.0f;
			pauseMenu.gameObject.SetActive(false);
			m_Paused = false;
		}

		private void Pause() {
			Time.timeScale = 0.0f;
			pauseMenu.gameObject.SetActive(true);
			m_Paused = true;
		}

		public void OnMove(InputValue inputValue) {
			if (m_Stunned != null) return;
			m_Movement = inputValue.Get<Vector2>();
		}

		public void OnA(InputValue inputValue) {
			if (m_Stunned != null) return;
			immobilize = inputValue.isPressed;
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
			if (m_Paused) {
				Resume();
			} else {
				Pause();
			}
		}

		public void OnStart() {
			if (m_Paused) {
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