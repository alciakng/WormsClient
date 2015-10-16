using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DownButtonController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
	
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;

	//늘일 로프.
	public string ropeType;

	bool pressed =false;
	
	public void Awake(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		pressed = true;
		Camera.main.GetComponent<Camera2D> ().touchMoved = false;

	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		pressed = false;
	}
	
	void FixedUpdate(){
		if (!pressed)
			return;
		else {
			Socket.Emit("req_rope_down",new Json("ropeType",ropeType).json);
		}
		
	}
	
	

	
}
