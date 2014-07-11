using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sword : IWeapon{

	public int _damage;
	public float swipeDuration;

	public Vector3 arcStart;
	public Vector3 arcEnd;

	public int damage {
		get { return _damage; }
	}

}
