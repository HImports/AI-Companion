﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Companion : Entity
{
    public List<Vector3> candidatePositions;
    public Player player;

    public float minimumDistance = 30;
    public float maximumDistance = 200;

    //Variables to find positions in a semi circle around the player.
    public int playerRadiusSegments;
    public float playerRadiusX;
    public float playerRadiusY;

    void Start()
    {
        EntityType type = EntityType.Companion;
        AddType(type);

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void GetCandidatePositions()
    {
        //Get positions in a semi circle behind the player
        Vector3[] positionsToCheck = FindPlayerRadiusPositions();

        //Clear the list
        candidatePositions.Clear();

        //Go through positions generated from the semi circle
        for (int i = 0; i < positionsToCheck.Length; i++)
        {
            //If there is a clear raycast to it i.e. No objects in the way to it and the player
            if (!Physics.Linecast(player.transform.position, positionsToCheck[i]))
            {
                //Get the forward direction from the player
                Vector3 forwardDirection = player.transform.forward.normalized;
                //Get a position in 'front' of the potential position
                Vector3 forwardPosition = positionsToCheck[i] + (forwardDirection * 2.0f);

                //If there's isn't anything directly in front of this position
                if (!Physics.Linecast(positionsToCheck[i], forwardPosition))
                {
                    //If theres a clear line to a forward position to the player. So the companion doesn't place obstacle between itself and the player. Feels more natural.
                    if (!Physics.Linecast(player.transform.position, forwardPosition))
                    {
                        //Add it to our potential positions to move to.
                        candidatePositions.Add(positionsToCheck[i]);
                    }
                }
            }
        }
    }

    Vector3[] FindPlayerRadiusPositions()
    {
        //Create an empty array for possible positions I can move to.
        Vector3[] radiusPositions = new Vector3[playerRadiusSegments + 1];

        //Get player position and rotation.
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        float z = player.transform.position.z;
        float angle = player.transform.localRotation.eulerAngles.y;

        //Draw a semi circle behind the player
        for (int i = 0; i < (playerRadiusSegments + 1); i++)
        {
            x -= (Mathf.Sin(Mathf.Deg2Rad * angle) * playerRadiusX);
            z -= (Mathf.Cos(Mathf.Deg2Rad * angle) * playerRadiusY);

            //Store each position in our array
            radiusPositions[i] = new Vector3(x, y, z);

            //180 for a semi circle.
            angle += (180f / playerRadiusSegments);
        }

        //Calculate the distance between the player(the start of the semicircle), and the final point of the semi circle and then divide by 2 to get the players distance to the centre of the semi circle. 
        float centreOffset = Vector3.Distance(radiusPositions[playerRadiusSegments], player.transform.position) / 2;

        //Go through all the positions
        for (int i = 0; i < (playerRadiusSegments + 1); i++)
        {
            //Move them to put the player in the centre of the semi circle.
            radiusPositions[i].x += player.transform.right.x * centreOffset;
            radiusPositions[i].z += player.transform.right.z * centreOffset;
        }

        return radiusPositions;
    }

    new void OnDrawGizmos()
    {
        if (candidatePositions.Count > 0)
        {
            for (int i = 0; i < candidatePositions.Count; i++)
            {
                Gizmos.color = Color.white;
                Vector3 forwardDirection = player.transform.forward.normalized;
                Vector3 forwardPosition = candidatePositions[i] + (forwardDirection * 2.0f);

                Gizmos.DrawWireCube(candidatePositions[i], Vector3.one);
                Gizmos.DrawLine(candidatePositions[i], player.transform.position);

                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(forwardPosition, Vector3.one);
                Gizmos.DrawLine(forwardPosition, player.transform.position);
            }
        }
    }

}
