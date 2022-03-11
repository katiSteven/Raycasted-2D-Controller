using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DirectionalInput(Vector2 input);
public delegate void GrabDirectionalInput(Vector2 input);
public delegate void JumpInputDown();
public delegate void JumpInputUp();
public delegate void GrabRelease();
public delegate void ClimbLedge();

public class PlayerManager : MonoBehaviour {

    //----------Attributes----------
    [SerializeField] GameObject playerObject;

    [SerializeField] public float maxJumpHeight = 4; //5
    [SerializeField] public float minJumpHeight = 1; //1
    [SerializeField] public float timeToJumpApex = .4f; //0.5

    [SerializeField] public float accelerationTimeAirborne = .2f; //0.2
    [SerializeField] public float accelerationTimeGrounded = .1f; //0.1
    [SerializeField] public float moveSpeed = 6; //6

    [SerializeField] public Vector2 wallJumpClimb; //7.5 16
    [SerializeField] public Vector2 wallJumpOff; //8.5 7
    [SerializeField] public Vector2 wallLeap; //18 17

    [SerializeField] public float wallSlideSpeedMax = 3; //3
    [SerializeField] public float wallStickTime = 0.25f; //0.25

    [SerializeField] public float ArmLength = 1f; //0.75
    [SerializeField] public GameObject wristPrefab;
    [SerializeField] public float WristLength = 0.1f; //0.1

    //static PlayerInfo playerInfo;
    //static InstanceID_Index_to_PlayerInfos instanceID_Index_to_PlayerInfos/* = new InstanceID_Index_to_PlayerInfos()*/;


    static Dictionary<int, PlayerMetaData> instanceID_Index_to_PlayerMetaData = new Dictionary<int, PlayerMetaData>();

    public static Dictionary<int, PlayerMetaData> InstanceID_Index_to_PlayerMetaData { get => instanceID_Index_to_PlayerMetaData; set => instanceID_Index_to_PlayerMetaData = value; }

    //PlayerManager(int instanceID) {
    //    //instanceID_Index_to_PlayerInfos = new InstanceID_Index_to_PlayerInfos();
    //    //playerInfo = new PlayerInfo(instanceID);
    //}
    //----------Properties----------
    //private static Dictionary<int,PlayerInfo> InstanceID_Index_to_PlayerInfos = new Dictionary<int,PlayerInfo>();
    //private static Dictionary<int, VariableReference> InstanceID_Index_to_PlayerInfos = new Dictionary<int, VariableReference>();

    //----------Events----------
    public static event DirectionalInput OnDirectionalInput;
    public static event GrabDirectionalInput OnGrabDirectionalInput;
    public static event JumpInputDown OnJumpInputDown;
    public static event JumpInputUp OnJumpInputUp;
    public static event GrabRelease OnGrabRelease;
    public static event GrabRelease OnClimbLedge;

    //----------Methods----------
    public static void ExecuteDirectionalUserInput(Vector2 input) { OnDirectionalInput(input); }
    public static void ExecuteGrabDirectionalUserInput(Vector2 input) { OnGrabDirectionalInput(input); }
    public static void ExecuteJumpInputDownUserInput() { OnJumpInputDown(); }
    public static void ExecuteJumpInputUpUserInput() { OnJumpInputUp(); }
    public static void ExecuteGrabRelease() { OnGrabRelease(); }
    public static void ExecuteClimbLedge() {
        if (OnClimbLedge != null) {
            OnClimbLedge();
        }
    }



    //public static PlayerInfo/*void*//*PlayerManager*/ GeneratePlayerInfo(int instanceID/*, out PlayerInfo playerInfo*/) {

    //    //PlayerInfo playerInfo = new PlayerInfo(instanceID);
    //    playerInfo = new PlayerInfo(instanceID);
    //    //PlayerManager pm = new PlayerManager(instanceID);
    //    //pm.playerInfo = new PlayerInfo(instanceID);
    //    //ref PlayerInfo pi => ref * playerInfo;
    //    //InstanceID_Index_to_PlayerInfos.Add(instanceID, pm);
    //    instanceID_Index_to_PlayerInfos.saveVar(instanceID, () => playerInfo, v => { playerInfo = (PlayerInfo)v; });
    //    return playerInfo;
    //    //return pm;
    //}

    //public static PlayerInfo/*void*//*VariableReference*/ GetPlayerInfo(int instanceID/*, out PlayerInfo playerInfo*/) {
    //    //if (InstanceID_Index_to_PlayerInfos.ContainsKey(instanceID))
    //    //{
    //    //    //playerInfo = InstanceID_Index_to_PlayerInfos[instanceID];
    //    //    PlayerManager pm = InstanceID_Index_to_PlayerInfos[instanceID];
    //    //    return pm;
    //    //}
    //    return instanceID_Index_to_PlayerInfos.GetVar(instanceID);
    //    //if (instanceID_Index_to_PlayerInfos.GetVar(instanceID) != null) {
    //    //    return instanceID_Index_to_PlayerInfos.GetVar(instanceID);
    //    //}
        

    //    //return new PlayerInfo(-1);
    //    //return new PlayerInfo(-1);
    //    //return null;
    //    //playerInfo = InstanceID_Index_to_PlayerInfos[instanceID];
    //}

    //
    //public struct PlayerInfo
    //{
    //    public int InstanceID;
    //    public float timeToWallUnstick;

    //    public float gravity;
    //    public float maxJumpVelocity;
    //    public float minJumpVelocity;
    //    public Vector3 velocity;
    //    public float velocityXSmoothing;
    //    public Vector2 directionalInput;
    //    public Vector2 grabDirectionalInput;

    //    public PlayerManager playerManager;
    //    public Player player;
    //    public Controller2D playerController;

    //    public Arm arm;
    //    public Wrist wrist;

    //    public bool wallSliding;
    //    public int wallDirX;
    //    public bool isJumpingApex;
    //    public bool ledgeGrabbing;

    //    //PlayerInfo() { }
    //    public PlayerInfo(int instanceID) {
    //        InstanceID = instanceID;

    //        timeToWallUnstick = 0;

    //        gravity = 0;
    //        maxJumpVelocity = 0;
    //        minJumpVelocity = 0;
    //        velocity = Vector3.zero;
    //        velocityXSmoothing = 0;
    //        directionalInput = Vector2.zero;
    //        grabDirectionalInput = Vector2.zero;

    //        playerManager = null;
    //        player = null;
    //        playerController = null;

    //        arm = null;
    //        wrist = null;

    //        wallSliding = false;
    //        wallDirX = 0;
    //        isJumpingApex = false;
    //        ledgeGrabbing = false;


    //    }
    //}
    //public class InstanceID_Index_to_PlayerInfos
    //{

    //    Dictionary<int, VariableReference> MyDict = new Dictionary<int, VariableReference>();

    //    public void saveVar(int key, Func<object> getter, Action<object> setter)
    //    {
    //        MyDict.Add(key, new VariableReference(getter, setter));
    //    }

    //    public PlayerInfo GetVar(int key) {
    //        if (MyDict[key].Get() is int) {
    //            return (PlayerInfo) MyDict[key].Get();
    //        }
    //        //return null;
    //        return new PlayerInfo(-1);
    //    }

    //    public void changeVar(int key) // changing any of them
    //    {
    //        if (MyDict[key].Get() is int)
    //        {
    //            MyDict[key].Set((int)MyDict[key].Get() * 2);
    //        }
    //        else if (MyDict[key].Get() is string)
    //        {
    //            MyDict[key].Set("Hello");
    //        }
    //    }
    //}
    //public sealed class VariableReference
    //{
    //    public Func<object> Get { get; private set; }
    //    public Action<object> Set { get; private set; }
    //    public VariableReference(Func<object> getter, Action<object> setter)
    //    {
    //        Get = getter;
    //        Set = setter;
    //    }

    //}
}
