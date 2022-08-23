using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLimbs : MonoBehaviour
{
    [SerializeField] public float radius;
    [SerializeField] public LayerMask collisionMask;

    Animator parentAnimator;
    BoxCollider2D boxCollider2D;
    protected PlayerMetaData pMD;
    //LimbsRaycastOrigins limbsRaycastOrigins;

    private Transform leftLeg, RightLeg;
    private int quadrant = -1;

    private void Awake()
    {
        if (PlayerManager.InstanceID_Index_to_PlayerMetaData.ContainsKey(transform.parent.GetInstanceID()) &&
            PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()].pI.InstanceID == transform.parent.GetInstanceID()) {
            pMD = PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()];
            print(gameObject.name + " got pMD, component: " + name);
        } else {
            print(gameObject.name + " not present in pMD, component: " + name);
            enabled = false;
        }
        boxCollider2D = GetComponent<BoxCollider2D>();
        float side = (Mathf.Sqrt(2) * radius * radius) / 2;
        boxCollider2D.size = new Vector2(side, side);
        parentAnimator = GetComponentInParent<Animator>();
    }

    private void Start() {
        leftLeg = transform.GetChild(0);
        RightLeg = transform.GetChild(1);
        parentAnimator.SetBool("WalkTrue_Bool", true);
    }

    public void Move(Vector2 moveAmount, Vector2 input) {
        Debug.Log("Mathf.Abs(moveAmount.x): " + Mathf.Abs(moveAmount.x));
        Debug.Log("input: " + input);
        if (Mathf.Abs(input.x) < 1f) {
            parentAnimator.ResetTrigger("Walk_Trigger");
            parentAnimator.SetBool("WalkTrue_Bool", false);
            quadrant = -1;
        } else {
            parentAnimator.SetBool("WalkTrue_Bool", true); 
        }
        LimbsAnimationCollisions();
        transform.Rotate(-2f * moveAmount.x * pMD.pI.playerManager.moveSpeed * Vector3.forward);
    }

    void LimbsAnimationCollisions() {
        //RaycastHit2D Righthit = Physics2D.Raycast(RightLeg.transform.localPosition, Vector2.down, 1f, collisionMask);
        //RaycastHit2D Lefthit = Physics2D.Raycast(leftLeg.transform.localPosition, Vector2.down, 1f, collisionMask);

        //Debug.DrawRay(RightLeg.transform.localPosition, Vector2.Reflect(Vector2.down, transform.localEulerAngles) * 1f, Color.magenta, 1f);
        //Debug.DrawRay(leftLeg.transform.localPosition, Vector2.Reflect(Vector2.down, transform.localEulerAngles) * 1f, Color.magenta, 1f);

        int wholeDegree = (int)gameObject.transform.localEulerAngles.z;
        int newQuadrant = wholeDegree / (360/8);

        if (newQuadrant != quadrant) {
            quadrant = newQuadrant;
            parentAnimator.SetTrigger("Walk_Trigger");
            //Debug.Log("Quadrant: " + quadrant);
        }
    }

    public void UpdateRaycastOrigins() {
        Bounds bounds = boxCollider2D.bounds;

        //limbsRaycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        //limbsRaycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        //limbsRaycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        //limbsRaycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        //limbsRaycastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
        //limbsRaycastOrigins.right = new Vector2(bounds.max.x, bounds.center.y);
        //limbsRaycastOrigins.left = new Vector2(bounds.min.x, bounds.center.y);
        //limbsRaycastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);

        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.min.x, bounds.max.y));     //topLeft
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.max.x, bounds.max.y));    //topRight
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.min.x, bounds.min.y));    //bottomLeft
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.max.x, bounds.min.y));    //bottomRight
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.center.x, bounds.max.y)); //top
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.max.x, bounds.center.y)); //right
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.min.x, bounds.center.y)); //left
        //limbsRaycastOrigins.limbsAnimationTriggers.Add(new Vector2(bounds.center.x, bounds.min.y)); //bottom
    }

    public struct LimbsRaycastOrigins {
        //public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        //public Vector2 top, right, left, bottom;
        public List<Vector2> limbsAnimationTriggers;


    }
}
