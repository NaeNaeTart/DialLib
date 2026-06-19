using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialLib
{
    public static class DialogueRegistry
    {
        // Holds registered default lists of dialogue lines for custom keys
        private static readonly Dictionary<string, List<string>> _registeredDefaults = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registers a custom dialogue key with a list of default lines.
        /// If the key does not exist in the active language JSON file, these default lines will be used.
        /// </summary>
        public static void RegisterDialogue(string key, List<string> defaultLines)
        {
            if (string.IsNullOrEmpty(key)) return;
            _registeredDefaults[key] = defaultLines ?? new List<string>();
        }

        /// <summary>
        /// Registers a custom dialogue key with a single default line.
        /// </summary>
        public static void RegisterDialogue(string key, string defaultLine)
        {
            RegisterDialogue(key, new List<string> { defaultLine });
        }

        /// <summary>
        /// Tries to look up a custom dialogue key's default lines.
        /// </summary>
        public static bool TryGetDefaultLines(string key, out List<string> defaultLines)
        {
            return _registeredDefaults.TryGetValue(key, out defaultLines);
        }

        /// <summary>
        /// Returns all registered custom dialogue keys.
        /// </summary>
        public static IEnumerable<string> GetRegisteredKeys()
        {
            return _registeredDefaults.Keys;
        }

        /// <summary>
        /// Formats dialogue line placeholders (tokens) dynamically based on active player and context.
        /// Supports:
        /// <player> / <char> - Character Name
        /// <hp> - Consciousness percentage
        /// <blood> - Blood volume percentage
        /// <pain> - Average pain
        /// <adrenaline> - Adrenaline percentage
        /// <limb> - Lowercase name of targeted/injured limb
        /// <weapon> / <item> - Name of currently held weapon or item
        /// </summary>
        public static string FormatLine(string line, Body body, Limb? limb = null)
        {
            if (string.IsNullOrEmpty(line) || body == null) return line;

            string formatted = line;

            // 1. Character names
            string name = body.gameObject.name;
            if (string.IsNullOrEmpty(name) || name.Contains("Clone") || name.Contains("New Game"))
            {
                name = "Player";
            }
            formatted = formatted.Replace("<player>", name);
            formatted = formatted.Replace("<char>", name);

            // 2. Health and Vitals
            formatted = formatted.Replace("<hp>", $"{body.consciousness:F0}%");
            formatted = formatted.Replace("<blood>", $"{body.bloodVolume:F0}%");
            formatted = formatted.Replace("<pain>", $"{body.averagePain:F0}");
            formatted = formatted.Replace("<adrenaline>", $"{body.adrenaline:F0}%");

            // 3. Limb replacement
            if (limb != null)
            {
                formatted = formatted.Replace("<limb>", limb.shortName.ToLower());
            }
            else
            {
                formatted = formatted.Replace("<limb>", "body");
            }

            // 4. Held Items / Weapons
            if (formatted.Contains("<weapon>") || formatted.Contains("<item>"))
            {
                Item? heldItem = null;
                if (body.slots != null)
                {
                    for (int i = 0; i < body.slots.Length; i++)
                    {
                        if (body.HoldingItem(i))
                        {
                            heldItem = body.GetItem(i);
                            if (heldItem != null) break;
                        }
                    }
                }

                string itemName = "nothing";
                if (heldItem != null)
                {
                    try
                    {
                        itemName = Locale.GetItem(heldItem.id);
                        if (string.IsNullOrEmpty(itemName) || itemName == heldItem.id)
                        {
                            itemName = heldItem.fullName ?? heldItem.id;
                        }
                    }
                    catch
                    {
                        itemName = heldItem.id ?? "something";
                    }
                }

                formatted = formatted.Replace("<weapon>", itemName);
                formatted = formatted.Replace("<item>", itemName);
            }

            return formatted;
        }

        /// <summary>
        /// Formats an entire list of dialogue lines.
        /// </summary>
        public static List<string> FormatLines(List<string> lines, Body body, Limb? limb = null)
        {
            if (lines == null || lines.Count == 0 || body == null) return lines!;

            var result = new List<string>(lines.Count);
            foreach (var line in lines)
            {
                result.Add(FormatLine(line, body, limb));
            }
            return result;
        }
    }
}
