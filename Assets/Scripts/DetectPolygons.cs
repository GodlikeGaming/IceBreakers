using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectPolygons : MonoBehaviour
{
    List<PathDrawer> pds = new List<PathDrawer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Check for polygons

        var paths = pds.Where(pd => !pd.cut).Select(pd => (pd, pd.positions)).ToList();

        FindPolygon(paths);
        Debug.Log(paths.Count());
    }

    void FindPolygon(List<(PathDrawer, List<Vector3>)> paths)
    {
        foreach(var tuple in paths)
        {
            var pd = tuple.Item1;
            var path = tuple.Item2;
            if (Geometry.IntersectsWithItself(path).HasValue)
            {
                pd.freeze = true;
                pd.cut = true;
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
}
