using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour
{

	void Update ()
    {
	    if(Input.anyKey)
        {
            Application.LoadLevel(1);
        }
	}
}
