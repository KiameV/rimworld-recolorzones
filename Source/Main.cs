using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ReColorStockpile
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.savestoragesettings.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("SaveStorageSettings: Adding Harmony Postfix to Zone_Stockpile.GetGizmos(IEnumerable<Gizmo>)");
            Log.Message("SaveStorageSettings: Adding Harmony Postfix to Zone_Growing.GetGizmos(IEnumerable<Gizmo>)");

            Dialog.ColorSelectDialog.ChangeColorTexture = ContentFinder<Texture2D>.Get("UI/changecolor", true);
            Dialog.ColorSelectDialog.ColorPickerTexture = ContentFinder<Texture2D>.Get("UI/colorpicker", true);

            foreach (ModContentPack current in LoadedModManager.RunningMods)
            {
                if (current.GetContentHolder<Texture2D>().Get("UI/colorpicker"))
                {
                    byte[] data = File.ReadAllBytes(current.RootDir + "/Textures/UI/colorpicker.png");
                    Dialog.ColorSelectDialog.ColorPickerTexture = new Texture2D(2, 2, TextureFormat.Alpha8, true);
                    Dialog.ColorSelectDialog.ColorPickerTexture.LoadImage(data, false);
                    break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_ZoneStockpile_GetGizmos
    {
        static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            List<Gizmo> l = new List<Gizmo>(__result);
            Command_Action a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/changecolor", true);
            a.defaultLabel = "ReColorStockpile.ChangeColor".Translate();
            a.defaultDesc = "ReColorStockpile.ChangeColorDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new Dialog.ColorSelectDialog(__instance)); };
            a.groupKey = 987767550;
            l.Add(a);
            __result = l;
        }
    }

    [HarmonyPatch(typeof(Zone_Growing), "GetGizmos")]
    static class Patch_ZoneGrowing_GetGizmos
    {
        static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            List<Gizmo> l = new List<Gizmo>(__result);
            Command_Action a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/changecolor", true);
            a.defaultLabel = "ReColorStockpile.ChangeColor".Translate();
            a.defaultDesc = "ReColorStockpile.ChangeColorDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new Dialog.ColorSelectDialog(__instance)); };
            a.groupKey = 987767550;
            l.Add(a);
            __result = l;
        }
    }
}
