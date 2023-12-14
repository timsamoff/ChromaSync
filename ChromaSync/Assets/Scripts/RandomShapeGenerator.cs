using UnityEngine;

public class RandomPlatonicSolidGenerator : MonoBehaviour
{
    private void Start()
    {
        GenerateRandomPlatonicSolid();
    }

    private void GenerateRandomPlatonicSolid()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        PlatonicSolidType solidType = GetRandomSolidType();

        switch (solidType)
        {
            case PlatonicSolidType.Tetrahedron:
                GenerateTetrahedron(mesh);
                break;
            case PlatonicSolidType.Cube:
                GenerateCube(mesh);
                break;
            case PlatonicSolidType.Octahedron:
                GenerateOctahedron(mesh);
                break;
            case PlatonicSolidType.Dodecahedron:
                GenerateDodecahedron(mesh);
                break;
            case PlatonicSolidType.Icosahedron:
                GenerateIcosahedron(mesh);
                break;
            case PlatonicSolidType.Triacontagon:
                GenerateTriacontagon(mesh);
                break;
        }

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;
    }

    private PlatonicSolidType GetRandomSolidType()
    {
        // Enum representing different platonic solid types
        PlatonicSolidType[] solidTypes = (PlatonicSolidType[])System.Enum.GetValues(typeof(PlatonicSolidType));

        // Get a random platonic solid type
        PlatonicSolidType randomSolidType = solidTypes[Random.Range(0, solidTypes.Length)];

        return randomSolidType;
    }

    private void GenerateTetrahedron(Mesh mesh)
    {
        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, -1, -1)
        };

        // Triangles
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 1,
            1, 3, 2
        };

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void GenerateCube(Mesh mesh)
    {
        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f)
        };

        // Triangles
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            4, 5, 6,
            4, 6, 7,
            0, 4, 7,
            0, 7, 3,
            1, 5, 6,
            1, 6, 2,
            0, 4, 5,
            0, 5, 1,
            2, 6, 7,
            2, 7, 3
        };

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void GenerateOctahedron(Mesh mesh)
    {
        // Vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, -1),
            new Vector3(0, -1, 0)
        };

        // Triangles
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
        };

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void GenerateDodecahedron(Mesh mesh)
    {
        // Golden ratio
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;
        float invPhi = 1f / phi;

        // Vertices
        Vector3[] vertices = new Vector3[20];

        vertices[0] = new Vector3(0, 1, phi);
        vertices[1] = new Vector3(0, 1, -phi);
        vertices[2] = new Vector3(0, -1, phi);
        vertices[3] = new Vector3(0, -1, -phi);

        vertices[4] = new Vector3(1, phi, 0);
        vertices[5] = new Vector3(1, -phi, 0);
        vertices[6] = new Vector3(-1, phi, 0);
        vertices[7] = new Vector3(-1, -phi, 0);

        vertices[8] = new Vector3(phi, 0, 1);
        vertices[9] = new Vector3(-phi, 0, 1);
        vertices[10] = new Vector3(phi, 0, -1);
        vertices[11] = new Vector3(-phi, 0, -1);

        vertices[12] = new Vector3(invPhi, invPhi, invPhi);
        vertices[13] = new Vector3(-invPhi, invPhi, invPhi);
        vertices[14] = new Vector3(invPhi, -invPhi, invPhi);
        vertices[15] = new Vector3(-invPhi, -invPhi, invPhi);

        vertices[16] = new Vector3(invPhi, invPhi, -invPhi);
        vertices[17] = new Vector3(-invPhi, invPhi, -invPhi);
        vertices[18] = new Vector3(invPhi, -invPhi, -invPhi);
        vertices[19] = new Vector3(-invPhi, -invPhi, -invPhi);

        // Triangles
        int[] triangles = new int[]
        {
            0, 8, 4,
            0, 5, 11,
            1, 4, 8,
            1, 10, 5,
            2, 9, 7,
            2, 3, 6,
            3, 7, 9,
            6, 11, 10,
            0, 4, 1,
            0, 11, 5,
            1, 8, 10,
            1, 5, 0,
            2, 7, 9,
            2, 6, 3,
            3, 9, 7,
            6, 10, 11,
            12, 14, 16,
            12, 17, 14,
            13, 16, 18,
            13, 19, 17,
            15, 18, 14,
            15, 14, 12,
            15, 12, 16,
            15, 16, 13,
            15, 13, 17,
            15, 17, 19,
            15, 19, 18,
            14, 18, 16,
            12, 14, 13,
            16, 18, 15,
            13, 19, 17
        };

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void GenerateIcosahedron(Mesh mesh)
    {
        // Golden ratio
        float phi = (1f + Mathf.Sqrt(5f)) / 2f;
        float invPhi = 1f / phi;

        // Vertices
        Vector3[] vertices = new Vector3[12];

        vertices[0] = new Vector3(0, 1, phi);
        vertices[1] = new Vector3(0, 1, -phi);
        vertices[2] = new Vector3(0, -1, phi);
        vertices[3] = new Vector3(0, -1, -phi);

        vertices[4] = new Vector3(1, phi, 0);
        vertices[5] = new Vector3(1, -phi, 0);
        vertices[6] = new Vector3(-1, phi, 0);
        vertices[7] = new Vector3(-1, -phi, 0);

        vertices[8] = new Vector3(phi, 0, 1);
        vertices[9] = new Vector3(-phi, 0, 1);
        vertices[10] = new Vector3(phi, 0, -1);
        vertices[11] = new Vector3(-phi, 0, -1);

        // Triangles
        int[] triangles = new int[]
        {
            0, 8, 4,
            0, 5, 11,
            1, 4, 8,
            1, 10, 5,
            2, 9, 7,
            2, 3, 6,
            3, 7, 9,
            6, 11, 10,
            0, 4, 1,
            0, 11, 5,
            1, 8, 10,
            1, 5, 0,
            2, 7, 9,
            2, 6, 3,
            3, 9, 7,
            6, 10, 11
        };

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void GenerateTriacontagon(Mesh mesh)
    {
        // Number of sides
        int sides = 30;

        // Vertices
        Vector3[] vertices = new Vector3[sides + 1];

        // Calculate vertices on the unit circle
        for (int i = 0; i < sides; i++)
        {
            float angle = (2 * Mathf.PI * i) / sides;
            vertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        }

        // Center vertex
        vertices[sides] = Vector3.zero;

        // Triangles
        int[] triangles = new int[sides * 3];

        for (int i = 0; i < sides; i++)
        {
            triangles[i * 3] = i;
            triangles[i * 3 + 1] = (i + 1) % sides;
            triangles[i * 3 + 2] = sides;
        }

        // Assign arrays to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    // Enum to represent different platonic solid types
    public enum PlatonicSolidType
    {
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron,
        Triacontagon
    }
}