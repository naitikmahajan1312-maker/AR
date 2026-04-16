using UnityEngine;
using Vuforia;

[RequireComponent(typeof(VuMarkBehaviour))]
public class TestVuMarkGame : MonoBehaviour
{
    public GameObject slingshotPrefab;
    public GameObject levelPrefab;   // blocks + targets as one prefab

    GameObject slingshotInstance;
    GameObject levelInstance;
    VuMarkBehaviour vb;

    void Awake() => vb = GetComponent<VuMarkBehaviour>();

    void OnEnable()  => vb.OnTargetStatusChanged += OnStatus;
    void OnDisable() => vb.OnTargetStatusChanged -= OnStatus;

    void OnStatus(ObserverBehaviour b, TargetStatus s)
    {
        bool tracked = s.Status == Status.TRACKED || s.Status == Status.EXTENDED_TRACKED;

        if (tracked)
        {
            if (!slingshotInstance)
            {
                slingshotInstance = Instantiate(slingshotPrefab, transform);
                slingshotInstance.transform.localPosition = new Vector3(-0.06f, 0f, 0f); // left side
                slingshotInstance.transform.localRotation = Quaternion.identity;
            }

            if (!levelInstance)
            {
                levelInstance = Instantiate(levelPrefab, transform);
                levelInstance.transform.localPosition = new Vector3(0.06f, 0f, 0f); // right side
                levelInstance.transform.localRotation = Quaternion.identity;
            }

            slingshotInstance.SetActive(true);
            levelInstance.SetActive(true);
        }
        else
        {
            if (slingshotInstance) slingshotInstance.SetActive(false);
            if (levelInstance) levelInstance.SetActive(false);
        }
    }
}
