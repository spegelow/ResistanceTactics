using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/Equipment/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;

    public TargetingInfo targetingInfo;

    [System.Serializable]
    public struct ItemEffect
    {
        public bool isHealEffect;
        public bool isDamageEffect;

        public int minAmount;
        public int maxAmount;
    }

    public ItemEffect effect;

    public IEnumerator ResolveEffect(Unit user, MapTile targetTile)
    {
        yield return new WaitForEndOfFrame();
        if (effect.isHealEffect)
        {
            Unit target = targetTile.occupant;
            if (target != null)
            {
                int amountHealed = Random.Range(effect.minAmount, effect.maxAmount + 1);
                amountHealed = Mathf.Min(amountHealed, target.GetMaxHealth() - target.currentHealth);
                target.ApplyHealing(amountHealed);
                BattleManager.instance.CreateFloatingText(targetTile.occupant, amountHealed + "", Color.green);
            }
        }
    }
}
