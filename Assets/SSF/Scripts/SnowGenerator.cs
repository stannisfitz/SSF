﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SnowGenerator : MonoBehaviour
{
    public GameObject SnowObjectsRoot;

    public Material SnowMaterial;

    private List<SnowBall> _snowBalls = new List<SnowBall>();

    public bool Generate = false;
    private Vector3[] startVertices;

    void Update()
    {
        if(Generate)
        {
            GenerateSnow(SnowObjectsRoot);
            Generate = false;
        }
    }
    
    //public List<Vector3> vertices = new List<Vector3>();

    private void GenerateSnow(GameObject g)
    {
        if (g.layer == LayerMask.NameToLayer("Terrain"))
        {
            BuildDecalForObject(g);
        }
        for (int i = 0; i < g.transform.childCount; ++i)
        {
            GenerateSnow(g.transform.GetChild(i).gameObject);
        }
    }

    private List<Vector3> bufVertices = new List<Vector3>();
    private List<Vector3> bufNormals = new List<Vector3>();
    private List<Vector2> bufTexCoords = new List<Vector2>();
    private List<int> bufIndices = new List<int>();


    public void BuildDecalForObject(GameObject affectedObject)
    {
        if (affectedObject == null || !affectedObject.activeInHierarchy || affectedObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            return;
        }
        MeshRenderer mr = affectedObject.transform.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            return;
        }
        bool added = false;
        Bounds bounds = mr.bounds;
        float size = 0.5f;
        float startX = bounds.min.x;
        float startZ = bounds.min.z;
        float endX = bounds.max.x;
        float endZ = bounds.max.z;
        float lengthX = (endX - startX);
        float lengthZ = (endZ - startZ);
        float offset = 0.02f;
        for (float i = startX + offset; i < endX; i += size)
        {
            for (float j = startZ + offset; j < endZ; j += size)
            {
                float right = i + size;
                if (right >= endX)
                {
                    right = endX - offset;
                }

                float forward = j + size;
                if (forward >= endZ)
                {
                    forward = endX - offset;
                }
                Vector3 up = new Vector3(0.0f, bounds.max.y + 0.1f, 0.0f);
                Vector3 v1 = new Vector3(i, 0.0f, j);
                Vector3 v2 = new Vector3(right, 0.0f, j);
                Vector3 v3 = new Vector3(i, 0.0f, forward);
                Vector3 v4 = new Vector3(right, 0.0f, forward);

                Ray r1 = new Ray(v1 + up, Vector3.down);
                Ray r2 = new Ray(v2 + up, Vector3.down);
                Ray r3 = new Ray(v3 + up, Vector3.down);
                Ray r4 = new Ray(v4 + up, Vector3.down);

                float h1 = float.NaN;
                float h2 = float.NaN;
                float h3 = float.NaN;
                float h4 = float.NaN;
                RaycastHit[] rh = Physics.RaycastAll(r1, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain"));
                foreach (var r in rh)
                {
                    if (r.collider.gameObject == affectedObject)
                    {
                        float height = (v1.x + size * 0.5f >= endX || v1.x - size * 0.5f <= startX || v1.z + size * 0.5f >= endZ || v1.z - size * 0.5f <= startZ) ? 0.0f : 0.4f;
                        h1 = r.point.y + height;
                        break;
                    }
                }

                rh = Physics.RaycastAll(r2, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain"));
                foreach (var r in rh)
                {
                    if (r.collider.gameObject == affectedObject)
                    {
                        float height = (v2.x + size * 0.5f >= endX || v2.x - size * 0.5f <= startX || v2.z + size * 0.5f >= endZ || v2.z - size * 0.5f <= startZ) ? 0.0f : 0.4f;
                        h2 = r.point.y + height;
                        break;
                    }
                }

                rh = Physics.RaycastAll(r3, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain"));
                foreach (var r in rh)
                {
                    if (r.collider.gameObject == affectedObject)
                    {
                        float height = (v3.x + size * 0.5f >= endX || v3.x - size * 0.5f <= startX || v3.z + size * 0.5f >= endZ || v3.z - size * 0.5f <= startZ) ? 0.0f : 0.4f;
                        h3 = r.point.y + height;
                        break;
                    }
                }
                rh = Physics.RaycastAll(r4, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain"));
                foreach (var r in rh)
                {
                    if (r.collider.gameObject == affectedObject)
                    {
                        float height = (v4.x + size * 0.5f >= endX || v4.x - size * 0.5f <= startX || v4.z + size * 0.5f >= endZ || v4.z - size * 0.5f <= startZ) ? 0.0f : 0.4f;
                        h4 = r.point.y + height;
                        break;
                    }
                }

                if (!float.IsNaN(h2) && !float.IsNaN(h3))
                {
                    v2.y = h2;
                    v3.y = h3;
                    if (!float.IsNaN(h1))
                    {
                        v1.y = h1;
                        Vector3 side1 = v2 - v1;
                        Vector3 side2 = v3 - v1;
                        Vector3 normal = Vector3.Cross(side1, side2).normalized;
                        DecalPolygon poly = new DecalPolygon(v1, v3, v2);
                        AddPolygon(poly, -normal);
                        added = true;
                    }
                    if (!float.IsNaN(h4))
                    {
                        v4.y = h4;
                        Vector3 side1 = v2 - v1;
                        Vector3 side2 = v4 - v1;
                        Vector3 normal = Vector3.Cross(side1, side2).normalized;
                        DecalPolygon poly = new DecalPolygon(v2, v3, v4);
                        AddPolygon(poly, normal);
                        added = true;
                    }
                }
            }
        }

        if (!added)
        {
            bufVertices.Clear();
            bufNormals.Clear();
            bufTexCoords.Clear();
            bufIndices.Clear();
            return;
        }

        GenerateTexCoords(0);

        Mesh mesh = CreateMesh();
        GameObject g = new GameObject("SnowObject" + affectedObject.name);
        g.layer = LayerMask.NameToLayer("SnowPatch");
        MeshFilter mf = g.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        startVertices = mesh.vertices;
        MeshRenderer renderer = g.AddComponent<MeshRenderer>();
        MeshCollider mc = g.AddComponent<MeshCollider>();
        renderer.material = SnowMaterial;
        g.transform.parent = transform;

        bufVertices.Clear();
        bufNormals.Clear();
        bufTexCoords.Clear();
        bufIndices.Clear();
    }

    private void AddPolygon(DecalPolygon poly, Vector3 normal)
    {
        int ind1 = AddVertex(poly.vertices[0], normal);
        for (int i = 1; i < poly.vertices.Count - 1; i++)
        {
            int ind2 = AddVertex(poly.vertices[i], normal);
            int ind3 = AddVertex(poly.vertices[i + 1], normal);

            bufIndices.Add(ind1);
            bufIndices.Add(ind2);
            bufIndices.Add(ind3);
        }
    }

    private int AddVertex(Vector3 vertex, Vector3 normal)
    {
        int index = FindVertex(vertex);
        if (index == -1)
        {
            bufVertices.Add(vertex);
            bufNormals.Add(normal);
            index = bufVertices.Count - 1;
        }
        else
        {
            Vector3 t = bufNormals[index] + normal;
            bufNormals[index] = t.normalized;
        }
        return (int)index;
    }

    private int FindVertex(Vector3 vertex)
    {
        for (int i = 0; i < bufVertices.Count; i++)
        {
            if (Vector3.Distance(bufVertices[i], vertex) < 0.01f)
            {
                return i;
            }
        }
        return -1;
    }

    private void GenerateTexCoords(int start)
    {
        for (int i = start; i < bufVertices.Count; i++)
        {
            Vector3 vertex = bufVertices[i];

            Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
            uv.x = Mathf.Lerp(0.0f, 50.0f, uv.x);
            uv.y = Mathf.Lerp(0.0f, 50.0f, uv.y);
            uv.x = 0.5f;
            uv.y = 0.5f;

            bufTexCoords.Add(uv);
        }
    }

    private Mesh CreateMesh()
    {
        if (bufIndices.Count == 0)
        {
            return null;
        }
        Mesh mesh = new Mesh();

        mesh.vertices = bufVertices.ToArray();
        mesh.normals = bufNormals.ToArray();
        mesh.uv = bufTexCoords.ToArray();
        mesh.uv2 = bufTexCoords.ToArray();
        mesh.triangles = bufIndices.ToArray();

        bufVertices.Clear();
        bufNormals.Clear();
        bufTexCoords.Clear();
        bufIndices.Clear();

        return mesh;
    }
}


public class DecalPolygon
{

    public List<Vector3> vertices = new List<Vector3>(9);

    public DecalPolygon(params Vector3[] vts)
    {
        vertices.AddRange(vts);
    }

    public static DecalPolygon ClipPolygon(DecalPolygon polygon, Plane plane)
    {
        bool[] positive = new bool[9];
        int positiveCount = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            positive[i] = !plane.GetSide(polygon.vertices[i]);
            if (positive[i]) positiveCount++;
        }

        if (positiveCount == 0) return null; // полностью за плоскостью
        if (positiveCount == polygon.vertices.Count) return polygon; // полностью перед плоскостью

        DecalPolygon tempPolygon = new DecalPolygon();

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            int next = i + 1;
            next %= polygon.vertices.Count;

            if (positive[i])
            {
                tempPolygon.vertices.Add(polygon.vertices[i]);
            }

            if (positive[i] != positive[next])
            {
                Vector3 v1 = polygon.vertices[next];
                Vector3 v2 = polygon.vertices[i];

                Vector3 v = LineCast(plane, v1, v2);
                tempPolygon.vertices.Add(v);
            }
        }

        return tempPolygon;
    }

    private static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b)
    {
        float dis;
        Ray ray = new Ray(a, b - a);
        plane.Raycast(ray, out dis);
        return ray.GetPoint(dis);
    }
}