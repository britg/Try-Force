using UnityEngine;
using System.Collections;

[System.Serializable]
public class Fireball : ISpell {

	public int _damage;
	public float _lifetime;
	public float _force;

	public int damage { get { return _damage; } }
	public float force { get { return _force; } }
	public float lifetime { get { return _lifetime; } }

}
