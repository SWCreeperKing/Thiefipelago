using HarmonyLib;
using Thiefipelago.Added;
using Thiefipelago.Archipelago;

namespace Thiefipelago.Patches;

[PatchAll]
public static class PickupPatch
{
    [DebugVariableMarker("Print Grab ID Messages")]
    public static bool PrintGrabMsgs = false;
    // [HarmonyPatch(typeof(vp_ThiefGrab), "LateDestroy"), HarmonyPrefix]
    // public static void PrintID1(vp_ThiefGrab __instance)
    // {
    //     var id = __instance.gameObject.GetComponent<UniqueId>(); 
    //     Core.DebugMsg($"Grabbed: [{id.uniqueId}]");
    // }
    //
    // [HarmonyPatch(typeof(vp_ThiefGrab), "LateGrab"), HarmonyPrefix]
    // public static void PrintID2(vp_ThiefGrab __instance)
    // {
    //     var id = __instance.gameObject.GetComponent<UniqueId>(); 
    //     Core.DebugMsg($"Grabbed: [{id.uniqueId}]");
    // }
    
    [HarmonyPatch(typeof(vp_ThiefGrab), "TryInteract"), HarmonyPrefix]
    public static void PrintID3(vp_ThiefGrab __instance, vp_PlayerEventHandler player)
    {
        if (!PrintGrabMsgs) return;
        var id = __instance.gameObject.GetComponent<UniqueId>();
        if (ThiefClient.ItemIdToLocation.ContainsKey(id.uniqueId)) return;
        Core.Log.Warning($"Grabbed: [{id.uniqueId}]");
    }

    [HarmonyPatch(typeof(ItemObject), "Awake"), HarmonyPrefix]
    public static void RemoveMoneyValue(ItemObject __instance)
    {
        if (!__instance.isCash || __instance.cashValue == 15) return;
        __instance.cashValue = 0;
    }

    [HarmonyPatch(typeof(SaveController), "RemoveItemFromSave"), HarmonyPrefix]
    public static void RemoveFromInv(string id) => SendLocation(id);
    
    [HarmonyPatch(typeof(SaveController), "SetToSold"), HarmonyPrefix]
    public static void SetToSold(string id) => SendLocation(id);

    public static void SendLocation(string id)
    {
        if (!ThiefClient.ItemIdToLocation.TryGetValue(id, out var locName)) return;
        ThiefClient.SendLocation(locName);
    }
}