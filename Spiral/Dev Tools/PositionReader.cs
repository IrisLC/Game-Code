using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;


public class PositionReader : EditorWindow
{
    static string FilePath;
    StreamReader reader;
    /// <summary>
    /// The characters that the positions are separated by in the log files
    /// </summary>
    static char[] separators = new char[] { '(', ')' };

    /// <summary>
    /// A list of lists of positions, each list holds one playthrough in order.
    /// </summary>
    static List<List<Vector3>> Positions;

    /// <summary>
    /// Whether to show all paths in the file at once (true), or one at a time (false)
    /// </summary>
    static bool ViewAllPaths = true;
    /// <summary>
    /// The index of the path to view when ViewAllPaths is false
    /// </summary>
    static int PathToView;
    /// <summary>
    /// Whether to view a single time point at a time (true), or all the time points in a path at once (false)
    /// </summary>
    static bool ViewSingleTimePoint;
    /// <summary>
    /// The index of the time point to view when ViewSingleTimePoint is true
    /// </summary>
    static int PointToView;
    /// <summary>
    /// Whether or not to show the lines connecting the time points together 
    /// </summary>
    static bool ShowLines;

    /// <summary>
    /// Array of different colors with which to draw the different paths
    /// </summary>
    static Color[] colors = { Color.aquamarine, Color.red, Color.forestGreen, Color.blue, Color.purple,
                            Color.brown, Color.violet, Color.beige, Color.azure, Color.tan, Color.lightCoral,
                            Color.black, Color.gold, Color.orange, Color.indigo };


    [MenuItem("Tools/ReadPlaytestPosition")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PositionReader));
    }

    private void OnGUI()
    {
        /*Start dealing with file*/
        GUILayout.Label("Read File", EditorStyles.boldLabel);

        if (GUILayout.Button("Find File"))
        {
            FilePath = EditorUtility.OpenFilePanelWithFilters("Select File", "Assets/LogFiles", new string[] { "Text File", "txt" });
        }
        GUILayout.TextArea("Selected File: " + FilePath);

        if (GUILayout.Button("Read File"))
        {
            ReadFile();
        }
        /*End dealing with file*/

        /*Start view methods*/
        ShowLines = EditorGUILayout.Toggle("Show Path Lines", ShowLines);
        ViewAllPaths = EditorGUILayout.Toggle("Show All Paths", ViewAllPaths);

        if (Positions != null)
        {
            if (!ViewAllPaths)
            {
                PathToView = EditorGUILayout.IntSlider(PathToView, 0, Positions.Count - 1);

                ViewSingleTimePoint = EditorGUILayout.Toggle("Show Single Point", ViewSingleTimePoint);
            }

            if (ViewSingleTimePoint)
            {
                PointToView = EditorGUILayout.IntSlider(PointToView, 0, Positions[PathToView].Count - 1);
            }
        }

        /*End view methods*/

        // Button incase there's an error in the reading, and the file doesn't properly close
        if (reader != null && GUILayout.Button("Close File"))
        {
            reader.Close();
        }

    }

    /// <summary>
    /// Reads a position log file, and fills the positions list with the different paths.
    /// </summary>
    void ReadFile()
    {
        Positions = new List<List<Vector3>>();
        bool ShouldContinue = false;
        reader = new StreamReader(FilePath);
        // The index of the current list in Positions
        int index = 0;

        // As long as we are not at the end of the file we should work on it.
        while (!reader.EndOfStream)
        {
            // Each time we loop through it will be a new playthrough.
            Positions.Add(new List<Vector3>());

            // Try to find the START statement, if we find it leave the loop and move on
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.Contains("START"))
                {
                    ShouldContinue = true;
                    break;
                }
            }

            // If we didn't find the START statement close the file and leave as there is no correctly formatted log to find
            if (!ShouldContinue)
            {
                reader.Close();
                return;
            }

            // Loop through the file until we get to the end, either of the file or the current log.
            //  While in the loop we will be getting the positions of the playthrough and adding them to Positions.
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                // END marks the end of a log
                if (line.Contains("END"))
                {
                    break;
                }

                // Logs will be in format of (1, 2, 3)

                // Remove the parenthesis and get the string with the numbers
                string split = line.Split(separators)[1];
                string[] FoundValues = split.Split(',');

                float x, y, z;
                float.TryParse(FoundValues[0], out x);
                float.TryParse(FoundValues[1], out y);
                float.TryParse(FoundValues[2], out z);

                Positions[index].Add(new Vector3(x, y, z));
            }
            index++;
        }

        reader.Close();

    }

    // While selecting the PositionLogger game object in the scene, it will draw the positions throughout the level
    [DrawGizmo(GizmoType.Active)]
    static void OnDrawGizmos(PositionLogger logger, GizmoType gizmoType)
    {
        if (Positions == null) return;

        Gizmos.color = colors[0];
        int index = 0;
        Vector3 LastPosition = Vector3.zero;

        if (!ViewAllPaths)
        {
            if (!ViewSingleTimePoint)
            {
                foreach (Vector3 position in Positions[PathToView])
                {
                    Gizmos.DrawSphere(position, 1);
                    if (ShowLines && LastPosition != Vector3.zero)
                    {
                        Gizmos.DrawLine(position, LastPosition);
                    }

                    LastPosition = position;
                }
            }
            else
            {
                Gizmos.DrawSphere(Positions[PathToView][PointToView], 1);
            }

            return;
        }

        // We get here if the user wants to draw all the positions
        foreach (List<Vector3> lists in Positions)
        {
            foreach (Vector3 position in lists)
            {
                Gizmos.DrawSphere(position, 1);
                if (ShowLines && LastPosition != Vector3.zero)
                {
                    Gizmos.DrawLine(position, LastPosition);
                }

                LastPosition = position;
            }
            LastPosition = Vector3.zero;
            Gizmos.color = colors[++index];
        }

    }


}
