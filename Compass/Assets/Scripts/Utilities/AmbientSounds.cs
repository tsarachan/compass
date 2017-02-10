using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmbientSounds : MonoBehaviour {


	private AudioSource foleySource;
	private const string FOLEY_OBJ = "Foley";

	//clips to play
	private AudioClip foley;
	private const string FOLEY_SOUNDS = "Audio/Ambient/Background foley";


	private void Start(){
		SetupAudioSource(transform.Find(FOLEY_OBJ).GetComponent<AudioSource>(), FOLEY_SOUNDS);
	}


	private void SetupAudioSource(AudioSource source, string soundName){
		source.clip = Resources.Load(soundName) as AudioClip;
		source.Play();
	}
}
