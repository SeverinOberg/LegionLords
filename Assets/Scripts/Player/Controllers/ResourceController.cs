using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ResourceController : NetworkBehaviour
{

    private TextMeshProUGUI _goldText;

    private NetworkVariable<int> _currentGold = new(150);

    private void Start()
    {
        if (IsClient)
        {
            if (_goldText == null)
            {
                _goldText = GameObject.Find("Gold Text").GetComponent<TextMeshProUGUI>();
                if (_goldText != null)
                {
                    _goldText.text = $"{_currentGold.Value} Gold";
                }
            }

            _currentGold.OnValueChanged += OnCurrentGoldUpdate;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsClient)
        {
            _currentGold.OnValueChanged -= OnCurrentGoldUpdate;
        }
    }

    private void OnCurrentGoldUpdate(int prev, int next)
    {
        _goldText.text = $"{next} Gold";
    }

    
    public bool CanBuy(int price)
    {
        if (_currentGold.Value >= price)
        {
            return true;
        }

        if (IsClient)
        {
            AnnouncementManager.S.Announce("NOT ENOUGH GOLD");
        }

        return false;
    }

    public void IncrGold(int amount)
    {
        if (IsServer)
        {
            IncreaseGold(amount);
        }
        else
        {
            IncreaseGoldServerRpc(amount);
        }
    }

    public void DecrGold(int amount)
    {
        if (IsServer)
        {
            DecreaseGold(amount);
        }
        else
        {
            DecreaseGoldServerRpc(amount);
        }
    }

    [ServerRpc]
    private void IncreaseGoldServerRpc(int amount)
    {
        IncreaseGold(amount);
    }

    [ServerRpc]
    private void DecreaseGoldServerRpc(int amount)
    {
        DecreaseGold(amount);
    }

    private void IncreaseGold(int amount)
    {
        if (amount < 1 && !CanBuy(amount)) return;
        _currentGold.Value += amount;
    }

    private void DecreaseGold(int amount)
    {
        if (amount < 1) return;
        if (_currentGold.Value - amount < 1)
        {
            _currentGold.Value = 0;
            return;
        }

        _currentGold.Value -= amount;
    }


}
