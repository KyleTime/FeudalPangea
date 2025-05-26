using Godot;
using System;

namespace CreatureBehaviors.CreatureConditions
{
    public class SeePlayerAtDistance : BehaviorCondition
    {

        public bool Condition(CreatureStateMachine self)
        {
            //get space! if you're not Kyle and you're reading this...
            //look!: https://docs.godotengine.org/en/stable/tutorials/physics/ray-casting.html 
            PhysicsDirectSpaceState3D space = self.GetWorld3D().DirectSpaceState;

            var query = PhysicsRayQueryParameters3D.Create(self.GetPosition(), Player.player.GetPosition());
            query.Exclude.Add(self.GetRid());

            var result = space.IntersectRay(query);

            if (result.Count > 0)
            {
                GD.Print("Collided RID: " + result["rid"] + " Player RID: " + Player.player.GetRid());
            }

            return false;
        }

        public ICreature GetTarget()
        {
            return Player.player;
        }
    }
}