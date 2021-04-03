using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{

    LineRenderer lr;
    GameObject holder;

    Vector3 previous_pos;
   
    float min_distance = 0.5f;
    internal bool freeze;

    internal bool cut = false;

    public List<Vector3> positions = new List<Vector3>();



    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLineRenderer(List<Vector3> positions)
    {
        lr.positionCount = positions.Count();
        lr.SetPositions(positions.ToArray());
    }

    internal Vector3? AddPosition(Vector3 current_pos, float min_distance_scale = 1f)
    {
        if (freeze) return null;
        if (Vector3.Distance(previous_pos, current_pos) > min_distance * min_distance_scale)
        {            
            previous_pos = current_pos;
            return current_pos;
        }
        return null
            ;
    }


    internal void SetPositions(List<Vector3> positions)
    {
        if (freeze) return;
        lr.positionCount = positions.Count();
        lr.SetPositions(positions.ToArray());
        this.positions = positions;
    }
}
