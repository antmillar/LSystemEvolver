using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//turtle graphics class
class Turtle
{
    Vector3 _pos, _heading;
    Stack<Vector3> _pointStack;
    Stack<Vector3[]> _transformStack;
    float _lineAngle, _lineWidth, _lineRadius, _stepSize;

    List<Mesh> _lineMeshes;

    public Mesh _finalMesh;

    public Turtle(float defaultAngle)
    {
        _pos = new Vector3();
        _heading = Vector3.up;
        _pointStack = new Stack<Vector3>();
        _transformStack = new Stack<Vector3[]>();
        _lineAngle = defaultAngle;
        _stepSize = 0.05f;
        _lineMeshes = new List<Mesh>();
        _lineRadius = 0.1f;
    }

    //parses the string instructions to draw the shape
    public void Decode(string instructions){

        if(instructions == null) {throw new NullReferenceException("ERROR : null input to Turtle");}
        foreach(char c in instructions){

            switch(c)
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
                    var memory = new Vector3[2] {_pos, _heading};
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

                    _lineRadius *= 0.5f;
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
            DrawLineMesh();
            //DrawLine();
        }
        else
         _pos += _heading * _stepSize;
    }

    private void AddPoint(){

        _pointStack.Push(_pos);
    }

    private void Turn (float angle){

        var rotation  = Quaternion.AngleAxis(angle , Vector3.forward);
        _heading = rotation * _heading;
    }

    //deprecated : draws a line connecting the two points in the point stack
    private void DrawLine(){

        GameObject myLine = new GameObject();
        Vector3 start = _pointStack.Pop();
        Vector3 end = _pointStack.Pop();
        Color color = new Color(1,1,1);

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
        Vector3 startL = start - _lineRadius * lineNormal;
        Vector3 endL = startL + lineVector;

        Vector3 startR = start + _lineRadius * lineNormal;
        Vector3 endR = startR + lineVector;

        Vector3[] vertices = new Vector3[4];

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

    //combines the line meshes into a final mesh
    public void DrawMesh()
    {
        //combine meshes
        var finalMesh = new Mesh();

        var instances = _lineMeshes.Select(m => new CombineInstance() { mesh = m, transform = Matrix4x4.identity });

        finalMesh.CombineMeshes(instances.ToArray());
        _finalMesh = finalMesh;
    }


}

//questions :
// why is the line not extending the full length
// is JSON sensible?
//quad subdivision