using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    //public Vector2[] ControlPoints = new Vector2[] {
    //        new Vector2(0,0),
    //        new Vector2(-10,15),
    //        new Vector2(10,25),
    //        new Vector2(45,35),
    //        new Vector2(55,25),
    //        new Vector2(50,15),
    //        new Vector2(45,-10),
    //        new Vector2(32,-3)
    //    };

    public Vector2[] ControlPoints = new Vector2[0];
    public int Segments = 20;
    public Material Mat;
    public bool Shrink = false;
    public float GracePeriod = 5.0f;
    public int SortingLayer = 0;
    public bool UseEgdeCollider = false;
    public List<Vector2> edge_points;


    private MeshRenderer meshRenderer;
    private Mesh msh;
    private EdgeCollider2D col;
    private Vector2 Center;
    void Start()
    {
        if(UseEgdeCollider) col = GetComponent<EdgeCollider2D>();
        Init();
    }

    public void Init()
    {
        if (ControlPoints.Length < 1 ) return;

        //find additional points between the point for a smoother curvature
        CurveHandler curveHandler = new CurveHandler(ControlPoints);

        Vector2[] vertices2D = curveHandler.EvalSegmentPoints(Segments);

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        Center = Geometry.GetCenterOfPolygon2D(vertices2D.ToList());

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
            triangles = indices,
        };
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        if (UseEgdeCollider)
        {
            col.points = edge_points.ToArray();
            edge_points = vertices2D.ToList();
            edge_points.Add(vertices2D[0]);
        }


        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = SortingLayer;
        meshRenderer.material = Mat;
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }

    private float localTimer = 0.0f;
    private float scaleTimer = 0.0f;
    public void Update()
    {
        if (Shrink)
        {
            localTimer += Time.deltaTime;

            if (localTimer >= GracePeriod)
            {
                scaleTimer += Time.deltaTime;
                if (scaleTimer > 0.5f)
                {
                    //RescaleMesh(0.9f);

                    ScaleAround(Center,  transform.localScale * 0.98f);
                    scaleTimer = 0.0f;
                }
            }
        }
    }

    public void ScaleAround(Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        transform.localScale = newScale;
        transform.localPosition = FP;
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

    void OnTriggerExit2D(Collider2D other)
    {
        other.GetComponent<Rigidbody2D>().velocity = Vector2.right;
    }

}
