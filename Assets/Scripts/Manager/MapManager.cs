using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    
    public static MapDatabase S { get; private set; }

    public List<MapData> Data { get; private set; } = new();

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Init();
    }

    private void Init()
    {
        SetDesertIslandData();
    }

    public MapData Get(Map map)
    {
        return Data.Find((m) => m.Map == map);
    }

    public MapDataPayload Get(Map map, byte team, byte spawnRow, MatchType matchType)
    {
        var m = Data.Find((m) => m.Map == map);

        if (matchType == MatchType.OneVSOne)
        {
            if (spawnRow == 0)
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.OneVSOne_Team1_Position1,
                        SpawnRotation = m.SpawnRotations.OneVSOne_Team1_Rotation1,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.OneVSOne_Team2_Position1,
                        SpawnRotation = m.SpawnRotations.OneVSOne_Team2_Rotation1,
                    };
                }
            }
        }
        else if (matchType == MatchType.TwoVSTwo)
        {
            if (spawnRow == 0)
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.TwoVSTwo_Team1_Position1,
                        SpawnRotation = m.SpawnRotations.OneVSOne_Team2_Rotation1,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.TwoVSTwo_Team1_Position1,
                        SpawnRotation = m.SpawnRotations.TwoVSTwo_Team2_Rotation1,
                    };
                }
            }
            else
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.TwoVSTwo_Team1_Position2,
                        SpawnRotation = m.SpawnRotations.TwoVSTwo_Team2_Rotation2,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.TwoVSTwo_Team1_Position2,
                        SpawnRotation = m.SpawnRotations.TwoVSTwo_Team2_Rotation2,
                    };
                }
            }
        }
        else
        {
            if (spawnRow == 0)
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team1_Position1,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team1_Rotation1,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team2_Position1,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team2_Rotation1,
                    };
                }
            }
            else if (spawnRow == 1)
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team1_Position2,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team1_Rotation2,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team2_Position2,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team2_Rotation2,
                    };
                }
            }
            else
            {
                if (team == 1)
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team1_Position3,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team1_Rotation3,
                    };
                }
                else
                {
                    return new MapDataPayload
                    {
                        SpawnPosition = m.SpawnPositions.ThreeVSThree_Team2_Position3,
                        SpawnRotation = m.SpawnRotations.ThreeVSThree_Team2_Rotation3,
                    };
                }
            }
        }

        return new MapDataPayload { };
        
    }

    private void SetDesertIslandData()
    {
        SpawnPositions spawnPositions = new SpawnPositions();
        SpawnRotations spawnRotations = new SpawnRotations();

        // + 1VS1 +
        // Team 1
        spawnPositions.OneVSOne_Team1_Position1 = new Vector3(-300, 0, -300);
        spawnRotations.OneVSOne_Team1_Rotation1 = new Vector3(0, 45, 0f);

        // Team 2
        spawnPositions.OneVSOne_Team2_Position1 = new Vector3(300, 0, 300);
        spawnRotations.OneVSOne_Team2_Rotation1 = new Vector3(0, -135, 0);
        // - 1VS1 -

        // + 2VS2 +
        // Team 1
        spawnPositions.TwoVSTwo_Team1_Position1 = new Vector3(-350, 0, -250);
        spawnRotations.TwoVSTwo_Team1_Rotation1 = new Vector3(0, 45, 0);

        spawnPositions.TwoVSTwo_Team1_Position2 = new Vector3(-250, 0, -350);
        spawnRotations.TwoVSTwo_Team1_Rotation2 = new Vector3(0, 45, 0);

        // Team 2
        spawnPositions.TwoVSTwo_Team2_Position1 = new Vector3(250, 0, 350);
        spawnRotations.TwoVSTwo_Team2_Rotation1 = new Vector3(0, -135, 0);

        spawnPositions.TwoVSTwo_Team2_Position2 = new Vector3(350, 0, 250);
        spawnRotations.TwoVSTwo_Team2_Rotation2 = new Vector3(0, -135, 0);
        // - 2VS2 -

        // + 3VS3 +
        // Team 1
        spawnPositions.ThreeVSThree_Team1_Position1 = new Vector3(-350, 0, -250);
        spawnRotations.ThreeVSThree_Team1_Rotation1 = new Vector3(0, 45, 0);

        spawnPositions.ThreeVSThree_Team1_Position2 = new Vector3(-300, 0, -300);
        spawnRotations.ThreeVSThree_Team1_Rotation2 = new Vector3(0, 45, 0);

        spawnPositions.ThreeVSThree_Team1_Position3 = new Vector3(-250, 0, -350);
        spawnRotations.ThreeVSThree_Team1_Rotation3 = new Vector3(0, 45, 0);

        // Team 2
        spawnPositions.ThreeVSThree_Team2_Position1 = new Vector3(250, 0, 350);
        spawnRotations.ThreeVSThree_Team2_Rotation1 = new Vector3(0, -135, 0);

        spawnPositions.ThreeVSThree_Team2_Position2 = new Vector3(300, 0, 300);
        spawnRotations.ThreeVSThree_Team2_Rotation2 = new Vector3(0, -135, 0);

        spawnPositions.ThreeVSThree_Team2_Position3 = new Vector3(350, 0, 250);
        spawnRotations.ThreeVSThree_Team2_Rotation3 = new Vector3(0, -135, 0);
        // - 3VS3 -

        var data = new MapData
        {
            Map = Map.DesertIsland,
            SpawnPositions = spawnPositions,
            SpawnRotations = spawnRotations,
        };

        Data.Add(data);
    }

}

public enum Map
{
    DesertIsland,
}


public struct MapDataPayload
{
    public Vector3 SpawnPosition;
    public Vector3 SpawnRotation;
}

public struct MapData
{
    public Map Map;
    public SpawnPositions SpawnPositions;
    public SpawnRotations SpawnRotations;
}

public struct SpawnPositions
{
    public Vector3 OneVSOne_Team1_Position1;
    public Vector3 OneVSOne_Team2_Position1;

    public Vector3 TwoVSTwo_Team1_Position1;
    public Vector3 TwoVSTwo_Team1_Position2;
    public Vector3 TwoVSTwo_Team2_Position1;
    public Vector3 TwoVSTwo_Team2_Position2;

    public Vector3 ThreeVSThree_Team1_Position1;
    public Vector3 ThreeVSThree_Team1_Position2;
    public Vector3 ThreeVSThree_Team1_Position3;
    public Vector3 ThreeVSThree_Team2_Position1;
    public Vector3 ThreeVSThree_Team2_Position2;
    public Vector3 ThreeVSThree_Team2_Position3;
}

public struct SpawnRotations
{
    public Vector3 OneVSOne_Team1_Rotation1;
    public Vector3 OneVSOne_Team2_Rotation1;

    public Vector3 TwoVSTwo_Team1_Rotation1;
    public Vector3 TwoVSTwo_Team1_Rotation2;
    public Vector3 TwoVSTwo_Team2_Rotation1;
    public Vector3 TwoVSTwo_Team2_Rotation2;

    public Vector3 ThreeVSThree_Team1_Rotation1;
    public Vector3 ThreeVSThree_Team1_Rotation2;
    public Vector3 ThreeVSThree_Team1_Rotation3;
    public Vector3 ThreeVSThree_Team2_Rotation1;
    public Vector3 ThreeVSThree_Team2_Rotation2;
    public Vector3 ThreeVSThree_Team2_Rotation3;
}