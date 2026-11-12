using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches.QoL;

[PatchAll]
public static class ACAB
{
    [DebugVariableMarker("911 Operators on Lunch Break")]
    public static bool PoliceBeGoneSpray = false;

    [HarmonyPatch(typeof(PlayerController), "CallPolice", []), HarmonyPrefix]
    public static bool CallPolice() => !PoliceBeGoneSpray;
}