using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public Transform TargetTransform;
    private Vector3 _initialDelta;

	void Start ()
    {
        _initialDelta = transform.position- TargetTransform.position;

    }
	
	void LateUpdate ()
    {
        transform.position = TargetTransform.position + _initialDelta;
    }
}
