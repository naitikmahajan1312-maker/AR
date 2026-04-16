using UnityEngine;
using Vuforia;

public class TargetAudioPlayer : MonoBehaviour
{
    ObserverBehaviour ob;

    void Awake()
    {
        ob = GetComponent<ObserverBehaviour>(); // ImageTarget has this
    }

    void OnEnable()
    {
        ob.OnTargetStatusChanged += OnStatus;
    }

    void OnDisable()
    {
        ob.OnTargetStatusChanged -= OnStatus;
    }

    void OnStatus(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool tracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

        if (tracked)
            SoundManager.Instance.PlayByTargetName(behaviour.TargetName);
        else
            SoundManager.Instance.Stop(); // optional
    }
}
