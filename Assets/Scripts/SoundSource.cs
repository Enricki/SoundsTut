using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct playerButtonsImage
{
    public  Sprite playButton;
    public Sprite stopButton;
    public Sprite pauseButton;

    public playerButtonsImage(Sprite play, Sprite stop, Sprite pause)
    {
        playButton = play;
        stopButton = stop;
        pauseButton = pause;
    }
}

public class SoundSource : MonoBehaviour
{
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private List<AudioClip> soundList;
    [SerializeField]
    private List<Sprite> buttonSprites;

    private AudioSource soundSource;
    private playerButtonsImage playerButtons;
    private StateSystem stateSystem;
    private ColoredText coloredText;


    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
        stateSystem = GetComponent<StateSystem>();
        coloredText = GetComponent<ColoredText>();
        playerButtons = new playerButtonsImage(buttonSprites[0], buttonSprites[2], buttonSprites[1]);
        PlayOnStart();
    }

    private void PlayOnStart()
    {
        System.Enum.TryParse(PlayerPrefs.GetString(stateSystem.SoundState), out stateSystem.CurrentState);
        Debug.Log(stateSystem.CurrentState);
        if (stateSystem.CurrentState == MusicState.playingMusic)
        {
            StartCoroutine(clipSequencePlaying(0));
            playButton.image.overrideSprite = playerButtons.stopButton;
            StartCoroutine(coloredText.CicledTextColor());
        }
        else
        {
            playButton.image.overrideSprite = playerButtons.playButton;
        }
    }

    public void ChangeButtonState()
    {
        if (!soundSource.isPlaying)
        {
            StartCoroutine(clipSequencePlaying(0));
            playButton.image.overrideSprite = playerButtons.stopButton;
            soundSource.Play();
            stateSystem.CurrentState = MusicState.playingMusic;
        }
        else
        {
            StopAllCoroutines();
            playButton.image.overrideSprite = playerButtons.playButton;
            soundSource.Stop();
            stateSystem.CurrentState = MusicState.stopPlayingMusic;
        }
        PlayerPrefs.SetString(stateSystem.SoundState, stateSystem.CurrentState.ToString());
    }

    private IEnumerator clipSequencePlaying(int index)
    {
        float trackLength;
        while (true)
        {
            RandomizeClips();

            for (int i = index; i < soundList.Count; i++)
            {
                soundSource.clip = soundList[i];
                trackLength = soundSource.clip.length;
                soundSource.Play();
                Debug.Log(soundSource.clip);
                yield return new WaitForSeconds(trackLength);
            }
        }
    }

    private void RandomizeClips()
    {
        List<AudioClip> Clips = new List<AudioClip>();
        int count = soundList.Count;
        for (int i = 0; i < count; i++)
        {
            int randomClipIndex = UnityEngine.Random.Range(0, soundList.Count);
            Clips.Add(soundList[randomClipIndex]);
            soundList.RemoveAt(randomClipIndex);
        }
        for (int i = 0; i < count; i++)
        {
            soundList.Add(Clips[i]);
        }
        soundSource.clip = soundList[0];
        Clips.Clear();
    }
}
