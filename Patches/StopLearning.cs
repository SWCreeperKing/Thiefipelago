using HarmonyLib;
using Thiefipelago.Added;

namespace Thiefipelago.Patches;

[PatchAll]
public static class StopLearning
{
    [HarmonyPatch(typeof(SkillsCanvasWindow), "OnSkillLearn"), HarmonyPrefix]
    public static bool StopTheLearning() => false;
}