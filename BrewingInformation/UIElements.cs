using UnityEngine;
using PotionCraft.DebugObjects.DebugWindows;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem;

namespace BrewingInformation
{
    public static class UIElements
    {
        internal static Vector3 position;

        public static DebugWindow CreateWindow()
        {
            if (BrewingInformation.laboratory == null)
                return null;

            DebugWindow window = DebugWindow.Init(LocalizationManager.GetText("#BrewingInformationTitle"), true);
            window.transform.SetParent(BrewingInformation.laboratory.transform, false);
            window.ShowText(BrewingInformation.GetInfo());
           
            Vector2 size = window.spriteBackground.size;
            size.x = 3.29f; // Default width
            window.spriteBackground.size = size;
            window.spriteScratches.size = size;
            window.colliderBackground.size = Vector2.zero; // Disable collisions for the window
            window.colliderBackground.offset = Vector2.zero;

            float scale = BrewingInformation.scale / 100f; // User defined window scale
            window.transform.localScale *= scale;
            position = new Vector3(6.14f - 3.29f * scale, 6.95f, 0f); // Upper right corner of the recipe map

            window.transform.position = position;

            window.movable = false; // The window is pinned to the upper right corner
            window.Visible = Managers.Menu.CurrentMenu == PotionCraft.ManagersSystem.Menu.MenuManager.MenuScreen.Off; // Hide the window if the menu is open
            window.transform.Find("Maximized/Background/Scratches").gameObject.SetActive(BrewingInformation.enableScratches);
            window.transform.Find("Maximized/Head").gameObject.SetActive(false); // Hide "Minimize" and "Close" buttons
            return window;
        }
    }
}
