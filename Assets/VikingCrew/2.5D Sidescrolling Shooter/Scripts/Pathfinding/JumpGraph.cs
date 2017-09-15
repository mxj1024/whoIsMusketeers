using UnityEngine;
using System.Collections.Generic;

using Pathfinding;
using Pathfinding.Serialization;
using System;

/*
----IMPORTANT----
You need to add the following to line 59 in AstarData.cs:
typeof(JumpGraph),

so that the code looks like this:

        public static readonly System.Type[] DefaultGraphTypes = new System.Type[] {
            typeof(GridGraph),
            typeof(PointGraph),
            typeof(NavMeshGraph),
            typeof(JumpGraph),
        };

    Otherwise JumpGraph will not show up in the editor
    */

// Inherit our new graph from a base graph type
[JsonOptIn]
public class JumpGraph : PointGraph {
    [JsonMember]
    public float maxJumpHeight;
    [JsonMember]
    public float maxDropHeight;

    public override bool IsValidConnection(GraphNode a, GraphNode b, out float dist) {
        var dir = (Vector3)(b.position - a.position);
        dist = dir.magnitude;
        if (dir.y > maxJumpHeight)
            return false;
        if (dir.y < -maxDropHeight)
            return false;
        return base.IsValidConnection(a, b, out dist);
    }
}
