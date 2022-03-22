using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleAction
{
    public string actionName;

    public bool isWaitAction;
    public bool isMoveAction;

    public bool isWeaponAttack;
    public Weapon weapon;

    public bool isItemAction;
    public InventoryItem item;

    public enum TargetingType {SingleTile };
    public TargetingType targetingType;

    public TargetingInfo targeting;

    public BattleAction()
    {
        
    }

    public BattleAction(Weapon weapon)
    {
        this.weapon = weapon;
        isWeaponAttack = true;
        this.actionName = weapon.itemName;
        targeting = new TargetingInfo(true, false, false, false, true, weapon.minAttackRange, weapon.maxAttackRange);
    }

    public BattleAction(InventoryItem item)
    {
        this.item = item;
        isItemAction = true;
        this.actionName = item.itemName;
        this.targeting = item.targetingInfo;
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
    public static BattleAction MoveAction
    {
        get
        {
            BattleAction ba = new BattleAction();
            ba.isMoveAction = true;
            ba.actionName = "Move";

            ba.targeting.requiresLineOfSight = false;
            ba.targeting.canTargetEmptyTiles = true;
            ba.targeting.canTargetSelf = true;

            return ba;
        }
    }

    public List<MapTile> GetRange(MapTile userTile, Unit user)
    {
        if(isMoveAction)
        {
            return user.GetMoveableTiles();
        }

        if(isWeaponAttack)
        {
            return MapManager.instance.GetTilesInRange(userTile.x, userTile.z, weapon.minAttackRange, weapon.maxAttackRange);
        }

        if (isItemAction)
        {
            return MapManager.instance.GetTilesInRange(userTile.x, userTile.z, targeting.minRange, targeting.maxRange);
        }

        return new List<MapTile>();
    }

    public List<MapTile> GetValidTargets(MapTile userTile, Unit user)
    {
        List<MapTile> targets = GetRange(userTile, user);

        //Filter out the tiles so that only the valid ones remain
        targets = targets.FindAll(tile => IsTargetValid(tile, userTile, user));
        
        return targets;
    }

    public bool IsTargetValid(MapTile targetTile, MapTile userTile, Unit user)
    {
        bool test;
        test = (!targeting.requiresLineOfSight || BattleManager.CheckLineOfSight(user, userTile, targetTile));
        test = test && (targeting.canTargetEmptyTiles || targetTile.occupant != null);
        test = test && (targeting.canTargetSelf || targetTile.occupant != user);
        test = test && (targeting.canTargetAllies || targetTile.occupant == null || targetTile.occupant.team != user.team);
        test = test && (targeting.canTargetEnemies || targetTile.occupant == null || targetTile.occupant.team == user.team);
        return test;
    }

    public IEnumerator ResolveAction(Unit actionUser, MapTile targetTile)
    {
        if(isWaitAction)
        {
            yield return new WaitForSeconds(1);
            BattleManager.instance.EndTurn();
        }
        else if(isMoveAction)
        {
            //TEMP Setup action panel for user but not AI
            yield return BattleManager.instance.MoveUnit(actionUser, targetTile, actionUser.team == 0);
        }
        else if(isWeaponAttack)
        {
            yield return BattleManager.instance.ResolveAttack(actionUser, targetTile);
        }
        else if(isItemAction)
        {
            yield return item.ResolveEffect(actionUser, targetTile);
            BattleManager.instance.EndTurn();
        }
    }
}
