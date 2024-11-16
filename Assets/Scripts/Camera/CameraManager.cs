using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

namespace Camera
{
	public class CameraManager : MonoBehaviour
	{
		private CinemachineMixingCamera _camera;
		[Header("Config")] public CinemachineCamera _roomPrefab;
		
		private Dictionary<RoomData, CinemachineCamera> _camMap = new Dictionary<RoomData, CinemachineCamera>();
		private void Awake()
		{
			_camera = GetComponent<CinemachineMixingCamera>();
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
			var r = GetRoom(pos);
			if (r != null)
			{
				var c = _camMap[r];
				for (int i = 0; i < transform.childCount; i++)
				{
					_camera.SetWeight(i, 0);
				}

				_camera.SetWeight(c, 1);
			}
		}

		RoomData GetRoom(Vector2Int pos)
		{
			foreach (var key in _camMap.Keys)
			{
				if (key.Contains(pos))
				{
					return key;
				}
			}

			return null;
		}

		public void SetCamera(Grid grid, RuntimeLevel runtimeLevel)
		{
			Clear();
			foreach (var rom in runtimeLevel.Rooms)
			{
				var cam = Instantiate(_roomPrefab, _camera.transform);
				_camMap.Add(rom,cam);
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
			_camera.Follow = p.transform;
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