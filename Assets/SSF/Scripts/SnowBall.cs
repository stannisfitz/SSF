using UnityEngine;
using System.Collections;

public class SnowBall : MonoBehaviour
{
    public enum State
    {
        Rolling,
        Free
    }

    public float MaxSize = 3.0f;
    public float GrowSpeed = 0.5f;
    public float BounceFactor = 50.0f;

    public Material[] SnowMaterial;

    private Rigidbody _rigidBody;
    private Collider _collider;
    private Renderer _renderer;

    private Vector3 _lasPosition;
    private Vector3 _lastHitPosition;
    private bool _wasOnFloor = false;
    private State _currentState = State.Rolling;
    public State CurrentState
    {
        get { return _currentState; }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();

    }
    void Start()
    {
        UpdatePosition(false);
    }

    void Update()
    {
        if (_currentState == State.Rolling)
        {
            UpdatePosition();
            _lasPosition = transform.position;
        }
    }

    private void UpdatePosition(bool canGrow = true)
    {
        float scale = transform.localScale.x;
        RaycastHit hitInfo;
        Ray r1 = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        Ray r2 = new Ray(transform.position + Vector3.up * 0.5f + Vector3.forward*scale*0.25f, Vector3.down);
        Ray r3 = new Ray(transform.position + Vector3.up * 0.5f - Vector3.forward * scale * 0.25f, Vector3.down);
        Ray r4 = new Ray(transform.position + Vector3.up * 0.5f + Vector3.right * scale * 0.25f, Vector3.down);
        Ray r5 = new Ray(transform.position + Vector3.up * 0.5f - Vector3.right * scale * 0.25f, Vector3.down);
        Vector3 p1 = Vector3.zero;
        Vector3 p2 = Vector3.zero;
        Vector3 p3 = Vector3.zero;
        Vector3 p4 = Vector3.zero;
        Vector3 p5 = Vector3.zero;
        int triangleId1 = -1;
        int triangleId2 = -1;
        int triangleId3 = -1;
        int triangleId4 = -1;
        int triangleId5 = -1;
        if (Physics.Raycast(r1, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            p1 = hitInfo.point;
            triangleId1 = hitInfo.triangleIndex;
        }

        if (Physics.Raycast(r2, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            p2 = hitInfo.point;
            triangleId2 = hitInfo.triangleIndex;
        }

        if (Physics.Raycast(r3, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            p3 = hitInfo.point;
            triangleId3 = hitInfo.triangleIndex;
        }

        if (Physics.Raycast(r4, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            p4 = hitInfo.point;
            triangleId4 = hitInfo.triangleIndex;
        }

        if (Physics.Raycast(r5, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            p5 = hitInfo.point;
            triangleId5 = hitInfo.triangleIndex;
        }

        float distance = (_lastHitPosition - transform.position).magnitude;
        float lastScale = scale;
        if (triangleId1 >= 0)
        {
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, p1, hitInfo.collider))
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
                _rigidBody.mass = scale;
            }
        }
        if (triangleId2 >= 0 && triangleId2 != triangleId1)
        {
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, p2, hitInfo.collider))
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
                _rigidBody.mass = scale;
            }
        }

        if (triangleId3 >= 0 && triangleId3 != triangleId1 && triangleId3 != triangleId2)
        {
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, p3, hitInfo.collider))
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
                _rigidBody.mass = scale;
            }
        }
        if (triangleId4 >= 0 && triangleId4 != triangleId1 && triangleId4 != triangleId2 && triangleId4 != triangleId3)
        {
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, p4, hitInfo.collider))
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
                _rigidBody.mass = scale;
            }
        }
        if (triangleId5 >= 0 && triangleId5 != triangleId1 && triangleId5 != triangleId2 && triangleId5 != triangleId4)
        {
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, p5, hitInfo.collider))
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
                _rigidBody.mass = scale;
            }
        }
        _lasPosition = transform.position;
        _lastHitPosition = transform.position;
    }

    public void SetTeam(int team)
    {
        _renderer.material = SnowMaterial[team];
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == gameObject.layer)
        {
            Vector3 delta = transform.position - collision.collider.transform.position;
            Rigidbody r = collision.collider.GetComponent<Rigidbody>();
            if (r != null)
            {
                _rigidBody.AddForce(delta.normalized * r.mass * BounceFactor);
            }
        }
    }
}
