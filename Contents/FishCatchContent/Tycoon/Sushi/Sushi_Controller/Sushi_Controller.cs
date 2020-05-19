using JHchoi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sushi_Controller : IFood
{
    protected override void CatchSoundPlay()
    {
        SoundManager.Instance.PlaySound((int)SoundFishCatch.Shshi_Catch);
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
