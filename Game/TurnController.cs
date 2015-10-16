using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;

public class TurnController : MonoBehaviour {
	
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//현재 턴
	public string currentTurn;
	//타이머 
	public Text timer;
	//타임
	private float time;

	//바람세기 슬라이더.
	public Slider WindSlider;  


	public GameObject LeftButton;
	public GameObject RightButton;
	public GameObject JumpButton;
	public GameObject ShootButton;
	public GameObject WeaponOpen;
	public GameObject Canvas;
	public GameObject MainCamera;
	public GameObject Wind;



	public void Start(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;

	}

	//업데이트 문 
	public void Update(){
		//턴 시간이 종료되면 턴 체인지.
		if (currentTurn == sc.clientId) {
			time += Time.fixedDeltaTime;
		
			float seconds = 40 - time % 60;
			timer.text = string.Format ("{0}", Mathf.RoundToInt (seconds));
		
			if (seconds < 0) {
				time = 0;
				print ("뭐지.");
				switch(ShootButton.GetComponent<WeaponController>().currentWeapon){
					case "donkey" :
						if(GameObject.Find ("donkey")!=null)
							Socket.Emit ("req_destroy_donkey",new Json("name","donkey").json);
						reqTurnChange ();
						break;
					case "explodeSheep" :
						Socket.Emit ("req_explode_sheep");
						break;

					case "superSheep":
						Socket.Emit ("req_explode_supersheep");
						break;
					default :
						reqTurnChange ();
						break;
				}
			}
		}
	}
	
	public void turnStateChange(JSONObject turn){
		bool state = turn.GetField ("state").b;
		currentTurn = turn.GetField ("turn").str;

		LeftButton.GetComponent<LeftButtonController> ().enabled = state;
		RightButton.GetComponent<RightButtonController> ().enabled = state;
		JumpButton.GetComponent<JumpButtonController> ().enabled = state;
		ShootButton.GetComponent<WeaponController> ().enabled = state;
		WeaponOpen.GetComponent<Button> ().interactable = state;
		Canvas.GetComponent<GameMenuManager> ().GameWeaponMenu.IsOpen = state;


		Transform target = GameObject.Find (currentTurn).transform;

		MainCamera.GetComponent<Camera2D> ().touchMoved = false;
		MainCamera.GetComponent<Camera2D> ().SetTarget (target);

		float windFos = turn.GetField ("wind").f;

		Wind.GetComponent<Wind>().setWindFos(windFos);


		if (windFos < 0) {
			WindSlider.SetDirection (Slider.Direction.RightToLeft,true);
			WindSlider.value = Mathf.Abs (windFos);
		} else {
			WindSlider.SetDirection (Slider.Direction.LeftToRight,true);
			WindSlider.value = Mathf.Abs (windFos);
		}

		syncPlayers (turn.GetField ("players"));
	}


	public void syncPlayers(JSONObject players){
		for (var i=0; i<players.Count; i++) {

			GameObject player = GameObject.Find (players[i].GetField("id").str);
			//위치 동기화
			player.transform.position = new Vector3(players[i].GetField("playerPositionX").f,players[i].GetField("playerPositionY").f,0);
			//체력 동기화
			player.GetComponent<Player>().health = players[i].GetField("hp").f;
		}
	}

	public void reqTurnChange(){
		//아무무기도 사용하지 않는 초기상태로 변경.
		GameObject.Find ("ShootButton").GetComponent<WeaponController> ().setNoWeapon ();

		if(sc.clientId==currentTurn){
			Socket.Emit ("req_move_stop",new Json("playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y,"playerPositionZ",getPlayerPosition().z).json);
		    Socket.Emit ("req_turn_change");
			time = 0;
			timer.text="";
		}
	}

	public Vector3 getPlayerPosition(){
		Vector3 playerPosition = GameObject.Find (sc.clientId).GetComponent<Transform> ().transform.position;
		return playerPosition; 
	}

	public void dead(string id){
		currentTurn = null;
		Destroy (GameObject.Find(id).gameObject);

		if (id == sc.clientId) {
			GameObject.Find ("Canvas").GetComponent<GameMenuManager> ().ShowLoseMenu ();
		}
	}
	
	public void victory(string id){
		currentTurn = null;
		Destroy (GameObject.Find (id).gameObject);
		if (id == sc.clientId) {
			GameObject.Find ("Canvas").GetComponent<GameMenuManager> ().ShowVictoryMenu ();
		}
	}




}
