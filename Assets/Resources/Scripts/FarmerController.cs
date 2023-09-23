using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts {
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(NavMeshAgent))]
	public class FarmerController : MonoBehaviour {
		private static readonly int Moving = Animator.StringToHash("moving");
		private static readonly int Aggressive = Animator.StringToHash("aggressive");
		private static readonly int Property = Animator.StringToHash("throw");

		[SerializeField] private float fireRate = 0.25f;
		[SerializeField] private float wanderRadius = 2.0f;
		[SerializeField] private GameObject pitchFork;

		[SerializeField] private FarmerTrigger alertTrigger;
		[SerializeField] private FarmerTrigger attackTrigger;
		[SerializeField] private FarmerTrigger chaseTrigger;
		private bool m_Aggravated;
		private Animator m_Animator;
		private float m_Cooldown;
		private NavMeshAgent m_NavMeshAgent;
		private PitchforkController m_PitchforkController;
		private SpriteRenderer m_SpriteRenderer;
		private IEnumerator m_WanderingCoroutine;


		private void Awake() {
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
			m_Animator = GetComponent<Animator>();
			m_NavMeshAgent = GetComponent<NavMeshAgent>();
		}

		private void Start() {
			m_NavMeshAgent.updateUpAxis = false;
			m_NavMeshAgent.updateRotation = false;
			StartWander();
		}

		private void Update() {
			if (m_Cooldown > 0.0f) m_Cooldown -= Time.deltaTime;
			if (attackTrigger.playerController) {
				m_Animator.SetBool(Moving, false);
				StopWander();
				Attack(attackTrigger.playerController.transform.position);
			} else if (alertTrigger.playerController && alertTrigger.playerController.immobilize) {
				Chase(alertTrigger.playerController.transform.position);
			} else if (chaseTrigger.playerController) {
				Chase(chaseTrigger.playerController.transform.position);
			} else if (m_Aggravated) {
				Calm();
			}

			if (m_NavMeshAgent.velocity != Vector3.zero) m_SpriteRenderer.flipX = m_NavMeshAgent.velocity.x < 0;
		}

		private void Move(Vector3 destination) {
			if (NavMesh.SamplePosition(destination, out var navMeshHit, wanderRadius, -1)) {
				m_NavMeshAgent.SetDestination(navMeshHit.position);
			}
		}

		private void Chase(Vector3 target) {
			StopWander();
			if (!m_Aggravated) {
				m_Aggravated = true;
				m_Animator.SetBool(Aggressive, m_Aggravated);
			}

			m_Animator.SetBool(Moving, true);
			Move(alertTrigger.playerController.transform.position);
		}

		private void Calm() {
			if (m_PitchforkController) Destroy(m_PitchforkController);
			m_PitchforkController = null;
			m_Aggravated = false;
			m_Animator.SetBool(Moving, false);
			m_Animator.SetBool(Aggressive, m_Aggravated);
			StartWander();
		}

		private void Attack(Vector2 target) {
			if (m_NavMeshAgent.velocity != Vector3.zero) m_NavMeshAgent.velocity = Vector3.zero;
			if (m_Cooldown > 0 || m_PitchforkController) return;
			m_PitchforkController = Instantiate(pitchFork, transform).GetComponent<PitchforkController>();
			m_Animator.SetTrigger(Property);
			m_PitchforkController.Throw(target);
			m_Cooldown = 1 / fireRate;
		}

		private IEnumerator Wander() {
			while (true) {
				Vector3 direction = Random.insideUnitCircle;
				Move(direction * wanderRadius + transform.position);
				yield return new WaitUntil(() => m_NavMeshAgent.velocity != Vector3.zero);
				m_Animator.SetBool(Moving, true);
				yield return new WaitUntil(() => m_NavMeshAgent.velocity == Vector3.zero);
				m_Animator.SetBool(Moving, false);
				yield return new WaitForSecondsRealtime(Random.Range(2.0f, 5.0f));
			}
		}

		private void StartWander() {
			if (m_WanderingCoroutine != null) return;
			m_WanderingCoroutine = Wander();
			StartCoroutine(m_WanderingCoroutine);
		}

		private void StopWander() {
			if (m_WanderingCoroutine == null) return;
			StopCoroutine(m_WanderingCoroutine);
			m_WanderingCoroutine = null;
		}
	}
}