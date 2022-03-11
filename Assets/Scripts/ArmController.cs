using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : RaycastController
{
    //Player.PlayerInfo playerInfo;
    //PlayerManager playerManager;
    //PlayerManager.PlayerInfo playerInfo;
    PlayerMetaData pMD;
    public override void Awake() {
        base.Awake();
        if (PlayerManager.InstanceID_Index_to_PlayerMetaData.ContainsKey(transform.parent.GetInstanceID()) &&
            PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()].pI.InstanceID == transform.parent.GetInstanceID())
        {
            pMD = PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()];
            print(gameObject.name + " got pMD, component: " + name);
        }
        else
        {
            print(gameObject.name + " could not find pMD, component: " + name);
            enabled = false;
        }
        //playerInfo = PlayerManager.GetPlayerInfo((transform.parent.tag == "Player") ? transform.parent.GetInstanceID() : transform.GetInstanceID());
        //if (playerInfo.InstanceID == -1) {
        //    print(gameObject.name + "|" + this.name + "|" + GetInstanceID() + "| Could not fetch valid playerInfo");
        //}
    }
    public override void Start() {
        base.Start();
        //playerInfo = PlayerManager.GetPlayerInfo((transform.parent.tag == "Player") ? transform.parent.GetInstanceID() : transform.GetInstanceID()/*, out playerInfo*/);
        
        //if (pMD.pI.InstanceID == -1/*playerManager != null*/)
        //{
        //    //playerInfo = playerManager.playerInfo;
        //    print("Player Instance ID from From arm controller " + playerInfo.InstanceID);
        //}
        //else
        //{
        //    print(gameObject.name + "|" + this.name + "|" + GetInstanceID() + "| Could not fetch valid playerInfo");
        //}
    }

    void Update() {
        UpdateRaycastOrigins();
    }

    public RaycastHit2D GrappleCollisions(Vector2 moveAmount, Vector2 input)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = pMD.pI.playerController.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input);
        for (int i = 0; i < armLengthRayCount; i++) {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY), playerManager.WristLength, collisionMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY) * playerManager.WristLength, Color.red);

            if (hit && hit.transform.tag == "PassablePlatform" && hit.distance > 0.05f) {
                float slopeAngle = Vector2.Angle(Vector2.down, hit.normal);
                //print("hit normal: " + hit.normal + " hit.distance: " + hit.distance + " Grab Angle: " + slopeAngle);
                if (slopeAngle <= pMD.pI.playerController.maxGrabAngle && hit.distance > 0f) {
                    GrabCollisionDetection(rayCastOrigins);
                    //pMD.pI.velocity = Vector2.zero;
                    return hit;
                }
            }
        }
        return new RaycastHit2D();
    }

    public RaycastHit2D ObjectCollision(Vector2 moveAmount, Vector2 input) {
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = pMD.pI.playerController.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input);
        for (int i = 0; i < armLengthRayCount; i++) {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i + + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY), playerManager.WristLength, objectMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY) * playerManager.WristLength, Color.blue);

            if (hit && !pMD.pI.playerController.collisions.holdingObject) {
                PickUpCollisionDetection(rayCastOrigins);
                pMD.pI.playerController.collisions.holdingObject = true;
                pMD.pI.playerController.collisions.objectHit = hit;
                return hit;
            }
        }
        return new RaycastHit2D();
    }

    public void GrabCollisionDetection(Vector2 rayOrigin) {
        if (rayOrigin == raycastOrigins.bottomLeft) { pMD.pI.playerController.collisions.bottomLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { pMD.pI.playerController.collisions.leftGrab = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { pMD.pI.playerController.collisions.topLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { pMD.pI.playerController.collisions.topGrab = true; }
        else if (rayOrigin == raycastOrigins.topRight) { pMD.pI.playerController.collisions.topRightGrab = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { pMD.pI.playerController.collisions.rightGrab = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { pMD.pI.playerController.collisions.bottomRightGrab = true; }
        else if (rayOrigin == raycastOrigins.center) { pMD.pI.playerController.collisions.bottomGrab = true; }
    }

    public void PickUpCollisionDetection(Vector2 rayOrigin) {
        if (rayOrigin == raycastOrigins.bottomLeft) { pMD.pI.playerController.collisions.bottomLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { pMD.pI.playerController.collisions.leftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { pMD.pI.playerController.collisions.topLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { pMD.pI.playerController.collisions.topPickUp = true; }
        else if (rayOrigin == raycastOrigins.topRight) { pMD.pI.playerController.collisions.topRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { pMD.pI.playerController.collisions.rightPickUp = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { pMD.pI.playerController.collisions.bottomRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.center) { pMD.pI.playerController.collisions.bottomPickUp = true; }
    }

    public void EquipmentPickup(Vector2 holdingPosition) {
        if (pMD.pI.playerController.collisions.objectHit) {
            pMD.pI.playerController.collisions.objectHit.transform.position = (Vector3)holdingPosition + Vector3.back;
        }
    }
}
