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
    private Button playButton;
    [SerializeField]
    private List<AudioClip> audioClips;
    [SerializeField]
    private List<Sprite> buttonSprites;
    private AudioSource audioSource;
    private MusicState state = MusicState.stopPlayingMusic;
    private Coroutine coroutine;

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
            Debug.Log(audioClips[i]);
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
                audioSource.Play();
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
        Debug.Log(state);
        
    }

    void stopCurrentCoroutine()
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
            coroutine = StartCoroutine(clipSequencePlaying());
        }
        else
        {
            stopCurrentCoroutine();
        }
        Debug.Log(state);
    }

    private void ChangeCurrentState()
    {
        if (state == MusicState.stopPlayingMusic)
        {
            state = MusicState.playingMusic;
            coroutine = StartCoroutine(clipSequencePlaying());
            ChangeButtonIcon(playButton, 2);
        }
        else if (state == MusicState.playingMusic)
        {
            state = MusicState.stopPlayingMusic;
            stopCurrentCoroutine();
            ChangeButtonIcon(playButton, 1);
        }

    }
}
