using UnityEngine;
using UnityEngine.UI;

public class Drop : MonoBehaviour
{
    [SerializeField]
    public Dropdown drop;
    
    public void SetMatchup(int matchup)
    {
        FastMatch.matchup = matchup;
    }

    public void SetAI(int AItype)
    {
        Players.party[0].type = (AI_TYPES) AItype;

        Destroy(Players.party[0].player);

        if (Players.party[0].type == AI_TYPES.NONE)
            Players.party[0].player = Instantiate((GameObject)Resources.Load("Prefabs/Player"), CombatSystem.PosToVec(Players.party[0].pos), Quaternion.identity);
        else
            Players.party[0].player = Instantiate((GameObject)Resources.Load("Prefabs/PlayerAI"), CombatSystem.PosToVec(Players.party[0].pos), Quaternion.identity);
    }
}
