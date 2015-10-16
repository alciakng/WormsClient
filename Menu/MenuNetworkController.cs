using UnityEngine;
using System.Collections;
using SocketIO;

// Classe que contém a função OnClick(), que é chamada ao clicarmos no botão
public class MenuNetworkController : MonoBehaviour {

	//ServerConnect
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//메뉴컨트롤러
	private MenuController mc;
	
	void Awake(){
		DontDestroyOnLoad(this);

		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		mc = GameObject.Find ("MenuController").GetComponent<MenuController> ();
		Socket = sc.Socket;
	}
	// Use this for initialization
	void Start () {
		EventHandler ();
	}

	void EventHandler(){
		Socket.On ("ack_update_info",mc.ackUpdateInfo);
		Socket.On ("ack_signUp", mc.ackSignUp);
		Socket.On ("ack_login", mc.ackLogin);
		Socket.On ("ack_logout", mc.ackLogout);
		Socket.On ("ack_global_chat",mc.ackGlobalChat);
		Socket.On ("ack_create_room", mc.ackCreateRoom);
		Socket.On ("ack_join_room", mc.ackJoinRoom);
		Socket.On ("ack_leave_room", mc.ackLeaveRoom);
		Socket.On ("ack_room_chat", mc.ackRoomChat);
		Socket.On ("ack_start",mc.ackStart);
	}

	    
}
