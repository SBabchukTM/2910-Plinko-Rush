using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public class BallHitController : MonoBehaviour
{
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float maxDragDistance = 2f;
    [SerializeField] private GameObject trajectoryPointPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider;
    [SerializeField] private List<GameObject> trajectoryPoints;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float stickyDelayIfGrounded = 0.3f;

    private bool wasAirborneAfterHit = false;
    private bool canSticky = false;
    private Vector2 startPoint;
    private bool isDragging = false;
    private bool isSlimeBall = false;
    private Vector3 _lastSafePosition;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public Camera mainCamera;
    public GolfGameManager gameManager;
    public Rigidbody2D rb;
    public int ShotCount = 0;
    public int Score = 0;

    public event Action OnBallHit;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        SetKinematic(true);
    }
    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (!collider.enabled) return;
        if (rb.velocity.magnitude > 0.05f) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(touchWorldPos, rb.position) < 1f)
            {
                startPoint = touchWorldPos;
                isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = currentPoint - startPoint;
            Vector2 clampedForceDir = Vector2.ClampMagnitude(dragVector, maxDragDistance);
            float forcePercent = clampedForceDir.magnitude / maxDragDistance;
            Vector2 finalForce = clampedForceDir.normalized * (forcePercent * maxForce);

            ShowTrajectory(finalForce);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 releasePoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 forceDir = releasePoint - (Vector2)transform.position;

            HideTrajectory();
            PerformHitAsync(forceDir);
        }

    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (Vector2.Distance(touchWorldPos, rb.position) < 1f)
                {
                    startPoint = touchWorldPos;
                    isDragging = true;
                }
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    Vector2 dragVector = touchWorldPos - startPoint;
                    Vector2 clampedForceDir = Vector2.ClampMagnitude(dragVector, maxDragDistance);
                    float forcePercent = clampedForceDir.magnitude / maxDragDistance;
                    Vector2 finalForce = clampedForceDir.normalized * (forcePercent * maxForce);

                    ShowTrajectory(finalForce);
                }

                break;

            case TouchPhase.Ended:
                if (isDragging)
                {
                    Vector2 forceDir = touchWorldPos - startPoint;
                    PerformHitAsync(forceDir);
                    HideTrajectory();
                }
                break;
        }
    }

    public void ResetShotCount()
    {
        ShotCount = 0;
    }

    async Task WaitForBallToStop(CancellationToken token)
    {
        try
        {
            while (rb.velocity.magnitude > 0.05f)
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            float stillTime = 0f;
            const float requiredStillTime = 0.75f;

            while (stillTime < requiredStillTime)
            {
                token.ThrowIfCancellationRequested();

                if (rb.velocity.magnitude > 0.05f)
                {
                    stillTime = 0f;
                }
                else
                {
                    stillTime += Time.deltaTime;
                }

                await Task.Yield();
            }

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("WaitForBallToStop was cancelled.");
        }
    }

    public void Activate()
    {
        SetAlpha(1f);

        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        collider.enabled = true;
    }

    public void Deactivate()
    {
        SetAlpha(0.25f);
        SetKinematic(true);

        rb.angularVelocity = 0f;
        rb.velocity = Vector2.zero;
        collider.enabled = false;
    }

    public void ApplyConfig(BallConfig config)
    {
        rb.mass = config.Mass;
        spriteRenderer.sprite = config.ItemSprite;
        isSlimeBall = config.IsSlime;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void ReturnToLastSafePosition()
    {
        transform.position = _lastSafePosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        SetKinematic(true);
    }

    public void SetKinematic(bool isKinematic)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.isKinematic = isKinematic;
        if (!isKinematic)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    async void PerformHitAsync(Vector2 forceDir)
    {
        _lastSafePosition = transform.position;

        SetKinematic(false);
        isDragging = false;

        wasAirborneAfterHit = false;
        canSticky = false;

        if (forceDir.magnitude > 0)
        {
            Vector2 clampedForceDir = Vector2.ClampMagnitude(forceDir, maxDragDistance);
            float forcePercent = clampedForceDir.magnitude / maxDragDistance;
            Vector2 finalForce = clampedForceDir.normalized * (forcePercent * maxForce);

            rb.AddForce(finalForce, ForceMode2D.Impulse);
            ShotCount++;

            OnBallHit?.Invoke();

            if (isSlimeBall)
                _ = CheckStickyAfterHitAsync(_cancellationTokenSource.Token);
        }

        await WaitForBallToStop(_cancellationTokenSource.Token);
        gameManager.NextTurn();
    }

    async Task CheckStickyAfterHitAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(100, token);

            if (!IsGrounded())
            {
                wasAirborneAfterHit = true;
                canSticky = true;
            }
            else
            {
                await Task.Delay((int)(stickyDelayIfGrounded * 100), token);
                if (IsGrounded())
                {
                    SetKinematic(true);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("CheckStickyAfterHitAsync was cancelled.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isSlimeBall || !isActiveAndEnabled) return;

        if (canSticky && IsGrounded())
        {
            SetKinematic(true);
            canSticky = false;
        }
    }

    private void ShowTrajectory(Vector2 force)
    {
        Vector2 startPos = rb.position;

        Vector2 velocity = force / rb.mass;

        Vector2 gravity = Physics2D.gravity * rb.gravityScale;

        float timeStep = 0.05f;

        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            float t = (i + 1) * timeStep;
            Vector2 pos = startPos + velocity * t + 0.5f * gravity * t * t;

            trajectoryPoints[i].transform.position = pos;
            trajectoryPoints[i].SetActive(true);
        }
    }

    private void HideTrajectory()
    {
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.05f;
        RaycastHit2D hit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, extraHeight, groundLayer);
        return hit.collider != null;
    }
}