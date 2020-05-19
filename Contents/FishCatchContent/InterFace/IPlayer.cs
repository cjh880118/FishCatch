using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPlayer 
{
    int index;
    int score;

    public virtual void InitPlayer(int index)
    {
        this.index = index;
        score = 0;
    }

    public virtual void SetScore(int score)
    {
        score += score;
    }
}
