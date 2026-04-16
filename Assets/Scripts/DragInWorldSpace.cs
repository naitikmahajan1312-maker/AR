using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DragInWorldSpace : MonoBehaviour
{
    public Camera cam;
    [Header("Throw")]
    public float throwPower = 8f;      // increase for stronger throw
    public float maxThrowSpeed = 20f;  // clamp

    Rigidbody rb;
    Plane dragPlane;
    bool dragging;

    Vector3 lastPos;
    Vector3 throwVel;   // estimated velocity while dragging

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!cam) cam = Camera.main;
    }

    void OnMouseDown()
    {
        // Drag on horizontal plane at object's current height
        float y = transform.position.y;
        dragPlane = new Plane(Vector3.up, new Vector3(0f, y, 0f));

        dragging = true;
        rb.isKinematic = true;

        lastPos = transform.position;
        throwVel = Vector3.zero;
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 p = ray.GetPoint(enter);
            transform.position = p;

            // Estimate velocity from movement (world space)
            float dt = Mathf.Max(Time.deltaTime, 0.0001f);
            throwVel = (transform.position - lastPos) / dt;
            lastPos = transform.position;
        }
    }

    void OnMouseUp()
    {
        dragging = false;
        rb.isKinematic = false;

        // Clamp and apply as an impulse (feels like a throw)
        Vector3 v = Vector3.ClampMagnitude(throwVel * throwPower, maxThrowSpeed);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(v, ForceMode.VelocityChange);
    }
}
