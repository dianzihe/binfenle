using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceDisplayer : MonoBehaviour{
	
	/*public GUISkin guiStyle;
	private Vector2 scrollPos = Vector2.zero;
	
	void Update()
	{
		//guiStyleStatic = guiStyle;
	}
	void OnGUI()
	{
		GUI.skin = guiStyle;
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(200), GUILayout.Height(200));
		GUILayout.Label("1");
		GUILayout.Label("2");
		GUILayout.Label("3");
		GUILayout.Label("4");
		GUILayout.Label("1");
		GUILayout.Label("2");
		GUILayout.Label("3");
		GUILayout.Label("4");
		GUILayout.Label("1");
		GUILayout.Label("2");
		GUILayout.Label("3");
		GUILayout.Label("4");
		GUILayout.Label("1");
		GUILayout.Label("2");
		GUILayout.Label("3");
		GUILayout.Label("4");
		GUILayout.EndScrollView();
	}*/
	
	private static Vector2   newPosition = Vector2.zero;
	
	private static Texture2D textureBg;
	
	public  static GUISkin   guiSkin;
	
	private enum RESOURCE_SELECTED 
	{
		OVERALL,
		TEXTURES,
		AUDIO_CLIPS,
		MESHES,
		MATERIALS,
		GAMEOBJECTS,
		COMPONENTS
	};
	
	private static System.Type[] types = 
	{
		typeof(Texture), typeof(Texture), typeof(AudioClip), typeof(Mesh), typeof(Material), typeof(GameObject), typeof(Component)
	};
	
	private static RESOURCE_SELECTED resourceSelected = RESOURCE_SELECTED.OVERALL;

	public void Start() {
		if(!Debug.isDebugBuild) {
			GameObject.Destroy(this);
		}
	}

	private bool visible = false;
	public void OnGUI() 
	{
		if(visible)
		{
			ResourceDisplayer.DrawInfo();
			if(GUILayout.Button("Hide"))
			{
				Resources.UnloadUnusedAssets();
				visible = false;
			}
		}
		else
		{	
			if(GUILayout.Button("Resources"))
			{
				Resources.UnloadUnusedAssets();
				visible = true;
			}
		}
	}
	
	public static void DrawInfo() 
	{
		if(textureBg == null)
		{
			Color[] pix = new Color[1];
	        for(int i = 0; i < pix.Length; i++)
	            pix[i] = new Color(1.0f, 1.0f, 1.0f,0.5f);
	        textureBg = new Texture2D(1, 1);
	        textureBg.SetPixels(pix);
	        textureBg.Apply();
		}
		
		if(guiSkin == null)
		{
			guiSkin = GUI.skin;
			
			guiSkin.window.normal.background = textureBg;
			
			guiSkin.verticalScrollbar.fixedWidth = 50;
			//guiSkin.verticalScrollbar.stretchWidth = true;
			
			guiSkin.verticalSlider.fixedWidth = 50;
			//guiSkin.verticalSlider.stretchHeight = true;
			
			//guiSkin.button.fontSize = 12;
		
			GUI.skin = guiSkin;
		}
		
		GUILayout.BeginVertical(guiSkin.window);
		if(resourceSelected == RESOURCE_SELECTED.OVERALL)
		{
			Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture));
			int nTextures = textures.Length;
			#if UNITY_EDITOR
			nTextures = 0;
			for(int i = 0; i < textures.Length; ++i) {
				if(textures[i].hideFlags != HideFlags.NotEditable) 
					continue;

				nTextures ++;
			}
			#endif

		    GUILayout.Label("All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
		    GUILayout.Label(
			                "Textures " + nTextures + "\n " +
			                "Size: " + SystemInfo.graphicsMemorySize
			                );
		    GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		    GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
		    GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
		    GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
		    GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
		}
		else
		{
			newPosition = GUILayout.BeginScrollView(newPosition,  GUILayout.Width (300), GUILayout.Height (400));
			
			switch(resourceSelected)
			{
				case RESOURCE_SELECTED.TEXTURES:
					GUIStyle  scrollViewStyle = new GUIStyle();
					scrollViewStyle.fixedWidth = 128.0f;
					scrollViewStyle.fixedHeight = 128.0f;
					scrollViewStyle.alignment = TextAnchor.MiddleCenter;

				    Texture[] allTextures = (Texture[])Resources.FindObjectsOfTypeAll(typeof(Texture));
					List< Texture > textures = new List< Texture >();
					foreach(Texture texture in allTextures)
					{
#if UNITY_EDITOR
						if(texture.hideFlags != HideFlags.NotEditable) 
					   		continue;
#endif
						textures.Add(texture);
					}

					//Sort by size
					textures.Sort(delegate(Texture tx1, Texture tx2) {return tx2.width * tx2.height - tx1.width * tx1.height;});

					foreach(Texture texture in textures)
					{
						GUILayout.BeginHorizontal();
							GUILayout.Label(texture, scrollViewStyle);
							GUILayout.Label(texture.name + "\nsize:" + texture.width + "x" + texture.height + "\n" + texture.hideFlags, scrollViewStyle);
						GUILayout.EndHorizontal();
					}
					break;
				
				default:
					createAllLabelsForResourceType(types[(int)resourceSelected]);
					break;
			}
			
		    
		    GUILayout.EndScrollView ();
		}
		GUILayout.BeginHorizontal();
			AddButton ("ALL", RESOURCE_SELECTED.OVERALL);
			AddButton ("TEXTURES", RESOURCE_SELECTED.TEXTURES);
			AddButton ("AUDIO_CLIPS", RESOURCE_SELECTED.AUDIO_CLIPS);
			//AddButton ("MESHES", RESOURCE_SELECTED.MESHES); //this crashes a lot so I' better disable it
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
			AddButton ("MATERIALS", RESOURCE_SELECTED.MATERIALS);
			AddButton ("GAMEOBJECTS", RESOURCE_SELECTED.GAMEOBJECTS);
			AddButton ("COMPONENTS", RESOURCE_SELECTED.COMPONENTS);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		if(GUILayout.Button("Unload Unused Assets"))
		{
			Resources.UnloadUnusedAssets();
		}
	}
	
	private static void AddButton(string text, RESOURCE_SELECTED resource)
	{
		if(GUILayout.Button(text))
		{
			resourceSelected = resource;
		}
	}
	
	private static void createAllLabelsForResourceType(System.Type type)
	{
		Object[] objects = Resources.FindObjectsOfTypeAll(type);
		for(int i = 0; i < objects.Length; ++i)
		{
			GUILayout.Label(objects[i].ToString());
		}
	}
}
