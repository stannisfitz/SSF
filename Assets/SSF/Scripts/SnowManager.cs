using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnowManager : MonoBehaviour
{
    public GameObject SnowPatchPrefab;
    public Material SnowMaterial;
    private static SnowManager _instance;
    public static SnowManager Instance
    {
        get { return _instance; }
    }

    public GameObject FortPrefab;

    private List<GameObject> _snowPatches;
    private List<SnowBall> _snowBalls = new List<SnowBall>();

    private const float k_patchSize = 0.5f;
    private const int k_size = 200;
    void Awake()
    {
        GenerateSnow();
        SnowManager._instance = this;
        _snowPatches = new List<GameObject>();
        /*for (float i = -k_size * 0.5f; i < k_size * 0.5f; i += k_patchSize)
        {
            for (float j = -k_size * 0.5f; j < k_size * 0.5f;)
            {
                RaycastHit hitInfo;
                float x = i + (k_patchSize * 0.5f);
                float y = j + (k_patchSize * 0.5f);
                Ray ray = new Ray(new Vector3(x, 9999.9f, y), -transform.up);
                if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
                {
                    GameObject sp = GameObject.Instantiate(SnowPatchPrefab, new Vector3(x, hitInfo.point.y, y), Quaternion.identity) as GameObject;
                    sp.transform.parent = transform;
                    sp.transform.up = hitInfo.normal;
                    _snowPatches.Add(sp);
                    j += sp.transform.forward.z*0.5f;
                }
                else
                {
                    j += k_patchSize;
                }
            }
        }*/
    }

    public void SnowBallDropped(SnowBall snowBall)
    {
        if (!TryCombine(snowBall))
        {
            _snowBalls.Add(snowBall);
            snowBall.transform.parent = transform;
        }
    }

    public bool TryCombine(SnowBall snowBall)
    {
        SnowBall closest = null;
        float bestDist = float.MaxValue;
        float scale = snowBall.transform.localScale.x;
        Vector3 position = snowBall.transform.position;
        foreach (var sb in _snowBalls)
        {
            float s = sb.transform.localScale.x;
            Vector3 p = sb.transform.position;
            float sizes = scale * 0.5f + s * 0.5f;
            float maxDist = sizes + 0.5f;
            float dist = Vector3.Distance(position, p);
            if (dist < maxDist && dist - sizes < bestDist)
            {
                bestDist = dist - sizes;
                closest = sb;
            }
        }
        if (closest != null)
        {
            _snowBalls.Remove(closest);
            Vector3 pos = (position + closest.transform.position) * 0.5f;
            float s = (scale + closest.transform.localScale.x) * 0.5f;
            GameObject fort = GameObject.Instantiate(FortPrefab, pos, Quaternion.identity) as GameObject;
            fort.transform.parent = transform;
            fort.transform.localScale = new Vector3(s, s, s);
            GameObject.Destroy(closest.gameObject);
            GameObject.Destroy(snowBall.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3[] startVertices;
    public bool UpdateSnow(Vector3 lastPos, Vector3 pos, Collider snowPatchCollider)
    {
        lastPos.y = pos.y;

        RaycastHit hitInfo;
        Ray r = new Ray(pos, Vector3.down);
        if (Physics.Raycast(r, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("SnowPatch")))
        {
            Mesh sharedMesh = hitInfo.collider.GetComponent<MeshFilter>().sharedMesh;
            Mesh mesh = hitInfo.collider.GetComponent<MeshFilter>().sharedMesh;

            Vector3[] sharedVertices = sharedMesh.vertices;
            int[] triangles = mesh.triangles;
            /*float diff = (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 0]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 0]].y) +
                (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 1]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 1]].y) +
                (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 2]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 2]].y);
            if(diff > 1.5f)
            {
                return false;
            }*/
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 0]] -= new Vector3(0.0f, 0.1f, 0.0f);
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 1]] -= new Vector3(0.0f, 0.1f, 0.0f);
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 2]] -= new Vector3(0.0f, 0.1f, 0.0f);
            mesh.vertices = sharedVertices;
            return true;
        }
        return false;
    }
    
    public List<Vector3> vertices = new List<Vector3>();

    private void GenerateSnow()
    {
        BuildDecalForObject(gameObject);
        Mesh mesh = CreateMesh();
        GameObject g = new GameObject("SnowObject");
        g.layer = LayerMask.NameToLayer("SnowPatch");
        MeshFilter mf = g.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        startVertices = mesh.vertices;
        MeshRenderer mr = g.AddComponent<MeshRenderer>();
        MeshCollider mc = g.AddComponent<MeshCollider>();
        mr.material = SnowMaterial;
        return;
    }

    private List<Vector3> bufVertices = new List<Vector3>();
    private List<Vector3> bufNormals = new List<Vector3>();
    private List<Vector2> bufTexCoords = new List<Vector2>();
    private List<int> bufIndices = new List<int>();


    public void BuildDecalForObject(GameObject affectedObject)
    {
        float size = 0.5f;
        for (float i = 0; i < 50; i+= size)
        {
            for (float j = 0; j < 50; j+= size)
            {
                Vector3 up = new Vector3(0.0f, 10.0f, 0.0f);
                Vector3 v1 = new Vector3(i, 0.0f, j);
                Vector3 v2 = new Vector3(i+ size, 0.0f, j);
                Vector3 v3 = new Vector3(i, 0.0f, j+ size);
                Vector3 v4 = new Vector3(i + size, 0.0f, j + size);

                Ray r1 = new Ray(v1 + up, Vector3.down);
                Ray r2 = new Ray(v2 + up, Vector3.down);
                Ray r3 = new Ray(v3 + up, Vector3.down);
                Ray r4 = new Ray(v4 + up, Vector3.down);
                float height = 0.3f;
                RaycastHit hitInfo;
                if (Physics.Raycast(r2, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
                {
                    v2.y = hitInfo.point.y + height;
                    if (Physics.Raycast(r3, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
                    {
                        v3.y = hitInfo.point.y + height;
                        if (Physics.Raycast(r1, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
                        {
                            v1.y = hitInfo.point.y + height;
                            Vector3 side1 = v2 - v1;
                            Vector3 side2 = v3 - v1;
                            Vector3 normal = Vector3.Cross(side1, side2).normalized;
                            DecalPolygon poly = new DecalPolygon(v2, v3,v1);
                            AddPolygon(poly, -normal);
                        }
                        if (Physics.Raycast(r4, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
                        {
                            v4.y = hitInfo.point.y + height;
                            Vector3 side1 = v2 - v1;
                            Vector3 side2 = v4 - v1;
                            Vector3 normal = Vector3.Cross(side1, side2).normalized;
                            DecalPolygon poly = new DecalPolygon(v2, v3,v4);
                            AddPolygon(poly, normal);
                        }
                    }
                }
            }
        }
        GenerateTexCoords(0);
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
        else {
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