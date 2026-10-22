using HarmonyLib;
using Thiefipelago.Added;
using UnityEngine.SceneManagement;

namespace Thiefipelago.Patches.SaveChanges;

[PatchAll]
public static class SaveItemOverride
{
    // [HarmonyPatch(typeof(SaveController), "LoadSceneObject"), HarmonyPrefix]
    // public static bool LoadSceneObject(ItemObject itemObj, string id)
    // {
    // return false;
    //     // if (SaveController.itemsSaved.savedItems.All(item => item.id != id)) return false;
    //     // UnityEngine.Object.Destroy(itemObj.gameObject);
    //     // return false;
    // }

    [HarmonyPatch(typeof(SaveController), "ResetItemsOnFloorInNeighborhoods"), HarmonyPrefix]
    public static bool ResetItemsOnFloorInNeighborhoods()
    {
        var active = SceneManager.GetActiveScene().name;
        if (active != "house" && active != "pawn" && active != "junkyard_done" && active != "hideout_scene")
            return false;
        SaveController
                 .itemsSaved.savedItems
                 .RemoveAll(item =>
                      item.storagedIn is "broken" or "stolenAndSold" // this should respawn everything
                      || !item.isPicked && item.storagedIn is "neighborhood1" or "shopping_center"
                          or "Luxury_houses_scene" or "farm_scene" or "neighborhood2" or "factory"
                  );
        return false;
    }
}