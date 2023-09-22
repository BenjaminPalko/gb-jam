using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CowBehaviour : MonoBehaviour
{

	NavMeshAgent agent;
	SpriteRenderer spriteRenderer; 
	Vector3 destination;
	Animator animator;
	IEnumerator movementCoroutine;

	public int radius = 4;

	private float directionChangeInterval = 0.5f;

	float minimumSpeed = 0.5f;

	
	
	void Awake() {
		agent = GetComponent<NavMeshAgent>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	void Start ()
	{
		animator.SetInteger("speed", 0);
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		movementCoroutine = NewHeading();
		StartCoroutine(movementCoroutine);
	}
	

	void Update ()
	{
		if(agent.velocity.magnitude > minimumSpeed){ 
			spriteRenderer.flipX = agent.velocity.x < 0;
		}	
		
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.onUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    
	IEnumerator NewHeading (){
		while(true) {
			CalculateNewDirection();
			animator.SetInteger("speed", 1);
			agent.SetDestination(destination);
			directionChangeInterval = Random.Range(2f, 5f);
			Debug.Log(animator.GetInteger("speed"));
			Debug.Log(animator.GetInteger("speed"));

			yield return new WaitUntil(() => agent.velocity.magnitude == 0);
			
			animator.SetInteger("speed", 0);
			yield return new WaitForSeconds(directionChangeInterval) ;
		}
	}

	void CalculateNewDirection() {
		if(agent.velocity.magnitude < minimumSpeed){
			destination = RandomNavSphere(transform.position, radius, -1);
		}
	}

	void OnDestroy() {
		StopCoroutine(movementCoroutine);
	}

}
