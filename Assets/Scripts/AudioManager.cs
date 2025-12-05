using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip menubgm;
    public AudioClip daybgm;
    public AudioClip nightbgm;

    private void Start()
    {
        musicSource.clip = menubgm;
        musicSource.Play();
    }
    
}