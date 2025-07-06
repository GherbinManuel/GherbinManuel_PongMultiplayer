using TMPro;
using UnityEngine;

public class UI_Gameplay : UIBase
{
    public override UIManager.UITypes UIType => UIManager.UITypes.Gameplay;

    [SerializeField] private TextMeshProUGUI _leftPlayerPointLabel;
    [SerializeField] private TextMeshProUGUI _rightPlayerPointLabel;

    public override void IsActive(bool isActive)
    {
        base.IsActive(isActive);

        if (isActive)
            PlayerBarPong.OnPointchangeEvent += PrintPoints;
        else
            PlayerBarPong.OnPointchangeEvent -= PrintPoints;
    }

    private void PrintPoints(int playerID, int playerPoints)
    {
        switch (playerID)
        {
            case 0:
                _leftPlayerPointLabel.text = playerPoints.ToString();
                break;
            case 1:
                _rightPlayerPointLabel.text = playerPoints.ToString();
                break;
            default:
                break;
        }
    }

}
