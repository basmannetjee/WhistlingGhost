using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace WhistlingGhost
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static AudioClip Audio;
        private void Awake()
        {
            string location = Info.Location.TrimEnd($"{PluginInfo.PLUGIN_GUID}.dll".ToCharArray());
            UnityWebRequest audioLoader = UnityWebRequestMultimedia.GetAudioClip($"File://{location}deathwhistle.mp3", AudioType.MPEG);

            audioLoader.SendWebRequest();
            while (!audioLoader.isDone) { }

            if (audioLoader.result == UnityWebRequest.Result.Success)
            {
                Audio = DownloadHandlerAudioClip.GetContent(audioLoader);

                new Harmony(PluginInfo.PLUGIN_GUID).PatchAll(typeof(GirlPatch));
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            }
            else
            {
                Logger.LogError($"Could not load audio file");
            }
        }
    }

    [HarmonyPatch(typeof(DressGirlAI))]
    internal class GirlPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void AudioPatch(DressGirlAI __instance)
        {
            __instance.breathingSFX = Plugin.Audio;
        }
    }
}