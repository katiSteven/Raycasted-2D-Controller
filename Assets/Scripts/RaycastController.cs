using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;
    public LayerMask objectMask;

    public const float skinWidth = .015f;
    const float dstBetweenRays = 0.20f;

    protected float armLength; //0.75f;//get it directly from Player Manager

    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;
    [HideInInspector]
    public int armLengthRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;
    [HideInInspector]
    public float armLengthRaySpacing;

    [HideInInspector]
    public Collider2D _collider;
    public RaycastOrigins raycastOrigins;

    protected PlayerManager playerManager;

    Vector2[] wristPlacementList;

    public virtual void Awake() {
        //_collider = (GetComponent<EdgeCollider2D>())? GetComponent<EdgeCollider2D>() : GetComponent<BoxCollider2D>();
        if (GetComponent<EdgeCollider2D>()) { _collider = GetComponent<EdgeCollider2D>(); }
        else if (GetComponent<BoxCollider2D>()) { _collider = GetComponent<BoxCollider2D>(); }
        //else { _collider = GetComponent<CapsuleCollider2D>(); }
        playerManager = FindObjectOfType<PlayerManager>();
        armLength = playerManager.ArmLength;

        
    }
    public virtual void Start() {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins() {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.leftEdge = new Vector2(bounds.min.x, bounds.center.y);
        raycastOrigins.rightEdge = new Vector2(bounds.max.x, bounds.center.y);
        raycastOrigins.topEdge = new Vector2(bounds.center.x, bounds.max.y);
        raycastOrigins.center = new Vector2(bounds.center.x, bounds.center.y);
        raycastOrigins.bottomEdge = new Vector2(bounds.center.x, bounds.min.y);
    }

    public void CalculateRaySpacing() {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);
        armLengthRayCount = Mathf.RoundToInt(armLength / dstBetweenRays);
        //add logic to display error in console or popup in inspector when ray count is less than 3, for all the 3 fields above
        //at least 3 rays should be present. 2 positioned at the collider vertices & 1 in the middle. for proper collision detection.

        horizontalRaySpacing = boundsHeight / (horizontalRayCount - 1);
        verticalRaySpacing = boundsWidth / (verticalRayCount - 1);
        armLengthRaySpacing = armLength / (armLengthRayCount - 1);
    }

    public Vector2 OriginPlacement(int index) {
        //if (index==1) { return new Vector2(-1f,-1f); }
        //else 
        //if (index==0) { return new Vector2(-1f, 0f); }
        //else 
        if (index==1) { return new Vector2(-1f, 1f); }
        else if (index==2) { return new Vector2(0f, 1f); }
        else if (index==3) { return new Vector2(1f, 1f); }
        //else if (index==4) { return new Vector2(1f, 0f); }
        //else if (index==7) { return new Vector2(1f, -1f); }
        //else if (index==8) { return new Vector2(0f, -1f); }
        return Vector2.zero;
    }

    public Vector2 CalculateFinalWristPlacement(Vector2 input) {

        //if (input.x == -1f && input.y == -1f) { return raycastOrigins.leftEdge; }
        //else 
        //if (input.x == -1f && input.y == 0f) { return raycastOrigins.leftEdge; }
        //else 
        if (input.x == -1f && input.y == 1f) { return raycastOrigins.topLeft; }
        else if (input.x == 0f && input.y == 1f) { return raycastOrigins.topEdge; }
        else if (input.x == 1f && input.y == 1f) { return raycastOrigins.topRight; }
        //else if (input.x == 1f && input.y == 0f) { return raycastOrigins.rightEdge; }
        //else if (input.x == 1f && input.y == -1f) { return raycastOrigins.rightEdge; }
        //else if (input.x == 0f && input.y == -1f) { return raycastOrigins.center; }
        return Vector2.zero;
    }

    public Vector2 CalculateFinalObjectPlacement(Vector2 input)
    {
        if (input.x == -1f && input.y == -1f) { return raycastOrigins.bottomLeft; }
        else if (input.x == -1f && input.y == 0f) { return raycastOrigins.leftEdge; }
        else if (input.x == -1f && input.y == 1f) { return raycastOrigins.topLeft; }
        else if (input.x == 0f && input.y == 1f) { return raycastOrigins.topEdge; }
        else if (input.x == 1f && input.y == 1f) { return raycastOrigins.topRight; }
        else if (input.x == 1f && input.y == 0f) { return raycastOrigins.rightEdge; }
        else if (input.x == 1f && input.y == -1f) { return raycastOrigins.bottomRight; }
        else if (input.x == 0f && input.y == -1f) { return raycastOrigins.bottomEdge; }
        else { return raycastOrigins.center; }
    }

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Vector2 leftEdge, rightEdge;
        public Vector2 topEdge, center, bottomEdge;
    }
}
