using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class Figure : MonoBehaviour {

    //STATIC

    private static Figure selectedFigure;

    public static void setSelectedFigure(Figure figure) {
        if (selectedFigure != null)
            selectedFigure.setSelected(false);
        if (figure != null)
            figure.setSelected(true);
        selectedFigure = figure;
    }

    public static Figure getSelectedFigure() {
        return selectedFigure;
    }

    public enum GameColor {
        Black,
        White
    }

    public enum Type {
        Queen,
        King,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    // Obj

    // CONST
    private const float animSpeed = 150f;
    private const float totalAnimatedHeight = 50f;

    // public 
    public GameColor color;
    public Type type;
    public int xOnBoard, zOnBoard;
    public int zDirection = -1; // nur für Pawn

    // private 
    private bool doAnimate = false;
    private bool hasMoved = false;
    private Vector3 targetToMove, animatedTarget1, animatedTarget2; // animatedTarget1 = moveFigureUP, animatedTarget2 = moveFigureOver, targetToMove = targetTilePos

    private MeshRenderer mesh;
    private Material standardColor;
    private GameManager gm;

    // only for knight
    private bool hasAnimatedUp = false, hasAnimatedOver = false;

    void Start() {
        GameManager.figs.Add(this);

        mesh = gameObject.GetComponent<MeshRenderer>();

        GameObject obj = GameObject.FindGameObjectWithTag("GameController");
        gm = obj.GetComponent<GameManager>();

        if (color == GameColor.White) {
            mesh.material = gm.whiteMaterial;
        } else {
            mesh.material = gm.blackMaterial;
        }
        standardColor = mesh.material;

    }

    private Vector3 target, moveDir;
    void Update() {

        if (doAnimate == true) {
            switch (type) {
                case Type.Pawn:
                case Type.Rook: {
                        Vector3 moveDir = targetToMove - transform.parent.position;
                        if (moveDir.magnitude > animSpeed * Time.deltaTime) {
                            transform.parent.Translate(moveDir.normalized * animSpeed * Time.deltaTime);
                        } else {
                            transform.parent.position = targetToMove;
                            doAnimate = false;
                        }
                    }
                    break;
                default:
                    if (!hasAnimatedUp && !hasAnimatedOver) { // Move up
                        Vector3 moveDir = animatedTarget1 - transform.parent.position;
                        if (moveDir.magnitude > animSpeed * Time.deltaTime) {
                            transform.parent.Translate(moveDir.normalized * animSpeed * Time.deltaTime);
                        } else {
                            transform.parent.position = animatedTarget1;
                            hasAnimatedUp = true;
                        }
                    } else if (hasAnimatedUp && !hasAnimatedOver) { // move above tile
                        Vector3 moveDir = animatedTarget2 - transform.parent.position;
                        if (moveDir.magnitude > animSpeed * Time.deltaTime) {
                            transform.parent.Translate(moveDir.normalized * animSpeed * Time.deltaTime);
                        } else {
                            transform.parent.position = animatedTarget2;
                            hasAnimatedOver = true;
                        }
                    } else { // move down
                        Vector3 moveDir = targetToMove - transform.parent.position;
                        if (moveDir.magnitude > animSpeed * Time.deltaTime) {
                            transform.parent.Translate(moveDir.normalized * animSpeed * Time.deltaTime);
                        } else {
                            transform.parent.position = targetToMove;
                            doAnimate = false;
                            hasAnimatedOver = false;
                            hasAnimatedUp = false;
                        }
                    }
                    break;
            }
        }
    }

    public void setSelected(bool b) {
        if (b) {
            mesh.material = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().selectedMaterial;
        } else {
            mesh.material = standardColor;
        }
    }

    private void OnMouseDown() {
        Debug.Log("Clicked: Figure(" + xOnBoard + "," + zOnBoard + ")");
        GameManager.clickedFigure(this);
    }

    public void moveObjectToTile(Tile t) {
        if (canMove(t)) {
            if (type == Type.King && isCastling(this, t)) {
                Figure rook = null;
                if (t.zOnBoard == 0) {
                    if (t.xOnBoard == 1) {
                        rook = GameManager.getFigure(0, 0);
                        rook.moveObjectToTile(GameManager.getTile(2, 0));
                    } else if (t.xOnBoard == 5) {
                        rook = GameManager.getFigure(7, 0);
                        rook.moveObjectToTile(GameManager.getTile(4, 0));
                    }
                } else if (t.zOnBoard == 7) {
                    if (t.xOnBoard == 1) {
                        rook = GameManager.getFigure(0, 7);
                        rook.moveObjectToTile(GameManager.getTile(2, 7));
                    } else if (t.xOnBoard == 5) {
                        rook = GameManager.getFigure(7, 7);
                        rook.moveObjectToTile(GameManager.getTile(4, 7));
                    }
                }
            }

            hasMoved = true;

            Figure figOnTile = GameManager.getFigure(t.xOnBoard, t.zOnBoard);
            if (figOnTile != null) {
                figOnTile.kill(this);
            }

            xOnBoard = t.xOnBoard;
            zOnBoard = t.zOnBoard;
            animatedTarget1 = new Vector3(transform.position.x, totalAnimatedHeight, transform.position.z);
            animatedTarget2 = new Vector3(t.transform.position.x, totalAnimatedHeight, t.transform.position.z);

            Tile target = GameManager.getTile(t.xOnBoard, t.zOnBoard);

            targetToMove = new Vector3(t.transform.position.x, (float)(target.transform.position.y + target.heightInWord / 2), t.transform.position.z);
            doAnimate = true;
        }
    }

    public void kill(Figure killer) {
        GameObject go = Instantiate(gm.killEffect);
        KillEffectTrigger ke = go.GetComponent<KillEffectTrigger>();
        ke.init(killer, this);
        Debug.Log("asdf");
    }

    public void destroy() {
        Destroy(gameObject);
    }

    public bool canMove(Tile tile) {

        if (tile == null)
            return false;

        if (xOnBoard == tile.xOnBoard && zOnBoard == tile.zOnBoard) {
            return false;
        }

        switch (type) {
            case Type.King:
                Debug.Log("King");
                return canKingMove(this, tile) || isCastling(this, tile);
            case Type.Pawn:
                Debug.Log("Pawn");
                return canPawnMove(this, tile);
            case Type.Bishop:
                Debug.Log("Bishop");
                return canBishopMove(this, tile);
            case Type.Rook:
                Debug.Log("Rook");
                return canRookMove(this, tile);
            case Type.Queen:
                Debug.Log("Queen");
                return canRookMove(this, tile) || canBishopMove(this, tile);
            case Type.Knight:
                Debug.Log("Knight");
                return canKnightMove(this, tile);
            default:
                Debug.LogError("NO TYPE DEFINED!");
                break;
        }
        return false;
    }

    private static bool canBishopMove(Figure f, Tile t) {
        List<Tile> moveableTiles = new List<Tile>();

        // Digonal Unten-Rechts
        for (int i = 1; i <= 7; i++) {
            Figure temp = GameManager.getFigure(f.xOnBoard + i, f.zOnBoard + i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard + i, f.zOnBoard + i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard + i, f.zOnBoard + i));
                break;
            } else {
                break;
            }
        }
        // Digonal Unten-Links
        for (int i = 1; i <= 7; i++) {
            Figure temp = GameManager.getFigure(f.xOnBoard - i, f.zOnBoard + i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard - i, f.zOnBoard + i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard - i, f.zOnBoard + i));
                break;
            } else {
                break;
            }
        }
        // Digonal Oben-Rechts
        for (int i = 1; i <= 7; i++) {
            Figure temp = GameManager.getFigure(f.xOnBoard + i, f.zOnBoard - i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard + i, f.zOnBoard - i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard + i, f.zOnBoard - i));
                break;
            } else {
                break;
            }
        }
        // Digonal Oben-Links
        for (int i = 1; i <= 7; i++) {
            Figure temp = GameManager.getFigure(f.xOnBoard - i, f.zOnBoard - i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard - i, f.zOnBoard - i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard - i, f.zOnBoard - i));
                break;
            } else {
                break;
            }
        }

        for (int i = 0; i < moveableTiles.Count; i++) {
            try {
                Debug.Log(t.gameObject.transform.parent.position + " " + moveableTiles[i].transform.position);
                if (moveableTiles[i] == t)
                    return true;
            } catch (Exception e) {

            }
        }
        return false;
    }

    private static bool canRookMove(Figure f, Tile t) {
        List<Tile> moveableTiles = new List<Tile>();
        for (int i = f.xOnBoard + 1; i < 8; i++) {
            Figure temp = GameManager.getFigure(i, f.zOnBoard);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(i, f.zOnBoard));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(i, f.zOnBoard));
                break;
            } else {
                break;
            }
        }
        for (int i = f.xOnBoard - 1; i >= 0; i--) {
            Figure temp = GameManager.getFigure(i, f.zOnBoard);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(i, f.zOnBoard));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(i, f.zOnBoard));
                break;
            } else {
                break;
            }
        }
        for (int i = f.zOnBoard + 1; i < 8; i++) {
            Figure temp = GameManager.getFigure(f.xOnBoard, i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard, i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard, i));
                break;
            } else {
                break;
            }
        }
        for (int i = f.zOnBoard - 1; i >= 0; i--) {
            Figure temp = GameManager.getFigure(f.xOnBoard, i);
            if (temp == null) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard, i));
            } else if (temp.color != f.color) {
                moveableTiles.Add(GameManager.getTile(f.xOnBoard, i));
                break;
            } else {
                break;
            }
        }

        for (int i = 0; i < moveableTiles.Count; i++) {
            Debug.Log(moveableTiles[i].xOnBoard + " " + moveableTiles[i].zOnBoard);
            if (moveableTiles[i] == t)
                return true;
        }
        return false;
    }

    private static bool canKnightMove(Figure f, Tile t) {
        List<Tile> moveableTiles = new List<Tile>();

        moveableTiles.Add(GameManager.getTile(f.xOnBoard - 2, f.zOnBoard - 1));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard - 1, f.zOnBoard - 2));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard + 2, f.zOnBoard - 1));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard + 1, f.zOnBoard - 2));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard - 2, f.zOnBoard + 1));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard - 1, f.zOnBoard + 2));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard + 2, f.zOnBoard + 1));
        moveableTiles.Add(GameManager.getTile(f.xOnBoard + 1, f.zOnBoard + 2));

        for (int i = 0; i < moveableTiles.Count; i++) {
            if (moveableTiles[i] == null)
                continue;
            if (moveableTiles[i] == t)
                return true;
        }
        return false;
    }

    private static bool canPawnMove(Figure f, Tile tile) {
        if (!f.hasMoved) {
            if (f.xOnBoard == tile.xOnBoard) {
                if (tile.zOnBoard == f.zOnBoard + (f.zDirection > 0 ? 1 : -1) * 2 && GameManager.getFigure(tile.xOnBoard, tile.zOnBoard) == null)
                    return true;
            }
        }

        if (tile.xOnBoard == f.xOnBoard) {
            if (tile.zOnBoard == f.zOnBoard + (f.zDirection > 0 ? 1 : -1)) {
                Figure fig = GameManager.getFigure(f.xOnBoard, tile.zOnBoard);
                return fig == null;
            }
        }
        if (tile.xOnBoard == f.xOnBoard - 1) {
            if (tile.zOnBoard == f.zOnBoard + (f.zDirection > 0 ? 1 : -1)) {
                Figure fig = GameManager.getFigure(f.xOnBoard - 1, tile.zOnBoard);
                if (fig != null) {
                    return fig.color != f.color;
                }
            }
        }
        if (tile.xOnBoard == f.xOnBoard + 1) {
            if (tile.zOnBoard == f.zOnBoard + (f.zDirection > 0 ? 1 : -1)) {
                Figure fig = GameManager.getFigure(f.xOnBoard + 1, tile.zOnBoard);
                if (fig != null) {
                    return fig.color != f.color;
                }
            }
        }
        return false;
    }

    public static bool isCastling(Figure f, Tile tile) {
        if (!f.hasMoved) {
            Figure rook = null;
            if (tile.zOnBoard == 0) {
                if (tile.xOnBoard == 1) {
                    rook = GameManager.getFigure(0, 0);
                } else if (tile.xOnBoard == 5) {
                    rook = GameManager.getFigure(7, 0);
                }
            } else if (tile.zOnBoard == 7) {
                if (tile.xOnBoard == 1) {
                    rook = GameManager.getFigure(0, 7);
                } else if (tile.xOnBoard == 5) {
                    rook = GameManager.getFigure(7, 7);
                }
            }

            if (rook != null && rook.color == f.color && rook.type == Type.Rook && !rook.hasMoved) {
                return true;
            }
        }
        return false;
    }

    private static bool canKingMove(Figure f, Tile tile) {

        for (int x = f.xOnBoard - 1; x <= f.xOnBoard + 1; x++) {
            for (int z = f.zOnBoard - 1; z <= f.zOnBoard + 1; z++) {
                if (tile.xOnBoard == x && tile.zOnBoard == z) {
                    Figure fig = GameManager.getFigure(x, z);
                    if (fig != null) {
                        return fig.color != f.color; // kann nicht eigenen Teammate schmeißen!
                    } else
                        return true;
                }
            }
        }
        return false;
    }

    public static List<Figure> getPossibleTiles(Figure f) {

        //TODO

        return null;
    }

}
