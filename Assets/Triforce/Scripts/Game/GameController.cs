using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public enum InputMode {
		Mouse,
		Controller,
		Touch
	}

	public static InputMode inputMode = InputMode.Mouse;

	GameObject _gameObj;
	protected GameObject gameObj {
		get {
			if (_gameObj == null) {
				_gameObj = GameObject.Find("Game");
			}
			return _gameObj;
		}
	}

	GameObject _playerObj;
	protected GameObject playerObj {
		get {
			if (_playerObj == null) {
				_playerObj = GameObject.Find("Player");
			}
			return _playerObj;
		}
	}

	PlayerController _playerController;
	protected PlayerController playerController {
		get {
			if (_playerController == null) {
				_playerController = playerObj.GetComponent<PlayerController>();
			}
			return _playerController;
		}
	}

	Player _player;
	protected Player player {
		get {
			if (_player == null) {
				_player = playerController.player;
			}
			return _player;
		}
	}

	protected Vector2 playerDirection {
		get {
			return (playerObj.transform.position - transform.position).normalized.XY();
		}
	}


	GameObject _aimObj;
	protected GameObject aimObj {
		get {
			if (_aimObj == null) {
				_aimObj = GameObject.Find ("Aim");
			}
			return _aimObj;
		}
	}

	AimController _aimController;
	protected AimController aimController {
		get {
			if (_aimController == null) {
				_aimController = aimObj.GetComponent<AimController>();
			}
			return _aimController;
		}
	}

	IInputController _currentInputController;
	protected IInputController currentInputController {
		get {
			if (_currentInputController == null) {
				if (inputMode == InputMode.Mouse) {
					_currentInputController = gameObj.GetComponent<MouseInputController>();
				}
			}
			return _currentInputController;
		}
	}

}
