using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.IO;


public class MenuController : MonoBehaviour {


	//플레이어 리스트의 플레이어 버튼
	public Button playerButton;
	//방 버튼
	public Button roomButton;
	//룸 플레이어 아이콘
	public Button roomPlayerIconButton;
	
	//ServerConnect
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//MenuManager
	private MenuManager mm;




	public GameObject LoginMessage;
	public GameObject SignUpMessage;
	
	void Start(){
		DontDestroyOnLoad(this);

		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		mm = GameObject.Find ("Canvas").GetComponent<MenuManager> ();
		Socket = sc.Socket;

	}

	public void ackUpdateInfo(SocketIOEvent e){
	
		//플레이어 리스트 업데이트.
		if (e.data.GetField ("playerList") != null){
			foreach (Transform child in  GameObject.Find("PlayerList").transform) {
				Destroy (child.gameObject);
			};
		
			for (int i=0; i<e.data.GetField("playerList").Count; i++) {
				createPlayerList (e.data.GetField ("playerList") [i].GetField("id").str);
			}
		}

		if (e.data.GetField ("roomList") != null){
			//룸 리스트 업데이트.
			foreach (Transform child in  GameObject.Find("RoomList").transform) {
				Destroy (child.gameObject);
			};
		
			for (int i=0; i<e.data.GetField("roomList").Count; i++) {
				createRoomList (e.data.GetField ("roomList") [i]);
			}
		}

		if (e.data.GetField ("room") != null){
			//룸 업데이트.
			for (int i=0; i<4; i++) {
				string slot = "RoomPlayer" + i.ToString ();
				if (GameObject.Find (slot).transform.childCount == 1) {
					Destroy (GameObject.Find (slot).transform.GetChild (0).gameObject);
				}
			}

			Debug.Log (e.data.GetField ("room"));
			this.createRoom (e.data.GetField ("room"));
		}
	}

	public void ackSignUp(SocketIOEvent e){

		//회원가입 성공시 
		if (e.data.GetField("flag").str.Equals("success")) {
			//메인메뉴 띄움.
			Debug.Log ("회원가입 성공");
			SignUpMessage.GetComponent<Text>().text="";
			GameObject.Find ("Canvas").GetComponent<MenuManager> ().ShowMenu(GameObject.Find ("MainMenu").GetComponent<Menu>());
		}
		//todo : 회원가입 실패시 dialog 띄움.
		else {
			SignUpMessage.GetComponent<Text>().text="";
			SignUpMessage.GetComponent<Text>().text += e.data.GetField("msg").str;
			Debug.Log ("회원가입 실패");

		}
	}

	public void ackLogin(SocketIOEvent e){
		//로그인 성공시
		if (e.data.GetField("flag").str.Equals("success")) {
			Debug.Log ("로그인 성공");
			//내 아이디 정보 등록 

			LoginMessage.GetComponent<Text>().text ="";
			sc.clientId=e.data.GetField("msg").str;

			Application.LoadLevel("LobbyRoom");

			//mm.ShowMenu(GameObject.Find ("Lobby").GetComponent<Menu>());

			print ("성공");
		}
		//todo : 로그인  실패시 dialog 띄움.
		else {
			LoginMessage.GetComponent<Text>().text="";
			LoginMessage.GetComponent<Text>().text += e.data.GetField("msg").str;
			Debug.Log ("로그인 실패");
		}
	}

	public void ackLogout(SocketIOEvent e){
		//로그아웃하면 글로벌 채팅창을 깨끗히 비운다.
		GameObject.Find ("ChatField").GetComponent<Text> ().text = "";
		GameObject.Find ("Canvas").GetComponent<MenuManager> ().ShowMenu(GameObject.Find ("LoginMenu").GetComponent<Menu>());
	}

	public void ackGlobalChat(SocketIOEvent e){

		string id = e.data.GetField ("id").str;
		string input = e.data.GetField ("input").str;
		
		if (input != "") {
			GameObject chatField = GameObject.Find("ChatField");
			chatField.GetComponent<Text> ().text += id+" : "+input+"\n";
			chatField.GetComponent<RectTransform>().sizeDelta = new Vector2(chatField.GetComponent<RectTransform>().sizeDelta.x,chatField.GetComponent<RectTransform>().sizeDelta.y+17);
		}
	}

	public void ackRoomChat(SocketIOEvent e){
		string id = e.data.GetField("id").str;
		string input = e.data.GetField ("input").str;

		if (input != "") {
			GameObject roomChatField = GameObject.Find("RoomChatField");
			roomChatField.GetComponent<Text> ().text += id+" : "+input+"\n";
			roomChatField.GetComponent<RectTransform>().sizeDelta = new Vector2(roomChatField.GetComponent<RectTransform>().sizeDelta.x,roomChatField.GetComponent<RectTransform>().sizeDelta.y+17);
		}
	}
	
	public void ackCreateRoom(SocketIOEvent e){
		GameObject.Find ("Canvas").GetComponent<MenuManager> ().ShowMenu (GameObject.Find("Room").GetComponent<Menu>());
	}
	
	public void ackJoinRoom(SocketIOEvent e){
		Debug.Log ("방 입장");
		GameObject.Find ("Canvas").GetComponent<MenuManager> ().ShowMenu (GameObject.Find("Room").GetComponent<Menu>());
	}

	public void ackLeaveRoom(SocketIOEvent e){
		Debug.Log ("방 퇴장");
		//방을 퇴장하면 룸 채팅창을 깨끗히 비운다.
		GameObject.Find ("RoomChatField").GetComponent<Text> ().text = "";
		GameObject.Find ("Canvas").GetComponent<MenuManager> ().ShowMenu (GameObject.Find ("Lobby").GetComponent<Menu>());
	}
	
	public void ackStart(SocketIOEvent e){
		sc.players = e.data;
		Application.LoadLevel ("gameScene");
	}


	private void createRoom(JSONObject room){

		float playerCount = room.GetField ("playerCount").n;

		if (playerCount >= 1)
			GameObject.Find ("StartButton").GetComponent<Button> ().interactable = true;
		else
			GameObject.Find ("StartButton").GetComponent<Button> ().interactable = false;

		for (int i=0; i<playerCount; i++) {
			Button roomPlayerIconBtn = Instantiate (roomPlayerIconButton);
			string slot = "RoomPlayer"+i.ToString();
			Transform PanelTransform = GameObject.Find (slot).transform;
	
			roomPlayerIconBtn.GetComponentInChildren<Text>().text = room.GetField("players")[i].GetField("id").str;
			roomPlayerIconBtn.transform.SetParent(PanelTransform,false);
			roomPlayerIconBtn.transform.position = PanelTransform.position;
		}
	}


	//todo : parameter로 ypos도 넘겨!
	private void createPlayerList(string id){
		Button playerBtn = Instantiate(playerButton);
		GameObject PlayerList = GameObject.Find ("PlayerList");

		PlayerList.GetComponent<RectTransform>().sizeDelta = new Vector2(PlayerList.GetComponent<RectTransform>().sizeDelta.x,PlayerList.GetComponent<RectTransform>().sizeDelta.y+20);
		playerBtn.GetComponentInChildren<Text>().text = id;
		playerBtn.transform.SetParent(PlayerList.transform,false);
		//playerBtn.transform.position = PlayerPanelTransform.position;
		//ypos를 용하여 for문 돌리면서 GetComponent를 설정해준다.
		//playerBtn.GetComponent<RectTransform> ().position = new Vector3 (PlayerPanelTransform.position.x, PlayerPanelTransform.position.y - (30+ypos), PlayerPanelTransform.position.z);
	}


	//todo : parameter로 ypos도 넘기고 원래 룸을 지워주고 다시 생성해야 됨.
	private void createRoomList(JSONObject room){
		Button roomBtn = Instantiate (roomButton);

		GameObject roomList = GameObject.Find ("RoomList");

		roomList.GetComponent<RectTransform>().sizeDelta = new Vector2(roomList.GetComponent<RectTransform>().sizeDelta.x,roomList.GetComponent<RectTransform>().sizeDelta.y+20);
		roomBtn.GetComponentInChildren<Text>().text = room.GetField("name").str + "[방 상태 :" +room.GetField("roomState")+", 인원 : " + room.GetField("playerCount") +"]";
		roomBtn.transform.SetParent(roomList.transform,false);
		//roomBtn.transform.position = roomPanelTransform.position;
		//roomBtn.GetComponent<RectTransform> ().position = new Vector3 (roomPanelTransform.position.x, roomPanelTransform.position.y -(30+ypos), roomPanelTransform.position.z);

		roomBtn.GetComponent<Button>().onClick.AddListener(()=>GameObject.Find ("Canvas").GetComponent<MenuManager> ().reqJoinRoom(room.GetField("name").str));
	}
}
