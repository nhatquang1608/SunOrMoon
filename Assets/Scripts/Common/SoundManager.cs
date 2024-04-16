using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioClip clickSound;
    public AudioClip completedSound;
    public AudioClip failedSound;
    public AudioClip mergeSound;
    public AudioClip moveSound;
    [SerializeField] private AudioSource audioSource;

    public void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        audioSource.Play();
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, 3f);
    }
}
