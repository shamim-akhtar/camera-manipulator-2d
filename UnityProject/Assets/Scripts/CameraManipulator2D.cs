using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraManipulator2D : MonoBehaviour
{
  public float CameraSizeMin = 1.0f;
  public float CameraSizeMax = 10.0f;

  public static bool IsCameraPanning
  {
    get;
    set;
  } = true;

  public Slider mSliderZoom;

  private Vector3 mDragPos;
  private Vector3 mOriginalPosition;

  private float mZoomFactor = 0.0f;
  private Camera mCamera;
  private bool mDragging = false;

  void Start()
  {
    SetCamera(Camera.main);
    if(CameraSizeMax < CameraSizeMin)
    {
      float tmp = CameraSizeMax;
      CameraSizeMax = CameraSizeMin;
      CameraSizeMin = tmp;
    }

    if(CameraSizeMax - CameraSizeMin < 0.01f)
    {
      CameraSizeMax += 0.1f;
    }
  }

  public void SetCamera(Camera camera)
  {
    mCamera = camera;
    mOriginalPosition = mCamera.transform.position;

    mZoomFactor = 
      (CameraSizeMax - mCamera.orthographicSize) / 
      (CameraSizeMax - CameraSizeMin);

    if (mSliderZoom)
    {
      mSliderZoom.value = mZoomFactor;
    }
  }

  void Update()
  {
    // Camera panning is disabled when a tile is selected.
    if (!IsCameraPanning)
    {
      mDragging = false;
      return;
    }

    // We also check if the pointer is not on UI item
    // or is disabled.
    if (EventSystem.current.IsPointerOverGameObject() || 
      enabled == false)
    {
      //mDragging = false;
      return;
    }

    // Save the position in worldspace.
    if (Input.GetMouseButtonDown(0))
    {
      mDragPos = mCamera.ScreenToWorldPoint(
        Input.mousePosition);
      mDragging = true;
    }

    if (Input.GetMouseButton(0) && mDragging)
    {
      Vector3 diff = mDragPos - 
        mCamera.ScreenToWorldPoint(
          Input.mousePosition);
      diff.z = 0.0f;
      mCamera.transform.position += diff;
    }
    if (Input.GetMouseButtonUp(0))
    {
      mDragging = false;
    }
  }

  public void ResetCameraView()
  {
    mCamera.transform.position = mOriginalPosition;
    mCamera.orthographicSize = CameraSizeMax;
    mZoomFactor = 0.0f;
    if (mSliderZoom)
    {
      mSliderZoom.value = 0.0f;
    }
  }

  public void OnSliderValueChanged()
  {
    Zoom(mSliderZoom.value);
  }

  public void Zoom(float value)
  {
    mZoomFactor = value;
    mZoomFactor = Mathf.Clamp01(mZoomFactor);

    if(mSliderZoom)
    {
      mSliderZoom.value = mZoomFactor;
    }

    mCamera.orthographicSize = CameraSizeMax -
        mZoomFactor * 
        (CameraSizeMax - CameraSizeMin);
  }

  public void ZoomIn()
  {
    Zoom(mZoomFactor + 0.01f);
  }

  public void ZoomOut()
  {
    Zoom(mZoomFactor - 0.01f);
  }
}
