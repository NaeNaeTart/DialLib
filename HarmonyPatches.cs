using System;
using System.Collections.Generic;
using HarmonyLib;

namespace DialLib
{
    [HarmonyPatch]
    public static class HarmonyPatches
    {
        /// <summary>
        /// Postfixes Locale.GetCharacter to intercept missing key fallbacks.
        /// If the key is not defined in the JSON file (which natively returns new List<string> { key }),
        /// we substitute our registered default dialogue lines.
        /// </summary>
        [HarmonyPatch(typeof(Locale), "GetCharacter", new Type[] { typeof(string), typeof(int) })]
        [HarmonyPostfix]
        public static void GetCharacter_Postfix(string str, int chr, ref List<string> __result)
        {
            if (str != null && __result != null && __result.Count == 1 && __result[0] == str)
            {
                if (DialogueRegistry.TryGetDefaultLines(str, out var defaultLines))
                {
                    __result = new List<string>(defaultLines);
                }
            }
        }

        /// <summary>
        /// Prefixes Talker.Talk to replace rich formatting tokens (placeholders) on-the-fly.
        /// Reassigns the 'lines' parameter to a formatted duplicate list, avoiding
        /// permanently altering the static language dictionary arrays.
        /// </summary>
        [HarmonyPatch(typeof(Talker), "Talk", new Type[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        public static void Talk_Prefix(Talker __instance, ref List<string> lines, Limb limb)
        {
            if (lines == null || lines.Count == 0 || __instance == null || __instance.body == null) return;

            // Clone and format the lines to protect the static language dict templates
            lines = DialogueRegistry.FormatLines(lines, __instance.body, limb);
        }
    }
}
