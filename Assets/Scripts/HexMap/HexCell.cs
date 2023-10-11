using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    int distance;
    public RectTransform uiRect;
    public HexCoordinates coordinates;
    public Color color;
    [SerializeField]
    public HexCell[] neighbors;
    public PlaybleCharacter characterOccupiedCell;
    private Image highlight;
    void UpdateDistanceLabel()
    {
        TMP_Text label = uiRect.GetComponent<TMP_Text>();
        label.text = distance.ToString();
    }
    private void Start()
    {
        highlight = uiRect.GetChild(0).GetComponent<Image>();
    }
    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            //UpdateDistanceLabel();
        }
    }
    public HexCell PathFrom { get; set; }
    public void DisableHighlight()
    {
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        highlight.color = color;
        highlight.enabled = true;
    }
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
