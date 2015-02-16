using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class CChunkEditorGen : MonoBehaviour
{
    #region Static Data
	public static CChunkEditorGen Instance = null;
    #endregion

    #region Public Data
    #endregion

    #region Private Data
    #endregion

    #region Unity Methods
    void Awake()
    {
		Instance = this;
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
#endif