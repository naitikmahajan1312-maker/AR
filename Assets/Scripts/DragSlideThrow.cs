using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DragSlideThrow : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private DragSlideThrow prefab;
    [SerializeField] private GameObject targets;

    [Header("Drag")]
    public float followSmooth = 25f;   // higher = snappier drag

    [Header("Shoot")]
    public float launchSpeed = 15f;    // speed along the release ray

    Rigidbody rb;
    bool dragging;
    float grabDistance;               // distance from camera to grabbed point
    Vector2 pointerPos;               // last pointer position while dragging
    Vector3 spawnPoint;
    public Vector3 targetSpawnPoint;
    GameObject targetInstance;

    void Awake()
    {
        spawnPoint = this.transform.position;
        targetInstance = targets;
        rb = GetComponent<Rigidbody>();
        if (!cam) cam = Camera.main;
    }

    private void Start()
    {
        if (!cam) cam = Camera.main;
        rb.isKinematic = true;
    }

    void Update()
    {
        GetPointer(out bool down, out bool held, out bool up, out Vector2 pos);

        if (down)
        {
            Ray ray = cam.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                // Start drag only if this object was hit
                if (hit.rigidbody == rb || hit.collider == GetComponent<Collider>())
                {
                    dragging = true;
                    rb.isKinematic = true;

                    pointerPos = pos;
                    grabDistance = Vector3.Distance(cam.transform.position, hit.point);
                }
            }
        }

        if (dragging && held)
        {
            pointerPos = pos;
        }

        if (dragging && up)
        {
            dragging = false;
            rb.isKinematic = false;
            rb.useGravity = true;

            // Shoot along the release ray direction
            Ray ray = cam.ScreenPointToRay(pointerPos);

            rb.linearVelocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;

            rb.linearVelocity = ray.direction * launchSpeed;      // (Unity standard)
            rb.linearVelocity = rb.linearVelocity;                // (Unity 6 compatibility)
            rb.WakeUp();
        }

        if (this.transform.position.y < -0.5)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            // rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            this.transform.position = spawnPoint;
        }
    }

    void FixedUpdate()
    {
        if (!dragging) return;

        Ray ray = cam.ScreenPointToRay(pointerPos);

        // Drag target is a point along the ray at the stored grab distance
        Vector3 target = cam.transform.position + ray.direction * grabDistance;

        // Smooth move (no teleport)
        Vector3 newPos = Vector3.Lerp(rb.position, target, followSmooth * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    void GetPointer(out bool down, out bool held, out bool up, out Vector2 pos)
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            pos = t.position;
            down = t.phase == TouchPhase.Began;
            held = t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary;
            up = t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled;
            return;
        }

        pos = Input.mousePosition;
        down = Input.GetMouseButtonDown(0);
        held = Input.GetMouseButton(0);
        up = Input.GetMouseButtonUp(0);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Plane"))
        {
            SoundManager.Instance.BowlingSound(true);
        }
        if (other.gameObject.tag.Equals("Target"))
        {
            prefab = this;
            // Destroy(other.gameObject);
            // Destroy(gameObject, 6f);
            // DragSlideThrow obj = Instantiate(prefab, spawnPoint, Quaternion.identity);
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            this.transform.position = spawnPoint;
            // targetInstance = Instantiate(targets, targetSpawnPoint, Quaternion.identity);
            SoundManager.Instance.BowlingStrike();
            SoundManager.Instance.BowlingSound(false);
            return;
        }
        if (other.gameObject.tag.Equals("Finish"))
        {
            prefab = this;
            if (targetInstance) Destroy(targetInstance);
            // Destroy(gameObject, 6f);
            DragSlideThrow obj = Instantiate(prefab, spawnPoint, Quaternion.identity);
            // targetInstance = Instantiate(targets, targetSpawnPoint, Quaternion.identity);
        }
        if (other.gameObject.tag.Equals("Basket"))
        {
            prefab = this;
            if (targetInstance) Destroy(targetInstance);
            // Destroy(gameObject, 6f);
            DragSlideThrow obj = Instantiate(prefab, spawnPoint, Quaternion.identity);
            // targetInstance = Instantiate(targets, targetSpawnPoint, Quaternion.identity);
        }
    }
}
