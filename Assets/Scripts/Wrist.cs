using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrist : MonoBehaviour
{
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float minJumpHeight = 1;
    [SerializeField] float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    [HideInInspector] public Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    GameObject player;
    Controller2D playerControllerScript;
    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;
    bool isJumping;
    [HideInInspector] public bool ledgeGrabbing;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Player>().wristScript = this;
        player.GetComponent<PlayerInput>().wristScript = this;
        //player.GetComponent<Player>().controller = controller;
        
        
    }

    // Start is called before the first frame update
    void Start() {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
        
    }

    public void DisablePlayerMovement() {
        player.GetComponent<Player>().enabled = false;
        player.GetComponent<Controller2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        if (isJumping)
        {
            GrabRelease();
        }
        

        if (directionalInput != null)
        {
            controller.Move(velocity * Time.deltaTime, directionalInput);
            player.transform.position = (Vector2)transform.position;/* + (Vector2.down * 0.1f);*/
            //playerControllerScript.Move(velocity * Time.deltaTime, directionalInput);
        }
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
        
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below || ledgeGrabbing)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { //not jumping against max slope
                    velocity.y = maxJumpHeight * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpHeight * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
        isJumping = true;
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
        isJumping = false;
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public void GrabRelease()
    {
        if (ledgeGrabbing)
        {
            ledgeGrabbing = false;
            player.GetComponent<Controller2D>().enabled = true;
            player.GetComponent<Player>().enabled = true;
            player.GetComponent<Controller2D>().collisions.ResetGrab();
            player.GetComponent<PlayerInput>().wristScript = null;
            player.GetComponent<Player>().wristScript = null;
            Destroy(gameObject, 0.5f);
            //DestroyImmediate(gameObject);
        }
    }
}
