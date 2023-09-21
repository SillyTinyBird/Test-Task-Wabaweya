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
    HexCell[] neighbors;
    public PlaybleCharacter characterOccupiedCell;
    void UpdateDistanceLabel()
    {
        TMP_Text label = uiRect.GetComponent<TMP_Text>();
        label.text = distance.ToString();
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
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
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
