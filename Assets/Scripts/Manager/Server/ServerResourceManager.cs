using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// SERVER ONLY - Manages all clients resources.
/// </summary>
public class ServerResourceManager : NetworkBehaviour
{

    public static ServerResourceManager S { get; private set; }

    private List<ClientResourceData> _data = new();

    private const int _startGoldPerSecond = 1;
    private const int _cooldown = 3;

    private float _timer;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        _timer += Time.deltaTime;

        if (_timer >= _cooldown)
        {
            _timer = 0;

            foreach (var d in _data)
            {
                d.ResourceController.IncrGold(d.GoldPerSecond * _cooldown);
            }
        }
    }

    public void AddClientResourceData(ClientResourceData data)
    {
        if (!IsServer) return;

        _data.Add(data);
    }

    public void IncreaseGoldPerSecond(ulong clientID, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].ClientID == clientID)
            {
                _data[i].GoldPerSecond += amount;
                return;
            }
        }
    }

    public void IncreaseGoldPerSecond(byte team, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Team == team)
            {
                _data[i].GoldPerSecond += amount;
            }
        }
    }

    public void DecreaseGoldPerSecond(ulong clientID, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].ClientID == clientID)
            {
                if (_data[i].GoldPerSecond - amount < 1)
                {
                    _data[i].GoldPerSecond = 1;
                }
                else
                {
                    _data[i].GoldPerSecond -= amount;
                }
                return;
            }
        }
    }

    public void DecreaseGoldPerSecond(byte team, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Team == team)
            {
                if (_data[i].GoldPerSecond - amount < 1)
                {
                    _data[i].GoldPerSecond = 1;
                }
                else
                {
                    _data[i].GoldPerSecond -= amount;
                }
            }
        }
    }

    public void AddGoldToClient(ulong clientID, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].ClientID == clientID)
            {
                _data[i].ResourceController.IncrGold(amount);
                return;
            }
        }
    }

    public void RemoveGoldFromClient(ulong clientID, int amount)
    {
        if (!IsServer) return;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].ClientID == clientID)
            {
                _data[i].ResourceController.DecrGold(amount);
                return;
            }
        }
    }

    public class ClientResourceData
    {
        public ulong ClientID;
        public byte Team;
        public int GoldPerSecond;
        public ResourceController ResourceController;    

        public ClientResourceData(byte clientID, byte team, ResourceController resourceController)
        {
            ClientID = clientID;
            Team = team;
            GoldPerSecond = _startGoldPerSecond;
            ResourceController = resourceController;
        }
    }

}

