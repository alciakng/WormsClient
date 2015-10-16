using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
	


	//마커 transform
	public GameObject marker;

	
	//소켓 연결객체 
	private  ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;



	//현재 무기 맵 
	public static Dictionary<string,Delegate> weaponChangeMap = new Dictionary<string,Delegate> ();
	//function delegate
	public delegate void weaponChange();


	//현재 무기
    public string currentWeapon="noweapon";
	//버튼눌렸는지 여부
    public bool pressed =false;


	//targetting
    public bool targetting=false;
	//pointing
	public bool marking = false;
	//슈팅시간 
    public float timeShooting;
	//슈팅시간 최대치
	public float maxTimeShooting = 1;
	//timeShooting smooth value
	public float timeShootingSmoothValue = 0.01f;


	//슛팅 슬라이더
	public Slider shootingSlider;
	//슛팅 슬라이더 게임오브젝트
	public GameObject shootingSliderGameObject;

	
	public Button ArrowUp;
	
	public Button ArrowDown;




	void Awake(){
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;

		weaponChangeMap.Add ("noweapon", new weaponChange (noWeapon));
		weaponChangeMap.Add ("jetpack", new weaponChange (reqUnsetJetPack));
		weaponChangeMap.Add ("bazooka",new weaponChange(reqUnsetBazooka));
		weaponChangeMap.Add ("bat", new weaponChange (reqUnsetBat));
		weaponChangeMap.Add ("sheep", new weaponChange (reqUnsetSheep));
		weaponChangeMap.Add ("explodeSheep",new weaponChange (reqUnsetSheep));
		weaponChangeMap.Add ("donkey", new weaponChange (reqUnsetDonkey));
		weaponChangeMap.Add ("hbomb", new weaponChange (reqUnsetHbomb));
		weaponChangeMap.Add ("teleport", new weaponChange (reqUnsetTeleport));
		weaponChangeMap.Add ("superSheep", new weaponChange(reqUnsetSuperSheep));
		weaponChangeMap.Add ("rope",new weaponChange(reqUnsetRope));
	}



	void Update(){
			if (targetting) {
				if (Input.touchCount > 0 && Input.touchCount<2 && Input.GetTouch(0).phase == TouchPhase.Moved && !pressed) {
						print ("타겟조정");
						Socket.Emit ("req_update_target", new Json ("shootVectorX",getPlayerToTouchPosition().x,"shootVectorY",getPlayerToTouchPosition().y).json);
					}
		    }
			if (marking) {
				if (Input.touchCount > 0 && Input.touchCount<2 && Input.GetTouch(0).phase == TouchPhase.Moved && !pressed) {
						print ("마커조정");
					  	Vector3 screenToWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y,0));
						Socket.Emit ("req_update_marker", new Json ("markerPositionX",screenToWorldPosition.x,"markerPositionY",screenToWorldPosition.y).json);
					}
			}
			if (!pressed) {
				return;
			}
			else {
				switch(currentWeapon){
					case "bazooka":
					case "hbomb":
						if(timeShooting < maxTimeShooting){
							timeShooting += Time.fixedDeltaTime*timeShootingSmoothValue;
							//슬라이더바 수치변경.
							shootingSlider.value+=timeShooting;
						}
						//Socket.Emit ("req_update_shooting",new Json("isShoot",true).json);
						break;
					case "jetpack":
						Socket.Emit ("req_update_jetpack");
						break;
				}
			}
		}

	public void OnPointerDown(PointerEventData eventData)
	{

		GameObject.Find ("MainCamera").GetComponent<Camera2D> ().touchMoved = false;
		pressed = true;

		switch(currentWeapon){
			case "bazooka":
			case "hbomb":
				timeShooting = 0f;

				//슛팅슬라이더 게임오브젝트 가림.
				shootingSliderGameObject.SetActive(true);
				//슛팅슬라이더 값 초기화.
				shootingSlider.value=0;

				Socket.Emit ("req_update_shoot_detection",new Json("isDetected",true).json);
				break;
			case "bat":
				Socket.Emit ("req_batting");
				this.enabled = false;
				break;
			case "sheep":
				Socket.Emit ("req_shoot_sheep",new Json("type","sheep").json);
				break;
			case "explodeSheep":
				Socket.Emit ("req_explode_sheep");
				this.enabled = false;
				break;
			case "donkey":
				Socket.Emit ("req_shoot_donkey");
				this.enabled = false;
				break;
			case "teleport":
				Socket.Emit ("req_teleporting");
				break;
			case "superSheep":
				Socket.Emit ("req_shoot_sheep",new Json("type","superSheep").json);
				this.enabled = false;
				break;
			case "rope":
				Socket.Emit("req_shoot_rope");
				break;
		}	
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		pressed = false;
		//Socket.Emit ("req_move_stop",new Json("playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y,"playerPositionZ",getPlayerPosition().z).json);

		switch (currentWeapon) {
			case "bazooka":
			case "hbomb":
				shootingSliderGameObject.SetActive(false);
				Socket.Emit ("req_shoot",new Json("timeShooting",shootingSlider.value,"weapon",currentWeapon).json);
				setNoWeapon();
				break;
		}
	}

	public void setNoWeapon(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();
		currentWeapon= "noweapon";

		pressed= false;

		//Socket.Emit ("req_move_stop",new Json("playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y,"playerPositionZ",getPlayerPosition().z).json);
	}


	public void noWeapon(){
		print ("장착된 무기가 없습니다!");
	}



	public void reqSetBazooka(){
		print ("바주카셋");
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();

		currentWeapon= "bazooka";
		targetting = true;
		print (targetting);
		print (currentWeapon);

		Socket.Emit ("req_set_bazooka");
		Socket.Emit ("req_update_target", new Json ("shootVectorX",getPlayerToSightInitialPosition().x,"shootVectorY",getPlayerToSightInitialPosition().y).json);
		Socket.Emit ("req_set_target");
	}

	public void reqUnsetBazooka(){
		print ("바주카 언셋");
		targetting = false;

		Socket.Emit ("req_unset_bazooka");
		Socket.Emit ("req_unset_target");
	}

	public void reqSetBat(){

		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();

		currentWeapon = "bat";

		targetting = true;

		Socket.Emit ("req_set_bat");
		Socket.Emit ("req_update_target", new Json ("shootVectorX",getPlayerToSightInitialPosition().x,"shootVectorY",getPlayerToSightInitialPosition().y).json);
		Socket.Emit ("req_set_target");

	}

	public void reqUnsetBat(){
		print ("야구배트 언셋");
		targetting = false;
	
		Socket.Emit ("req_unset_bat");
		Socket.Emit ("req_unset_target");
	}


	public void reqSetJetPack(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();

		currentWeapon = "jetpack";
		Socket.Emit ("req_set_jetpack");
	}


	public void reqUnsetJetPack(){
		Socket.Emit ("req_unset_jetpack");
	}

	//set sheep
	public void reqSetSheep(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();
		
		currentWeapon = "sheep";

		Socket.Emit ("req_set_sheep");
	}
	//unset sheep
	public void reqUnsetSheep(){
		Socket.Emit ("req_unset_sheep");
	}

	public void reqSetDonkey(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();

		currentWeapon = "donkey";

		marking = true;

		Socket.Emit ("req_set_marker");
		Socket.Emit ("req_set_donkey");
	}


	public void reqUnsetDonkey(){
		marking = false;

		Socket.Emit ("req_unset_donkey");
		Socket.Emit ("req_unset_marker");
	}
	
	public void reqSetHbomb(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();
		
		currentWeapon = "hbomb";

		targetting = true;

		Socket.Emit ("req_set_hbomb");
		Socket.Emit ("req_update_target", new Json ("shootVectorX",getPlayerToSightInitialPosition().x,"shootVectorY",getPlayerToSightInitialPosition().y).json);
		Socket.Emit ("req_set_target");
	}

	public void reqUnsetHbomb(){
		targetting = false;
		
		Socket.Emit ("req_unset_hbomb");
		Socket.Emit ("req_unset_target");
	}

	public void reqSetTeleport(){
		weaponChange unsetWeapon = (weaponChange)weaponChangeMap [currentWeapon].DynamicInvoke ();

		currentWeapon = "teleport";

		marking = true;

		Socket.Emit ("req_set_marker");
		Socket.Emit ("req_set_teleport");
	}

	public void reqUnsetTeleport(){

		marking = false;

		Socket.Emit ("req_unset_teleport");
		Socket.Emit ("req_unset_marker");
	}

	public void reqSetSuperSheep(){
		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();
		
		currentWeapon = "superSheep";
		
		Socket.Emit ("req_set_sheep");
	}

	public void reqUnsetSuperSheep(){
		Socket.Emit ("req_unset_sheep");
	}

	public void reqSetRope(){
	
		ArrowUp.gameObject.SetActive(true);
		ArrowDown.gameObject.SetActive(true);

		weaponChange unsetWeapon=(weaponChange)weaponChangeMap[currentWeapon].DynamicInvoke();

		currentWeapon ="rope";

		targetting = true;

		Socket.Emit ("req_set_rope");
		Socket.Emit ("req_update_target", new Json ("shootVectorX",getPlayerToSightInitialPosition().x,"shootVectorY",getPlayerToSightInitialPosition().y).json);
		Socket.Emit ("req_set_target");
	}

	public void reqUnsetRope(){
		targetting = false;

		ArrowUp.gameObject.SetActive(false);
		ArrowDown.gameObject.SetActive(false);

		Socket.Emit ("req_unset_rope");
		Socket.Emit ("req_unset_target");
	}


	public Vector3 getPlayerPosition(){
		Vector3 playerPosition = GameObject.Find (sc.clientId).GetComponent<Transform> ().transform.position;
		return playerPosition; 
	}


	//터치포지션과 플레이어와의 벡터 계산.
 	public Vector2 getPlayerToTouchPosition(){
			Vector3 WorldTouchPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.GetTouch (0).position.x, Input.GetTouch (0).position.y, 0));
			//Vector3 WorldTouchPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
			
			Vector2 PlayerToTouchPosition = new Vector2 (WorldTouchPos.x - GameObject.Find (sc.clientId).transform.position.x,
			                                    WorldTouchPos.y - GameObject.Find (sc.clientId).transform.position.y);
			PlayerToTouchPosition.Normalize ();
			return PlayerToTouchPosition;
	}

	//게임시작시에 플레이어와 총구와의 초기벡터
	public Vector2 getPlayerToSightInitialPosition(){
		Vector2 PlayerToSightPosition = new Vector2 (GameObject.Find(sc.clientId).GetComponent<Player>().sightTransform.position.x - GameObject.Find (sc.clientId).transform.position.x,
		                                             GameObject.Find(sc.clientId).GetComponent<Player>().sightTransform.position.y - GameObject.Find (sc.clientId).transform.position.y);
		PlayerToSightPosition.Normalize ();
		return PlayerToSightPosition;
	}
	

}
