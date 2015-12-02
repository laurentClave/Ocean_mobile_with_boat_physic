using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Ocean))]
public class OceanGeneratorInspector : Editor
{

	static public Texture2D blankTexture
	{
		get
		{
			return EditorGUIUtility.whiteTexture;
		}
	}

	private Texture2D logo;
	private Texture2D back;

	private int oldEveryXframe;

	private string GetPluginPath() {
		MonoScript ms = MonoScript.FromScriptableObject( this );
		string scriptPath = AssetDatabase.GetAssetPath( ms );
		
		var directoryInfo = Directory.GetParent( scriptPath ).Parent;
		return directoryInfo != null ? directoryInfo.FullName : null;
	}

	private string FilePathToAssetPath(string filePath) {
		int indexOfAssets = filePath.LastIndexOf("Assets");
		
		return filePath.Substring(indexOfAssets);
	}

	void OnEnable ()
	{
		//Load dynamic resources
		string pluginPath = FilePathToAssetPath( GetPluginPath() );

		string logoPath = Path.Combine(pluginPath, "Editor");
		logoPath = Path.Combine(logoPath, "OceanBanner.png");
		
		logo = AssetDatabase.LoadAssetAtPath<Texture2D>(logoPath);
		
		if (null == logo)
			Debug.LogError("null == logo");

		string backPath = Path.Combine(pluginPath, "Editor");
		backPath = Path.Combine(backPath, "Background.png");
		
		back = AssetDatabase.LoadAssetAtPath<Texture2D>(backPath);
		
		if (null == back)
			Debug.LogError("null == back");
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.BeginHorizontal(); 
		GUILayout.FlexibleSpace();
		DrawBackground ();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUI.backgroundColor = new Color(.1f, 1f, 1f, 1f);
		GUI.contentColor = new Color(1f, 1f, 1f, 1f);

        EditorGUIUtility.labelWidth = 80F;// LookLikeControls(80f);
		Ocean ocean = target as Ocean;

		DrawSeparator();
		 
		    GUILayout.Space(-15);

		    EditorGUILayout.BeginHorizontal(); 
		    float bannerWidth = 256;
		    GUILayout.Space((Screen.width * 0.5f) - (bannerWidth * 0.55f));
		    EditorGUILayout.LabelField(new GUIContent( logo ), GUILayout.Width( bannerWidth), GUILayout.Height( bannerWidth * 0.40f ));
		    EditorGUILayout.EndHorizontal();

		    GUILayout.Space(-20);

	    DrawSeparator();

		DrawVerticalSeparator();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginVertical();

    		EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Targets");
            GUILayout.Space(16);
            EditorGUILayout.EndVertical();

		    EditorGUILayout.BeginVertical();
		    EditorGUILayout.LabelField("Target/Player");
		    ocean.player = (Transform) EditorGUILayout.ObjectField(ocean.player , typeof(Transform), true, GUILayout.MinWidth(200));
		    EditorGUILayout.EndVertical();
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Follow");
		    ocean.followMainCamera = EditorGUILayout.Toggle(ocean.followMainCamera);
		    
		    EditorGUILayout.EndHorizontal();

		    EditorGUILayout.LabelField("Ocean material");
			ocean.material = (Material) EditorGUILayout.ObjectField(ocean.material , typeof(Material),true);
		
		    EditorGUILayout.LabelField("Ocean shader");
			ocean.oceanShader = (Shader) EditorGUILayout.ObjectField(ocean.oceanShader , typeof(Shader),true);

		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Interactive water");

		    ocean.waterInteractionEffects = EditorGUILayout.Toggle(ocean.waterInteractionEffects);
		    EditorGUILayout.EndHorizontal();

		    GUILayout.Space(56);

		DrawSeparator(); 

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Waves settings");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();
		
		    EditorGUILayout.LabelField("Scale");
			ocean.scale = (float)EditorGUILayout.Slider(ocean.scale, 0, 100);
		
		    EditorGUILayout.LabelField("Choppy scale");
		ocean.choppy_scale = (float)EditorGUILayout.Slider(ocean.choppy_scale, 0, 100);
		
		    EditorGUILayout.LabelField("Waves speed");
			ocean.speed = (float)EditorGUILayout.Slider(ocean.speed, 0.1f, 3f);

		    EditorGUILayout.LabelField("Waves Offset animation speed");
		    ocean.waveSpeed = (float)EditorGUILayout.Slider(ocean.waveSpeed, 0.1f, 10f);
		
		    EditorGUILayout.LabelField("Wake distance");
			ocean.wakeDistance = (float)EditorGUILayout.Slider(ocean.wakeDistance, 1f, 15f);
		
		DrawSeparator(); 
		 
		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Mist");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();
		
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Enable Mist");
		    ocean.mistEnabled = EditorGUILayout.Toggle(ocean.mistEnabled);
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Mist Low");
		    GUILayout.Space(-170);
		    ocean.mistLow = (GameObject) EditorGUILayout.ObjectField(ocean.mistLow , typeof(GameObject),true, GUILayout.MaxWidth(130));
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Mist Hi");
		    GUILayout.Space(-170);
		    ocean.mist = (GameObject) EditorGUILayout.ObjectField(ocean.mist , typeof(GameObject),true, GUILayout.MaxWidth(130));
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Mist Clouds");
		    GUILayout.Space(-170);
		    ocean.mistClouds = (GameObject) EditorGUILayout.ObjectField(ocean.mistClouds , typeof(GameObject),true, GUILayout.MaxWidth(130));
		    EditorGUILayout.EndHorizontal();

		DrawSeparator();

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Wind");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();
		
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Dynamic waves");

		    ocean.dynamicWaves = EditorGUILayout.Toggle(ocean.dynamicWaves);
		    EditorGUILayout.EndHorizontal();
		
		    EditorGUILayout.LabelField("Wind power");
		    ocean.humidity = (float)EditorGUILayout.Slider(ocean.humidity, 0.01f, 1f);
		
		    EditorGUILayout.LabelField("Wind direction");
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("X");
		    GUILayout.Space(-100);
		    ocean.pWindx = EditorGUILayout.FloatField(ocean.pWindx);
		    EditorGUILayout.LabelField("Y");
		    GUILayout.Space(-100);
		    ocean.pWindy = EditorGUILayout.FloatField(ocean.pWindy);
		    EditorGUILayout.EndHorizontal();

			GUILayout.Space(25);
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Spread along frames");

		    ocean.spreadAlongFrames = EditorGUILayout.Toggle(ocean.spreadAlongFrames);
		    EditorGUILayout.EndHorizontal();


		EditorGUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Space(20);
		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginVertical();

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Tiles settings");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();
		
		    EditorGUILayout.LabelField("Tiles count");
		    ocean.tiles = (int)EditorGUILayout.Slider(ocean.tiles, 1, 15);
		    EditorGUILayout.LabelField("Tiles size");
		    ocean.size = EditorGUILayout.Vector3Field("",ocean.size);
		
		    EditorGUILayout.LabelField("Tiles poly count");
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Width");
		    GUILayout.Space(-90);
		    ocean.width = EditorGUILayout.IntField(ocean.width);
		    EditorGUILayout.LabelField("Height");
		    GUILayout.Space(-90);
		    ocean.height = EditorGUILayout.IntField(ocean.height);
		    EditorGUILayout.EndHorizontal();
		
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Fixed tiles");
		    ocean.fixedTiles = EditorGUILayout.Toggle(ocean.fixedTiles);
		    EditorGUILayout.EndHorizontal();
		
		    EditorGUILayout.LabelField("Fixed tiles distance");
		    ocean.fTilesDistance = (int)EditorGUILayout.Slider(ocean.fTilesDistance, 1, 5);
		
		    EditorGUILayout.LabelField("Fixed tiles lod");
		    ocean.fTilesLod = (int)EditorGUILayout.Slider(ocean.fTilesLod, 0, 5);

		////------------------------------------------
		    GUILayout.Space(14);

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Reflection & Refraction");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();

		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Render reflection");
		    ocean.renderReflection = EditorGUILayout.Toggle(ocean.renderReflection);
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Render refraction");;
		    ocean.renderRefraction = EditorGUILayout.Toggle(ocean.renderRefraction);
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.LabelField("Render textures size");
		    EditorGUILayout.BeginHorizontal();
		    EditorGUILayout.LabelField("Width");
		    GUILayout.Space(-90);
		    ocean.renderTexWidth = EditorGUILayout.IntField(ocean.renderTexWidth);
		    EditorGUILayout.LabelField("Height");
		    GUILayout.Space(-90);
		    ocean.renderTexHeight = EditorGUILayout.IntField(ocean.renderTexHeight);
		    EditorGUILayout.EndHorizontal();
		    EditorGUILayout.LabelField("Reflection clip plane offset");
		    ocean.m_ClipPlaneOffset = EditorGUILayout.FloatField(ocean.m_ClipPlaneOffset);
		    
		    EditorGUILayout.LabelField("Render layers");
		    int mask = LayerMaskField(ocean.renderLayers);
		
		    if (ocean.renderLayers != mask)
		    {
			    ocean.renderLayers = mask;
		    }

		////------------------------------------------
		    GUILayout.Space(48);

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Sun reflection");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();
		
		    EditorGUILayout.LabelField("Sun transform");
		    ocean.sun = (Transform) EditorGUILayout.ObjectField(ocean.sun , typeof(Transform),true);
		
		    EditorGUILayout.LabelField("Sun direction");
		    ocean.SunDir = EditorGUILayout.Vector3Field("",ocean.SunDir);

		////------------------------------------------
		    GUILayout.Space(14);

		    EditorGUI.DropShadowLabel(EditorGUILayout.BeginVertical(), "Water color");
		    GUILayout.Space(16);
		    EditorGUILayout.EndVertical();

		    GUI.backgroundColor = Color.white;
		    GUI.contentColor = Color.white;
		
		    EditorGUILayout.LabelField("Water color");
		    ocean.waterColor = EditorGUILayout.ColorField(ocean.waterColor);

		    EditorGUILayout.LabelField("Water surface color");
		    ocean.surfaceColor = EditorGUILayout.ColorField(ocean.surfaceColor);

		    GUI.backgroundColor = new Color(1f, 1f, 0f, 1f);
		    GUI.contentColor = new Color(1f, 1f, 0f, 1f);

			

		    GUILayout.Space(25);
			DrawSeparator();
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.white;

			if(ocean.spreadAlongFrames) {
				EditorGUILayout.LabelField("Calc waves every x frames:");
				EditorGUILayout.BeginHorizontal();
				ocean.everyXframe = (int)EditorGUILayout.Slider(ocean.everyXframe, 3, 8);

				if(oldEveryXframe != ocean.everyXframe) {
					oldEveryXframe = ocean.everyXframe;
					if(ocean.everyXframe > 3) { ocean.fr1=0; ocean.fr2=1; ocean.fr3=2; ocean.fr4=3; }
					 else {ocean.fr1=0; ocean.fr2=1; ocean.fr3=2; ocean.fr4=2;}
				}

				EditorGUILayout.EndHorizontal();
			} else {
				EditorGUILayout.LabelField("");
				EditorGUILayout.LabelField("");
			}

			GUILayout.Space(15);


		   // EditorGUILayout.LabelField("Editor script by 'MindBlocks Studio'", GUILayout.MinWidth(170));

		EditorGUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		DrawSeparator();
		
		if (GUI.changed)
        {
            EditorUtility.SetDirty (ocean);
        }	
	}
	
	public static int LayerMaskField (string label, int mask, params GUILayoutOption[] options)
	{
		List<string> layers = new List<string>();
		List<int> layerNumbers = new List<int>();

		string selectedLayers = "";

		for (int i = 0; i < 32; ++i)
		{
			string layerName = LayerMask.LayerToName(i);

			if (!string.IsNullOrEmpty(layerName))
			{
				if (mask == (mask | (1 << i)))
				{
					if (string.IsNullOrEmpty(selectedLayers))
					{
						selectedLayers = layerName;
					}
					else
					{
						selectedLayers = "Mixed";
					}
				}
			}
		}

		if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
		{
			if (mask == 0)
			{
				layers.Add("Nothing");
			}
			else if (mask == -1)
			{
				layers.Add("Everything");
			}
			else
			{
				layers.Add(selectedLayers);
			}
			layerNumbers.Add(-1);
		}

		layers.Add((mask == 0 ? "[+] " : "      ") + "Nothing");
		layerNumbers.Add(-2);

		layers.Add((mask == -1 ? "[+] " : "      ") + "Everything");
		layerNumbers.Add(-3);

		for (int i = 0; i < 32; ++i)
		{
			string layerName = LayerMask.LayerToName(i);

			if (layerName != "")
			{
				if (mask == (mask | (1 << i)))
				{
					layers.Add("[+] " + layerName);
				}
				else
				{
					layers.Add("      " + layerName);
				}
				layerNumbers.Add(i);
			}
		}

		bool preChange = GUI.changed;

		GUI.changed = false;

		int newSelected = 0;

		if (Event.current.type == EventType.MouseDown)
		{
			newSelected = -1;
		}

		if (string.IsNullOrEmpty(label))
		{
			newSelected = EditorGUILayout.Popup(newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		}
		else
		{
			newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		}

		if (GUI.changed && newSelected >= 0)
		{
			if (newSelected == 0)
			{
				mask = 0;
			}
			else if (newSelected == 1)
			{
				mask = -1;
			}
			else
			{
				if (mask == (mask | (1 << layerNumbers[newSelected])))
				{
					mask &= ~(1 << layerNumbers[newSelected]);
				}
				else
				{
					mask = mask | (1 << layerNumbers[newSelected]);
				}
			}
		}
		else
		{
			GUI.changed = preChange;
		}
		return mask;
	}

	public static int LayerMaskField (int mask, params GUILayoutOption[] options)
	{
		return LayerMaskField(null, mask, options);
	}

	public void DrawSeparator ()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.Space(12f);
		
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = blankTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
		EditorGUILayout.EndVertical();
	}

	public void DrawVerticalSeparator ()
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = blankTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(Screen.width * 0.5f, rect.yMin + 10f, 4f, 695f), tex);
			GUI.DrawTexture(new Rect(Screen.width * 0.5f, rect.yMin + 10f, 1f, 695f), tex);
			GUI.DrawTexture(new Rect(Screen.width * 0.5f + 3f, rect.yMin + 10f, 1f, 695), tex);
			GUI.color = Color.white;
		}
	}

	public void DrawBackground ()
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = back;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0.6f, 0.6f, 0.6f, 1f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 812), tex);
			GUI.color = Color.white;
		}
	}
}