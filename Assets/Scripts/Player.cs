using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : NetworkBehaviour
{

    private Rigidbody2D rb;
    [SyncVar]//all the essental varibles of a rigidbody
    public Vector3 Velocity;
    [SyncVar]
    public float Rotation;
    [SyncVar]
    public Vector3 Position;
    [SyncVar]
    public float AngularVelocity;



    public float speed = 20f;
    public float max_speed = 20f;
    private bool jumping = false;

    PathDrawer pd;

    SyncList<Vector3> positions = new SyncList<Vector3>();
    public float sync_cd = 1.0f;

    public GameObject prefab_lr_holder;


    float last_sync = 0.0f;
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            // Handle jump input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                //CmdAddForce(Vector3.up * speed / 5);
                CmdPlayerJump(); // send msg to server
            }

            // handles WASD movement
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var dir = new Vector3(horizontal, vertical, 0);

            var force = dir * speed * Time.deltaTime;
            rb.AddForce(force);
            CmdAddForce(force);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, max_speed);
        }
        if (isServer)//if we are the server update the varibles with our cubes rigidbody info
        {
            Position = rb.position;
            Rotation = rb.rotation;
            Velocity = rb.velocity;
            AngularVelocity = rb.angularVelocity;
        }

        if (Time.time - last_sync > sync_cd) {

            if (isClient)//if we are a client update our rigidbody with the servers rigidbody info
            {

                //StartCoroutine(SyncRBOverTime(0.0f));
                if (hasAuthority) { 
                    CmdSetVelocity(rb.velocity);
                    CmdSetPosition(rb.position);
                }
                else
                {
                    StartCoroutine(SyncRBOverTime(0.0f));
                }
            }
            last_sync = Time.time;
        }

    }

    IEnumerator SetGravityAfterSeconds(float gravity, float seconds)
    {
        var pos = rb.position;
        var max_height = 20f;

        var curr_height = 0f;
        var i = 1f;
        var step_size = 0.1f;
        var n = max_height / step_size;
        var prev_time = Time.time;
        while (true) { 
            if (curr_height > max_height)
            {
                i *= -1;
            }
            if (curr_height < 0)
            {
                FinishJump();
                yield break;
            }
            var y = step_size * i;
            var desired_position = new Vector2(rb.position.x, rb.position.y + y);
            curr_height += y;

            rb.MovePosition(desired_position + (rb.velocity * 0.01f));
            prev_time = Time.time;
            yield return new WaitForSeconds(0.001f);
        }
        //rb.gravityScale = gravity;
    }

   
    IEnumerator SyncRBOverTime(float duration)
    {
        var curr_time = 0.0f;

        while (true)
        {
            
            var lerp_scale = duration == 0.0f ? 0f : curr_time / duration;
            rb.position = Vector2.Lerp(new Vector2(Position.x, Position.y) + rb.velocity * (float)NetworkTime.rtt, rb.position, lerp_scale);//account for the lag and update our varibles
            rb.rotation = Mathf.Lerp(Rotation * AngularVelocity * (float)NetworkTime.rtt, rb.rotation, lerp_scale);
            rb.velocity = Vector3.Lerp(Velocity, rb.velocity, lerp_scale);
            rb.angularVelocity = Mathf.Lerp(AngularVelocity, rb.angularVelocity, lerp_scale);

            var time = 0.001f;
            curr_time += time;

            if (curr_time >= duration)
            {
                yield break;
            }

            yield return new WaitForSeconds(time);
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
        HandlePathDrawer();
        HandleMovement();
        

        if (isServer)
        {
            // Update rigidbodies and positions

        }
    }

    private void HandleJumping()
    {
        if (jumping)
        {

        }
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

    void Jump()
    {
        //rb.AddForce(Vector3.up * speed / 5);
        //rb.gravityScale = 1f;
        pd.freeze = true;
        StartCoroutine(SetGravityAfterSeconds(0, 1f));
    }

    private void FinishJump()
    {
        var lr_holder = Instantiate(prefab_lr_holder) as GameObject;
        lr_holder.transform.parent = transform;
        pd = lr_holder.GetComponent<PathDrawer>();
    }


    [Command]
    private void CmdSetPosition(Vector2 position)
    {
        Position = position;
        rb.position = position;
    }
    [Command]
    private void CmdSetVelocity(Vector2 velocity)
    {
        Velocity = velocity;
        rb.velocity = velocity;
    }

    [Command]
    void CmdAddForce(Vector3 force)
    {
        rb.AddForce(force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, max_speed);
    }
    // THIS FUNCTION IS SENT TO THE SERVER AND ONLY EXECUTED THERE
    [Command]
    void CmdPlayerJump()
    {
        positions.Clear();
        Jump();

        ClientPlayerJumped();
    }

    // THIS FUNCTION IS SENT TO ALL CLIENTS AND EXECUTED THERE (NOT SERVER)
    [ClientRpc]
    void ClientPlayerJumped()
    {
        Jump();
    }

 
}
