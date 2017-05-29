using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RayData
{
#if UNITY_EDITOR
    public bool unfolded = true;
#endif

    public RayDesigner.FaceMode faceMode;
    public GameObject RayHolder;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Material Mat;
    public int Steps = 10;

    public AnimationCurve Shape = new AnimationCurve();
    public AnimationCurve AmplitudeMask = new AnimationCurve();
    public float WidthAmplitude = 1f;

    public float TextureSpeed = 1f;
    public float DistortionSpeed = 1f;

    public ParticleSystem StartEffects;
    public ParticleSystem HitEffects;
    public ParticleSystem EndEffects;
    public Light PointLights;

    //Buffer
    private Mesh mesh;
    private Quaternion dir = Quaternion.identity;
    private float lower;
    float Width = 0;
    int index = 0;

    private Vector3[] Vertices;
    //private Vector3[] Normals;
    private Vector2[] UVs;
    private int[] Triangles;
    private Color[] VertColor;

    public Mesh CreateMesh(Transform _parent, Vector3[] _BezierPoints, float _Fade)
    {
        if (mesh == null)
            mesh = new Mesh();

        index = 0;

        switch (faceMode)
        {
            case RayDesigner.FaceMode.Camera:
                if (_BezierPoints.Length != Steps || Vertices == null || Vertices.Length != Steps * 2)
                {
                    mesh = new Mesh();
                    Vertices = new Vector3[Steps * 2];
                    //Normals = new Vector3[Steps * 2];
                    UVs = new Vector2[Steps * 2];
                    Triangles = new int[Steps * 6 - 6];
                    VertColor = new Color[Steps * 2];
                }

                for (int i = 0; i < _BezierPoints.Length; i++)
                {
                    Width = Shape.Evaluate((float)i / (float)_BezierPoints.Length) * WidthAmplitude * _Fade;

                    if (i < _BezierPoints.Length - 1)
                    {
                        if (Camera.main == null)
                        {
                            Debug.LogWarning("[URD] Main Camera not assigned. Ray cannot face Camera! Assign a main Camera to use this feature!");
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, Vector3.up);
                        }
                        else {
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, (Camera.main.transform.position - _BezierPoints[i]).normalized);
                        }
                    }

                    lower = (float)i / (float)_BezierPoints.Length;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(-Width, 0, 0));
                    //Normals[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * Vector3.up);
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(0, lower);
                    index++;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(Width, 0, 0));
                    //Normals[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * Vector3.up);
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(1, lower);
                    index++;
                }

                index = 0;

                //Triangles
                for (int i = 0; i < _BezierPoints.Length - 1; i++)
                {
                    int ii = i * 2;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 1;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;
                    Triangles[index] = ii + 2;
                    index++;
                }

                mesh.vertices = Vertices;
                //mesh.normals = Normals;
                mesh.uv = UVs;
                mesh.triangles = Triangles;
                mesh.colors = VertColor;
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();
                return mesh;

            case RayDesigner.FaceMode.Cross:
                if (_BezierPoints.Length != Steps || Vertices == null || Vertices.Length != Steps * 4)
                {
                    mesh = new Mesh();
                    Vertices = new Vector3[Steps * 4];
                    UVs = new Vector2[Steps * 4];
                    Triangles = new int[Steps * 12 - 12];
                    VertColor = new Color[Steps * 4];
                }

                for (int i = 0; i < _BezierPoints.Length; i++)
                {
                    Width = Shape.Evaluate((float)i / (float)_BezierPoints.Length) * WidthAmplitude * _Fade;

                    if (i < _BezierPoints.Length - 1)
                    {
                        if (i < _BezierPoints.Length - 1)
                        {
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, (Camera.main.transform.position - _BezierPoints[i]).normalized);
                        }
                    }

                    lower = (float)i / (float)_BezierPoints.Length;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(-Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(0, lower);
                    index++;


                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(1, lower);
                    index++;
                }

                for (int i = 0; i < _BezierPoints.Length; i++)
                {
                    Width = Shape.Evaluate((float)i / (float)_BezierPoints.Length) * WidthAmplitude * _Fade;

                    if (i < _BezierPoints.Length - 1)
                    {
                        if (i < _BezierPoints.Length - 1)
                        {
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, (Camera.main.transform.position - _BezierPoints[i]).normalized);
                        }
                    }

                    lower = (float)i / (float)_BezierPoints.Length;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(0, Width, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(0, lower);
                    index++;


                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(0, -Width, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(1, lower);
                    index++;
                }

                index = 0;

                for (int i = 0; i < _BezierPoints.Length - 1; i++)
                {
                    int ii = i * 2;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 1;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;
                    Triangles[index] = ii + 2;
                    index++;
                }

                for (int i = _BezierPoints.Length; i < ((_BezierPoints.Length) * 2) - 1; i++)
                {
                    int ii = i * 2;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 1;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;
                    Triangles[index] = ii + 2;
                    index++;
                }

                mesh.vertices = Vertices;
                mesh.uv = UVs;
                mesh.triangles = Triangles;
                mesh.colors = VertColor;
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();
                return mesh;

            case RayDesigner.FaceMode.Vertical:
                if (_BezierPoints.Length != Steps || Vertices == null || Vertices.Length != Steps * 2)
                {
                    mesh = new Mesh();
                    Vertices = new Vector3[Steps * 2];
                    UVs = new Vector2[Steps * 2];
                    Triangles = new int[Steps * 6 - 6];
                    VertColor = new Color[Steps * 2];
                }

                for (int i = 0; i < _BezierPoints.Length; i++)
                {
                    Width = Shape.Evaluate((float)i / (float)_BezierPoints.Length) * WidthAmplitude * _Fade;

                    if (i < _BezierPoints.Length - 1)
                    {
                        dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, Vector3.back);
                    }

                    lower = (float)i / (float)_BezierPoints.Length;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(-Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(0, lower);
                    index++;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(1, lower);
                    index++;
                }

                index = 0;

                //Triangles
                for (int i = 0; i < _BezierPoints.Length - 1; i++)
                {
                    int ii = i * 2;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 1;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;
                    Triangles[index] = ii + 2;
                    index++;
                }

                mesh.vertices = Vertices;
                mesh.uv = UVs;
                mesh.triangles = Triangles;
                mesh.colors = VertColor;
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();
                return mesh;

            case RayDesigner.FaceMode.Horizontal:
                if (_BezierPoints.Length != Steps || Vertices == null || Vertices.Length != Steps * 2)
                {
                    mesh = new Mesh();
                    Vertices = new Vector3[Steps * 2];
                    UVs = new Vector2[Steps * 2];
                    Triangles = new int[Steps * 6 - 6];
                    VertColor = new Color[Steps * 2];
                }

                for (int i = 0; i < _BezierPoints.Length; i++)
                {
                    Width = Shape.Evaluate((float)i / (float)_BezierPoints.Length) * WidthAmplitude * _Fade;

                    if (i < _BezierPoints.Length - 1)
                    {
                        if (Camera.main == null)
                        {
                            Debug.LogWarning("[URD] Main Camera not assigned. Ray cannot face Camera! Assign a main Camera to use this feature!");
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, Vector3.up);
                        }
                        else {
                            dir = Quaternion.LookRotation((_BezierPoints[i] - _BezierPoints[i + 1]).normalized, Vector3.up);
                        }
                    }

                    lower = (float)i / (float)_BezierPoints.Length;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(-Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(0, lower);
                    index++;

                    Vertices[index] = _parent.InverseTransformPoint(_BezierPoints[i] + dir * new Vector3(Width, 0, 0));
                    VertColor[index] = Color.white * AmplitudeMask.Evaluate((float)i / (float)_BezierPoints.Length);
                    UVs[index] = new Vector2(1, lower);
                    index++;
                }

                index = 0;

                //Triangles
                for (int i = 0; i < _BezierPoints.Length - 1; i++)
                {
                    int ii = i * 2;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 1;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;

                    Triangles[index] = ii;
                    index++;
                    Triangles[index] = ii + 3;
                    index++;
                    Triangles[index] = ii + 2;
                    index++;
                }

                mesh.vertices = Vertices;
                mesh.uv = UVs;
                mesh.triangles = Triangles;
                mesh.colors = VertColor;
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();
                return mesh;
        }

        return null;
    }
}
