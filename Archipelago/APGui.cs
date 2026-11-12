using Thiefipelago.Added;
using UnityEngine;
using static Thiefipelago.Archipelago.ThiefClient;

namespace Thiefipelago.Archipelago;

public class APGui : GuiBuilder
{
    public string State = "";
    public Window NotConnectedBox;
    public Window ConnectedBox;

    protected override void InitGUI()
    {
        SetterGetter<string> addPortSetterGetter = new(
            () => ThiefClient.Data.AddressPort, s => ThiefClient.Data.AddressPort = s
        );
        SetterGetter<string> passwordSetterGetter = new(
            () => ThiefClient.Data.Password, s => ThiefClient.Data.Password = s
        );
        SetterGetter<string> slotNameSetterGetter = new(
            () => ThiefClient.Data.SlotName, s => ThiefClient.Data.SlotName = s
        );

        NotConnectedBox = new Window(0, 100, 100, 200, "AP Client")
           .AddChildren(
                new Label("Address:port", 30), new TextField(addPortSetterGetter, 25),
                new Label("Password", 30), new TextField(passwordSetterGetter, 25),
                new Label("Slot Name", 30), new TextField(slotNameSetterGetter, 25),
                new Spacer(30), new UiButton("Connect", 30, TryConnectClient),
                new Label(
                    new TextRef(() => State is not "" ? State : "Not Connected"), 20, Color.red,
                    alignment: TextAnchor.UpperLeft
                )
            );

        ConnectedBox = new Window(1, 100, 100, 200, "AP Client")
           .AddChildren(
                new Spacer(30), new UiButton("Disconnect", 30, () => Client.TryDisconnect()),
                new Label("Connected", 20, Color.green, alignment: TextAnchor.UpperLeft)
            );
    }

    protected override void OnGUI() { (!Client.IsConnected ? NotConnectedBox : ConnectedBox).Render(); }

    public void TryConnectClient()
    {
        var error = TryConnect(ThiefClient.Data.AddressPort, ThiefClient.Data.Password, ThiefClient.Data.SlotName);
        State = error is not null ? string.Join("\n", error) : "";
        if (error is not null) return;
        SaveFile();
    }
}