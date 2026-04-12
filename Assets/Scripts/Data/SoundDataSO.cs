using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "GameSettings/SoundData")]

public class SoundDataSO : ScriptableObject
{
    public AudioMixer mixer;
    public float masterVol;
    public float musicVol;
    public float sfxVol;
}
