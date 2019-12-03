using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//turtle graphics class
class Turtle
{
    Vector3 _pos, _heading, _orientation;
    Stack<Vector3> _pointStack;
    Stack<Vector3[]> _transformStack;
    float _lineAngle, _lineWidth, _widthRatio, _stepSize, _lineWidthRatio;

    List<Mesh> _lineMeshes;

    public Mesh _finalMesh;

    public Turtle(float defaultAngle)
    {
        _pos = new Vector3();
        _heading = Vector3.up;
        _orientation = Vector3.back;
        _pointStack = new Stack<Vector3>();
        _transformStack = new Stack<Vector3[]>();
        _lineAngle = defaultAngle;
        _stepSize = 0.05f;
        _lineMeshes = new List<Mesh>();
        _widthRatio = 0.25f;
        _lineWidthRatio = 0.2f;
    }

    //parses the string instructions to draw the shape
    public void Decode(string instructions)
    {

        if (instructions == null) { throw new NullReferenceException("ERROR : null input to Turtle"); }
        foreach (char c in instructions)
        {

            switch (c)
            {
                case 'F': //move fwd and draw
                    Move();
                    break;

                case 'f': //move fwd only
                    Move(false);
                    break;

                case '+': //turn left by angle
                    Turn(-1 * _lineAngle);
                    break;

                case '-': //turn right by angle
                    Turn(_lineAngle);
                    break;

                case '|': //turn around
                    Turn(180f);
                    break;

                case '[': //push pos & heading to memory
                    var memory = new Vector3[2] { _pos, _heading };
                    _transformStack.Push(memory);
                    break;

                case ']': //pop pos & heading from memory

                    var recall = new Vector3[2];
                    recall = _transformStack.Pop();
                    _pos = recall[0];
                    _heading = recall[1];
                    break;

                case '"': //rescale step size

                    _stepSize *= 0.5f;
                    break;

                case '!': //rescale line width

                    _widthRatio *= 0.75f;
                    break;

                case '&': //pitch down by angle

                    Pitch(-90f);
                    break;

                case '^': //pitch up by angle
                    
                    Pitch(90f);
                    break;

                default:

                    throw new ArgumentException(c + " is not a valid argument");
            }
        }


    }

    private void Move(bool draw = true)
    {
        if (draw)
        {
            AddPoint();
            _pos += _heading * _stepSize;
            AddPoint();
            Draw3DStripMesh();
            //DrawLine();
        }
        else
            _pos += _heading * _stepSize;
    }

    private void AddPoint()
    {

        _pointStack.Push(_pos);
    }

    private void Pitch(float angle)
    {
        var rotationAxis = Vector3.Cross(_heading, _orientation);
        var rotation = Quaternion.AngleAxis(angle, rotationAxis);
        _heading = rotation * _heading;
        _orientation = Vector3.Cross(rotationAxis, _heading);
    }

    private void Turn(float angle)
    {
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _heading = rotation * _heading;
    }

    //deprecated : draws a line connecting the two points in the point stack
    private void DrawLine()
    {

        GameObject myLine = new GameObject();
        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();
        Color color = new Color(1, 1, 1);

        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = myLine.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

    //draws a line as a mesh
    private void DrawLineMesh()
    {

        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();

        Vector3 lineVector = end - start; //vector pointing from start to end
        Vector3 lineNormal = Vector3.Cross(lineVector, Vector3.forward);

        //draw a rectangle around the start and end points to represent a line
        Vector3 startL = start - _lineWidthRatio * lineNormal;
        Vector3 endL = startL + lineVector;

        Vector3 startR = start + _lineWidthRatio * lineNormal;
        Vector3 endR = startR + lineVector;

        Vector3[] vertices = new Vector3[8];

        vertices[0] = startL;
        vertices[1] = startR;
        vertices[2] = endL;
        vertices[3] = endR;

        int[] indices = new int[6] { 0, 2, 3, 3, 1, 0 }; //vertex ordering, must be clockwise

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        //mesh.SetIndices(indices, MeshTopology.Lines, 0); //can't use different mesh topologies with combine mesh..
        mesh.triangles = indices;
        _lineMeshes.Add(mesh);
    }

    //draws a two side strip
    private void DrawStripMesh()
    {
        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();

        Vector3 lineVector = end - start; //vector pointing from start to end
        Vector3 lineNormal = Vector3.Cross(lineVector, -_orientation);

        //draw a rectangle around the start and end points to represent a line
        Vector3 startL = start - _widthRatio * lineNormal;
        Vector3 endL = startL + lineVector;

        Vector3 startR = start + _widthRatio * lineNormal;
        Vector3 endR = startR + lineVector;

        Vector3[] vertices = new Vector3[8];

        //front vertices
        vertices[0] = startL;
        vertices[1] = startR;
        vertices[2] = endL;
        vertices[3] = endR;

        //back vertices
        vertices[4] = startL;
        vertices[5] = startR;
        vertices[6] = endL;
        vertices[7] = endR;

        //backfaces
        int[] indices = 
        { //frontfaces
          0, 3, 2,
          1, 3, 0,  
          //backfaces
          7, 4, 6,
          7, 5, 4
        }; //vertex ordering, must be clockwise

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        //mesh.SetIndices(indices, MeshTopology.Lines, 0); //can't use different mesh topologies with combine mesh..
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        _lineMeshes.Add(mesh);
    }


    //draws a two side strip
    private void Draw3DStripMesh()
    {
        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();

        Vector3 lineVector = end - start; //vector pointing from start to end
        Vector3 lineNormal = Vector3.Cross(lineVector, -_orientation);

        //draw a rectangle around the start and end points to represent a line
        Vector3 startL = start - _widthRatio * lineNormal;
        Vector3 endL = startL + lineVector;

        Vector3 startR = start + _widthRatio * lineNormal;
        Vector3 endR = startR + lineVector;

        Vector3 startLD = startL - _widthRatio * _orientation * lineNormal.magnitude;
        Vector3 endLD = endL - _widthRatio  * _orientation * lineNormal.magnitude;
        Vector3 startRD = startR - _widthRatio  * _orientation * lineNormal.magnitude;
        Vector3 endRD = endR - _widthRatio  * _orientation * lineNormal.magnitude;

        Vector3[] vertices = new Vector3[8];

        //front vertices
        vertices[0] = startL;
        vertices[1] = startR;
        vertices[2] = endR;
        vertices[3] = endL;

        //back vertices
        vertices[4] = endLD;
        vertices[5] = endRD;
        vertices[6] = startRD;
        vertices[7] = startLD;


        int[] indices = {
            0, 2, 1, //face front
	        0, 3, 2,
            2, 3, 4, //face top
	        2, 4, 5,
            1, 2, 5, //face right
	        1, 5, 6,
            0, 7, 4, //face left
	        0, 4, 3,
            5, 4, 7, //face back
	        5, 7, 6,
            0, 6, 7, //face bottom
	        0, 1, 6
        }; //vertex ordering, must be clockwise

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        //mesh.SetIndices(indices, MeshTopology.Lines, 0); //can't use different mesh topologies with combine mesh..
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        _lineMeshes.Add(mesh);
    }

    private void DrawBoxMesh()
    {

        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();

        Vector3 lineVector = end - start; //vector pointing from start to end
        Vector3 lineNormal = Vector3.Cross(lineVector, Vector3.forward);

        //draw a rectangle around the start and end points to represent a line
        Vector3 startL = start - lineNormal/2;
        Vector3 endL = startL + lineVector ;

        Vector3 startR = start + lineNormal/2;
        Vector3 endR = startR + lineVector;

        Vector3 startLT = start - lineNormal/2 + _stepSize * Vector3.forward;
        Vector3 endLT = startL + lineVector + _stepSize * Vector3.forward;

        Vector3 startRT = start + lineNormal/2 + _stepSize * Vector3.forward;
        Vector3 endRT = startR + lineVector + _stepSize * Vector3.forward;
        Vector3[] vertices = new Vector3[8];

        vertices[0] = startL;
        vertices[1] = startR;
        vertices[2] = endL;
        vertices[3] = endR;
        vertices[4] = endRT;
        vertices[5] = endLT;
        vertices[6] = startRT;
        vertices[7] = startLT;
        //int[] indices = new int[6] { 0, 2, 3, 3, 1, 0 }; //vertex ordering, must be clockwise



        int[] triangles = {
        0, 2, 1, //face front
	    1, 2, 3,
        2, 5, 3, //face top
	    3, 5, 4,
        1, 3, 6, //face right
	    6, 3, 4,
        7, 6, 5, //face left
	    6, 4, 5,
        0, 7, 2, //face back
	    7, 5, 2,
        1, 6, 0, //face bottom
	    0, 6, 7,
     //   //backfaces
     //   0, 1, 2, 
	    //0, 2, 3,
     //   2, 4, 3, 
	    //2, 5, 4,
     //   1, 5, 2, 
	    //1, 6, 5,
     //   0, 4, 7, 
	    //0, 3, 4,
     //   5, 7, 4, 
	    //5, 6, 7,
     //   0, 7, 6, 
	    //0, 6, 1,

        };



        

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        //mesh.SetIndices(indices, MeshTopology.Lines, 0); //can't use different mesh topologies with combine mesh..
        mesh.triangles = triangles;
        _lineMeshes.Add(mesh);




    }

    //combines the line meshes into a final mesh
    public void CreateMesh()
    {
        //combine meshes
        var finalMesh = new Mesh();

        var instances = _lineMeshes.Select(m => new CombineInstance() { mesh = m, transform = Matrix4x4.identity });

        finalMesh.CombineMeshes(instances.ToArray());
        _finalMesh = finalMesh;
    }
}