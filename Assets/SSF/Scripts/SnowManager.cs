using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnowManager : MonoBehaviour
{
    private static SnowManager _instance;
    public static SnowManager Instance
    {
        get { return _instance; }
    }

    public GameObject FortPrefab;
    
    private List<SnowBall> _snowBalls = new List<SnowBall>();

    void Awake()
    {
        SnowManager._instance = this;
    }

    public void GetPoints(out int team1, out int team2)
    {
        team1 = 0;
        team2 = 0;
        float p1 = 0.0f;
        float p2 = 0.0f;
        foreach(var s in _snowBalls)
        {
            if(s == null)
            {
                continue;
            }
            if(s.Team == 0)
            {
                p1 += s.transform.localScale.x;
            }
            else
            {
                p2 += s.transform.localScale.x;
            }
        }
        team1 = (int) p1;
        team2 = (int) p2;
    }

    public void AddSnowBall(SnowBall snowBall)
    {
        _snowBalls.Add(snowBall);
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
            if(sharedVertices[triangles[hitInfo.triangleIndex * 3 + 0]].y < 0.0f)
            {
                return false;
            }
            /*float diff = (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 0]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 0]].y) +
                (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 1]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 1]].y) +
                (sharedVertices[triangles[hitInfo.triangleIndex * 3 + 2]].y - startVertices[triangles[hitInfo.triangleIndex * 3 + 2]].y);
            if(diff > 1.5f)
            {
                return false;
            }*/
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 0]] -= new Vector3(0.0f, 0.3f, 0.0f);
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 1]] -= new Vector3(0.0f, 0.3f, 0.0f);
            sharedVertices[triangles[hitInfo.triangleIndex * 3 + 2]] -= new Vector3(0.0f, 0.3f, 0.0f);
            mesh.vertices = sharedVertices;
            return true;
        }
        return false;
    }
}