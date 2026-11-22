using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PathGenerator : MonoBehaviour
{
    [Header("Map")]
    public GameObject map;
    MeshRenderer mr;
    float map_length;
    float map_width;
    public GameObject guideParent;


    [Header("Grid")]
    int row;
    int col;
    Vector3[,] posMatrix;
    List<(float x, float z)> posList = new List<(float, float)>();



    

    [Header("Path Generations")]
    [UnityEngine.Range(1, 10)]
    public int forwardWeight = 3;
    public int minLength = 20;
    public bool noTouching = true;
    public int maxAttempts =  100;
    public List<Vector2Int> path = new List<Vector2Int>();
    HashSet<Vector2Int> onPath = new HashSet<Vector2Int>();
    Vector2Int startNode;
    Vector2Int endNode;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mr = map.GetComponent<MeshRenderer>();
        map_length = mr.bounds.size.x;
        map_width = mr.bounds.size.z;


        initializeMatrix();

        makeGrid();
        pickStartEnd();
        generatePath();
        drawPath();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Debug print the size of the platform
    public void printSize() 
    {
        print("Length: " + map_length.ToString());
        print("Width: " + map_width.ToString());
    }


    // initialize matrix with columns and row

    /* ================================
                GRID STUFF 
       ================================ */
    // divide the platform into grid
    void makeGrid() 
    {
        // top left/ starting location

        float start_x = map.transform.position.x - map_length / 2;
        float start_z = map.transform.position.z + map_width / 2 ;

        float end_x = map.transform.position.x + map_length / 2;
        float end_z = map.transform.position.z - map_width / 2;
        



        (float x, float z) pos;
        Vector3 start_pos = new Vector3(start_x, map.transform.position.y + 0.5f, start_z);
        Vector3 start_pos_z = new Vector3(start_x, map.transform.position.y + 0.5f, start_z);

        int c = 0;
        int r = 0;
        while (start_pos.z > end_z) 
        {
            c = 0;
            start_pos = start_pos_z;
            while (start_pos.x <= end_x) 
            {
                pos = (start_pos.x, start_pos.z);
                posMatrix[c, r] = new Vector3(start_pos.x, map.transform.position.y + 0.5f, start_pos.z);
                start_pos = start_pos + new Vector3(2, 0, 0);
                
                c++;
                posList.Add(pos);
                
            }

            r++;
            start_pos_z = start_pos_z - new Vector3(0, 0, 2);
        }
    }

    public void makeGuide() 
    {
        foreach (var position in posList)
        {
            Debug.Log($"x: {position.x}, z: {position.z}");


            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            tmp.transform.position = new Vector3(position.x, map.transform.position.y + 0.5f, position.z);
            tmp.transform.SetParent(guideParent.transform);
        }
    }


    /* ================================
                MATRIX STUFF 
       ================================ */
    private void initializeMatrix()
    {
        float start_x = map.transform.position.x - map_length / 2;
        float start_z = map.transform.position.z + map_width / 2;

        float end_x = map.transform.position.x + map_length / 2;
        float end_z = map.transform.position.z - map_width / 2;

        Vector3 start_pos = new Vector3(start_x, map.transform.position.y + 0.5f, start_z);
        Vector3 start_pos_z = new Vector3(start_x, map.transform.position.y + 0.5f, start_z);


        while (start_pos.z > end_z)
        {
            start_pos = start_pos_z;
            int c = 0;
            while (start_pos.x <= end_x)
            {
                c++;
                start_pos = start_pos + new Vector3(2, 0, 0);
            }
            col = c;
            start_pos_z = start_pos_z - new Vector3(0, 0, 2);
            row++;
        }

        posMatrix = new Vector3[col, row];
    }
    public void printMatrix() 
    {
        StringBuilder sb = new StringBuilder();
        int c = posMatrix.GetLength(0);
        int r = posMatrix.GetLength(1);

        Debug.Log($"{r} {c}");

        for (int i = 0; i < r; i++) 
        {
            for (int j = 0; j < c; j++) 
            {
                sb.Append(posMatrix[j, i]);
                sb.Append("\t");

            }
            sb.AppendLine();

        }

        Debug.Log(sb.ToString());
    }

    public void genKey(string a) 
    {
        
        string[] part = a.Split(",");
        int c = int.Parse(part[0]);
        int r = int.Parse(part[1]);
        print(posMatrix[c, r]);
        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tmp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        tmp.transform.position = posMatrix[c,r];
        tmp.transform.SetParent(guideParent.transform);
    }

    public void pickStartEnd() 
    {
        print(row);
        print(col);
        startNode = new Vector2Int(0, Random.Range(0, row));
        endNode = new Vector2Int(col-1, Random.Range(0, row));

        
        makeNode(startNode,"Start");
        makeNode(endNode,"End");
    }


    void makeNode(Vector2Int n,string name)
    {
        GameObject tmp =   GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tmp.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        tmp.transform.position = posMatrix[n.x, n.y];
        tmp.transform.name = name;
        //tmp.transform.SetParent(guideParent.transform);
        tmp.GetComponent<Renderer>().material.color = Color.red;
    }

    /* ================================
                Path STUFF 
       ================================ */


    public void generatePath()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            walk();

            if (path.Count >= minLength)
            {
                Debug.Log($"Path found: {path.Count} nodes (attempt {attempt + 1})");
                return;
            }
        }

        Debug.LogWarning("Could not reach minLength. Lower it, lower forwardWeight, or turn off noTouching.");
    }

    
    void walk()
    {
        path.Clear();
        onPath.Clear();

        Vector2Int pos = startNode;
        path.Add(pos);
        onPath.Add(pos);

        while (pos != endNode)
        {
            Vector2Int next;

            if (pos.x == col - 1)
            {
                // last column: walk straight up or down to the end row
                int step = endNode.y > pos.y ? 1 : -1;
                next = new Vector2Int(pos.x, pos.y + step);
            }
            else
            {
                List<Vector2Int> options = new List<Vector2Int>();

                // right is always allowed, added several times so the path favors moving forward
                Vector2Int right = pos + new Vector2Int(1, 0);
                for (int i = 0; i < forwardWeight; i++) options.Add(right);

                Vector2Int up = pos + new Vector2Int(0, -1);   // row 0 is the top
                Vector2Int down = pos + new Vector2Int(0, 1);

                if (canStep(up, pos)) options.Add(up);
                if (canStep(down, pos)) options.Add(down);

                next = options[Random.Range(0, options.Count)];
            }

            pos = next;
            path.Add(pos);
            onPath.Add(pos);
        }
    }

    
    bool canStep(Vector2Int cell, Vector2Int cameFrom)
    {
        if (cell.y < 0 || cell.y >= row) return false;        // off the grid
        if (onPath.Contains(cell)) return false;               // already used
        if (noTouching && touchesPath(cell, cameFrom)) return false;
        return true;
    }

    
    bool touchesPath(Vector2Int cell, Vector2Int cameFrom)
    {
        Vector2Int[] around = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

        foreach (Vector2Int d in around)
        {
            Vector2Int n = cell + d;
            if (n != cameFrom && onPath.Contains(n)) return true;
        }
        return false;
    }

    
    public void drawPath()
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 a = posMatrix[path[i].x, path[i].y];
            Vector3 b = posMatrix[path[i + 1].x, path[i + 1].y];
            Debug.DrawLine(a, b, Color.red, 100f);


        }
        visualizePath();
    }

    public void visualizePath() 
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 a = posMatrix[path[i].x, path[i].y];
            Vector3 b = posMatrix[path[i + 1].x, path[i + 1].y];

            a.y = map.transform.position.y + 0.1f;
            b.y = map.transform.position.y + 0.1f;

            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.transform.position = (a + b) / 2f;
            road.transform.rotation = Quaternion.LookRotation(b - a);
            road.transform.localScale = new Vector3(1f,0.2f,Vector3.Distance(a, b) + 1f);
            road.transform.parent = guideParent.transform;
        }
    }
    // added by gmo:
    // returns the path as world positions for enemies to walk it
    public List<Vector3> GetWorldPath()
    {
        List<Vector3> points = new List<Vector3>();
        foreach (Vector2Int p in path)
            points.Add(posMatrix[p.x, p.y]);
        return points;
    }
}
