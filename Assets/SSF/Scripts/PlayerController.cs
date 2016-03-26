using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    static int _numPlayers = 0;
    public int Team = 0;
    public float MoveForce = 10.0f;
    public float TurnSpeed = 100.0f;
    public Rigidbody RigidBodyComponent;
    public Animator AnimatorComponent;
    public GameObject SnowBallPrefab;
    public Transform Princess;

    private int _playerId = 0;
	
    void Awake()
    {
        _playerId = _numPlayers++;
    }
	void Update ()
    {
        Vector3 forward = RigidBodyComponent.transform.position- Princess.position;
        forward.y = 0.0f;
        Vector3 force = forward*CrossPlatformInputManager.GetAxis("Vertical" + (_playerId + 1));
        float turn = TurnSpeed*CrossPlatformInputManager.GetAxis("Horizontal" + (_playerId + 1));
        Princess.parent.RotateAround(RigidBodyComponent.transform.position, Vector2.up, turn * Time.deltaTime);
        if (CrossPlatformInputManager.GetButtonDown("Throw" + (_playerId + 1)))
        {
            AnimatorComponent.SetTrigger("Action");
            GameObject snowBall = GameObject.Instantiate(SnowBallPrefab, RigidBodyComponent.transform.position+AnimatorComponent.transform.forward*0.5f, RigidBodyComponent.transform.rotation) as GameObject;
            snowBall.transform.localScale = RigidBodyComponent.gameObject.transform.localScale;
            snowBall.GetComponent<SnowBall>().SetTeam(Team);
            Rigidbody r = snowBall.GetComponent<Rigidbody>();
            r.mass = RigidBodyComponent.mass;
            r.velocity = RigidBodyComponent.velocity;
            Vector3 f = r.velocity.magnitude > 0.05f ? r.velocity*2.0f : AnimatorComponent.transform.forward*2.0f;
            r.AddForce(f);
            RigidBodyComponent.gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        float mult = (RigidBodyComponent.mass < 1.0f) ? RigidBodyComponent.mass : 1.0f;
        force = force.normalized * MoveForce*mult;

        RigidBodyComponent.AddForce(force);

        float speed = RigidBodyComponent.velocity.magnitude;
        AnimatorComponent.SetBool("Walking", speed < 5.0f && speed > 0.1f);
        AnimatorComponent.SetBool("Running", speed >= 5.0f);
    }
}
