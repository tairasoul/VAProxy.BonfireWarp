using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BonfireWarp
{
    public static class DictionaryExtensions
    {
        public static string GetKey(this Dictionary<string, string> dict, string value)
        {
            foreach (string key in dict.Keys)
            {
                if (dict.GetValueSafe(key) == value) return key;
            }
            return null;
        }
    }

    [BepInPlugin("tairasoul.vaproxy.bonfirewarp", "BonfireWarp", "1.1.2")]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony harmony = new Harmony("tairasoul.vaproxy.bonfirewarp");
        private static Dictionary<string, string> NameReferences = new Dictionary<string, string>();
        public static ManualLogSource Log;
        private static Window WindowInstance;
        private void Awake()
        {
            Log = Logger;
            NameReferences.Add("Station", "Scrap Pits");
            NameReferences.Add("Station (1)", "Tower Base");
            NameReferences.Add("Station (2)", "ERI Grave");
            NameReferences.Add("Station (3)", "Rusty Outskirts, City");
            NameReferences.Add("Station (4)", "Rusty Outskirts, Fireplace");
            NameReferences.Add("Station (5)", "Upper Sewers");
            NameReferences.Add("Station (6)", "Heating Structure F1");
            NameReferences.Add("Station (7)", "Lectors Archive");
            NameReferences.Add("Station (8)", "ERI Grave, Front of ERI");
            NameReferences.Add("Station (9)", "Rusty Outskirts, Buckets");
            NameReferences.Add("Station (10)", "Rusty Outskirts, Castle");
            harmony.PatchAll();
        }

        bool init = false;

        internal void OnDestroy() => Init();
        internal void Start() => Init();

        internal void Init()
        {
            if (!init)
            {
                init = true;
                GameObject window = new GameObject("BonfireWarp");
                WindowInstance = window.AddComponent<Window>();
                DontDestroyOnLoad(window);
                foreach (string NameReference in NameReferences.Values)
                {
                    if (PlayerPrefs.GetInt(NameReference, 0) == 0)
                    {
                        PlayerPrefs.SetInt(NameReference, 0);
                    }
                    else
                    {
                        WindowInstance.StartCoroutine(WindowInstance.AddButton(NameReference, NameReferences.GetKey(NameReference)));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Station))]
        static class StationPatches
        {
            [HarmonyPatch("sit")]
            [HarmonyPostfix]
            static void Postfix(ref Station __instance)
            {
                string NameRef = NameReferences.GetValueSafe(__instance.name);
                bool VisitedStation = PlayerPrefs.GetInt(NameRef) == 1;
                if (!VisitedStation)
                {
                    PlayerPrefs.SetInt(NameRef, 1);
                    WindowInstance.StartCoroutine(WindowInstance.AddButton(NameRef, __instance.name));
                }
            }
        }
    }
}
