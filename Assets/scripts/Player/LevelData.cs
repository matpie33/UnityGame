using UnityEditor;
using UnityEngine;

public class LevelData
{
    public int level { get; set; }
    public int experience { get; set; }
    public int experienceNeededForNextLevel { get; set; }

    public LevelData()
    {
        level = 1;
        experience = 0;
        experienceNeededForNextLevel = 1000;
    }

    public bool AddExperience(int experience)
    {
        this.experience += experience;
        if (this.experience >= experienceNeededForNextLevel)
        {
            this.experience = 0;
            level++;
            return true;
        }
        return false;
    }
}
