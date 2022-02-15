using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrist : Player {

    //float accelerationTimeAirborne = .2f;
    //float accelerationTimeGrounded = .1f;
    //float moveSpeed = 6;

    //public Vector2 wallJumpClimb;
    //public Vector2 wallJumpOff;
    //public Vector2 wallLeap;

    //public float wallSlideSpeedMax = 3;
    //public float wallStickTime = 0.25f;
    //float timeToWallUnstick;

    //float gravity;
    //float maxJumpVelocity;
    //float minJumpVelocity;
    //[HideInInspector] public Vector3 velocity;
    //float velocityXSmoothing;

    //Controller2D controller;
    //GameObject player;
    ArmController armControllerScript;
    Player player;
    //Arm arm;
    //Player.Info playerInfo;
    //Vector2 directionalInput;
    //bool wallSliding;
    //int wallDirX;
    //bool isJumping;
    //[HideInInspector] public bool ledgeGrabbing;

    public override void Awake() {
        //base.Awake();
        //controller = GetComponent<Controller2D>();
        //player = GameObject.FindGameObjectWithTag("Player");//need to change this
        //playerInfo.arm = GetComponentInParent<Arm>();
        player = transform.parent.parent.GetComponent<Player>();
        playerInfo = player.playerInfo;
        armControllerScript = GetComponentInParent<ArmController>();
        playerInfo.controller._collider = GetComponent<BoxCollider2D>();
        playerInfo.controller.CalculateRaySpacing();
        //arm.wristScript = this;
        //armControllerScript.WristInstance = this.gameObject;
        //player.GetComponent<PlayerInput>().wristScript = this;
    }

    private void OnEnable()
    {
        PlayerManager.OnDirectionalInput += SetDirectionalInput;
        PlayerManager.OnJumpInputDown += OnJumpInputDown;
        PlayerManager.OnJumpInputUp += OnJumpInputUp;
        PlayerManager.OnGrabRelease += GrabRelease;
        PlayerManager.OnDirectionalInput -= player.SetDirectionalInput;
        PlayerManager.OnJumpInputDown -= player.OnJumpInputDown;
        PlayerManager.OnJumpInputUp -= player.OnJumpInputUp;

    }
    private void OnDisable()
    {
        PlayerManager.OnDirectionalInput -= SetDirectionalInput;
        PlayerManager.OnJumpInputDown -= OnJumpInputDown;
        PlayerManager.OnJumpInputUp -= OnJumpInputUp;
        PlayerManager.OnGrabRelease -= GrabRelease;
        PlayerManager.OnDirectionalInput += player.SetDirectionalInput;
        PlayerManager.OnJumpInputDown += player.OnJumpInputDown;
        PlayerManager.OnJumpInputUp += player.OnJumpInputUp;
    }

    void Start() {
        
        //player = GetComponentInParent<Player>();
        //playerInfo = GetComponentInParent<Player>().playerInfo;
        //playerInfo.gravity = -(2 * playerInfo.playerManager.maxJumpHeight) / Mathf.Pow(playerInfo.playerManager.timeToJumpApex, 2);
        //playerInfo.maxJumpVelocity = Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.timeToJumpApex;
        //playerInfo.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.minJumpHeight);
        //print("Wrist: Gravity: " + playerInfo.gravity + " Jump Velocity: " + playerInfo.maxJumpVelocity);
    }

    //public void DisablePlayerMovement() {
    //    player.GetComponent<Player>().enabled = false;
    //    player.GetComponent<Controller2D>().enabled = false;
    //}

    public override void Update() {
        CalculateVelocity();
        HandleWallSliding();

        if (playerInfo.isJumping & !playerInfo.controller.collisions.below)
        {
            GrabRelease();
        }
        playerInfo.controller.Move(playerInfo.velocity * Time.deltaTime, playerInfo.directionalInput);
        //player.transform.position = (Vector2)transform.position;

        //if (playerInfo.directionalInput != null)
        //{

        //}
        if (playerInfo.controller.collisions.above || playerInfo.controller.collisions.below)
        {
            if (playerInfo.controller.collisions.slidingDownMaxSlope)
            {
                playerInfo.velocity.y += playerInfo.controller.collisions.slopeNormal.y * -playerInfo.gravity * Time.deltaTime;
            }
            else
            {
                playerInfo.velocity.y = 0;
            }
        }
    }

    //public void SetDirectionalInput(Vector2 input) {
    //    directionalInput = input;
    //}

    void CalculateVelocity() {
        float targetVelocityX = playerInfo.directionalInput.x * playerInfo.playerManager.moveSpeed;
        playerInfo.velocity.x = Mathf.SmoothDamp(playerInfo.velocity.x, targetVelocityX, ref playerInfo.velocityXSmoothing, (playerInfo.controller.collisions.below) ? playerInfo.playerManager.accelerationTimeGrounded : playerInfo.playerManager.accelerationTimeAirborne);
        playerInfo.velocity.y += playerInfo.gravity * Time.deltaTime;
    }

    //public void OnJumpInputDown() {
    //    if (wallSliding) {
    //        if (wallDirX == directionalInput.x){
    //            velocity.x = -wallDirX * playerManager.wallJumpClimb.x;
    //            velocity.y = playerManager.wallJumpClimb.y;
    //        } else if (directionalInput.x == 0) {
    //            velocity.x = -wallDirX * playerManager.wallJumpOff.x;
    //            velocity.y = playerManager.wallJumpOff.y;
    //        } else {
    //            velocity.x = -wallDirX * playerManager.wallLeap.x;
    //            velocity.y = playerManager.wallLeap.y;
    //        }
    //    }
    //    if (controller.collisions.below || ledgeGrabbing) {
    //        if (controller.collisions.slidingDownMaxSlope) {
    //            if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { //not jumping against max slope
    //                velocity.y = playerManager.maxJumpHeight * controller.collisions.slopeNormal.y;
    //                velocity.x = playerManager.maxJumpHeight * controller.collisions.slopeNormal.x;
    //            }
    //        } else {
    //            //if (playerControllerScript.collisions.holdingObject)
    //            //{
    //            //    velocity.y = maxJumpVelocity + playerScript.ObjectScript.maxJumpVelocity;
    //            //}
    //            //else
    //            //{
    //            //    velocity.y = maxJumpVelocity;
    //            //}
    //            velocity.y = maxJumpVelocity;
    //        }
    //    }
    //    isJumping = true;
    //}

    void HandleWallSliding() {
        playerInfo.wallDirX = (playerInfo.controller.collisions.left) ? -1 : 1;
        playerInfo.wallSliding = false;
        if ((playerInfo.controller.collisions.left || playerInfo.controller.collisions.right) && !playerInfo.controller.collisions.below && playerInfo.velocity.y < 0) {
            playerInfo.wallSliding = true;

            if (playerInfo.velocity.y < -playerInfo.playerManager.wallSlideSpeedMax) {
                playerInfo.velocity.y = -playerInfo.playerManager.wallSlideSpeedMax;
            }

            if (playerInfo.timeToWallUnstick > 0) {
                playerInfo.velocityXSmoothing = 0;
                playerInfo.velocity.x = 0;

                if (playerInfo.directionalInput.x != playerInfo.wallDirX && playerInfo.directionalInput.x != 0) {
                    playerInfo.timeToWallUnstick -= Time.deltaTime;
                } else {
                    playerInfo.timeToWallUnstick = playerInfo.playerManager.wallStickTime;
                }
            } else {
                playerInfo.timeToWallUnstick = playerInfo.playerManager.wallStickTime;
            }
        }
    }

    public void GrabRelease() {
        playerInfo.ledgeGrabbing = false;

        //armControllerScript.enabled = true;
        //playerScript.enabled = true;//-------------------------------
        //playerScript.wristScript = null;
        //playerControllerScript.WristInstance = null;
        playerInfo.controller.collisions.ResetGrab();
        playerInfo.controller._collider = player.GetComponent<BoxCollider2D>();
        playerInfo.controller.CalculateRaySpacing();
        //gravity = 0;
        Destroy(gameObject, 0.5f);
        //player.GetComponent<PlayerInput>().wristScript = null;



        //StartCoroutine("WristDestroy");
        //if (playerInfo.ledgeGrabbing) {

            
        //}
    }

    //IEnumerator WristDestroy() {
    //    yield return new WaitForSeconds(0.1f);
    //    Destroy(gameObject);
    //    ledgeGrabbing = false;
    //    armControllerScript.enabled = true;
    //    playerScript.enabled = true;
        
    //    armControllerScript.collisions.ResetGrab();

    //    //player.GetComponent<PlayerInput>().wristScript = null;
    //    //arm.wristScript = null;
    //    //Destroy(gameObject, 0.5f);
       

    //}
}
