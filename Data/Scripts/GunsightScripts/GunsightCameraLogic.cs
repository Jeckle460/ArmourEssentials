using System;
using System.Collections.Generic;
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

namespace Jeckle.Scripts
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CameraBlock), false)]
    public class GunsightCamLogic : MyGameLogicComponent
    {
        // retry counter is handled locally during rangefind, no need for static field

        private static int rangeDelayTicks = 360; // default 6 seconds
        private static int tick = 0;
        private static int preventUntilTick = 0;
        private static bool pendingOutput = false;
        private static bool pendingRelation = false;
        private static double storedDistance = 0.0;
        public static double publicDistance = 0.0;
        private static MyRelationsBetweenPlayerAndBlock storedRelation = MyRelationsBetweenPlayerAndBlock.NoOwnership;
        public IMyHudNotification Notification;


        public IMyCameraBlock Camera;

        public override void UpdateBeforeSimulation()
        {
            //IMyCameraBlock cameraBlock = (MyAPIGateway.Session?.CameraController as IMyCameraBlock);

            // Get the camera block definition
            //var def = MyDefinitionManager.Static.GetCubeBlockDefinition(new MyDefinitionId(typeof(MyObjectBuilder_CameraBlock))) as MyCameraBlockDefinition;
            tick = MyAPIGateway.Session.GameplayFrameCounter;
            if (pendingOutput)
            {
                //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"preventUntilTick: {preventUntilTick}");
                //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"tick: {tick}");
                //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"ticks remaining: {preventUntilTick - tick}");
                if (pendingRelation && (storedRelation == MyRelationsBetweenPlayerAndBlock.Friends || storedRelation == MyRelationsBetweenPlayerAndBlock.Owner))
                {
                    MyAPIGateway.Utilities.ShowNotification("Allies at Coordinates!", 2500, MyFontEnum.Green);
                    pendingRelation = false;
                    Camera.EnableRaycast = false;
                }

                if (preventUntilTick <= tick)
                {
                    
                    MyAPIGateway.Utilities.ShowNotification($"Range: {storedDistance:n0} m", 2500, MyFontEnum.Green);
                    publicDistance = storedDistance;
                    pendingOutput = false;
                    Camera.EnableRaycast = false;
                    
                }
            }
            
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME | MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            GunsightCam.DoOnce(ModContext);

            Camera = (IMyCameraBlock)Entity;
            if (Camera.CubeGrid?.Physics == null)
                return; // ignore ghost/projected grids

        }

        public static void Rangefind(IMyCameraBlock camera)
        {
            //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"camera: {camera?.CustomName}");
            //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"ticks: {rangeDelayTicks}");
            //MyAPIGateway.Utilities.ShowMessage("GC DEBUG", $"pending: {pendingOutput}");

            if (pendingOutput)
            {
                MyAPIGateway.Utilities.ShowNotification("Rangefinding...", 2500, MyFontEnum.LoadingScreen);
                return;
            }

            float ClassMultiplier = 1f;
            float _maxRange = 200f;

            var subtypeId = camera.BlockDefinition.SubtypeId.ToString();
            int Class0Multiplier = 200; // Manual Rangefinding
            int Class1Multiplier = 1000; // Optical Rangefinding
            int Class2Multiplier = 2000; // Laser Rangefinding
            int Class3Multiplier = 5000; // Advanced Laser Rangefinding

            if (subtypeId.Contains("Class0_"))
            {
                ClassMultiplier = Class0Multiplier;
                rangeDelayTicks = 360; // 6 seconds
            }
            else if (subtypeId.Contains("Class1_"))
            {
                ClassMultiplier = Class1Multiplier;
                rangeDelayTicks = 180; // 3 seconds
            }
            else if (subtypeId.Contains("Class2_"))
            {
                ClassMultiplier = Class2Multiplier;
                rangeDelayTicks = 60; // 1 second
            }
            else if (subtypeId.Contains("Class3_"))
            {
                ClassMultiplier = Class3Multiplier;
                rangeDelayTicks = 0; // 0 seconds
            }
            else
            {
                ClassMultiplier = Class0Multiplier;
                rangeDelayTicks = 360; // 6 seconds (DEFAULT)
            }
            _maxRange = ClassMultiplier;

            if (!camera.EnableRaycast)
                camera.EnableRaycast = true;

            var detectedInfo = camera.Raycast(_maxRange);
            // immediately turn off raycast to conserve resources; we only need it for one shot

            preventUntilTick = MyAPIGateway.Session.GameplayFrameCounter + rangeDelayTicks;

            if (detectedInfo.IsEmpty())
            {
                // try a few more times in case of a transient miss
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    if (!camera.EnableRaycast)
                        camera.EnableRaycast = true;

                    detectedInfo = camera.Raycast(_maxRange);
                    if (!detectedInfo.IsEmpty())
                    {
                        break; // got a hit, exit retry loop
                    }

                    
                }

                if (detectedInfo.IsEmpty())
                {
                    // still empty after retries, give up
                    MyAPIGateway.Utilities.ShowNotification("Rangefinding failed. No target detected.", 2500, MyFontEnum.Red);
                    return;
                }
            }
            
            pendingRelation = true;

            var distance = Vector3D.Distance((Vector3D)detectedInfo.HitPosition, camera.GetPosition());
            if (distance <= _maxRange)
            {
                MyAPIGateway.Utilities.ShowNotification("Rangefinding...", 1350, MyFontEnum.LoadingScreen);
                storedRelation = detectedInfo.Relationship;
                storedDistance = distance;
                pendingOutput = true;

                return;
            }
            
        }
    }

}