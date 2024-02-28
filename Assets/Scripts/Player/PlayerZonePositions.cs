using System;
using UnityEngine;

public class PlayerZonePositions
{

    [Serializable]
    public class OneVSOne
    {
        public Transform Team1Position_1;
        public Transform Team2Position_1;
    }

    [Serializable]
    public class TwoVSTwo
    {
        public Transform Team1Position_1;
        public Transform Team1Position_2;
        public Transform Team2Position_1;
        public Transform Team2Position_2;
    }

    [Serializable]
    public class ThreeVSThree
    {
        public Transform Team1Position_1;
        public Transform Team1Position_2;
        public Transform Team1Position_3;
        public Transform Team2Position_1;
        public Transform Team2Position_2;
        public Transform Team2Position_3;
    }

}
