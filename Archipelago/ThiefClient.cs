using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Thiefipelago.Patches.SaveChanges;

namespace Thiefipelago.Archipelago;

public static class ThiefClient
{
    public static Dictionary<string, string> ItemIdToLocation = [];
        
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static string GameUUID = "";
    public static Dictionary<string, string[]> AbilityToolMap = [];

    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () =>
        {
            Core.Log.Error("Lost Connection to Ap");
        };

        Client.OnConnectionEvent += _ =>
        {
            ItemHandler.HandlerReset();
            try
            {
                Client.GetFromStorageAsync("item_count", i => ItemHandler.TotalItemCount = i, Scope.Slot, 0);
                
                GameUUID = (string)Client.SlotData["uuid"];
                var path = $"{Directory.GetCurrentDirectory()}/Archipelago/{GameUUID}";
                Directory.CreateDirectory(path);
                SaveChangeDirectory.SaveFilePrefix = path;
                LoadingController.instance.LoadScene("mainmenu");
                CheckGoal();
            }
            catch (Exception e) { Core.Log.Error(e); }
        };

        Client.ItemHandlerInitialized += handler =>
        {
            handler.OnItemReceived += (item, _) => ItemHandler.HandleItem(item);
        };

        Client.OnConnectionErrorReceived += (e, _) => Core.Log.Error(e);
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "Thief Simulator 1", ItemsHandlingFlags.AllItems);
    }

    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));

    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;
            if (Core.Skills.Count != 24) return; 
            if (Core.Scene is "nm logo" or "mainmenu" or "play way logo") return;
            if (PlayerInventory.instance is null) return;
            if (PlayerInventory.itemsList is null) return;
            Client.UpdateItemHandler();
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    public static void SendLocation(string loc)
    {
        if (!Client.IsMissingLocation(loc)) return;
        Client.SendLocation(loc);
    }

    public static void CheckGoal()
    {
        if (Client is null) return;
        if (!Client.IsConnected) return;
        if (Client.MissingLocations.Any()) return;
        Client.TryGoal();
    }
}