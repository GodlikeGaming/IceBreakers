using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CurveHandler
    {
        private List<CatmullRomCurveSegment> curves;

        public List<CatmullRomCurveSegment> GetCurves()
        {
            return curves;
        }

        public CurveHandler(Vector2[] points)
        {
            curves = new List<CatmullRomCurveSegment>();

            // build the curve segments
            for (int i = 0; i < points.Length; i++)
            {
                var curveSeg = new CatmullRomCurveSegment(
                    points[i], 
                    points[(i + 1) % points.Length],
                    points[(i + 2) % points.Length],
                    points[(i + 3) % points.Length]
                    );
                curves.Add(curveSeg);
            }
        }

        public Vector2[] EvalSegmentPoints(int segments)
        {
            var list = new List<Vector2>();

            float steps = 1.0f / segments;
            foreach (var curve in curves)
            {
                for (int i = 1; i <= segments; i++)
                {
                    var u = steps * (float)i;
                    list.Add(curve.Evaluate(u));
                }
            }
            return list.ToArray();
        }
    }
