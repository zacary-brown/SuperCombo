using System;
using UnityEngine;

public static class FastMatch
{
    public static int iterations = 0;
    public static int matchup = 0;
}

public class Match
{
    public float timer = 0;
    public bool isDone = false;
    public bool serialized = false;
    private int PlayerWin;

    private bool FastMode;

    public Match(bool fast, AI_TYPES aitype, params ENEMY_TYPES[] enemyTypes)
    {
        FastMode = fast;
        
        if (FastMode == false)
            Players.party.Add(new Warrior(POS.P_FRONT_CENTER, true, aitype));
        else
            Players.party.Add(new Warrior(POS.P_FRONT_CENTER, false, aitype));
        
        // Players.enemies.Add(new Enemy(enemyTypes[0]));
        
        foreach(ENEMY_TYPES type in enemyTypes)
            Players.enemies.Add(new Enemy(type));
    }

    // Update is called once per frame
    public void UpdateObject()
    {
        if (!(Game.Pause || isDone))
        {
            //Update Players if not dead
            foreach (BaseClass member in Players.party)
                if (!member.isDead)
                    member.UpdateObject();

            //Update Enemies if not dead
            foreach (BaseClass member in Players.enemies)
                if (!member.isDead)
                    member.UpdateObject();

            timer += Time.deltaTime;
        }

        if (!isDone)
        {
            //Check to see if the player has died
            if (Players.party[0].isDead)
            {
                isDone = true;
                ++FastMatch.iterations;
            }

            int deadCount = 0;

            //Count dead enemies
            foreach (BaseClass member in Players.enemies)
            {
                if (member.isDead)
                    ++deadCount;
                else
                    break;
            }

            //Check if all enemies are killed
            if (deadCount == Players.enemies.Count)
            {
                ++Players.party[0].wins;
                isDone = true;
                ++FastMatch.iterations;
                
                //If player wins, give them a random Item
                bag.inventory.addItem();
            }
        }
    }

    public override string ToString()
    {
        string data = String.Empty;
        float enemyDps = 0;

        //Player Data
        foreach (BaseClass member in Players.party)
            data += member.ToString(timer);

        //Enemy Data
        foreach (BaseClass member in Players.enemies)
        {
            enemyDps += member.damage;
            data += member.ToString(timer);
        }

        //Enemy DPS
        data += "," + (enemyDps / timer).ToString() + ",";
        
        //Match time
        data += $"{timer.ToString()}";

        return data;
    }
}
