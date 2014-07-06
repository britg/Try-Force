using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour {

	public static GameTime gameTimeInstance;

	public float _speed = 1f;
	public float _minSpeed = 0.2f;
	public float _maxSpeed = 5f;
	public float _speedIncrement = 0.2f;
	public float _currentTime = 0f;
	public float _currentTickProgress = 0f;
	public int _currentTick = 0;

	float _deltaTime { get { return Time.deltaTime*gameTimeInstance._speed; } }

	public static float deltaTime { get { return gameTimeInstance._deltaTime; } }
	public static int currentTick { get { return gameTimeInstance._currentTick; } }
	public static float gameSpeed { get { return gameTimeInstance._speed; } }

	public static void IncreaseSpeed () {
		gameTimeInstance._increaseSpeed();
	}

	public static void DecreaseSpeed () {
		gameTimeInstance._decreaseSpeed();
	}

	void Start () {
		GameTime.gameTimeInstance = this;
	}

	void Update () {
		_currentTime += _deltaTime;
		_currentTickProgress += _deltaTime;	

		if (_currentTickProgress >= 1f) {
			_currentTick ++;
			_currentTickProgress -= 1f;
		}
	}

	void _increaseSpeed () {
		_speed = Mathf.Clamp(_speed + _speedIncrement, _minSpeed, _maxSpeed);
	}

	void _decreaseSpeed () {
		_speed = Mathf.Clamp(_speed - _speedIncrement, _minSpeed, _maxSpeed);
	}


}
