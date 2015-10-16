using UnityEngine;
using System.Collections;


/*http://stackoverflow.com/questions/25752429/how-to-get-height-of-2d-gamobject-with-sprite-renderer-component-in-unity3d-4-5*/

public class GroundController : MonoBehaviour {

	public string which;

	private SpriteRenderer sr;
	private float widthWorld, heightWorld;
	private int widthPixel, heightPixel;
	private Color transp; 

	// Start() de GroundController
	void Start(){
		sr = GetComponent<SpriteRenderer>(); 

		Texture2D tex = (Texture2D) Resources.Load(which);

		Texture2D tex_clone = (Texture2D)Instantiate(tex);


		sr.sprite = Sprite.Create(tex_clone, 
		                          new Rect(0f,0f, tex_clone.width, tex_clone.height),
		                          new Vector2(0.5f, 0.5f), 100f);

		transp = new Color(0f, 0f, 0f, 0f);
		InitSpriteDimensions();
	//	BulletController.groundController = this;
	}



	private void InitSpriteDimensions() {

		widthWorld = sr.bounds.size.x;
		heightWorld = sr.bounds.size.y;

		widthPixel = sr.sprite.texture.width;
		heightPixel = sr.sprite.texture.height;
	}

	void Update () {
	
	}

	public void DestroyGround(JSONObject coll){
		V2int c = World2Pixel(coll.GetField("centerX").f, coll.GetField("centerY").f);

		int r = Mathf.RoundToInt(coll.GetField("sizeX").f*widthPixel/widthWorld);


		Debug.Log (c);
		Debug.Log (r);
		int x, y, px, nx, py, ny, d;
		
		for (x = 0; x <= r; x++)
		{
			d = (int)Mathf.RoundToInt(Mathf.Sqrt(r * r - x * x));

			for (y = 0; y <= d; y++)
			{
				px = c.x + x;
				nx = c.x - x;
				py = c.y + y;
				ny = c.y - y;

				sr.sprite.texture.SetPixel(px, py, transp);
				sr.sprite.texture.SetPixel(nx, py, transp);
				sr.sprite.texture.SetPixel(px, ny, transp);
				sr.sprite.texture.SetPixel(nx, ny, transp);
			}
		}

		sr.sprite.texture.Apply();
		Destroy(GetComponent<PolygonCollider2D>());
		gameObject.AddComponent<PolygonCollider2D>();
	}

	private V2int World2Pixel(float x, float y) {
		V2int v = new V2int();
		
		float dx = x-transform.position.x;
		v.x = Mathf.RoundToInt(0.5f*widthPixel+ dx*widthPixel/widthWorld);
		
		float dy = y - transform.position.y;
		v.y = Mathf.RoundToInt(0.5f * heightPixel + dy * heightPixel / heightWorld);
		
		return v;
	}

}
