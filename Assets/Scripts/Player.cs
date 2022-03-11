using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {

    protected PlayerMetaData pMD;

    public virtual void Awake() {
        if (PlayerManager.InstanceID_Index_to_PlayerMetaData.ContainsKey(transform.GetInstanceID()) &&
            PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.GetInstanceID()].pI.InstanceID == transform.GetInstanceID()) {
            pMD = PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.GetInstanceID()];
            print(gameObject.name + " got pMD, component: " + name);
        } else {
            print(gameObject.name + " not present in pMD, component: " + name);
            enabled = false;
        }
        
        pMD.pI.playerManager = FindObjectOfType<PlayerManager>();
        pMD.pI.player = this;
        pMD.pI.playerController = GetComponent<Controller2D>();
        pMD.pI.arm = GetComponentInChildren<Arm>();
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
        pMD.pI.gravity = -(2 * pMD.pI.playerManager.maxJumpHeight) / Mathf.Pow(pMD.pI.playerManager.timeToJumpApex, 2);
        pMD.pI.maxJumpVelocity = Mathf.Abs(pMD.pI.gravity) * pMD.pI.playerManager.timeToJumpApex;
        pMD.pI.minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(pMD.pI.gravity) * pMD.pI.playerManager.minJumpHeight);
        print("Gravity: " + pMD.pI.gravity + " Jump Velocity: " + pMD.pI.maxJumpVelocity);
    }

    public virtual void Update() {
        CalculateVelocity();
        HandleWallSliding();

        pMD.pI.playerController.Move(pMD.pI.velocity * Time.deltaTime, pMD.pI.directionalInput);
        if (pMD.pI.playerController.collisions.above || pMD.pI.playerController.collisions.below)
        {
            if (pMD.pI.playerController.collisions.slidingDownMaxSlope)
            {
                pMD.pI.velocity.y += pMD.pI.playerController.collisions.slopeNormal.y * -pMD.pI.gravity * Time.deltaTime;
            }
            else
            {
                pMD.pI.velocity.y = 0;
            }
        }
    }

    public void SetDirectionalInput(Vector2 input) {
        pMD.pI.directionalInput = input;
    }

    public void OnJumpInputDown() {
        pMD.pI.isJumpingApex = true;
        if (pMD.pI.wallSliding) {
            if (pMD.pI.wallDirX == pMD.pI.directionalInput.x) {
                pMD.pI.velocity.x = -pMD.pI.wallDirX * pMD.pI.playerManager.wallJumpClimb.x;
                pMD.pI.velocity.y = pMD.pI.playerManager.wallJumpClimb.y;
            } else if (pMD.pI.directionalInput.x == 0) {
                pMD.pI.velocity.x = -pMD.pI.wallDirX * pMD.pI.playerManager.wallJumpOff.x;
                pMD.pI.velocity.y = pMD.pI.playerManager.wallJumpOff.y;
            } else {
                pMD.pI.velocity.x = -pMD.pI.wallDirX * pMD.pI.playerManager.wallLeap.x;
                pMD.pI.velocity.y = pMD.pI.playerManager.wallLeap.y;
            }
        }
        if (pMD.pI.playerController.collisions.below /*&& !pMD.pI.ledgeGrabbing*/) {
            if (pMD.pI.playerController.collisions.slidingDownMaxSlope) {
                if (pMD.pI.directionalInput.x != -Mathf.Sign(pMD.pI.playerController.collisions.slopeNormal.x)) { //not jumping against max slope
                    pMD.pI.velocity.y = pMD.pI.playerManager.maxJumpHeight * pMD.pI.playerController.collisions.slopeNormal.y;
                    pMD.pI.velocity.x = pMD.pI.playerManager.maxJumpHeight * pMD.pI.playerController.collisions.slopeNormal.x;
                }
            } else {
                pMD.pI.velocity.y = pMD.pI.maxJumpVelocity;
            }
        }
    }

    public virtual void OnJumpInputUp()
    {
        pMD.pI.isJumpingApex = false;
        if (pMD.pI.velocity.y > pMD.pI.minJumpVelocity)
        {
            pMD.pI.velocity.y = pMD.pI.minJumpVelocity;
        }
    }

    void HandleWallSliding() {
        pMD.pI.wallDirX = (pMD.pI.playerController.collisions.left) ? -1 : 1;
        pMD.pI.wallSliding = false;
        if ((pMD.pI.playerController.collisions.left || pMD.pI.playerController.collisions.right) && !pMD.pI.playerController.collisions.below && pMD.pI.velocity.y < 0 && !pMD.pI.ledgeGrabbing) {
            pMD.pI.wallSliding = true;

            if (pMD.pI.velocity.y < -pMD.pI.playerManager.wallSlideSpeedMax) {
                pMD.pI.velocity.y = -pMD.pI.playerManager.wallSlideSpeedMax;
            }

            if (pMD.pI.timeToWallUnstick > 0) {
                pMD.pI.velocityXSmoothing = 0;
                pMD.pI.velocity.x = 0;

                if (pMD.pI.directionalInput.x != pMD.pI.wallDirX && pMD.pI.directionalInput.x != 0) {
                    pMD.pI.timeToWallUnstick -= Time.deltaTime;
                } else {
                    pMD.pI.timeToWallUnstick = pMD.pI.playerManager.wallStickTime;
                }
            } else {
                pMD.pI.timeToWallUnstick = pMD.pI.playerManager.wallStickTime;
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = pMD.pI.directionalInput.x * pMD.pI.playerManager.moveSpeed;
        pMD.pI.velocity.x = Mathf.SmoothDamp(pMD.pI.velocity.x, targetVelocityX, ref pMD.pI.velocityXSmoothing, (pMD.pI.playerController.collisions.below) ? pMD.pI.playerManager.accelerationTimeGrounded : pMD.pI.playerManager.accelerationTimeAirborne);
        pMD.pI.velocity.y += pMD.pI.gravity * Time.deltaTime;
    }
}

