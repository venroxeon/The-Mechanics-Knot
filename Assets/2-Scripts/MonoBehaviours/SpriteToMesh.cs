using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

public class SpriteToMesh : MonoBehaviour
{
    public Sprite sprite;
    public Material material;

    Sprite prevSprite;
    MeshFilter meshFilter;
    MeshRenderer meshRend;

    void OnValidate()
    {
        if (sprite != null && sprite != prevSprite)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRend = GetComponent<MeshRenderer>();
            AssignMesh();
        }
    }

    void AssignMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = sprite.name + "_mesh";
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);

        meshFilter.mesh = mesh;

        material.mainTexture = sprite.texture;
        meshRend.material = material;

        prevSprite = sprite;
    }

    //[CustomEditor(typeof(SpriteToMesh))]
    //public class SpriteToMeshEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        if (GUI.changed)
    //        {
    //            ((SpriteToMesh)target).OnValidate();
    //        }
    //    }
    //}

    //void AssignMesh()
    //{
    //    Mesh mesh = new Mesh();
    //    Vector2[] spriteVertices = sprite.vertices;

    //    int[] spriteTriangles = new int[spriteVertices.Length];
    //    ushort[] _spriteTriangles = sprite.triangles;

    //    for (int i = 0; i < spriteTriangles.Length; i++)
    //        spriteTriangles[i] = _spriteTriangles[i];

    //    Vector2[] spriteUVs = sprite.uv;

    //    Vector3[] vertices = new Vector3[spriteVertices.Length];
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        vertices[i] = spriteVertices[i];
    //    }

    //    mesh.vertices = vertices;
    //    mesh.triangles = spriteTriangles;
    //    mesh.uv = spriteUVs;
    //    mesh.RecalculateNormals();
    //    mesh.RecalculateBounds();

    //    meshFilter.mesh = mesh;

    //    material.mainTexture = sprite.texture;
    //    meshRend.material = material;

    //    prevSprite = sprite;
    //}
}