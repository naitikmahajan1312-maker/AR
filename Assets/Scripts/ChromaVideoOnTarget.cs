using UnityEngine;
using UnityEngine.Video;
using Vuforia;

[RequireComponent(typeof(ObserverBehaviour))]
public class ChromaVideoOnTarget : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Renderer videoRenderer;      // VideoQuad Renderer
    public RenderTexture videoRT;       // VideoRT

    public Color keyColor = Color.green;
    [Range(0,1)] public float threshold = 0.25f;
    [Range(0,1)] public float smoothing = 0.10f;

    ObserverBehaviour ob;
    Material mat;

    void Awake()
    {
        ob = GetComponent<ObserverBehaviour>();

        // Make material instance so changes don’t affect other objects
        mat = videoRenderer.material;

        // Send key settings to shader
        mat.SetColor("_KeyColor", keyColor);
        mat.SetFloat("_Threshold", threshold);
        mat.SetFloat("_Smoothing", smoothing);

        // Video -> RenderTexture
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRT;

        // RenderTexture -> material
        mat.mainTexture = videoRT;
    }

    void OnEnable()  => ob.OnTargetStatusChanged += OnStatus;
    void OnDisable() => ob.OnTargetStatusChanged -= OnStatus;

    void OnStatus(ObserverBehaviour b, TargetStatus s)
    {
        bool tracked = s.Status == Status.TRACKED || s.Status == Status.EXTENDED_TRACKED;

        if (tracked)
        {
            if (!videoPlayer.isPlaying) videoPlayer.Play();
            SoundManager.Instance.PlayVideoSound();
        }
        else
        {
            videoPlayer.Stop(); // or Pause() if you want resume
            SoundManager.Instance.StopVideoSound();
        }
    }
}
