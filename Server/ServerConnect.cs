using UnityEngine;
using System.Collections;
using SocketIO;
using System;
using System.Text;


public class ServerConnect : MonoBehaviour {


	//클라이언트 소켓 
	public SocketIOComponent Socket;
	//로그인 여부
	public bool loggedIn;
	//me 
	public string clientId;
	//player object
	public JSONObject players;
	//서버연결여부
	public bool connectedToServer =false;
	//다음화면 
	string jumpSceneName;

	void Awake () {
		DontDestroyOnLoad(this);
		//Screen.SetResolution(Screen.width,(Screen.width/3)*2,true);
		Screen.SetResolution(800,480,true);

		if(!Socket.IsConnected){
			Socket.Connect ();
		}
	}

}
