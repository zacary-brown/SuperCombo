using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

static class DEFINE
{
    public const float OFFSET = 1.0f;
    public static GameObject staticText = (GameObject) Resources.Load("Prefabs/StaticText");
}

class Game
{
    //public CombatSystem system = new CombatSystem();
    public static bool Pause = false;
    public static bool FastMode = false;
    public static float TimeScale = 1.0f;
    public static List<string> MatchData = new List<string>();
    public static Dictionary<KeyCode, Action<BaseClass>> actionDict;
}

public enum POS
{
    E_BACK_LEFT = 0,
    E_BACK_CENTER = 1,
    E_BACK_RIGHT = 2,
    E_FRONT_LEFT = 3,
    E_FRONT_CENTER = 4,
    E_FRONT_RIGHT = 5,
    P_FRONT_LEFT = 6,
    P_FRONT_CENTER = 7,
    P_FRONT_RIGHT = 8,
    P_BACK_LEFT = 9,
    P_BACK_CENTER = 10,
    P_BACK_RIGHT = 11
}

public static class Players
{
    public static List<BaseClass> party = new List<BaseClass>();
    public static List<BaseClass> enemies = new List<BaseClass>();
}

public class CombatSystem : MonoBehaviour
{
    [SerializeField]
    public GameObject damageNumberPrefab;
    public static GameObject canvas;
    public GameObject AbilitTimer1;
    public GameObject AbilitTimer2;
    public GameObject AbilitTimer3;
    public GameObject AbilitTimer4;

    private Match match;
    
    float dt = 0;
    private float fixeddt;

    // Start is called before the first frame update
    void Start()
    {
        fixeddt = Time.fixedDeltaTime;
        //Finding Prefabs
        damageNumberPrefab = (GameObject)Resources.Load("Prefabs/DamageNumber");
        canvas = GameObject.Find("Canvas");
        
        match = new Match(false, AI_TYPES.NONE, ENEMY_TYPES.ENEMY_BASIC);
        Game.actionDict = new Dictionary<KeyCode, Action<BaseClass>>();
        bag.inventory = new Inventory();
        
        //Add new player actions to the dictionary
        Game.actionDict.Add(KeyCode.Alpha1, ActivateAbility1);
        Game.actionDict.Add(KeyCode.Alpha2, ActivateAbility2);
        Game.actionDict.Add(KeyCode.Alpha3, ActivateAbility3);
        Game.actionDict.Add(KeyCode.Alpha4, ActivateAbility4);
        
        Game.actionDict.Add(KeyCode.UpArrow, ActivateMoveUp);
        Game.actionDict.Add(KeyCode.DownArrow, ActivateMoveDown);
        // Game.actionDict.Add(KeyCode.LeftArrow, ActivateMoveLeft);
        // Game.actionDict.Add(KeyCode.RightArrow, ActivateMoveRight);

        //Parse Abilities for Characters
        ParseAbilityData();
    }

    // Update is called once per frame
    void Update()
    {
        match.UpdateObject();

        if (match.isDone)
        {
            if (!match.serialized)
            {
                dt = 0;
                Game.MatchData.Add(match.ToString());
                match.serialized = true;
            }

            dt += Time.timeScale;
            
            if (dt / 60 > 4 && !Game.FastMode)
                Restart();
            
            if (Game.FastMode)
                Restart();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
            
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Serialize();
            Application.Quit();
        }

        //FAST AND SLOW MODES
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Game.FastMode)
                FastMode();
            else
                ShowMode();

            Time.fixedDeltaTime = fixeddt * Time.deltaTime;
        }
        
        //STOP TIME
        if (Input.GetKeyDown(KeyCode.G))
        {
            Pause();
        }

        UpdateTimers(Players.party[0]);
    }

    private void Serialize()
    {
        foreach (string data in Game.MatchData)
        {
            File.AppendAllText("Assets/Resources/Telemetry/rawdata.csv", data + Environment.NewLine);
        }
    }

    private void FastMode()
    {
        Game.TimeScale = 100.0f;
        Time.timeScale = 100.0f;

        Game.FastMode = true;
        
        Time.fixedDeltaTime = fixeddt * Time.deltaTime;
        
        Restart();
    }

    private void ShowMode()
    {
        Game.TimeScale = 1.0f;
        Time.timeScale = 1.0f;
        Game.FastMode = false;
        
        Time.fixedDeltaTime = fixeddt * Time.deltaTime;
        
        Restart();
    }

    private void Pause()
    {
        //Pause game if not pause, otherwise reset timeScale to previous value before pause
        Time.timeScale = !Game.Pause ? 0.0f : Game.TimeScale;
        
        Game.Pause = !Game.Pause;
        
        Time.fixedDeltaTime = fixeddt * Time.deltaTime;
    }
    
    public void Restart()
    {
        AI_TYPES type = Players.party[0].type;
        
        foreach (BaseClass member in Players.party)
        {
            member.healthbar.Destroy();
            Destroy(((Warrior)member).comboText);
            Destroy(member.player);
        }

        foreach (BaseClass member in Players.enemies)
        {
            member.healthbar.Destroy();
            Destroy(((Enemy)member).typeText);
            Destroy(member.player);
        }

        Players.party.Clear();
        Players.enemies.Clear();

        Array values = Enum.GetValues(typeof(AI_TYPES));
            
        Debug.Log(((AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)).ToString());
        
        if (Game.FastMode)
            FastMatch.matchup = (FastMatch.iterations / 9) % 6;
        
        switch (FastMatch.matchup)
        {
            //Group matches --------------------------------------------------------------------------------------------
            
            case 0:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_BASIC);
                break;
                
            case 1:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_RANGED1);
                break;
                
            case 2:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_BASIC, ENEMY_TYPES.ENEMY_RANGED2);
                break;
                
            case 3:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_BASIC, ENEMY_TYPES.ENEMY_MELEE1);
                break;
                
            case 4:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_BASIC, ENEMY_TYPES.ENEMY_MELEE1, ENEMY_TYPES.ENEMY_MELEE2);
                break;
                
            case 5:
                match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
                                    ENEMY_TYPES.ENEMY_BASIC, ENEMY_TYPES.ENEMY_MELEE2, ENEMY_TYPES.ENEMY_RANGED3);
                break;
            
            //Single matches -------------------------------------------------------------------------------------------
            
            // case 0:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_BASIC);
            //     break;
            //     
            // case 1:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_MELEE1);
            //     break;
            //     
            // case 2:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_MELEE2);
            //     break;
            //     
            // case 3:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_RANGED1);
            //     break;
            //     
            // case 4:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_RANGED2);
            //     break;
            //     
            // case 5:
            //     match = new Match(Game.FastMode, Game.FastMode ? AI_TYPES.OPTIMIZED/*(AI_TYPES)values.GetValue((FastMatch.iterations % 9) + 1)*/ : type, 
            //         ENEMY_TYPES.ENEMY_RANGED3);
            //     break;
        }
    }

    public static Enemy inThreshold()
    {
        Enemy enemy = null;
        float time = 10000;

        //Check for the enemy
        foreach (Enemy member in Players.enemies)
        {
            if (!member.isDead && member.nextAbility.timer.time < time)
            {
                time = member.nextAbility.timer.time;
                enemy = member;
            }
        }

        return enemy;
    }

    private void ParseAbilityData()
    {
        
    }

    private void UpdateTimers(BaseClass member)
    {
        Text txt = AbilitTimer1.GetComponent<Text>();
        txt.color = (!member.t1.isDone ? Color.red : Color.green);
        txt.text = $"{(member.t1.time / 60):F2}";
                
        txt = AbilitTimer2.GetComponent<Text>();
        txt.color = (!member.t2.isDone ? Color.red : Color.green);
        txt.text = $"{(member.t2.time / 60):F2}";
                
        txt = AbilitTimer3.GetComponent<Text>();
        txt.color = (!member.t3.isDone ? Color.red : Color.green);
        txt.text = $"{(member.t3.time / 60):F2}";
                
        txt = AbilitTimer4.GetComponent<Text>();
        txt.color = (!member.t4.isDone ? Color.red : Color.green);
        txt.text = $"{(member.t4.time / 60):F2}";
    }

    public static Vector3 PosToVec(POS pos)
    {
        return new Vector3((int)pos % 3, -(int)pos / 3, 0) * 1.5f;
    }

    public static void CheckAbilityInput(BaseClass member)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            member.key = KeyCode.Alpha1;
            Debug.Log($"Key: {member.key.ToString()}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            member.key = KeyCode.Alpha2;
            Debug.Log($"Key: {member.key.ToString()}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            member.key = KeyCode.Alpha3;
            Debug.Log($"Key: {member.key.ToString()}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            member.key = KeyCode.Alpha4;
            Debug.Log($"Key: {member.key.ToString()}");
        }
    }

    public static void CheckMoveInput(BaseClass member)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            member.key = KeyCode.UpArrow;
            Debug.Log($"Key: {member.key.ToString()}");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            member.key = KeyCode.DownArrow;
            Debug.Log($"Key: {member.key.ToString()}");
        }
        // else if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     member.key = KeyCode.LeftArrow;
        //     Debug.Log($"Key: {member.key.ToString()}");
        // }
        // else if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     member.key = KeyCode.RightArrow;
        //     Debug.Log($"Key: {member.key.ToString()}");
        // }
    }

    private void ActivateAbility1(BaseClass member)
    {
        member.Ability1();
    }
    
    private void ActivateAbility2(BaseClass member)
    {
        member.Ability2();
    }
    
    private void ActivateAbility3(BaseClass member)
    {
        member.Ability3();
    }
    
    private void ActivateAbility4(BaseClass member)
    {
        member.Ability4();
    }

    private void ActivateMoveUp(BaseClass member)
    {
        member.player.transform.position = member.MoveUp(member.player.transform).position;
    }
    
    private void ActivateMoveDown(BaseClass member)
    {
        member.player.transform.position = member.MoveDown(member.player.transform).position;
    }
    
    private void ActivateMoveLeft(BaseClass member)
    {
        member.player.transform.position = member.MoveLeft(member.player.transform).position;
    }
    
    private void ActivateMoveRight(BaseClass member)
    {
        member.player.transform.position = member.MoveRight(member.player.transform).position;
    }
}
