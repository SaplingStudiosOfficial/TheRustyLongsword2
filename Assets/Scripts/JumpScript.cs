using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]

public class JumpScript : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] Animator anim;
    private Vector3 _playerVelocity;
    private bool isGrounded;
    public PlayerController Rana;

    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.81f;
    private bool jumpPressed = false;

    private bool isSliding;
    private Vector3 slopeSlideVelocity;

    public AudioClip JumpNoise;
    private AudioSource SoundPlayer;


    // Start is called before the first frame update
    void Start()
    {
        SoundPlayer = this.GetComponent<AudioSource>();
        controller = this.GetComponent< CharacterController >();
        controller.slopeLimit = 45;
    }

    // Update is called once per frame
    void Update()
    {
        if (Rana.isPlaying) {
            JumpMovement();
        }
    }

    void JumpMovement()
    {
        isGrounded = controller.isGrounded;
        //if the player is touching the ground set the y velocity to 0.
        if (isGrounded && isSliding == false)
        {
            _playerVelocity.y = 0.0f;
        }

        //Jump if the jump key is pressed and the player is on the ground.
        if (Input.GetKey(KeyCode.Space) && isGrounded && isSliding == false)
        {
            SoundPlayer.Stop();
            SoundPlayer.PlayOneShot(JumpNoise);
            anim.SetInteger("state", 3);
            _playerVelocity.y += Mathf.Sqrt(jumpHeight * -1 * gravity);
            
        }

        //every frame cause player to fall at the speed of gravity adjusted for real time.
        _playerVelocity.y += gravity * Time.deltaTime;

        SetSlopeSlide();
        if (slopeSlideVelocity == Vector3.zero)
        {
            isSliding = false;
        }
        if (slopeSlideVelocity != Vector3.zero)
        {
            isSliding = true;
        }
        
            controller.Move(_playerVelocity * Time.deltaTime);
        
        if (isSliding)
        {
            Vector3 velocity = slopeSlideVelocity;
            controller.Move(velocity * Time.deltaTime);

        }

    }

    private void SetSlopeSlide()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit HitInfo, 2))
        {
            float angle = Vector3.Angle(HitInfo.normal, Vector3.up);

            if (angle >= controller.slopeLimit)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0f, gravity, 0f), HitInfo.normal);
                return;
            }
        }
        slopeSlideVelocity = Vector3.zero;
    }
}
