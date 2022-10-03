using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instence;
    public static bool isAudioOn = true;
    public static bool isMusicOn = true;
    public static string playing_song = "";
    public static Sound song;

    void Awake()
    {
        if (instence == null) instence = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        BackgroundSong();
    }

    public void BackgroundSong(){
        if(isMusicOn) {
            Song("Song1");
            playing_song = "Song1";
        }
    }

    public void Play(string name) {
        if(isAudioOn){
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.PlayOneShot(s.clip);
        }
    }
    public void Song(string name) {
        if(isAudioOn){
            StopMusic();
            song = Array.Find(sounds, sound => sound.name == name);
            song.source.Play();
        }
    }
    public void StopMusic(){
        if(song != null) song.source.Stop();
    }
}
