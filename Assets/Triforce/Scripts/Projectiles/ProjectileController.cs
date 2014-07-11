using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public IProjectile projectile;
	public iTween.EaseType easetype;

	bool ready = false;
	public Vector2 destination;

	GameObject _body;
	GameObject body {
		get {
			if (_body == null) {
				_body = transform.FindChild("body").gameObject;
			}
			return _body;
		}
	}

	void Start () {
	}
	
	void Update () {
		if (ready) {
			Fire();
		}
	}
	
	void Fire () {
		Vector2 direction = (destination - transform.position.XY()).normalized;
		Vector2 endpoint = direction * projectile.force * projectile.lifetime;
		
		transform.LookAt2D(endpoint);
		body.rigidbody2D.AddForce(projectile.force * direction);
		ready = false;
	}
	
	public void SetDestination (Vector2 _destination) {
		destination = _destination;
		ready = true;
	}
	
	void DestroyOnComplete () {
		Destroy(gameObject);
	}
}
