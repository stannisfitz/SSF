using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public float TotalTime = 120.0f;

    public float EndTime = 30.0f;

    private float _timer;

    private int _pointsTeam1 = -1;
    private int _pointsTeam2 = -1;

    private int _numPlayers = 0;
    public int NumPlayer
    {
        get { return _numPlayers; }
    }


    public int AddPlayer()
    {
        return _numPlayers++;
    }

    void Awake ()
    {
        GameManager._instance = this;
        _timer = TotalTime;
	}
	
	void Update ()
    {
        if (_pointsTeam1 < 0)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0.0f)
            {
                _timer = 0.0f;
                Time.timeScale = 0.0f;
                SnowManager.Instance.GetPoints(out _pointsTeam1, out _pointsTeam2);
                _timer = EndTime;
            }
        }
        else
        {
            _timer -= Time.fixedDeltaTime;
            if (_timer <=EndTime*0.75f && Input.anyKeyDown)
            {
                Time.timeScale = 1.0f;
                Application.LoadLevel(1);
            }
            if (_timer <= 0.0f)
            {
                Time.timeScale = 1.0f;
                Application.LoadLevel(1);
            }
        }
    }

    void OnGUI()
    {
        GUI.color = Color.red;

        if (_pointsTeam1 < 0)
        {
            GUI.Label(new Rect(Screen.width * 0.45f, Screen.height * 0.01f, Screen.width * 0.25f, Screen.height * 0.25f), ("Time: " + ((int)_timer) / 60 + ":" + ((int)_timer) % 60));
        }
        else
        {
            GUI.Label(new Rect(Screen.width * 0.45f, Screen.height * 0.01f, Screen.width * 0.25f, Screen.height * 0.25f), ("Team1: " + _pointsTeam1 + " Team2: " + _pointsTeam2));
        }
    }
}
