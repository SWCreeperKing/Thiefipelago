using MelonLoader;
using Thiefipelago.Added;

[assembly: MelonInfo(typeof(Thiefipelago.Core), "Thiefipelago", "0.1.0", "Pizza", null)]
[assembly: MelonGame("Noble Muffins", "Thief Simulator")]

namespace Thiefipelago
{
    public class Core : MelonMod
    {
        public static MelonLogger.Instance Log;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            
            HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);
            
            LoggerInstance.Msg("Initialized.");
        }
    }
}