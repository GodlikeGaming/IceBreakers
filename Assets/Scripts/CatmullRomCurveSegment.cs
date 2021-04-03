using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomCurveSegment
{
    /// <summary>
    /// the M matrix contains the coefficients of the cubic polynomials used in the curve formulation
    /// </summary>
    // CatmullRom Coefficients matrix
    private static Matrix4x4 M = new Matrix4x4(
                    new Vector4(-1, 3, -3, 1) / 2.0f, 
                    new Vector4(2, -5, 4, -1) / 2.0f,  
                    new Vector4(-1, 0, 1, 0) / 2.0f,  
                    new Vector4(0, 2, 0, 0) / 2.0f);

    /// <summary>
    /// B contains the control parameters (points/vectors) of the curve
    /// </summary>
    public Matrix4x4 B;
    public CatmullRomCurveSegment(Vector4 cv1, Vector4 cv2, Vector4 cv3, Vector4 cv4)
    {
        B = new Matrix4x4(cv1, cv2, cv3, cv4);
    }

    public Vector4 Evaluate(float u)
    {
        // compute parameter matrix U and evaluate p at u
        Vector4 U = new Vector4(u * u * u, u * u, u, 1f); 
        Vector4 p = B * M * U;
        return p;
    }

    /// <summary>
    /// evaluate tangent of curve segment at u, for u in the normalized range [0,1]
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public Vector4 EvaluateDv(float u)
    {
        //compute the first derivative of U
        Vector4 U = new Vector4(u * u * 3, u * 2f, 1f, 0f);
        Vector4 p = B * M * U;
        return p;
    }

    /// <summary>
    /// evaluate curvature of curve segment at u, for u in the normalized range [0,1]
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public Vector4 EvaluateDv2(float u)
    {

        //compute the second derivative of U
        Vector4 U = new Vector4(u * 6, 2, 0, 0); 
        Vector4 p = B * M * U; 
        return p;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
