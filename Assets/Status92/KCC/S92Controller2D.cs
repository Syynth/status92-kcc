using System;
using UnityEngine;

namespace Status92.KCC
{
    [RequireComponent(typeof(S92ControllerMotor))]
    public class S92Controller2D : MonoBehaviour
    {
        public S92ControllerMotor Motor;

        public float XSpeed = 5;
        public float LaunchSpeed = 5f;
        public Vector2 ShortHopForce = Vector2.down;

        public float MaxSpeed = 10;

        public float JumpStrength = 15;
        
        public AnimationCurve RunSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public float RunSpeedTime;
        public float TurnAroundTime;
        public int LastRunSpeedSign;

        private void Awake()
        {
            Motor = GetComponent<S92ControllerMotor>();
        }

        public void UpdateVelocity(ref Vector2 velocity)
        {
            var inputX = Input.GetAxis("Horizontal");
            var inputScaled = Time.deltaTime * inputX * XSpeed;
            velocity.x += inputScaled;
            var inputSignX = Math.Sign(inputX);
            var velocitySignX = Math.Sign(velocity.x);

            if (inputSignX == LastRunSpeedSign)
            {
                RunSpeedTime += Time.deltaTime;
                TurnAroundTime = 0;
            }
            else
            {
                RunSpeedTime = 0;
            }
            LastRunSpeedSign = inputSignX;

            if (velocitySignX != inputSignX)
            {
                TurnAroundTime += Time.deltaTime;
            }
            else
            {
                TurnAroundTime = 0;
            }

            if (TurnAroundTime > 0.1f)
            {
                velocity.x *= -1f;
            }

            if (RunSpeedTime is < 0.25f and > 0f)
            {
                velocity.x += LaunchSpeed * Time.deltaTime;
            }

            var absVelocityX = Mathf.Abs(velocity.x);
            var absInputX = Mathf.Abs(inputX);

            if (absVelocityX > MaxSpeed)
            {
                velocity.x = Mathf.Sign(velocity.x) * MaxSpeed;
            }

            if (Math.Abs(inputSignX - velocitySignX) > 0.1f)
            {
                velocity.x += inputScaled * 5f;
            }

            if (absInputX < 0.01f || !Mathf.Approximately(inputSignX, velocitySignX))
            {
                velocity.x *= Mathf.Clamp01(1 - Time.deltaTime * 7);
            }

            if (Input.GetButtonDown("Jump") && Motor.Grounded)
            {
                velocity.y = JumpStrength;
                Motor.Jump();
            }

            if (!Input.GetButton("Jump") && !Motor.Grounded && velocity.y > 0f)
            {
                velocity += ShortHopForce * Time.deltaTime;
            }
        }
    }
}