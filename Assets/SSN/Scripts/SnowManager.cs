using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnowManager : MonoBehaviour
{
    public GameObject SnowPatchPrefab;
    private static SnowManager _instance;
    public static SnowManager Instance
    {
        get { return _instance; }
    }

    public GameObject FortPrefab;

    private GameObject[,] _snowPatches;
    private List<SnowBall> _snowBalls = new List<SnowBall>();

    private const int k_numPatches = 100;
    void Awake()
    {
        SnowManager._instance = this;
        _snowPatches = new GameObject[k_numPatches, k_numPatches];
        for(int i = 0; i < k_numPatches; i++)
        {
            for(int j = 0; j < k_numPatches; ++j)
            {
                RaycastHit hitInfo;
                Ray ray = new Ray(new Vector3(((float)i) * 0.5f+0.25f, 9999.9f, ((float)j) * 0.5f + 0.25f), -transform.up);
                if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1<<LayerMask.NameToLayer("Terrain")))
                {
                    GameObject sp = GameObject.Instantiate(SnowPatchPrefab, new Vector3(((float)i) * 0.5f + 0.25f, hitInfo.point.y, ((float)j) * 0.5f + 0.25f), Quaternion.identity) as GameObject;
                    sp.transform.parent = transform;
                    _snowPatches[i, j] = sp;
                }
            }
        }
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

    public void UpdateSnow(Vector3 lastPos, Vector3 pos)
    {
        lastPos.y = pos.y;
        if(Vector3.Distance(lastPos, pos) < 0.01f)
        {
            return;
        }
        int x = (int)(lastPos.x*2.0f);
        int y = (int)(lastPos.z * 2.0f);
        GameObject sp = _snowPatches[x, y];
        sp.transform.localScale -= new Vector3(0.0f, 0.1f, 0.0f);
        if (sp.transform.localScale.y <= 0.15f)
        {
            sp.SetActive(false);
        }
    }
}
