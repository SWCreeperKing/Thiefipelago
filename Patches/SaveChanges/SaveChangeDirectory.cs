using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches.SaveChanges;

[PatchAll]
public static class SaveChangeDirectory
{
    //todo: reload main menu when doing this
    
    // [HarmonyPatch(typeof(SaveController), "GetProfileKey"), HarmonyPostfix]
    // public static void ChangePath(int pId, ref string __result) => __result = $"Archipelago_{__result}";
}