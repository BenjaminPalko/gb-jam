using UnityEngine;

namespace Scripts {
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
	public class FarmerController : MonoBehaviour {
		private static readonly int Property = Animator.StringToHash("throw");
		private static readonly int Aggressive = Animator.StringToHash("aggressive");

		[SerializeField] private float speed = 0.5f;
		[SerializeField] private float fireRate = 0.25f;

		[SerializeField] private GameObject pitchFork;

		[SerializeField] private FarmerTrigger alertTrigger;
		[SerializeField] private FarmerTrigger attackTrigger;
		[SerializeField] private FarmerTrigger chaseTrigger;
		private bool m_Aggravated;
		private Animator m_Animator;
		private float m_Cooldown;
		private PitchforkController m_PitchforkController;

		private SpriteRenderer m_SpriteRenderer;

		private void Awake() {
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
			m_Animator = GetComponent<Animator>();
		}

		private void Update() {
			if (m_Cooldown > 0.0f) m_Cooldown -= Time.deltaTime;
			if (attackTrigger.playerController) {
				Attack(attackTrigger.playerController.transform.position);
			} else if (alertTrigger.playerController && alertTrigger.playerController.immobilize) {
				if (!m_Aggravated) Chase();
				Move(alertTrigger.playerController.transform.position);
			} else if (chaseTrigger.playerController) {
				if (!m_Aggravated) Chase();
				Move(chaseTrigger.playerController.transform.position);
			} else if (m_Aggravated) {
				Calm();
			}
		}

		private void Move(Vector2 destination) {
			var diff = destination - (Vector2)transform.position;
			if (diff != Vector2.zero) m_SpriteRenderer.flipX = diff.x < 0;
			transform.position = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * speed);
		}

		private void Chase() {
			m_Aggravated = true;
			m_Animator.SetBool(Aggressive, m_Aggravated);
		}

		private void Calm() {
			if (m_PitchforkController) Destroy(m_PitchforkController);
			m_PitchforkController = null;
			m_Aggravated = false;
			m_Animator.SetBool(Aggressive, m_Aggravated);
		}

		private void Attack(Vector2 target) {
			if (m_Cooldown > 0 || m_PitchforkController) return;
			m_PitchforkController = Instantiate(pitchFork, transform).GetComponent<PitchforkController>();
			m_Animator.SetTrigger(Property);
			m_PitchforkController.Throw(target);
			m_Cooldown = 1 / fireRate;
		}
	}
}