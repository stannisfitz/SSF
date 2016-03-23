using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    static int _numPlayers = 0;
    public int Team = 0;
    public float MoveForce = 10.0f;
    public Rigidbody RigidBodyComponent;
    public Animator AnimatorComponent;
    public GameObject SnowBallPrefab;

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

        if ((_playerId == 0 && Input.GetKeyUp(KeyCode.Space)) || (_playerId == 1 && Input.GetKeyUp(KeyCode.RightControl)))
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
        force = force.normalized * MoveForce;

        RigidBodyComponent.AddForce(force);

        float speed = RigidBodyComponent.velocity.magnitude;
        AnimatorComponent.SetBool("Walking", speed < 5.0f && speed > 0.1f);
        AnimatorComponent.SetBool("Running", speed >= 5.0f);
    }
}
