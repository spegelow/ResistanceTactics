using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TargetingInfo
{
    public bool requiresLineOfSight;
    public bool canTargetEmptyTiles;
    public bool canTargetSelf;
    public bool canTargetAllies;
    public bool canTargetEnemies;
    public int minRange;
    public int maxRange;

    public TargetingInfo(bool requiresLineOfSight, bool canTargetEmptyTiles,
        bool canTargetSelf, bool canTargetAllies, bool canTargetEnemies,
        int minRange, int maxRange)
    {
        this.requiresLineOfSight = requiresLineOfSight;
        this.canTargetEmptyTiles = canTargetEmptyTiles;
        this.canTargetSelf = canTargetSelf;
        this.canTargetAllies = canTargetAllies;
        this.canTargetEnemies = canTargetEnemies;
        this.minRange = minRange;
        this.maxRange = maxRange;
    }

}
