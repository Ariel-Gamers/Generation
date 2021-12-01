using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

/**
 * This class demonstrates the CaveGenerator on a Tilemap.
 * 
 * By: Erel Segal-Halevi
 * Since: 2020-12
 */

// Based on the YouTube tutorial https://www.youtube.com/watch?v=pWZg1oChtnc by Sunny Valley Studio
// code adapted to Erel's

public class TilemapCaveGenerator: BinarySpacePartition {
    [SerializeField] Tilemap tilemap = null;

    [Tooltip("The tile that represents a wall (an impassable block)")]
    [SerializeField] TileBase wallTile = null;

    [Tooltip("The tile that represents a floor (a passable block)")]
    [SerializeField] TileBase floorTile = null;

    [Tooltip("The percent of walls in the initial random map")]
    [Range(0, 1)]
    [SerializeField] float randomFillPercent = 0.5f;

    [Tooltip("Length and height of the grid")]
    [SerializeField] int gridSize = 100;

    [Tooltip("How many steps do we want to simulate?")]
    [SerializeField] int simulationSteps = 20;

    [Tooltip("For how long will we pause between each simulation step so we can look at the result?")]
    [SerializeField] float pauseTime = 1f;

    [SerializeField] GameObject colliderObject;

    [SerializeField] public static BoundsInt SpaceToSplit = new BoundsInt(0, 0, 0, 100, 30, 0);

    private CaveGenerator caveGenerator;

    void Start()  {

        colliderObject = GameObject.Find("Collider");

        //To get the same random numbers each time we run the script
        Random.InitState(100);

        caveGenerator = new CaveGenerator(randomFillPercent, gridSize);
        caveGenerator.RandomizeMap();
                
        //For testing that init is working
        GenerateAndDisplayTexture(caveGenerator.GetMap());
            
        //Start the simulation
        StartCoroutine(SimulateCavePattern());

        
    }


    //Do the simulation in a coroutine so we can pause and see what's going on
    private IEnumerator SimulateCavePattern()  {
        for (int i = 0; i < simulationSteps; i++)   {
            yield return new WaitForSeconds(pauseTime);

            //Calculate the new values
            caveGenerator.SmoothMap();

            //Generate texture and display it on the plane
            GenerateAndDisplayTexture(caveGenerator.GetMap());
        }
        Debug.Log("Simulation completed!");
    }



    //Generate a black or white texture depending on if the pixel is cave or wall
    //Display the texture on a plane
    //private void GenerateAndDisplayTexture(int[,] data) {
    //    for (int y = 0; y < gridSize; y++) {
    //        for (int x = 0; x < gridSize; x++) {
    //            var position = new Vector3Int(x, y, 0);
    //            var tile = data[x, y] == 1 ? wallTile: floorTile;
    //            tilemap.SetTile(position, tile);
    //        }
    //    }
    //}


    /**
     * IGNORE THIS PLEASE
     */

    public static List<BoundsInt> BinarySpacePartitioning()
    {
        //Basically, we are going to split room horizontally and vertically, add them to the queue and
        // continue doing so to them up to a certain limit

        int minH = 5;
        int minW = 5;
        int maxW = 20;
        int maxH = 20;

        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> rooms = new List<BoundsInt>();

        roomsQueue.Enqueue(SpaceToSplit);

        bool change_check = false;
        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minH && room.size.x >= minW)
            {
                //since both horizontal and vertical splits can happen, change what you check first
 
                if (change_check)
                {
                    if(room.size.y >= 2 * minH)
                    {
                        SplitHorizontal(roomsQueue, room);
                    }
                    else if (room.size.x >= 2 * minW)
                    {
                        SplitVertical(roomsQueue, room);
                    }
                    else if (room.size.x >= minW && room.size.y >= minH)
                    {
                        rooms.Add(room); // add it to my existing rooms
                    }
                }
                else
                {

                    if (room.size.x >= 2 * minW)
                    {
                        SplitVertical(roomsQueue, room);
                    }
                    else if (room.size.y >= 2 * minH)
                    {
                        SplitHorizontal(roomsQueue, room);
                    }
                    else if (room.size.x >= minW && room.size.y >= minH)
                    {
                        rooms.Add(room); // add it to my existing rooms
                    }
                }
            }
            change_check = !change_check; // flip
        }
        return rooms;
    }

    private static void SplitVertical(Queue<BoundsInt> roomsQueue, BoundsInt room)
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

    private static void SplitHorizontal(Queue<BoundsInt> roomsQueue, BoundsInt room)
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

    private void GenerateAndDisplayTexture(int[,] data)
    {

        List<BoundsInt> rooms = BinarySpacePartition.BinarySpacePartitioning(SpaceToSplit, 5, 5);

        bool random = false;
        foreach(BoundsInt room in rooms)
        {
            for(int x = room.xMin; x < room.xMax;  x++)
            {
                for(int y = room.yMin; y < room.yMax; y++ )
                {
                    //simple fill method to demonstrate rooms, we fill with two different tyles each room randomly
                    //of course, anything can be done to a room

                    var position = new Vector3Int(x, y, 0);
                    if(random)
                        tilemap.SetTile(position, wallTile);
                    else
                        tilemap.SetTile(position, floorTile);
                }
            }
            random = !random;
        }
    }
}
