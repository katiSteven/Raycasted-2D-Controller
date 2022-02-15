using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : RaycastController
{
    //public float ArmLength { get; set; }
    //public float WristLength { get; set; }
    //[SerializeField] public GameObject wristPrefab;


    //public GameObject WristInstance;

    //public new CollisionInfo collisions;
    //Controller2D controller2D;
    //Player player;
    Player.Info playerInfo;
    public override void Awake()
    {
        base.Awake();
        //player = GetComponentInParent<Player>();
        playerInfo = GetComponentInParent<Player>().playerInfo;
    }
    public override void Start()
    {
        //armLength = ArmLength;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
    }

    public RaycastHit2D GrappleCollisions(Vector2 moveAmount, Vector2 input)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = playerInfo.controller.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input);
        for (int i = 0; i < armLengthRayCount; i++)
        {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY), playerManager.WristLength, collisionMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.up * directionY) * playerManager.WristLength, Color.red);

            if (hit)
            {
                //float slopeAngle = Vector2.Angle(((input.x == 0f) ? Vector2.right * directionX : Vector2.up), hit.normal);
                //float slopeAngle1 = Vector2.Angle(((input.x == 0f) ? Vector2.right * directionX : Vector2.down), hit.normal);
                //float slopeAngle2 = Vector2.Angle(Vector2.right * directionX, hit.normal);
                float slopeAngle = Vector2.Angle(Vector2.down, hit.normal);
                //float slopeAngle3 = Vector2.Angle(hit.collider.Raycast(hit.normal * -1,), hit.normal);
                //if()
                print("hit normal: " + hit.normal + " hit.distance: " + hit.distance + " Grab Angle: " + slopeAngle);
                if (slopeAngle <= playerInfo.controller.maxGrabAngle && hit.distance > 0f)
                {
                    GrabCollisionDetection(rayCastOrigins);
                    //LedgeCollisions(hit);
                    return hit;
                    //break;
                }
            }
        }
        return new RaycastHit2D();
    }

    public void ObjectCollision(Vector2 moveAmount, Vector2 input)
    {
        //if (gameObject.tag == "Player")
        //{
            
        //}
        float directionY = Mathf.Sign(moveAmount.y);
        float directionX = playerInfo.controller.collisions.faceDir;

        Vector2 rayCastOrigins = CalculateFinalWristPlacement(input); ;
        for (int i = 0; i < armLengthRayCount; i++)
        {
            Vector2 rayOrigin = rayCastOrigins;
            rayOrigin += input * (armLengthRaySpacing * i);
            RaycastHit2D hitObject = Physics2D.Raycast(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY), playerManager.WristLength, objectMask);

            Debug.DrawRay(rayOrigin, ((input.x == 0f) ? new Vector2(directionX, directionY) : Vector2.down * directionY) * playerManager.WristLength, Color.blue);

            if (hitObject && !playerInfo.controller.collisions.holdingObject)
            {
                PickUpCollisionDetection(rayCastOrigins);
                playerInfo.controller.collisions.holdingObject = true;
                playerInfo.controller.collisions.objectHit = hitObject;
                break;
            }
        }
    }

    

    public void GrabCollisionDetection(Vector2 rayOrigin)
    {
        if (rayOrigin == raycastOrigins.bottomLeft) { playerInfo.controller.collisions.bottomLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { playerInfo.controller.collisions.leftGrab = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { playerInfo.controller.collisions.topLeftGrab = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { playerInfo.controller.collisions.topGrab = true; }
        else if (rayOrigin == raycastOrigins.topRight) { playerInfo.controller.collisions.topRightGrab = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { playerInfo.controller.collisions.rightGrab = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { playerInfo.controller.collisions.bottomRightGrab = true; }
        else if (rayOrigin == raycastOrigins.center) { playerInfo.controller.collisions.bottomGrab = true; }
    }

    public void PickUpCollisionDetection(Vector2 rayOrigin)
    {
        if (rayOrigin == raycastOrigins.bottomLeft) { playerInfo.controller.collisions.bottomLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.leftEdge) { playerInfo.controller.collisions.leftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topLeft) { playerInfo.controller.collisions.topLeftPickUp = true; }
        else if (rayOrigin == raycastOrigins.topEdge) { playerInfo.controller.collisions.topPickUp = true; }
        else if (rayOrigin == raycastOrigins.topRight) { playerInfo.controller.collisions.topRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.rightEdge) { playerInfo.controller.collisions.rightPickUp = true; }
        else if (rayOrigin == raycastOrigins.bottomRight) { playerInfo.controller.collisions.bottomRightPickUp = true; }
        else if (rayOrigin == raycastOrigins.center) { playerInfo.controller.collisions.bottomPickUp = true; }
    }

    public void EquipmentPickup(Vector2 holdingPosition)
    {
        playerInfo.controller.collisions.objectHit.transform.position = (Vector3)holdingPosition + Vector3.back;
    }

    //public new struct CollisionInfo
    //{
        
    //}
}
