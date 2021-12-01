using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Based on the YouTube tutorial https://www.youtube.com/watch?v=pWZg1oChtnc by Sunny Valley Studio
public class BinarySpacePartition : MonoBehaviour
{
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        // Basically, we're going to split rooms horizontally or vertically each time. Then split that room again
        // up to a certain limit, of course.(not forever)


        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);

        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth)
            {
                //since both horizontal and vertical splits can happen, change what you check first



                if (Random.value < 0.5f) // start horizontal
                {
                    if(room.size.y >= minHeight*2)
                    {
                        SplitHorizontally(roomsQueue, room);

                    }
                    else if(room.size.x >= minWidth * 2)
                    {
                        //split vertically anyways
                        SplitVertically(roomsQueue, room);

                    }
                    else if(room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else // start vertical
                {
                    if (room.size.x >= minWidth * 2)
                    {
          
                        SplitVertically(roomsQueue, room);

                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(roomsQueue, room);

                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        // We need to split the room vertically, so we generate a random splitting point
        // Then, we define the first room to start at the minimum point(room.min)
        // then, its size would be up until xSplit, with the minimum size y(and z)
        Vector3Int firstRoomSize = new Vector3Int(xSplit, room.min.y, room.min.z); // room position will be room.min 

        //The second room would start at the split point, so room.min.x + xSplit
        //and its size would be the difference between the rooms original size and the splitting point
        Vector3Int secondRoomPosition = new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z); // calculate new position for second room
        Vector3Int secondRoomSize = new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z); // calculate its size

        BoundsInt room1 = new BoundsInt(room.min, firstRoomSize);
        BoundsInt room2 = new BoundsInt(secondRoomPosition, secondRoomSize);

        //We enqueue the rooms so they can be further split(they will be ignored if they are too small)
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally( Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        //Same thing as vertical, except we're working on the y axis instead 
        var ySplit = Random.Range(1, room.size.y);
        Vector3Int firstRoomSize = new Vector3Int(room.size.x, ySplit, room.size.z);

        Vector3Int secondRoomPosition = new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z);
        Vector3Int secondRoomSize = new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z);

        BoundsInt room1 = new BoundsInt(room.min, firstRoomSize);
        BoundsInt room2 = new BoundsInt(secondRoomPosition, secondRoomSize);

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
