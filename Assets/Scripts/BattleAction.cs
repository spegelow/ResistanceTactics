using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleAction
{
    public string actionName;

    public bool isWaitAction;

    public bool isWeaponAttack;
    public Weapon weapon;

    public enum TargetingType {SingleTile };
    public TargetingType targetingType;

    public bool requiresLineOfSight = true;
    public bool canTargetEmptyTiles = false;
    public bool canTargetSelf = false;
    public bool canTargetAllies = false;
    public bool canTargetEnemies = true;

    public BattleAction()
    {
        
    }

    public BattleAction(Weapon weapon)
    {
        this.weapon = weapon;
        this.actionName = weapon.itemName;
    }

    public static BattleAction WaitAction
    {
        get
        {
            BattleAction ba = new BattleAction();
            ba.isWaitAction = true;
            ba.actionName = "Wait";
            return ba;
        }
    }

    public List<MapTile> GetRange(MapTile userTile)
    {
        if(isWeaponAttack)
        {
            return MapManager.instance.GetTilesInRange(userTile.x, userTile.z, weapon.minAttackRange, weapon.maxAttackRange);
        }

        return null;
    }

    public List<MapTile> GetValidTargets(MapTile userTile, Unit user)
    {
        List<MapTile> targets = GetRange(userTile);

        //Filter out the tiles so that only the valid ones remain
        bool test;
        targets = targets.FindAll(tile =>
        {
            test = (!requiresLineOfSight || BattleManager.CheckLineOfSight(user, userTile, tile));
            test = test && (canTargetEmptyTiles || tile.occupant != null);
            test = test && (canTargetSelf || tile.occupant != user);
            test = test && (canTargetAllies || tile.occupant == null || tile.occupant.team != user.team);
            test = test && (canTargetEnemies || tile.occupant == null || tile.occupant.team == user.team);
            return test;
        });
        
        return targets;
    }

    public IEnumerator ResolveAction(Unit actionUser, MapTile targetTile)
    {
        if(isWaitAction)
        {
            yield return new WaitForSeconds(1);
            BattleManager.instance.EndTurn();
        }
        else if(isWeaponAttack)
        {
            BattleManager.instance.ResolveAttack(actionUser, targetTile);
        }
    }
}
