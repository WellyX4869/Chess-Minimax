using UnityEngine;
using UnityEngine.UI;

public class Queen : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);
        role = "Q";
        // Queen stuff
        mMovement = new Vector3Int(7, 7, 7);
        mValue = 9;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Queen");
    }
}
