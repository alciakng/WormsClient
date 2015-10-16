using UnityEngine;
using System.Collections;
using SocketIO;

public class Bat : MonoBehaviour {

	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	
	//턴 컨트롤러
	private TurnController tc;

	//방향벡터 
	public Vector2 dirVec;
	
	void Start () {
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	
		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
	}

	
	public void OnCollisionEnter2D(Collision2D coll){
		print (coll.gameObject.name);
		if (coll.gameObject.tag == "Player") {
			print ("맞았다");

			if(sc.clientId == tc.currentTurn){
				Socket.Emit ("req_take_damage",new Json("damage",12,"socketid",coll.gameObject.GetComponent<Player>().socketid).json);
			}

			Socket.Emit ("req_player_batted",new Json("id",coll.gameObject.name,"dirVecX",dirVec.x,"dirVecY",dirVec.y).json);

			//coll.gameObject.GetComponent<Rigidbody2D>().AddForce(dirVec*500f);

			//카메라 설정
			GameObject.Find ("MainCamera").GetComponent<Camera2D> ().SetTarget (coll.transform);

			//8초 후에 턴 바꿈
			if(sc.clientId == tc.currentTurn){
				tc.Invoke("reqTurnChange",8f);
			}
		}
	}
}
