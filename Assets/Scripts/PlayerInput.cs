using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
    public Wrist wristScript { get; set; }
    //BoxCollider2D LedgeCollider;
    void Start() {
        player = GetComponent<Player>();
        
    }

    void Update() {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 grabDirectionalInput = new Vector2(Input.GetAxisRaw("Grab Horizontal"), Input.GetAxisRaw("Grab Vertical"));
        if (wristScript != null) {
            wristScript.SetDirectionalInput(directionalInput);
            if (Input.GetAxisRaw("Grab Release") != 0f)
            {
                wristScript.GrabRelease();
            }
            if (Input.GetAxisRaw("Jump") != 0f)
            {
                wristScript.OnJumpInputDown();
            }
            //if (Input.GetAxisRaw("Jump") == 0f)
            //{
            //    wristScript.OnJumpInputUp();
            //}
        } else {
            player.SetDirectionalInput(directionalInput);
            if (Input.GetAxisRaw("Jump") != 0f)
            {
                player.OnJumpInputDown();
            }
            if (Input.GetAxisRaw("Jump") == 0f)
            {
                player.OnJumpInputUp();
            }
            if (grabDirectionalInput != Vector2.zero)
            {
                player.GrabInputdown(grabDirectionalInput);
            }
        }

        

        //Player playerInstance = !player.enabled ? wrist : player;

        //if (Input.GetAxisRaw("Jump") != 0f) {
        //    player.OnJumpInputDown();
        //}
        //if (Input.GetAxisRaw("Jump") == 0f) {
        //    player.OnJumpInputUp();
        //}
        //if (Input.GetAxisRaw("Grab Release") != 0f) {
        //    player.GrabRelease();
        //} else if (grabDirectionalInput != Vector2.zero) {
        //    player.GrabInputdown(grabDirectionalInput);
        //}
    }
}
