using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Rope : MonoBehaviour {

	//로프 초기발사 위치
	public Transform ropeInitialTransform;

	//로프 초기 그리는 위치 
	public Transform bodyTransform;

	//line Renderer
	public LineRenderer line;

	//hinge joint
	public HingeJoint2D hinge;

	//player hinge joint
	public HingeJoint2D playerHinge;


	public bool isLine=false;


	//hit position GameObject
	public GameObject hitPosition;


	//box collider
	public BoxCollider2D boxCollider;


	//rope prefab
	public GameObject ropePrefab;

	//child rope
	public GameObject childRope;

	//rope Count
	public int ropeCount=1;


	//bool
	public bool isGround;



	// Use this for initialization
	void Awake () {
		line.SetColors(Color.white,Color.white);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		 line.SetPosition(ropeCount,new Vector3(this.GetComponentInParent<Transform>().position.x,this.GetComponentInParent<Transform>().position.y,-10));

		 boxCollider.offset = new Vector2(hinge.anchor.x/2,0f);
		 boxCollider.size = new Vector2(Mathf.Abs(hinge.anchor.x),0.1f);


	}
	
	public void shootRope(JSONObject rope){

		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "rope";

		hinge.enabled = true;
		line.enabled = true;
		playerHinge.enabled = true;


		RaycastHit2D hit = Physics2D.Raycast(ropeInitialTransform.position,new Vector2(rope.GetField("shootVectorX").f,rope.GetField("shootVectorY").f));

		if(hit.collider.tag == "Ground"){

			line.SetVertexCount(2);
			line.SetPosition(0,new Vector3(hit.point.x,hit.point.y,-10));
			line.SetPosition(1,bodyTransform.position);

			hinge.connectedBody = hit.collider.GetComponent<Rigidbody2D>();

			//접착지점에 게임 오브젝트 생성.
			hitPosition.transform.position = hit.point;
			hitPosition.transform.SetParent(hit.collider.transform);

			hinge.connectedAnchor = hitPosition.transform.localPosition;
			hinge.anchor = new Vector2(new Vector2(bodyTransform.position.x-hit.point.x,bodyTransform.position.y-hit.point.y).magnitude,0);
			//hinge.anchor = new Vector2(hit.point.x - bodyTransform.position.x,hit.point.y-bodyTransform.position.y);

			playerHinge.enabled =true;
			playerHinge.connectedBody = this.GetComponent<Rigidbody2D>();

		}
	}


	public void ropeJump(){

		hinge.enabled = false;
		line.enabled = false;
		playerHinge.enabled = false;

		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "player";
	}

	public void ropeDown(){
		hinge.anchor = new Vector2(hinge.anchor.x+0.1f,0);
	}

	public void ropeUp(){
		hinge.anchor = new Vector2(hinge.anchor.x-0.1f,0);
	}

	/*
	void OnCollisionEnter2D(Collision2D coll){
		if(coll.gameObject.tag=="Ground"){
			isGround =true;

			ropeCount++;
			line.SetVertexCount(ropeCount+1);
			line.SetPosition(ropeCount,coll.contacts[0].point);

			hinge.connectedBody = coll.gameObject.GetComponent<Rigidbody2D>();
			hitPosition.transform.position = coll.contacts[0].point;
			hitPosition.transform.SetParent(coll.gameObject.transform);

			hinge.connectedAnchor = hitPosition.transform.localPosition;
		}
	}

	*/



   /*
	void OnCollisionExit2D(Collision2D coll){
		isGround = false;
		//Destroy(childRope);
	}
	*/
   

}
