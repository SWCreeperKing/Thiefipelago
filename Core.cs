using MelonLoader;
using Thiefipelago.Added;
using Thiefipelago.Archipelago;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly:
    MelonInfo(typeof(Thiefipelago.Core), "Thiefipelago", Thiefipelago.Core.VersionNumber, "SW_CreeperKing", null)]
[assembly: MelonGame("Noble Muffins", "Thief Simulator")]

namespace Thiefipelago;

// console -> ConsoleHud

// skill tree -> SkillsCanvasWindow; SkillUiButton

// tools -> ToolsShopCanvasWindow
// black bay -> AuctionCanvasWindow
// tips -> HintForumWindow
// secondary objectives -> HintForumWindow

// select destination -> TravelCanvasWindow
// pawnshop -> ShopCanvasWindow

// jewelry -> JewelryToolSpot
// buy van -> VehiclePurchaseContainer
public class Core : MelonMod
{
    public const string VersionNumber = "0.1.0";
    public const string DataFolder = "Mods/SW_CreeperKing.Thiefipelago/Data";

    public static MelonLogger.Instance Log;
    public static string Scene;
    public static APGui APGui = new();
    public static Dictionary<string, SkillAsset> Skills = [];
    public static Dictionary<string, ItemAsset> Tools = [];

    public DebugWindow DebugWindow;
    public bool ShowDebugWindow;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);
        ThiefClient.Init();

        DebugWindow = new DebugWindow(MelonAssembly.Assembly);
        KeyRegistry.AddKey(KeyCode.F5, () => ShowDebugWindow = !ShowDebugWindow);

        ThiefClient.ItemIdToLocation =
            File.ReadAllLines($"{DataFolder}/locationIds.txt")
                .Select(s => s.Split('|'))
                .ToDictionary(arr => arr[1], arr => arr[0]);

        ThiefClient.AbilityToolMap =
            File.ReadAllLines($"{DataFolder}/tools.txt")
                .Select(s => s.Split('|')).GroupBy(arr => arr[0])
                .ToDictionary(g => g.Key, g => (string[])[.. g.Select(arr => arr[1])]);

        Log.Msg("Initialized.");
    }

    public override void OnUpdate()
    {
        KeyRegistry.Update();
        if (LoadingController.instance.isLoading) return;
        ThiefClient.Update();
    }

    public override void OnGUI()
    {
        if (ShowDebugWindow) DebugWindow.Render();
        if (LoadingController.instance.isLoading) return;
        if (Scene is "mainmenu") APGui.Render();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Scene = sceneName;
        Log.Msg($"Scene Loaded: [{sceneName}]");

        switch (Scene)
        {
            case "nm logo":
            case "play way logo":
                Skills = [];
                Tools = [];
                break;
            case "mainmenu":
                GameObject[] containers =
                [
                    GameObject.Find(
                        "Game Gui camera New (1)/Game GUI/Main Menu/New profile/Item menu/Buttons container/Setting (8)"
                    ),
                    GameObject.Find(
                        "Game Gui camera New (1)/Game GUI/Main Menu/New profile/Item menu/Buttons container/Setting (9)"
                    )
                ];

                foreach (var go in containers)
                {
                    foreach (var child in go.GetChildren()) child.SetActive(false);
                }

                Skills = [];
                Tools = [];
                break;
            default:
                if (Skills.Count == 0)
                {
                    Skills = Resources.FindObjectsOfTypeAll<SkillAsset>().ToDictionary(
                        s => $"Progressive {s.skillName.Split("lvl")[0].Trim()},{s.name.Split("lvl")[^1].Trim()}",
                        s => s
                    );
                }

                if (Tools.Count == 0)
                {
                    var rawItems = Resources.FindObjectsOfTypeAll<ItemAsset>()
                                            .Where(i => i?.itemCategory?.categoryName is "TOOLS"
                                                        && i.name is not ("Under Door Camera" or "Decryption tools")
                                             );
                    Tools = rawItems
                       .ToDictionary(i => i.FunnyId, i => i);
                }
                // File.WriteAllLines("items.txt", rawItems.Select(t => $"[{t?.itemCategory?.categoryName}] = [{t.itemName}] = [{t.toolId}] = [{t.requiredSkillToPurchase?.skillName}] = [{t.itemType}] = [{t.GetPrice()}]").OrderBy(s => s, StringComparer.Ordinal));
                // Log.Msg(
                //     $"\n{string.Join('\n', PlayerInventory.itemsList
                //                                           .Where(i => i.itemAsset.itemCategory.categoryName is "TOOLS")
                //                                           .Select(i => i.itemAsset.FunnyId))}"
                // );
                break;
        }
    }
}

public static class Extensions
{
    extension(ItemAsset item)
    {
        public string FunnyId
            => $"{item.itemName} {item.toolId} {item.requiredSkillToPurchase?.skillName} {item.itemType} {item.GetPrice()}"
               .Replace(' ', '_');
    }
}