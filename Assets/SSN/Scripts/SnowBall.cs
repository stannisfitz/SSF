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
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position+transform.up*0.5f,-transform.up);
        if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1<<LayerMask.NameToLayer("Terrain")))
        {
            Vector3 hitPos = hitInfo.point;
            Vector3 parentPos = transform.parent.position;
            parentPos.y = 0.0f;
            float scale = transform.localScale.x;
            hitPos.x = 0.0f;
            hitPos.z = 0.0f;
            transform.position = parentPos + transform.parent.forward * 0.3f + transform.parent.forward * scale * 0.5f + hitPos + transform.up * 0.5f * scale;
            float distance = (_lastHitPosition - transform.position).magnitude;
            float lastScale = scale;
            if (_wasOnFloor && canGrow)
            {
                scale += Mathf.Min(0.1f, distance * Time.deltaTime * GrowSpeed);
                scale = Mathf.Min(scale, MaxSize);
                transform.localScale = new Vector3(scale, scale, scale);
            }
            _wasOnFloor = true;
            SnowManager.Instance.UpdateSnow(_lastHitPosition, transform.position);
        }
        else
        {
            _wasOnFloor = false;
        }
        if (canGrow && Mathf.Abs(transform.parent.position.y - transform.position.y) > 1.0f)
        {
            //grr
            ShouldDrop = true;
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
