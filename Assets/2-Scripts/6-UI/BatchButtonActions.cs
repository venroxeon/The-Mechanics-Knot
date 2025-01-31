using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Batch
{
    public GameObject batchObj;

    public Image buttonImage;

    public Shadow buttonShadow;

    public TextMeshProUGUI buttonText;

    public Batch(GameObject _batch, Image _buttonImage, Shadow _buttonShadow, TextMeshProUGUI _buttonText)
    {
        batchObj = _batch;
        buttonImage = _buttonImage;
        buttonShadow = _buttonShadow;
        buttonText = _buttonText;
    }
}

public struct Level
{
    public int batchNo;
    public int curLevel;

    public Level(int _batchNo, int _curLevel)
    {
        batchNo = _batchNo;
        curLevel = _curLevel;
    }
}

public class BatchButtonActions : MonoBehaviour
{
    public static BatchButtonActions instance;

    [SerializeField] float fadeAlpha;

    [SerializeField] GameObject batchParent, alphabetParent;
    
    Batch prevBatch;

    List<Batch> listAlphabetButtons = new();
    

    void Awake()
    {
        instance = this;
    }

    void SetBatches()
    {
        GameObject batchObj, buttonObj;
        Batch alphButton;

        for (int i = 0; i < batchParent.transform.childCount; i++)
        {
            batchObj = batchParent.transform.GetChild(i).gameObject;
            buttonObj = alphabetParent.transform.GetChild(i).gameObject;
            alphButton = new Batch(batchObj, buttonObj.GetComponent<Image>(), buttonObj.GetComponent<Shadow>(), buttonObj.GetComponentInChildren<TextMeshProUGUI>());

            listAlphabetButtons.Add(alphButton);
        }
    }

    private void Start()
    {
        SetBatches();

        prevBatch = listAlphabetButtons[0];
    }

    void FadeAlpha(Batch batch)
    {
        Color tempColor;

        tempColor = batch.buttonImage.color;
        tempColor.a = fadeAlpha;
        batch.buttonImage.color = tempColor;

        tempColor = batch.buttonText.color;
        tempColor.a = fadeAlpha;
        batch.buttonText.color = tempColor;

        batch.buttonShadow.enabled = false;
    }

    void ResetAlpha(Batch batch)
    {
        Color tempColor;

        tempColor = batch.buttonImage.color;
        tempColor.a = 1;
        batch.buttonImage.color = tempColor;

        tempColor = batch.buttonText.color;
        tempColor.a = 1;
        batch.buttonText.color = tempColor;

        batch.buttonShadow.enabled = true;
    }

    public void ViewBatch(int index)
    {
        Batch batch;

        batch = listAlphabetButtons[index - 1];

        if (batch.batchObj.activeSelf) return;

        batch.batchObj.SetActive(true);
        ResetAlpha(batch);

        prevBatch.batchObj.SetActive(false);

        FadeAlpha(prevBatch);

        prevBatch = batch;
    }
}
