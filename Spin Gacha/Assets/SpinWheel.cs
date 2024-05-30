using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] private float speedRotation;
    private float coundownTimer;
    private bool isSpinDefault;
    private float zRotation;

    [SerializeField] private List<string> pricesValue = new List<string>();
    [SerializeField]private GameObject textPrefab;
    [SerializeField] private Transform startingArrowPoint;
    [SerializeField] private Transform priceParent;
    [SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private bool randomResult;
    private int indexResulRandomObject;
    private float originAngle;
    const float angleOffSet = 15;
    const float smallestThreshold = 1.5f;

    private void Start()
    {
        InitializeTextObject();
    }

    private void Update()
    {
        if (isSpinDefault)
        {
            coundownTimer -= Time.deltaTime;
            if (coundownTimer <= smallestThreshold && randomResult)
            {
                SpinToTarget();
                isSpinDefault = false;
                randomResult = false;
                return;
            }

            if (coundownTimer <= 0)
            {
                isSpinDefault = false;
                rewardText.text = "Your Reward : " + GetPrice();
                return;
            }
            zRotation = speedRotation * coundownTimer * Time.deltaTime;
            transform.Rotate(0, 0, -zRotation);
        }
    }

    public void StartSpin()
    {
        RandomResultBeforeSpin();
        coundownTimer = Random.Range(4, 7);
        isSpinDefault = true;
        rewardText.text = "";
        originAngle = transform.eulerAngles.z + angleOffSet;
    }

    private void InitializeTextObject()
    {
        int priceOffSet = (360 / pricesValue.Count) / 2;
        for(int i=0;i< pricesValue.Count; i++)
        {
            int angle = i * (360 / pricesValue.Count) + priceOffSet;
            float angleRad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleRad) * 2;
            float y = Mathf.Sin(angleRad) * 2;

            GameObject newText = Instantiate(textPrefab, new Vector3(x,y, 0),Quaternion.identity, priceParent.transform);
            newText.transform.rotation = Quaternion.Euler(0, 0, angle);
            newText.GetComponent<TextMeshProUGUI>().text = pricesValue[i];
        }
    }

    private string GetPrice()
    {
        float priceClosest = Mathf.Infinity;
        string priceName = "";
        for(int i=0; i< priceParent.childCount; i++)
        {
            Transform priceChild = priceParent.GetChild(i).gameObject.transform;
            float distanceToPrice = Vector2.Distance(startingArrowPoint.position, priceChild.position);
            if(distanceToPrice < priceClosest)
            {
                priceClosest = distanceToPrice;
                priceName = priceChild.GetComponent<TextMeshProUGUI>().text;
            }
        }
        return priceName;
    }

    private void RandomResultBeforeSpin()
    {
        randomResult = true;
        indexResulRandomObject = Random.Range(0, pricesValue.Count);
        Debug.Log("Random result before spin : "+ pricesValue[indexResulRandomObject]);
        priceParent.GetChild(indexResulRandomObject).GetComponent<TextMeshProUGUI>().color = Color.red;
        Debug.Log("Index = " + indexResulRandomObject);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 60);
       
    }
    private void SpinToTarget()
    {
        float currentAngle = transform.eulerAngles.z;
        float angleToSpin = indexResulRandomObject * 360/ priceParent.childCount;
        while(angleToSpin < 0 || angleToSpin > 360)
        {
            if(angleToSpin < 0)
            {
                angleToSpin += 360;
            }

            if(angleToSpin > 360)
            {
                angleToSpin -= 360;
            }
        }  
        Debug.Log("Angle To Spin"+ angleToSpin);
        StartCoroutine(SpinRoutine(currentAngle, originAngle - angleToSpin));
    }

    private IEnumerator SpinRoutine(float currentAngle,float angleToSpin)
    {
        float elapsedTime = 0f;
        while (elapsedTime < smallestThreshold)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / smallestThreshold);
            float angle = Mathf.Lerp(currentAngle, angleToSpin, t);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, angle);
            yield return null;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angleToSpin);
        rewardText.text = "Your Reward : " + GetPrice();
    }

}
