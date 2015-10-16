using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour {


	//ServerConnect
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	
	
	//id
	private string id;
	//password
	private string password;


	//현재메뉴
	public Menu CurrentMenu;
	
	void Awake(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}


	// Use this for initialization
	void Start () {
		ShowMenu (CurrentMenu);
	}
		
	public void  ShowMenu(Menu menu){
		print (menu.name);
		print (CurrentMenu.name);
		if (CurrentMenu != null){
			CurrentMenu.IsOpen = false;
		}else{
			print("?");
		}
	
		CurrentMenu = menu;
		CurrentMenu.IsOpen = true;
	}
	
	public void reqSignUp(){
		id = GameObject.Find("SignUpIdField").GetComponent<InputField>().text;
		password = GameObject.Find ("SignUpPasswordField").GetComponent<InputField>().text;
		Socket.Emit ("req_signUp",new Json ("id",id, "password",password).json);
	}
	
	public void reqLogin(){
		id = GameObject.Find("LoginIdField").GetComponent<InputField>().text;
		password = GameObject.Find ("LoginPasswordField").GetComponent<InputField>().text;
		Socket.Emit ("req_login",new Json("id",id,"password",password).json);
	}

	public void reqLogout(){
		Socket.Emit ("req_logout");

		Destroy (GameObject.Find ("ServerConnect"));
		Destroy (GameObject.Find ("MenuController"));
		Destroy(GameObject.Find ("MenuNetworkController"));

		Application.LoadLevel("MainMenu");
	}

	public void reqGlobalChat(){
		string input = GameObject.Find ("ChatInput").GetComponent<InputField> ().text;
		GameObject.Find ("ChatInput").GetComponent<InputField> ().text = "";
		Socket.Emit ("req_global_chat",new Json("input",input).json);
	}

	public void reqCreateRoom(){
		string roomname = GameObject.Find ("RoomNameField").GetComponent<InputField> ().text;
		Socket.Emit ("req_create_room",new Json("roomname",roomname).json);
	}

	public void reqJoinRoom(string roomname){
		Socket.Emit ("req_join_room",new Json("roomname",roomname).json);
	}

	public void reqLeaveRoom(){
		Socket.Emit ("req_leave_room");
	}
	
	public void reqRoomChat(){
		InputField ChatField = GameObject.Find ("RoomChatInput").GetComponent<InputField> ();
		string input = ChatField.text;
		ChatField.text = "";
		Socket.Emit ("req_room_chat",new Json("input",input).json);
	}

	public void reqStart(){
		Socket.Emit ("req_start");
	}

}
