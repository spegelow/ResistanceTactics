using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TMP_Text damageText;
    public float lifespan = 1.5f;
    public Vector3 movementDirection = new Vector3(0,1,0);
    public float movementSpeed = 1;

    float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > lifespan)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //Move the text along its path
            this.transform.position += movementDirection.normalized * movementSpeed * Time.deltaTime;
        }
    }

    public void InitializeFloatingText(string text, Color color)
    {
        damageText.text = "" + text;
        damageText.color = color;
    }
}
