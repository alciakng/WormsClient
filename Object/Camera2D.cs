using UnityEngine;
using System.Collections;


public class Camera2D : MonoBehaviour {

	//터치에 따른 카메라 이동
	public float moveSensitivityX = 1.0f;
	public float moveSensitivityY = 1.0f;
	public bool updateZoomSensitivity = true;
	public float ZoomSpeed = 0.05f;
	public float minZoom = 1.0f;
	public float maxZoom = 20.0f;
	public bool invertMoveX = false;
	public bool invertMoveY = false;


	private Transform _transform;
	private Camera _camera;

	//터치로 카메라 이동을 하고 있는지 여부
	public bool touchMoved;

	
	private Vector2 velocity;

	public float smoothTimeY;
	public float smoothTimeX;

	public Transform target;

	//소켓 연결객체
	private ServerConnect sc;
	
	// Use this for initialization
	void Start () {
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		target = GameObject.Find (sc.clientId).GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

//		print (Camera.main.ViewportToWorldPoint (new Vector3 (0, 1)));

		if (updateZoomSensitivity) {
			moveSensitivityX = Camera.main.orthographicSize/2.0f;
			moveSensitivityY = Camera.main.orthographicSize/2.0f;
		}

		Touch[] touches = Input.touches;

		if (touches.Length ==2) {
			if(touches[0].phase == TouchPhase.Moved && touches[1].phase == TouchPhase.Moved){
				touchMoved =true;

				Touch touchOne = touches[0];
				Touch touchTwo = touches[1];

				//카메라 이동
				Vector2 delta = touchOne.deltaPosition;

				float positionX =  -delta.x * moveSensitivityX * Time.deltaTime;
				float positionY =  -delta.y * moveSensitivityY * Time.deltaTime;

				Vector3 prevPosition = transform.position;
				transform.position += new Vector3(positionX,positionY,0);

				//카메라 줌
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;
				
				float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
				float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;
				
				float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;

				float prevOrthosize = Camera.main.orthographicSize;
				Camera.main.orthographicSize += deltaMagDiff*ZoomSpeed;
				 
				if(transform.position.y + Camera.main.orthographicSize>74 ||transform.position.y -Camera.main.orthographicSize<-12 || transform.position.x + Camera.main.orthographicSize*Camera.main.aspect>170 || transform.position.x - Camera.main.orthographicSize*Camera.main.aspect<-40){
					transform.position = prevPosition;
					Camera.main.orthographicSize = prevOrthosize;
				}
			}
		}

		if (!touchMoved && target!= null) {

			Camera.main.orthographicSize =5f;

			float posX = Mathf.SmoothDamp (transform.position.x, target.position.x, ref velocity.x, smoothTimeX);
			float posY = Mathf.SmoothDamp (transform.position.y, target.position.y, ref velocity.y, smoothTimeY);

			Vector3 prevPos = transform.position;
			transform.position = new Vector3 (posX, posY, transform.position.z);	

			if(posY + Camera.main.orthographicSize>74 ||posY -Camera.main.orthographicSize<-12 || posX + Camera.main.orthographicSize*Camera.main.aspect>170 || posX - Camera.main.orthographicSize*Camera.main.aspect<-40){
				transform.position = prevPos;
			}
		}
	}

	public void SetTarget(Transform _target){
		target = _target;
	}

}
