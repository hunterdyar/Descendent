using System;
using System.Collections.Generic;
using DefaultNamespace;
using NUnit.Framework.Internal.Filters;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

namespace Camera
{
	public class CameraManager : MonoBehaviour
	{
		[Header("Config")] public CinemachineCamera _roomPrefab;
		
		private Dictionary<RoomData, CinemachineCamera> _camMap = new Dictionary<RoomData, CinemachineCamera>();
		private void Awake()
		{
		}

		private void OnEnable()
		{
			Player.OnPlayerPositionChange += OnPlayerPositionChange;
		}

		private void OnDisable()
		{
			Player.OnPlayerPositionChange -= OnPlayerPositionChange;
		}

		void OnPlayerPositionChange(Vector2Int pos)
		{
			foreach (var kvp in _camMap)
			{
				if (kvp.Key.Contains(pos))
				{
					kvp.Value.Priority = 10;
				}
				else
				{
					kvp.Value.Priority = 0;
				}
			}
		}

		

		public void SetCamera(Grid grid, RuntimeLevel runtimeLevel)
		{
			Clear();
			var player = runtimeLevel.GetPlayer();
			foreach (var rom in runtimeLevel.Rooms)
			{
				var cam = Instantiate(_roomPrefab, transform);
				_camMap.Add(rom, cam);
				cam.Follow = player.transform;
				var centerGridPos = (Vector3Int)(rom.Position + rom.Size / 2);
				var center = grid.CellToWorld(centerGridPos) + grid.cellSize / 2f; //left to center offset
				var dsizeint = (Vector3Int)rom.Size;
				var dsize = grid.CellToWorld(dsizeint) + Vector3.one; //one is just padding.
				cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);
				float s = dsize.y > dsize.x ? dsize.y : dsize.x;
				s = s / cam.Lens.Aspect;
				cam.Lens.OrthographicSize = s / 2;
				//
			}

			var p = runtimeLevel.GetPlayer();
			OnPlayerPositionChange(p.CurrentPos);
		}

		private void Clear()
		{
			foreach (var child in _camMap.Values)
			{
				Destroy(child.gameObject);
			}
			_camMap.Clear();
		}
	}
}