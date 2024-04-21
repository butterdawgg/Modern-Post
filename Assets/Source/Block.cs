using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Block : MonoBehaviour
{
    [SerializeField] LayerMask layerMaskGround;
    [SerializeField] LayerMask layerMaskSelf;
    [SerializeField] float lerpK;
    [SerializeField] float acceleration;
    [SerializeField] GameObject graphic;
    [SerializeField] ParticleSystem ps;
    [SerializeField] ParticleSystem ps1;
    [SerializeField] ParticleSystem ps2;

    private bool selected;
    private bool grabbed;
    private bool isAscending;
    private bool isAppearing;
    private bool isDetonating;
    private bool cantMove;
    private Vector3 placedPosition;
    private Vector3 initialPosition;
    private float velocity;
    private float maxX;
    private float maxZ;
    private Camera cam;

    private void Awake()
    {
        cam = FindObjectOfType<Camera>();
        Appear();
    }

    private void Update()
    {
        if (isAscending)
        {
            velocity += acceleration * Time.deltaTime;
            transform.position += Vector3.up * velocity * Time.deltaTime;
            return;
        }

        if (cantMove)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3Int.RoundToInt(transform.position), lerpK * Time.deltaTime);
            return;
        }

        if (isDetonating | isAppearing)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3Int cursorPosition = Vector3Int.zero;

        if (Physics.Raycast(ray, out RaycastHit hit1, 10000f, layerMaskSelf))
        {
            if (hit1.transform == transform)
                selected = true;
            else
                selected = false;
        }
        else
            selected = false;

        if(Physics.Raycast(ray, out RaycastHit hit2, 10000f, layerMaskGround))
        {
            cursorPosition = Vector3Int.RoundToInt(new Vector3(hit2.point.x, 0f, hit2.point.z));
        }

        if (selected & Input.GetKeyDown(KeyCode.Mouse0))
        {
            initialPosition = Vector3Int.RoundToInt(new Vector3(transform.position.x, 0f, transform.position.z));
            grabbed = true;
        }
        if (grabbed & Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (Physics.CheckSphere(Vector3Int.RoundToInt(new Vector3(hit2.point.x, 0f, hit2.point.z)), 0.1f, layerMaskSelf) | cursorPosition.x >= maxX | cursorPosition.z >= maxZ | cursorPosition.x < 0 | cursorPosition.z < 0)
                placedPosition = initialPosition;
            else
                placedPosition = cursorPosition;

            grabbed = false;

            AudioManager.Instance.PlaySound("Place");
        }

        if (grabbed)
            transform.position = Vector3.Lerp(transform.position, hit2.point + (Vector3.up * 1f), lerpK * Time.deltaTime);
        else
            transform.position = Vector3.Lerp(transform.position, placedPosition, lerpK * Time.deltaTime);
    }

    public void SetBoundaries(float maxX, float maxZ)
    {
        this.maxX = maxX;
        this.maxZ = maxZ;
    }

    public void Detonate()
    {
        isDetonating = true;
        ps1.Play();
        Destroy(graphic);
        Destroy(gameObject, 2f);
        AudioManager.Instance.PlaySound("Explosion");
    }

    public void Appear()
    {
        placedPosition = transform.position;
        StartCoroutine(AppearCoroutine());
    }

    private IEnumerator AppearCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        graphic.SetActive(false);
        isAppearing = true;
        ps2.Play();
        AudioManager.Instance.PlaySound("Appear");
        yield return new WaitForSeconds(0.5f);
        graphic.SetActive(true);
        isAppearing = false;
    }

    public void Ascend()
    {
        StartCoroutine(AscendCoroutine());
    }

    private IEnumerator AscendCoroutine()
    {
        cantMove = true;

        yield return new WaitForSeconds(0.25f);

        isAscending = true;

        ps.Play();

        AudioManager.Instance.PlaySound("Ascend");
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
