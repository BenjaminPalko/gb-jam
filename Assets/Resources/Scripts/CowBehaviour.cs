using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CowBehaviour : MonoBehaviour
{

	NavMeshAgent agent;
	SpriteRenderer spriteRenderer; 
	Vector3 destination;
	public int radius = 4;

	public float directionChangeInterval = 0.1f;

	float minimumSpeed = 0.35f;

	
	

	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		agent.updateUpAxis = false;
		agent.updateRotation = false;
		StartCoroutine(NewHeading());
		
	}
	

	void Update ()
	{
	
	
		if(agent.velocity.magnitude > minimumSpeed){ 
			spriteRenderer.flipX = agent.velocity.x < 0;
		}

		if(destination != Vector3.positiveInfinity){
			agent.SetDestination(destination);
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
		for(;;) {
			CalculateNewDirection();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}

	void CalculateNewDirection() {
		if(agent.velocity.magnitude < minimumSpeed){
			destination = RandomNavSphere(transform.position, radius, -1);
		}
	}

	void OnDestroy() {
		StopCoroutine(NewHeading());
	}

}
