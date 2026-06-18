using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip clipFlip;
    [SerializeField] private AudioClip clipMatch;
    [SerializeField] private AudioClip clipMismatch;
    [SerializeField] private AudioClip clipGameOver;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = this.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play card flip sound
    /// </summary>
    public void PlayFlip()
    {
        PlayClip(clipFlip);
    }

    /// <summary>
    /// Play match success sound
    /// </summary>
    public void PlayMatch()
    {
        PlayClip(clipMatch);
    }

    /// <summary>
    /// Play mismatch fail sound
    /// </summary>
    public void PlayMismatch()
    {
        PlayClip(clipMismatch);
    }

    /// <summary>
    /// Play game over sound
    /// </summary>
    public void PlayGameOver()
    {
        PlayClip(clipGameOver);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (_audioSource == null)
        {
            return;
        }

        _audioSource.PlayOneShot(clip);
    }
}