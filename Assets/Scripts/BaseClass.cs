using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class BaseClass : MonoBehaviour
{
    [SerializeField]
    public GameObject damageNumberPrefab /*= (GameObject)Resources.Load("Prefabs/DamageNumber")*/;
    public GameObject canvas /*= GameObject.Find("Canvas")*/;
    public GameObject playerPrefab /*= (GameObject) Resources.Load("Prefabs/Player")*/;
    public GameObject enemyPrefab /*= (GameObject) Resources.Load("Prefabs/Enemy")*/;
    public GameObject txtPrefab /*= (GameObject) Resources.Load("Prefabs/DamageNumber")*/;
    //public GameObject typeSignifier;
    
    public bool isDead = false;
    public bool Active;
    public KeyCode key;
    public POS pos = POS.E_FRONT_CENTER;
    public GameObject player;
    public Healthbar healthbar;
    public Ability nextAbility;

    //Effects
    public bool hurt;
    public bool parry;
    public bool blinded;
    // private int comboMeter = 0;

    //Effects Timers
    public Timer interuptTimer = new Timer(120);
    public Timer parryTimer = new Timer(60);
    public Timer blockTimer = new Timer(60);

    //Ability Timers
    public Timer t1;
    public Timer t2;
    public Timer t3;
    public Timer t4;
    
    public Timer RangedT1;
    public Timer RangedT2;
    public Timer RangedT3;

    //Stats
    public string myName = "Joe";
    public int wins;
    public int minDamange = 100;
    public int maxDamage = 0;
    public int damage = 0;
    
    //Player/Enemy Types
    public AI_TYPES type = AI_TYPES.NONE;
    public ENEMY_TYPES enemyType = ENEMY_TYPES.NONE;

    public int ability = 0;

    public virtual void Start()
    {
        playerPrefab = (GameObject) Resources.Load("Prefabs/Player");
        enemyPrefab = (GameObject) Resources.Load("Prefabs/Enemy");
        txtPrefab = (GameObject) Resources.Load("Prefabs/DamageNumber");
        damageNumberPrefab = (GameObject) Resources.Load("Prefabs/DamageNumber");
        canvas = GameObject.Find("Canvas");
        
        nextAbility = new Ability();
        nextAbility.timer = new Timer(120);
        
        nextAbility.timer.Start();
        
        t1 = new Timer(60);
        t2 = new Timer(120);
        t3 = new Timer(120);
        t4 = new Timer(240);
        
        RangedT1 = new Timer(0);
        RangedT2 = new Timer(0);
        RangedT3 = new Timer(0);
    }

    public virtual void UpdateObject()
    {
        if (healthbar.health <= 0 && !isDead)
        {
            healthbar.Destroy();
            Destroy(player);

            isDead = true;
        }

        //Reset effects at the end of object update
        hurt = false;
        
        ReduceTimers();
    }

    ~BaseClass()
    {
        healthbar.Destroy();
        Destroy(player);
    }

    public virtual string ToString(float Matchtime)
    {
        string data = String.Empty;
        data += $"Player,{myName},";
        data += $"MinDamage,{minDamange.ToString()},";
        data += $"MaxDamage,{maxDamage.ToString()},";
        data += $"AI_Type,{type.ToString()},";
        data += $"Wins,{wins.ToString()},";
        
        return data;
    }

    public void UpdateRandom()
    {
        int random = UnityEngine.Random.Range(1, 4 + 1);
        
        if (random == 1)
            Ability1();
        else if (random == 2)
            Ability2();
        else if (random == 3)
            Ability3();
        else
            Ability4();
    }

    public void UpdateAbility1()
    {
        Ability1();
    }

    public void UpdateAbility2()
    {
        Ability2();
    }
    
    public void UpdateAbility3()
    {
        Ability3();
    }

    public void UpdateAbility4()
    {
        Ability4();
    }
    
    public void UpdatePattern1()
    {
        Ability1();
        Ability2();
    }

    public void UpdatePattern2()
    {
        Ability1();
        Ability2();
        Ability4();
    }
    
    public void UpdatePattern3()
    {
        Ability1();
        Ability2();
        Ability3();
    }

    public void UpdateOptimized()
    {
        Enemy enemy = CombatSystem.inThreshold();

        if (enemy.nextAbility.timer.inThreshold())
        {
            if (t1.isDone && enemy.enemyType < ENEMY_TYPES.ENEMY_RANGED1)
                Ability1();
            else if (t2.isDone && enemy.enemyType >= ENEMY_TYPES.ENEMY_RANGED1)
                Ability2();
            else if (t4.isDone)
                Ability4();
            else if (healthbar.health < 30)
                Ability3();
        }
        else
        {
            if (enemy.nextAbility.timer.time - t1.duration > 0 && enemy.enemyType < ENEMY_TYPES.ENEMY_RANGED1)
                Ability1();
            else if (enemy.nextAbility.timer.time - t2.time > 0 && enemy.enemyType >= ENEMY_TYPES.ENEMY_RANGED1)
                Ability2();
        }
    }

    public void Interupt()
    {
        if (interuptTimer.isDone)
        {
            interuptTimer.Reset();
            
            nextAbility.timer.Reset();
            nextAbility.timer.Stop();
        }
    }

    public void CheckEnemyEffects()
    {
        if (hurt && nextAbility.timer.inThreshold())
        {
            Interupt();
        }
        
        if (interuptTimer.isDone)
        {
            nextAbility.timer.Start();
        }
    }

    public virtual void Ability1()
    {
        string message = $"{myName}, Default Ability1";
        Debug.Log(message);
    }

    public virtual void Ability2()
    {
        string message = $"{myName}, Default Ability2";
        Debug.Log(message);
    }

    public virtual void Ability3()
    {
        string message = $"{myName}, Default Ability3";
        Debug.Log(message);
    }

    public virtual void Ability4()
    {
        string message = $"{myName}, Default Ability4";
        Debug.Log(message);
    }

    public Transform MoveUp(Transform transform)
    {
        if (pos < POS.P_BACK_LEFT) return transform;
        
        pos -= 3;
        transform.Translate(0, DEFINE.OFFSET, 0);

        return transform;
    }

    public Transform MoveDown(Transform transform)
    {
        if (pos > POS.P_FRONT_RIGHT) return transform;
        
        pos += 3;
        transform.Translate(0, - DEFINE.OFFSET, 0);
            
        return transform;
    }
    
    public Transform MoveLeft(Transform transform)
    {
        if (pos == POS.P_FRONT_LEFT || pos == POS.P_BACK_LEFT) return transform;
        
        pos -= 1;
        transform.Translate( - DEFINE.OFFSET, 0, 0);
        
        return transform;
    }
    
    public Transform MoveRight(Transform transform)
    {
        if (pos == POS.P_FRONT_RIGHT || pos == POS.P_BACK_RIGHT) return transform;
        
        pos += 1;
        transform.Translate(DEFINE.OFFSET, 0, 0);

        return transform;
    }

    public void ReduceTimers()
    {
        if (!t1.isDone)
            t1.Update();

        if (!t2.isDone)
            t2.Update();

        if (!t3.isDone)
            t3.Update();

        if (!t4.isDone)
            t4.Update();
        
        if (!RangedT1.isDone)
            RangedT1.Update();
        
        if (!RangedT2.isDone)
            RangedT2.Update();
        
        if (!RangedT3.isDone)
            RangedT3.Update();
        
        if (!interuptTimer.isDone)
            interuptTimer.Update();
        
        if (!blockTimer.isDone)
            blockTimer.Update();
        
        if (!parryTimer.isDone)
            parryTimer.Update();
    }

    public void TargetEnemy(int value, bool blind, bool ranged = false)
    {
        BaseClass enemy = null;
        float time = 10000;
        
        //Check for the enemy
        foreach (BaseClass member in Players.enemies)
        {
            if (!member.isDead && member.nextAbility.timer.time < time)
            {
                time = member.nextAbility.timer.time;
                enemy = member;
            }
        }
        
        if (!ranged && enemy.enemyType >= ENEMY_TYPES.ENEMY_RANGED1)
        {
            AttackMiss(enemy);
        }
        else
        {
            ChangeHealth(enemy, value, blind);
        }
    }
    
    public void TargetHealth(int value, bool blind, params object[] positions)
    {
        foreach (POS pos in positions)
        {
            //Check for the player
            foreach (BaseClass member in Players.party)
            {
                if (member.pos == pos)
                {
                    ChangeHealth(member, value, blind);
                    break;
                }
            }
            
            //Check for the enemy
            foreach (BaseClass member in Players.enemies)
            {
                if (member.pos == pos)
                {
                    ChangeHealth(member, value, blind);
                    break;
                }
            }
        }
    }

    public bool TargetStamina(int value, params object[] positions)
    {
        foreach (POS pos in positions)
        {
            //Check for the player
            foreach (BaseClass member in Players.party)
            {
                if (member.pos == pos && !member.isDead)
                {
                    if (!ChangeStamina(member, value))
                    {
                        return false;
                    }
                    
                    break;
                }
            }
            
            //Check for the Enemies
            foreach (BaseClass member in Players.enemies)
            {
                if (member.pos == pos && !member.isDead)
                {
                    if (!ChangeStamina(member, value))
                    {
                        return false;
                    }

                    break;
                }
            }
        }
        
        return true;
    }

    public void ChangeHealth(BaseClass member, int value, bool blind = false)
    {
        if (member.blockTimer.isDone && member.parryTimer.isDone)
        {
            member.blinded = blind;

            if (blinded)
                value = 0;
            
            //Change Value of Health when attacked
            member.healthbar.health += value;
            
            //Indicate that the enemy/player has been hurt so they can resolve it any way they want
            if (value != 0)
                member.hurt = true;
            
            //Visuals
            CreateDamageNumber(member, value);
            
            //Add to damage Stat
            damage += (-1 * value);
        }
        else if (!member.blockTimer.isDone)
        {
            //TODO BLOCK FEEDBACK
        }
        else if (!member.parryTimer.isDone)
        {
            value *= (bag.item != null ? Int32.Parse(bag.item.array[bag.item.damage3Index]) : 1);
            //Reflect damage back to itself
            healthbar.health += value;
            hurt = true;

            //Add to attackers damage stat if damage reflected
            member.damage += -1 * value;

            //Visuals
            CreateDamageNumber(this, value);
            
            //TODO PARRY FEEDBACK
            
            //Track Damage stats
            if (-1 * value > member.maxDamage)
                member.maxDamage = value * -1;

            if (-1 * value < member.minDamange)
                member.minDamange = value * -1;

            return;
        }

        //Track Damage stats
        if (-1 * value > maxDamage)
            maxDamage = value * -1;

        if (-1 * value < minDamange)
            minDamange = value * -1;
    }

    private void CreateDamageNumber(BaseClass member, int value)
    {
        GameObject damageNumber = Instantiate(txtPrefab, CombatSystem.PosToVec(member.pos) + new Vector3(-1, 0), Quaternion.identity, canvas.transform);
        Text text = damageNumber.GetComponent<Text>();

        text.text = value.ToString();
        Destroy(damageNumber, 0.5f);
    }
    
    public void AttackMiss(BaseClass member)
    {
        GameObject damageNumber = Instantiate(txtPrefab, CombatSystem.PosToVec(member.pos) + new Vector3(-1, 0), Quaternion.identity, canvas.transform);
        Text text = damageNumber.GetComponent<Text>();

        text.text = "Miss!";
        Destroy(damageNumber, 0.5f);
    }

    public bool ChangeStamina(BaseClass member, int value)
    {
        GameObject damageNumber = Instantiate(txtPrefab, CombatSystem.PosToVec(member.pos) + new Vector3(1, 0), Quaternion.identity, canvas.transform);
        Text text = damageNumber.GetComponent<Text>();
        damageNumber.GetComponent<Text>().color = Color.green;

        if (member.healthbar.stamina + value < 0)
        {
            text.text = "No Stamina!";
            Destroy(damageNumber, 0.5f);
            return false;
        }
        
        if (value > 0)
        {
            text.text = $"+{value.ToString()}";
        }
        else
        {
            text.text = value.ToString();
        }
        member.healthbar.stamina += value;

        Destroy(damageNumber, 0.5f);

        return true;
    }
}

public class Warrior : BaseClass
{
    private Ability ability1;
    private Ability ability2;
    private Ability ability3;
    private Ability ability4;
    public GameObject comboText;
    public int comboMeter = 0;
    public int maxCombo = 0;
    public Warrior(POS position = POS.P_FRONT_CENTER, bool active = false, AI_TYPES aitype = AI_TYPES.NONE, int hp = 100, int stam = 50)
    {
        pos = position;

        base.Start();

        healthbar = new Healthbar(pos, hp + (bag.item != null ? Int32.Parse(bag.item.array[bag.item.hpIndex]) : 0), stam);
        t1 = new Timer(90);
        t2 = new Timer(180);
        t3 = new Timer(120);
        t4 = new Timer(360);

        comboText = Instantiate((GameObject) Resources.Load("Prefabs/ComboMeter"),
            CombatSystem.PosToVec(pos) + new Vector3(0, -DEFINE.OFFSET * 1.5f),
                    Quaternion.identity,
                    canvas.transform);
                    
        // comboText.GetComponent<Text>().color = Color.yellow;

        Active = active;
        enemyType = ENEMY_TYPES.NONE;
        
        if (aitype != AI_TYPES.NONE)
        {
            player = Instantiate((GameObject)Resources.Load("Prefabs/PlayerAI"), CombatSystem.PosToVec(pos), Quaternion.identity);
            type = aitype;
        }
        else
        {
            player = Instantiate(playerPrefab, CombatSystem.PosToVec(pos), Quaternion.identity);
        }
    }

    ~Warrior()
    {
        Destroy(player);
        Destroy(comboText);
    }

    public override void UpdateObject()
    {
        switch (type)
        {
            case AI_TYPES.NONE :
                UpdatePlayer();
                break;
                    
            case AI_TYPES.RANDOM :
                UpdateRandom();
                break;
            
            case AI_TYPES.ABILITY1 :
                UpdateAbility1();
                break;
            
            case AI_TYPES.ABILITY2 :
                UpdateAbility2();
                break;
            
            case AI_TYPES.ABILITY3 :
                UpdateAbility3();
                break;
            
            case AI_TYPES.ABILITY4 :
                UpdateAbility4();
                break;
            
            case AI_TYPES.PATTERN1 :
                UpdatePattern1();
                break;
            
            case AI_TYPES.PATTERN2 :
                UpdatePattern2();
                break;
            
            case AI_TYPES.PATTERN3 :
                UpdatePattern3();
                break;
            
            case AI_TYPES.OPTIMIZED :
                UpdateOptimized();
                break;
        }
        
        UpdateComboMeter();
        healthbar.Update();
        base.UpdateObject();
    }

    public void UpdatePlayer()
    {
        CombatSystem.CheckAbilityInput(this);
        CombatSystem.CheckMoveInput(this);
        
        //Check Confirmation
        if (Input.GetKeyDown(key))
        {
            Game.actionDict[key].Invoke(this);
        }
    }

    public void UpdateComboMeter()
    {
        if (hurt)
            comboMeter = 0;
        
        comboText.GetComponent<Text>().text = comboMeter.ToString();
    }
    
    public override string ToString(float Matchtime)
    {
        string data = String.Empty;
        data += $"{type.ToString()},";
        data += $"{minDamange.ToString()},";
        data += $"{maxDamage.ToString()},";
        data += $"{(damage / Matchtime).ToString()},";
        data += $"{maxCombo},";
        data += $"{wins.ToString()},";
        
        return data;
    }

    public override void Ability1()
    {
        if (t1.isDone && (bag.item != null ? TargetCombo(+(bag.item.array[bag.item.nameIndex] == "Gold Medal" ? 2 : 1)) : TargetCombo(+1)))
        {
            //Every multiple of 5 combo multiplies player damage by multiplier + 1
            int value = -2 * (comboMeter / 5 + 1) - (bag.item != null ? Int32.Parse(bag.item.array[bag.item.damage1Index]) : 0);
            TargetEnemy(value, false);
            // TargetHealth(value, false, POS.E_FRONT_CENTER, POS.E_BACK_CENTER);
            t1.Reset();
        }

        if (comboMeter > maxCombo)
            maxCombo = comboMeter;

        //base.Ability1();
    }
    
    public override void Ability2()
    {
        if (t2.isDone && (bag.item != null ? TargetCombo(+(bag.item.array[bag.item.nameIndex] == "Gold Medal" ? 2 : 1)) : TargetCombo(+1)))
        {
            TargetEnemy(-5 - (bag.item != null ? Int32.Parse(bag.item.array[bag.item.damage2Index]) : 0), false, true);
            //TargetHealth(-5, false, POS.E_FRONT_CENTER, POS.E_BACK_CENTER);
            t2.Reset();
        }
        
        if (comboMeter > maxCombo)
            maxCombo = comboMeter;

        //base.Ability2();
    }

    public override void Ability3()
    {
        if (t3.isDone && TargetCombo(-comboMeter))
        {
            //TargetHealth(-30, POS.E_FRONT_CENTER, POS.E_BACK_CENTER);
            parry = true;
            parryTimer.Reset();
            t3.Reset();
        }

        //base.Ability3();
    }

    public override void Ability4()
    {
        if (t4.isDone)
        {
            TargetEnemy(-10 - (bag.item != null ? Int32.Parse(bag.item.array[bag.item.damage4Index]) : 0), true, true);
            // TargetHealth(-10, true, POS.E_FRONT_CENTER, POS.E_BACK_CENTER);
            t4.Reset();
        }

        //base.Ability4();
    }
    
    public bool TargetCombo(int value)
    {
        if (comboMeter >= -1 * value)
        {
            comboMeter += value;
            return true;
        }
        
        return false;
    }
}
