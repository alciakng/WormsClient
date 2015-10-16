using UnityEngine;
using System.Collections;
using System.Diagnostics;


public class StopWatch : MonoBehaviour {
	
	public Stopwatch sw;

	// Use this for initialization
	void Start () {
		sw = new Stopwatch();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StopWatchReset(){
		sw.Reset();
	}

	public void StopWatchStart(){
		sw.Start ();
	}

	public void StopWatchStop(){
		sw.Stop ();
	}

	public float GetElapsedMilliseconds(){
		return sw.ElapsedMilliseconds;  
	}


}
