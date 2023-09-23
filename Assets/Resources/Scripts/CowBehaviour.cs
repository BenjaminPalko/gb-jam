using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(NavMeshAgent))]
	public class CowBehaviour : MonoBehaviour
	{
        private const int running_distance_multiplier = 3;
        private static readonly int Speed = Animator.StringToHash("speed");
		private static readonly int Graze = Animator.StringToHash("graze");
		private static readonly int Abducting = Animator.StringToHash("abducting");

		private float walkSpeed = 3.5f;
		private float runSpeed = 5f;


		[SerializeField] private int wanderRadius = 4;
		private Animator m_Animator;
		private NavMeshAgent m_NavMeshAgent;

		private SpriteRenderer m_SpriteRenderer;
		private IEnumerator m_WanderCoroutine;

		private void Awake()
		{
			m_NavMeshAgent = GetComponent<NavMeshAgent>();
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
			m_Animator = GetComponent<Animator>();
		}

		private void Start()
		{
			m_Animator.SetInteger(Speed, 0);
			m_NavMeshAgent.updateUpAxis = false;
			m_NavMeshAgent.updateRotation = false;
			m_NavMeshAgent.speed = walkSpeed;
			StartWander();
		}


		private void Update()
		{
			if (m_NavMeshAgent.velocity.magnitude > 0.5)
			{
				m_SpriteRenderer.flipX = m_NavMeshAgent.velocity.x < 0;
			}
		}

		private void OnDisable()
		{
			StopWander();
		}

		private void OnDestroy()
		{
			StopWander();
		}

		private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
		{
			var randDirection = Random.insideUnitSphere * dist;
			randDirection += origin;
			return NavMesh.SamplePosition(randDirection, out var navHit, dist, layermask) ? navHit.position : origin;
		}

		private IEnumerator Wander()
		{
			while (true)
			{
				var destination = RandomNavSphere(transform.position, wanderRadius, -1);
				if (destination != transform.position)
				{
					m_NavMeshAgent.SetDestination(destination);
				}

				yield return new WaitUntil(() => m_NavMeshAgent.velocity != Vector3.zero);
				m_Animator.SetInteger(Speed, 1);
				yield return new WaitUntil(() => m_NavMeshAgent.velocity == Vector3.zero);
				m_Animator.SetInteger(Speed, 0);
				var wait = Random.Range(2.0f, 5.0f);
				yield return new WaitForSecondsRealtime(wait / 2);
				m_Animator.SetTrigger(Graze);
				yield return new WaitForSecondsRealtime(wait / 2);
			}
		}

		private IEnumerator RestartWander()
		{
			while (true)
			{
				yield return new WaitForSecondsRealtime(1);
				yield return new WaitUntil(() => m_NavMeshAgent.velocity == Vector3.zero);
				m_Animator.SetBool(Abducting, false);
				m_NavMeshAgent.speed = walkSpeed;
				StartWander();
				yield break;
			}
		}

		public void Scare(Vector3 position)
		{
			StopWander();
			RunFrom(position);
			StartCoroutine(RestartWander());

		}

		private void StartWander()
		{
			if (m_WanderCoroutine != null) return;
			m_WanderCoroutine = Wander();
			StartCoroutine(m_WanderCoroutine);
		}

		private void StopWander()
		{
			if(m_NavMeshAgent.enabled) m_NavMeshAgent.ResetPath();
			if (m_WanderCoroutine == null) return;
			StopCoroutine(m_WanderCoroutine);
			m_WanderCoroutine = null;
		}

		public void RunFrom(Vector3 position)
		{
			Vector3 runTo = running_distance_multiplier * (transform.position  - position);
			NavMeshHit hit;
			NavMesh.SamplePosition(runTo, out hit, wanderRadius, -1);
			m_NavMeshAgent.speed = runSpeed;
			var destination = runTo != Vector3.zero ? hit.position : RandomNavSphere(transform.position, running_distance_multiplier * wanderRadius, -1);
			m_NavMeshAgent.SetDestination(destination);
			m_Animator.SetBool(Abducting, true);
			m_Animator.SetInteger(Speed, 2);
		}
	}
}