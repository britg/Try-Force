using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {
	
	public Arrow arrow;
	
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
		Vector2 endpoint = direction * arrow.speed * arrow.lifetime;
		
		transform.LookAt2D(endpoint);
		
		iTween.MoveTo(gameObject, iTween.Hash("position", endpoint.to3(), 
		                                      "time", arrow.lifetime,
		                                      "axis", "z",
		                                      "easetype", iTween.EaseType.linear,
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