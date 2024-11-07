using System;
using DefaultNamespace;
using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
	private Player _player;

	private void Awake()
	{
		_player = GetComponent<Player>();
	}

	private void Update()
	{
		KeyboardInputTick();
	}

	private void KeyboardInputTick()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			_player.Move(Vector2Int.up);
		}else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			_player.Move(Vector2Int.down);
		}else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			_player.Move(Vector2Int.right);
		}else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_player.Move(Vector2Int.left);
		}
	}
}
