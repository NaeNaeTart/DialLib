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

            // 1. Boss / Heavy Threat Spotted Mode
            DialogueRegistry.RegisterDialogue("boss_spotted", new List<string>
            {
                "What... what is that thing?!",
                "That is NOT a normal specimen! Look at the size of it!",
                "I don't think my <weapon> is going to be enough for this...",
                "My heart is at <adrenaline> adrenaline just looking at that monster!"
            });

            // 2. Out of Ammo / Reloading Mode
            DialogueRegistry.RegisterDialogue("low_ammo", new List<string>
            {
                "Click... empty! Need to reload!",
                "I'm out! Covering fire while I load my <weapon>!",
                "Need a fresh mag! Just give me a second!"
            });

            // 3. Heavy Bleeding Mode
            DialogueRegistry.RegisterDialogue("critical_bleed", new List<string>
            {
                "I'm leaking... so much blood...!",
                "I need a bandage now! I've lost <blood> of my blood!",
                "Everything is turning red... I'm bleeding out!"
            });

            // 4. Fracture / Bone Break Mode
            DialogueRegistry.RegisterDialogue("broken_bone", new List<string>
            {
                "Crack... my <limb>! I think it's broken!",
                "Ah! I can't move my <limb>!",
                "That hurt! My <limb> is fractured!"
            });

            // 5. Treatment / Healing Mode
            DialogueRegistry.RegisterDialogue("healed_limb", new List<string>
            {
                "Bandage is secure. My <limb> feels a bit better.",
                "Applied the dressing. Pain is down to <pain>.",
                "That's patched up. Back to the fight!"
            });

            // 6. Stumbling / Tripping Mode
            DialogueRegistry.RegisterDialogue("stumble", new List<string>
            {
                "Whoa! Watch your footing!",
                "Tripped on the loose rocks! Nearly broke my <limb>!",
                "Ugh! Stupid loose gravel!"
            });

            Log.LogInfo("Default dialogue modes ('laststand', 'boss_spotted', 'low_ammo', 'critical_bleed', 'broken_bone', 'healed_limb', 'stumble') registered successfully with DialLib.");
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
