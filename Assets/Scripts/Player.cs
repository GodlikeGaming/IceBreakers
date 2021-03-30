using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{

    private Rigidbody2D rb;
    public float speed = 20f;
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var dir = new Vector3(horizontal, vertical, 0);

            rb.AddForce(dir * speed * Time.deltaTime);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            rb.AddForce(Vector3.left * speed);
            CmdPlayerJump();
        }
        HandleMovement();        
    }

    // THIS FUNCTION IS SENT TO THE SERVER AND ONLY EXECUTED THERE
    [Command]
    void CmdPlayerJump()
    {
        Debug.Log("Player jumped, now telling the clients!");
        ClientPlayerJumped();
    }

    // THIS FUNCTION IS SENT TO ALL CLIENTS AND EXECUTED THERE (NOT SERVER)
    [ClientRpc]
    void ClientPlayerJumped()
    {
        Debug.Log($"Player {netId} just jumped!");
        //rb.AddForce(Vector3.left * speed);
    }

 
}
