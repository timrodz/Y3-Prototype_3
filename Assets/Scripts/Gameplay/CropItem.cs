using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropItem : MonoBehaviour {

    [Header("Variables")]
    [SerializeField]
    private GameObject CropMesh;
    [SerializeField]
    private float RestoreTimer = 60.0f;
    [SerializeField]
    private bool CropHarvested = false;
    [SerializeField]
    private ItemPickUp ItemScript;

    private AudioManager audioManager;


    private float m_timer;

    private void Awake()
    {
        if(CropMesh)
        {
            if(CropHarvested)
            {
                CropMesh.SetActive(false);
            }
        }
    }

    // Use this for initialization
    void Start () {

        audioManager = FindObjectOfType<AudioManager>();
    }
	
	// Update is called once per frame
	void Update () {
		if(CropHarvested)
        {
            m_timer += Time.deltaTime;
            if(m_timer >= RestoreTimer)
            {
                RestoreCrop();
            }
        }
	}

    public void HarvestCrop()
    {
        audioManager.Play("Carrot_Harvest");

        CropMesh.SetActive(false);
        CropHarvested = true;
        if (ItemScript)
            ItemScript.enabled = false;
    }

    private void RestoreCrop()
    {
        CropMesh.SetActive(true);
        CropHarvested = false;
        m_timer = 0.0f;
        if (ItemScript)
            ItemScript.enabled = true;
    }
}
