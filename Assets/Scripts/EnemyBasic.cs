//using System.Collections;
//using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.UI;

public enum AI_TYPES
{
    NONE,
    RANDOM,
    ABILITY1,
    ABILITY2,
    ABILITY3,
    ABILITY4,
    PATTERN1,
    PATTERN2,
    PATTERN3,
    OPTIMIZED
}

public enum ENEMY_TYPES
{
    NONE,
    ENEMY_BASIC,
    ENEMY_MELEE1,
    ENEMY_MELEE2,
    ENEMY_RANGED1,
    ENEMY_RANGED2,
    ENEMY_RANGED3
}

public class Enemy : BaseClass
{
    public GameObject typeText;
    public Enemy(ENEMY_TYPES type = ENEMY_TYPES.ENEMY_BASIC, int hp = 100, int stam = 50)
    {
        enemyType = type;

        base.Start();

        restart:
        foreach (Enemy enemy in Players.enemies)
        {
            if (pos == enemy.pos)
            {
                pos = (POS) ((int) pos + 1);
                pos = (POS) ((int)pos % 3);
                //pos = (POS) (((int) pos + 1) % (int)POS.E_FRONT_LEFT);
                goto restart;
            }
        }
        // if (type >= ENEMY_TYPES.ENEMY_RANGED1)
        // {
        //     foreach (Enemy enemy in Players.enemies)
        //     {
        //         if (pos == enemy.pos)
        //         {
        //             pos = (POS)(((int) pos + 1) % (int)POS.P_FRONT_LEFT) + 3;
        //             goto restart;
        //         }
        //     }
        // }
        // else
        // {
        //     foreach (Enemy enemy in Players.enemies)
        //     {
        //         if (pos == enemy.pos)
        //         {
        //             pos = (POS) ((int) pos + 1);
        //             pos = (POS) ((int)pos % 3);
        //             //pos = (POS) (((int) pos + 1) % (int)POS.E_FRONT_LEFT);
        //             goto restart;
        //         }
        //     }
        // }

        //typeSignifier = Instantiate(DEFINE.staticText, CombatSystem.PosToVec(pos), Quaternion.identity,
        //    canvas.transform);
        healthbar = new Healthbar(pos, hp, stam);

        //Text typeText = typeSignifier.GetComponent<Text>();
        //typeText.color = new Color(255, 215, 0);

        if (enemyType <= ENEMY_TYPES.ENEMY_MELEE2)
        {
            enemyPrefab = (GameObject) Resources.Load("Prefabs/EnemyMelee");
        }
        else
        {
            enemyPrefab = (GameObject) Resources.Load("Prefabs/EnemyRanged");
        }
        
        typeText = Instantiate((GameObject)Resources.Load("Prefabs/StaticText"), CombatSystem.PosToVec(pos) + new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
        typeText.GetComponent<Text>().color = Color.yellow;

        player = Instantiate(enemyPrefab, CombatSystem.PosToVec(pos) + new Vector3(0, 0, -5), Quaternion.identity);
        
        switch (enemyType)
        {
            case ENEMY_TYPES.ENEMY_BASIC:
                t1 = new Timer(300, 150);
                typeText.GetComponent<Text>().text = "B";
                // typeText.text = "B";
                break;
            
            case ENEMY_TYPES.ENEMY_MELEE1:
                t1 = new Timer(240, 120);
                t2 = new Timer(200, 80);
                typeText.GetComponent<Text>().text = "M1";
                // typeText.text = "M1";
                break;
            
            case ENEMY_TYPES.ENEMY_MELEE2:
                t1 = new Timer(150, 50);
                t3 = new Timer(300, 80);
                typeText.GetComponent<Text>().text = "M2";
                // typeText.text = "M2";
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED1:
                RangedT1 = new Timer(300, 100);
                typeText.GetComponent<Text>().text = "R1";
                // typeText.text = "R1";
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED2:
                RangedT1 = new Timer(200, 80);
                RangedT2 = new Timer(250, 80);
                typeText.GetComponent<Text>().text = "R2";
                // typeText.text = "R2";
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED3:
                RangedT1 = new Timer(150, 80);
                RangedT2 = new Timer(200, 80);
                typeText.GetComponent<Text>().text = "R3";
                // typeText.text = "R3";
                break;
        }
        
        
        // t2 = new Timer(360, 50);
        // t3 = new Timer(600, 50);
        //
        // RangedT1 = new Timer(240, 40);
        // RangedT2 = new Timer(360, 40);
        // RangedT3 = new Timer(420, 40);
    }

    public override string ToString(float Matchtime)
    {
        string data = String.Empty;
        data += $"{enemyType.ToString()} ";

        return data;
    }

    // Update is called once per frame
    public override void UpdateObject()
    {
        CheckEnemyEffects();

        if (nextAbility.timer.isDone)
        {
            nextAbility.Activate();
            
            //Reset Effects After attack
            blinded = false;
        }

        switch (enemyType)
        {
            case ENEMY_TYPES.ENEMY_BASIC:
                UpdateBasic();
                break;
            
            case ENEMY_TYPES.ENEMY_MELEE1:
                UpdateMelee1();
                break;
            
            case ENEMY_TYPES.ENEMY_MELEE2:
                UpdateMelee2();
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED1:
                UpdateRanged1();
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED2:
                UpdateRanged2();
                break;
            
            case ENEMY_TYPES.ENEMY_RANGED3:
                UpdateRanged3();
                break;
        }

        healthbar.Update(nextAbility.timer);
        base.UpdateObject();
    }

    ~Enemy()
    {
        healthbar.Destroy();
        Destroy(player);
    }

    private void UpdateBasic()
    {
        if (!nextAbility.queued)
        {
            nextAbility.Queue(Ability1, t1);
            t1.Reset();
        }
    }
    
    private void UpdateMelee1()
    {
        if (healthbar.stamina < 10 && !nextAbility.queued)
        {
            nextAbility.Queue(Ability1, t1);
            t1.Reset();
        }
        else if (!nextAbility.queued)
        {
            nextAbility.Queue(Ability2, t2);
            t2.Reset();
        }
    }
    
    private void UpdateMelee2()
    {
        if (healthbar.stamina < 20 && !nextAbility.queued)
        {
            nextAbility.Queue(Ability1, t1);
            t1.Reset();
        }
        else if (!nextAbility.queued)
        {
            nextAbility.Queue(Ability3, t3);
            t3.Reset();
        }
    }
    
    private void UpdateRanged1()
    {
        if (!nextAbility.queued)
        {
            nextAbility.Queue(RangedAbility1, RangedT1);
            RangedT1.Reset();
        }
    }
    
    private void UpdateRanged2()
    {
        if (healthbar.stamina < 10 && !nextAbility.queued)
        {
            nextAbility.Queue(RangedAbility1, RangedT1);
            RangedT1.Reset();
        }
        else if (!nextAbility.queued)
        {
            nextAbility.Queue(RangedAbility2, RangedT2);
            RangedT2.Reset();
        }
    }
    
    private void UpdateRanged3()
    {
        if (healthbar.stamina < 20 && !nextAbility.queued)
        {
            nextAbility.Queue(RangedAbility1, RangedT1);
            RangedT1.Reset();
        }
        else if (!nextAbility.queued)
        {
            nextAbility.Queue(RangedAbility2, RangedT2);
            RangedT2.Reset();
        }
    }
    
    public override void Ability1()
    {
        if (TargetStamina(+5, pos))
        {
            TargetHealth(-5 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t1.Reset();
        }
    }
    
    public override void Ability2()
    {
        if (TargetStamina(-10, pos))
        {
            TargetHealth(-15 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t2.Reset();
        }
    }

    public override void Ability3()
    {
        if (TargetStamina(-20, pos))
        {
            TargetHealth(-40 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t2.Reset();
        }
    }

    public void RangedAbility1()
    {
        if (TargetStamina(+5, pos))
        {
            TargetHealth(-5 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t1.Reset();
        }
    }

    public void RangedAbility2()
    {
        if (TargetStamina(-10, pos))
        {
            TargetHealth(-15 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t2.Reset();
        }
    }

    public void RangedAbility3()
    {
        if (TargetStamina(-20, pos))
        {
            TargetHealth(-40 + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.resistIndex]) : 0), false, POS.P_FRONT_CENTER, POS.P_BACK_CENTER);
            t2.Reset();
        }
    }
}
