using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;

namespace Thiefipelago.Archipelago;

public static class ItemHandler
{
    public static Dictionary<string, int> AbilityAmounts = [];
    public static int LocalItemCount;
    public static int TotalItemCount;
    public static int Diamonds;

    public static void HandleItem(ItemInfo item)
    {
        LocalItemCount++;
        var name = item.ItemName;
        if (name.StartsWith("Progressive "))
        {
            var current = AbilityAmounts.GetValueOrDefault(name, 0) + 1;
            var key = $"{name},{current}";
            var tools = ThiefClient.AbilityToolMap.GetValueOrDefault(key, []);
            Core.Skills[key].isLearned = true;
            foreach (var tool in tools) TryGiveItem(tool);
            AbilityAmounts[name] = current;
        }

        if (name is "Diamond")
        {
            Diamonds++;
            Core.Log.Msg($"Diamonds: [{Diamonds}/40]");
            if (Diamonds >= 40) ThiefClient.Client.TryGoal();
        }
        
        if (LocalItemCount < TotalItemCount) return;
        TotalItemCount = LocalItemCount;
        ThiefClient.Client.SendToStorage("item_count", TotalItemCount, Scope.Slot);
        if (name.StartsWith("$")) PlayerInventory.instance.EarnCash(int.Parse(name[1..]));
    }

    public static void TryGiveItem(string itemId)
    {
        if (!Core.Tools.TryGetValue(itemId, out var item))
        {
            Core.Log.Error($"Item tool doesn't exist: [{itemId}]");
            return;
        }

        if (PlayerInventory.itemsList.Any(i => i.itemAsset == item)) return;
        PlayerInventory.instance.AddItem(item, 100, $"shoptool{UnityEngine.Random.Range(100000, 999999)}", "");
    }

    public static void HandlerReset()
    {
        AbilityAmounts.Clear();
        LocalItemCount = 0;
        TotalItemCount = 0;
        Diamonds = 0;
    }
}