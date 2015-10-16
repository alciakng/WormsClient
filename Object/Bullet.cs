using UnityEngine;
using System.Collections;
using SocketIO;


public class Bullet : MonoBehaviour {

	protected Rigidbody2D rb; 
	public Transform bulletSpriteTransform; 

	public bool updateAngle = true; 

	protected GameObject bulletSmoke;

	public CircleCollider2D destructionCircle;
	public GroundController groundController;
	
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;


	//턴 컨트롤러
	private TurnController tc;


	//파티클
	public GameObject explosionParticle;

	//폭파사운드
	public AudioClip explodeSound;

	//발사로부터 시간
	private float time;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();

		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();

		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}
	
	// Update is called once per frame
	void Update () {
		if( updateAngle ){
			Vector2 dir = new Vector2(rb.velocity.x, rb.velocity.y);

            dir.Normalize();			
			float angle = Mathf.Asin (dir.y)*Mathf.Rad2Deg;
			if( dir.x < 0f ){
				angle = 180 - angle;
			}
			bulletSpriteTransform.localEulerAngles = new Vector3(0f, 0f, angle+45f);
		}

		time += Time.fixedDeltaTime;

		if (time > 8f) {
			if(sc.clientId == tc.currentTurn){
				tc.Invoke("reqTurnChange",2f);
			}
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D( Collision2D coll ){
		if( coll.collider.tag == "Ground" ){	

			AudioSource.PlayClipAtPoint (explodeSound,transform.position);

			//주변 물체에게 데미지 가하기 
			Vector2 explosionPos = transform.position;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,2f);
			
			foreach (Collider2D collider in colliders) {
				if(collider.gameObject.tag=="Player"){
					if(sc.clientId == tc.currentTurn){
						Socket.Emit ("req_take_damage",new Json("damage",20,"socketid",collider.gameObject.GetComponent<Player>().socketid).json);
					}
				}
			}

			updateAngle = false;
			GroundController gc =coll.collider.GetComponent<GroundController>();
			if(sc.clientId == tc.currentTurn) Socket.Emit ("req_destroy_ground",new Json("ground",coll.collider.name,"centerX",destructionCircle.bounds.center.x,"centerY",destructionCircle.bounds.center.y,"sizeX",destructionCircle.bounds.size.x).json);
			//gc.DestroyGround( destructionCircle );


			GameObject particle = Instantiate (explosionParticle,transform.position,Quaternion.identity) as GameObject;
			Destroy(particle,2f);

			if(sc.clientId == tc.currentTurn){
				tc.Invoke("reqTurnChange",2f);
			}

			Destroy(this.gameObject);
		}
	}


}
