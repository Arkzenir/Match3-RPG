using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Empty matrix
boosterReferenceList[0].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
 */


public static class Utils
{
    private static List<List<int[,]>> boosterReferenceList;
    private static List<int[,]> exclusionList;
    private static List<Vector2> lastList;
    private static List<Vector2> recentList;

    static Utils()
    {
        boosterReferenceList = new List<List<int[,]>>();
        exclusionList = new List<int[,]>();
        lastList = new List<Vector2>();
        recentList = new List<Vector2>();

        #region VRocket
        //Vertical Rocket
        boosterReferenceList.Add(new List<int[,]>());
        boosterReferenceList[0].Add(new int[5,5] 
            {
                {1,1,1,1,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        /*
        boosterReferenceList[0].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {1,1,1,1,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[0].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[0].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,0},
                {0,0,0,0,0}
            }
        );

        boosterReferenceList[0].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,0},
            }
        );
        */
        #endregion

        #region HRocket
        //Horizontal Rocket
        boosterReferenceList.Add(new List<int[,]>());
        boosterReferenceList[1].Add(new int[5,5] 
            {
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        /*
        boosterReferenceList[1].Add(new int[5,5] 
            {
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,0,0,0,0}
            }
        );
        boosterReferenceList[1].Add(new int[5,5] 
            {
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,0,0,0}
            }
        );
        boosterReferenceList[1].Add(new int[5,5] 
            {
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,0,0}
            }
        );
        boosterReferenceList[1].Add(new int[5,5] 
            {
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,0}
            }
        );
        */
        #endregion

        #region Star
        //Star
        boosterReferenceList.Add(new List<int[,]>());
        boosterReferenceList[2].Add(new int[5,5] 
            {
                {1,1,0,0,0},
                {1,1,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        #endregion

        #region Bomb
        //Bomb
        boosterReferenceList.Add(new List<int[,]>());
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {1,1,1,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {0,0,1,0,0},
                {1,1,1,0,0},
                {0,0,1,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {0,1,0,0,0},
                {0,1,0,0,0},
                {1,1,1,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {1,0,0,0,0},
                {1,1,1,0,0},
                {1,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {1,1,1,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {1,1,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {0,0,1,0,0},
                {0,0,1,0,0},
                {1,1,1,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[3].Add(new int[5,5] 
            {
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,1,1,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        #endregion

        #region Crystal
        //Crystal
        boosterReferenceList.Add(new List<int[,]>());
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {1,1,1,1,1},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        /*
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,0},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,1},
            }
        );
        */
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,0,0}
            }
        );
        /*
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,1,0},
                {0,0,0,1,0}
            }
        );
        
        boosterReferenceList[4].Add(new int[5,5] 
            {
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1},
                {0,0,0,0,1}
            }
        );
        */
        #endregion
        
        
    }
    
    public static List<List<int[,]>> GetReferenceMatrixList()
    {
        return boosterReferenceList;
    }

    public static void RotateMatrixInPlace(int[,] inMatrix)
    {
        int n = inMatrix.GetLength(0);
        int tmp;
        for (int i = 0; i < n / 2; i++)
        {
            for (int j = i; j < n - i - 1; j++)
            {
                tmp             = inMatrix[i,j];
                inMatrix[i,j]         = inMatrix[j,n-i-1];
                inMatrix[j,n-i-1]     = inMatrix[n-i-1,n-j-1];
                inMatrix[n-i-1,n-j-1] = inMatrix[n-j-1,i];
                inMatrix[n-j-1,i]     = tmp;
            }
        }
    }
    
    public static List<Vector2> GetLastPosList()
    {
        return lastList;
    }

    public static void AddToPosList(Vector2 add)
    {
        if (lastList.Count == 0)
        {
            lastList.Add(add);
        }else
            recentList.Add(add);
    }

    public static void ResetSwitchLists()
    {
        lastList.Clear();
        foreach (var v2 in recentList)
        {
            lastList.Add(v2);
        }
        recentList.Clear();
    }
    
}
