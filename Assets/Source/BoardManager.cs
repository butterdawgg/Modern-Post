using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private int maxX;
    [SerializeField] private int maxZ;
    [SerializeField] private Image gridImage;
    [SerializeField] private Image blockImagePrefab;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI highestStreakText;
    [SerializeField] private float offset;
    [SerializeField] private string[] losePhrases;
    [SerializeField] private string[] winPhrases;

    private Block[] blocks;
    private Image[] blockImages;
    private Vector3Int[] targetPositions;
    private Vector3Int[] initialPositions;
    private bool won = false;
    private float streak = 0;
    private CameraShake cameraShake;

    private void Awake()
    {
        stateText.gameObject.SetActive(false);
        cameraShake = FindObjectOfType<CameraShake>();
        StartCoroutine(ChangeCoroutine());
    }

    private void Update()
    {
        streakText.text = "Current streak: " + streak;
        highestStreakText.text = "Highest streak: " + SerializeManager.Instance.GetFloat(FloatType.HighestStreak);

        if (SerializeManager.Instance.GetFloat(FloatType.HighestStreak) < streak)
            SerializeManager.Instance.SetFloat(FloatType.HighestStreak, streak);

        stateText.rectTransform.localScale = Vector3.one + (new Vector3(Mathf.Sin(Time.time * 3f), Mathf.Sin(Time.time * 3f), Mathf.Sin(Time.time * 3f)) * 0.02f);

        int k = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
                for (int j = 0; j < targetPositions.Length; j++)
                {
                    if (Vector3Int.RoundToInt(blocks[i].transform.position) == targetPositions[j])
                    {
                        k++;
                        break;
                    }
                }
            }
        }

        if (!won & k == blocks.Length)
        {
            StopAllCoroutines();
            StartCoroutine(WinCoroutine());
            won = true;
        }
    }

    private void SetNewTargetPositions()
    {
        for (int i = 0; i < targetPositions.Length; i++)
        {
            Vector3Int random = Vector3Int.zero;
            bool fits = false;
            while (!fits)
            {
                random = new Vector3Int(Random.Range(0, maxX), 0, Random.Range(0, maxZ));
                fits = true;
                for (int j = 0; j < i; j++)
                {
                    if (random == targetPositions[j])
                    {
                        fits = false;
                        break;
                    }
                }
            }
            targetPositions[i] = random;
        }
    }

    private void SetNewInitialPositions()
    {
        initialPositions = new Vector3Int[targetPositions.Length];
        for (int i = 0; i < targetPositions.Length; i++)
        {
            Vector3Int random = Vector3Int.zero;
            bool fits = false;
            while (!fits)
            {
                random = new Vector3Int(Random.Range(0, maxX), 0, Random.Range(0, maxZ));
                fits = true;
                for (int j = 0; j < i; j++)
                {
                    if (random == initialPositions[j])
                    {
                        fits = false;
                        break;
                    }
                    else if (random == targetPositions[j])
                    {
                        fits = false;
                        break;
                    }
                }
            }
            initialPositions[i] = random;
        }
    }

    private IEnumerator ChangeCoroutine()
    {
        stateText.gameObject.SetActive(true);

        targetPositions = new Vector3Int[Random.Range(8, 13)];
        initialPositions = new Vector3Int[targetPositions.Length];

        SetNewTargetPositions();
        SetNewInitialPositions();

        blocks = new Block[initialPositions.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = Instantiate(blockPrefab.gameObject, initialPositions[i], Quaternion.Euler(0f, 0f, 0f), transform).GetComponent<Block>();
            blocks[i].SetBoundaries(maxX, maxZ);
        }

        blockImages = new Image[targetPositions.Length];
        for (int i = 0; i < blockImages.Length; i++)
        {
            blockImages[i] = Instantiate(blockImagePrefab, gridImage.rectTransform);
            blockImages[i].rectTransform.anchoredPosition = new Vector2(((targetPositions[i].x + 1) * offset) + (targetPositions[i].x * blockImages[i].rectTransform.rect.width), ((targetPositions[i].z + 1) * offset) + (targetPositions[i].z * blockImages[i].rectTransform.rect.height));
        }

        cameraShake.Shake(0.5f, 0.025f);

        yield return new WaitForSeconds(1f);

        won = false;

        for (int i = 10; i >= 0; i--)
        {
            stateText.text = "You have " + i + " seconds left";
            yield return new WaitForSeconds(1f);
        }

        streak = 0;
        stateText.text = losePhrases[Random.Range(0, losePhrases.Length)];

        cameraShake.Shake(0.25f, 0.1f);

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
                blocks[i].Detonate();
        }

        for (int i = 0; i < blockImages.Length; i++)
        {
            if (blockImages[i] != null)
                Destroy(blockImages[i].gameObject);
        }

        yield return new WaitForSeconds(2f);

        yield return null;
        StartCoroutine(ChangeCoroutine());
    }

    private IEnumerator WinCoroutine()
    {
        streak += 1;
        stateText.text = winPhrases[Random.Range(0, winPhrases.Length)];

        cameraShake.Shake(1.5f, 0.025f);

        for (int i = 0; i < blocks.Length; i++)
        {
            if(blocks[i] != null)
                blocks[i].Ascend();
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < blockImages.Length; i++)
        {
            if (blockImages[i] != null)
                Destroy(blockImages[i].gameObject);
        }

        StartCoroutine(ChangeCoroutine());
    }
}
