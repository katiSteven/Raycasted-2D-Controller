using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

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
    private Vector3 velocity;
    float velocityXSmoothing;

    [HideInInspector] public Controller2D controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;
    bool isJumping;


    //bool ledgeGrabbing;
    [SerializeField] public float ArmLength = 1f;
    [SerializeField] public float WristLength = 0.1f;
    //public GameObject wrist { get; set; }
    public Wrist wristScript { get; set; }

    public Vector3 Velocity { get => velocity; set => velocity = value; }

    private void Awake() {
        controller = GetComponent<Controller2D>();
        controller.ArmLength = ArmLength;
        controller.WristLength = WristLength;
    }

    void Start() {
        
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
    }

    void Update() {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below) {
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            } else {
                velocity.y = 0;
            }
        }
        if (controller.collisions.Grabbing())
        {
            print("Grab detected 1");
            //wrist = FindObjectOfType<Wrist>().gameObject;
            if (wristScript != null)
            {
                print("wristScript detected 1");
                //wristScript = wrist.GetComponent<Wrist>();
                wristScript.ledgeGrabbing = true;
                //wristScript.DisablePlayerMovement();
                controller.enabled = false;
                enabled = false;
                print("reached");
            }
        }
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    public void OnJumpInputDown() {
        isJumping = true;
        if (wallSliding) {
            if (wallDirX == directionalInput.x) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            } else if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            } else {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below || CheckLedgeGrabbingActive()) {
            if (controller.collisions.slidingDownMaxSlope) {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { //not jumping against max slope
                    velocity.y = maxJumpHeight * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpHeight * controller.collisions.slopeNormal.x;
                }
            } else {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp() {
        if (velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }
        isJumping = false;
    }

    void HandleWallSliding() {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && !CheckLedgeGrabbingActive()) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0) {
                    timeToWallUnstick -= Time.deltaTime;
                } else {
                    timeToWallUnstick = wallStickTime;
                }
            } else {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void GrabInputdown(Vector2 grabDirectionalInput) {
        //if (!controller.collisions.Grabbing()) {
            
        //}

         if(!CheckLedgeGrabbingActive()) {
            controller.GrappleCollisions(velocity * Time.deltaTime, grabDirectionalInput);
        }
    }

    bool CheckLedgeGrabbingActive() {
        if (wristScript != null) {
            if (wristScript.ledgeGrabbing) {
                return true;
            }
        }
        return false;
    }

    //public void GrabRelease() {
    //    if (ledgeGrabbing) {
    //        ledgeGrabbing = false;
    //        controller.collisions.ResetGrab();
    //    }
    //}
}
//LedgeCollider