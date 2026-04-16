using UnityEngine;

public class GenericCharacter : MonoBehaviour
{
    public float rotateSpeed = 0.2f;   // adjust

    bool dragging = false;
    Vector2 lastPos;

    void Update()
    {
        // Mouse (Editor)
        if (Input.GetMouseButtonDown(0))
        {
            if (HitThisObject(Input.mousePosition))
            {
                dragging = true;
                lastPos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0)) dragging = false;

        if (dragging && Input.GetMouseButton(0))
        {
            Vector2 cur = Input.mousePosition;
            RotateByDelta(cur - lastPos);
            lastPos = cur;
        }

        // Touch (Mobile)
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                if (HitThisObject(t.position))
                {
                    dragging = true;
                    lastPos = t.position;
                }
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                dragging = false;
            }
            else if (dragging && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
            {
                RotateByDelta(t.position - lastPos);
                lastPos = t.position;
            }
        }
    }

    bool HitThisObject(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    void RotateByDelta(Vector2 delta)
    {
        // horizontal drag = rotate around Y
        transform.Rotate(0f, -delta.x * rotateSpeed, 0f, Space.World);

        // vertical drag = rotate around X (optional)
        transform.Rotate(delta.y * rotateSpeed, 0f, 0f, Space.World);
    }
}
