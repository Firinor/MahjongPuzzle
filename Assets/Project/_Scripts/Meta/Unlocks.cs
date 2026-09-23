using UnityEngine;

[CreateAssetMenu(fileName = "Unlocks", menuName = "Majhong/Unlocks")]
public class Unlocks : ScriptableObject
{
    public int[] Levels =
    {
        3000,//Cat
        7000,//Euro
        13000,//Flower
        20000,//Plane
        29000,//Gems
        40000,//Mountain
        53000,//Rabbit
        68000,//Castle
        85000,//Batterfly
        //Fish
        //Elefatn
        1000000//Castle
    };
    public int[] MedalLevels =
    {
        1,//Mountain
        2,//Taro
        5,//Wall
        20,//TheCube
    };
    public string[] KeyWords =
    {
        "Cat",
        "Euro",
        "Flower",
        "Plane",
        "Gems",
        "Mountain",
        "Rabbit",
        "Castle",
        "Butterfly",
    };
    public string[] MedalKeyWords =
    {
        "Mountain",
        "Taro",
        "Wall",
        "TheCube"
    };
    public Sprite[] Sprites;
}