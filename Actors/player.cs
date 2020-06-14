using Godot;
using System;

public class player : Spatial
{

    private PackedScene _flowerPlatform = (PackedScene)GD.Load("res://Environment/flowerPlatform.tscn");
    private moveWrapper _moveWrapper = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _moveWrapper = GetChild<moveWrapper>(0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        InputMovement();
        InputSpawn();
        InputReset();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    // Handles the movement from player input
    private void InputMovement()
    {
        Vector3 dir = new Vector3();
        if(Input.IsActionJustPressed("ui_right"))
        {
            dir += Vector3.Right;
            _moveWrapper.RotationDegrees = new Vector3(0, 0, 0);
        }
        if(Input.IsActionJustPressed("ui_left"))
        {
            dir += Vector3.Left;
            _moveWrapper.RotationDegrees = new Vector3(0, 180, 0);
        }
        if(Input.IsActionJustPressed("ui_up"))
        {
            dir += Vector3.Forward;
            _moveWrapper.RotationDegrees = new Vector3(0, 90, 0);
        }
        if(Input.IsActionJustPressed("ui_down"))
        {
            dir += Vector3.Back;
            _moveWrapper.RotationDegrees = new Vector3(0, 270, 0);
        }
        if(dir != Vector3.Zero)
            _moveWrapper.Move(dir);
    }

    private Vector3 FacingVec()
    {
        return Vector3.Right.Rotated(Vector3.Up, _moveWrapper.Rotation.y);
    }

    private void InputSpawn()
    {
        RayCast frontRayCast = _moveWrapper.GetChild<RayCast>(1);
        RayCast frontGroundRayCast = _moveWrapper.GetChild<RayCast>(2);
        var frontGround = (Node)frontGroundRayCast.GetCollider();
        string name = NameGetter.FromCollision(frontGround);
        //check the player requested a spawn on an empty tile above ground
        if(frontGround != null)
            if(Input.IsActionJustPressed("ui_select") &&
            !frontRayCast.IsColliding() &&
            (name == "GridMap" || name == "pot"))
            {
                //spawn a flower in front of the player
                Spatial flowerInstance = (Spatial)_flowerPlatform.Instance();
                flowerInstance.Translation = Translation + 
                                             _moveWrapper.Translation +
                                             moveWrapper.SPEED * FacingVec();
                GetParent().AddChild(flowerInstance);
                if(name == "pot")
                    ((moveWrapper) frontGround).planted = true;
            }
    }

    private void InputReset()
    {
        if(Input.IsActionJustPressed("ui_reset"))
            GetTree().ReloadCurrentScene();
    }
}
