using System.Collections.Generic;

public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove;

    //Constructor for zombie goals
    public SubGoal(string s, int i, bool r)
    {
        sgoals = new Dictionary<string, int>();
        sgoals.Add(s, i);
        remove = r;
    }
}