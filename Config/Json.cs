using UnityEngine;
using System.Collections;
using SocketIO;


public class Json : MonoBehaviour {

	public JSONObject json;

	public Json(string name,float arg){
		json = new JSONObject ();
		json.AddField (name, arg);
	}

	public Json(string name,string arg){
		json = new JSONObject ();
		json.AddField (name, arg);
	}

	public Json(string name,bool flag){
		json = new JSONObject ();
		json.AddField (name, flag);
	}
	
	public Json(string name1,string arg1,string name2,string arg2){	
		json = new JSONObject ();
		json.AddField (name1, arg1);
		json.AddField (name2, arg2);
	}
	

	public Json(string name1,float arg1,string name2,float arg2){
		json = new JSONObject ();
		json.AddField (name1, arg1);
		json.AddField (name2, arg2);
	}

	

	public Json(string name1,float arg1,string name2,string arg2){
		json = new JSONObject ();
		json.AddField (name1,arg1);
		json.AddField (name2, arg2);
	}

	public Json(string name1,float arg1,string name2,float arg2,string name3,float arg3){
		json = new JSONObject ();
		json.AddField (name1, arg1);
		json.AddField (name2, arg2);
		json.AddField (name3, arg3);
	}

	public Json(string name1,string arg1,string name2,float arg2,string name3,float arg3){
		json = new JSONObject ();
		json.AddField (name1, arg1);
		json.AddField (name2, arg2);
		json.AddField (name3, arg3);
	}

	public Json(string name1,string arg1,string name2,float arg2,string name3,float arg3,string name4,float arg4){
		json = new JSONObject ();
		json.AddField (name1, arg1);
		json.AddField (name2, arg2);
		json.AddField (name3, arg3);
		json.AddField (name4, arg4);
	}

}
