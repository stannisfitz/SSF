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

    private Rigidbody _rigidBody;
    private Collider _collider;

    private Vector3 _lasPosition;
    private Vector3 _lastHitPosition;
    private bool _wasOnFloor = false;

    public bool ShouldDrop = false;//grr
    private State _currentState = State.Rolling;
    public State CurrentState
    {
        get { return _currentState; }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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
    public void Drop()
    {
        if(_currentState != State.Rolling)
        {
            return;
        }
        transform.parent = null;
        _currentState = State.Free;
        _collider.enabled = true;
        _rigidBody.isKinematic = false;
        _rigidBody.WakeUp();
        _rigidBody.velocity = (transform.position - _lasPosition).normalized * 3.0f;
        SnowManager.Instance.SnowBallDropped(this);
    }
}
