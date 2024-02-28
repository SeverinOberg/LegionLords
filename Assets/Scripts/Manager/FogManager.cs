using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class FogManager : NetworkBehaviour
{

    [System.Serializable]
    private class CloudFogData
    {
        public GameObject FogGameObject;
        public byte Team;

        public CloudFogData(GameObject fogGameObject, byte team)
        {
            FogGameObject = fogGameObject;
            Team = team;
        }
    }

    [SerializeField] private CloudFogData[] _cloudFogObjects;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;
            DisableOwnTeamClouds();
    }

    private void DisableOwnTeamClouds()
    {
        foreach (var player in Server.S.Players)
        {
            foreach (var fog in _cloudFogObjects)
            {
                if (player.Team.Value == fog.Team)
                {
                    DisableCloudFogClientRpc(fog.Team, player.ClientRpcParams);
                }
            }
        }
    }

    [ClientRpc]
    private void DisableCloudFogClientRpc(byte team, ClientRpcParams clientRpcParams)
    {
        foreach (var fog in _cloudFogObjects)
        {
            if (fog.Team == team)
            {
                fog.FogGameObject.gameObject.SetActive(false);
                break;
            }
        }
    }



}
