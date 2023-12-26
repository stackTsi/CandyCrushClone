using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    
    public AudioSource backgroundMusic;
    public AudioSource introSection;
    private bool introPlayed = false;
    public void Start(){
        PlayIntro();
    }
    public void Update(){
        CheckPlayIntroToPlayBG();
    }

    private void PlayBackground(){
        backgroundMusic.Play();
    }
    private void CheckPlayIntroToPlayBG(){
        if(!introSection.isPlaying && !introPlayed){
            introPlayed = true;
            PlayBackground();
        }
    }
    private void PlayIntro(){
        introSection.Play();
    }
    public void PlayRandomDestroyNoise(){
        //choose a random destroy audio
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        //play clip
        destroyNoise[clipToPlay].Play();
    }
}
