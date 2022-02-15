using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArmController))]
public class Arm : MonoBehaviour
{
    //public Player player;
    Player.Info playerInfo;
    ArmController armController;
    //PlayerManager playerManager;
    //BoxCollider2D wristBoxCollider2D;
    //public Wrist wristScript { get; set; }
    public Equipment ObjectScript { get; set; }

    /*protected override */void Awake() {
        //player = GetComponentInParent<Player>();
        playerInfo = GetComponentInParent<Player>().playerInfo;
        //base. = GetComponentInParent<Player>();
        //playerManager = FindObjectOfType<PlayerManager>();
        armController = GetComponent<ArmController>();
        //armController.ArmLength = playerManager.ArmLength;
        //armController.WristLength = playerManager.WristLength;
    }

    private void OnEnable()
    {
        PlayerManager.OnGrabDirectionalInput += GrabInputdown;
        PlayerManager.OnGrabDirectionalInput += PickUpInputDown;
        PlayerManager.OnGrabRelease += ReleaseObjectHeld;
    }

    private void OnDisable()
    {
        PlayerManager.OnGrabDirectionalInput -= GrabInputdown;
        PlayerManager.OnGrabDirectionalInput -= PickUpInputDown;
        PlayerManager.OnGrabRelease -= ReleaseObjectHeld;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (playerInfo.controller.collisions.Grabbing())
        {
            //if (wristScript != null) { ledgeGrabbing = true; wristScript.DisablePlayerMovement(); }
            playerInfo.ledgeGrabbing = true;
            //DisablePlayerMovement();
        }
        PickupObject();
    }

    public void GrabInputdown(Vector2 grabDirectionalInput) {
        if (playerInfo.wrist == null) {
            RaycastHit2D contactPoint = armController.GrappleCollisions(playerInfo.velocity * Time.deltaTime, grabDirectionalInput);
            if (contactPoint.collider != null) { LedgeCollisions(contactPoint); }
        }
    }

    //public bool CheckLedgeGrabbingActive()
    //{
    //    if (wristScript != null)
    //    {
    //        if (wristScript.ledgeGrabbing) { return true; }
    //    }
    //    return false;
    //}

    public void LedgeCollisions(RaycastHit2D contactPoint)
    {
        //if (gameObject.tag == "Player")
        //{

        //}
        //&& !collisions.Grabbing()
        //if (WristInstance == null)
        //{

        //}
        GameObject wristObject = Instantiate(playerInfo.playerManager.wristPrefab,transform);

        BoxCollider2D wristBoxCollider2D = wristObject.GetComponent<BoxCollider2D>();
        //wristObject.transform.position = transform.InverseTransformPoint(contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(0f, wristBoxCollider2D.bounds.center.y));
        //wristObject.transform.position = new Vector2(0f, wristBoxCollider2D.bounds.center.y);
        playerInfo.wrist = wristObject.GetComponent<Wrist>();
        //wrist
    }

    public void PickUpInputDown(Vector2 grabDirectionalInput)
    {
        armController.ObjectCollision(playerInfo.velocity * Time.deltaTime, grabDirectionalInput);
    }

    void PickupObject()
    {
        //^ controller.collisions.Grabbing()
        if (playerInfo.controller.collisions.holdingObject && playerInfo.controller.collisions.PickingUp())
        {
            if (playerInfo.controller.collisions.bottomLeftPickUp || playerInfo.controller.collisions.leftPickUp) { armController.EquipmentPickup(armController.raycastOrigins.leftEdge); }
            else if (playerInfo.controller.collisions.rightPickUp || playerInfo.controller.collisions.bottomRightPickUp) { armController.EquipmentPickup(armController.raycastOrigins.rightEdge); }
            else if (playerInfo.controller.collisions.topLeftPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topLeft); }
            else if (playerInfo.controller.collisions.topPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topEdge); }
            else if (playerInfo.controller.collisions.topRightPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topRight); }
            else if (playerInfo.controller.collisions.bottomPickUp) { armController.EquipmentPickup(armController.raycastOrigins.center); }

            if (ObjectScript == null)
            {
                ObjectScript = playerInfo.controller.collisions.objectHit.transform.gameObject.GetComponent<Equipment>();
                //ObjectScript.controller.collisions.Reset();
            }
        }
    }
    public void ReleaseObjectHeld()
    {
        if (playerInfo.controller.collisions.holdingObject)
        { // && controller.collisions.Grabbing()
            playerInfo.controller.collisions.DropObjectHeld();
            //controller.collisions.objectHit = new RaycastHit2D();
        }
    }
}
