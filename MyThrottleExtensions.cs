using System;
using System.Reflection;
using System.Collections.Generic;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.EntityComponents;
using VRageMath;

namespace SEAnalogWheels
{
    public static class MyThrottleExtensions
    {
        private static readonly Dictionary<MyMotorSuspension, float> throttles = new Dictionary<MyMotorSuspension, float>();

        private static MethodInfo _Accelerate;
        private static MethodInfo _OnPerFrameUpdatePropertyChanged;

        static MyThrottleExtensions()
        {
            _Accelerate = typeof(MyMotorSuspension).GetMethod("Accelerate", BindingFlags.NonPublic | BindingFlags.Instance);
            _OnPerFrameUpdatePropertyChanged = typeof(MyMotorSuspension).GetMethod("OnPerFrameUpdatePropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        }



        // These two methods are supposed to be combined into a field but we can't have everything, can we?
        private static float m_throttle(this MyMotorSuspension _this)
        {
            float num;

            if (throttles.TryGetValue(_this, out num))
            {
                throttles.Remove(_this);
            }
            return num;
        }

        private static void m_throttle(this MyMotorSuspension _this, float value)
        {
            throttles[_this] = value;
        }
        //


        public static float ComputeRequiredPowerInput(this MyMotorSuspension _this, bool m_wasAccelerating)
        {
            if (_this.TopBlock == null)

            {
                return 0f;
            }
            float num = _this.base_ComputeRequiredPowerInput();
            if (num > 0f)
            {
                float requiredIdlePowerInput = _this.BlockDefinition.RequiredIdlePowerInput;
                float amount = Math.Abs(m_throttle(_this));
                float num2 = MathHelper.Lerp(requiredIdlePowerInput, num, amount);
                float num3 = (num2 - requiredIdlePowerInput) / (m_wasAccelerating ? 15 : -50);
                num = MathHelper.Clamp(_this.ResourceSink.RequiredInputByType(MyResourceDistributorComponent.ElectricityId) + num3, requiredIdlePowerInput, num2);
                _this.OnPerFrameUpdatePropertyChanged();
            }
            return num;
        }

        // Bypassing some ancestry business here
        public static float base_ComputeRequiredPowerInput(this MyMotorSuspension _this)
        {
            if (!_this.Enabled || !_this.IsFunctional) // This is what CheckIsWorking() does under the hood.
            {
                return 0f;
            }
            return _this.ResourceSink.MaxRequiredInputByType(MyResourceDistributorComponent.ElectricityId);
        }

        // Wrapper for private method
        public static void OnPerFrameUpdatePropertyChanged(this MyMotorSuspension _this)
        {
            _OnPerFrameUpdatePropertyChanged.Invoke(_this, null);
        }



        // The method call in MyGridWheelSystem.Update() must be changed to myMotorSuspension2.UpdatePropulsion(m_angularVelocity.Z) to work with the new signature
        public static void UpdatePropulsion(this MyMotorSuspension _this, float throttle)
        {
            float propulsionOverride = _this.PropulsionOverride;
            if (propulsionOverride != 0f)
            {
                bool flag = propulsionOverride > 0f;
                if (_this.InvertPropulsion)
                {
                    flag = !flag;
                }

                _this.m_throttle(propulsionOverride);
                _this.Accelerate(Math.Abs(propulsionOverride) * _this.BlockDefinition.PropulsionForce, flag);
                return;
            }
            if (throttle != 0)
            {
                bool flag = throttle < 0f;
                if (_this.InvertPropulsion)
                {
                    flag = !flag;
                }

                _this.m_throttle(_this.Power * throttle);
                _this.Accelerate(Math.Abs(throttle) * _this.BlockDefinition.PropulsionForce * _this.Power, flag);
                return;
            }
        }

        // Wrapper for private method
        public static void Accelerate(this MyMotorSuspension _this, float force, bool forward)
        {
            _Accelerate.Invoke(_this, new object[] { force, forward });
        }
    }
}
