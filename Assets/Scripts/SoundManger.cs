using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MusicState
{
    stopPlayingMusic, playingMusic, pauseMusic
}


public class SoundManger : MonoBehaviour
{
    [SerializeField]
    private Slider progressBar;
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private List<Sprite> buttonSprites;
    [SerializeField]
    private List<AudioClip> audioClips;


    private AudioSource audioSource;
    private MusicState state = MusicState.stopPlayingMusic;
    private bool onStart = true;
    private Coroutine pauseCoroutine;

    const string currentState = "curretState";
    const string volumeLevel = "currentVolumeLevel";

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(volumeLevel);
        volumeSlider.value = audioSource.volume;
        System.Enum.TryParse(PlayerPrefs.GetString(currentState), out state);
        RandomizeClips();
        Debug.Log("Randomized");
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



    private void ChangeButtonIcon(Button button, byte i)
    {
        playButton.GetComponent<Image>().overrideSprite = buttonSprites[i];
    }


    bool pauseState()
    {
        if (state == MusicState.pauseMusic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void PlayButton()
    {
        ChangeCurrentState();
    }

    public void PauseButton()
    {
        state = MusicState.pauseMusic;
        audioSource.Pause();
        ChangeButtonIcon(playButton, 1);
    }

    public void NextButton()
    {
        Debug.Log(audioClips[0]);
        Debug.Log(audioClips[1]);
        Debug.Log(audioClips[2]);
        Debug.Log(audioClips[3]);
        Debug.Log(audioClips[4]);
        state = MusicState.pauseMusic;
        for (int i = 0; i < audioClips.Count; i++)
        {
            if (i + 1 <= audioClips.Count)
            {
                if (audioSource.clip == audioClips[i])
                {
                    audioSource.clip = audioClips[i + 1];
                    Debug.Log("      ");
                    Debug.Log(audioSource.clip);
                    return;
                }
            }
            else
            {
                audioSource.clip = audioClips[0];
                Debug.Log("      ");
                Debug.Log(audioSource.clip);
                return;
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat(volumeLevel, audioSource.volume);
        PlayerPrefs.SetString(currentState, state.ToString());
    }

    public void ChangeVolumeLevel(Slider slider)
    {
        audioSource.volume = slider.value;
    }

    private void PlayMusic()
    {
        if (state == MusicState.playingMusic)
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
        else if (state == MusicState.pauseMusic)
        {
            audioSource.Play();
            state = MusicState.playingMusic;
            ChangeButtonIcon(playButton, 2);
        }
    }

    private void PlayTrack()
    {
        audioSource.Play();
        StartCoroutine(ProgressBar(audioSource.clip.length));
    }

    private IEnumerator clipSequencePlaying()
    {
        while (state != MusicState.stopPlayingMusic)
        {
            RandomizeClips();
            for (int i = 0; i < audioClips.Count; i++)
            {
                if (state == MusicState.pauseMusic)
                {
                    yield return new WaitWhile(pauseState);
                }
                else
                {
                    audioSource.clip = audioClips[i];
                    PlayTrack();
                    yield return new WaitForSeconds(audioSource.clip.length);
                }
            }
        }
    }


    private IEnumerator ProgressBar(float duration)
    {
        float time = 0.0f;
        progressBar.maxValue = duration;
        while (time < duration)
        {
            if (state == MusicState.pauseMusic)
            {
                yield return new WaitWhile(pauseState);
            }
            else
            {
                time += 1;
                progressBar.value = time;
                yield return new WaitForSeconds(1f);
            }
        }
    }


    //void stopCurrentCoroutine(Coroutine coroutine)
    //{
    //    if (coroutine != null)
    //    {
    //        StopCoroutine(coroutine);
    //        coroutine = null;
    //        audioSource.Stop();
    //    }
    //}
}
