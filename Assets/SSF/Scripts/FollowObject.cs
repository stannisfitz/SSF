using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public bool SnapOnFloor = false;
    public Transform TargetTransform;
    private Vector3 _initialDelta;

	void Start ()
    {
        _initialDelta = transform.position- TargetTransform.position;

    }
	
	void LateUpdate ()
    {
        transform.position = TargetTransform.position + _initialDelta;
        if(SnapOnFloor)
        {
            Snap();
        }
    }
    void Snap()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -Vector3.up);
        if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
        {
            Vector3 pos = hitInfo.point;
            pos.x = transform.position.x;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}
