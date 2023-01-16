using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject neutralCameraAnlge, blackCameraAngle, whiteCameraAngle, topDownCameraAngle;

    public enum CameraAngle {
        TopDown,
        Neutral,
        Black,
        White
    }

    private CameraAngle currentAngle;

    public void setAngle(CameraAngle angle) {
        switch (angle) {
            case CameraAngle.TopDown:
                this.transform.position = topDownCameraAngle.transform.position;
                this.transform.rotation = topDownCameraAngle.transform.rotation;
                break;
            case CameraAngle.Neutral:
                this.transform.position = neutralCameraAnlge.transform.position;
                this.transform.rotation = neutralCameraAnlge.transform.rotation;
                break;
            case CameraAngle.Black:
                this.transform.position = blackCameraAngle.transform.position;
                this.transform.rotation = blackCameraAngle.transform.rotation;
                break;
            case CameraAngle.White:
                this.transform.position = whiteCameraAngle.transform.position;
                this.transform.rotation = whiteCameraAngle.transform.rotation;
                break;
            default:
                Debug.LogError("No parameters set!!!");
                break;
        }
        currentAngle = angle;
    }

    public void cycleCamera() {
        switch (currentAngle) {
            case CameraAngle.TopDown:
                setAngle(CameraAngle.Neutral);
                break;
            case CameraAngle.Neutral:
                setAngle(CameraAngle.White);
                break;
            case CameraAngle.White:
                setAngle(CameraAngle.Black);
                break;
            case CameraAngle.Black:
                setAngle(CameraAngle.TopDown);
                break;
            default:
                Debug.LogError("No parameters set!!!");
                break;
        }
    }
}
