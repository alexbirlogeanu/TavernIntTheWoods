using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScrollComponent : MonoBehaviour {
    [Range(0f, 0.5f)]
    [Tooltip("Defines the area (in percent) from the edges of the screen where if the mouse pointer enters camera will rotate")]
    public float TopScrollAreaPercent = 0.1f;
    [Range(0f, 0.5f)]
    [Tooltip("Defines the area (in percent) from the edges of the screen where if the mouse pointer enters camera will rotate")]
    public float BotScrollAreaPercent = 0.1f;
    [Range(0f, 0.5f)]
    [Tooltip("Defines the area (in percent) from the edges of the screen where if the mouse pointer enters camera will rotate")]
    public float LeftScrollAreaPercent = 0.1f;
    [Range(0f, 0.5f)]
    [Tooltip("Defines the area (in percent) from the edges of the screen where if the mouse pointer enters camera will rotate")]
    public float RightScrollAreaPercent = 0.1f;
    [Tooltip("Rotation (in degrees) for the top and bottom areas relative to the initial camera rotation. Rotation is applied to the X axe")]
    public float TopBottomRotateAmount = 10f;
    [Tooltip("Rotation (in degrees) for the left and right areas relative to the initial camera rotation.Rotation is applied to the Y axe")]
    public float LeftRightRoateAmount = 10f;

    public float ScrollSpeed = 5f;
    //private:
    private struct ScrollArea
    {
        public Rect Area;
        public Vector2 ScrollAmount;
    };

    private Vector3 InitialRotation;
    private ScrollArea[] ScrollAreas = new ScrollArea[4];

    void Start ()
    {
        InitialRotation = transform.rotation.eulerAngles;

        //bottom part of the screen
        ScrollAreas[0].Area = Rect.MinMaxRect(0f, 0f, (float)Screen.width, BotScrollAreaPercent * (float)Screen.height);
        ScrollAreas[0].ScrollAmount = new Vector2(TopBottomRotateAmount, 0f); //scroll downwards

        //top part of the screen
        ScrollAreas[1].Area = Rect.MinMaxRect(0, (1f - TopScrollAreaPercent) *(float)Screen.height, (float)Screen.width, (float)Screen.height);
        ScrollAreas[1].ScrollAmount = new Vector2(-TopBottomRotateAmount, 0f); //scroll upwards

        //left part of the screen
        ScrollAreas[2].Area = Rect.MinMaxRect(0f, 0f, (float)Screen.width * LeftScrollAreaPercent, Screen.height);
        ScrollAreas[2].ScrollAmount = new Vector2(0f, -LeftRightRoateAmount);

        //right part of the screen
        ScrollAreas[3].Area = Rect.MinMaxRect((1f - RightScrollAreaPercent) * (float)Screen.width, 0f, Screen.width, Screen.height);
        ScrollAreas[3].ScrollAmount = new Vector2(0f, LeftRightRoateAmount);
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 pointerPos = Input.mousePosition;
        Vector2 xyRotationOffsets = new Vector2(0f, 0f);

        foreach (ScrollArea areas in ScrollAreas)
        {
            if (areas.Area.Contains(pointerPos))
                xyRotationOffsets += areas.ScrollAmount;
        }

        Vector3 desiredRotInEuler = InitialRotation;
        desiredRotInEuler.x += xyRotationOffsets.x;
        desiredRotInEuler.y += xyRotationOffsets.y;

        Quaternion desiredRotation = Quaternion.Euler(desiredRotInEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, ScrollSpeed * Time.deltaTime);
	}
}
