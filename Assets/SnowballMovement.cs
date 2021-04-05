using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMovement : NetworkBehaviour
{

   // public Vector2 dir = Vector2.right;
    public float speed = 10f;

    Rigidbody2D rb;

    [SyncVar]//all the essental varibles of a rigidbody
    public Vector3 Velocity;
    [SyncVar]
    public float Rotation;
    [SyncVar]
    public Vector3 Position;
    [SyncVar]
    public float AngularVelocity;

    float last_sync = 0.0f;
    public float sync_cd = 0.1f;

    [SyncVar]
    public GameObject ignorePlayer;

    public float max_height = 4;
    float curr_height = 0f;
    float i = 1f;
    float step_size = 0.1f;
    Vector2 shadow_offset;
    public GameObject shadow_prefab;
    public GameObject shadow;
    public float jump_start_y = 0f;
    public float accumulated_rb_jump_y = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), ignorePlayer.GetComponent<Collider2D>());
        rb = GetComponent<Rigidbody2D>();
        rb.position = Position;
        //rb.velocity = Velocity;
        jump_start_y = rb.position.y;
        shadow = transform.GetChild(0).gameObject;
        shadow_offset = new Vector2(shadow.transform.position.x, shadow.transform.position.y);
        shadow.transform.parent = transform;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (curr_height > max_height)
        {
            i *= -1;
        }
        if (curr_height < 0)
        {
            FinishJump();
            return;
        }
        var y = step_size * i;
        shadow.transform.localScale -= shadow.transform.localScale * y / 4;
        var desired_position = new Vector2(rb.position.x, rb.position.y + y);
        curr_height += y;

        // shadow.transform.position = rb.position;
        var pos = shadow_offset + (desired_position + ((Vector2) Velocity * Time.deltaTime * speed));
        accumulated_rb_jump_y += y;//(rb.velocity * Time.deltaTime * 0.5f).y;
        shadow.transform.position = new Vector2((desired_position + ((Vector2)Velocity * Time.deltaTime * speed)).x, (desired_position + ((Vector2)Velocity * Time.deltaTime * speed)).y - accumulated_rb_jump_y);
        rb.MovePosition(desired_position + ((Vector2) Velocity * Time.deltaTime * speed));

        //var force = (Velocity * speed * Time.deltaTime);
        //var desired_position = rb.position + (Vector2)force;
        //rb.MovePosition(desired_position);
    }

    void Update()
    {
        HandleSyncing();
    }

    private void FinishJump()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void HandleSyncing()
    {
        if (Time.time - last_sync > sync_cd)
        {


            //StartCoroutine(SyncRBOverTime(0.0f));
            if (isServer)
            {
                Position = rb.position;

                Velocity = Velocity;
            }
            else
            {
                rb.position = Position + (Velocity * speed) * (float)NetworkTime.rtt;
                // rb.rotation =Rotation * AngularVelocity * (float)NetworkTime.rtt;
                // rb.velocity = Velocity;
                rb.angularVelocity = AngularVelocity;
                }
            
            last_sync = Time.time;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) { 
            Debug.Log($"HIT! {collision.gameObject.name}");
            NetworkServer.Destroy(gameObject);
        }
    }
}
