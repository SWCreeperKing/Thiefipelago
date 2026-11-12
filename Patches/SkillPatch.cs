using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches;

[PatchAll]
public static class SkillPatch
{
    // [HarmonyPatch(typeof(SkillsCanvasWindow), "")]
    [HarmonyPatch(typeof(SkillUiButton), "Start"), HarmonyPrefix]
    public static void GetSkills(SkillUiButton __instance)
    {
        var skill = __instance.skillAsset;
        Core.Log.Msg($"skill: [{skill.skillName}] ([{skill.isLearned}])");
    }
}