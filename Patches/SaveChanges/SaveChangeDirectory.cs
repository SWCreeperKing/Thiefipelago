using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches.SaveChanges;

[PatchAll]
public static class SaveChangeDirectory
{
     public static string SaveFilePrefix = "";
    
     [HarmonyPatch(typeof(SaveController), "GetProfileKey"), HarmonyPostfix]
     public static void ChangePath(int pId, ref string __result) => __result = $"{SaveFilePrefix}{__result}";
}