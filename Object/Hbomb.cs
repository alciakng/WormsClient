using UnityEngine;
using System.Collections;
using SocketIO;

public class Hbomb : MonoBehaviour {


	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;

	//턴 컨트롤러
	private TurnController tc;

	//지형파괴 콜라이더
	public CircleCollider2D destructionCircle;

	public float radius = 4.0f;
	public float power = 10.0f;


	public AudioClip HolySound;
	//폭파사운드
	public AudioClip explodeSound;

	public void Start(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;

		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
	}

	//파티클
	public GameObject explosionParticle;

	IEnumerator OnCollisionEnter2D(Collision2D coll){
		if(coll.gameObject.tag=="Ground"){
			yield return new WaitForSeconds(3f);

			AudioSource.PlayClipAtPoint (HolySound,transform.position);

			yield return new WaitForSeconds(2f);

			AudioSource.PlayClipAtPoint (explodeSound,transform.position);

			//주변 물체에게 데미지 가하기 
			Vector2 explosionPos = transform.position;
			Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos,radius);

			foreach (Collider2D collider in colliders) {
				if(collider.gameObject.tag=="Player"){
					if(sc.clientId == tc.currentTurn){
						Socket.Emit ("req_take_damage",new Json("damage",20,"socketid",collider.gameObject.GetComponent<Player>().socketid).json);
					}
				}
			}


			//지형파괴 
			if(sc.clientId == tc.currentTurn) Socket.Emit ("req_destroy_ground",new Json("ground",coll.gameObject.name,"centerX",destructionCircle.bounds.center.x,"centerY",destructionCircle.bounds.center.y,"sizeX",destructionCircle.bounds.size.x).json);



			GameObject particle = Instantiate (explosionParticle,transform.position,Quaternion.identity) as GameObject;
			Destroy(particle,2f);

			if(sc.clientId == tc.currentTurn){
				tc.Invoke("reqTurnChange",2f);
			}


			
			Destroy(this.gameObject);
			yield return null;
		}
	}



}
