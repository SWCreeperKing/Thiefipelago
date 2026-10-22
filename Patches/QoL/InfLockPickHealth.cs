using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches.QoL;

[PatchAll]
public static class InfLockPickHealth
{
    [HarmonyPatch(typeof(LockpickObject), "TakeDamage"), HarmonyPrefix]
    public static bool NoDamage(float damage) => false;
    
    [HarmonyPatch(typeof(RoundLockpickPin), "LockPin"), HarmonyPostfix]
    public static void NoDamage(RoundLockpickPin __instance) => __instance.SetPrivateField("failNum", 0);
}