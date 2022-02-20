using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] float startAfterSeconds;
    [SerializeField] float bounceTime = 3f;
    [SerializeField] float maxJumpHeight = 4;
    [SerializeField] float timeToJumpApex = .4f;
    [SerializeField] float moveSpeed = 6;
    [SerializeField] [Range(-1f,1f)] float directionX; //0.5f
    
    float gravity;
    [HideInInspector] public float maxJumpVelocity;
    private Vector3 velocity;
    Vector2 directionalInput;
    Vector2 currentVelocity;
    
    bool isbouncing = false;
    bool isInitialbounce = true;
    [HideInInspector] public Controller2D controller;
    public ItemInfo itemInfo;

    private void Awake() {
        controller = GetComponent<Controller2D>();
    }

    void Start() {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        //print(gameObject.name + ": Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
        Invoke("ItemDropMovement", Time.deltaTime);
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }


    void Update() {
        CalculateVelocity();

        controller.Move(velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below) {
            if (isbouncing) {
                if (controller.collisions.slidingDownMaxSlope) {
                    velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
                } else {
                    //print("Floor was hit " + controller.collisions.floorHit);
                    //print(controller.collisions.slopeNormal);
                    if (controller.collisions.floorHit) {
                        directionalInput.x = Mathf.Sign(controller.collisions.floorHit.normal.x) * Time.deltaTime;
                    }
                    velocity = Vector2.SmoothDamp(velocity * new Vector2(1, -1), Vector2.zero, ref currentVelocity, bounceTime * Time.deltaTime);
                }
            } else {
                velocity = Vector2.zero;
            }
        }
        else if(!isInitialbounce) {
            isbouncing = true;
            StartCoroutine("StopBouncing");
        }
    }

    void CalculateVelocity() {
        velocity.x += directionalInput.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
    }

    void ItemDropMovement() {
        isbouncing = true;
        isInitialbounce = true;
        directionalInput.x = directionX;
        velocity.y = maxJumpVelocity;
        StartCoroutine("StopBouncing");
    }

    IEnumerator StopBouncing() {
        CancelInvoke("ItemDropMovement");
        yield return new WaitForSeconds(Time.deltaTime * bounceTime);
        directionalInput.x = 0f;

        yield return new WaitForSeconds(bounceTime);
        isbouncing = false;
        isInitialbounce = false;
        StopCoroutine("StopBouncing");
    }

    public struct ItemInfo {
        public bool isPickedUp;
    }
}
