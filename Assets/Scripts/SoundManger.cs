using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MusicState
{
    stopPlayingMusic, playingMusic
}


public class SoundManger : MonoBehaviour
{
    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private Button playButton;
    [SerializeField]
    private List<AudioClip> audioClips;
    [SerializeField]
    private List<Sprite> buttonSprites;
    private AudioSource audioSource;
    private MusicState state = MusicState.stopPlayingMusic;

    const string currentState = "curretState";

    public bool finishPlaySound = false;

    private void Awake()
    {

        audioSource = this.GetComponent<AudioSource>();
        System.Enum.TryParse(PlayerPrefs.GetString(currentState), out state);
        PlayMusic();
    }

    private void RandomizeClips()
    {
        List<AudioClip> Clips = new List<AudioClip>();
        int count = audioClips.Count;
        for (int i = 0; i < count; i++)
        {
            int randomClipIndex = Random.Range(0, audioClips.Count);
            Clips.Add(audioClips[randomClipIndex]);
            audioClips.RemoveAt(randomClipIndex);
        }
        for (int i = 0; i < count; i++)
        {
            audioClips.Add(Clips[i]);
        }
        audioSource.clip = audioClips[0];
        Clips.Clear();
    }

    private IEnumerator clipSequencePlaying()
    {
        while (state != MusicState.stopPlayingMusic)
        {
            finishPlaySound = false;
            RandomizeClips();
            for (int i = 0; i < audioClips.Count; i++)
            {
                audioSource.clip = audioClips[i];
                progressStep = audioSource.clip.length / 100;
                Debug.Log(progressStep);
                Debug.Log(audioSource.clip.length);
                audioSource.Play();
                StartCoroutine(ProgressBar(audioSource.clip.length));
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void ChangeButtonIcon(Button button, byte i)
    {
        playButton.GetComponent<Image>().overrideSprite = buttonSprites[i];
    }

    public void PlayButton()
    {
        ChangeCurrentState();
        PlayerPrefs.SetString(currentState, state.ToString());
    }

    void stopCurrentCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            audioSource.Stop();
        }
    }
    private void PlayMusic()
    {
        if (state == MusicState.playingMusic && audioSource.enabled)
        {
            ChangeButtonIcon(playButton, 2);
            StartCoroutine(clipSequencePlaying());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private void ChangeCurrentState()
    {
        if (state == MusicState.stopPlayingMusic)
        {
            state = MusicState.playingMusic;
            StartCoroutine(clipSequencePlaying());
            ChangeButtonIcon(playButton, 2);
        }
        else if (state == MusicState.playingMusic)
        {
            state = MusicState.stopPlayingMusic;
            StopAllCoroutines();
            audioSource.Stop();
            ChangeButtonIcon(playButton, 1);
        }

    }

    float progressStep;
    IEnumerator ProgressBar(float duration)
    {
        float time = 0.0f;
        progressBar.maxValue = duration;
        while (time < duration)
        {
            time += 1;
            progressBar.value = time;
            Debug.Log(time);
            yield return new WaitForSeconds(1f);
        }

    }
}
