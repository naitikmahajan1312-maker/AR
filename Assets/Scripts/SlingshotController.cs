using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    [Header("Rig")]
    public Transform planeBasis; // assign SlingshotRoot (defines forward/up/right)
    public Transform leftAnchor;
    public Transform rightAnchor;
    public Transform restPoint;
    public Transform pouch;          // <-- ADD (Empty transform)

    [Header("Bands")]
    public LineRenderer leftBand;
    public LineRenderer rightBand;

    [Header("Projectile")]
    public Rigidbody projectilePrefab;
    public float maxPullDistance = 0.25f;

    [Header("Angry Birds Launch")]
    public float springStrength = 35f;   // impulse strength
    public float maxLaunchSpeed = 18f;   // clamp for consistency

    [Header("Camera")]
    public Camera cam;

    Rigidbody currentProjectile;
    bool dragging;

    void Awake()
    {
        // Make sure line renderers use world space since we feed world positions
        if (leftBand) leftBand.useWorldSpace = true;
        if (rightBand) rightBand.useWorldSpace = true;
    }

    void Start()
    {
        ResetPouch();
        SpawnProjectile();
        UpdateBands();
    }

    void Update()
    {
        if (currentProjectile == null) return;

        bool down = Input.GetMouseButtonDown(0);
        bool held = Input.GetMouseButton(0);
        bool up = Input.GetMouseButtonUp(0);

        if (down)
        {
            // Only start drag if we clicked the projectile
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                if (hit.rigidbody != null && hit.rigidbody == currentProjectile)
                {
                    dragging = true;
                    currentProjectile.isKinematic = true;
                }
            }
        }

        if (dragging && held)
            DragPouchAndProjectile(Input.mousePosition);

        if (dragging && up)
            ReleaseProjectile();

        UpdateBands();
    }

    void ResetPouch()
    {
        if (pouch)
        {
            pouch.position = restPoint.position;
            pouch.rotation = restPoint.rotation;
        }
    }

    void SpawnProjectile()
    {
        // Spawn on pouch, parent to pouch while waiting/dragging
        currentProjectile = Instantiate(projectilePrefab, pouch.position, pouch.rotation, pouch);
        currentProjectile.isKinematic = true;

        // Use velocity or linearVelocity depending on your Unity version
        currentProjectile.linearVelocity = Vector3.zero;
        currentProjectile.angularVelocity = Vector3.zero;
    }

    // void DragPouchAndProjectile(Vector3 screenPos)
    // {
    //     Ray ray = cam.ScreenPointToRay(screenPos);

    //     // Stable drag plane in AR: facing camera, passing through rest point
    //     Plane dragPlane = new Plane(-cam.transform.forward, restPoint.position);

    //     if (!dragPlane.Raycast(ray, out float enter)) return;

    //     Vector3 hitPoint = ray.GetPoint(enter);

    //     // Clamp pull around restPoint (Angry Birds behavior)
    //     Vector3 offset = hitPoint - restPoint.position;
    //     if (offset.magnitude > maxPullDistance)
    //         hitPoint = restPoint.position + offset.normalized * maxPullDistance;

    //     // Move pouch, projectile follows pouch
    //     pouch.position = hitPoint;
    //     currentProjectile.transform.position = pouch.position;
    // }

    void DragPouchAndProjectile(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        // 1) Intersect with a plane that always faces the camera (so it always hits)
        Plane camPlane = new Plane(-cam.transform.forward, restPoint.position);
        if (!camPlane.Raycast(ray, out float enter)) return;

        Vector3 hit = ray.GetPoint(enter);

        // 2) Now project that hit onto the slingshot plane (true 3D pull plane)
        // Slingshot plane contains forward+up, so its normal is "right"
        Vector3 n = planeBasis.right; // plane normal

        // Projection onto plane through restPoint:
        Vector3 toHit = hit - restPoint.position;
        Vector3 hitOnSlingPlane = hit - n * Vector3.Dot(toHit, n);

        // 3) Optional: prevent sideways drift (feels like real slingshot)
        // Keep the pouch centered left/right by removing right-axis component.
        // Comment this out if you want free 2D aim.
        Vector3 off = hitOnSlingPlane - restPoint.position;
        off -= n * Vector3.Dot(off, n);
        hitOnSlingPlane = restPoint.position + off;

        // 4) Clamp pull radius
        if (off.magnitude > maxPullDistance)
            hitOnSlingPlane = restPoint.position + off.normalized * maxPullDistance;

        pouch.position = hitOnSlingPlane;
        currentProjectile.transform.position = pouch.position;
    }

    // void ReleaseProjectile()
    // {
    //     dragging = false;

    //     // Stretch vector from pouch -> rest point (this defines launch direction)
    //     Vector3 stretch = restPoint.position - pouch.position;
    //     float pull = Mathf.Clamp(stretch.magnitude, 0f, maxPullDistance);

    //     // Detach projectile and enable physics
    //     currentProjectile.transform.parent = null;
    //     currentProjectile.isKinematic = false;

    //     // Apply Angry Birds style impulse proportional to pull
    //     Vector3 launchDir = stretch.normalized;
    //     float impulse = (pull / maxPullDistance) * springStrength;

    //     currentProjectile.AddForce(launchDir * impulse, ForceMode.Impulse);

    //     // Clamp speed for consistent feel
    //     if (currentProjectile.linearVelocity.magnitude > maxLaunchSpeed)
    //         currentProjectile.linearVelocity = currentProjectile.linearVelocity.normalized * maxLaunchSpeed;

    //     currentProjectile = null;

    //     // Snap pouch back (bands snap back like Angry Birds)
    //     ResetPouch();

    //     // Spawn next projectile
    //     SpawnProjectile();
    // }

    void ReleaseProjectile()
    {
        dragging = false;

        // Pull vector in 3D (rest -> pouch)
        Vector3 pullVec = pouch.position - restPoint.position;
        float pull = Mathf.Clamp(pullVec.magnitude, 0f, maxPullDistance);
        if (pull < 0.001f) { ResetPouch(); return; }

        // Launch opposite of pull
        Vector3 launchDir = (-pullVec).normalized;

        // Speed proportional to pull (3D)
        float speed = (pull / maxPullDistance) * maxLaunchSpeed;

        currentProjectile.transform.SetParent(null, true);
        currentProjectile.isKinematic = false;
        currentProjectile.constraints = RigidbodyConstraints.None;

        currentProjectile.linearVelocity = launchDir * speed;       // 3D velocity
        currentProjectile.linearVelocity = launchDir * speed; // Unity 6 support
        currentProjectile.angularVelocity = Vector3.zero;

        // Destroy(currentProjectile.gameObject, 3f);
        currentProjectile = null;

        ResetPouch();
        Invoke(nameof(SpawnProjectile), 0.2f);
    }

    void UpdateBands()
    {
        if (!pouch) return;

        Vector3 p = pouch.position;

        if (leftBand != null)
        {
            leftBand.positionCount = 2;
            leftBand.SetPosition(0, leftAnchor.position);
            leftBand.SetPosition(1, p);
        }

        if (rightBand != null)
        {
            rightBand.positionCount = 2;
            rightBand.SetPosition(0, rightAnchor.position);
            rightBand.SetPosition(1, p);
        }
    }
}
