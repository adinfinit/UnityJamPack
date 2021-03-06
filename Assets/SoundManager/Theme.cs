﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Theme : ScriptableObject
{
	public List<SoundEntry> sounds;
	private static Texture2D logo;

	[MenuItem ("Assets/Create/Sound/Theme")]
	public static void Init()
	{
		var asset = ScriptableObject.CreateInstance<Theme> ();
		logo = Resources.Load("Assets/Editor/Icons/sfx_icon.png", typeof(Texture2D)) as Texture2D; 
		ProjectWindowUtil.CreateAsset (asset, "NewTheme.asset");
	}

	void OnGui()
	{
		GUILayout.Label (logo);
	}


}