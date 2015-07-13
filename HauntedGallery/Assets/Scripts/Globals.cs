using UnityEngine;
using System.Collections;

public class Globals {

	public static float iWidth = 10f;
	public static float iHalfWidth = iWidth / 2;
	public static float iHeight = 12f;
	public static float iHalfHeight = iHeight / 2;
	public static float iLength = 30f;
	public static float iHalfLength = iLength / 2;
	public static float wallWidth = 2f;
	public static Vector3 LEFT = new Vector3(-1f, 0f, 0f);
	public static  Vector3 RIGHT = new Vector3(1f, 0f, 0f);
	public static bool developMode = false;

	public static float totalWidthUnits = Globals.iLength * 2f + Globals.iWidth;
	public static float totalHeightUnits = Globals.iHeight;
	public static float guiUnit = Screen.width / totalWidthUnits;
	public static float guiYStart = Screen.height - guiUnit * totalHeightUnits;
	
	private static Texture2D _staticRectTexture= new Texture2D( 1, 1 );
	private static GUIStyle _staticRectStyle;
	
	// Note that this function is only meant to be called from OnGUI() functions.
	public static void GUIDrawRectPreScale( Rect position, Color color, string s = "" ){
		Rect r = position;
		r.center = new Vector2(r.center.x * guiUnit, (iHeight - r.center.y) * guiUnit + guiYStart);
		r.size = new Vector2(r.size.x * guiUnit, r.size.y * guiUnit);
		GUIDrawRect(r, color, s);
	}
	
	public static void GUIDrawRect( Rect position, Color color, string s = "" )
	{
		if( _staticRectStyle == null )
		{
			_staticRectStyle = new GUIStyle();
		}
		
		_staticRectTexture.SetPixel( 0, 0, color );
		_staticRectTexture.Apply();
		
		_staticRectStyle.normal.background = _staticRectTexture;
		GUI.skin.box.normal.background = _staticRectTexture;
		GUI.Box( position, s, _staticRectStyle);
	}

	static GameManager _gameManager = null;
	public static GameManager gameManager{
		get{
			if(_gameManager == null)
				_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
			return _gameManager;
		}
	}
	
	static PictureManager _pictureManager = null;
	public static PictureManager pictureManager{
		get{
			if(_pictureManager == null)
				_pictureManager = GameObject.Find("PictureManager").GetComponent<PictureManager>();
			return _pictureManager;
		}
	}
	/*
	public static void GUIDrawRect( Rect position, Color color )
	{
		if( _staticRectTexture == null )
		{
			_staticRectTexture = new Texture2D( 1, 1 );
		}
		
		if( _staticRectStyle == null )
		{
			_staticRectStyle = new GUIStyle();
		}
		
		_staticRectTexture.SetPixel( 0, 0, color );
		_staticRectTexture.Apply();
		
		_staticRectStyle.normal.background = _staticRectTexture;
		
		GUI.Box( position, GUIContent.none, _staticRectStyle );
	}
	*/
}
