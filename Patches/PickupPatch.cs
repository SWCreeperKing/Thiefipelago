using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches;

[PatchAll]
public static class PickupPatch
{
    // [HarmonyPatch(typeof(vp_ThiefGrab), "LateDestroy"), HarmonyPrefix]
    // public static void PrintID1(vp_ThiefGrab __instance)
    // {
    //     var id = __instance.gameObject.GetComponent<UniqueId>(); 
    //     Core.Log.Msg($"Grabbed: [{id.uniqueId}]");
    // }
    //
    // [HarmonyPatch(typeof(vp_ThiefGrab), "LateGrab"), HarmonyPrefix]
    // public static void PrintID2(vp_ThiefGrab __instance)
    // {
    //     var id = __instance.gameObject.GetComponent<UniqueId>(); 
    //     Core.Log.Msg($"Grabbed: [{id.uniqueId}]");
    // }
    
    [HarmonyPatch(typeof(vp_ThiefGrab), "TryInteract"), HarmonyPrefix]
    public static void PrintID3(vp_ThiefGrab __instance, vp_PlayerEventHandler player)
    {
        var id = __instance.gameObject.GetComponent<UniqueId>(); 
        Core.Log.Msg($"Grabbed: [{id.uniqueId}]");
    }
}