using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using CavernWars;

/// <summary>
/// Message sent by a player to update their state to other players.
/// </summary>
public class GameUpdateMessage : MessageBase
{
    public int id; // id of the player
    public Vector2 position;
    public Vector2 movement;
    // 4-bit value, where bits, starting from the most significant represent engines in the following order: top, right, bottom, left
    // For instance 12=1100 means that top and right engines are on.
    public int activeEngines;
    public float hp;
    public bool alive;
    public ProjectileChange[] projectileChanges;

   /* public override void Serialize(NetworkWriter writer)
    {
        writer.Write(id);
        writer.Write(position);
        writer.Write(activeEngines);
        writer.Write(hp);
        writer.Write(alive);
        NetworkWriter childWriter = new NetworkWriter();
        childWriter.StartMessage((short)MessageType.GAME_UPDATE);
        foreach (ProjectileChange proj in projectileChanges)
        {
            proj.Serialize(childWriter);
        }
        writer.Write();
    }*/
}

// TODO: Check if the Serializable tag / inheriting MessageBase are needed.
[Serializable]
public class ProjectileChange : MessageBase
{
    public int id;
    public Vector2 position;
    public Vector2 movement;
    public bool alive;
    public int hitTarget; // Id of the target that was hit, 
}