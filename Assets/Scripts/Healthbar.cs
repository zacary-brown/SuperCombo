using JetBrains.Annotations;
using UnityEngine;

public static class SCALE
{
    public static int scale = 2;
}

public class Healthbar
{
    public int health;
    public int stamina;
    private GameObject healthbar;
    private GameObject staminabar;
    private GameObject abilitybar;
    
    public Healthbar(POS position, int life = 100, int stam = 50)
    {
        if (position <= POS.E_FRONT_RIGHT)
        {
            abilitybar = MonoBehaviour.Instantiate((GameObject) Resources.Load("Prefabs/Abilitytimer"),
                CombatSystem.PosToVec(position), Quaternion.identity);
            
            abilitybar.GetComponent<Transform>().localPosition += new Vector3(0.0f, -abilitybar.GetComponent<Transform>().localScale.y + (position <= POS.E_FRONT_RIGHT ? 1.0f : -1.0f));
            
            //abilitybar.GetComponent<SpriteRenderer>().color = Color.blue;
            
            staminabar = MonoBehaviour.Instantiate((GameObject) Resources.Load("Prefabs/Healthbar"), CombatSystem.PosToVec(position), Quaternion.identity);
        
            staminabar.GetComponent<Transform>().localPosition += new Vector3(0.0f, (position <= POS.E_FRONT_RIGHT ? 1.0f : -1.0f));
        
            staminabar.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            abilitybar = null;
        }
        
        healthbar = MonoBehaviour.Instantiate((GameObject) Resources.Load("Prefabs/Healthbar"), CombatSystem.PosToVec(position), Quaternion.identity);
        
        healthbar.GetComponent<Transform>().localPosition += new Vector3(0.0f, healthbar.GetComponent<Transform>().localScale.y + (position <= POS.E_FRONT_RIGHT ? 1.0f : -1.0f));
        
        //healthbar.GetComponent<SpriteRenderer>().color = Color.red;
        
        health = life;
        stamina = stam;
    }

    public void Update([CanBeNull] Timer t = null)
    {
        healthbar.GetComponent<Transform>().localScale = new Vector3(health / 100.0f, healthbar.GetComponent<Transform>().localScale.y);

        if (abilitybar != null)
        {
            staminabar.GetComponent<Transform>().localScale = new Vector3(stamina / 100.0f, staminabar.GetComponent<Transform>().localScale.y);
            abilitybar.GetComponent<Transform>().localScale = new Vector3(t.time / (t.duration * 0.5f), abilitybar.GetComponent<Transform>().localScale.y);
            
            //Change color of ability bar depending on whether its in the critical threshold or not
             if (t.time < t.threshold)
                 abilitybar.GetComponent<SpriteRenderer>().color = Color.yellow;
             else
                 abilitybar.GetComponent<SpriteRenderer>().color = Color.LerpUnclamped(new Color(0.078f,0.23f,0.31f), new Color(0.54f, 0.77f, 0.81f), t.time / t.duration);
        }
    }

    public void Destroy()
    {
        MonoBehaviour.Destroy(healthbar);
        MonoBehaviour.Destroy(staminabar);
        MonoBehaviour.Destroy(abilitybar);
        health = 0;
        stamina = 0;
    }
}
