using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CityBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject buildingBlockPrefab;
    public Obstacle obstaclePrefab;

    [Header("Maps (text matrix)")]
    public TextAsset cityMap;
    public TextAsset obstaclesMap;

    [Header("Obstacles and Free Positions")]
    public List<Obstacle> obstacles;
    public List<Vector3> freePositions;

    [Header("Max Points")]
    public int maxPoints;

    [Header("Global City Scale")]
    public float globalScale = 50f;

    void Awake()
    {
        freePositions = new List<Vector3>();
        obstacles = new List<Obstacle>();

        string cityMapText = cityMap.text;
        string obstaclesMapText = obstaclesMap.text;

        string[] cmLines = Regex.Split(cityMapText, @"\r\n");
        string[] omLines = Regex.Split(obstaclesMapText, @"\r\n");

        for (int i = 0; i < cmLines.Length; i++)
        {            
            string[] cmCells = Regex.Split(cmLines[i], @"\s+");
            string[] omCells = Regex.Split(omLines[i], @"\s+");

            for (int j = 0; j < cmCells.Length; j++)
            {                
                int.TryParse(omCells[j], out int obstaclePoints);

                if (cmCells[j].Equals("0"))
                {
                    if (obstaclePoints == 0)
                    {
                        // block (negative points)
                        GameObject block = Instantiate(buildingBlockPrefab);
                        Vector3 position = new Vector3(j, 0, -i) * globalScale;
                        block.transform.position = position;
                        block.transform.parent = transform;
                                          
                        // make city more interesting by adding noise randomnes for height
                        float height = KenPerlin.Noise(i * 1.23f, j * 4.217f).Remap(-1f, 1f, 0.05f, 5f);
                        block.transform.localScale = new Vector3(1f, height, 1f) * globalScale;
                    }
                    else
                    {
                        // big building obstacle (positive points)
                        instantiateObstacle(obstaclePoints, j, -i, 1f * globalScale);
                    } 
                }
                else if (obstaclePoints != 0)
                {
                    // small obstacles (positive points)                    
                    instantiateObstacle(obstaclePoints, j, -i, 0.25f * globalScale);
                }
                else
                {                       
                    freePositions.Add(new Vector3(j, 0, -i) * globalScale);
                }
            }
        }
    }

    void instantiateObstacle(int points, int x, int z, float scale)
    {
        maxPoints += points;
       
        Obstacle obstacle = Instantiate<Obstacle>(obstaclePrefab);
        obstacle.totalPoints = points;
        obstacle.currPoints = points;
        Vector3 position = new Vector3(x, 0, z) * globalScale;
        obstacle.transform.position = position;
        obstacle.transform.parent = transform;
        obstacle.transform.localScale = new Vector3(scale, scale, scale);

        obstacles.Add(obstacle);
    } 

    public void restartAllObstacles()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].restart();            
        }
    }
}
