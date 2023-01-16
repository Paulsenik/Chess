using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    private static Tile selectedTile;

    public static void setSelectedTile(Tile t) {
        if (selectedTile != null)
            selectedTile.setSelected(false);
        if (t != null)
            t.setSelected(true);
        selectedTile = t;
    }
    public static Tile getSelectedTile() {
        return selectedTile;
    }

    public float heightInWord = 10;// Height in Units

    public Figure.GameColor color;
    public int xOnBoard, zOnBoard;

    private MeshRenderer mesh;
    public Material standardColor;

    void Start() {
        GameManager.tiles.Add(this);
        mesh = gameObject.GetComponent<MeshRenderer>();
    }
    public void setSelected(bool b) {
        if (b) {
            mesh.material = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().selectedMaterial;
        } else {
            mesh.material = standardColor;
        }
    }

    // Update is called once per frame
    void Update() {

    }
    private void OnMouseDown() {
        Debug.Log("Clicked: Tile(" + xOnBoard + "," + zOnBoard + ")");
        GameManager.clickedTile(this);
    }
}
