using UnityEngine;
using System.Collections;

[System.Serializable]
public class Arrow : IProjectile {

	public int _damage;
	public float _lifetime;
	public float _speed;

	public int damage { get { return _damage; } }
	public float speed { get { return _speed; } }
	public float lifetime { get { return _lifetime; } }

}
