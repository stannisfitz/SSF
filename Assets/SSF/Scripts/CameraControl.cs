using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0.0f, 1.0f, 0.0f);

   // public float FollowSpeed = 10f;
    //public float RotationSpeed = 10f;

    private Vector3 relCameraPos;
    private Vector3 _lasttargetPos;

    void Awake()
    {
        relCameraPos = transform.position - target.position;
    }

    void LateUpdate()
    {
        transform.position = target.position + target.TransformVector(relCameraPos);// Vector3.MoveTowards(transform.position, target.position + target.TransformVector(relCameraPos), FollowSpeed * Time.deltaTime);

        Vector3 targetPos = target.position + target.TransformVector(targetOffset);
        //_lasttargetPos =  Vector3.MoveTowards(_lasttargetPos, targetPos, RotationSpeed * Time.deltaTime);
        //Quaternion r = Quaternion.LookRotation(_lasttargetPos - transform.position);
        Quaternion r = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = r;
    }
}
