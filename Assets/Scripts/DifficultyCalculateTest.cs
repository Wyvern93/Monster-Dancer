using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyCalculateTest : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        CalculateDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CalculateDifficulty()
    {
        float runnerMultiplier = 1f;
        float bomberMultiplier = 2f;
        float shooterMultiplier = 3f;

        List<float> difficultyValues = new List<float>();

        // Runners 0-5x
        for (int i = 0; i < 5; i++)
        {
            difficultyValues.Add(i * runnerMultiplier);
            // Additional Bombers
            for (int j = 0; j < 5; j++)
            {
                difficultyValues.Add((i * runnerMultiplier) + (j * bomberMultiplier));
            }
            // Additional Shooters
            for (int j2 = 0; j2 < 3; ++j2)
            {
                difficultyValues.Add((i * runnerMultiplier) + (j2 * shooterMultiplier));
            }
        }

        // Shooters 0-5x
        for (int i = 0; i < 3; i++)
        {
            difficultyValues.Add(i * shooterMultiplier);
            // Additional Bombers
            for (int j = 0; j < 5; j++)
            {
                difficultyValues.Add((i * shooterMultiplier) + (j * bomberMultiplier));
            }
        }

        difficultyValues.Sort();

        DrawLineRenderer(difficultyValues);
    }

    void DrawLineRenderer(List<float> values)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0;i < values.Count;i++)
        {
            positions.Add(new Vector3(i * 0.3f, values[i], 0));
        }
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
