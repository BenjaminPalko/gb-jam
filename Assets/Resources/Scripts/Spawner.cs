using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;

namespace Resources.Scripts {
	[RequireComponent(typeof(Collider2D))]
	public class Spawner : MonoBehaviour {
		[SerializeField] private int cows;
		[SerializeField] private int farmers;

		[SerializeField] private float visionRadius;
		private CircleCollider2D m_Collider2D;

		private GameObject m_CowPrefab;
		private List<CowBehaviour> m_Cows = new();
		private GameObject m_FarmerPrefab;
		private List<FarmerController> m_Farmers = new();

		public static Spawner instance { get; private set; }

		private void Awake() {
			if (instance) {
				Debug.LogError("Only one instance of Spawner is allowed!");
				return;
			}

			instance = this;
			m_Collider2D = GetComponent<CircleCollider2D>();
			m_CowPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Units/Cow");
			m_FarmerPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Units/Farmer");
		}

		private void Start() {
			SpawnCows();
			SpawnFarmers();
		}

		private void OnDestroy() {
			DespawnAll();
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (other.TryGetComponent<CowBehaviour>(out var cowBehaviour)) Despawn(cowBehaviour);
			if (other.TryGetComponent<FarmerController>(out var farmerController)) Despawn(farmerController);
		}


		private void SpawnCows() {
			var cowDiff = cows - m_Cows.Count;
			for (var c = 0; c < cowDiff; c++) {
				var cow = Instantiate(m_CowPrefab,
					transform.position + (Vector3)RandomVector(visionRadius, m_Collider2D.radius), Quaternion.identity);
				if (cow.TryGetComponent<CowBehaviour>(out var cowBehaviour)) m_Cows.Add(cowBehaviour);
			}
		}

		private void SpawnFarmers() {
			var farmerDiff = farmers - m_Farmers.Count;
			for (var f = 0; f < farmerDiff; f++) {
				var farmer = Instantiate(m_FarmerPrefab,
					transform.position + (Vector3)RandomVector(visionRadius, m_Collider2D.radius), Quaternion.identity);
				if (farmer.TryGetComponent<FarmerController>(out var farmerController)) m_Farmers.Add(farmerController);
			}
		}

		public void Despawn(CowBehaviour cow) {
			m_Cows.Remove(cow);
			Destroy(cow.gameObject);
			SpawnCows();
		}

		public void Despawn(FarmerController farmer) {
			m_Farmers.Remove(farmer);
			Destroy(farmer.gameObject);
			SpawnFarmers();
		}

		public void DespawnAll() {
			foreach (var cowBehaviour in m_Cows.ToList().Where(cowBehaviour => cowBehaviour != null)) {
				m_Cows.Remove(cowBehaviour);
				Destroy(cowBehaviour.gameObject);
			}

			foreach (var farmerController in m_Farmers.ToList().Where(farmerController => farmerController != null)) {
				m_Farmers.Remove(farmerController);
				Destroy(farmerController.gameObject);
			}
		}

		private static Vector2 RandomVector(float innerRadius, float outerRadius) {
			var direction = Random.insideUnitCircle;
			return direction.normalized * innerRadius + direction * (outerRadius - innerRadius);
		}
	}
}