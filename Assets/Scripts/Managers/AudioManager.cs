using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*                      singleton class
 *   rudimentary class, manages soundtrack from editor due to small OST.
 */
public class AudioManager : MonoBehaviour
{
    public bool isMainMenu = false;
    [SerializeField]
    AudioSource morningMusic, eveningMusic, endMusic, mainMenuMusic;
    [SerializeField]
    float fadingDurations = 0.5f;
    public static AudioManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogWarning("More than one Audio Manager found in scene.");
        }
        else Instance = this;
    }
    void Start()
    {
        if (isMainMenu)
        {
            mainMenuMusic.volume = 0;
            mainMenuMusic.Play();
            StartCoroutine(StartFade(mainMenuMusic, fadingDurations, Settings.Volume));
        }
        else
        {
            morningMusic.volume = 0;
            morningMusic.Play();
            StartCoroutine(StartFade(morningMusic, fadingDurations, Settings.Volume));
        }
    }

    private IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
    /*              startFade override method:  
     *  takes into account an existing audio. Fades out audio and replaces it with a new one. 
     */
    private IEnumerator StartFade(AudioSource toFade, AudioSource toBegin, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = toFade.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            toFade.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        toFade.Stop();
        toBegin.volume = 0;
        toBegin.Play();
        StartCoroutine(StartFade(toBegin, fadingDurations, Settings.Volume));
        yield break;
    }
    public void StartEveningMusic()
    {
        StartCoroutine(StartFade(morningMusic, eveningMusic, fadingDurations, Settings.Volume));
    }
    public void StartEndingMusic()
    {
        StartCoroutine(StartFade(eveningMusic, endMusic, fadingDurations, Settings.Volume));
    }
    public void StopMusic()
    {
        if (morningMusic.isPlaying) morningMusic.Stop();
        if (eveningMusic.isPlaying) eveningMusic.Stop();
        if (endMusic.isPlaying) endMusic.Stop();
        if (mainMenuMusic.isPlaying) mainMenuMusic.Stop();
    }
}
