using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyBasicInfo
{
    
}

public class Ability
{
    public Action ability;
    public Timer timer;
    public bool queued = false;
    public string name;
    public string desc;
    public int damage;
    public int cost;
    public int range;

    public void Queue(Action ab, Timer t)
    {
        ability = ab;
        timer = t;
        queued = true;
        timer.Reset();
    }

    public void Activate()
    {
        ability.Invoke();
        queued = false;
        timer.Reset();
    }
}
