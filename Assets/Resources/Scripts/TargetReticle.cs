using System.Collections.Generic;
using UnityEngine;

namespace Scripts {
	[RequireComponent(typeof(Collider2D))]
	public class TargetReticle : MonoBehaviour {
		public readonly List<Abductable> Abductables = new();

		private void OnTriggerEnter2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			if (Abductables.Contains(abductable)) return;
			Abductables.Add(abductable);
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (!other.TryGetComponent<Abductable>(out var abductable)) return;
			if (!Abductables.Contains(abductable)) return;
			Abductables.Remove(abductable);
		}
	}
}