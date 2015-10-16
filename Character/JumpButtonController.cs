using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class JumpButtonController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler  {
	
	//소켓 연결객체 
	private  ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//현재 컨트롤하고 있는 게임오브젝트
	public string currentControl="player";

	// Use this for initialization
	void Start () {
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{

		Camera.main.GetComponent<Camera2D> ().touchMoved = false;

		switch(currentControl){
			case "player" : 
				Socket.Emit ("req_jump");
				break;
			case "sheep" :
				Socket.Emit ("req_sheep_jump");
				break;
			case "rope" :
				Socket.Emit ("req_rope_jump");
			break;
		}
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
	}


}
