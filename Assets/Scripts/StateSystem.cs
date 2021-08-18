using UnityEngine;

public enum MusicState
{
    stopPlayingMusic, playingMusic, pauseMusic
}

public class StateSystem : MonoBehaviour
{
    [HideInInspector]
    public MusicState CurrentState;
    public readonly string SoundState = "currentState";
}
