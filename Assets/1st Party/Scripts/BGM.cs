using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles background music behaviors
/// </summary>
public class BGM : MonoBehaviour
{

    private static BGM instance = null;
    public static BGM Instance
    {
        get { return instance; }
    }
    public AudioSource undetected;
    public AudioSource detected;
    private float musicVolume = .6f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        } else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Begins playing undetected theme if not already playing
    /// </summary>
    public void Play()
    {
        if (!undetected.isPlaying)
        {
            undetected.Play();
            detected.Stop();
        }
    }

    /// <summary>
    /// Fades out detected theme into undetected
    /// </summary>
    public void Unseen()
    {
        StartCoroutine(Fade());
    }

    /// <summary>
    /// 
    /// </summary>
    public void Spotted()
    {
        StopAllCoroutines();
        undetected.Stop();
        detected.Play();
        detected.volume = musicVolume; //Resets volume for when detected while music if fading
    }

    /// <summary>
    /// Stops all music
    /// </summary>
    public void Stop()
    {
        undetected.Stop();
        detected.Stop();
    }

    /// <summary>
    /// Fades out detected theme into undetected
    /// </summary>
    /// <returns></returns>
    private IEnumerator Fade()
    {
        float currentTime = 0;
        undetected.Play();
        undetected.volume = musicVolume;
        while (currentTime <= 5)
        {
            currentTime += Time.deltaTime;
            undetected.volume = Mathf.Lerp(0, musicVolume, currentTime);
            detected.volume = Mathf.Lerp(musicVolume, 0, currentTime*2);
            yield return null;
        }
        detected.Stop();
    }
}
