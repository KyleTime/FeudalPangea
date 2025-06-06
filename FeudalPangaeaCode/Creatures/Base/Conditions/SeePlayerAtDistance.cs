using Godot;
using System;

namespace CreatureBehaviors.CreatureConditions
{
    public class SeePlayerAtDistance : BehaviorCondition
    {
        private bool limitedDistance;
        private float maxDistance;
        float raiseRay;


        /// <summary>
        /// Condition to detect whether creature can see the player in particular
        /// </summary>
        /// <param name="maxDistance">Maximum distance the creature can see, -1 causes infinite range</param>
        /// <param name="raiseRay">[Optional] How much to raise or lower the ray by on the y axis</param>
        public SeePlayerAtDistance(float maxDistance, float raiseRay = 1)
        {
            if (maxDistance == -1)
            {
                limitedDistance = false;
            }
            else
            {
                limitedDistance = true;
                this.maxDistance = maxDistance;
            }

            this.raiseRay = raiseRay;
        }

        public bool Condition(CreatureStateMachine self)
        {
            //get space! if you're not Kyle and you're reading this...
            //look!: https://docs.godotengine.org/en/stable/tutorials/physics/ray-casting.html 
            PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;

            Vector3 rayOrigin = self.GetCreaturePosition() + new Vector3(0, raiseRay, 0);
            Vector3 rayEnd = Player.player.GetCreaturePosition();

            if (limitedDistance)
            {
                Vector3 dir = (Player.player.GetCreaturePosition() - self.GetCreaturePosition()).Normalized();

                rayEnd = rayOrigin + dir * maxDistance;
            }

            var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
            query.Exclude.Add(self.GetRid());

            var result = space.IntersectRay(query);

            if (result.Count > 0)
            {
                if ((Rid)result["rid"] == Player.player.GetRid())
                {
                    return true;
                }
            }

            return false;
        }

        public ICreature GetTarget()
        {
            return Player.player;
        }
    }
}