using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int totalPoints;
    public int currPoints;

    public AreaLight areaLight;

    public GameObject cube;

    void Start()
    {
        areaLight.GetComponent<MeshRenderer>().enabled = false;
    } 
   
    public void playHitAnimation()
    {
        currPoints -= 1;

        float animTime = 0.45f;

        float newScale = (float)currPoints / (float)totalPoints;
        newScale.Remap(0f, 1f, cube.transform.localScale.x / 2, cube.transform.localScale.x);

        cube.transform.DOScaleZ(newScale, animTime).SetEase(Ease.InOutBack);
        cube.transform.DOScaleX(newScale, animTime).SetEase(Ease.InOutBack).OnComplete(() => {
            if (currPoints == 0)
            {
                gameObject.SetActive(false);
            }
        });
    }

    public void restart()
    {
        currPoints = totalPoints;
        gameObject.SetActive(true);
        cube.transform.localScale = Vector3.one;
    }

    public bool isActive()
    {
        return currPoints > 0;
    } 

}
