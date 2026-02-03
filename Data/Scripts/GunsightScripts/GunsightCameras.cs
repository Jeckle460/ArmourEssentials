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
                MyAPIGateway.Utilities.ShowMessage("Big Update! Rangefinders added!", "Use the Rangefinder action or terminal button to get a range reading to the object in the center of the camera's view.");
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
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Default Overlay", "Default_Overlay", 0.05f, 0.8f));

            AddDopaminSights();
            CreateControls();
            CreateActions(context);

            MyAPIGateway.Utilities.ShowMessage("GunsightCameras", "Type /GChelp for help.");
        }

        static void AddDopaminSights()
        {
            // these are all the options and they're not all required so use only what you need.
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Binoculars", "Binocular", 0.65f, 0.7f));

            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Vision Port 1", "Vision_Port_1", 0.05f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Vision Port 2", "Vision_Port_2", 0.05f, 0.8f));
            //WW2 = World War 2
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Allied 1", "WW2_Allied_1", 0.03f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Allied 2", "WW2_Allied_2", 0.03f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Axis 1", "WW2_Axis_1", 0.03f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "WW2 Axis 2", "WW2_Axis_2", 0.03f, 0.8f));
            //ECW = Early Cold War
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "ECW Allied 1", "ECW_Allied_1", 0.025f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "ECW Axis 1", "ECW_Axis_1", 0.025f, 0.8f));
            //LCW = Late Cold War
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "LCW Allied 1", "LCW_Allied_1", 0.0125f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "LCW Axis 1", "LCW_Axis_1", 0.0125f, 0.8f));
            //Holo = Holographic
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Holographic 1", "Holographic_1", 0.05f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Holographic 2", "Holographic_2", 0.05f, 0.8f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Holographic 3", "Holographic_3", 0.05f, 0.8f));
            //Thermal = Thermal Vision
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Thermal Sight 1", "Thermal_Sight_1", 0.01f, 0.5f));
            AddOverlay(new Overlay(GunsightCam.Instance.NextRegistryKey, "Thermal Sight 2", "Thermal_Sight_2", 0.01f, 0.5f));

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
                const string IdPrefix = "Jeckle_"; // to avoid conflicts with other mods, prefix your control ids with something unique to your mod
                var a = MyAPIGateway.TerminalControls.CreateAction<IMyCameraBlock>(IdPrefix + "OverlaySelector" + overlay.RegistryKey.ToString());
                a.Name = new StringBuilder("Set & View Overlay: " + overlay.DisplayName);
                a.ValidForGroups = false; // If the action is visible for grouped blocks (as long as they all have this action).
                a.Icon = GunsightCam.Instance.HalfPath + "GunsightCam.dds";

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