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

    private List<GameObject> _snowPatches;
    private List<SnowBall> _snowBalls = new List<SnowBall>();

    private const float k_patchSize = 0.5f;
    private const int k_size = 200;
    void Awake()
    {
        SnowManager._instance = this;
        _snowPatches = new List<GameObject>();
        for (float i = -k_size * 0.5f; i < k_size * 0.5f; i += k_patchSize)
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

    public bool UpdateSnow(Vector3 lastPos, Vector3 pos, Collider snowPatchCollider)
    {
        lastPos.y = pos.y;
        if (Vector3.Distance(lastPos, pos) < 0.01f)
        {
            return false;
        }
        GameObject sp = snowPatchCollider.transform.parent.gameObject;
        if(sp.transform.localScale.y <= 0.15f)
        {
            return false;
        }
        sp.transform.localScale -= new Vector3(0.0f, 0.02f, 0.0f);
        if (sp.transform.localScale.y <= 0.15f)
        {
            sp.SetActive(false);
        }
        return true;

    }
}
