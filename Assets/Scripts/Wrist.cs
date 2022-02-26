using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrist : Player {
    ArmController armControllerScript;
    Player player;
    float armLength;

    public override void Awake() {
        player = transform.parent.parent.GetComponent<Player>();
        playerInfo = player.playerInfo;
        armControllerScript = GetComponentInParent<ArmController>();
        playerInfo.playerController._collider = GetComponent<BoxCollider2D>();
        playerInfo.playerController.CalculateRaySpacing();
    }

    //public override void Start()
    //{
    //    //base.Start();
    //    playerInfo.gravity = -(2 * playerInfo.playerManager.maxJumpHeight) / Mathf.Pow(playerInfo.playerManager.timeToJumpApex, 2);
    //    playerInfo.maxJumpVelocity = Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.timeToJumpApex;
    //    playerInfo.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.minJumpHeight);
    //    print("Gravity: " + playerInfo.gravity + " Jump Velocity: " + playerInfo.maxJumpVelocity);

    //    player = transform.parent.parent.GetComponent<Player>();
    //    playerInfo = player.playerInfo;
    //    armControllerScript = GetComponentInParent<ArmController>();
    //    playerInfo.playerController._collider = GetComponent<BoxCollider2D>();
    //    playerInfo.playerController.CalculateRaySpacing();
    //}

    private void OnEnable() {
        PlayerManager.OnDirectionalInput += SetDirectionalInput;
        PlayerManager.OnJumpInputDown += OnJumpInputDown;
        PlayerManager.OnClimbLedge += ClimbLedge;
        PlayerManager.OnJumpInputUp += OnJumpInputUp;
        PlayerManager.OnGrabRelease += GrabRelease;
        //PlayerManager.OnDirectionalInput -= player.SetDirectionalInput;
        //PlayerManager.OnJumpInputDown -= player.OnJumpInputDown;
        //PlayerManager.OnJumpInputUp -= player.OnJumpInputUp;

    }
    private void OnDisable() {
        PlayerManager.OnDirectionalInput -= SetDirectionalInput;
        PlayerManager.OnJumpInputDown -= OnJumpInputDown;
        PlayerManager.OnClimbLedge -= ClimbLedge;
        PlayerManager.OnJumpInputUp -= OnJumpInputUp;
        PlayerManager.OnGrabRelease -= GrabRelease;
        //PlayerManager.OnDirectionalInput += player.SetDirectionalInput;
        //PlayerManager.OnJumpInputDown += player.OnJumpInputDown;
        //PlayerManager.OnJumpInputUp += player.OnJumpInputUp;
    }

    public override void Update() {
        CalculateVelocity();
        HandleWallSliding();
        LedgeEdgeCollision();
        
        playerInfo.playerController.Move(playerInfo.velocity * Time.deltaTime, playerInfo.directionalInput);
        if (playerInfo.playerController.collisions.above || playerInfo.playerController.collisions.below) {
            if (playerInfo.playerController.collisions.slidingDownMaxSlope) {
                playerInfo.velocity.y += playerInfo.playerController.collisions.slopeNormal.y * -playerInfo.gravity * Time.deltaTime;
            } else {
                playerInfo.velocity.y = 0;
            }
        }
        //player.transform.Translate(transform.position * Time.deltaTime, transform);
        //transform.Translate(player.transform.position * Time.deltaTime, Space.Self);
        //if (Mathf.Abs(player.transform.position.y - transform.position.y) >= playerInfo.playerManager.ArmLength*2) {
        //    player.transform.position = Vector2.MoveTowards(player.transform.position, new Vector2(transform.position.x, transform.position.y), 0.05f);
        //    transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y), 0.05f);
        //}
        //player.transform.position = Vector2.MoveTowards(player.transform.position, new Vector2(transform.position.x, player.transform.position.y), 0.05f);
        //transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), 0.05f);
        if (Mathf.Abs(player.transform.position.y - transform.position.y) >= playerInfo.playerManager.ArmLength * 2)
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, new Vector2(transform.position.x, transform.position.y + playerInfo.playerManager.ArmLength * 2), 0.05f);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + playerInfo.playerManager.ArmLength * 2), 0.05f);
        }
        else
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, new Vector2(transform.position.x, player.transform.position.y), 0.05f);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), 0.05f);
        }
        
        //else if (Mathf.Sign(playerInfo.velocity.y) < 0) {
        //    print("falling grab release");
        //    GrabRelease();
        //}

    }

    
    //private void FixedUpdate()
    //{
        
    //}

    //private void LateUpdate()
    //{
        
    //}

    public void LedgeEdgeCollision() {
        if (playerInfo.playerController.collisions.floorHit.collider.tag == "PassablePlatform")
        {
            if (playerInfo.playerController._collider.bounds.min.x <= playerInfo.playerController.collisions.otherColliderLeftVertex.x)
            {
                if (Mathf.Sign(playerInfo.directionalInput.x) == -1)
                {
                    playerInfo.velocity.x = 0;
                }
            }
            if (playerInfo.playerController._collider.bounds.max.x >= playerInfo.playerController.collisions.otherColliderRightVertex.x)
            {
                if (Mathf.Sign(playerInfo.directionalInput.x) == 1)
                {
                    playerInfo.velocity.x = 0;
                }
            }
        }

    }

    public void ClimbLedge() {
        if (playerInfo.playerController.collisions.floorHit) {
            if (playerInfo.playerController.collisions.floorHit.collider.tag == "PassablePlatform") {
                if (playerInfo.playerController.collisions.below && !playerInfo.ledgeGrabbing) {
                    playerInfo.velocity.y = playerInfo.maxJumpVelocity;
                }
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = playerInfo.directionalInput.x * playerInfo.playerManager.moveSpeed;
        playerInfo.velocity.x = Mathf.SmoothDamp(playerInfo.velocity.x, targetVelocityX, ref playerInfo.velocityXSmoothing, (playerInfo.playerController.collisions.below) ? playerInfo.playerManager.accelerationTimeGrounded : playerInfo.playerManager.accelerationTimeAirborne);
        playerInfo.velocity.y += playerInfo.gravity * Time.deltaTime;
    }

    public override void OnJumpInputUp()
    {
        if (playerInfo.velocity.y > playerInfo.minJumpVelocity)
        {
            playerInfo.velocity.y = playerInfo.minJumpVelocity;
            GrabRelease();
        }
    }

    void HandleWallSliding() {
        playerInfo.wallDirX = (playerInfo.playerController.collisions.left) ? -1 : 1;
        playerInfo.wallSliding = false;
        if ((playerInfo.playerController.collisions.left || playerInfo.playerController.collisions.right) && !playerInfo.playerController.collisions.below && playerInfo.velocity.y < 0) {
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
        playerInfo.playerController.collisions.ResetGrab();
        //playerInfo.playerController.collisions.moveAmountOld = Vector2.zero;

        player.enabled = true;
        playerInfo.velocity.x = 0f;
        playerInfo.playerController._collider = player.GetComponent<BoxCollider2D>();/*GetComponent<CapsuleCollider2D>();*/
        playerInfo.playerController.CalculateRaySpacing();
        //gravity = 0;
        Destroy(gameObject, 0.25f);
        //player.GetComponent<PlayerInput>().wristScript = null;
    }
}
