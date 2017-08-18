using UnityEngine;

public class ButtonSubscriber
{
    public UnityEngine.UI.Button button;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
		button.onClick.AddListener(() => OnButtonClick());
    }
	
	public virtual void OnButtonClick()
	{
		Debug.Log("Default OnButtonClick ");
	}	
	
	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable()
	{
		button.onClick.RemoveAllListeners();
	}

}