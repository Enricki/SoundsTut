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
    private Button firstPlayList;
    [SerializeField]
    private Button secondPlayList;
    [SerializeField]
    private List<Sprite> buttonSprites;

    [SerializeField]
    private List<AudioClip> shortListForTesting;
    [SerializeField]
    private List<AudioClip> NormalList;

    public List<AudioClip> audioClips;


    private AudioSource audioSource;
    private MusicState state;
    private float trackLength;
    private float elapsedTrackTime;
    private int trackIndex;
    private bool blockSort;
    private int playlistToLoad; 

    const string currentState = "curretState";
    const string volumeLevel = "currentVolumeLevel";
    const string currentPlaylist = "currentPlaylist";

    private void Start()
    {

        playlistToLoad = PlayerPrefs.GetInt(currentPlaylist);
        Debug.Log(playlistToLoad);

        if (playlistToLoad == 0)
        {
            for (int i = 0; i < shortListForTesting.Count; i++)
            {
                audioClips.Add(shortListForTesting[i]);
            }
            firstPlayList.interactable = false;
        }
        else if (playlistToLoad == 1)
        {
            for (int i = 0; i < NormalList.Count; i++)
            {
                audioClips.Add(NormalList[i]);
            }
            secondPlayList.interactable = false;
        }
        //for (int i = 0; i < shortListForTesting.Count; i++)
        //{
        //    audioClips.Add(shortListForTesting[i]);
        //}
        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(volumeLevel);
        volumeSlider.value = audioSource.volume;
        System.Enum.TryParse(PlayerPrefs.GetString(currentState), out state);
        if (state == MusicState.pauseMusic)
        {
            state = MusicState.stopPlayingMusic;
            pauseButton.interactable = false;
        }
        else if (state == MusicState.stopPlayingMusic)
        {
            pauseButton.interactable = false;
        }

        ChangeButtonIcon();
        RandomizeClips();
        trackLength = audioSource.clip.length;
        StartCoroutine(clipSequencePlaying(trackIndex));
        blockSort = false;
    }
    /// <summary>
    /// Public Methods for Interract with Interface
    /// </summary>

    public void PlayButton()
    {

        if (state == MusicState.playingMusic)
        {
            state = MusicState.stopPlayingMusic;
            StopAllCoroutines();
            ChangeButtonIcon(playButton, 1);
            audioSource.Stop();
            trackLength = 0;
            progressBar.value = 0;
            pauseButton.interactable = false;
        }
        else if (state == MusicState.stopPlayingMusic)
        {
            state = MusicState.playingMusic;
            RandomizeClips();
            trackLength = audioSource.clip.length;
            StartCoroutine(clipSequencePlaying(trackIndex));
            ChangeButtonIcon(playButton, 2);
            pauseButton.interactable = true;

        }
        else if (state == MusicState.pauseMusic)
        {
            StartCoroutine(clipSequencePlaying(trackIndex));
            ChangeButtonIcon(playButton, 2);
            pauseButton.interactable = true;
            blockSort = false;
            state = MusicState.playingMusic;
        }
    }

    public void PauseButton()
    {
        state = MusicState.pauseMusic;
        StopAllCoroutines();
        audioSource.Pause();
        ChangeButtonIcon(playButton, 1);
        pauseButton.interactable = false;

        for (int i = 0; i < audioClips.Count; i++)
        {
            if (audioSource.clip == audioClips[i])
            {
                trackIndex = i;
                Debug.Log(trackIndex);
                break;
            }
        }

        elapsedTrackTime = progressBar.value;
        trackLength = audioSource.clip.length - elapsedTrackTime;
        blockSort = true;
        Debug.Log(elapsedTrackTime);
        Debug.Log(trackLength);
    }

    public void ChangeVolumeLevel(Slider slider)
    {
        audioSource.volume = slider.value;
    }

    public void ChangePlayList(int index)
    {
        blockSort = false;
        if (audioClips.Count != 0)
        {
            audioClips.Clear();
        }
        if (index == 0)
        {
            for (int i = 0; i < shortListForTesting.Count; i++)
            {
                audioClips.Add(shortListForTesting[i]);
            }
            firstPlayList.interactable = false;
            secondPlayList.interactable = true;
            playlistToLoad = 0;
            Debug.Log(playlistToLoad);
        }
        else if (index == 1)
        {
            for (int i = 0; i < NormalList.Count; i++)
            {
                audioClips.Add(NormalList[i]);
            }
            secondPlayList.interactable = false;
            firstPlayList.interactable = true;
            playlistToLoad = 1;
            Debug.Log(playlistToLoad);
        }
        StopAllCoroutines();
        audioSource.Stop();
        state = MusicState.stopPlayingMusic;
        trackIndex = 0;
        elapsedTrackTime = 0;
        progressBar.value = 0;
        ChangeButtonIcon(playButton, 1);
        pauseButton.interactable = false;
    }

    /// <summary>
    /// Enumerators
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator clipSequencePlaying(int index)
    {
        while (state != MusicState.stopPlayingMusic)
        {
            if (!blockSort)
            {
                RandomizeClips();
            }

            for (int i = index; i < audioClips.Count; i++)
            {
                audioSource.clip = audioClips[i];
                if (state == MusicState.pauseMusic)
                {
                }
                else if (state == MusicState.playingMusic)
                {
                    trackLength = audioSource.clip.length;
                    elapsedTrackTime = 0;
                }
                audioSource.Play();
                Debug.Log(audioSource.clip);
                StartCoroutine(ProgressBar(elapsedTrackTime, audioSource.clip.length));
                yield return new WaitForSeconds(trackLength);
            }
        }
    }

    private IEnumerator ProgressBar(float elapsedTime, float TrackLength)
    {
        progressBar.value = 0;
        progressBar.maxValue = TrackLength;
        while (elapsedTime <= TrackLength)
        {

            progressBar.value = elapsedTime;
            elapsedTime++;
            yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// Other
    /// </summary>
    private void RandomizeClips()
    {
        List<AudioClip> Clips = new List<AudioClip>();
        int count = audioClips.Count;
        for (int i = 0; i < count; i++)
        {
            int randomClipIndex = UnityEngine.Random.Range(0, audioClips.Count);
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

    private void ChangeButtonIcon(Button button, int index)
    {
        button.GetComponent<Image>().overrideSprite = buttonSprites[index];
    }

    private void ChangeButtonIcon()
    {
        if (state == MusicState.playingMusic)
        {
            ChangeButtonIcon(playButton, 2);
        }
        else if (state == MusicState.stopPlayingMusic)
        {
            ChangeButtonIcon(playButton, 1);
        }
    }


    /// <summary>
    /// Saving Current Music on/off state
    /// </summary>
    private void OnApplicationQuit()
    {

        PlayerPrefs.SetFloat(volumeLevel, audioSource.volume);
        PlayerPrefs.SetString(currentState, state.ToString());
        PlayerPrefs.SetInt(currentPlaylist, playlistToLoad);
    }
}
