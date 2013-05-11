using UnityEngine;
using System.Collections;

public class Positions : MonoBehaviour {

	// Position Variables
    // Screen and Movement boundaries
    public static float leftBorder;
    public static float rightBorder;
    public static float topBorder;
    public static float bottomBorder;
    public static float rightMovementLimit;
    private static float boundProportion = 0.4f;

    public static float baseZ = 15;
    public static float generationOffset = 5;
    //private static float boundProportion = 1f;


    // Called on instantiation of the level prefab to set up the correct positions
    public void updatePositions() {
        // Set up screen boundaries
        // Fix "action" plane at a certain distance away from the camera

        if (Network.isClient) {
            leftBorder = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0, 0, baseZ)).x;
            rightBorder = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1, 0, baseZ)).x;
            rightMovementLimit = Camera.mainCamera.ViewportToWorldPoint(new Vector3(boundProportion, 0, baseZ)).x;
            topBorder = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0, 1, baseZ)).y;
            bottomBorder = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0, 0, baseZ)).y;
            networkView.RPC("sentPositions", RPCMode.Server, leftBorder, rightBorder, rightMovementLimit, topBorder, bottomBorder);
        }
        if (Network.isServer) {
            print("RPC server ");
        }
    }

    [RPC]
    public void sentPositions(float updateLeftBorder, float updateRightborder, float updateRightMovementLimit, float updateTopBorder, float updateBottomBorder) {
        leftBorder = updateLeftBorder;
        rightBorder = updateRightborder;
        rightMovementLimit = updateRightMovementLimit;
        topBorder = updateTopBorder;
        bottomBorder = updateBottomBorder;
        print("Server border " + leftBorder);
    }

}
