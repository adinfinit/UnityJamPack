using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

public class SoundManager : MonoBehaviour
{

	[Header ("Mixer")]
	public AudioMixerGroup mixer;

	[Header ("General attributes")]
	[Range (0.001f, 20f)] public float fadeTime = 5f;
	[Range (0f, 1f)] public float masterVolume = 0.5f;
	[Range (0f, 1f)] public float musicVolume = 1.0f;
	[Range (0f, 1f)] public float sfxVolume = 1.0f;
	[Range (1, 50)] public int numSFXPlayers = 10;

	[Header ("Playlist")]
	public AudioClip[] playlist;
	private AudioClip[] randomizedPlaylist;

	[Header ("Sound Effect Composer")]
	public Theme currentTheme;
	public Theme defaultTheme;
	private Theme newTheme;

	public List<Theme> themes;

	private float nextUpdateSFXComposer = -1.0f;
	private float updateIntervalSFXComposer = 0.5f;
	private int curIdx = 0;
	private AudioClip inSound;

	private AudioSource mainSound;
	private AudioSource secondarySound;

	private bool fading;
	private float startedFadingAt;

	private AudioSource[] sfxPlayers;
	private int cur_sfx_idx = 0;

	// true volumes used
	private float _musicVolume;
	private float _sfxVolume;

	private AnimationCurve fadeCurve;

	// Use this for initialization
	void Start ()
	{
		// default to defaultTheme or first in list
		currentTheme = defaultTheme;

		// set true volumes
		_musicVolume = musicVolume * masterVolume;
		_sfxVolume = sfxVolume * masterVolume;

		// Animation curve to good state automatically
		fadeCurve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));
		fadeCurve.preWrapMode = WrapMode.PingPong;
		fadeCurve.postWrapMode = WrapMode.PingPong;

		mainSound = gameObject.AddComponent<AudioSource> ();
		secondarySound = gameObject.AddComponent<AudioSource> ();

		this.sfxPlayers = new AudioSource[numSFXPlayers];

		for (int i = 0; i < this.numSFXPlayers; i++)
			sfxPlayers [i] = gameObject.AddComponent<AudioSource> ();

		mainSound.outputAudioMixerGroup = mixer;
		secondarySound.outputAudioMixerGroup = mixer;

		mainSound.Play ();
		secondarySound.Play ();

		fading = false;

		// checks every second if the mainSound is behaving as it should
		InvokeRepeating ("clampVolume", 0f, 1.0f);

		curIdx = Random.Range (0, playlist.Length - 1);
		PlayRandomMusicIndefinitely ();

	}


	public void playSFX (AudioClip c)
	{
		cur_sfx_idx = (cur_sfx_idx + 1) % numSFXPlayers;
		AudioSource player = sfxPlayers [cur_sfx_idx];

		player.clip = c; // set the clip to secondary audio player
		player.time = 0f;
		player.Play ();
	}

	// Update is called once per frame
	void Update ()
	{
		PlayMusic ();
		SFXComposer ();
	}

	public void changeTheme (Theme newTheme)
	{
		this.newTheme = newTheme;
	}


	private void SFXComposer ()
	{
		// if sfx composer interval has not been met, return
		if (Time.time > nextUpdateSFXComposer) {
			nextUpdateSFXComposer = Time.time + updateIntervalSFXComposer;
		} else {
			return;
		}

		List<SoundEntry> curSounds = currentTheme.sounds;

		foreach (SoundEntry curSound in curSounds) {
			if (Time.time > curSound.shouldPlayNext) {
				cur_sfx_idx = (cur_sfx_idx + 1) % numSFXPlayers;
				AudioSource player = sfxPlayers [cur_sfx_idx];

				curSound.lastPlayed = Time.time;
				curSound.Play (player, sfxVolume);
			}
		}
	}

	private void PlayMusic ()
	{
		// if we are not currently fading
		if (!fading) {
			// if we get a fade request
			if (inSound != null) {
				startedFadingAt = Time.time; // save the time we started fading
				secondarySound.clip = inSound; // set the clip to secondary audio player
				secondarySound.time = 0f;
				secondarySound.Play ();
				inSound = null; // reset the in sound to null so we wont do it again
				fading = true;
			}

		} else {
			float curveTime = (Time.time - startedFadingAt) / fadeTime;
			float curveValue = fadeCurve.Evaluate (curveTime);

			mainSound.volume = (1f - curveValue) * _musicVolume;
			secondarySound.volume = (curveValue) * _musicVolume;

			// if we have finished our fading  
			if (curveTime >= 1f) {
				fading = false;

				// switch mainSound and secondarySound
				mainSound.clip = secondarySound.clip;
				mainSound.volume = _musicVolume;
				mainSound.Play ();
				mainSound.time = secondarySound.time;

				secondarySound.clip = null;
				secondarySound.volume = 0f;
				secondarySound.time = 0f;
			}
		}
	}


	public void clampVolume ()
	{
		_musicVolume = musicVolume * masterVolume;
		_sfxVolume = sfxVolume * masterVolume;

		// music players
		mainSound.volume = Mathf.Min (_musicVolume, mainSound.volume);
		secondarySound.volume = Mathf.Min (_musicVolume, secondarySound.volume);

		// sfx players
		foreach (AudioSource player in this.sfxPlayers)
			player.volume = Mathf.Min (_sfxVolume, player.volume);
	}

	// Use this function from outside to fade another track in
	public void MakeFade (AudioClip inMusic)
	{
		inSound = inMusic;
	}


	public void PlayRandomMusicIndefinitely ()
	{
		if (playlist.Length == 0)
			return;
		
		curIdx = (curIdx + 1) % playlist.Length;
		// pick next
		AudioClip luckyOne = playlist [curIdx];

		this.MakeFade (luckyOne);

		Invoke ("PlayRandomMusicIndefinitely", this.inSound.length - this.fadeTime);
	}

	public void RandomTheme ()
	{
		if (this.themes.Count > 1) {
			int idx = Random.Range (0, this.themes.Count - 1);	
			this.changeTheme (themes [idx]);
			Debug.Log ("Theme Changed");
		}
	}
}