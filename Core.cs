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
    public static SkillAsset[] Skills = [];
    public static ItemAsset[] Tools = [];

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

        Log.Msg("Initialized.");
    }

    public override void OnUpdate()
    {
        KeyRegistry.Update();
        ThiefClient.Update();
    }

    public override void OnGUI()
    {
        if (Scene is "mainmenu") APGui.Render();
        if (ShowDebugWindow) DebugWindow.Render();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Scene = sceneName;
        Log.Msg($"Scene Loaded: [{sceneName}]");

        switch (Scene)
        {
            case "house":
                Skills = Resources.FindObjectsOfTypeAll<SkillAsset>();
                var rawItems = Resources.FindObjectsOfTypeAll<ItemAsset>();
                Tools = [.. rawItems.Where(i => i.itemType is ItemAsset.ItemTypes.Tool)];
                // File.WriteAllLines("items.txt", rawItems.Select(t => $"[{t?.itemCategory?.categoryName}] = [{t.itemName}] = [{t.toolId}] = [{t.requiredSkillToPurchase?.skillName}] = [{t.itemType}] = [{t.GetPrice()}]").OrderBy(s => s, StringComparer.Ordinal));
                Log.Msg(
                    $"\n{string.Join('\n', PlayerInventory.itemsList
                                                          .Where(i => i.itemAsset.itemCategory.categoryName is "TOOLS")
                                                          .Select(i => i.itemAsset.FunnyId))}"
                );
                break;
            case "mainmenu":
                Skills = [];
                Tools = [];
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