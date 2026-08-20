using System;
using System.Collections;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("REEL SETUP")] [SerializeField]
    private SCO_SlotItem[] m_availableItems;

    [SerializeField] private float m_slotHeight = 2.5f;

    [Header("SPIN")] [SerializeField] private float m_spinSpeed = 12f;
    [SerializeField] private float m_acceleration = 25f;
    [SerializeField] private float m_deceleration = 10f;
    [SerializeField] private float m_stopDistance = 0.2f;
    [SerializeField] private float m_stopDuration = 0.25f;

    private RNG m_rng;
    private float m_currentSpeed;
    private Coroutine m_stopCoroutine;

    public bool IsSpinning { get; private set; }
    public bool shouldSpin { get; private set; }

    public void ReelInit(SCO_SlotItem[] availableItems, RNG rng)
    {
        m_availableItems = availableItems;
        m_rng = rng;
    }

    //Randomizing the items in the reel at the start of the game
    private void Start()
    {
        foreach (Transform symbol in transform)
        {
            SCO_SlotItem randomItem = m_rng.Pick(m_availableItems, s => s.weight);

            symbol.GetComponent<SpriteRenderer>().sprite = randomItem.icon;
            symbol.GetComponent<ReelSymbol>().item = randomItem;
        }
    }

    private void Update()
    {
        if (!shouldSpin)
            return;

        m_currentSpeed = Mathf.MoveTowards(
            m_currentSpeed,
            m_spinSpeed,
            m_acceleration * Time.deltaTime
        );

        MoveSymbols();
    }

    private void MoveSymbols()
    {
        foreach (Transform symbol in transform)
        {
            symbol.localPosition +=
                Vector3.down * m_currentSpeed * Time.deltaTime;

            if (symbol.localPosition.y < -m_slotHeight)
            {
                symbol.localPosition = new Vector3(
                    symbol.localPosition.x,
                    -m_slotHeight,
                    symbol.localPosition.z
                );

                SpawnNewSymbol(symbol.gameObject);
            }
        }
    }

    private void SpawnNewSymbol(GameObject symbol)
    {
        SCO_SlotItem item = m_rng.Pick(m_availableItems, s => s.weight);

        SpriteRenderer renderer = symbol.GetComponent<SpriteRenderer>();
        ReelSymbol reelSymbol = symbol.GetComponent<ReelSymbol>();

        renderer.sprite = item.icon;
        reelSymbol.item = item;

        symbol.transform.localPosition = new Vector3(
            symbol.transform.localPosition.x,
            m_slotHeight,
            symbol.transform.localPosition.z
        );
    }

    public void RequestSpin()
    {
        if (!IsSpinning)
        {
            shouldSpin = true;
            IsSpinning = true;
        }
        else
        {
            shouldSpin = false;

            if (m_stopCoroutine != null)
                StopCoroutine(m_stopCoroutine);

            m_stopCoroutine = StartCoroutine(StopReel());
        }
    }

    //To keep the reel moving until it is at a suitable position
    private IEnumerator StopReel()
    {
        while (true)
        {
            Transform targetSymbol = GetClosestSymbolToCenter();

            if (targetSymbol != null &&
                Mathf.Abs(targetSymbol.localPosition.y) <= m_stopDistance)
            {
                break;
            }

            m_currentSpeed = Mathf.MoveTowards(
                m_currentSpeed,
                m_spinSpeed,
                m_acceleration * Time.deltaTime
            );

            MoveSymbols();

            yield return null;
        }

        Transform symbolToStop = GetClosestSymbolToCenter();

        if (symbolToStop == null)
            yield break;

        // Ease the symbol into the exact center position.
        float startY = symbolToStop.localPosition.y;
        float elapsed = 0f;

        while (elapsed < m_stopDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / m_stopDuration);

            // Ease out.
            t = 1f - Mathf.Pow(1f - t, 3f);

            float targetY = Mathf.Lerp(startY, 0f, t);
            float movement = targetY - symbolToStop.localPosition.y;

            foreach (Transform symbol in transform)
            {
                symbol.localPosition += Vector3.up * movement;
            }

            yield return null;
        }

        float finalOffset = -symbolToStop.localPosition.y;

        foreach (Transform symbol in transform)
        {
            symbol.localPosition += Vector3.up * finalOffset;
        }

        m_currentSpeed = 0f;
        IsSpinning = false;
        shouldSpin = false;
        m_stopCoroutine = null;
    }

    private Transform GetClosestSymbolToCenter()
    {
        Transform closestSymbol = null;
        float closestDistance = float.MaxValue;

        foreach (Transform symbol in transform)
        {
            float distance = Mathf.Abs(symbol.localPosition.y);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSymbol = symbol;
            }
        }

        return closestSymbol;
    }
}
