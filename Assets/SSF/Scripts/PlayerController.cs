using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int Team = 0;
    public float MoveForce = 10.0f;
    public Rigidbody RigidBodyComponent;
    public Animator AnimatorComponent;
	
	void Update ()
    {
        Vector3 force = Vector3.zero;
        if ((Team == 0 && Input.GetKey(KeyCode.W)) || (Team == 1 && Input.GetKey(KeyCode.UpArrow)))
        {
            force += Vector3.forward;
        }
        else if ((Team == 0 && Input.GetKey(KeyCode.S)) || (Team == 1 && Input.GetKey(KeyCode.DownArrow)))
        {
            force -= Vector3.forward;
        }

        if ((Team == 0 && Input.GetKey(KeyCode.D)) || (Team == 1 && Input.GetKey(KeyCode.RightArrow)))
        {
            force += Vector3.right;
        }
        else if ((Team == 0 && Input.GetKey(KeyCode.A)) || (Team == 1 && Input.GetKey(KeyCode.LeftArrow)))
        {
            force -= Vector3.right;
        }

        force = force.normalized * MoveForce;

        RigidBodyComponent.AddForce(force);

        float speed = RigidBodyComponent.velocity.magnitude;
        AnimatorComponent.SetBool("Walking", speed < 5.0f && speed > 0.1f);
        AnimatorComponent.SetBool("Running", speed >= 5.0f);
    }
}
