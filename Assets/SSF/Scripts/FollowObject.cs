using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public bool SnapOnFloor = false;
    public Transform TargetTransform;
    private Vector3 _initialDelta;
    public float _initialScale;

	void Awake ()
    {
        _initialDelta = transform.InverseTransformDirection(transform.position- TargetTransform.position);
        _initialScale = TargetTransform.localScale.x;

    }
	
	void LateUpdate ()
    {
        float scale = TargetTransform.localScale.x;
        float r = scale/_initialScale;
        transform.position = TargetTransform.position + transform.TransformDirection(_initialDelta*r);
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
