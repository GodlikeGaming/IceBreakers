using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathDrawer : NetworkBehaviour
{

    LineRenderer lr;
    GameObject holder;

    Vector3 previous_pos;
   
    float min_distance = 0.5f;
    internal bool freeze;

    internal bool cut = false;

    public SyncList<Vector3> positions = new SyncList<Vector3>();

    [SyncVar]
    public GameObject player;

    Player player_script;


    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
    }

    void Start()
    {
        player_script = player.GetComponent<Player>();
        player_script.pd = this;
    }


    // Update is called once per frame
    void Update()
    {
        HandlePathDrawer();
    }

    private void HandlePathDrawer()
    {
        if (isLocalPlayer)
        {

        }

        if (isServer)
        {
            if (freeze) return;
            if (player_script.jumping) return;
            var current_pos = player.transform.position;

            var added = AddPosition(current_pos);
            if (added != null)
            {
                //CmdAddPosition(current_pos);
                positions.Add(current_pos);
            }
        }

        SetPositions(positions.ToList());
    }


    internal Vector3? AddPosition(Vector3 current_pos, float min_distance_scale = 1f)
    {
        if (freeze) return null;
        if (Vector3.Distance(previous_pos, current_pos) > min_distance * min_distance_scale)
        {            
            previous_pos = current_pos;
            return current_pos;
        }
        return null;
    }


    internal void SetPositions(List<Vector3> positions)
    {
        lr.positionCount = positions.Count();
        lr.SetPositions(positions.ToArray());
    }

    internal void Stop()
    {
        freeze = true;
    }
    internal void StopAndCreateNew()
    {
        Stop();
        cut = true;

        FindObjectOfType<Server>().AddPathDrawer(player);
    }
}
