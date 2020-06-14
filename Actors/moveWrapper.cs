using Godot;
using System;

public class moveWrapper : KinematicBody
{
    public const float SPEED = 2;
    //whether a block is planted above and should move with it
    public bool planted = false;
    private bool _collisionLoaded = false;
    private RayCast plantRaycast = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        plantRaycast = GetChild<RayCast>(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if(_collisionLoaded)
        {
            Fall();
        }
        else if(MoveAndCollide(new Vector3(0, -1 * SPEED, 0), testOnly: true) != null)
        {
            _collisionLoaded = true;
        }
    }

    
    //provides grid based movement and handles collision information
    public KinematicCollision Move(Vector3 relVec)
    {
        if(!_collisionLoaded)
            return null;
        relVec *= SPEED;
        KinematicCollision collision = MoveAndCollide(relVec, testOnly: true);
        //don't move if it would cause a collision and return that collision
        if(collision != null)
        {
            //handle the collision
            Node collider = (Node)collision.Collider;
            string name = NameGetter.FromCollision(collider);
            switch(name)
            {
                case "bambooPlatformStatic":
                    Spatial staticBody = (StaticBody)collider;
                    if(staticBody.Translation.y != 0)
                    {
                        staticBody.Translation = Vector3.Zero;
                        relVec += new Vector3(0, SPEED, 0);
                        collision = null;
                    }
                    break;
                case "pot":
                    moveWrapper colliderMove = (moveWrapper)collider;
                    if(colliderMove.Move(relVec/SPEED) == null)
                        collision = null;
                    break;
                case "GridMap":
                case "flowerPlatform":
                    break;
                default:
                    GD.Print("Unhandled collision against " + name);
                    break;
            }
        }
        //move if it wouldn't cause a collision
        if(collision == null)
        {
            Translation += relVec;
            if(planted)
            {
                var plant = (Spatial)plantRaycast.GetCollider();
                if(plant != null)
                    plant.Translation += relVec;
                else
                    planted = false;
            }
        }
        return collision;
    }

    private void Fall()
    {
        KinematicCollision collision = null;
        for(uint i = 0; i < 10; i++)
        {
            collision = Move(Vector3.Down);
            if(collision != null)
                break;
        }
    }

}
