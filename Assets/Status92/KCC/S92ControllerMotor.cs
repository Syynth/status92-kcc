using UnityEngine;

namespace Status92.KCC
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class S92ControllerMotor : MonoBehaviour
    {
        public S92Controller2D Controller;

        private Collider2D Collider;
        private Rigidbody2D Rigidbody;

        public Vector2 Velocity;
        public Vector2 Gravity = Vector2.one;
        public Vector2 _surfaceAnchor = new Vector2(0, -0.1f);

        public Rigidbody2D.SlideMovement SlideMovement;

        private void Awake()
        {
            Collider = GetComponent<Collider2D>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CalculateVelocity();
            Move();
        }

        public bool Grounded;
        public bool _skipGrounding;

        private RaycastHit2D[] _raycastHits = new RaycastHit2D[1];
        
        public ContactFilter2D _contactFilter = new();

        public float _jumpTimer = 0f;

        private void CalculateVelocity()
        {
            Controller.UpdateVelocity(ref Velocity);
            
            SlideMovement.surfaceAnchor = _skipGrounding ? Vector2.zero : _surfaceAnchor;
            
            var hitCount = Rigidbody.Cast(Vector2.down, _contactFilter, _raycastHits, 0.1f);
            
            if (hitCount > 0)
            {
                Grounded = true;
            }
            else
            {
                Grounded = false;
            }

            if (!Grounded)
            {
                Velocity += Gravity * Time.deltaTime;                
            }

            if (_jumpTimer > 0f)
            {
                _jumpTimer -= Time.deltaTime;
                if (_jumpTimer <= 0f)
                {
                    _skipGrounding = false;
                    _jumpTimer = -0.1f;
                }
            }
        }

        public void Jump()
        {
            if (_skipGrounding) return;
            _surfaceAnchor = SlideMovement.surfaceAnchor;
            Grounded = false;
            _skipGrounding = true;
            _jumpTimer = 0.1f;
        }

        private void Move()
        {
            // SlideMovement.gravity = Gravity;

            var results = Rigidbody.Slide(Velocity, Time.deltaTime, SlideMovement);

            if (!results.slideHit && !results.surfaceHit)
            {
                return;
            }

            if (Mathf.Abs(results.surfaceHit.normal.y) > 0.5f || Mathf.Abs(results.slideHit.normal.y) > 0.5f)
            {
                // Debug.Log($"Hit a vertical surface! {results.surfaceHit.normal}");
                Velocity.y = 0f;
            }

            if (Mathf.Abs(results.surfaceHit.normal.x) > 0.75f && Vector2.Dot(Velocity, results.surfaceHit.normal) < -0.75)
            {
                Debug.Log($"Hit a horizontal surface! {results.surfaceHit.normal} {results.iterationsUsed} {results.remainingVelocity}");
                Velocity.x = 0f;
            }
            
            Debug.Log($"remaining velocity {results.remainingVelocity}");
            
            if (results.slideHit.rigidbody == null) return;
            
            Debug.Log($"Hit a rigidbody! {results.slideHit.rigidbody.name} {results.slideHit.normal}");
            
            var rb = results.slideHit.rigidbody;
            var point = results.slideHit.point;

            var distanceFromFeet = Vector2.Distance(point,
                transform.position + new Vector3(0, Collider.bounds.size.y / 2f));

            // if (!(distanceFromFeet > 1f))
            {
                // Debug.Log($"Only applying force if distance from feet is greater than 1f {distanceFromFeet}");
                // return;
            }
            
            Debug.Log($"Adding force at {point}, {transform.position} distance from feet {distanceFromFeet}");
            rb.AddForceAtPosition(
                Velocity * 0.1f, 
                point,
                ForceMode2D.Impulse
            );
        }
    }
}