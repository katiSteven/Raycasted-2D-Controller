using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController {

    public float maxSlopeAngle = 80;
    public float maxGrabAngle = 30;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;
    

    public override void Start() {
        base.Start();
        collisions.faceDir = 1;
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform) {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.y < 0) {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0) {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);
        
        if (moveAmount.y != 0) {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform) {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount) {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit) {

                if (hit.distance == 0) {
                    continue;
                }
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle) {
                    if (collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;
                    if (hit.collider.tag == "PassablePlatform" && tag == "Player") {
                        if (playerInput.y == -1 && !collisions.Grabbing()) {
                            moveAmount.y = (moveAmount.y + skinWidth) * -2;
                        }
                    }
                }
                if (hit.collider.tag == "PassablePlatform" && tag == "Player") { continue; }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    if (collisions.climbingSlope) {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
                if (tag == "Equipment")
                {
                    moveAmount += Reflect(hit.point, hit.normal);
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveAmount.y <= climbmoveAmountY) {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveAmount) {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!collisions.slidingDownMaxSlope) {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
                    if (Mathf.Sign(hit.normal.x) == directionX) {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) {
        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle) {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount) {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        collisions.raycastHits = new RaycastHit2D[verticalRayCount];
        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
            collisions.raycastHits[i] = hit;
            if (hit) {
                collisions.floorHit = hit;
                if (hit.collider.tag == "PassablePlatform" && tag == "Player") {
                    if (collisions.fallingThroughPlatform) { continue; }
                    if (playerInput.y == -1 && !collisions.Grabbing()) {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.2f);
                        continue;
                    }
                    if (directionY == 1 || hit.distance == 0) { continue; }
                }
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
                if (tag == "Equipment") {
                    moveAmount += Reflect(hit.point, hit.normal);
                }
            }
        }
        if (collisions.climbingSlope) {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle) {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
        if (tag == "Player")
        {
            WristLedgeCollisions(collisions.raycastHits, ref moveAmount);
        }
    }

    public static Vector2 Reflect(Vector2 v, Vector2 normal)
    {
        var dp = 2 * Vector2.Dot(v, normal);
        return new Vector2(v.x - normal.x * dp, v.y - normal.y * dp);
    }

    private void WristLedgeCollisions(RaycastHit2D[] raycastHits, ref Vector2 moveAmount)
    {
        if (raycastHits[verticalRayCount - 1].collider != null)
        {
            if (raycastHits[verticalRayCount - 1].transform.GetComponent<PlatformController>() || raycastHits[verticalRayCount - 1].transform.tag == "PassablePlatform")
            {
                Bounds platformBounds = raycastHits[verticalRayCount - 1].collider.bounds;
                collisions.otherColliderLeftVertex = new Vector2(platformBounds.min.x, platformBounds.min.y);
                collisions.otherColliderRightVertex = new Vector2(platformBounds.max.x, platformBounds.max.y);
            }
        }
        if (raycastHits[0].collider != null)
        {
            if (raycastHits[0].transform.GetComponent<PlatformController>() || raycastHits[0].transform.tag == "PassablePlatform")
            {
                Bounds platformBounds = raycastHits[0].collider.bounds;
                collisions.otherColliderLeftVertex = new Vector2(platformBounds.min.x, platformBounds.max.y);
                collisions.otherColliderRightVertex = new Vector2(platformBounds.max.x, platformBounds.min.y);
            }
        }
        if(raycastHits[verticalRayCount - 1].collider == null && raycastHits[0].collider == null && raycastHits[verticalRayCount / 2].collider == null) {
            collisions.otherColliderLeftVertex = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            collisions.otherColliderRightVertex = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            Vector2 rayOrigin = raycastOrigins.bottomEdge /*raycastHits[verticalRayCount / 2].*/;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, transform.localScale.y * 4, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.down * transform.localScale.y * 4, Color.yellow);
            if (hit) {
                if (hit.collider.tag != "PassablePlatform" && Mathf.Abs(hit.distance) <= transform.localScale.y * 2) {
                    collisions.longWayDown = true;
                }
                else { collisions.longWayDown = false; }
                //moveAmount.y = 0;
            }
            //else { collisions.longWayDown = false; }
        }
    }

    //public void WristLedgeCollisions()
    //{

    //}

    void ResetFallingThroughPlatform() {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope;
        public bool slidingDownMaxSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;

        public RaycastHit2D floorHit;
        public RaycastHit2D[] raycastHits;
        public bool fallingThroughPlatform;
        public bool longWayDown;
        //public bool Edge;
        public Vector2 otherColliderLeftVertex;
        public Vector2 otherColliderRightVertex;

        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
        public bool leftGrab, rightGrab, topGrab, bottomGrab;
        public bool topLeftGrab, topRightGrab, bottomLeftGrab, bottomRightGrab;

        public bool leftPickUp, rightPickUp, topPickUp, bottomPickUp;
        public bool topLeftPickUp, topRightPickUp, bottomLeftPickUp, bottomRightPickUp;

        public bool holdingObject;
        public RaycastHit2D objectHit;

        public bool Grabbing()
        {
            if (leftGrab || rightGrab || topGrab || bottomGrab ||
                topLeftGrab || topRightGrab || bottomLeftGrab || bottomRightGrab)
            {
                return true;
            }
            return false;
        }

        public bool PickingUp()
        {
            if (leftPickUp || rightPickUp || topPickUp || bottomPickUp ||
                topLeftPickUp || topRightPickUp || bottomLeftPickUp || bottomRightPickUp)
            {
                return true;
            }
            return false;
        }
        public void ResetGrab()
        {
            leftGrab = rightGrab = topGrab = bottomGrab = false;
            topLeftGrab = topRightGrab = bottomLeftGrab = bottomRightGrab = false;
        }

        public void ResetPickUp()
        {
            leftPickUp = rightPickUp = topPickUp = bottomPickUp = false;
            topLeftPickUp = topRightPickUp = bottomLeftPickUp = bottomRightPickUp = false;
        }

        public void DropObjectHeld()
        {
            holdingObject = false;
        }
    }
}
