# Unity week 5: Two-dimensional scene-building and path-finding


https://ariel-gamers.itch.io/generation

generation changed to be simulated with `BinarySpaceParition.cs`

In Erel's code, I merely changed the tile to draw the rooms and inherited from `BinarySpaceParition.cs`


`TilemapCaveGenerator.cs`:

```csharp
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
 ```
 
 The method `BinarySpacePartionining` splits the rooms vertically or horizontally at random, up to a certain size limit, example:
 
 ```csharp
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
 ```
 
 
