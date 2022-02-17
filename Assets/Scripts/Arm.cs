using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArmController))]
public class Arm : MonoBehaviour {
    Player.PlayerInfo playerInfo;
    ArmController armController;
    public Equipment ObjectScript { get; set; }

    void Awake() {
        playerInfo = GetComponentInParent<Player>().playerInfo;
        armController = GetComponent<ArmController>();
    }

    private void OnEnable() {
        PlayerManager.OnGrabDirectionalInput += GrabInputdown;
        PlayerManager.OnGrabDirectionalInput += PickUpInputDown;
        PlayerManager.OnGrabDirectionalInput += SetGrabDirectionalInput;
        PlayerManager.OnGrabRelease += ReleaseObjectHeld;
    }

    private void OnDisable() {
        PlayerManager.OnGrabDirectionalInput -= GrabInputdown;
        PlayerManager.OnGrabDirectionalInput -= PickUpInputDown;
        PlayerManager.OnGrabDirectionalInput -= SetGrabDirectionalInput;
        PlayerManager.OnGrabRelease -= ReleaseObjectHeld;
    }

    void Update() {
        if (playerInfo.playerController.collisions.Grabbing()) {
            playerInfo.ledgeGrabbing = true;
        }
        
    }

    public void SetGrabDirectionalInput(Vector2 input) {
        playerInfo.grabDirectionalInput = input;
    }

    private void LateUpdate() {
        PickupObject(playerInfo.grabDirectionalInput);
    }

    public void GrabInputdown(Vector2 grabDirectionalInput) {
        if (playerInfo.wrist == null) {
            RaycastHit2D contactPoint = armController.GrappleCollisions(playerInfo.velocity * Time.deltaTime, grabDirectionalInput);
            if (contactPoint.collider != null) { InstantiateWristCollider(contactPoint); }
        }
    }

    public void InstantiateWristCollider(RaycastHit2D contactPoint) { //Offset-> (0,0.9), Size -> (1,0.5)
        GameObject wristObject = Instantiate(playerInfo.playerManager.wristPrefab, transform);
        //playerInfo.wrist.gameObject.SetActive(true);
        //GameObject wristObject = playerInfo.wrist.gameObject;

        BoxCollider2D wristBoxCollider2D = wristObject.GetComponent<BoxCollider2D>();
        //wristObject.transform.position = wristObject.transform.InverseTransformPoint(contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(0f, wristBoxCollider2D.bounds.center.y));
        //wristObject.transform.position = new Vector2(0f, wristBoxCollider2D.bounds.center.y);
        wristObject.transform.position = contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(0f, wristObject.transform.localScale.y);
        playerInfo.wrist = wristObject.GetComponent<Wrist>();
        GetComponentInParent<Player>().enabled = false;
    }

    public void PickUpInputDown(Vector2 grabDirectionalInput) {
        if (!playerInfo.playerController.collisions.holdingObject) {
            RaycastHit2D contactPoint = armController.ObjectCollision(playerInfo.velocity * Time.deltaTime, grabDirectionalInput);
            if (contactPoint.collider != null) {
                contactPoint.transform.GetComponent<Equipment>().enabled = false;
            }
        }
    }

    void PickupObject(Vector2 grabDirectionalInput)
    {
        armController.EquipmentPickup(armController.CalculateFinalObjectPlacement(grabDirectionalInput));
        //^ controller.collisions.Grabbing()
        //if (grabDirectionalInput != Vector2.zero)
        //{
        //    armController.EquipmentPickup(armController.CalculateFinalObjectPlacement(grabDirectionalInput));
        //}
        //else {
        //    armController.EquipmentPickup(armController.raycastOrigins.center);
        //}

        //else if (playerInfo.playerController.collisions.holdingObject && playerInfo.playerController.collisions.PickingUp())
        //{
        //    armController.EquipmentPickup(armController.raycastOrigins.center);

        //    //if (playerInfo.controller.collisions.bottomLeftPickUp || playerInfo.controller.collisions.leftPickUp) { armController.EquipmentPickup(armController.raycastOrigins.leftEdge); }
        //    //else if (playerInfo.controller.collisions.rightPickUp || playerInfo.controller.collisions.bottomRightPickUp) { armController.EquipmentPickup(armController.raycastOrigins.rightEdge); }
        //    //else if (playerInfo.controller.collisions.topLeftPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topLeft); }
        //    //else if (playerInfo.controller.collisions.topPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topEdge); }
        //    //else if (playerInfo.controller.collisions.topRightPickUp) { armController.EquipmentPickup(armController.raycastOrigins.topRight); }
        //    //else if (playerInfo.controller.collisions.bottomPickUp) { armController.EquipmentPickup(armController.raycastOrigins.center); }


        //}
    }
    public void ReleaseObjectHeld()
    {
        if (playerInfo.playerController.collisions.holdingObject)
        { // && controller.collisions.Grabbing()
            //ObjectScript = null;
            playerInfo.playerController.collisions.DropObjectHeld();
            //controller.collisions.objectHit = new RaycastHit2D();
        }
    }
}
