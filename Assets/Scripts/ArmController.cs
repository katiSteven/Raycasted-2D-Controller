using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : RaycastController
{
    Player.PlayerInfo playerInfo;
    public override void Awake() {
        base.Awake();
        playerInfo = GetComponentInParent<Player>().playerInfo;
    }
    public override void Start() {
        base.Start();
    }

    void Update() {
        UpdateRaycastOrigins();
    }

    public RaycastHit2D GrappleCollisions(Vector2 moveAmount, Vector2 input)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = playerInfo.playerController.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input);
        for (int i = 0; i < armLengthRayCount; i++) {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY), playerManager.WristLength, collisionMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY) * playerManager.WristLength, Color.red);

            if (hit && hit.transform.tag == "PassablePlatform" && hit.distance > 0.05f) {
                float slopeAngle = Vector2.Angle(Vector2.down, hit.normal);
                //print("hit normal: " + hit.normal + " hit.distance: " + hit.distance + " Grab Angle: " + slopeAngle);
                if (slopeAngle <= playerInfo.playerController.maxGrabAngle && hit.distance > 0f) {
                    GrabCollisionDetection(rayCastOrigins);
                    //playerInfo.velocity = Vector2.zero;
                    return hit;
                }
            }
        }
        return new RaycastHit2D();
    }

    public RaycastHit2D ObjectCollision(Vector2 moveAmount, Vector2 input) {
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = playerInfo.playerController.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input);
        for (int i = 0; i < armLengthRayCount; i++) {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY), playerManager.WristLength, objectMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY) * playerManager.WristLength, Color.blue);

            if (hit && !playerInfo.playerController.collisions.holdingObject) {
                PickUpCollisionDetection(rayCastOrigins);
                playerInfo.playerController.collisions.holdingObject = true;
                playerInfo.playerController.collisions.objectHit = hit;
                return hit;
            }
        }
        return new RaycastHit2D();
    }

    public void GrabCollisionDetection(Vector2 rayOrigin) {
        if (rayOrigin == raycastOrigins.bottomLeft) { playerInfo.playerController.collisions.bottomLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { playerInfo.playerController.collisions.leftGrab = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { playerInfo.playerController.collisions.topLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { playerInfo.playerController.collisions.topGrab = true; }
        else if (rayOrigin == raycastOrigins.topRight) { playerInfo.playerController.collisions.topRightGrab = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { playerInfo.playerController.collisions.rightGrab = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { playerInfo.playerController.collisions.bottomRightGrab = true; }
        else if (rayOrigin == raycastOrigins.center) { playerInfo.playerController.collisions.bottomGrab = true; }
    }

    public void PickUpCollisionDetection(Vector2 rayOrigin) {
        if (rayOrigin == raycastOrigins.bottomLeft) { playerInfo.playerController.collisions.bottomLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { playerInfo.playerController.collisions.leftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { playerInfo.playerController.collisions.topLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { playerInfo.playerController.collisions.topPickUp = true; }
        else if (rayOrigin == raycastOrigins.topRight) { playerInfo.playerController.collisions.topRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { playerInfo.playerController.collisions.rightPickUp = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { playerInfo.playerController.collisions.bottomRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.center) { playerInfo.playerController.collisions.bottomPickUp = true; }
    }

    public void EquipmentPickup(Vector2 holdingPosition) {
        if (playerInfo.playerController.collisions.objectHit) {
            playerInfo.playerController.collisions.objectHit.transform.position = (Vector3)holdingPosition + Vector3.back;
        }
    }
}
