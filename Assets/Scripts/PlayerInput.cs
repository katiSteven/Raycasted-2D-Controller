using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {
    PlayerManager playerManager;

    void Start() {
        playerManager = GetComponent<PlayerManager>();
    }

    void Update() {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 grabDirectionalInput = new Vector2(Input.GetAxisRaw("Grab Horizontal"), Input.GetAxisRaw("Grab Vertical"));

        PlayerManager.ExecuteDirectionalUserInput(directionalInput);
        if (grabDirectionalInput != Vector2.zero) {
            PlayerManager.ExecuteGrabDirectionalUserInput(grabDirectionalInput);
        }
        if (Input.GetAxisRaw("Jump") != 0f) {
            PlayerManager.ExecuteJumpInputDownUserInput();
        } else {
            PlayerManager.ExecuteJumpInputUpUserInput();
        }
        if (Input.GetAxisRaw("Grab Release") != 0f)
        {
            PlayerManager.ExecuteGrabRelease();
        }
    }
}
