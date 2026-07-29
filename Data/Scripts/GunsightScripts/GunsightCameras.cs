using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Game.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Input;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;
using VRage.Utils;
using Sandbox.ModAPI.Interfaces.Terminal;

namespace Jeckle.Scripts
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class GunsightCam : MySessionComponentBase

    {
        public static GunsightCam Instance = null;
        public IMyHudNotification Notification;
        public Dictionary<MyCameraBlockDefinition, string> OriginalDefData;
        public string HalfPath = "Textures\\GUI\\Icons\\Screens\\";

        private static Dictionary<long, int> SelectedOverlayIndex = new Dictionary<long, int>();

        private static readonly List<Overlay> OverlayRegistryList = new List<Overlay>();
        public int NextRegistryKey = 0;
        public bool camtoggle = false;

        //For The Registry
        private static readonly List<MyTerminalControlComboBoxItem> _items = new List<MyTerminalControlComboBoxItem>();

        public override void LoadData()
        {
            Instance = this;
            MyAPIGateway.Utilities.MessageEntered += OnMessageEntered;

            HalfPath = ModContext.ModPath + "\\Textures\\GUI\\Icons\\Screens\\";

            

        }

        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
        }

        void OnMessageEntered(string message, ref bool sendToOthers)
        {
            if (message.Equals("/GChelp", StringComparison.OrdinalIgnoreCase))
            {
                MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "/GCnews - Shows the latest news about this mod");
                MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "/GCregistry - Lists all registered overlays");
                MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "/GCtoggle - Toggle messages on/off");
            }
            if (message.Equals("/GCnews", StringComparison.OrdinalIgnoreCase))
            {
                SelectedOverlayIndex.Clear();
                MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "Big Update! Updates to V2! - redesigns of many gunsights to function better with WARECRAFTER's AutomaticSight mod. ");
            }
            if (message.Equals("/GCregistry", StringComparison.OrdinalIgnoreCase))
            {
                // Example: get the definition for GunsightCam
                foreach (var overlay in OverlayRegistryList)
                {
                    MyAPIGateway.Utilities.ShowMessage("GunsightCameras", $"Key: {overlay.RegistryKey}, Name: {overlay.DisplayName}, Path: {overlay.TexturePath}, Zoom: {overlay.MinZoom} - {overlay.MaxZoom}");
                }
            }
            if (message.Equals("/GCtoggle", StringComparison.OrdinalIgnoreCase))
            {
                if (camtoggle == false)
                {
                    camtoggle = true;
                    MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "Camera Toggle On");
                }
                else
                {
                    camtoggle = false;
                    MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "Camera Toggle Off");
                }
            }
            

        }

        static bool Done = false;
        public static void DoOnce(IMyModContext context) // called by GunsightCameraLogic.cs
        {
            if (Done)
                return;
            Done = true;

            // these are all the options and they're not all required so use only what you need.
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Clear Overlay", "Default_Overlay", 0.05f, 0.8f));

            AddDopaminSights();
            CreateControls();
            CreateActions(context);

            MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "Type /GChelp for help.");
        }

        static void AddDopaminSights()
        {

            //Utility
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Simple Sight 1", "Simple_1", 0.1f, 0.1f)); // index 1
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Simple Sight 2", "Simple_2", 0.1f, 0.1f)); // index 2
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "RoR Binoculars", "Binocular_1", 0.4f, 1f)); // index 3 // index 4
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Stadiametric Binoculars", "Binocular_2", 0.4f, 1f)); // index 4
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Commander Binoculars", "Binocular_3", 0.4f, 1f)); // index 12
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 13
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 14
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Sight Line", "Vision_Port_2", 0.4f, 1f)); // index 15
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Aircraft Holographic", "Holographic_1", 0.05f, 0.8f)); // index 5
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Chevron Holographic", "Holographic_2", 0.05f, 0.8f)); // index 6
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Crosshair Holographic", "Holographic_3", 0.05f, 0.8f)); // index 7
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 8
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 9
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 10
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 11
            

            //WW2
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Vickers Mk.IV Sight", "WW2_Allied_1", 0.03f, 0.8f)); // index 16
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Allied No.30 Sight", "WW2_Allied_2", 0.03f, 0.8f)); // index 17
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 18
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 19
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 20
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 21
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 22
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 23
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 24
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 25
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 26
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Selbstfahrlafette-Zielfernrohr Sight", "WW2_Triangle", 0.03f, 0.8f)); // index 27
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Turmzielfernrohr Sight 1", "WW2_Axis_1", 0.03f, 0.8f)); // index 28
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Turmzielfernrohr Sight 2", "WW2_Axis_2", 0.03f, 0.8f)); // index 29

            //ECW
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "ECW Generica Sight", "ECW_Allied_1", 0.0125f, 0.8f)); // index 30
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "ECW Полукруг Sight ", "ECW_Axis_1", 0.0125f, 0.8f)); // index 31
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "ECW Réglage Sight", "Reglage_Sight", 0.1f, 0.1f)); // index 32
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 33
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 34
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 35
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 36
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 37
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 38
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Lotfe ATGM Sight", "Lotfe_ATGM_Sight", 0.1f, 0.1f)); // index 39

            //LCW
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "LCW Raytheon Sight", "LCW_Allied_1", 0.0125f, 0.8f)); // index 40
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "LCW TPD-K1 Sight", "LCW_Axis_1", 0.0125f, 0.8f)); // index 41
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 42
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 43
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 44
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 45
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 46
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 47
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 48
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 49

            //MDRN
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Green Thermal Gunner", "Thermal_1_Green", 0.01f, 0.5f)); // index 50
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "White Thermal Gunner", "Thermal_1_White", 0.01f, 0.5f)); // index 51
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Green Thermal Commander", "Thermal_2_Green", 0.01f, 0.5f)); // index 52
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "White Thermal Commander", "Thermal_2_White", 0.01f, 0.5f)); // index 53
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 54
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 55
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "PLACEHOLDER", "FOR FUTURE SIGHTS", 0.1f, 0.1f)); // index 56
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "DRN Screen Sight", "DRN_Screen_Sight", 0.1f, 0.1f)); // index 57
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "M67 Stadia Sight", "M67_Stadia_Sight", 0.1f, 0.1f)); // index 58
        }
        static void CreateControls()
        {
            const string IdPrefix = "Jeckle_"; // to avoid conflicts with other mods, prefix your control ids with something unique to your mod

            if (MyAPIGateway.TerminalControls != null)
            {
                if (MyAPIGateway.TerminalControls == null)
                    return;


                var comboBox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCombobox, IMyCameraBlock>(IdPrefix + "OverlaySelector");

                comboBox.Title = MyStringId.GetOrCompute("Gunsight Overlay Selector");
                comboBox.SupportsMultipleBlocks = false;

                // Populate options

                comboBox.ComboBoxContent = (list) =>
                {
                    foreach (var overlay in OverlayRegistryList)
                    {
                        if (overlay.DisplayName == "PLACEHOLDER") continue; // skip placeholders in the combo box

                        list.Add(new MyTerminalControlComboBoxItem()
                        {
                            Key = overlay.RegistryKey,
                            Value = MyStringId.GetOrCompute(overlay.DisplayName)
                        });
                    }

                };

                comboBox.Getter = (b) =>
                {
                    int idx;
                    if (SelectedOverlayIndex.TryGetValue(b.EntityId, out idx))
                        return idx; // restores selection
                    return 0; // default
                };

                comboBox.Setter = (block, key) =>
                {
                    SelectedOverlayIndex[block.EntityId] = (int)key;

                    if ((int)key <= OverlayRegistryList.Count && OverlayRegistryList[(int)key] != null)
                    {
                        string fullPath = GunsightCam.Instance.HalfPath + OverlayRegistryList[(int)key].TexturePath + ".dds";

                        GunsightCam.Instance.EditCamera(block.BlockDefinition, fullPath, OverlayRegistryList[(int)key].MinZoom, OverlayRegistryList[(int)key].MaxZoom);


                    }


                };

                MyAPIGateway.TerminalControls.AddControl<IMyCameraBlock>(comboBox);

                var c = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyCameraBlock>(IdPrefix + "Rangefinder");
                c.Title = MyStringId.GetOrCompute("Rangefinder");
                c.Tooltip = MyStringId.GetOrCompute("Gets a range reading to the object in the center of the camera's view.");
                c.SupportsMultipleBlocks = false;

                c.Action = (b) =>
                {
                    var camera = b as IMyCameraBlock;
                    if (camera == null)
                        return;

                    GunsightCamLogic.Rangefind(camera);
                };

                MyAPIGateway.TerminalControls.AddControl<IMyCameraBlock>(c);
            }
        }
        static void CreateActions(IMyModContext context)
        {
            var rangefinderAction = MyAPIGateway.TerminalControls.CreateAction<IMyCameraBlock>("Jeckle_" + "Rangefinder");
            rangefinderAction.Name = new StringBuilder("Rangefinder");
            rangefinderAction.ValidForGroups = false; // If the action is visible for grouped blocks (as long as they all have this action).
            rangefinderAction.Icon = GunsightCam.Instance.HalfPath + "Rangefinder.dds";

            // Called when the toolbar slot is triggered
            // Should not be unassigned.

            rangefinderAction.Action = (b) =>
            {
                var camera = b as IMyCameraBlock;
                if (camera == null)
                    return;

                GunsightCamLogic.Rangefind(camera);
            };

            // The status of the action, shown in toolbar icon text and can also be read by mods or PBs.
            rangefinderAction.Writer = (b, sb) =>
            {
                sb.Append(GunsightCamLogic.publicDistance.ToString("n0") + " m");
            };
            
            MyAPIGateway.TerminalControls.AddAction<IMyCameraBlock>(rangefinderAction);
            // yes, there's only one type of action
            foreach (Overlay overlay in OverlayRegistryList) // create one action per registered overlay
            {
                if (overlay.DisplayName == "PLACEHOLDER") continue; // skip placeholders in the combo box
                const string IdPrefix = "Jeckle_"; // to avoid conflicts with other mods, prefix your control ids with something unique to your mod
                var a = MyAPIGateway.TerminalControls.CreateAction<IMyCameraBlock>(IdPrefix + "OverlaySelector" + overlay.RegistryKey.ToString());
                a.Name = new StringBuilder("Set & View Overlay: " + overlay.DisplayName);
                a.ValidForGroups = false; // If the action is visible for grouped blocks (as long as they all have this action).
                if(overlay.RegistryKey < 15)
                {
                    a.Icon = GunsightCam.Instance.HalfPath + "Utility.dds";
                }
                else if(overlay.RegistryKey < 30)
                {
                    a.Icon = GunsightCam.Instance.HalfPath + "WW2.dds";
                }
                else if(overlay.RegistryKey < 40)
                {
                    a.Icon = GunsightCam.Instance.HalfPath + "ECW.dds";
                }
                else if(overlay.RegistryKey < 50)
                {
                    a.Icon = GunsightCam.Instance.HalfPath + "LCW.dds";
                }
                else
                {
                    a.Icon = GunsightCam.Instance.HalfPath + "MDRN.dds";
                }   

                // Called when the toolbar slot is triggered
                // Should not be unassigned.
                a.Action = (b) =>
                {


                    string fullPath = GunsightCam.Instance.HalfPath + OverlayRegistryList[overlay.RegistryKey].TexturePath + ".dds";

                    GunsightCam.Instance.EditCamera(b.BlockDefinition, fullPath, OverlayRegistryList[overlay.RegistryKey].MinZoom, OverlayRegistryList[overlay.RegistryKey].MaxZoom);

                    if (GunsightCam.Instance.camtoggle == true)
                    {
                        MyAPIGateway.Utilities.ShowMessage("GunsightCameras", $"Selected: {OverlayRegistryList[overlay.RegistryKey].DisplayName}");
                        MyAPIGateway.Utilities.ShowMessage("GunsightCameras", $"Path: {OverlayRegistryList[overlay.RegistryKey].TexturePath}, key: {OverlayRegistryList[overlay.RegistryKey].RegistryKey}");
                    }


                    MyCameraBlock camBlock = b as MyCameraBlock;
                    if (camBlock != null)
                    {
                        camBlock.RequestSetView();
                    }
                };

                // The status of the action, shown in toolbar icon text and can also be read by mods or PBs.
                a.Writer = (b, sb) =>
                {
                    sb.Append(overlay.DisplayName);
                };

                a.InvalidToolbarTypes = new List<MyToolbarType>()
                {
                    MyToolbarType.ButtonPanel,
                };

                MyAPIGateway.TerminalControls.AddAction<IMyCameraBlock>(a);

            }

            
        }
        

        public override void BeforeStart()
        {
            try
            {
                if (!MyAPIGateway.Utilities.IsDedicated)
                {
                    OriginalDefData = new Dictionary<MyCameraBlockDefinition, string>();

                }
            }
            catch (Exception e)
            {

            }

            
        }

        public void EditCamera(MyDefinitionId defId, string NewOverlayTexture, float minZoom, float maxZoom)
        {
            try
            {
                // defensive: avoid passing empty/null subtype (this causes the "(null)" log spam)
                if (defId.Equals(default(MyDefinitionId)))
                    return;

                // many VRage versions expose SubtypeName; guard defensively
                var subtypeName = "";
                try { subtypeName = defId.SubtypeName ?? ""; } catch { subtypeName = ""; }
                if (string.IsNullOrEmpty(subtypeName))
                    return;

                // optionally ensure we're dealing with a camera block id
                // if (defId.TypeId != typeof(MyObjectBuilder_CameraBlock)) return;

                MyCubeBlockDefinition blockDef;
                if (!MyDefinitionManager.Static.TryGetCubeBlockDefinition(defId, out blockDef))
                    return;

                MyCameraBlockDefinition camDef = blockDef as MyCameraBlockDefinition;
                if (camDef != null)
                {
                    if (OriginalDefData == null) OriginalDefData = new Dictionary<MyCameraBlockDefinition, string>();
                    OriginalDefData[camDef] = camDef.OverlayTexture ?? "";

                    camDef.OverlayTexture = NewOverlayTexture;
                    camDef.MinFov = minZoom;
                    camDef.MaxFov = maxZoom;
                }
            }
            catch (Exception)
            {
                // swallow - we deliberately avoid logging here to prevent spam
                return;
            }
        }

        public static void AddOverlay(Overlay overlay)
        {

            OverlayRegistryList.Add(overlay);
            _items.Add(new MyTerminalControlComboBoxItem()
            {
                Key = overlay.RegistryKey,
                Value = MyStringId.GetOrCompute(overlay.DisplayName)

            });
            GunsightCam.Instance.NextRegistryKey++;


        }

        
    }

    public class Overlay
    {
        public int RegistryKey { get; }           // internal index key
        public string DisplayName { get; }  // what shows in the combo box
        public string TexturePath { get; }  // overlay texture file path without extension (e.g., "Default_Overlay")
        public float MinZoom { get; }       // FOV min
        public float MaxZoom { get; }       // FOV max

        public Overlay(int key, string displayName, string texturePath, float minZoom, float maxZoom)
        {
            RegistryKey = key;
            DisplayName = displayName;
            TexturePath = texturePath;
            MinZoom = minZoom;
            MaxZoom = maxZoom;
        }
    }
}