using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;
    public LayerMask edgeMask;

    public const float skinWidth = .015f;
    const float dstBetweenRays = 0.25f;

    protected float armLength = 0.75f;

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
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake() {
        collider = GetComponent<BoxCollider2D>();
    }
    public virtual void Start() {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.centerLeft = new Vector2(bounds.min.x, bounds.center.y);
        raycastOrigins.centerRight = new Vector2(bounds.max.x, bounds.center.y);
        raycastOrigins.topCenter = new Vector2(bounds.center.x, bounds.max.y);
        raycastOrigins.bottomCenter = new Vector2(bounds.center.x, bounds.min.y);
    }

    public void CalculateRaySpacing() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);
        armLengthRayCount = Mathf.RoundToInt(armLength / dstBetweenRays);

        horizontalRaySpacing = boundsHeight / (horizontalRayCount - 1);
        verticalRaySpacing = boundsWidth / (verticalRayCount - 1);
        armLengthRaySpacing = armLength / (armLengthRayCount - 1);
    }

    public Vector2 CalculateGrabOrigins(Vector2 input) {
        if (input.x == -1f && input.y == -1f) { return raycastOrigins.bottomLeft; }
        else if (input.x == -1f && input.y == 0f) { return raycastOrigins.centerLeft; }
        else if (input.x == -1f && input.y == 1f) { return raycastOrigins.topLeft; }
        else if (input.x == 0f && input.y == 1f) { return raycastOrigins.topCenter; }
        else if (input.x == 1f && input.y == 1f) { return raycastOrigins.topRight; }
        else if (input.x == 1f && input.y == 0f) { return raycastOrigins.centerRight; }
        else if (input.x == 1f && input.y == -1f) { return raycastOrigins.bottomRight; }
        else if (input.x == 0f && input.y == -1f) { return raycastOrigins.bottomCenter; }
        return Vector2.zero;
    }

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Vector2 centerLeft, centerRight;
        public Vector2 topCenter, bottomCenter;
    }
}
