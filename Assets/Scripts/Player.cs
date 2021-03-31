using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{

    private Rigidbody2D rb;
    public float speed = 20f;
    public float max_speed = 20f;

    PathDrawer pd;

    SyncList<Vector3> positions = new SyncList<Vector3>();

    public GameObject prefab_lr_holder;
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var dir = new Vector3(horizontal, vertical, 0);

            rb.AddForce(dir * speed * Time.deltaTime);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, max_speed);
            Debug.Log(rb.velocity.magnitude);
        }
    }

    void Start()
    {
        var lr_holder = Instantiate(prefab_lr_holder) as GameObject;
        lr_holder.transform.parent = transform;
        pd = lr_holder.GetComponent<PathDrawer>();


        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            rb.AddForce(Vector3.left * speed);
            CmdPlayerJump();
        }

        HandlePathDrawer();
        HandleMovement();        
    }

    private void HandlePathDrawer()
    {
        if (isLocalPlayer) {
            
        }

        if (isServer)
        {
            var current_pos = transform.position;

            var added = pd.AddPosition(current_pos);
            if (added != null)
            {
                //CmdAddPosition(current_pos);
                positions.Add(current_pos);
            }
        }

        pd.SetPositions(positions.ToList());
    }

    [Command]
    void CmdAddPosition(Vector3 pos)
    {
        positions.Add(pos);
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
