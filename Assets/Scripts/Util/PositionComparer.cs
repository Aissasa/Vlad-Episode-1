using UnityEngine;
using System;
using System.Collections.Generic;

public class PositionComparer : IComparer<GameObject>
{

    private static PositionComparer pc;

    private PositionComparer()
    {

    }

    public static PositionComparer Instance
    {
        get
        {
            if (pc == null)
            {
                pc = new PositionComparer();
            }

            return pc;
        }
    }

    public int Compare(GameObject one, GameObject two)
    {
        if (one.transform.position.y > two.transform.position.y)
        {
            return 1;
        }
        if (one.transform.position.y < two.transform.position.y)
        {
            return -1;
        }
        return 0;
    }
}
