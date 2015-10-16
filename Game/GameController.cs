using UnityEngine;
using System.Collections;
using SocketIO;
using System.Threading;
using UnityEngine.UI;


public class GameController : MonoBehaviour {
	
	//플레이어 프리팹 
	public GameObject Player;
	//플레이어 컨트롤러
	private Player pc;
	//ServerConnect
	private ServerConnect sc;
	//ClientSocket
	private SocketIOComponent Socket;

	//턴 컨트롤러 
	private TurnController tc;

	
	//동기화를 위한 스탑워치.
	public StopWatch sw;

	// Use this for initialization
	void Start () {
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
		sw = GameObject.Find ("StopWatch").GetComponent<StopWatch>();

		Socket = sc.Socket;
	}

	public void turnChange(SocketIOEvent e){
		print ("턴체인지");
		GameObject.Find ("TurnController").GetComponent<TurnController> ().turnStateChange(e.data);
	}

	public void gameChat(SocketIOEvent e){
		print (e);
		string id = e.data.GetField ("id").str;
		string input = e.data.GetField ("input").str;
		
		if (input != "") {
			GameObject GameChatField = GameObject.Find ("GameChatField");
			print (GameObject.Find ("GameChatField").GetComponent<RectTransform> ().sizeDelta.x);
			print (GameObject.Find ("GameChatField").GetComponent<RectTransform> ().sizeDelta.y);

			GameChatField.GetComponent<Text> ().text += id + " : " + input + "\n";
			GameChatField.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameChatField.GetComponent<RectTransform> ().sizeDelta.x, GameChatField.GetComponent<RectTransform> ().sizeDelta.y + 17);
		}
	}

	public void toLobby(SocketIOEvent e){
		Application.LoadLevel("LobbyRoom");
	}

	public void positionSync(SocketIOEvent e){
		sw.StopWatchStop();
		float syncTime = sw.GetElapsedMilliseconds();

		Socket.Emit ("req_move_stop",new Json("moveDir",e.data.GetField("moveDir").str,"playerPositionX",e.data.GetField("playerPositionX").f,"playerPositionY",e.data.GetField("playerPositionY").f,"syncTime",syncTime/1000).json);
	}



	public void moveRight(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().moveRight ();
	}

	public void moveLeft(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().moveLeft ();
	}

	public void moveStop(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().moveStop(e.data.GetField("player"));
	}

	public void jump(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().jump ();
	}

	public void jumpStop(SocketIOEvent e){
		pc = GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ();
		pc.SendMessage ("jumpStop");
	}

	public void setTarget(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().setTarget();
	}

	public void unsetTarget(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().unsetTarget ();
	}

	public void updateTarget(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().updateTarget(e.data.GetField("player"));
		//pc.updateTarget (e.data.GetField ("mx").f, e.data.GetField ("my").f);
	}
	
	public void setMarker(SocketIOEvent e){
		print ("마커 셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().setMarker ();
	}

	public void unsetMarker(SocketIOEvent e){
		print ("마커 언셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().unsetMarker ();
	}

	public void updateMarker(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().updateMarker (e.data);
	}

	/*
	public void updateShootDetection(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().Invoke ("updateShootDetection", 0f);
		//pc.updateShootDetection (e.data.GetField ("isDetected").b);
	
	}

	public void updateShooting(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().updateShooting(e.data);
		//pc.updateShooting (e.data.GetField ("isShoot").b);
		//pc.SendMessage("updateShooting",e.data);
	}
	*/

	public void shoot(SocketIOEvent e){
		print (e.data);
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().shoot(e.data.GetField("player"));
	}

	public void setBazooka(SocketIOEvent e){
		pc = GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ();
		pc.SendMessage("setBazooka");
	}

	public void unsetBazooka(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player> ().Invoke ("unsetBazooka", 0f);
	}

	public void setBomb(SocketIOEvent e){
		pc = GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>();
		pc.SendMessage("setBomb");
	}

	public void unsetBomb(SocketIOEvent e){
		pc = GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>();
		pc.SendMessage ("unsetBomb");
	}

	public void setBat(SocketIOEvent e){
		pc = GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>();
		pc.SendMessage ("setBat");
	}

	public void unsetBat(SocketIOEvent e){
		print (e.data.GetField("player").GetField("id").str);
		print ("플레이어 야구배트 언셋");
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>().Invoke("unsetBat",0f);
		//pc.SendMessage ("unsetBat");
	}

	public void batting(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>().batting (e.data);
	}

	public void playerBatted(SocketIOEvent e){
		GameObject.Find (e.data.GetField("id").str).GetComponent<Player>().batted(e.data);
	}

	public void setJetPack(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>().setJetPack();
	}

	public void unsetJetPack(SocketIOEvent e){
		GameObject.Find (e.data.GetField("player").GetField("id").str).GetComponent<Player>().unsetJetPack();
	}

	public void updateJetPack(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("player").GetField ("id").str).GetComponent<Player> ().updateJetpack ();
	}

	public void setSheep(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().setSheep ();
	}

	public void unsetSheep(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().unsetSheep ();
	}

	public void shootSheep(SocketIOEvent e){
		print ("양발사 요청");
		print (e.data);
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().shootSheep(e.data.GetField("type").str);
	}



	public void sheepDirLeft(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<Sheep> ().SetDir (e.data.GetField ("sheepDir").f);
	}

	public void sheepDirRight(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<Sheep> ().SetDir (e.data.GetField ("sheepDir").f);
	}

	public void superSheepRotationLeft(SocketIOEvent e){
		print ("양회전");
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<SuperSheep> ().rotationLeft ();
	}
	
	public void superSheepRotationRight(SocketIOEvent e){
		print ("양회전");
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<SuperSheep> ().rotationRight ();
	}


	public void sheepJump(SocketIOEvent e){
		print ("양 점프!");
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<Sheep> ().Jump ();
	}

	public void explodeSheep(SocketIOEvent e){
		print ("양 폭파!");
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<Sheep> ().Explode ();
	}

	public void explodeSuperSheep(SocketIOEvent e){
		print ("수퍼 양 폭파");
		GameObject.Find (e.data.GetField ("id").str + "sheep").GetComponent<SuperSheep> ().Explode ();
	}


	public void syncSheep(SocketIOEvent e){
		GameObject.Find (e.data.GetField ("name").str).GetComponent<Sheep> ().syncSheep (e.data);
	}


	public void setDonkey(SocketIOEvent e){
		print ("당나귀 셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().setDonkey ();
	}

	public void unsetDonkey(SocketIOEvent e){
		print ("당나귀 언셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().unsetDonkey ();
	}

	public void shootDonkey(SocketIOEvent e){
		print ("당나귀 발사");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().shootDonkey (e.data);
	}

	public void destroyDonkey(SocketIOEvent e){
		print ("당나귀 제거");
		Destroy(GameObject.Find (e.data.GetField ("name").str));
	}

	public void setHbomb(SocketIOEvent e){
		print ("할렐루야 셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().setHbomb ();
	}

	public void unsetHbomb(SocketIOEvent e){
		print ("할렐루야 언셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().unsetHbomb ();
	}

	public void setTeleport(SocketIOEvent e){
		print ("텔레포트 셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().setTeleport ();
	}

	public void unsetTeleport(SocketIOEvent e){
		print ("텔레포트 언셋");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().unsetTeleport ();
	}

	public void teleporting(SocketIOEvent e){
		print ("텔레포팅");
		GameObject.Find (e.data.GetField ("id").str).GetComponent<Player> ().StartCoroutine ("teleporting", e.data);
	}
	
	public void setRope(SocketIOEvent e){
		print ("로프장착");
		GameObject.Find (e.data.GetField("id").str).GetComponent<Player>().setRope();
	}

	public void unsetRope(SocketIOEvent e){
		print ("로프장착해제");
		GameObject.Find (e.data.GetField("id").str).GetComponent<Player>().unsetRope();
	}

	public void shootRope(SocketIOEvent e){
		print ("슛 로프!");
		//플레이어의 6번째 자식 = 로프 게임오브젝트
		GameObject.Find (e.data.GetField("player").GetField("id").str).transform.GetChild(6).gameObject.GetComponent<Rope>().shootRope(e.data.GetField("player"));
	}

	public void ropeJump(SocketIOEvent e){
		print ("로프 점프!");
		GameObject.Find (e.data.GetField("id").str).transform.GetChild(6).GetComponent<Rope>().ropeJump();
	}

	public void ropeDown(SocketIOEvent e){
		print ("로프 다운");
		GameObject.Find (e.data.GetField("id").str).transform.GetChild(6).GetComponent<Rope>().ropeDown();
	}

	public void ropeUp(SocketIOEvent e){
		print ("로프 업");
		GameObject.Find (e.data.GetField("id").str).transform.GetChild(6).GetComponent<Rope>().ropeUp();
	}

	public void destroyGround(SocketIOEvent e){
		print ("지형 파괴");
		print (e.data);
		GameObject.Find (e.data.GetField("ground").str).GetComponent<GroundController> ().DestroyGround (e.data);
	}

	public void playerDead(SocketIOEvent e){
		print ("플레이어 사망");
		GameObject.Find ("TurnController").GetComponent<TurnController>().dead(e.data.GetField("id").str);
	}

	public void playerVictory(SocketIOEvent e){
		print ("플레이어 승리");
		GameObject.Find ("TurnController").GetComponent<TurnController> ().victory (e.data.GetField("id").str);
	}


	public void CreatePlayer(JSONObject Clients)
	{
		//서버는 방에 접속한 클라이언트들의 정보를 담고 있어야 하며 이를 게임시작시 각 클라이언트에게 뿌려주어 동적으로 프리팹을 생성하게 한다.
		Debug.Log (Clients.Count);

		//플레이어의 랜덤한 생성위치 산출 
		for(int i=0;i<Clients.Count;i++) {
			InstantiatePlayer(Player,new Vector2(Clients[i].GetField("playerPositionX").f,Clients[i].GetField("playerPositionY").f),Clients[i].GetField("id").str,Clients.keys[i].ToString());
		}
	}
	
	public void InstantiatePlayer(GameObject Player,Vector3 pos,string id,string socketid){
		GameObject player = Instantiate (Player,pos, Quaternion.identity) as GameObject;
	

		player.GetComponent<Player> ().id = id;
		player.GetComponent<Player> ().socketid = socketid;
		player.name = id;
	}

	public Player getPlayerById(string id){
		GameObject.Find (id).GetComponent<Player>();
		return pc;
	}
	

}
