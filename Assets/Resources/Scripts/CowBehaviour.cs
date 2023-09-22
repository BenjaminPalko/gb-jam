using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CowBehaviour : MonoBehaviour {
	private static readonly int Speed = Animator.StringToHash("speed");
	private static readonly int Graze = Animator.StringToHash("graze");
	[SerializeField] private int wanderRadius = 4;
	private Animator m_Animator;
	private IEnumerator m_MovementCoroutine;

	private NavMeshAgent m_NavMeshAgent;
	private SpriteRenderer m_SpriteRenderer;


	private void Awake() {
		m_NavMeshAgent = GetComponent<NavMeshAgent>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_Animator = GetComponent<Animator>();
	}

	private void Start() {
		m_Animator.SetInteger(Speed, 0);
		m_NavMeshAgent.updateUpAxis = false;
		m_NavMeshAgent.updateRotation = false;
		m_MovementCoroutine = NewHeading();
		StartCoroutine(m_MovementCoroutine);
	}


	private void Update() {
		if (m_NavMeshAgent.velocity != Vector3.zero) {
			m_SpriteRenderer.flipX = m_NavMeshAgent.velocity.x < 0;
		}
	}

	private void OnDestroy() {
		StopCoroutine(m_MovementCoroutine);
	}

	private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		var randDirection = Random.insideUnitSphere * dist;
		randDirection += origin;
		return NavMesh.SamplePosition(randDirection, out var navHit, dist, layermask) ? navHit.position : origin;
	}

	private IEnumerator NewHeading() {
		while (true) {
			var destination = RandomNavSphere(transform.position, wanderRadius, -1);
			if (destination != transform.position) {
				m_NavMeshAgent.SetDestination(destination);
			}

			yield return new WaitUntil(() => m_NavMeshAgent.velocity != Vector3.zero);
			m_Animator.SetInteger(Speed, 1);
			yield return new WaitUntil(() => m_NavMeshAgent.velocity == Vector3.zero);
			m_Animator.SetInteger(Speed, 0);
			m_Animator.SetTrigger(Graze);
			yield return new WaitForSecondsRealtime(Random.Range(2.0f, 5.0f));
		}
	}
}