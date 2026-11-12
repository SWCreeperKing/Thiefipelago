using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches.QoL;

[PatchAll]
public static class InfLockPickHealth
{
    [DebugVariableMarker("Lv. 1 Lock Pick Inf Durability")]
    public static bool InfLv1LockPick = true;
    
    [DebugVariableMarker("Lv. 2 Lock Pick Inf Durability")]
    public static bool InfLv2LockPick = true;
    
    [HarmonyPatch(typeof(LockpickObject), "TakeDamage"), HarmonyPrefix]
    public static bool NoDamage(float damage) => !InfLv1LockPick;

    [HarmonyPatch(typeof(LockpickObvController), "LockpickControl"), HarmonyPrefix]
    public static void NoDamage(LockpickObvController __instance)
    {
        if (!InfLv2LockPick) return;
        __instance.SetPrivateField("durability", 3);
    }
}