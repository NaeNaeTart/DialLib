using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace DialLib
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("CasualtiesUnknown.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log = null!;
        private Harmony? _harmony;

        private void Awake()
        {
            Log = base.Logger;
            Log.LogInfo($"DialLib (Dialogue Library) version {MyPluginInfo.PLUGIN_VERSION} is waking up!");

            // Apply Harmony patches
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(typeof(HarmonyPatches));
            Log.LogInfo("Central Dialogue and Translation hooks applied successfully.");

            // Register default lines for "laststand" to satisfy the user's requested example immediately
            DialogueRegistry.RegisterDialogue("laststand", new List<string>
            {
                "I'm not dying here! Not like this!",
                "Still breathing... still standing!",
                "Is that all you've got?! I can still swing my <weapon>!",
                "My <limb> is broken, but I'm not finished yet!",
                "This is my last stand... and I'm taking you down with me!"
            });

            Log.LogInfo("Default 'laststand' dialogue registered successfully with DialLib.");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }

    public static class MyPluginInfo
    {
        public const string PLUGIN_GUID = "com.kanisuko.diallib";
        public const string PLUGIN_NAME = "DialLib";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
