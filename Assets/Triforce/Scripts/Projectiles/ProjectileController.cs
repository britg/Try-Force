using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public IProjectile projectile;
	public iTween.EaseType easetype;
	
	bool ready = false;
	public Vector2 destination;
	
	void Start () {
	}
	
	void Update () {
		if (ready) {
			Fire();
		}
	}
	
	void Fire () {
		Vector2 direction = (destination - transform.position.XY()).normalized;
		Vector2 endpoint = direction * projectile.speed * projectile.lifetime;
		
		transform.LookAt2D(endpoint);
		
		iTween.MoveTo(gameObject, iTween.Hash("position", endpoint.to3(), 
		                                      "time", projectile.lifetime,
		                                      "axis", "z",
		                                      "easetype", easetype,
		                                      "oncomplete", "DestroyOnComplete"
		                                      ));
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
