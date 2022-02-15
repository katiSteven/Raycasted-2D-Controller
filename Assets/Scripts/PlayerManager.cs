using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DirectionalInput(Vector2 input);
public delegate void GrabDirectionalInput(Vector2 input);
public delegate void JumpInputDown();
public delegate void JumpInputUp();
public delegate void GrabRelease();

public class PlayerManager : MonoBehaviour
{
    //[SerializeField] public static float maxJumpHeight = 4; //5
    //[SerializeField] public static float minJumpHeight = 1; //1
    //[SerializeField] public static float timeToJumpApex = .4f; //0.5

    //[SerializeField] public static float accelerationTimeAirborne = .2f; //0.2
    //[SerializeField] public static float accelerationTimeGrounded = .1f; //0.1
    //[SerializeField] public static float moveSpeed = 6; //6

    //[SerializeField] public static Vector2 wallJumpClimb; //7.5 16
    //[SerializeField] public static Vector2 wallJumpOff; //8.5 7
    //[SerializeField] public static Vector2 wallLeap; //18 17

    //[SerializeField] public static float wallSlideSpeedMax = 3; //3
    //[SerializeField] public static float wallStickTime = 0.25f; //0.25

    //[SerializeField] public static float ArmLength = 1f; //0.75

    //[SerializeField] public static float WristLength = 0.1f; //0.1

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

    //----------Properties----------
    

    //----------Events----------
    public static event DirectionalInput OnDirectionalInput;
    public static event GrabDirectionalInput OnGrabDirectionalInput;
    public static event JumpInputDown OnJumpInputDown;
    public static event JumpInputUp OnJumpInputUp;
    public static event GrabRelease OnGrabRelease;

    //----------Methods----------
    public static void ExecuteDirectionalUserInput(Vector2 input) { OnDirectionalInput(input); }
    public static void ExecuteGrabDirectionalUserInput(Vector2 input) { OnGrabDirectionalInput(input); }
    public static void ExecuteJumpInputDownUserInput() { OnJumpInputDown(); }
    public static void ExecuteJumpInputUpUserInput() { OnJumpInputUp(); }
    public static void ExecuteGrabRelease() { OnGrabRelease(); }
}
