using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;

public class LeftButtonController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
	
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//현재 컨트롤 하고 있는 객체
	public string currentControl="player";
	
	bool pressed =false;

	public StopWatch sw;
	
	public void Awake(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();

		Socket = sc.Socket;
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		pressed = true;
		Camera.main.GetComponent<Camera2D> ().touchMoved = false;
		switch (currentControl) {
			case "sheep":
				Socket.Emit ("req_sheep_dir_left");
				break;
		}
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		switch(currentControl){
			case "player":
				
				sw.StopWatchReset();
				sw.StopWatchStart();	

				//Socket.Emit ("req_player_position_sync",new Json("moveDir","left","playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y).json);
				Socket.Emit ("req_move_stop",new Json("moveDir","left","playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y).json);
				break;
		}
		pressed = false;
	}

	void Update(){
		if (!pressed)
			return;
		else {
			switch (currentControl) {
				case "player":
					Socket.Emit ("req_move_left");
					break;
				case "superSheep":
					Socket.Emit ("req_supersheep_rotation_left");
					break;
			}
		}
	}


	public Vector3 getPlayerPosition(){
		Vector3 playerPosition = GameObject.Find (sc.clientId).GetComponent<Transform> ().transform.position;
		return playerPosition; 
	}

}
