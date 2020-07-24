using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityRoyale;

public class AudioManager : MonoBehaviour
{
	public bool muteMusic = false;
	
	public AudioMixer audioMixer;
	public AudioMixerSnapshot gameplaySnapshot, endMatchSnapshot;
	public AudioSource musicSource;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();

		muteMusic = PlayerPrefs.GetInt(OptionsScreen.PlayerPrefsMuteMusicKey, 0) == 0;
		
		audioSource.Play();
		
		if (!muteMusic)
		{
			musicSource.Play();
		}
	}
}
