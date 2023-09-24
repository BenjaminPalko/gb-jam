using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scripts {
	public class MapController : MonoBehaviour {
		[SerializeField] private NavMeshSurface navMeshSurface;
		[SerializeField] private TileBase tile1;
		[SerializeField] private TileBase tile2;
		private Vector3Int m_CurrentCell;

		private Grid m_Grid;
		private PlayerController m_PlayerController;

		[Range(0, 3)] private int m_Quadrant;

		private Tilemap[] m_Tilemaps;

		private void Awake() {
			m_Grid = GetComponent<Grid>();
			m_Tilemaps = GetComponentsInChildren<Tilemap>();
			m_PlayerController = FindObjectOfType<PlayerController>();
		}

		private void Start() {
			var playerPosition = m_PlayerController.transform.position;
			m_CurrentCell = m_Tilemaps[0].WorldToCell(playerPosition);

			var currentQuadrant = CalculateCurrentQuadrant(playerPosition, m_CurrentCell);
			CreateNavmesh(currentQuadrant, m_CurrentCell);
			m_Quadrant = currentQuadrant;
		}

		private void Update() {
			var playerPosition = m_PlayerController.transform.position;
			var cellLocation = m_Tilemaps[0].WorldToCell(playerPosition);

			var currentQuadrant = CalculateCurrentQuadrant(playerPosition, cellLocation);

			if (cellLocation != m_CurrentCell) {
				m_CurrentCell = cellLocation;
				m_Quadrant = currentQuadrant;
				return;
			}

			if (currentQuadrant == m_Quadrant) return;

			UpdateNavmesh(currentQuadrant, cellLocation);
			m_Quadrant = currentQuadrant;
		}

		private int CalculateCurrentQuadrant(Vector3 playerPosition, Vector3Int cellLocation) {
			var cellCenter = m_Tilemaps[0].GetCellCenterWorld(cellLocation);

			var playerLocalPosition = playerPosition - cellCenter;

			return playerLocalPosition switch {
				{ x: < 0, y: > 0 } => 0,
				{ x: > 0, y: > 0 } => 1,
				{ x: < 0, y: < 0 } => 2,
				{ x: > 0, y: < 0 } => 3,
				_ => m_Quadrant
			};
		}

		private void CreateNavmesh(int initialQuadrant, Vector3Int cellLocation) {
			m_Tilemaps[0].SetTile(cellLocation, tile1);
			m_Tilemaps[1].SetTile(cellLocation, tile2);
			if (initialQuadrant == 0) {
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.left, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.up, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.left + Vector3Int.up, tile1);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.left, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.up, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.left + Vector3Int.up, tile2);
			} else if (initialQuadrant == 1) {
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.right, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.up, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.right + Vector3Int.up, tile1);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.right, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.up, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.right + Vector3Int.up, tile2);
			} else if (initialQuadrant == 2) {
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.left, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.down, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.left + Vector3Int.down, tile1);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.left, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.down, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.left + Vector3Int.down, tile2);
			} else if (initialQuadrant == 3) {
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.right, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.down, tile1);
				m_Tilemaps[0].SetTile(cellLocation + Vector3Int.right + Vector3Int.down, tile1);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.right, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.down, tile2);
				m_Tilemaps[1].SetTile(cellLocation + Vector3Int.right + Vector3Int.down, tile2);
			}

			navMeshSurface.BuildNavMeshAsync();
		}

		private void UpdateNavmesh(int newQuadrant, Vector3Int cellLocation) {
			foreach (var tilemap in m_Tilemaps) {
				if (newQuadrant is 0 or 2 && m_Quadrant is 1 or 3) {
					var tile = tilemap.GetTile(cellLocation + Vector3Int.right);
					tilemap.SetTile(cellLocation + Vector3Int.left, tile);
					tilemap.SetTile(cellLocation + Vector3Int.right, null);
				} else if (newQuadrant is 1 or 3 && m_Quadrant is 0 or 2) {
					var tile = tilemap.GetTile(cellLocation + Vector3Int.left);
					tilemap.SetTile(cellLocation + Vector3Int.right, tile);
					tilemap.SetTile(cellLocation + Vector3Int.left, null);
				}

				if (newQuadrant is 2 or 3 && m_Quadrant is 0 or 1) {
					var tile = tilemap.GetTile(cellLocation + Vector3Int.up);
					tilemap.SetTile(cellLocation + Vector3Int.down, tile);
					tilemap.SetTile(cellLocation + Vector3Int.up, null);
				} else if (newQuadrant is 0 or 1 && m_Quadrant is 2 or 3) {
					var tile = tilemap.GetTile(cellLocation + Vector3Int.down);
					tilemap.SetTile(cellLocation + Vector3Int.up, tile);
					tilemap.SetTile(cellLocation + Vector3Int.down, null);
				}

				var formerCornerPosition = CalculateCornerPositionFromQuadrant(cellLocation, m_Quadrant);
				var newCornerPosition = CalculateCornerPositionFromQuadrant(cellLocation, newQuadrant);
				var cornerTile = tilemap.GetTile(formerCornerPosition);
				tilemap.SetTile(newCornerPosition, cornerTile);
				tilemap.SetTile(formerCornerPosition, null);
			}

			navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
		}

		private static Vector3Int CalculateCornerPositionFromQuadrant(Vector3Int cellPosition, int quadrant) {
			return quadrant switch {
				0 => cellPosition + Vector3Int.left + Vector3Int.up,
				1 => cellPosition + Vector3Int.right + Vector3Int.up,
				2 => cellPosition + Vector3Int.left + Vector3Int.down,
				3 => cellPosition + Vector3Int.right + Vector3Int.down,
				_ => cellPosition
			};
		}
	}
}