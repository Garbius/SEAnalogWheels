using System.Collections.Generic;
using Sandbox;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using VRageMath;
using VRage.Utils;
using VRage.Plugins;
using HarmonyLib;

namespace SEAnalogWheels
{
    public class SEAnalogWheels : IPlugin
    {
        public void Init(object gameinstance = null)
        {
            var harmony = new Harmony("SEAnalogWheels");
            harmony.PatchAll();

            MyLogExtensions.Error(MySandboxGame.Log, "SEAnalogWheels initialized successfully");
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }
    }

    [HarmonyPatch(typeof(MyGridWheelSystem), "Update")]
    public class UpdateGridWheelSystemPatch
    {
        public static void Postfix(MyGridWheelSystem __instance, bool ___m_handbrake, Vector3 ___m_angularVelocity, HashSet<MyMotorSuspension> ___m_wheels)
        {
            // This is for the braking
            __instance.Update(___m_angularVelocity, ___m_wheels, ___m_handbrake);

            // This is for the throttle
            foreach (var myMotorSuspension2 in ___m_wheels)
            {
                if (myMotorSuspension2.IsWorking && myMotorSuspension2.Propulsion)
                {
                    myMotorSuspension2.UpdatePropulsion(___m_angularVelocity.Z);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Sandbox.Game.Entities.MyShipController), "TryEnableBrakes")]
    public static class TryEnableBrakesPatch
    {
        // Remove default spacebar behavior (or whatever you've bound it to)
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    // This doesn't work anyway. Short-circuited by GetWheelAndLinearVelocity()
    [HarmonyPatch(typeof(MyMotorSuspension), "ArtificialBreakingLogic")]
    public static class ArtificialBreakingLogicPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }


    [HarmonyPatch(typeof(MyMotorSuspension), "UpdatePropulsion")]
    public class UpdatePropulsionPatch
    {
        public static bool Prefix(bool forward, bool backwards)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(MyMotorSuspension), "ComputeRequiredPowerInput")]
    public class ComputeRequiredPowerInputPatch
    {
        public static bool Prefix(MyMotorSuspension __instance, ref float __result, bool ___m_wasAccelerating)
        {
            __result = __instance.ComputeRequiredPowerInput(___m_wasAccelerating);
            return false;
        }
    }
}
