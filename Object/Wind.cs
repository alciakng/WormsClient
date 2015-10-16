using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {

	//바람 초기점
	Vector2 WindPos;

	//바람세기
	public float WindFos=2f;

	//떨어지는 나뭇잎.
	public GameObject BlowingLeaves;


	// Use this for initialization
	void Start () {
		WindPos = transform.position;

		foreach(Transform Leave in BlowingLeaves.transform){
			Leave.GetComponent<ParticleRenderer>().sortingLayerName="object";
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Collider2D[] colliders = Physics2D.OverlapAreaAll(new Vector2(-40f,80f),new Vector2(170f,-20f));

		foreach (Collider2D collider in colliders) {
			string tag = collider.gameObject.tag;
			
			if (tag =="Bullet" || tag == "Hbomb" ){
				print ("됏어");
				collider.gameObject.GetComponent<Rigidbody2D> ().AddForce (Vector2.right*WindFos);
			}
		}
	}


	//나뭇잎 방향 설정
	public void setWindFos(float _WindFos){
		this.WindFos = _WindFos;

		foreach(Transform Leave in BlowingLeaves.transform){
			Leave.GetComponent<ParticleAnimator>().force = new Vector3(_WindFos,-4,0);
		}
	}

}
