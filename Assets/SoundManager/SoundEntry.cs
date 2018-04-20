using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SoundEntry : ScriptableObject
{

	public AudioClip soundEffect;

	[Range (0f, 1f)] public float volume = 1.0f;
	[Range (0f, 0.5f)] public float pitchModulation = 0.05f;
	// public Effect[] effects;

	public bool isActive = true;

	[HideInInspector] public float lastPlayed = -1.0f;
	[HideInInspector] public float shouldPlayNext = -1.0f;

	protected virtual void _Play (AudioSource player, float volume)
	{
		player.clip = soundEffect; // set the clip to secondary audio player
		player.pitch = (1.0f + pitchModulation * (Random.value - 0.5f) * 2.0f);
		player.volume = this.volume * volume;
		player.time = 0f;
		player.Play ();

		//foreach (Effect e in effects)
		//	e.Play ();
	}

	public virtual void Play (AudioSource player, float volume)
	{
		this.shouldPlayNext = Time.time + soundEffect.length;
		this._Play (player, volume);
	}


	public virtual void OnEnable ()
	{
		this.shouldPlayNext = Time.time;
		this.lastPlayed = Time.time;
	}


	[MenuItem ("Assets/Create/Sound/SoundEntry")]
	public static void CreateSoundEntry ()
	{
		var asset = ScriptableObject.CreateInstance<SoundEntry> ();
		ProjectWindowUtil.CreateAsset (asset, "NewSoundEntry.asset");
	}


}
