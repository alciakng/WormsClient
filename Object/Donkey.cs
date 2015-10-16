using UnityEngine;
using System.Collections;
using SocketIO;

public class Donkey : MonoBehaviour {
	
	public Rigidbody2D rb;


	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//턴 컨트롤러
	private TurnController tc;


	//지형파괴 콜라이더
	public CircleCollider2D destructionCircle;

	//파티클
	public GameObject explosionParticle;

	//타임
	private float time;

	
	//폭파사운드
	public AudioClip explodeSound;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();

		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
		Socket = sc.Socket;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		time += Time.fixedDeltaTime;

		if (time > 8f) {
			if(sc.clientId == tc.currentTurn){
				tc.Invoke("reqTurnChange",2f);
			}

			Destroy (this.gameObject);

		}
	}




	
	void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {

			AudioSource.PlayClipAtPoint (explodeSound,transform.position);

			//주변 물체에게 데미지 가하기 
			Vector2 explosionPos = transform.position;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,5f);
			
			foreach (Collider2D collider in colliders) {
				if(collider.gameObject.tag=="Player"){
					if(sc.clientId == tc.currentTurn){
						Socket.Emit ("req_take_damage",new Json("damage",4,"socketid",collider.gameObject.GetComponent<Player>().socketid).json);
					}
				}
			}

			GameObject particle = Instantiate (explosionParticle,transform.position,Quaternion.identity) as GameObject;
			Destroy(particle,2f);


			if(sc.clientId == tc.currentTurn) Socket.Emit ("req_destroy_ground",new Json("ground",coll.gameObject.name,"centerX",destructionCircle.bounds.center.x,"centerY",destructionCircle.bounds.center.y,"sizeX",destructionCircle.bounds.size.x).json);
			//coll.gameObject.GetComponent<GroundController>().DestroyGround(this.GetComponent<CircleCollider2D>());
		}
	}
}
