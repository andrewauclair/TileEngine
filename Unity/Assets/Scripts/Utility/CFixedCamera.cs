using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CFixedCamera : MonoBehaviour
{
    #region Static Data
    #endregion

    #region Public Data
	public float ScreenHeight = 0f;
	public float PixelsPerUnit = 0f;
    #endregion

    #region Private Data
	private Camera m_Camera = null;
    #endregion

    #region Unity Methods
    void Awake()
    {
		m_Camera = GetComponent<Camera>();

		if (ScreenHeight <= 0f)
		{
			Debug.LogError("Screen Height not set");
			Debug.Break();
		}

		if (PixelsPerUnit <= 0f)
		{
			Debug.LogError("Pixels per unit is not set, must be greater than zero.");
			Debug.Break();
		}

		if (m_Camera.orthographic == false)
		{
			Debug.LogError("This script is for Orthographic Camera's");
			Debug.Break();
		}

		m_Camera.orthographicSize = ScreenHeight / (PixelsPerUnit * 2);
    }
    void Start()
    {
	}
	void Update()
    {
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
