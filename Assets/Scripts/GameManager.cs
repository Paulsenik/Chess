using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour {

    public static List<Figure> figs = new List<Figure>();
    public static List<Tile> tiles = new List<Tile>();

    public Material whiteMaterial, blackMaterial, selectedMaterial;
    public GameObject killEffect;

    public static void clickedTile(Tile t) {
        if (Figure.getSelectedFigure() != null) {
            Tile.setSelectedTile(t);
            Figure.getSelectedFigure().moveObjectToTile(t);
            Figure.setSelectedFigure(null);
            Tile.setSelectedTile(null);
        } else {
            Figure.setSelectedFigure(getFigure(t.xOnBoard, t.zOnBoard));
        }
    }

    public static void clickedFigure(Figure f) {
        if (Figure.getSelectedFigure() == null) {
            Figure.setSelectedFigure(f);
        } else if (Figure.getSelectedFigure() == f) {
            Figure.setSelectedFigure(null);
        } else if (Figure.getSelectedFigure().color == f.color) {
            Figure.setSelectedFigure(f);
        } else {
            clickedTile(getTile(f.xOnBoard, f.zOnBoard));
        }
    }

    public static Figure getFigure(int xOnBoard, int zOnBoard) {
        for (int i = 0; i < figs.Count; i++) {
            if (figs[i].xOnBoard == xOnBoard) {
                if (figs[i].zOnBoard == zOnBoard)
                    return figs[i];
            }
        }
        return null;
    }

    public static Figure getFigure(Tile t) {
        return getFigure(t.xOnBoard, t.zOnBoard);
    }

    public static Tile getTile(int xOnBoard, int zOnBoard) {
        for (int i = 0; i < tiles.Count; i++) {
            if (tiles[i].xOnBoard == xOnBoard && tiles[i].zOnBoard == zOnBoard)
                return tiles[i];
        }
        return null;
    }

}
