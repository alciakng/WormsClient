using UnityEngine;
using System.Collections;
using SocketIO;
using UnityEngine.UI;


public class GameMenu : MonoBehaviour {

	private Animator an;


	void Awake () {
		an = GetComponent<Animator> ();
	}

	public bool IsOpen
	{
		get{return an.GetBool("IsOpen");}
		set{ an.SetBool("IsOpen",value);}
	}

}

