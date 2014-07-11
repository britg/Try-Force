using UnityEngine;
using System.Collections;

[System.Serializable]
public class Enemy {

	public enum AggroState {
		Idle,
		Aggro
	}

	public int hitPoints;
	public float moveSpeed;
	public float attackPower;
	public float attackDuration;
	public float attackCooldown;

	public Enemy.AggroState aggroState;

	public bool aggro { get { return aggroState == Enemy.AggroState.Aggro; } }
	public bool idle { get { return aggroState == Enemy.AggroState.Idle; } }

	public void EnterAggroState () {
		aggroState = Enemy.AggroState.Aggro;
	}

}
