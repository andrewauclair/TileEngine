using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSingleton<T> : MonoBehaviour
{
    #region Static Data
	
    #endregion

    #region Public Data
    #endregion

    #region Private Data
	protected static T m_instance = default(T);
	private static bool ms_fAwake = false;
    #endregion

    #region Unity Methods
    void Awake()
    {
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
