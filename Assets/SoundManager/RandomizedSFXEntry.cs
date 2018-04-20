using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RandomizedSFXEntry : SoundEntry
{
	
	[Range (0.001f, 15f)] public float minTimeBetween = 1.0f;
	[Range (0.002f, 15f)] public float maxTimeBetween = 5.0f;
	[Range (-1.0f, 1.0f)] public float minPan = -1.0f;
	[Range (-1.0f, 1.0f)] public float maxPan = 1.0f;

	public override void Play (AudioSource player, float volume)
	{
		player.panStereo = minPan + Random.value * (maxPan - minPan);

		float timeBetween = minTimeBetween + Random.value * Mathf.Abs (maxTimeBetween - minTimeBetween);
		this.shouldPlayNext = Time.time + timeBetween;

		this._Play (player, volume);
	}

	public override void OnEnable ()
	{
		if (this.soundEffect != null) {
			float timeBetween = minTimeBetween + Random.value * Mathf.Abs (maxTimeBetween - minTimeBetween);
			this.shouldPlayNext = Time.time + timeBetween;

			this.lastPlayed = Time.time;
		}
	}


	[MenuItem ("Assets/Create/Sound/RandomizedSFX")]
	public static void CreateRandomizedSFX ()
	{
		var asset = ScriptableObject.CreateInstance<RandomizedSFXEntry> ();
		ProjectWindowUtil.CreateAsset (asset, "NewRandomizedSFX.asset");
	}
}
