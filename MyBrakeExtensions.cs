using System;
using System.Collections.Generic;
using System.Reflection;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.GameSystems;
using Sandbox.Game.Multiplayer;
using VRageMath;
using VRage.Sync;
using Havok;

namespace SEAnalogWheels
{
    public static class MyBrakeExtensions
    {
        private static MethodInfo _SafeBody;
        private static MethodInfo _PropagateFriction;

        private static FieldInfo _m_constraint;
        private static FieldInfo _m_breakingConstraint;

        static MyBrakeExtensions()
        {
            _SafeBody = typeof(MyMotorSuspension).GetMethod("get_SafeBody", BindingFlags.NonPublic | BindingFlags.Instance);
            _PropagateFriction = typeof(MyMotorSuspension).GetMethod("PropagateFriction", BindingFlags.NonPublic | BindingFlags.Instance);

            _m_constraint = typeof(MyMotorSuspension).GetField("m_constraint", BindingFlags.NonPublic | BindingFlags.Instance);
            _m_breakingConstraint = typeof(MyMotorSuspension).GetField("m_breakingConstraint", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void Update(this MyGridWheelSystem _this, Vector3 m_angularVelocity, HashSet<MyMotorSuspension> m_wheels, bool m_handbrake)
        {
            if (m_angularVelocity.Y > 0f)
            {
                _this.Brake = true;
            }
            else
            {
                _this.Brake = false;
            }

            foreach (var myMotorSuspension2 in m_wheels)
            {
                if (myMotorSuspension2.IsWorking && myMotorSuspension2.BrakingEnabled)
                {
                    if (m_handbrake)
                    {
                        myMotorSuspension2.UpdateBrake(1f);
                    }
                    else
                    {
                        myMotorSuspension2.UpdateBrake(m_angularVelocity.Y);
                    }
                }
            }
        }

        private static Dictionary<MyMotorSuspension, float> m_brake_override = new Dictionary<MyMotorSuspension, float>();
        private static float BrakeOverride(this MyMotorSuspension _this)
        {
            if (m_brake_override.ContainsKey(_this))
            {
                return m_brake_override[_this];
            }
            else
            {
                return 0f;
            }
        }

        public static void UpdateBrake(this MyMotorSuspension _this, float brake)
        {
            if (_this.SafeBody() == null)
                return;

            if (_this.BrakeOverride() > 0f)
            {
                brake = _this.BrakeOverride();
            }
            _this.PropagateFriction();
            if (brake > 0f)
            {
                var propulsionForce = _this.BlockDefinition.PropulsionForce;
                _this.SafeBody().AngularDamping = _this.CubeGrid.Physics.AngularDamping + propulsionForce * .2f * (float)Math.Pow(brake, 3) + propulsionForce * .8f * (float)Math.Floor(brake);
            }
            else
            {
                _this.SafeBody().AngularDamping = _this.CubeGrid.Physics.AngularDamping;

                if (Sync.IsServer && _this.m_constraint() != null && _this.m_breakingConstraint())
                {
                    _this.m_breakingConstraint().Value = false;
                }
            }
        }


        // Wrappers for private fields and methods
        private static object[] dummyObject = new object[0];
        private static HkRigidBody SafeBody(this MyMotorSuspension _this)
        {
            return (HkRigidBody)_SafeBody.Invoke(_this, dummyObject);
        }

        private static void PropagateFriction(this MyMotorSuspension _this)
        {
            _PropagateFriction.Invoke(_this, dummyObject);
        }

        private static HkConstraint m_constraint(this MyMotorSuspension _this)
        {
            return (HkConstraint)_m_constraint.GetValue(_this);
        }

        private static Sync<bool, SyncDirection.FromServer> m_breakingConstraint(this MyMotorSuspension _this)
        {
            return (Sync<bool, SyncDirection.FromServer>)_m_breakingConstraint.GetValue(_this);
        }
    }
}
