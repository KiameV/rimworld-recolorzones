﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ReColorStockpile
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.recolorstockpile.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message(
                "ReColorStockpiles: Harmony Patches:\n" +
                "    Postfix:\n" +
                "        Zone_Stockpile.GetGizmos(IEnumerable<Gizmo>)\n" +
                "        Zone_Growing.GetGizmos(IEnumerable<Gizmo>)");
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_ZoneStockpile_GetGizmos
    {
        static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            List<Gizmo> l = new List<Gizmo>(__result);
            l.Add(new Command_Action
            {
                icon = Dialog.ColorSelectDialog.ChangeColorTexture,
                defaultLabel = "ReColorStockpile.ChangeColor".Translate(),
                defaultDesc = "ReColorStockpile.ChangeColorDesc".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate { Find.WindowStack.Add(new Dialog.ColorSelectDialog(__instance)); },
                groupKey = 987767550
            });
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
            a.icon = Dialog.ColorSelectDialog.ChangeColorTexture;
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
