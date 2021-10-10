using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequareGrid : MonoBehaviour
{
    private int cellCountX;
    private int cellCountZ;
    public int chunkCountX = 4;
    public int chunkCountZ = 3;
    public SequareGridChunk chunkPrefab;

    private SequareGridChunk[] chunks;

    public SequareCell cellPrefab;
    public TMP_Text textPrefab;
    public Color defaultColor = Color.white;

    public Texture2D noiseSource;
    private SequareCell[] cells;


    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

    }
    private void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }
    private void CreateChunks()
    {
        chunks = new SequareGridChunk[chunkCountX * chunkCountZ];
        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                SequareGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }

    }
    private void CreateCells()
    {
        cells = new SequareCell[cellCountZ * cellCountX];
        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }



    //private void Start()
    //{
    //    sequareMesh.Triangulate(cells);
    //}

    //public void Refresh()
    //{
    //    sequareMesh.Triangulate(cells);
    //}

    //public void ColorCell(Vector3 position, Color color)
    //{
    //    SequareCell cell = GetCell(position);
    //    cell.color = color;
    //    sequareMesh.Triangulate(cells);
    //}


    public SequareCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        SequareCoordinates coordinates = HexMetrics.FromPosition(position);
        int index = HexMetrics.PositionConvertToIndex(coordinates, cellCountX);

        return cells[index];
    }


    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = x * 10f;
        position.y = 0f;
        position.z = z * 10f;

        SequareCell cell = cells[i] = Instantiate<SequareCell>(cellPrefab);

        cell.transform.localPosition = position;
        cell.coordinates = HexMetrics.FromOffsetCoordinates(x, z);
        cell.SetColor(defaultColor);
        if (x > 0)
        {
            cell.SetNeighbor(EnumDirection.West, cells[i - 1]);
            cells[i - 1].SetNeighbor(EnumDirection.East, cell);
            if (z > 0)//西南东北邻居
            {
                int indexWs = HexMetrics.PositionConvertToIndex(x - 1, z - 1, cellCountX);
                cells[indexWs].SetNeighbor(EnumDirection.WestSouth, cell);
                cell.SetNeighbor(EnumDirection.EastNorth, cells[indexWs]);
            }
        }
        if (z > 0)
        {
            cell.SetNeighbor(EnumDirection.South, cells[i - cellCountX]);
            cells[i - cellCountX].SetNeighbor(EnumDirection.North, cell);
            if (x < cellCountX - 1)//东南 西北邻居
            {
                int indexEs = HexMetrics.PositionConvertToIndex(x + 1, z - 1, cellCountX);
                cells[indexEs].SetNeighbor(EnumDirection.WestNorth, cell);
                cell.SetNeighbor(EnumDirection.EastSouth, cells[indexEs]);
            }
        }



        TMP_Text label = Instantiate<TMP_Text>(textPrefab);
        // label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToString();
        cell.uiRect = label.rectTransform;


        cell.SetElevation(0);
        cell.SetColor(defaultColor);

        AddCellToChunk(x, z, cell);
    }
    private void AddCellToChunk(int x, int z, SequareCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        SequareGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);

        cell.gameObject.name = cell.ToString();
    }
}
