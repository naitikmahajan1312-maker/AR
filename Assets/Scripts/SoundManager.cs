using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;

    public string[] targetNames;   // Apple, Ant, Aeroplane
    public AudioClip[] clips;      // AppleClip, AntClip, AeroplaneClip
    public AudioClip video;
    public AudioClip bowling;
    public AudioClip bowlingStrike;

    public bool soundOn = false;

    string lastPlayedTarget = "";

    void Awake()
    {
        if (Instance == null) Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayByTargetName(string targetName)
    {
        if (!soundOn || audioSource == null) return;

        // // don't restart again and again while it stays tracked
        if (lastPlayedTarget == targetName && audioSource.isPlaying) return;
        Debug.Log(targetName);
        for (int i = 0; i < targetNames.Length; i++)
        {
            Debug.Log(targetNames.Length);
            if (targetNames[i] == targetName)
            {
                Debug.Log(targetNames[i]);
                audioSource.clip = clips[i];
                audioSource.Play();
                lastPlayedTarget = targetName;
                return;
            }
        }
    }

    public void PlayVideoSound()
    {
        audioSource.clip = video;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void StopVideoSound()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }

    public void Stop()
    {
        if (audioSource != null) audioSource.Stop();
        lastPlayedTarget = "";
    }

    public void BowlingSound(bool Bowling)
    {
        if (Bowling)
        {
            audioSource.clip = bowling;
            audioSource.Play();
            audioSource.loop = true;
        }
        else
        {
            audioSource.clip = null;
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void BowlingStrike()
    {
        audioSource.clip = bowlingStrike;
        audioSource.Play();
        audioSource.loop = false;
    }
}
