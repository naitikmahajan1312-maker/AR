using UnityEngine;
using Vuforia;

public class TapPlaceOnGround : MonoBehaviour
{
    public PlaneFinderBehaviour planeFinder;
    public ContentPositioningBehaviour contentPos;
    public Transform groundStage;
    public GameObject prefabToPlace;

    public bool placeOnlyOnce = true;
    bool placed;

    void OnEnable()
    {
        // We want tap placement
        planeFinder.HitTestMode = HitTestMode.INTERACTIVE;
        planeFinder.OnInteractiveHitTest.AddListener(OnHit);

        // Also listen to automatic hit tests to know if a plane is currently found
        planeFinder.OnAutomaticHitTest.AddListener(OnAutoHit);

        // Hide indicator until a plane is found
        if (planeFinder.PlaneIndicator) planeFinder.PlaneIndicator.SetActive(false);
    }

    void OnDisable()
    {
        planeFinder.OnInteractiveHitTest.RemoveListener(OnHit);
        planeFinder.OnAutomaticHitTest.RemoveListener(OnAutoHit);
    }

    void OnAutoHit(HitTestResult result)
    {
        // If we already placed, keep indicator off
        if (placeOnlyOnce && placed)
        {
            if (planeFinder.PlaneIndicator) planeFinder.PlaneIndicator.SetActive(false);
            return;
        }

        // Show indicator only when plane is detected
        bool hasPlane = result != null;
        if (planeFinder.PlaneIndicator)
            planeFinder.PlaneIndicator.SetActive(hasPlane);
    }

    void OnHit(HitTestResult result)
    {
        if (result == null) return;
        if (placeOnlyOnce && placed) return;

        contentPos.PositionContentAtPlaneAnchor(result);

        Instantiate(prefabToPlace, groundStage.position, groundStage.rotation, groundStage);

        placed = true;

        if (planeFinder.PlaneIndicator) planeFinder.PlaneIndicator.SetActive(false);
    }
}