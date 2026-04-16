using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeAndShootingScript : MonoBehaviour
{
    [SerializeField] private Transform transPoint1;
    [SerializeField] private Transform transPoint2;
    [SerializeField] private Transform ballPrefab;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Camera cam;


    private LineRenderer _lineRenderer;
    private Transform _newBall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _newBall == null)
        {
            _newBall = Instantiate(ballPrefab, spawnPoint, Quaternion.identity);

            if (_newBall)
            {
                if (_lineRenderer.positionCount < 3)
                {
                    _lineRenderer.positionCount = 3;
                }

                Vector3 newPos = _newBall.position;
                newPos.z = -.55f;
                newPos.y = .6f;

                _lineRenderer.SetPosition(1, newPos);
            }
        }

        if (Input.GetMouseButton(0) && _newBall)
        {
            // Drag on a horizontal plane at the ball height (same height you were forcing)
            float yPlane = 0.25f; // keep your existing height
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, yPlane, 0f));

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);

                // Now it will move freely in X and Z (any mouse direction), while staying at yPlane
                _newBall.position = new Vector3(worldPos.x, worldPos.y, worldPos.z);

                Vector3 newPos = _newBall.position;
                newPos.z -= .55f;
                newPos.y += .6f;
                _lineRenderer.SetPosition(1, newPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 newPos = _newBall.position;
            newPos.z = 0f;
            newPos.y = 0.5f;
            _lineRenderer.SetPosition(1, newPos);

            _newBall.GetComponent<Rigidbody>().isKinematic = false;
            _newBall.GetComponent<Rigidbody>().AddForce(_newBall.forward * 100f);
            _newBall = null;
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, transPoint1.position);
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transPoint2.position);
    }
}
