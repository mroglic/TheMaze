
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody rb;
    public Vector3 direction = Vector3.forward;
    public Vector3 forceDirection;
    public float speed;
    public float rotationAmount;
    public float rotationSpeed;
    public float maxSpeed = 10;
    public Vector3 force;
    private float z;
    private float x;

    [Header("Light")]
    public GameObject pointLight;
    public float lightFollowSpeed = 10;
    public float lightHeight = 1.2f;

    [Header("Camera")]
    public GameObject cam;
    public float cameraFollowSpeed = 1f;
    public float cameraHeight = 1.3f;
    public float cameraDistance = 2f;
    
    [Header("Score")]
    public int totalPoints;
    public int positivePoints;
    public int negativePoints;

    [Header("Gameplay Manager")]
    public GameplayManager gameplayManager;

    [Header("GUI")]
    public RectTransform pointsAnimPanelPrefab;
    public Canvas canvas;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        direction = Vector3.forward;
    }  

    void FixedUpdate()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if (gameplayManager.cameraViewType == GameplayManager.CameraViewType.Top)
        {  
            Vector3 force = new Vector3(x, 0f, z) * speed;

            if (force.magnitude > 0)
            {
                forceDirection = force.normalized;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(forceDirection * 0.7f, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(force);
            }
        }
        else if (gameplayManager.cameraViewType == GameplayManager.CameraViewType.Player)
        {
            x *= rotationAmount;
            direction = Quaternion.Euler(0, x, 0) * direction;
            
            force = direction * speed * z;

            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(force);
            }

            Debug.DrawRay(transform.position, direction);

            // player cam
            cam.transform.position = Vector3.Lerp(cam.transform.position, transform.position - direction * cameraDistance + Vector3.up * cameraHeight, Time.deltaTime * cameraFollowSpeed);            
        }  

        // light follow player
        pointLight.transform.position = Vector3.Lerp(pointLight.transform.position, transform.position + direction*5 + Vector3.up * transform.localScale.x * lightHeight, Time.deltaTime * lightFollowSpeed);
    } 

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {   
            // increase points
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                // get obstacle
                Obstacle obstacle = collision.gameObject.GetComponentInParent<Obstacle>();

                // player can accidentally collide with obstacle which is playing fade away final animation, but we don't want to count that hit
                if (obstacle.isActive())
                {
                    // repel player from obstacle
                    rb.angularVelocity = Vector3.zero;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(-forceDirection * 1.7f, ForceMode.Impulse);

                    // update points
                    positivePoints += gameplayManager.pointsIncrement;
                    totalPoints += gameplayManager.pointsIncrement;

                    // play points animation
                    playPointsAnimation(gameplayManager.pointsIncrement, "+");

                    // play obstacle animation
                    obstacle.playHitAnimation();
                }                 
            }
            // decrease points
            else if (collision.gameObject.CompareTag("Wall"))
            {
                // update points                
                negativePoints += gameplayManager.pointsDecrement;
                totalPoints -= gameplayManager.pointsDecrement;

                // play points animation
                playPointsAnimation(gameplayManager.pointsDecrement, "-");
            }
        }
    }    

    void playPointsAnimation(int points, string sign)
    {
        float animTime = 2f;
        float panelYStartPos;
        float panelYEndPos;
        
        RectTransform pointsAnimPanel = Instantiate<RectTransform>(pointsAnimPanelPrefab, canvas.transform);

        if (sign.Equals("-"))
        {
            // negative poinst move down
            panelYStartPos = -450f;
            panelYEndPos = -800f;
            pointsAnimPanel.DOShakeScale(animTime, 0.75f, 2, 90);
            pointsAnimPanel.GetComponent<Image>().color = Color.black;
        }
        else
        {
            // positive poinst move up
            panelYStartPos = -320f;
            panelYEndPos = -70f;
            pointsAnimPanel.DOScale(2f, animTime);
        }
        
        // move to end pos
        pointsAnimPanel.anchoredPosition = new Vector2(pointsAnimPanel.anchoredPosition.x, panelYStartPos); 
        pointsAnimPanel.DOAnchorPosY(panelYEndPos, animTime).SetEase(Ease.InOutBack);

        // fade out
        CanvasGroup pointsAnimPanelCanvasGroup = pointsAnimPanel.GetComponent<CanvasGroup>();
        pointsAnimPanelCanvasGroup.alpha = 1f;
        pointsAnimPanelCanvasGroup.DOFade(0, animTime).OnComplete(() => Destroy(pointsAnimPanel.gameObject));
        
        // set points text
        pointsAnimPanel.GetComponent<PointsAnimPanel>().pointsTxt.text = sign + points;
    } 
} 