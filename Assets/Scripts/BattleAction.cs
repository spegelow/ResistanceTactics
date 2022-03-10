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
        isWeaponAttack = true;
        this.actionName = weapon.itemName;
    }

    public BattleAction(InventoryItem item)
    {
        this.item = item;
        isItemAction = true;
        this.actionName = item.itemName;
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

            ba.requiresLineOfSight = false;
            ba.canTargetEmptyTiles = true;
            ba.canTargetSelf = true;

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

        return new List<MapTile>();
    }

    public List<MapTile> GetValidTargets(MapTile userTile, Unit user)
    {
        List<MapTile> targets = GetRange(userTile, user);

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
        else if(isMoveAction)
        {
            //TEMP Setup action panel for user but not AI
            yield return BattleManager.instance.MoveUnit(actionUser, targetTile, actionUser.team == 0);
        }
        else if(isWeaponAttack)
        {
            yield return BattleManager.instance.ResolveAttack(actionUser, targetTile);
        }
    }
}
