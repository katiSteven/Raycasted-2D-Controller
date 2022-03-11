using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrist : Player {
    //PlayerMetaData pMD;
    ArmController armControllerScript;
    //Player player;
    //PlayerManager playerManager;
    //PlayerManager.PlayerInfo playerInfoWrist;
    float armLength;

    public override void Awake() {
        if (PlayerManager.InstanceID_Index_to_PlayerMetaData.ContainsKey(transform.parent.parent.GetInstanceID()) &&
            PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.parent.GetInstanceID()].pI.InstanceID == transform.parent.parent.GetInstanceID())
        {
            pMD = PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.parent.GetInstanceID()];
            print(gameObject.name + " got pMD, component: " + name);
        }
        else
        {
            print(gameObject.name + " could not find pMD, component: " + name);
            enabled = false;
        }
        //player = transform.parent.parent.GetComponent<Player>();
        //playerInfoWrist = PlayerManager.GetPlayerInfo((transform.parent.parent.tag == "Player") ? transform.parent.parent.GetInstanceID() : transform.GetInstanceID()/*, out playerInfo*/);

        //if (pMD.pI.InstanceID == -1/*playerManager != null*/)
        //{
        //    //playerInfo = playerManager.playerInfo;
        //    print("Player Instance ID from From wrist " + pMD.pI.InstanceID);
        //}
        //else
        //{
        //    print(gameObject.name + "|" + this.name + "|" + GetInstanceID() + "| Could not fetch valid playerInfo");
        //}
        //playerInfo
        armControllerScript = GetComponentInParent<ArmController>();
        pMD.pI.wrist = GetComponent<Wrist>();
        
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
        pMD.pI.playerController._collider = GetComponent<BoxCollider2D>();
        pMD.pI.playerController.CalculateRaySpacing();
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

    

    //private void OnDestroy()
    //{
    //    PlayerManager.OnDirectionalInput -= SetDirectionalInput;
    //    PlayerManager.OnJumpInputDown -= OnJumpInputDown;
    //    PlayerManager.OnClimbLedge -= ClimbLedge;
    //    PlayerManager.OnJumpInputUp -= OnJumpInputUp;
    //    PlayerManager.OnGrabRelease -= GrabRelease;
    //}

    public override void Update() {
        CalculateVelocity();
        HandleWallSliding();
        LedgeEdgeCollision();
        
        pMD.pI.playerController.Move(pMD.pI.velocity * Time.deltaTime, pMD.pI.directionalInput);
        if (pMD.pI.playerController.collisions.above || pMD.pI.playerController.collisions.below) {
            if (pMD.pI.playerController.collisions.slidingDownMaxSlope) {
                pMD.pI.velocity.y += pMD.pI.playerController.collisions.slopeNormal.y * -pMD.pI.gravity * Time.deltaTime;
            } else {
                pMD.pI.velocity.y = 0;
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
        if (Mathf.Abs(pMD.pI.player.transform.position.y - transform.position.y) > pMD.pI.playerManager.ArmLength * 2 || Mathf.Abs(pMD.pI.player.transform.position.x - transform.position.x) > pMD.pI.playerManager.ArmLength * 2)
        {
            pMD.pI.player.transform.position = Vector2.MoveTowards(pMD.pI.player.transform.position, new Vector2(transform.position.x, transform.position.y + pMD.pI.playerManager.ArmLength * 2), 0.05f);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(pMD.pI.player.transform.position.x, pMD.pI.player.transform.position.y + pMD.pI.playerManager.ArmLength * 2), 0.05f);
        }
        else
        {
            pMD.pI.player.transform.position = Vector2.MoveTowards(pMD.pI.player.transform.position, new Vector2(transform.position.x, pMD.pI.player.transform.position.y), 0.05f);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(pMD.pI.player.transform.position.x, transform.position.y), 0.05f);
        }

        //else if (Mathf.Sign(playerInfo.velocity.y) < 0) {
        //    print("falling grab release");
        //    GrabRelease();
        //}
        LongWayDown_ReleaseLedge();
    }

    private void LongWayDown_ReleaseLedge()
    {
        //use collisions.Longwaydown here to GrabRelease() the wrist object
        if (pMD.pI.playerController.collisions.longWayDown) {
            GrabRelease();
        }
    }


    //private void FixedUpdate()
    //{

    //}

    //private void LateUpdate()
    //{

    //}

    public void LedgeEdgeCollision() {
        if (pMD.pI.playerController.collisions.floorHit.collider.tag == "PassablePlatform")
        {
            if (pMD.pI.playerController._collider.bounds.min.x >= pMD.pI.playerController.collisions.otherColliderRightVertex.x ||
                    pMD.pI.playerController._collider.bounds.max.x >= pMD.pI.playerController.collisions.otherColliderRightVertex.x)
            {
                if (Mathf.Sign(pMD.pI.directionalInput.x) == 1 && !pMD.pI.isJumpingApex)
                {
                    pMD.pI.velocity.x = 0;
                    pMD.pI.velocity.y = 0;
                }

            }

            //if (Mathf.Sign(playerInfo.directionalInput.x) == 1 && !playerInfo.isJumpingApex) {
                
            //    //if (playerInfo.playerController._collider.bounds.max.x >= playerInfo.playerController.collisions.otherColliderRightVertex.x)
            //    //{
            //    //    if (Mathf.Sign(playerInfo.directionalInput.x) == 1)
            //    //    {
            //    //        playerInfo.velocity.x = 0;
            //    //        playerInfo.velocity.y = 0;
            //    //    }
            //    //}
            //}
            //if (playerInfo.playerController._collider.bounds.min.x <= playerInfo.playerController.collisions.otherColliderLeftVertex.x)
            //{
            //    if (Mathf.Sign(playerInfo.directionalInput.x) == -1)
            //    {
            //        playerInfo.velocity.x = 0;
            //        playerInfo.velocity.y = 0;
            //    }
            //}
            //if (playerInfo.playerController._collider.bounds.max.x <= playerInfo.playerController.collisions.otherColliderLeftVertex.x)
            //{
            //    if (Mathf.Sign(playerInfo.directionalInput.x) == -1)
            //    {
            //        playerInfo.velocity.x = 0;
            //        playerInfo.velocity.y = 0;
            //    }
            //}
            //if (Mathf.Sign(playerInfo.directionalInput.x) == -1) {
                
            //}
            if (pMD.pI.playerController._collider.bounds.min.x <= pMD.pI.playerController.collisions.otherColliderLeftVertex.x ||
                    pMD.pI.playerController._collider.bounds.max.x <= pMD.pI.playerController.collisions.otherColliderLeftVertex.x)
            {
                if (Mathf.Sign(pMD.pI.directionalInput.x) == -1 && !pMD.pI.isJumpingApex)
                {
                    pMD.pI.velocity.x = 0;
                    pMD.pI.velocity.y = 0;
                }

            }
        }

    }

    public void ClimbLedge() {
        if (pMD.pI.playerController.collisions.floorHit) {
            if (pMD.pI.playerController.collisions.floorHit.collider.tag == "PassablePlatform") {
                if (pMD.pI.playerController.collisions.below && !pMD.pI.ledgeGrabbing) {
                    pMD.pI.velocity.y = pMD.pI.maxJumpVelocity;
                }
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = pMD.pI.directionalInput.x * pMD.pI.playerManager.moveSpeed;
        pMD.pI.velocity.x = Mathf.SmoothDamp(pMD.pI.velocity.x, targetVelocityX, ref pMD.pI.velocityXSmoothing, (pMD.pI.playerController.collisions.below) ? pMD.pI.playerManager.accelerationTimeGrounded : pMD.pI.playerManager.accelerationTimeAirborne);
        pMD.pI.velocity.y += pMD.pI.gravity * Time.deltaTime;
    }

    //public override void OnJumpInputUp()
    //{
    //    playerInfo.isJumpingApex = true;
    //    if (playerInfo.velocity.y > playerInfo.minJumpVelocity)
    //    {
    //        playerInfo.velocity.y = playerInfo.minJumpVelocity;
    //        //GrabRelease();
    //    }
    //}

    void HandleWallSliding() {
        pMD.pI.wallDirX = (pMD.pI.playerController.collisions.left) ? -1 : 1;
        pMD.pI.wallSliding = false;
        if ((pMD.pI.playerController.collisions.left || pMD.pI.playerController.collisions.right) && !pMD.pI.playerController.collisions.below && pMD.pI.velocity.y < 0) {
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

    public void GrabRelease() {
        pMD.pI.ledgeGrabbing = false;
        //transform.parent = null;
        //armControllerScript.enabled = true;
        //playerScript.enabled = true;//-------------------------------
        //playerScript.wristScript = null;
        //playerControllerScript.WristInstance = null;
        pMD.pI.playerController.collisions.ResetGrab();
        //playerInfo.playerController.collisions.moveAmountOld = Vector2.zero;

        pMD.pI.player.enabled = true;
        pMD.pI.velocity.x = 0f;
        pMD.pI.playerController._collider = pMD.pI.player.GetComponent<BoxCollider2D>();/*GetComponent<CapsuleCollider2D>();*/
        pMD.pI.playerController.CalculateRaySpacing();
        //gravity = 0;
        //playerInfo.playerController.transform.
        //pMD.pI.wrist.enabled = false;
        pMD.pI.wrist.gameObject.SetActive(false);
        //DestroyImmediate(gameObject);
        //player.GetComponent<PlayerInput>().wristScript = null;
        //Destroy(gameObject, 1f);
    }
}
