using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectPolygons : MonoBehaviour
{
    List<PathDrawer> pds = new List<PathDrawer>();

    public Material DeadZoneMaterial;

    public Material IceMaterial;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Check for polygons

        var paths = pds.Where(pd => !pd.cut).Select(pd => (pd, pd.positions.ToList())).ToList();

        FindPolygon(paths);
    }

    void FindPolygon(List<(PathDrawer, List<Vector3>)> paths)
    {
        foreach (var tuple in paths)
        {
            var pd = tuple.Item1;
            var path = tuple.Item2;
            Vector2[] points = Geometry.IntersectsWithItself(path);
            if (points.Length != 0)
            {
                GenerateDeadZone(points);
                GenerateShrinkingIce(points);

                pd.StopAndCreateNew();
                break;
            }

            var other_paths = paths.Where(p => p.Item2 != path).Select(p => p.Item2).ToList();
            //if (Geometry.IntersectWithAny(path, other_paths))
            //{
            //    pd.freeze = true;
            //    pd.cut = true;
            //}
        }
    }

    public void AddPD(PathDrawer pd)
    {
        pds.Add(pd);
    }

    private void GenerateDeadZone(Vector2[] points) 
    {
        //create new gameobject, setup mesh parameters, call init
        var gameObj = new GameObject("DeadZone");

        var meshComp = gameObj.AddComponent<MeshGenerator>();

        meshComp.Shrink = false;
        meshComp.GracePeriod = 0;
        meshComp.ControlPoints = points;
        meshComp.Segments = 10;
        meshComp.Mat = DeadZoneMaterial;
        meshComp.SortingLayer = 0;
    }


    private void GenerateShrinkingIce(Vector2[] points)
    {
        //create new gameobject, setup mesh parameters, call init
        var gameObj = new GameObject("Iceland");

        var meshComp = gameObj.AddComponent<MeshGenerator>();

        meshComp.Shrink = true;
        meshComp.GracePeriod = 2.0f;
        meshComp.ControlPoints = points;
        meshComp.Segments = 10;
        meshComp.Mat = IceMaterial;
        meshComp.SortingLayer = 1;
    }
}
