using UnityEngine;
using System.Collections;

public class OilDrumController : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("왜안돼");
		if (other.tag == "bat") {
			Debug.Log ("드럼통이 배트에 맞았다");
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		Debug.Log ("충돌!");
	}

}
