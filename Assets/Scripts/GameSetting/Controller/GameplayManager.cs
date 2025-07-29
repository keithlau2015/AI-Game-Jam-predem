using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    public class GameplaySetting
    {
        public float camMaxSpd = 100f;
        public float camMaxRotSpd = 1f;
    }
}