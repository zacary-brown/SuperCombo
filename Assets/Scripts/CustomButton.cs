using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    [SerializeField]
    public AI_TYPES ai_type = AI_TYPES.RANDOM;
    public Button mybutton;
    
    // Start is called before the first frame update
    void Start()
    {
        Button bt = mybutton.GetComponent<Button>();
        bt.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        ((Warrior) Players.party[0]).type = ai_type;

        Destroy(Players.party[0].player);

        if (ai_type == AI_TYPES.NONE)
            Players.party[0].player = Instantiate((GameObject)Resources.Load("Prefabs/Player"), CombatSystem.PosToVec(Players.party[0].pos), Quaternion.identity);
        else
            Players.party[0].player = Instantiate((GameObject)Resources.Load("Prefabs/PlayerAI"), CombatSystem.PosToVec(Players.party[0].pos), Quaternion.identity);
    }
}


        
