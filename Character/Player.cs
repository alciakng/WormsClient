using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	//WeaponController
	private WeaponController wc;
	//턴 컨트롤러
	private TurnController tc;
	//소켓 연결객체 
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;
	//클라이언트 아이디
	public string id;
	//클라이언트 소켓아이디
	public string socketid;
	

	//중력가속도 
	public float gravity; 
	//이동스피드 
	public float runSpeed;
	//groundDamping or inAitDamping
	public float smoothedMovementFactor;
	//땅에서 이동할 때 스피드 보정
	public float groundDamping;
	//공중에서 이동할 때 스피드 보정 
	public float inAirDamping;
	//점프파워 
	public float jumpPower;
	
	//bullet최대 속도.
	public float bulletMaxInitialVelocity; 
	public float maxTimeShooting;
	
	//max health
	public float maxHealth;
	public float health;
	public bool isDead;

	//지면 체크 
	public bool isGround;


	//로프 카운트
	public int ropeCount;

	
	//애니메이터 
	public Animator an; 

	
	//GUI HP TEXT
	public Transform HP;
	public Transform PlayerHP;


	//bulleat 프리팹 
	public GameObject bulletPrefab;
	//sheep 프리팹 
	public GameObject sheepPrefab;
	//superSheep 프리팹
	public GameObject superSheepPrefab;
	//donkey 프리팹
	public GameObject donkeyPrefab;
	//hbomb 프리팹
	public GameObject hbombPrefab;
	//rope 프리팹
	public GameObject ropePrefab;
	//슛팅 이펙트  
	public GameObject shootingEffect;
	// 총구 transform 
	public Transform sightTransform;
	// 마커 transform 
	public Transform markerTransform;
	// 총 transform 
	public Transform gunTransform;
	// 플레이어 몸 transform 
	public Transform bodyTransform;
	// 총알 생성 초기위치 transform
	public Transform bulletInitialTransform;
	// 양 생성 초기위치 transform
	public Transform sheepInitialTransform;
	//젯트팩 트랜스폼
	public Transform jetpackTransform;
	// rigidbody2d
	public Rigidbody2D rb;
	//rope
	public GameObject rope;


	public AudioClip jetpackSound;

	public AudioClip coughSound;

	public AudioClip superSheepSound;

	public AudioClip SheepSound;

	//동기화를 위한 스탑워치.
	public StopWatch sw;


	void Awake(){
		wc = GameObject.Find ("ShootButton").GetComponent<WeaponController> ();
		tc = GameObject.Find ("TurnController").GetComponent<TurnController> ();
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		sw = GameObject.Find ("StopWatch").GetComponent<StopWatch>();


		Socket = sc.Socket;

		an = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody2D> ();

		Physics2D.IgnoreLayerCollision (15, 10);
		Physics2D.IgnoreLayerCollision (15, 11);
	}


	void Start () {
		PlayerHP = Instantiate (HP) as Transform;
		PlayerHP.GetComponent<GUIText>().color = new Color( Random.value, Random.value, Random.value, 1.0f );
		health = maxHealth;
	}
	

	// Update is called once per frame
	void Update () {
		displayHP ();
		print (rb.velocity);
	}

	

	//플레이어 오른쪽으로이동.
	public void moveRight(){
		if (bodyTransform.localScale.x > 0f) {
			bodyTransform.localScale = new Vector3 (-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
			jetpackTransform.localPosition = new Vector3 (-jetpackTransform.localPosition.x, jetpackTransform.localPosition.y, 0f);
		}

		rb.velocity = new Vector2(1f * 70f * Time.fixedDeltaTime,rb.velocity.y);

		if (isGround) {
			an.SetBool("moving",true);
		}
	}

	//플레이어 왼쪽으로 이동.
	public void moveLeft(){
		if (bodyTransform.localScale.x < 0f) {
			bodyTransform.localScale = new Vector3 (-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
			jetpackTransform.localPosition = new Vector3 (-jetpackTransform.localPosition.x, jetpackTransform.localPosition.y, 0f);
		}

		rb.velocity = new Vector2(-1f * 70f * Time.fixedDeltaTime,rb.velocity.y);

		if (isGround) {
			an.SetBool("moving",true);
		}
	}

	public void moveStop(JSONObject player){
			
		Vector3 prevPlayerPosition = new Vector3 (player.GetField ("playerPositionX").f, player.GetField ("playerPositionY").f);
		Vector3 curPlayerPosition = new Vector3();

		//calculate position by dead reckoning.

		print (player);

		switch(player.GetField("moveDir").str){
			case "right":
				curPlayerPosition = new Vector3(prevPlayerPosition.x + (0.015f*70f*Time.fixedDeltaTime),prevPlayerPosition.y);
				break;
			case "left":
				curPlayerPosition = new Vector3(prevPlayerPosition.x - (0.015f*70f*Time.fixedDeltaTime),prevPlayerPosition.y);
				break;
			case "stop":
				curPlayerPosition = prevPlayerPosition;
			break;
		}
	
		transform.position = curPlayerPosition;

		an.SetBool("moving",false);
	}


	//플레이어 점프
	public void jump(){
		if (isGround) {
			rb.AddForce (Vector2.up * 300f);
			an.SetTrigger ("jump");
		}
	}


	
	public void setTarget(){
		//gunTransform.gameObject.SetActive(true);
		print ("타겟셋");
		sightTransform.gameObject.SetActive (true);
	
	}
	
	public void unsetTarget(){
		print ("타겟언셋");
		sightTransform.gameObject.SetActive (false);
	}

	public void setMarker(){
		markerTransform.gameObject.SetActive (true);
	}

	public void unsetMarker(){
		markerTransform.gameObject.SetActive (false);
	}

	public void updateTarget(JSONObject target){
		
		float angle = Mathf.Asin(target.GetField("shootVectorY").f)*Mathf.Rad2Deg;
		if( target.GetField("shootVectorX").f < 0f )
			angle = 180-angle;
		
		gunTransform.localEulerAngles = new Vector3(0f, 0f, angle);
		sightTransform.localEulerAngles = new Vector3(0f, 0f, angle);
	}

	public void updateMarker(JSONObject marker){
		//print (Camera.main.ScreenToWorldPoint (new Vector3 (marker.GetField ("markerPositionX").f, marker.GetField ("markerPositionY").f, 0)));

		markerTransform.position = new Vector3 (marker.GetField ("markerPositionX").f, marker.GetField ("markerPositionY").f, 0);
	}

	public void shoot(JSONObject shoot){

		Vector2 shootDirection = new Vector2(shoot.GetField("shootVectorX").f,shoot.GetField("shootVectorY").f);
		GameObject weapon = null;

		print (shoot.GetField ("timeShooting").f);

		switch (shoot.GetField ("weapon").str) {
			case "bazooka" :
			print ("바주카 발사");
			weapon = Instantiate(bulletPrefab);
			break;
			case "hbomb":
			print ("할렐루야 발사");
			weapon = Instantiate(hbombPrefab);
			break;
		}

		//카메라 설정
		GameObject.Find ("MainCamera").GetComponent<Camera2D> ().SetTarget (weapon.transform);
	
		weapon.transform.position = bulletInitialTransform.position;
		print (shoot.GetField ("timeShooting").f);
		weapon.GetComponent<Rigidbody2D>().AddForce(shootDirection*bulletMaxInitialVelocity*(shoot.GetField("timeShooting").f/maxTimeShooting));
	}
	
	public void setBazooka(){
		gunTransform.gameObject.SetActive(true);
	}

	public void unsetBazooka(){
		gunTransform.gameObject.SetActive(false);
	}

	//수류탄 셋
	public void setBomb(){
	
	}

	//수류탄 언셋
	public void unsetBomb(){

	}

	public void setBat(){
		an.SetBool ("setbat", true);

	}

	public void unsetBat(){
		print ("야구배트 언셋되었다");
		an.SetBool ("setbat", false);

	}

	public void batting(JSONObject player){
		Vector2 batDirection = new Vector2(player.GetField("player").GetField("shootVectorX").f,player.GetField("player").GetField("shootVectorY").f);
		transform.GetChild (0).GetChild (0).GetComponent<Bat> ().dirVec = batDirection;
		an.SetTrigger ("batting");
	}

	public void batted(JSONObject dirVec){
		rb.AddForce(new Vector2(dirVec.GetField("dirVecX").f,dirVec.GetField("dirVecY").f)*500f);
	}


	public void setJetPack(){
		print ("젯트팩장착");
		an.SetBool("jetpack",true);
		jetpackTransform.gameObject.SetActive (true);
	}
	
	public void unsetJetPack(){
		an.SetBool("jetpack",false);
		jetpackTransform.gameObject.SetActive (false);
	}
	
	public void updateJetpack(){
		//transform.Translate (Vector2.up * runSpeed * Time.fixedDeltaTime);
		rb.velocity = Vector2.up * 200f * Time.fixedDeltaTime;
		//rb.velocity=new Vector2(rb.velocity.x,3.5f);
	}

	public void setSheep(){
		an.SetBool ("setsheep",true);
	}

	public void unsetSheep(){
		an.SetBool ("setsheep", false);
	}

	public void shootSheep(string type){

		float dir;
		GameObject sheep=null;

		//sheep 프리팹 초기위치 설정.
		if (bodyTransform.localScale.x < 0) {
			print ("양 위치 오른편으로 설정");
			sheepInitialTransform.localPosition = new Vector3( Mathf.Abs(sheepInitialTransform.localPosition.x),sheepInitialTransform.localPosition.y,0f);
			dir = 1f;
		} else {
			print ("양 위치 왼쪽으로 설정");
			sheepInitialTransform.localPosition = new Vector3(-Mathf.Abs(sheepInitialTransform.localPosition.x),sheepInitialTransform.localPosition.y,0f);
			dir = -1f;
		}

		print (type);
		//sheep 프리팹 동적생성 및 초기설정 
		switch (type) {
			case "sheep":
				sheep = Instantiate(sheepPrefab);
				sheep.GetComponent<Sheep> ().SetDir(dir);
				
				AudioSource.PlayClipAtPoint(SheepSound,this.transform.position);
				
				//shoot 버튼을 양 폭발 버튼으로 변경.
				GameObject.Find ("ShootButton").GetComponent<WeaponController> ().currentWeapon = "explodeSheep";
				break;
			case "superSheep":
				sheep = Instantiate(superSheepPrefab);
				sheep.GetComponent<SuperSheep> ().SetDir(Vector2.up);
				
				AudioSource.PlayClipAtPoint(superSheepSound,this.transform.position);
				break;
		}

		sheep.name = this.name + "sheep";
		sheep.transform.position = sheepInitialTransform.position;

		//카메라 설정
		GameObject.Find ("MainCamera").GetComponent<Camera2D> ().SetTarget (sheep.transform);

		//ui버튼의 컨트롤을 양으로 변경.
		GameObject.Find("RightButton").GetComponent<RightButtonController>().currentControl = type;
		GameObject.Find ("LeftButton").GetComponent<LeftButtonController> ().currentControl = type;
		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = type;
	}


	public void setDonkey(){
		an.SetBool ("setdonkey",true);
	}
	
	public void unsetDonkey(){
		an.SetBool ("setdonkey", false);
	}
	
	public void shootDonkey(JSONObject marker){

		GameObject donkey = Instantiate (donkeyPrefab);

		donkey.name = "donkey";
		donkey.transform.position = new Vector3 (marker.GetField ("markerPositionX").f, marker.GetField ("markerPositionY").f, 0);

		//카메라 설정
		GameObject.Find ("MainCamera").GetComponent<Camera2D> ().SetTarget (donkey.transform);
	}

	public void setHbomb(){
		an.SetBool ("sethbomb", true);
	}

	public void unsetHbomb(){
		an.SetBool ("sethbomb", false);
	}

	public void setTeleport(){
		an.SetBool ("setTeleport", true);
	}

	public void unsetTeleport(){
		an.SetBool("setTeleport",false);
	}
	
	public IEnumerator teleporting(JSONObject marker){
		an.SetTrigger ("teleporting");

		yield return new WaitForSeconds(1.5f);

		this.transform.position = new Vector3 (marker.GetField ("markerPositionX").f, marker.GetField ("markerPositionY").f, 0);
		wc.setNoWeapon ();

		if(sc.clientId == tc.currentTurn){
			tc.Invoke("reqTurnChange",2f);
		}


		yield break;
	}

	public void setRope(){
		an.SetBool("setRope",true);
	}
	
	public void unsetRope(){
		an.SetBool ("setRope",false);
	}


	public void shootRope(JSONObject shootVector){

		//로프 게임 오브젝트 생성 및 이름설정.
		Destroy(GameObject.Find ("Rope"));
		rope = Instantiate(ropePrefab);
		rope.name = "Rope";
	
		//rope.transform.SetParent(this.transform);


		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "rope";

		rope.GetComponent<Rope>().hinge.enabled = true;
		rope.GetComponent<Rope>().line.enabled = true;

		this.GetComponent<HingeJoint2D>().enabled = true;
		
		
		RaycastHit2D hit = Physics2D.Raycast(bulletInitialTransform.position,new Vector2(shootVector.GetField("shootVectorX").f,shootVector.GetField("shootVectorY").f));
		
		if(hit.collider.tag == "Ground"){
			//rope.GetComponent<Rope>().line.SetPosition(0,bodyTransform.position);

			rope.GetComponent<Rope>().line.SetPosition(1,new Vector3(hit.point.x,hit.point.y,-10));
			rope.GetComponent<Rope>().hinge.connectedBody = hit.collider.GetComponent<Rigidbody2D>();
			
			//접착지점에 게임 오브젝트 생성.
			rope.GetComponent<Rope>().hitPosition.transform.position = hit.point;
			rope.GetComponent<Rope>().hitPosition.transform.SetParent(hit.collider.transform);


			rope.GetComponent<Rope>().hinge.connectedAnchor = rope.GetComponent<Rope>().hitPosition.transform.localPosition;
			rope.GetComponent<Rope>().hinge.anchor = new Vector2(new Vector2(bodyTransform.position.x-hit.point.x,bodyTransform.position.y-hit.point.y).magnitude,0);
		
			this.GetComponent<HingeJoint2D>().connectedBody = rope.GetComponent<Rigidbody2D>();
		}
		
	}
	
	
	public void ropeJump(){
		rope.GetComponent<Rope>().hinge.enabled = false;
		rope.GetComponent<Rope>().line.enabled = false;
		rope.GetComponent<Rope>().playerHinge.enabled = false;
		
		GameObject.Find ("JumpButton").GetComponent<JumpButtonController> ().currentControl = "player";
	}
	
	public void ropeDown(){
		print ("왜안돼!");
		rope.GetComponent<Rope>().hinge.anchor = new Vector2(rope.GetComponent<Rope>().hinge.anchor.x+0.1f,0);
	}
	
	public void ropeUp(){
		rope.GetComponent<Rope>().hinge.anchor = new Vector2(rope.GetComponent<Rope>().hinge.anchor.x-0.1f,0);
	}



	public void cough(){
		AudioSource.PlayClipAtPoint(coughSound,this.transform.position);
	}


	void displayHP(){
		Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
		PlayerHP.SetParent (this.transform);
		pos.x -= 0.03f; 
		pos.y += 0.15f;
		PlayerHP.position = pos;
	
		PlayerHP.GetComponent<GUIText> ().fontStyle = FontStyle.Bold;
		PlayerHP.GetComponent<GUIText> ().alignment = TextAlignment.Center;
		PlayerHP.GetComponent<GUIText> ().text = this.name + "\n"+this.health;
	}

	public Vector3 getPlayerPosition(){
		Vector3 playerPosition = GameObject.Find (sc.clientId).GetComponent<Transform> ().transform.position;
		return playerPosition; 
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround = true;
		}
		if(sc.clientId == this.name){
			Socket.Emit ("req_move_stop",new Json("moveDir","stop","playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y).json);
		}
			//Socket.Emit ("req_player_position_sync",new Json("moveDir","right","playerPositionX",getPlayerPosition().x,"playerPositionY",getPlayerPosition().y).json);
	}

	public void OnCollisionStay2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround = true;
			//StartCoroutine ("syncPlayer");
		}
	}

	public void OnCollisionExit2D(Collision2D coll){
		if (coll.gameObject.tag == "Ground") {
			isGround=false;
			//StopCoroutine ("syncPlayer");
		}
	}




}
