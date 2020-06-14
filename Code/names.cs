using Godot;
using System;

public class NameGetter
{

    public static string FromCollision(Node node)
    {
        string name = node.Name;
        if(name == "moveWrapper")
        {
            name = node.GetParent().Name;
        }
        return name;
    }

}