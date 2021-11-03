using UnityEngine;

public class Timer
{
    public float duration;
    public float time;
    public bool isDone = true;
    public bool paused = false;
    public float threshold;

    public Timer(float timer, float thresh = 0)
    {
        duration = timer;
        time = timer;
        threshold = thresh;
    }
    
    public void Start()
    {
        isDone = false;
        paused = false;

        if (time <= 0)
            isDone = true;
    }

    public void Stop()
    {
        paused = true;
    }

    public void Update()
    {
        if (time <= 0)
            isDone = true;
        
        if (!isDone && !paused)
            time -= Time.timeScale;
    }

    public bool inThreshold()
    {
        return time < threshold;
    }

    public void Reset()
    {
        time = duration;
        isDone = false;
    }
}
