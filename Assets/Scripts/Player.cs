using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public PlayerInfo playerInfo;

    public virtual void Awake() {
        playerInfo.playerManager = FindObjectOfType<PlayerManager>();
        playerInfo.playerController = GetComponent<Controller2D>();
        playerInfo.arm = GetComponentInChildren<Arm>();
    }

    private void OnEnable() {
        PlayerManager.OnDirectionalInput += SetDirectionalInput;
        PlayerManager.OnJumpInputDown += OnJumpInputDown;
        PlayerManager.OnJumpInputUp += OnJumpInputUp;
    }

    private void OnDisable() {
        PlayerManager.OnDirectionalInput -= SetDirectionalInput;
        PlayerManager.OnJumpInputDown -= OnJumpInputDown;
        PlayerManager.OnJumpInputUp -= OnJumpInputUp;
    }

    private void Start() {
        playerInfo.gravity = -(2 * playerInfo.playerManager.maxJumpHeight) / Mathf.Pow(playerInfo.playerManager.timeToJumpApex, 2);
        playerInfo.maxJumpVelocity = Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.timeToJumpApex;
        playerInfo.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerInfo.gravity) * playerInfo.playerManager.minJumpHeight);
        print("Gravity: " + playerInfo.gravity + " Jump Velocity: " + playerInfo.maxJumpVelocity);
    }

    public virtual void Update() {
        if (playerInfo.wrist == null) {
            CalculateVelocity();
            HandleWallSliding();

            playerInfo.playerController.Move(playerInfo.velocity * Time.deltaTime, playerInfo.directionalInput);
            if (playerInfo.playerController.collisions.above || playerInfo.playerController.collisions.below) {
                if (playerInfo.playerController.collisions.slidingDownMaxSlope) {
                    playerInfo.velocity.y += playerInfo.playerController.collisions.slopeNormal.y * -playerInfo.gravity * Time.deltaTime;
                } else {
                    playerInfo.velocity.y = 0;
                }
            }
        }
    }

    public void SetDirectionalInput(Vector2 input) {
        playerInfo.directionalInput = input;
    }

    public void OnJumpInputDown() {
        playerInfo.isJumpingApex = true;
        if (playerInfo.wallSliding) {
            if (playerInfo.wallDirX == playerInfo.directionalInput.x) {
                playerInfo.velocity.x = -playerInfo.wallDirX * playerInfo.playerManager.wallJumpClimb.x;
                playerInfo.velocity.y = playerInfo.playerManager.wallJumpClimb.y;
            } else if (playerInfo.directionalInput.x == 0) {
                playerInfo.velocity.x = -playerInfo.wallDirX * playerInfo.playerManager.wallJumpOff.x;
                playerInfo.velocity.y = playerInfo.playerManager.wallJumpOff.y;
            } else {
                playerInfo.velocity.x = -playerInfo.wallDirX * playerInfo.playerManager.wallLeap.x;
                playerInfo.velocity.y = playerInfo.playerManager.wallLeap.y;
            }
        }
        if (playerInfo.playerController.collisions.below && !playerInfo.ledgeGrabbing) {
            if (playerInfo.playerController.collisions.slidingDownMaxSlope) {
                if (playerInfo.directionalInput.x != -Mathf.Sign(playerInfo.playerController.collisions.slopeNormal.x)) { //not jumping against max slope
                    playerInfo.velocity.y = playerInfo.playerManager.maxJumpHeight * playerInfo.playerController.collisions.slopeNormal.y;
                    playerInfo.velocity.x = playerInfo.playerManager.maxJumpHeight * playerInfo.playerController.collisions.slopeNormal.x;
                }
            } else {
                playerInfo.velocity.y = playerInfo.maxJumpVelocity;
            }
        }
    }

    public virtual void OnJumpInputUp() {
        if (playerInfo.velocity.y > playerInfo.minJumpVelocity) {
            playerInfo.velocity.y = playerInfo.minJumpVelocity;
        }
    }

    void HandleWallSliding() {
        playerInfo.wallDirX = (playerInfo.playerController.collisions.left) ? -1 : 1;
        playerInfo.wallSliding = false;
        if ((playerInfo.playerController.collisions.left || playerInfo.playerController.collisions.right) && !playerInfo.playerController.collisions.below && playerInfo.velocity.y < 0 && !playerInfo.ledgeGrabbing) {
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

    void CalculateVelocity() {
        float targetVelocityX = playerInfo.directionalInput.x * playerInfo.playerManager.moveSpeed;
        playerInfo.velocity.x = Mathf.SmoothDamp(playerInfo.velocity.x, targetVelocityX, ref playerInfo.velocityXSmoothing, (playerInfo.playerController.collisions.below) ? playerInfo.playerManager.accelerationTimeGrounded : playerInfo.playerManager.accelerationTimeAirborne);
        playerInfo.velocity.y += playerInfo.gravity * Time.deltaTime;
    }

    void GainFootHold() {
        if (playerInfo.playerController._collider.tag == "Player") {
            
        }
    }
    public struct PlayerInfo {
        public float timeToWallUnstick;

        public float gravity;
        public float maxJumpVelocity;
        public float minJumpVelocity;
        public Vector3 velocity;
        public float velocityXSmoothing;
        public Vector2 directionalInput;
        public Vector2 grabDirectionalInput;

        public PlayerManager playerManager;
        public Controller2D playerController;
        
        public Arm arm;
        public Wrist wrist;

        public bool wallSliding;
        public int wallDirX;
        public bool isJumpingApex;
        public bool ledgeGrabbing;
    }
}

