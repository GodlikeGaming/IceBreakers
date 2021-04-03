using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public Vector2[] ControlPoints = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,50),
            new Vector2(50,50),
            new Vector2(50,100),
            new Vector2(0,100),
            new Vector2(0,150),
            new Vector2(150,150),
            new Vector2(150,100),
            new Vector2(100,100),
            new Vector2(100,50),
            new Vector2(150,50),
            new Vector2(150,0),
        };

    public int segments = 20;

    public Material mat;

    public bool shrink = false;

    public float gracePeriod = 5.0f;

    private MeshRenderer meshRenderer;
    private Mesh msh;

    void Start()
    {
        //find additional points between 

        CurveHandler curveHandler = new CurveHandler(ControlPoints);

        Vector2[] vertices2D = curveHandler.EvalNewPoints(segments);


        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        msh = new Mesh
        {
            vertices = vertices,
            triangles = indices
        };
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = mat;
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }


    private float localTimer = 0.0f;
    private float scaleTimer = 0.0f;
    public void Update()
    {
        if (shrink)
        {
            localTimer += Time.deltaTime;

            if (localTimer >= gracePeriod)
            {
                scaleTimer += Time.deltaTime;
                if (scaleTimer > 2.0f )
                { 
                    RescaleMesh(0.9f);
                    scaleTimer = 0.0f;
                }
            }
        }
    }

    private void RescaleMesh(float scale)
    {

        var baseVertices = msh.vertices;
        var vertices = new Vector3[baseVertices.Length];

        for (var i = 0; i < vertices.Length; i++)
        {
            var vertex = baseVertices[i];
            vertex.x *= scale;
            vertex.y *= scale;
            vertex.z *= scale;

            vertices[i] = vertex;
        }

        //TODO: if object becomes very small just delete it. 

        msh.vertices = vertices;

        msh.RecalculateNormals();
        msh.RecalculateBounds();
    }

}
