namespace snorri
{
    using UnityEngine;

    public class AbilityPlayerLocomotion : AbilityPlayer
    {
        float yFactor;
        float verticalMovementSpeed;

        float yForceCurrent;
        
        protected override void Setup()
        {
            base.Setup();

            yFactor = Vars.Get<float>("y_factor", 20f);
            verticalMovementSpeed = Vars.Get<float>("vertical_speed", 16f);
        }
        protected override void Launch()
        {
            base.Launch();
        }

        public override bool CanRun()
        {
            return body.IsGrounded;
        }
        public override void Begin() { 
            base.Begin(); 
        }
        public override void End() { 
            base.End(); 
        }
        public override void TickPhysicsAbility() { 
            base.TickPhysicsAbility(); 

            if (camera == null)
                return;

            Map args = new Map();
            args.Set<bool>("is_gravity", false);
            Vec input = GAME.Vars.Get<Vec>("input:direction", new Vec());
            Vec moveDir = Node.Body.GetWorldDirectionFromInput(input, this.camera.Node.Point);
            Vec moveDirNormalized = moveDir.Normalize();

            //float yForce = GetYForce();
            float yForce = -5f;

            float distanceFeet = caster["feet_forward"].Distance;
            float distanceHead = caster["head_forward"].Distance;
            
            Vec velocityDir = new Vec(moveDir); 

            bool isMovingUp = false;

            Cast terrainCast = caster["terrain"];
            if (input.Magnitude() > 0.1f && (terrainCast.Hits > 0 && distanceHead > 1f || distanceFeet < 0.75f && distanceHead > 1f))
            {
                yForce = yFactor;
                velocityDir.x = 0f;
                velocityDir.z = 0f;
            }

            yForceCurrent = Mathf.Lerp(yForceCurrent, yForce, TIME.DeltaPhysics*verticalMovementSpeed);
            
            args.Set<float>("y_force", yForceCurrent);

            Node.Body.MoveTowardsDirection(velocityDir, args);
        }
        public override void TickAbility() { 
            base.TickAbility(); 
        }

        private int GetYForce()
        {
            if (!body.IsGrounded)
                return -5;
            
            Cast groundCast = caster["grounded"];
            if (groundCast == null || groundCast.Hits == 0)
            {
                return -5;
            }

            //float groundedDistance = c.Distance;

            return -1;
        }
    }
}