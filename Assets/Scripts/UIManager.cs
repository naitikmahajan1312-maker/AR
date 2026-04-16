using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public  bool isLog = false;

    public Reporter reporter;

    [Serializable]
    public class TargetPrefab
    {
        public string prefabName;   // "Apple", "Ant", "Aeroplane"
        public GameObject prefab;   // prefab to show
    }

    public TextMeshProUGUI soundTxt;

    public List<TargetPrefab> targetPrefabs = new List<TargetPrefab>();
    public Transform spawnParent; // optional (set AR content parent). can be null.

    string playAgain = "";
    GameObject spawnedObj;

    void Awake()
    {
        if (Instance == null) Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (isLog)
        {
            reporter.gameObject.SetActive(true);
        }
        else
        {
            reporter.gameObject.SetActive(false);
        }
        OnSoundBtnTap();
    }

    public void TempBtnTap(string name)
    {
        playAgain = name;

        // Spawn prefab (if mapped)
        var tp = targetPrefabs.Find(x => x.prefabName == name);
        if (tp != null && tp.prefab != null)
        {
            if (spawnedObj != null) Destroy(spawnedObj);

            spawnedObj = Instantiate(tp.prefab, spawnParent);
            spawnedObj.transform.localPosition = new Vector3(0, 0, 1);
            spawnedObj.transform.localRotation = Quaternion.identity;
        }

        // Play sound
        SoundManager.Instance.PlayByTargetName(name);
    }

    public void PlayAgainBtntap()
    {
        if (!string.IsNullOrEmpty(playAgain))
            SoundManager.Instance.PlayByTargetName(playAgain);
    }

    public void OnSoundBtnTap()
    {
        SoundManager.Instance.soundOn = !SoundManager.Instance.soundOn;

        if (soundTxt != null)
            soundTxt.text = SoundManager.Instance.soundOn ? "Sound ON" : "Sound OFF";

        // if (!SoundManager.Instance.soundOn)
        SoundManager.Instance.audioSource.mute = !SoundManager.Instance.soundOn;
    }
}
