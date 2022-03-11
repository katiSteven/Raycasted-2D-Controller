using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArmController))]
public class Arm : MonoBehaviour {
    //Player player;
    //PlayerManager playerManager;
    //PlayerManager.PlayerInfo playerInfo;
    ArmController armController;
    //public Equipment ObjectScript { get; set; }
    PlayerMetaData pMD;
    void Awake() {
        if (PlayerManager.InstanceID_Index_to_PlayerMetaData.ContainsKey(transform.parent.GetInstanceID()) &&
            PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()].pI.InstanceID == transform.parent.GetInstanceID()) {
            pMD = PlayerManager.InstanceID_Index_to_PlayerMetaData[transform.parent.GetInstanceID()];
            print(gameObject.name + " got pMD, component: " + name);
        } else {
            print(gameObject.name + " could not find pMD, component: " + name);
            enabled = false;
        }
        //playerInfo = PlayerManager.GetPlayerInfo((transform.parent.tag == "Player") ? transform.parent.GetInstanceID() : transform.GetInstanceID());
        //if (playerInfo.InstanceID == -1) {
        //    print(gameObject.name + "|" + this.name + "|" + GetInstanceID() + "| Could not fetch valid playerInfo");
        //}
        //player = GetComponentInParent<Player>();

        armController = GetComponent<ArmController>();
    }

    private void Start()
    {
        //playerInfo = PlayerManager.GetPlayerInfo((transform.parent.tag == "Player") ? transform.parent.GetInstanceID() : transform.GetInstanceID()/*, out playerInfo*/);

        
        //if (playerInfo.InstanceID == -1/*playerManager != null*/)
        //{
        //    //playerInfo = playerManager.playerInfo;
        //    print("Player Instance ID from From arm " + playerInfo.InstanceID);
        //}
        //else {
        //    print(gameObject.name + "|" + this.name + "|" + GetInstanceID() + "| Could not fetch valid playerInfo");
        //}
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
        if (pMD.pI.playerController.collisions.Grabbing()) {
            pMD.pI.ledgeGrabbing = true;
        }
        //GrabInputdown(new Vector2(-1, 1));
    }

    public void SetGrabDirectionalInput(Vector2 input) {
        pMD.pI.grabDirectionalInput = input;
    }

    private void LateUpdate() {
        PickupObject(pMD.pI.grabDirectionalInput);
    }

    public void GrabInputdown(Vector2 grabDirectionalInput) {
        if (pMD.pI.wrist == null || !pMD.pI.wrist.isActiveAndEnabled) {
            RaycastHit2D contactPoint = armController.GrappleCollisions(pMD.pI.velocity * Time.deltaTime, grabDirectionalInput);
            if (contactPoint.collider != null) {
                InstantiateWristCollider(contactPoint);
            }
        }
    }

    public void InstantiateWristCollider(RaycastHit2D contactPoint) { //Offset-> (0,0.9), Size -> (1,0.5)
        GameObject wristObject;
        if (pMD.pI.wrist != null) {
            wristObject = pMD.pI.wrist.gameObject;
            wristObject.SetActive(true);

        } else {
            wristObject = Instantiate(pMD.pI.playerManager.wristPrefab, transform);
        }
        
        //GameObject wristObject = Instantiate(playerInfo.playerManager.wristPrefab);
        //playerInfo.wrist.gameObject.SetActive(true);
        //GameObject wristObject = playerInfo.wrist.gameObject;

        //BoxCollider2D wristBoxCollider2D = wristObject.GetComponent<BoxCollider2D>();
        //wristObject.transform.position = wristObject.transform.InverseTransformPoint(contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(0f, wristBoxCollider2D.bounds.center.y));
        //wristObject.transform.position = new Vector2(0f, wristBoxCollider2D.bounds.center.y);
        if (Mathf.Sign(pMD.pI.directionalInput.x) != Mathf.Sign(pMD.pI.grabDirectionalInput.x))
        {
            wristObject.transform.position = contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(wristObject.transform.localScale.x, /*wristObject.transform.localScale.y +*/ pMD.pI.playerManager.ArmLength * 0.75f) * pMD.pI.grabDirectionalInput;
        }
        else
        {
            wristObject.transform.position = contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(-wristObject.transform.localScale.x, /*wristObject.transform.localScale.y +*/ pMD.pI.playerManager.ArmLength * 0.75f) * pMD.pI.grabDirectionalInput;
        }
        //wristObject.transform.position = contactPoint.collider.ClosestPoint(contactPoint.point) + new Vector2(wristObject.transform.localScale.x, wristObject.transform.localScale.y + playerInfo.playerManager.ArmLength) * playerInfo.grabDirectionalInput;


        
        GetComponentInParent<Player>().enabled = false;
    }

    public void PickUpInputDown(Vector2 grabDirectionalInput) {
        if (!pMD.pI.playerController.collisions.holdingObject) {
            RaycastHit2D contactPoint = armController.ObjectCollision(pMD.pI.velocity * Time.deltaTime, grabDirectionalInput);
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
        if (pMD.pI.playerController.collisions.holdingObject)
        { // && controller.collisions.Grabbing()
            //ObjectScript = null;
            pMD.pI.playerController.collisions.DropObjectHeld();
            //controller.collisions.objectHit = new RaycastHit2D();
        }
    }
}
