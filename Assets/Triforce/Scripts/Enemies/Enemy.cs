using UnityEngine;
using System.Collections;

[System.Serializable]
public class Enemy : IDamageReceiver {

	public enum AggroState {
		Idle,
		Aggro,
		InRange,
		Stunned,
		KnockedBack,
		Dead
	}

	public int _hitPoints;
	public int hitPoints { get { return _hitPoints; } }

	public float moveSpeed;
	public float attackPower;
	public float attackDuration;
	public float attackCooldown;
	public float stunDuration;

	public Enemy.AggroState aggroState;
	public bool idle { get { return aggroState == Enemy.AggroState.Idle; } }
	public bool aggro { get { return aggroState == Enemy.AggroState.Aggro; } }
	public bool inRange { get { return aggroState == Enemy.AggroState.InRange; } }
	public bool stunned { get { return aggroState == Enemy.AggroState.Stunned; } }
	public bool knockedBack { get { return aggroState == Enemy.AggroState.KnockedBack; } }
	public bool dead { get { return aggroState == Enemy.AggroState.Dead; } }

	public void EnterAggroState () {
		aggroState = Enemy.AggroState.Aggro;
	}

	public void EnterInRangeState () {
		aggroState = Enemy.AggroState.InRange;
	}

	public void TakeDamageFrom (IProjectile projectile) {
		Debug.Log ("Enemy model: taking damage from projectile " + projectile);
		Die();
	}

	public void TakeDamageFrom (IWeapon weapon) {
		Debug.Log ("Enemy model: taking damage from weapon " + weapon);
		Die();
	}

	public void TakeDamageFrom (ISpell spell) {
		Debug.Log ("Enemy model: taking damage from spell " + spell);
		Die();
	}

	void KnockBack () {
		aggroState = Enemy.AggroState.KnockedBack;
	}

	public void AfterKnockBack () {
		aggroState = Enemy.AggroState.Stunned;
	}

	void Die () {
		aggroState = Enemy.AggroState.Dead;
	}
}
