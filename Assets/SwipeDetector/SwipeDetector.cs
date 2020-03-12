using System;
using UnityEngine;
using TMPro;

public class SwipeDetector : MonoBehaviour
{

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showScreenRect;
    [SerializeField] private float screenRectY;

    public int allowedTouches = 2;

    public static Rect screenRect;

    private bool[] swipeActive;
    private Vector2[] touchCurrentPositions;
    private Vector2[] touchStartPositions;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 20f;

    private Touch[] frameTouches;

    public delegate void SwipeDetectedDelegate(SwipeData swipeData);
    public static event SwipeDetectedDelegate OnSwipeDetected;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        frameTouches = new Touch[allowedTouches];
        touchCurrentPositions = new Vector2[allowedTouches];
        touchStartPositions = new Vector2[allowedTouches];
        swipeActive = new bool[allowedTouches];
        for(int i = 0; i < allowedTouches; i++){
            swipeActive[i] = false;
        }

        SwipeDetector.screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
        if(SwipeDetector.screenRect != null && showScreenRect){
            SwipeDetector.screenRect.y = screenRectY;
            GUI.Box(SwipeDetector.screenRect, "SCREEN RECT");
        }
    }

    private void Update()
    {
        frameTouches = Input.touches;
        for (int i = 0; i < Input.touchCount && i < allowedTouches; i++)
        {
            if (frameTouches[i].phase == TouchPhase.Began)
            {
                touchStartPositions[i] = frameTouches[i].position;
                touchCurrentPositions[i] = frameTouches[i].position;
            }

            if (!detectSwipeOnlyAfterRelease && frameTouches[i].phase == TouchPhase.Moved)
            {
                touchCurrentPositions[i] = frameTouches[i].position;
                DetectSwipe(i);
            }

            if (frameTouches[i].phase == TouchPhase.Ended)
            {
                touchCurrentPositions[i] = frameTouches[i].position;
                DetectSwipeEnded(i);
            }
        }
    }

    private void DetectSwipe(int touchIndex)
    {
        if (SwipeDistanceCheckMet(touchIndex) || swipeActive[touchIndex])
        {
            swipeActive[touchIndex] = true;
            if (IsVerticalSwipe(touchIndex))
            {
                var direction = touchCurrentPositions[touchIndex].y - touchStartPositions[touchIndex].y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction, touchIndex);
            }
            else
            {
                var direction = touchCurrentPositions[touchIndex].x - touchStartPositions[touchIndex].x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction, touchIndex);
            }
            // touchStartPositions[touchIndex] = touchCurrentPositions[touchIndex];
        }
    }

    private void DetectSwipeEnded(int touchIndex)
    {
        // Debug.Log("Detect swipe ended");
        if (SwipeDistanceCheckMet(touchIndex) || swipeActive[touchIndex])
        {
            swipeActive[touchIndex] = true;
            if (IsVerticalSwipe(touchIndex))
            {
                var direction = touchCurrentPositions[touchIndex].y - touchStartPositions[touchIndex].y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipeEnded(direction, touchIndex);
            }
            else
            {
                var direction = touchCurrentPositions[touchIndex].x - touchStartPositions[touchIndex].x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipeEnded(direction, touchIndex);
            }
            // touchStartPositions[touchIndex] = touchCurrentPositions[touchIndex];
        }
    }

    private bool IsVerticalSwipe(int touchIndex)
    {
        return VerticalMovementDistance(touchIndex) > HorizontalMovementDistance(touchIndex);
    }

    private bool SwipeDistanceCheckMet(int touchIndex)
    {
        return VerticalMovementDistance(touchIndex) > minDistanceForSwipe || HorizontalMovementDistance(touchIndex) > minDistanceForSwipe;
    }

    private float VerticalMovementDistance(int touchIndex)
    {
        return Mathf.Abs(touchCurrentPositions[touchIndex].y - touchStartPositions[touchIndex].y);
    }

    private float HorizontalMovementDistance(int touchIndex)
    {
        return Mathf.Abs(touchCurrentPositions[touchIndex].x - touchStartPositions[touchIndex].x);
    }

    private void SendSwipe(SwipeDirection direction, int touchIndex)
    {
        SwipeData swipeData = new SwipeData()
        {
            TouchIndex = touchIndex,
            Direction = direction,
            StartPosition = touchStartPositions[touchIndex],
            EndPosition = touchCurrentPositions[touchIndex],
            Distance = touchCurrentPositions[touchIndex] - touchStartPositions[touchIndex],
            TouchPhase = frameTouches[touchIndex].phase
        };
        // Debug.Log(swipeData.StartPosition.x + " -> " + swipeData.EndPosition.x);
        if(OnSwipeDetected != null)
            OnSwipeDetected(swipeData);

        swipeActive[touchIndex] = false;

        if(debugText){
            debugText.text = swipeData.StartPosition + "\n" + swipeData.EndPosition + "\n" + swipeData.Distance;
        }
    }

    private void SendSwipeEnded(SwipeDirection direction, int touchIndex)
    {
        SwipeData swipeData = new SwipeData()
        {
            TouchIndex = touchIndex,
            Direction = direction,
            StartPosition = touchStartPositions[touchIndex],
            EndPosition = touchCurrentPositions[touchIndex],
            Distance = touchCurrentPositions[touchIndex] - touchStartPositions[touchIndex],
            TouchPhase = TouchPhase.Ended
        };
        // Debug.Log(swipeData.StartPosition.x + " -> " + swipeData.EndPosition.x);
        if(OnSwipeDetected != null)
            OnSwipeDetected(swipeData);
        
        swipeActive[touchIndex] = false;

        if(debugText){
            debugText.text = swipeData.StartPosition + "\n" + swipeData.EndPosition + "\n" + swipeData.Distance;
        }
    }
}

public struct SwipeData
{
    public int TouchIndex;
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Vector2 Distance;
    public SwipeDirection Direction;
    public TouchPhase TouchPhase;
}

public enum SwipeDirection
{
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}