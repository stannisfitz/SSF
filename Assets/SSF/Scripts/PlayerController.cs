using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    static int _numPlayers = 0;
    public int Team = 0;
    public float MoveForce = 10.0f;
    public Rigidbody RigidBodyComponent;
    public Animator AnimatorComponent;

    private int _playerId = 0;
	
    void Awake()
    {
        _playerId = _numPlayers++;
    }
	void Update ()
    {
        Vector3 force = Vector3.zero;
        if ((_playerId == 0 && Input.GetKey(KeyCode.W)) || (_playerId == 1 && Input.GetKey(KeyCode.UpArrow)))
        {
            force += Vector3.forward;
        }
        else if ((_playerId == 0 && Input.GetKey(KeyCode.S)) || (_playerId == 1 && Input.GetKey(KeyCode.DownArrow)))
        {
            force -= Vector3.forward;
        }

        if ((_playerId == 0 && Input.GetKey(KeyCode.D)) || (_playerId == 1 && Input.GetKey(KeyCode.RightArrow)))
        {
            force += Vector3.right;
        }
        else if ((_playerId == 0 && Input.GetKey(KeyCode.A)) || (_playerId == 1 && Input.GetKey(KeyCode.LeftArrow)))
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
