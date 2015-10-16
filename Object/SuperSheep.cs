using UnityEngine;
using System.Collections;
using SocketIO;

public class SuperSheep : MonoBehaviour {

	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	
	//턴 컨트롤러
	private TurnController tc;
	
	//지형파괴 콜라이더
	public CircleCollider2D destructionCircle;

	//Rigidbody2D
	public Rigidbody2D rb;
	public float dir;
	public float speed;


	//방향벡터
	Vector2 dirVec;
	
	//파티클 
	public GameObject explosionParticle;
	
	//지면 체크 
	public bool isGround;

	//초기 각도
	public float initialAngle;
	
	
	//폭파지점 그라운드 이름
	public string groundName;


	//폭파사운드
	public AudioClip explodeSound;

	// Use this for initialization
	void Start () {
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;

		rb = GetComponent<Rigidbody2D> ();
		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
	}

	void FixedUpdate () {
		rb.velocity = dirVec* speed;
	}


	public void SetDir(Vector2 dir){
		dirVec = dir;
	}

	public void rotationRight(){

		initialAngle -= 0.1f;
		dirVec = new Vector2 (Mathf.Cos (initialAngle), Mathf.Sin (initialAngle));
		dirVec.Normalize ();

		float angle = Mathf.Asin (dirVec.y)*Mathf.Rad2Deg;

		if( dirVec.x < 0f ){
			angle = 180 - angle;
		}
		this.transform.localEulerAngles = new Vector3(0,0,angle-90);

	}

	public void rotationLeft(){

		initialAngle += 0.1f;
		dirVec = new Vector2 (Mathf.Cos (initialAngle), Mathf.Sin (initialAngle));
		dirVec.Normalize ();

		float angle = Mathf.Asin (dirVec.y)*Mathf.Rad2Deg;
		
		if( dirVec.x < 0f ){
			angle = 180 - angle;
		}
		this.transform.localEulerAngles = new Vector3(0,0,angle-90);
	}

	public void Explode(){

		AudioSource.PlayClipAtPoint (explodeSound, transform.position);

		//주변 물체에게 데미지 가하기 
		Vector2 explosionPos = transform.position;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,1.5f);
		
		foreach (Collider2D collider in colliders) {
			if(collider.gameObject.tag=="Player"){
				if(sc.clientId == tc.currentTurn){
					Socket.Emit ("req_take_damage",new Json("damage",14,"socketid",collider.gameObject.GetComponent<Player>().socketid).json);
				}
			}
			if(collider.gameObject.tag=="Ground"){
				groundName = collider.gameObject.name;
			}
		}
		

		
		//ui 버튼 컨트롤을 다시 플레이어로 변경
		GameObject.Find("RightButton").GetComponent<RightButtonController>().currentControl = "player";
		GameObject.Find ("LeftButton").GetComponent<LeftButtonController> ().currentControl = "player";
		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "player";
		
		
		GameObject particle = Instantiate (explosionParticle,transform.position,Quaternion.identity) as GameObject;
		Destroy(particle,2f);
		
		if(sc.clientId == tc.currentTurn) Socket.Emit ("req_destroy_ground",new Json("ground",groundName,"centerX",destructionCircle.bounds.center.x,"centerY",destructionCircle.bounds.center.y,"sizeX",destructionCircle.bounds.size.x).json);
		//coll.gameObject.GetComponent<GroundController>().DestroyGround(this.GetComponent<CircleCollider2D>());
		
		if(sc.clientId == tc.currentTurn){
			tc.Invoke("reqTurnChange",2f);
		}
		
		Destroy(this.gameObject);

	}


	public void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			Explode();
		}
	}
	


}
