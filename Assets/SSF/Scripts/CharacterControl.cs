using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

    [RequireComponent(typeof(Character))]
    public class CharacterControl : MonoBehaviour
    {
        public GameObject SnowBallPrefab;
        public GameObject _currentSnowball = null;

        private Character m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private Animator _animator;
        private Vector3 _startPosition;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _startPosition = transform.position;
    }
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<Character>();
        }


        private void Update()
        {
        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = _startPosition;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(1);
        }

        if (_currentSnowball == null && SnowBallPrefab != null && Input.GetButtonDown("Action"))
        {
            _currentSnowball = GameObject.Instantiate(SnowBallPrefab, transform.position+transform.forward*2.5f, Quaternion.identity) as GameObject;
            _currentSnowball.transform.parent = transform;
            _animator.SetTrigger("Action");
        }
        else if (_currentSnowball != null && (_currentSnowball.GetComponent<SnowBall>().ShouldDrop || Input.GetButtonDown("Action")))
        {
            _currentSnowball.GetComponent<SnowBall>().Drop();
            _animator.SetTrigger("Action");
            _currentSnowball = null;
        }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);
            _animator.SetBool("Walking", Mathf.Abs(h) > Mathf.Epsilon || Mathf.Abs(v) > Mathf.Epsilon);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
        // walk speed multiplier
        if (Input.GetButton("Dash"))
        {
            _animator.SetBool("Running", true);
            m_Move *= 2.0f;
        }
        else
        {
            _animator.SetBool("Running", false);
        }



            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
