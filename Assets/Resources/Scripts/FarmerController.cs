using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts {
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(NavMeshAgent))]
	public class FarmerController : MonoBehaviour {
		private static readonly int Moving = Animator.StringToHash("moving");
		private static readonly int Aggressive = Animator.StringToHash("aggressive");
		private static readonly int Throw = Animator.StringToHash("throw");

		[SerializeField] private float fireRate = 0.25f;
		[SerializeField] private float wanderRadius = 2.0f;
		[SerializeField] private GameObject pitchFork;
		[SerializeField] private Vector3 throwPosition;

		[SerializeField] private FarmerTrigger alertTrigger;
		[SerializeField] private FarmerTrigger chaseTrigger;
		private bool m_Aggravated;
		private Animator m_Animator;
		private bool m_Cooldown;
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
			if (chaseTrigger.playerController &&
			    Vector3.Distance(TargetPosition(chaseTrigger.playerController.transform.position), transform.position) <
			    0.10f) {
				m_Animator.SetBool(Moving, false);
				StopWander();
				Attack(chaseTrigger.playerController.transform.position);
			} else if (alertTrigger.playerController && alertTrigger.playerController.immobilize) {
				Chase(alertTrigger.playerController.transform.position);
			} else if (chaseTrigger.playerController) {
				Chase(chaseTrigger.playerController.transform.position);
			} else if (m_Aggravated) {
				Calm();
			}

			if (Mathf.Abs(m_NavMeshAgent.velocity.x) > 0.10f) m_SpriteRenderer.flipX = m_NavMeshAgent.velocity.x < 0;
		}

		private void OnDestroy() {
			if (m_WanderingCoroutine != null) {
				StopCoroutine(m_WanderingCoroutine);
			}
		}

		private Vector3 TargetPosition(Vector3 target) {
			var temp = throwPosition + Vector3.zero;
			var diff = transform.position - target;
			temp.x *= Mathf.Sign(diff.x);
			return target + temp;
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
			Move(TargetPosition(target));
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
			if (m_Cooldown || m_PitchforkController || !m_Aggravated) return;
			m_PitchforkController = Instantiate(pitchFork, transform).GetComponent<PitchforkController>();
			m_Animator.SetTrigger(Throw);
			m_PitchforkController.Throw(target);
			StartCoroutine(AttackCooldown(1 / fireRate));
		}

		private IEnumerator Wander() {
			while (true) {
				yield return new WaitUntil(() => m_NavMeshAgent.isOnNavMesh);
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

		private IEnumerator AttackCooldown(float time) {
			m_Cooldown = true;
			yield return new WaitForSeconds(time);
			m_Cooldown = false;
		}
	}
}