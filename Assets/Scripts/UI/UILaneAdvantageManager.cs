using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UILaneAdvantageManager : NetworkBehaviour
{

    [SerializeField]
    private Image _display;

    private NetworkVariable<byte> _teamWithAdvantage = new();

    private Color _team1Color = Color.blue;
    private Color _team2Color = Color.red;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            _teamWithAdvantage.OnValueChanged += Refresh;
        }

        _team1Color = ReferenceManager.S.MaterialTeam1.color;
        _team2Color = ReferenceManager.S.MaterialTeam2.color;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsClient)
        {
            _teamWithAdvantage.OnValueChanged -= Refresh;
        }
    }

    private void Refresh(byte previousValue, byte newValue)
    {
        if (newValue != 0)
        {
            _display.color = (newValue == 1) ? _team1Color : _team2Color;
        }
    }

}
