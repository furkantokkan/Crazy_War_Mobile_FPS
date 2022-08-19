using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource levelMusic, gameOverMusic, winMusic;

    public AudioSource[] sfx;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (levelMusic != null && !levelMusic.isPlaying)
        {
            levelMusic.Play();
        }
    }
    public void PlaySFX(int sfxToPlay)
    {
        sfx[sfxToPlay].transform.position = Vector3.zero;
        sfx[sfxToPlay].Stop();
        sfx[sfxToPlay].Play();
    }
    public void PlaySfxAtPosition(int sfxToPlay, Vector3 position)
    {
        sfx[sfxToPlay].transform.position = position;
        sfx[sfxToPlay].Stop();
        sfx[sfxToPlay].Play();
    }
}