using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;


public class Menu : MonoBehaviour {


	public Animator an;
	public CanvasGroup cg;

	
	public bool IsOpen
	{
		get{return an.GetBool("IsOpen");}
		set{ an.SetBool("IsOpen",value);}
	}
	
	// Use this for initialization
	void Awake () {
		an = GetComponent<Animator> ();
		cg = GetComponent<CanvasGroup> ();

		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}

	void Update(){

		if (!an.GetCurrentAnimatorStateInfo (0).IsName ("open")) {
			cg.blocksRaycasts = cg.interactable = false;
		} else {
			cg.blocksRaycasts = cg.interactable = true; 
		}
	}


}
