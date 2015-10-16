using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameMenuManager : MonoBehaviour {
	
	
	//ServerConnect
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;


	
	//id
	private string id;
	//password
	private string password;
	
	
	//채팅메뉴
	public GameMenu GameChatMenu;
	//무기메뉴
	public GameMenu GameWeaponMenu;
	//Lose메뉴
	public GameMenu LoseMenu;
	//Victory메뉴
	public GameMenu VictoryMenu;
	//Exit메뉴
	public GameMenu ExitMenu;
	
	void Awake(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}

	public void  ShowChatMenu(){
		if (GameChatMenu.IsOpen)
			GameChatMenu.IsOpen = false;
		else
			GameChatMenu.IsOpen = true;
	}

	public void  ShowWeaponMenu(){
		if (GameWeaponMenu.IsOpen)
			GameWeaponMenu.IsOpen = false;
		else
			GameWeaponMenu.IsOpen = true;
	}

	public void  ShowLoseMenu(){
		LoseMenu.IsOpen = true;
	}

	public void  ShowVictoryMenu(){
		VictoryMenu.IsOpen = true;
	}

	public void  ShowExitMenu(){
		if (ExitMenu.IsOpen)
			ExitMenu.IsOpen = false;
		else
			ExitMenu.IsOpen = true;
	}


	public void reqGameChat(){
		InputField ChatField = GameObject.Find ("GameChatInput").GetComponent<InputField> ();
		string input = ChatField.text;
		ChatField.text = "";
		Socket.Emit ("req_room_chat",new Json("input",input).json);
	}

	public void VictoryToLobby(){
		Socket.Emit ("req_to_lobby",new Json("isVictory",true).json);
	}

	public void LoseToLobby(){
		Socket.Emit ("req_to_lobby", new Json ("isVictory", false).json);
	}
}