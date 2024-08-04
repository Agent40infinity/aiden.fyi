using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout : MonoBehaviour
{
    [Header("Room Attributes")]
    public int roomMaxWidth;
    public int roomMaxHeight;
    public Vector2Int roomRange;
    public int maxRoomNumber;
    public Vector2Int roomDimensions;

    [Header("Generation")]
    public float generationChance;
    public int generateOffset;
    bool[] generatedUniques = new bool[2];

    private int numberOfRooms;
    private Vector2Int startRoom;

    [Header("Layout")]
    public List<Vector2Int> generateRoomCoords = new List<Vector2Int>();
    public Room[,] roomLayout;

    [Header("Variants")]
    public List<int> variantsLeft = new List<int>();
    public int variantCount;
    public Dictionary<int, List<bool>> forestVariants = new Dictionary<int, List<bool>>
    {
        { -1, new List<bool> { false, false, false, true } },
        { 2, new List<bool> { true, true, true, true } },
        { 4, new List<bool> { true, true, true, true } },
    };

    [Header("Difficulty")]
    public Dictionary<int, List<int>> difficultiesLeft = new Dictionary<int, List<int>>
    {
        { 1, new List<int> { 1, 2, 3, 4} },
        { 2, new List<int> { 1, 2, 3, 4} },
        { 3, new List<int> { 1, 2, 3, 4} },
        { 4, new List<int> { 1, 2, 3, 4} },
        { 5, new List<int> { 1, 2, 3, 4} },
    };

    public Dictionary<string, int> directions = new Dictionary<string, int>
    {
        { "Left", -1 },
        { "Right", 1 },
        { "Up", 1 },
        { "Down", -1 },
    };

    [Header("Reference")]
    public Transform floorParent;

    [Header("File Paths")]
    public string varietyPath = "Prefabs/Generation/Normal/";
    public string difficultyPath = "Prefabs/Generation/Normal/Content/";
    public string fillPath = "Prefabs/Generation/Fill/";

    public int FloorSize(Vector2Int maxRooms)
    {
        return Random.Range(maxRooms.x, maxRooms.y);
    }

    public void VariantSetUp()
    {
        for (int i = 0; i < variantCount; i++)
        {
            variantsLeft.Add(i + 1);
        }
    }

    public void GenerationCall()
    {
        maxRoomNumber = FloorSize(roomRange);
        GenerationStart();
    }

    public void GenerationStart()
    {
        numberOfRooms = maxRoomNumber;

        startRoom = new Vector2Int(roomMaxWidth / 2, roomMaxWidth / 2);

        roomLayout = new Room[roomMaxWidth, roomMaxHeight];
        CreateRoom(startRoom, "S");

        VariantSetUp();

        GenerateRoomLayout();

        switch (GeneratedCorrectly())
        {
            case false:
                Debug.Log("Generation failed, re-starting!" + System.Environment.NewLine + "Number of Rooms Left: " + numberOfRooms);
                generateRoomCoords.Clear();
                variantsLeft.Clear();
                GenerationStart();
                break;
            case true:
                GameManager.currentRoom = startRoom;
                BuildRoomLayout(); //Move to GameManager or Level script later
                break;
        }
    }

    public void CreateRoom(Vector2Int selectedRoom, string type, int difficulty, int variant)
    {
        roomLayout[selectedRoom.x, selectedRoom.y] = new Room(type, difficulty, variant);
    }

    public void CreateRoom(Vector2Int selectedRoom, string type)
    {
        roomLayout[selectedRoom.x, selectedRoom.y] = new Room(type);
    }

    public void GenerateRoomLayout()
    {
        generateRoomCoords.Add(new Vector2Int(startRoom.x, startRoom.y));

        for (int i = 0; i < generateRoomCoords.Count; i++)
        {
            GenerateAjacentRooms(generateRoomCoords[i]);
        }

        GenerateUniqueStart();

        MatrixDebug.CheckMatrix(roomLayout);
    }

    public void GenerateAjacentRooms(Vector2Int selectedRoom)
    {
        if (numberOfRooms > 0)
        {
            List<Vector2Int> sidesToAdd = AddSides(selectedRoom);

            for (int i = 0; i < sidesToAdd.Count; i++)
            {
                if (CheckChance(generationChance) && numberOfRooms > 0)
                {
                    Vector2Int newRoom = new Vector2Int(sidesToAdd[i].x, sidesToAdd[i].y);
                    CreateRoom(newRoom, "R", GenerateRoomDifficulty(), GenerateRoomVariant());
                    generateRoomCoords.Add(new Vector2Int(sidesToAdd[i].x, sidesToAdd[i].y));
                    numberOfRooms--;
                }
            }
        }
        else
        {
            return;
        }
    }

    public List<Vector2Int> AddSides(Vector2Int selectedRoom)
    {
        List<Vector2Int> temp = new List<Vector2Int>();
        List<Vector2Int> avaliableSides = CheckSides(selectedRoom);

        for (int i = 0; i < avaliableSides.Count; i++)
        {
            if (CheckNeighbour(avaliableSides[i], 1))
            {
                temp.Add(avaliableSides[i]);
            }
        }

        return temp;
    }

    public List<Vector2Int> CheckSides(Vector2Int selectedRoom)
    {
        List<Vector2Int> temp = new List<Vector2Int>();

        if (roomLayout[selectedRoom.x + directions["Left"], selectedRoom.y] == null)
        {
            temp.Add(new Vector2Int(selectedRoom.x + directions["Left"], selectedRoom.y));
        }
        if (roomLayout[selectedRoom.x + directions["Right"], selectedRoom.y] == null)
        {
            temp.Add(new Vector2Int(selectedRoom.x + directions["Right"], selectedRoom.y));
        }
        if (roomLayout[selectedRoom.x, selectedRoom.y + directions["Up"]] == null)
        {
            temp.Add(new Vector2Int(selectedRoom.x, selectedRoom.y + directions["Up"]));
        }
        if (roomLayout[selectedRoom.x, selectedRoom.y + directions["Down"]] == null)
        {
            temp.Add(new Vector2Int(selectedRoom.x, selectedRoom.y + directions["Down"]));
        }

        return temp;
    }

    public bool CheckNeighbour(Vector2Int neighbourRoom, int neighbourMax)
    {
        int counter = 0;

        Vector2Int[] position = new Vector2Int[]
{
            new Vector2Int( neighbourRoom.x + directions["Left"], neighbourRoom.y ),
            new Vector2Int( neighbourRoom.x + directions["Right"], neighbourRoom.y),
            new Vector2Int( neighbourRoom.x, neighbourRoom.y + directions["Up"]),
            new Vector2Int( neighbourRoom.x, neighbourRoom.y + directions["Down"]),
};

        for (int i = 0; i < position.Length; i++)
        {
            if (roomLayout[position[i].x, position[i].y] != null)
            {
                counter++;
            }
        }

        if (counter != neighbourMax || counter == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckChance(float expected)
    {
        float chance = Random.Range(1, 11);

        if (chance < expected)
        {
            return true;
        }

        return false;
    }

    public void GenerateUniqueStart()
    {
        generatedUniques = new bool[] { false, false };

        for (int x = 1; x < roomLayout.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < roomLayout.GetLength(1) - 1; y++)
            {
                if (!generatedUniques[0] || !generatedUniques[1])
                {
                    if (roomLayout[x, y] == null)
                    {
                        Vector2Int selectedRoom = new Vector2Int(x, y);
                        switch (generatedUniques[0])
                        {
                            case false:
                                if (GenerateUniqueRoom(SpecialState.Boss, selectedRoom))
                                {
                                    generatedUniques[0] = true;
                                }
                                break;
                        }
                        switch (generatedUniques[1])
                        {
                            case false:
                                if (GenerateUniqueRoom(SpecialState.Shop, selectedRoom))
                                {
                                    generatedUniques[1] = true;
                                }
                                break;
                        }
                    }
                }
                else if (generatedUniques[0] && generatedUniques[1])
                {
                    return;
                }
            }
        }
    }

    public bool GenerateUniqueRoom(SpecialState special, Vector2Int selectedRoom)
    {
        switch (special)
        {
            case SpecialState.Boss:
                if (CheckNeighbour(selectedRoom, 1) && CheckSpecialNeighbour(selectedRoom, SpecialState.Boss) && CheckChance(2.5f))
                {
                    CreateRoom(selectedRoom, "B");
                    return true;
                }
                else
                {
                    return false;
                }
            case SpecialState.Shop:
                if (CheckNeighbour(selectedRoom, 2) && CheckSpecialNeighbour(selectedRoom, SpecialState.Shop) && CheckChance(2.5f))
                {
                    CreateRoom(selectedRoom, "C");
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                Debug.Log("Return Error, Unique not found!");
                return false;
        }
    }

    public int GenerateRoomDifficulty()
    {
        float roomsLeft = (float)numberOfRooms / maxRoomNumber;

        return DetermineDifficultyScale(roomsLeft);
    
    }

    public int DetermineDifficultyScale(float roomsLeft)
    {
        if (roomsLeft <= 1 && roomsLeft > 0.8)
        {
            return 1;
        }
        else if (roomsLeft <= 0.8 && roomsLeft > 0.6)
        {
            return 2;
        }
        else if (roomsLeft <= 0.6 && roomsLeft > 0.4)
        {
            return 3;
        }
        else if (roomsLeft <= 0.4 && roomsLeft > 0.2)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }

    public int GenerateRoomVariant()
    {
        int index = Random.Range(0, variantsLeft.Count);
        int temp = variantsLeft[index];

        variantsLeft.RemoveAt(index);

        return temp;
    }

    public bool CheckSpecialNeighbour(Vector2Int neighbourRoom, SpecialState state)
    {
        Vector2Int[] position = new Vector2Int[]
        {
            new Vector2Int( neighbourRoom.x + directions["Left"], neighbourRoom.y ),
            new Vector2Int( neighbourRoom.x + directions["Right"], neighbourRoom.y),
            new Vector2Int( neighbourRoom.x, neighbourRoom.y + directions["Up"]),
            new Vector2Int( neighbourRoom.x, neighbourRoom.y + directions["Down"]),
        };

        for (int i = 0; i < position.Length; i++)
        {
            if (roomLayout[position[i].x, position[i].y] != null)
            {
                if (CheckLowLevel(position[i], state))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckLowLevel(Vector2Int neighbourRoom, SpecialState state)
    {
        switch (state)
        {
            case SpecialState.Boss:
                if (roomLayout[neighbourRoom.x, neighbourRoom.y].type == "C")
                {
                    return true;
                }
                else if (roomLayout[neighbourRoom.x, neighbourRoom.y].difficulty <= 2)
                {
                    return true;
                }
                break;
            case SpecialState.Shop:
                if (roomLayout[neighbourRoom.x, neighbourRoom.y].type == "B")
                {
                    return true;
                }
                else if (roomLayout[neighbourRoom.x, neighbourRoom.y].difficulty <= 1)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    public bool GeneratedCorrectly()
    {
        if (numberOfRooms == 0 && generatedUniques[0] && generatedUniques[1])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void BuildRoomLayout()
    {
        for (int x = 0; x < roomLayout.GetLength(0); x++)
        {
            for (int y = 0; y < roomLayout.GetLength(1); y++)
            {
                if (roomLayout[y, x] != null)
                {
                    string variantPath = "";

                    switch (roomLayout[y, x].type)
                    {
                        case "S":
                            variantPath = varietyPath + "Start";
                            break;
                        case "B":
                            variantPath = varietyPath + "Boss";
                            break;
                        case "C":
                            variantPath = varietyPath + "Shop";
                            break;
                        case "R":
                            variantPath = varietyPath + "Rooms/Variant_" + roomLayout[y, x].variant;
                            break;
                    }

                    GameObject room = Instantiate(Resources.Load(variantPath) as GameObject, new Vector2((x - generateOffset) * roomDimensions.x, (-y + generateOffset) * roomDimensions.y), Quaternion.identity, floorParent);
                    roomLayout[y, x].room = room;
                }
            }
        }
    }
}

public enum SpecialState
{
    Boss,
    Shop
}