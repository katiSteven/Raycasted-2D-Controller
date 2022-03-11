using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetaData : MonoBehaviour
{

    public PlayerInfo pI;

    private void Awake() {
        PlayerManager.InstanceID_Index_to_PlayerMetaData.Add(transform.GetInstanceID(), this);
        pI = new PlayerInfo(transform.GetInstanceID());
    }

    public struct PlayerInfo
    {
        public int InstanceID;
        public float timeToWallUnstick;

        public float gravity;
        public float maxJumpVelocity;
        public float minJumpVelocity;
        public Vector3 velocity;
        public float velocityXSmoothing;
        public Vector2 directionalInput;
        public Vector2 grabDirectionalInput;

        public PlayerManager playerManager;
        public Player player;
        public Controller2D playerController;

        public Arm arm;
        public Wrist wrist;

        public bool wallSliding;
        public int wallDirX;
        public bool isJumpingApex;
        public bool ledgeGrabbing;

        //PlayerInfo() { }
        public PlayerInfo(int instanceID)
        {
            InstanceID = instanceID;

            timeToWallUnstick = 0;

            gravity = 0;
            maxJumpVelocity = 0;
            minJumpVelocity = 0;
            velocity = Vector3.zero;
            velocityXSmoothing = 0;
            directionalInput = Vector2.zero;
            grabDirectionalInput = Vector2.zero;

            playerManager = null;
            player = null;
            playerController = null;

            arm = null;
            wrist = null;

            wallSliding = false;
            wallDirX = 0;
            isJumpingApex = false;
            ledgeGrabbing = false;


        }
    }
}
