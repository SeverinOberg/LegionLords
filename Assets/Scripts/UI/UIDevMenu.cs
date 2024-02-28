using Unity.Netcode;

public class UIDevMenu : NetworkBehaviour
{

    private const int _addGoldAmount = 5000;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnClickSpeedUp()
    {

        if (MatchManager.S.GameSpeed.Value == 0)
        {
            MatchManager.S.GameSpeed.Value = 1;
            return;
        }
        else if (MatchManager.S.GameSpeed.Value == 1)
        {
            MatchManager.S.GameSpeed.Value = 5;
            return;
        }
        else if (MatchManager.S.GameSpeed.Value >= 20)
        {
            return;
        }
            
        MatchManager.S.GameSpeed.Value += 5;
    }

    public void OnClickSpeedDown()
    {
        if (MatchManager.S.GameSpeed.Value == 1)
        {
            MatchManager.S.GameSpeed.Value = 0;
            return;
        }
        else if (MatchManager.S.GameSpeed.Value == 5)
        {
            MatchManager.S.GameSpeed.Value = 1;
            return;
        }
        else if (MatchManager.S.GameSpeed.Value == 0)
        {
            return;
        }
        else if (MatchManager.S.GameSpeed.Value < 1)
        {
            MatchManager.S.GameSpeed.Value = 1;
            return;
        }

        MatchManager.S.GameSpeed.Value -= 5;
    }

    public void OnClickAddGold()
    {
        foreach (var player in Server.S.Players)
        {
            player.ResourceController.IncrGold(_addGoldAmount);
        }
    }

}
