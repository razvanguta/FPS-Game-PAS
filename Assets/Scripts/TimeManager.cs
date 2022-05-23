using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.3f;
	public float slowdownLength = 3f;

    public bool slowMoActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale != 1f && slowMoActive) {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
		    Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }
        else
            slowMoActive = false;
    }

    public void DoSlowmotion ()
	{
        Time.timeScale = slowdownFactor;
		Time.fixedDeltaTime = Time.timeScale * .02f;
        slowMoActive = true;   
	}
}
