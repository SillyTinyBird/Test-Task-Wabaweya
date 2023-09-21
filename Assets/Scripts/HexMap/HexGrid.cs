using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;
[DefaultExecutionOrder(-1)]
public class HexGrid : MonoBehaviour
{
    private static HexGrid instance;

    private HexGrid()
    { }

    public static HexGrid getInstance()
    {
        if (instance == null)
            instance = new HexGrid();
        return instance;
    }

    [SerializeField] private int _width = 6;
    [SerializeField] private int _height = 6;

    [SerializeField] private HexCell _cellPrefab;
    [SerializeField] private TMP_Text cellLabelPrefab;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color touchedColor = Color.magenta;
    private static HexCell[] cells;
    private Canvas gridCanvas;
    private HexMesh hexMesh;
    private static HexCell lastTouchedCell;
    private static HexCell activeCell;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[_height * _width];

        for (int z = 0, i = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
        activeCell = cells[0];//just so we dont have null ptr error
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    public HexCell GetRandomCell()
    {
        int index = Random.Range(0, cells.Length);
        return cells[index];
    }
    public void SetActiveCell(HexCell cell)
    {
        ClearHighlights();
        activeCell.DisableHighlight();
        activeCell = cell;
        activeCell.EnableHighlight(Color.blue);
    }
    public HexCell GetLastTouchedCell() => lastTouchedCell;
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        if(lastTouchedCell != null)
        {
            lastTouchedCell.DisableHighlight();
        }
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        //Debug.Log("touched at " + coordinates.ToString());
        int index = coordinates.X + coordinates.Z * _width + coordinates.Z / 2;
        HexCell cell = cells[index];
        //cell.color = touchedColor;
        
        lastTouchedCell = cell;
        lastTouchedCell.EnableHighlight(Color.red);
        FindPath(activeCell, cell);
        //hexMesh.Triangulate(cells);
    }
    void Start()
    {
        hexMesh.Triangulate(cells);
    }
    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.z = 0f;//becouse unity
        position.y = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(_cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - _width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - _width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - _width]);
                if (x < _width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - _width + 1]);
                }
            }
        }
        TMP_Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
    }
    public void FindPath(HexCell fromCell, HexCell toCell)
    {
        StopAllCoroutines();
        StartCoroutine(Search(fromCell, toCell));
    }
    private void ClearHighlights()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].DisableHighlight();
        }
    }
    IEnumerator Search(HexCell fromCell, HexCell toCell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
            if (cells[i] == fromCell || cells[i] == toCell)
                continue;
            cells[i].DisableHighlight();
        }

        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
        List<HexCell> frontier = new List<HexCell>();
        fromCell.Distance = 0;
        frontier.Add(fromCell);
        while (frontier.Count > 0)
        {
            yield return delay;
            HexCell current = frontier[0];
            frontier.RemoveAt(0);
            if (current == toCell)
            {
                current = current.PathFrom;
                while (current != fromCell)
                {
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
                break;
            }

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.Distance != int.MaxValue)
                {
                    continue;
                }
                if (neighbor.characterOccupiedCell != null)
                {
                    continue;
                }
                frontier.Add(neighbor);
                int distance = current.Distance + 1;
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    frontier.Add(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                }
                frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            }
        }
    }
}
