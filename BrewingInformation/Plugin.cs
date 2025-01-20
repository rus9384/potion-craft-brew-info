using UnityEngine;
using BepInEx;
using HarmonyLib;
using PotionCraft.ObjectBased.Mortar;
using PotionCraft.LocalizationSystem;
using PotionCraft.ObjectBased;
using PotionCraft.DebugObjects.DebugWindows;
using System.Collections.Generic;

namespace BrewingInformation
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class BrewingInformation : BaseUnityPlugin
    {
        #region Fields and Properties
        internal static bool enableGrindStatus;
        internal static bool enableHealth;
        internal static bool enablePotionCoordinates;
        internal static bool enablePotionRotation;
        internal static bool enablePathAngle;
        internal static float scale;
        internal static bool enableScratches;

        internal static Room laboratory;
        private static DebugWindow window;

        private static System.Reflection.FieldInfo healthFI;

        internal static bool needsUpdating = false;

        private static float _health;
        private static float? _grind;
        private static Vector2 _position;
        private static float _rotation;
        private static float? _pathAngle;

        internal static float Health
        {
            get => _health;
            set
            {
                if (value != _health)
                {
                    _health = value;
                    needsUpdating = true;
                }
            }
        }

        internal static float? Grind 
        { 
            get => _grind; 
            set 
            {
                if (value != _grind)
                {
                    _grind = value;
                    needsUpdating = true;
                }
            } 
        }

        internal static Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    needsUpdating = true;
                }
            }
        }

        internal static float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    needsUpdating = true;
                }
            }
        }

        internal static float? PathAngle
        {
            get => _pathAngle;
            set
            {
                if (_pathAngle != value)
                {
                    _pathAngle = value;
                    needsUpdating = true;
                }
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(BrewingInformation));

            enableGrindStatus = Config.Bind("General", "EnableGrindStatus", true).Value;
            enableHealth = Config.Bind("General", "EnableHealth", true).Value;
            enablePotionCoordinates = Config.Bind("General", "EnablePotionCoordinates", true).Value;
            enablePotionRotation = Config.Bind("General", "EnablePotionRotation", true).Value;
            enablePathAngle = Config.Bind("General", "EnablePathAngle", true).Value;
            scale = Config.Bind("Graphics", "Scale", 100f, "Window scale in %").Value;
            enableScratches = Config.Bind("Graphics", "EnableScratches", true, "Set this to 'false' if you want a cleaner but less authentic view").Value;

            LocalizationManager.OnInitialize.AddListener(SetModLocalization);

            healthFI = AccessTools.Field(typeof(PotionCraft.ObjectBased.RecipeMap.RecipeMapItem.IndicatorMapItem.IndicatorMapItem), "health");
        }

        private void Update()
        {
            if (window == null)
                window = UIElements.CreateWindow();

            if (needsUpdating)
            {
                window.ShowText(GetInfo());
                needsUpdating = false;
            }
        }

        private static void SetModLocalization()
        {
            RegisterLoc("#BrewingInformationTitle", "Information");
        }

        private static void RegisterLoc(string key, string en)
        {
            for (int localeIndex = 0; localeIndex <= 13; ++localeIndex)
                AccessTools.StaticFieldRefAccess<PotionCraft.Assemblies.DataBaseSystem.PreparedObjects.LocalizationData>(typeof(LocalizationManager), "localizationData").Add(localeIndex, key, en);
        }

        internal static string GetInfo()
        {
            List<string> infos = [];

            if (enableGrindStatus)
                infos.Add($"Grind: {Grind:P2}");
            if (enableHealth)
                infos.Add($"Health: {Health:P2}");
            if (enablePotionCoordinates)
                infos.Add($"x: {Position.x:#0.####}, y: {Position.y:#0.####}");
            if (enablePotionRotation)
                infos.Add($"Angle: {Rotation:##0.###}");
            if (enablePathAngle)
                infos.Add($"Path Angle: {PathAngle:##0.###}");

            return string.Join("\n", infos);
        }
        #endregion

        #region Patches
        [HarmonyPatch(typeof(Room), "Awake")]
        [HarmonyPostfix]
        public static void Room_Awake_Patch(Room __instance)
        {
            if (__instance.roomIndex != RoomIndex.Laboratory)
                return;
            laboratory = __instance;
        }

        [HarmonyPatch(typeof(DebugWindow), "Update")]
        [HarmonyPostfix]
        private static void DebugWindowUpdatePatch(DebugWindow __instance)
        {
            if (__instance == window && __instance.transform.localPosition != UIElements.position)
                __instance.transform.localPosition = UIElements.position;
        }

        [HarmonyPatch(typeof(Mortar), "Update")]
        [HarmonyPostfix]
        private static void MortarUpdateInfo(Mortar __instance)
        {
            if (!enableGrindStatus) 
                return;

            if (__instance.ContainedStack != null)
                Grind = Mathf.Clamp01(__instance.ContainedStack.overallGrindStatus);
            else
                Grind = null;
        }

        [HarmonyPatch(typeof(PotionCraft.ManagersSystem.RecipeMap.RecipeMapManager), "Update")]
        [HarmonyPostfix]
        private static void RecipeMapUpdateInfo(PotionCraft.ManagersSystem.RecipeMap.RecipeMapManager __instance)
        {
            if (!enableHealth && !enablePotionCoordinates && !enablePotionRotation && !enablePathAngle)
                return;

            if (__instance.indicator == null || __instance.indicatorRotation == null || __instance.path == null)
                return;

            if (enableHealth)
                Health = (float)healthFI.GetValue(__instance.indicator);
            if (enablePotionCoordinates)
                Position = __instance.indicator.TargetPosition;
            if (enablePotionRotation)
                Rotation = __instance.indicatorRotation.Value;

            if (enablePathAngle)
            {
                float? pathAngle = null;
                PotionCraft.ObjectBased.RecipeMap.Path.FixedHint pathHint = __instance.path.fixedPathHints.Find(hint => hint.evenlySpacedPointsFixedPhysics.points.Length > 1);
                if (pathHint != default)
                {
                    Vector3 segment = pathHint.evenlySpacedPointsFixedPhysics.points[1] - pathHint.evenlySpacedPointsFixedPhysics.points[0];
                    pathAngle = (Mathf.Atan2(-segment.x, segment.y) * 180f / Mathf.PI + 360f) % 360f;
                }
                PathAngle = pathAngle;
            }
        }

        [HarmonyPatch(typeof(DebugWindow), "RecalculateWindowSize")]
        [HarmonyPrefix]
        private static bool DebugWindowRecalculateWindowSizePatch(DebugWindow __instance) => __instance != window;

        [HarmonyPatch(typeof(DarkScreenSystem.DarkScreen), "ShowObject")]
        [HarmonyPatch(typeof(PotionCraft.ManagersSystem.Menu.MenuManager), "OpenMainMenu")]
        [HarmonyPatch(typeof(PotionCraft.ManagersSystem.Menu.MenuManager), "OpenPauseMenu")]
        [HarmonyPostfix]
        private static void HideWindow()
        {
            if (window != null)
                window.Visible = false;
        }

        [HarmonyPatch(typeof(DarkScreenSystem.DarkScreen), "HideObject")]
        [HarmonyPatch(typeof(PotionCraft.ManagersSystem.Menu.MenuManager), "CloseMenu")]
        [HarmonyPostfix]
        private static void ShowWindow()
        {
            if (window != null)
                window.Visible = true;
        }
        #endregion
    }
}
