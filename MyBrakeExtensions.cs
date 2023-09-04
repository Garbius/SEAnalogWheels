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
        private static MethodInfo _Constraint;
        private static MethodInfo _PropagateFriction;

        private static FieldInfo _m_breakingConstraint;

        static MyBrakeExtensions()
        {
            _SafeBody = typeof(MyMotorSuspension).GetMethod("get_SafeBody", BindingFlags.NonPublic | BindingFlags.Instance);
            _Constraint = typeof(MyMotorSuspension).GetMethod("get_Constraint", BindingFlags.NonPublic | BindingFlags.Instance);
            _PropagateFriction = typeof(MyMotorSuspension).GetMethod("PropagateFriction", BindingFlags.NonPublic | BindingFlags.Instance);

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

        public static void UpdateBrake(this MyMotorSuspension _this, float brake)
        {
            if (_this.SafeBody() == null)
                return;

            _this.PropagateFriction();
            if (brake > 0f)
            {
                var propulsionForce = _this.BlockDefinition.PropulsionForce;
                _this.SafeBody().AngularDamping = _this.CubeGrid.Physics.AngularDamping + propulsionForce * .2f * (float)Math.Pow(brake, 3) + propulsionForce * .8f * (float)Math.Floor(brake);
            }
            else
            {
                _this.SafeBody().AngularDamping = _this.CubeGrid.Physics.AngularDamping;

                if (Sync.IsServer && _this.Constraint() != null && _this.m_breakingConstraint())
                {
                    _this.m_breakingConstraint().Value = false;
                }
            }
        }

        private static HkRigidBody SafeBody(this MyMotorSuspension _this)
        {
            return (HkRigidBody)_SafeBody.Invoke(_this, null);
        }

        private static HkConstraint Constraint(this MyMotorSuspension _this)
        {
            return (HkConstraint)_Constraint.Invoke(_this, null);
        }

        private static void PropagateFriction(this MyMotorSuspension _this)
        {
            _PropagateFriction.Invoke(_this, null);
        }

        private static Sync<bool, SyncDirection.FromServer> m_breakingConstraint(this MyMotorSuspension _this)
        {
            return (Sync<bool, SyncDirection.FromServer>)_m_breakingConstraint.GetValue(_this);
        }
    }
}
