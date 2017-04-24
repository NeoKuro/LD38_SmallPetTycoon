using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public float maxAlpha = 100.0f;
    public float minAlpha = 30.0f;
    public float speed = 125.0f;
    public bool showOutline = true;


    private float alpha = 30.0f;
    private List<MeshRenderer> mat = new List<MeshRenderer>();
    private GameObject glowBox;
    private Vector3 initPos;
    private Vector3 initRot;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("GlowBox"))
            {
                glowBox = transform.GetChild(i).gameObject;
            }
        }

        if (glowBox == null)
        {
            Debug.Log("GlowBox not found");
            return;
        }

        for (int i = 0; i < glowBox.transform.childCount; i++)
        {
            if (glowBox.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                mat.Add(glowBox.transform.GetChild(i).GetComponent<MeshRenderer>());
            }
        }

        initPos = transform.localPosition;
        initRot = transform.localEulerAngles;
    }

    private void Update()
    {
        if (showOutline)
        {
            glowBox.SetActive(true);
            alpha += speed * Time.deltaTime;

            if (alpha >= maxAlpha || alpha <= minAlpha)
            {
                speed *= -1;
            }

            for (int i = 0; i < mat.Count; i++)
            {
                Color col = new Color(200.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, alpha / 255.0f);
                mat[i].material.color = col;
            }
        }
        else
        {
            alpha = 0;
            for (int i = 0; i < mat.Count; i++)
            {
                Color col = new Color(200.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, alpha / 255.0f);
                mat[i].material.color = col;
            }

            glowBox.SetActive(false);
        }

        gameObject.transform.localPosition = initPos;
        gameObject.transform.localEulerAngles = initRot;
    }
}
