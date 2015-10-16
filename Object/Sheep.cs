using UnityEngine;
using System.Collections;
using SocketIO;

public class Sheep : MonoBehaviour {

	
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;

	//턴 컨트롤러
	private TurnController tc;

	//지형파괴 콜라이더
	public CircleCollider2D destructionCircle;

	public Rigidbody2D rb;
	public float dir;
	public float speed=100f;

	//파티클 
	public GameObject explosionParticle;

	//지면 체크 
	public bool isGround;


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
		rb.velocity = new Vector2(dir* speed * Time.fixedDeltaTime,rb.velocity.y);
	}

	//플레이어나 땅에 enter or exit시에 양 포지션 동기화.
	public void syncSheep(JSONObject position){
		this.transform.position = new Vector3 (position.GetField ("sheepX").f, position.GetField ("sheepY").f, 0);
	}
	
	public void Jump(){
		if(isGround) rb.AddForce (Vector2.up*300f);
	}

	public void SetDir(float _dir){

		dir = _dir;

		if (dir > 0) {
			this.transform.localScale = new Vector3 (Mathf.Abs (this.transform.localScale.x), this.transform.localScale.y, 0);
		} else {
			this.transform.localScale = new Vector3 (-Mathf.Abs (this.transform.localScale.x), this.transform.localScale.y, 0);
		}
	}

	public void Explode(){

		AudioSource.PlayClipAtPoint (explodeSound,transform.position);

		//주변 물체에게 데미지 가하기 
		Vector2 explosionPos = transform.position;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,1f);


		foreach (Collider2D collider in colliders) {
			if(collider.gameObject.tag=="Player"){
				if(sc.clientId == tc.currentTurn){
					Socket.Emit ("req_take_damage",new Json("damage",20,"socketid",collider.gameObject.GetComponent<Player>().socketid).json);
				}
				//collider.gameObject.GetComponent<Player>().takeDamage(20);
			}
			if(collider.gameObject.tag=="Ground"){
				groundName = collider.gameObject.name;
			}
		}

		if(sc.clientId == tc.currentTurn) Socket.Emit ("req_destroy_ground",new Json("ground",groundName,"centerX",destructionCircle.bounds.center.x,"centerY",destructionCircle.bounds.center.y,"sizeX",destructionCircle.bounds.size.x).json);



		//ui 버튼 컨트롤을 다시 플레이어로 변경
		GameObject.Find("RightButton").GetComponent<RightButtonController>().currentControl = "player";
		GameObject.Find ("LeftButton").GetComponent<LeftButtonController> ().currentControl = "player";
		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "player";

		GameObject particle = Instantiate (explosionParticle,transform.position,Quaternion.identity) as GameObject;
		Destroy(particle,2f);

		if(sc.clientId == tc.currentTurn){
			tc.Invoke("reqTurnChange",2f);
		}

		
		//게임오브젝트 삭제.
		Destroy (this.gameObject);
	}


	public void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround=true;
			//땅에 접촉했을시에 동기화.
		}
		Socket.Emit ("req_sync_sheep", new Json ("name",this.name,"sheepX", transform.position.x, "sheepY", transform.position.y).json);
	}
	
	public void OnCollisionStay2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround=true;
		}
	}
	
	public void OnCollisionExit2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround=false;
		}
		Socket.Emit ("req_sync_sheep", new Json ("name",this.name,"sheepX", transform.position.x, "sheepY", transform.position.y).json);
	}




}
