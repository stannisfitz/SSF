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
        Ray ray = new Ray(transform.position+Vector3.up*0.5f,-Vector3.up);
        RaycastHit[] hits = Physics.SphereCastAll(ray, transform.localScale.x*0.25f, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch"));
        foreach(var hit in hits)
        {
            float scale = transform.localScale.x;
            float distance = (_lastHitPosition - transform.position).magnitude;
            float lastScale = scale;
            if (SnowManager.Instance.UpdateSnow(_lastHitPosition, transform.position, hit.collider))
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
}
