using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using static ModHelper.UI.Elements.OptionElement;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : BasePanel
    {
        public List<ModSourcesElement> modSourcesElements = [];

        // enabled mods
        private readonly List<ModElement> modElements = [];
        private readonly OptionElement toggleAllEnabledMods;

        // all mods
        private readonly List<ModElement> allMods = [];
        private readonly OptionElement toggleAllAllMods;

        #region Constructor
        public ModsPanel() : base(header: "Mods")
        {
            // Active = true; // uncomment to show the panel by default
            AddPadding(5);
            AddHeader("Mod Sources", GoToModSources, "Click to exit world and go to mod sources");
            ConstructModSources();
            AddPadding();

            AddHeader("Enabled Mods", onLeftClick: GoToModsList, "Click to exit world and go to mods list");
            ConstructEnabledMods();
            toggleAllEnabledMods = AddOption("Toggle All", leftClick: ToggleAllEnabledMods, hover: "Toggle all enabled mods on or off");
            toggleAllEnabledMods.SetState(State.Enabled);
            AddPadding();

            AddHeader("All Mods", onLeftClick: GoToModsList, "Click to exit world and go to mods list");
            ConstructAllMods();
            toggleAllAllMods = AddOption("Toggle All", leftClick: ToggleAllAllMods, hover: "Toggle all disabled mods on or off");
            AddPadding();
            AddPadding(3f);
        }
        #endregion

        #region Constructing mod lists

        private void ConstructModSources()
        {
            Log.Info("Constructing Mod Sources");
            // Get all the mod sources paths
            foreach (string fullModPath in GetModSourcesPaths())
            {
                // Get the clean name
                string cleanName = GetModSourcesCleanName(fullModPath);

                // Cut to max 20 chars
                if (cleanName.Length > 20)
                    cleanName = string.Concat(cleanName.AsSpan(0, 20), "...");

                ModSourcesElement modSourcesElement = new(fullModPath: fullModPath, cleanName: cleanName);
                modSourcesElements.Add(modSourcesElement);
                uiList.Add(modSourcesElement);
                AddPadding(3);
            }
        }

        private void ConstructEnabledMods()
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (Mod mod in mods)
            {
                // you could change this to send the "clean name"
                // this is where we set the text of the mod element
                ModElement modElement = new(mod.DisplayNameClean, mod.Name);
                uiList.Add(modElement);
                modElements.Add(modElement);
                AddPadding(3);
            }
        }

        private void ConstructAllMods()
        {
            List<object> sortedMods = GetAllWorkshopMods();

            // Get all mods the user has installed via reflection
            foreach (var mod in sortedMods) // "mod" is of type LocalMod
            {
                // Get the clean name using reflection for the LocalMod mod.
                string cleanName = GetCleanName(mod);
                string internalName = mod.ToString();

                // Skip mods that are already in the enabled mods list
                // to avoid duplicates.
                if (modElements.Any(modElement => modElement.internalName == internalName))
                {
                    continue;
                }

                // Log.Info("InternalName: " + internalName + " CleanName: " + cleanName);

                // We want to pass the LocalMod's TmodFile to GetModIconFromAllMods
                object tmod = getTmodFile(mod);

                Texture2D modIcon = GetModIconFromAllMods(tmod);

                ModElement modElement = new(
                    cleanModName: cleanName,
                    internalModName: internalName,
                    icon: modIcon
                    );

                modElement.SetState(State.Disabled);
                uiList.Add(modElement);
                allMods.Add(modElement);
                AddPadding(3);
            }
        }

        #endregion

        #region Helpers for getting mod lists
        // Note: Lots of reflection is used here, so be careful with error handling.

        // Helper method to get all workshop mods
        private static List<object> GetAllWorkshopMods()
        {
            // Get all mods the user has installed via reflection
            try
            {
                Assembly assembly = typeof(ModLoader).Assembly;
                Type modOrganizerType = assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
                MethodInfo findWorkshopModsMethod = modOrganizerType.GetMethod("FindWorkshopMods", BindingFlags.NonPublic | BindingFlags.Static);

                var workshopMods = (IReadOnlyList<object>)findWorkshopModsMethod.Invoke(null, null);

                // Sort workshop mods by clean name
                var sortedMods = workshopMods.ToList();
                sortedMods.Sort((a, b) =>
                {
                    // Get clean names for comparison
                    string nameA = GetCleanName(a);
                    string nameB = GetCleanName(b);
                    return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
                });

                Log.Info("Found " + sortedMods.Count + " workshop mods.");
                return sortedMods;
            }
            catch
            {
                Log.Warn("An error occurred while retrieving workshop mods.");
            }
            return [];
        }

        // Helper method to get clean name from a mod object
        private static string GetCleanName(object mod)
        {
            FieldInfo displayNameField = mod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
            return (string)displayNameField.GetValue(mod);
        }

        private Texture2D GetModIconFromAllMods(object TmodFile)
        {
            try
            {
                // Check if the file exists
                MethodInfo hasFileMethod = TmodFile.GetType().GetMethod("HasFile", BindingFlags.Public | BindingFlags.Instance);
                bool hasIcon = (bool)hasFileMethod.Invoke(TmodFile, ["icon.png"]);
                if (!hasIcon)
                {
                    Log.Warn("The TmodFile does not have an icon.");
                    return null;
                }

                // Retrieve the Open method (no parameters).
                MethodInfo openMethod = TmodFile.GetType().GetMethod("Open", BindingFlags.Public | BindingFlags.Instance);

                // Retrieve the GetStream method that takes a string and a bool.
                MethodInfo getStreamMethod = TmodFile.GetType().GetMethod(
                    "GetStream",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    [typeof(string), typeof(bool)],
                    null
                );

                // Use a nested using block so both the open result and the stream are disposed.
                using var openResult = openMethod.Invoke(TmodFile, []) as IDisposable;

                // Get the stream for "icon.png". Note we pass both parameters.
                using Stream s = (Stream)getStreamMethod.Invoke(TmodFile, ["icon.png", true]);

                Asset<Texture2D> iconTexture = Main.Assets.CreateUntracked<Texture2D>(s, ".png", AssetRequestMode.ImmediateLoad);

                // note: Probably MUST be immediateLoad, otherwise it doesnt show up.

                // Log.Info("Successfully loaded icon from TmodFile.");
                return iconTexture.Value;
            }
            catch (Exception ex)
            {
                Log.Info("Error while retrieving icon from TmodFile via reflection: " + ex);
            }
            return null;
        }

        private string GetModSourcesCleanName(string modFolder)
        {
            // Get the assembly and the ModCompile type.
            Assembly assembly = typeof(ModLoader).Assembly;
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");

            // Get the non-public nested type "ConsoleBuildStatus".
            Type consoleBuildStatusType = modCompileType.GetNestedType("ConsoleBuildStatus", BindingFlags.NonPublic);
            // Create an instance of ConsoleBuildStatus.
            object consoleBuildStatusInstance = Activator.CreateInstance(consoleBuildStatusType, nonPublic: true);

            // Create an instance of ModCompile using the constructor that takes an IBuildStatus.
            object modCompileInstance = Activator.CreateInstance(
                modCompileType,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                [consoleBuildStatusInstance],
                null);

            // Retrieve the private instance method ReadBuildInfo.
            MethodInfo readBuildInfoMethod = modCompileType.GetMethod("ReadBuildInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            // Invoke the method on the instance.
            object buildingMod = readBuildInfoMethod.Invoke(modCompileInstance, [modFolder]);

            // Since DisplayNameClean is a field, use GetField instead of GetProperty.
            FieldInfo displayNameField = buildingMod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
            return (string)displayNameField?.GetValue(buildingMod);
        }

        private List<string> GetModSourcesPaths()
        {
            List<string> strings = [];

            // 1. Getting Assembly 
            Assembly assembly = typeof(Main).Assembly;

            // 2. Gettig method for finding modSources paths
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            for (int i = 0; i < modSources.Length; i++)
            {
                strings.Add(modSources[i]);
            }
            return strings;
        }

        private object getTmodFile(object mod)
        {
            Assembly assembly = typeof(ModLoader).Assembly;
            Type localModType = assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo modFileField = localModType.GetField("modFile", BindingFlags.Public | BindingFlags.Instance);
            object tmod = modFileField.GetValue(mod);
            return tmod;
        }

        private object getLocalModName(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("Name", BindingFlags.Public | BindingFlags.Instance);
            object name = Name.GetValue(mod);
            return name;
        }

        private object getLastModified(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("lastModified", BindingFlags.Public | BindingFlags.Instance);
            object lastModified = Name.GetValue(mod);
            return lastModified;
        }

        #endregion

        #region Toggle all methods

        private void ToggleAllAllMods()
        {
            // Determine the new state based on whether all mods are currently enabled
            bool anyDisabled = allMods.Any(modElement => modElement.GetState() == State.Disabled);
            State newState = anyDisabled ? State.Enabled : State.Disabled;

            // Set the state for all mod elements
            foreach (ModElement modElement in allMods)
            {
                modElement.SetState(newState);
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, newState == State.Enabled]);
            }

            // Update the "Toggle All" option's state
            toggleAllAllMods.SetState(newState);
        }

        private void ToggleAllEnabledMods()
        {
            // Determine the new state based on whether all mods are currently enabled
            bool anyDisabled = modElements.Any(modElement => modElement.GetState() == State.Disabled);
            State newState = anyDisabled ? State.Enabled : State.Disabled;

            // Set the state for all mod elements
            foreach (ModElement modElement in modElements)
            {
                modElement.SetState(newState);
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, newState == State.Enabled]);
            }

            // Update the "Toggle All" option's state
            toggleAllEnabledMods.SetState(newState);
        }

        #endregion

        #region Navigation methods
        private void GoToModSources()
        {
            WorldGen.JustQuit();
            Main.menuMode = 10001;
        }

        private void GoToModsList()
        {
            WorldGen.JustQuit();
            Main.menuMode = 10000;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }
            base.Update(gameTime);
        }

        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            // first draw everything in the panel
            base.Draw(spriteBatch);

            // last, draw the hover texture
            foreach (var element in uiList._items.ToList())
            {
                if (element is ModElement modElement)
                {
                    var icon = modElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.updatedTex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 4, Main.mouseY - icon.Height.Pixels * 2);
                        spriteBatch.Draw(icon.updatedTex, mousePos, Color.White);
                    }
                }
                else if (element is ModSourcesElement modSourcesElement)
                {
                    var icon = modSourcesElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.tex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 4, Main.mouseY - icon.Height.Pixels * 2);
                        spriteBatch.Draw(icon.tex, mousePos, Color.White);

                        // Determine the color based on the time ago
                        TimeSpan timeAgo = DateTime.Now - icon.lastModified;
                        Color timeColor = timeAgo.TotalSeconds < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalMinutes < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalHours < 24 ? Color.Orange :
                                          Color.Red;

                        string builtAgo = ConvertLastModifiedToTimeAgo(icon.lastModified);

                        if (!string.IsNullOrEmpty(builtAgo))
                        {
                            Utils.DrawBorderString(
                                spriteBatch,
                                text: $"Built {builtAgo}",
                                new Vector2(mousePos.X + icon.Width.Pixels, mousePos.Y - 10),
                                timeColor,
                                scale: 1.0f,
                                0.5f,
                                0.5f
                            );
                        }
                    }
                }
            }
        }
        #endregion

        private static string ConvertLastModifiedToTimeAgo(DateTime lastModified)
        {
            TimeSpan timeAgo = DateTime.Now - lastModified;
            if (timeAgo.TotalSeconds < 60)
            {
                return $"{timeAgo.Seconds} seconds ago";
            }
            else if (timeAgo.TotalMinutes < 2)
            {
                return $"{timeAgo.Minutes} minute ago";
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return $"{timeAgo.Minutes} minutes ago";
            }
            else if (timeAgo.TotalHours < 2)
            {
                return $"{timeAgo.Hours} hour ago";
            }
            else if (timeAgo.TotalHours < 24)
            {
                return $"{timeAgo.Hours} hours ago";
            }
            else if (timeAgo.TotalDays < 2)
            {
                return $"{timeAgo.Days} day ago";
            }
            else
            {
                return $"{timeAgo.Days} days ago";
            }
        }
    }
}
