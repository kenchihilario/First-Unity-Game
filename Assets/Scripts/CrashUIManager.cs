using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrashUIManager : MonoBehaviour
{
    [Header("Core UI References")]
    public Text multiplierText;
    public Text statusText;
    public Text balanceText;
    public Button actionButton;
    public Text actionButtonText;
    public Image backgroundImage;
    public Transform graphContainer;
    public InputField betAmountInput;
    public Button halfBetBtn;
    public Button doubleBetBtn;

    private float betAmount = 10f;
    private Color bgWaiting = new Color(0.043f, 0.055f, 0.078f, 1f);
    private Color bgPlaying = new Color(0.031f, 0.051f, 0.078f, 1f);
    private Color bgCrashed = new Color(0.118f, 0.039f, 0.051f, 1f);

    private RectTransform flightViewport;
    private CrashCurveGraphic curveGraphic;
    private RectTransform launchPad;
    private RectTransform rocketRoot;
    private Image rocketImage;
    private RectTransform flameRoot;
    private Image flameImage;

    private const int ParticlePoolSize = 24;
    private struct SparkParticle
    {
        public RectTransform rt;
        public Image img;
        public Vector2 velocity;
        public float life;
        public float maxLife;
        public Color startColor;
    }
    private SparkParticle[] sparkPool;
    private int sparkPoolIndex = 0;
    private float sparkSpawnTimer = 0f;

    private RectTransform explosionRoot;
    private Image shockwaveRing;
    private struct ExplosionDebris
    {
        public RectTransform rt;
        public Image img;
        public Vector2 vel;
        public float life;
        public float maxLife;
    }
    private ExplosionDebris[] explosionDebris;
    private bool isExplosionActive = false;
    private float explosionTimer = 0f;

    private readonly List<Vector2> curvePoints = new List<Vector2>();
    private float flightTimer = 0f;
    private float currentMult = 1.0f;
    private float waitingCountdown = 5.0f;

    void Start()
    {
        if (graphContainer == null) graphContainer = transform.Find("GraphContainer");
        if (multiplierText == null) multiplierText = transform.Find("MultiplierText")?.GetComponent<Text>();
        if (statusText == null) statusText = transform.Find("StatusText")?.GetComponent<Text>();
        if (actionButton == null) actionButton = transform.Find("ActionBtn")?.GetComponent<Button>() ?? transform.Find("BetPanel/ActionBtn")?.GetComponent<Button>();
        if (actionButtonText == null && actionButton != null) actionButtonText = actionButton.GetComponentInChildren<Text>();
        if (betAmountInput == null) betAmountInput = transform.Find("BetPanel/BetInput")?.GetComponent<InputField>() ?? GetComponentInChildren<InputField>(true);
        if (halfBetBtn == null) halfBetBtn = transform.Find("BetPanel/HalfBtn")?.GetComponent<Button>();
        if (doubleBetBtn == null) doubleBetBtn = transform.Find("BetPanel/DoubleBtn")?.GetComponent<Button>();

        if (CrashGameManager.Instance != null)
        {
            CrashGameManager.Instance.OnMultiplierUpdated -= UpdateMultiplier;
            CrashGameManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
            CrashGameManager.Instance.OnGameStateChanged -= UpdateGameState;
            CrashGameManager.Instance.OnGameStateChanged += UpdateGameState;
        }

        if (betAmountInput != null)
        {
            betAmountInput.onEndEdit.AddListener(val =>
            {
                if (float.TryParse(val, out float res)) betAmount = Mathf.Max(1f, res);
                UpdateBetDisplay();
            });
        }

        if (halfBetBtn != null) halfBetBtn.onClick.AddListener(() =>
        {
            betAmount = Mathf.Max(1f, Mathf.Floor(betAmount / 2f));
            UpdateBetDisplay();
        });

        if (doubleBetBtn != null) doubleBetBtn.onClick.AddListener(() =>
        {
            betAmount = Mathf.Floor(betAmount * 2f);
            UpdateBetDisplay();
        });

        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionButtonClicked);
        }

        UpdateBetDisplay();
        SetupFlightScene();

        if (CrashGameManager.Instance != null)
        {
            UpdateGameState(CrashGameManager.Instance.currentState);
        }
    }

    void OnDestroy()
    {
        if (CrashGameManager.Instance != null)
        {
            CrashGameManager.Instance.OnMultiplierUpdated -= UpdateMultiplier;
            CrashGameManager.Instance.OnGameStateChanged -= UpdateGameState;
        }
    }

    void UpdateBetDisplay()
    {
        if (betAmountInput != null) betAmountInput.text = betAmount.ToString("F2");
        if (CrashGameManager.Instance != null && CrashGameManager.Instance.currentState == CrashGameManager.GameState.Waiting)
        {
            if (actionButtonText != null) actionButtonText.text = $"BET ₱{betAmount:F2}";
        }
    }

    void Update()
    {
        if (PlayerManager.Instance != null && balanceText != null)
        {
            balanceText.text = $"<color=#888888>BALANCE</color>\n<color=#00FFAA><size=50>₱{PlayerManager.Instance.balance:N2}</size></color>";
        }

        if (CrashGameManager.Instance == null) return;

        CrashGameManager.GameState state = CrashGameManager.Instance.currentState;

        if (state == CrashGameManager.GameState.Playing)
        {
            UpdateFlyingAnimation();
        }
        else if (state == CrashGameManager.GameState.Waiting)
        {
            UpdateWaitingAnimation();
        }
        else if (state == CrashGameManager.GameState.Crashed)
        {
            UpdateCrashedAnimation();
        }

        UpdateParticles();
    }

    void SetupFlightScene()
    {
        if (graphContainer == null) return;

        Transform oldLine = graphContainer.Find("CrashLine");
        if (oldLine != null) Destroy(oldLine.gameObject);
        Transform oldDot = graphContainer.Find("RocketDot");
        if (oldDot != null) Destroy(oldDot.gameObject);
        Transform oldZone = graphContainer.Find("FlightViewport");
        if (oldZone != null) Destroy(oldZone.gameObject);

        GameObject zoneGO = new GameObject("FlightViewport", typeof(RectTransform));
        zoneGO.transform.SetParent(graphContainer, false);
        flightViewport = zoneGO.GetComponent<RectTransform>();
        flightViewport.anchorMin = new Vector2(0.04f, 0.08f);
        flightViewport.anchorMax = new Vector2(0.96f, 0.92f);
        flightViewport.offsetMin = Vector2.zero;
        flightViewport.offsetMax = Vector2.zero;

        CreateBackgroundRadarDecor(flightViewport);

        GameObject curveGO = new GameObject("TrajectoryCurve", typeof(RectTransform));
        curveGO.transform.SetParent(flightViewport, false);
        RectTransform curveRt = curveGO.GetComponent<RectTransform>();
        curveRt.anchorMin = Vector2.zero;
        curveRt.anchorMax = Vector2.one;
        curveRt.offsetMin = Vector2.zero;
        curveRt.offsetMax = Vector2.zero;

        curveGraphic = curveGO.AddComponent<CrashCurveGraphic>();
        curveGraphic.lineWidth = 7.5f;
        curveGraphic.lineColor = new Color(0.0f, 0.95f, 0.65f, 1f);
        curveGraphic.lineEndColor = new Color(0.0f, 0.92f, 1f, 1f);
        curveGraphic.crashLineColor = new Color(0.95f, 0.25f, 0.25f, 1f);
        curveGraphic.fillTopColor = new Color(0.0f, 0.95f, 0.7f, 0.28f);
        curveGraphic.fillBottomColor = new Color(0.02f, 0.12f, 0.2f, 0.0f);
        curveGraphic.drawFill = true;

        GameObject padGO = new GameObject("LaunchPad", typeof(RectTransform), typeof(Image));
        padGO.transform.SetParent(flightViewport, false);
        launchPad = padGO.GetComponent<RectTransform>();
        launchPad.anchorMin = Vector2.zero;
        launchPad.anchorMax = Vector2.zero;
        launchPad.sizeDelta = new Vector2(50, 6);
        launchPad.pivot = new Vector2(0.5f, 0.5f);
        launchPad.anchoredPosition = new Vector2(40f, 30f);
        Image padImg = padGO.GetComponent<Image>();
        padImg.color = new Color(0.0f, 0.95f, 0.65f, 0.8f);

        GameObject sparksRoot = new GameObject("ExhaustSparks", typeof(RectTransform));
        sparksRoot.transform.SetParent(flightViewport, false);
        sparkPool = new SparkParticle[ParticlePoolSize];
        Sprite glowSprite = CrashVisuals.GetGlowDotSprite();

        for (int i = 0; i < ParticlePoolSize; i++)
        {
            GameObject spGO = new GameObject($"Spark_{i}", typeof(RectTransform), typeof(Image));
            spGO.transform.SetParent(sparksRoot.transform, false);
            RectTransform spRt = spGO.GetComponent<RectTransform>();
            spRt.anchorMin = Vector2.zero;
            spRt.anchorMax = Vector2.zero;
            spRt.sizeDelta = new Vector2(16, 16);
            Image spImg = spGO.GetComponent<Image>();
            spImg.sprite = glowSprite;
            spImg.raycastTarget = false;
            spGO.SetActive(false);

            sparkPool[i] = new SparkParticle
            {
                rt = spRt,
                img = spImg,
                life = 0f,
                maxLife = 0.5f
            };
        }

        GameObject rRootGO = new GameObject("RocketRoot", typeof(RectTransform));
        rRootGO.transform.SetParent(flightViewport, false);
        rocketRoot = rRootGO.GetComponent<RectTransform>();
        rocketRoot.anchorMin = Vector2.zero;
        rocketRoot.anchorMax = Vector2.zero;
        rocketRoot.sizeDelta = new Vector2(74, 37);
        rocketRoot.pivot = new Vector2(0.5f, 0.5f);

        GameObject flameGO = new GameObject("ThrusterFlame", typeof(RectTransform), typeof(Image));
        flameGO.transform.SetParent(rocketRoot, false);
        flameRoot = flameGO.GetComponent<RectTransform>();
        flameRoot.anchorMin = new Vector2(0.5f, 0.5f);
        flameRoot.anchorMax = new Vector2(0.5f, 0.5f);
        flameRoot.pivot = new Vector2(1.0f, 0.5f);
        flameRoot.anchoredPosition = new Vector2(-28f, 0f);
        flameRoot.sizeDelta = new Vector2(56, 28);
        flameImage = flameGO.GetComponent<Image>();
        flameImage.sprite = CrashVisuals.GetFlameSprite();
        flameImage.raycastTarget = false;

        GameObject rImgGO = new GameObject("RocketImage", typeof(RectTransform), typeof(Image));
        rImgGO.transform.SetParent(rocketRoot, false);
        RectTransform rImgRt = rImgGO.GetComponent<RectTransform>();
        rImgRt.anchorMin = Vector2.zero;
        rImgRt.anchorMax = Vector2.one;
        rImgRt.offsetMin = Vector2.zero;
        rImgRt.offsetMax = Vector2.zero;
        rocketImage = rImgGO.GetComponent<Image>();
        rocketImage.sprite = CrashVisuals.GetRocketSprite();
        rocketImage.raycastTarget = false;

        GameObject explGO = new GameObject("ExplosionRoot", typeof(RectTransform));
        explGO.transform.SetParent(flightViewport, false);
        explosionRoot = explGO.GetComponent<RectTransform>();
        explosionRoot.anchorMin = Vector2.zero;
        explosionRoot.anchorMax = Vector2.zero;
        explosionRoot.pivot = new Vector2(0.5f, 0.5f);

        GameObject ringGO = new GameObject("ShockwaveRing", typeof(RectTransform), typeof(Image));
        ringGO.transform.SetParent(explosionRoot, false);
        RectTransform ringRt = ringGO.GetComponent<RectTransform>();
        ringRt.anchorMin = new Vector2(0.5f, 0.5f);
        ringRt.anchorMax = new Vector2(0.5f, 0.5f);
        ringRt.pivot = new Vector2(0.5f, 0.5f);
        ringRt.anchoredPosition = Vector2.zero;
        shockwaveRing = ringGO.GetComponent<Image>();
        shockwaveRing.sprite = CrashVisuals.GetShockwaveRingSprite();
        shockwaveRing.color = new Color(1.0f, 0.35f, 0.15f, 0f);
        shockwaveRing.raycastTarget = false;

        explosionDebris = new ExplosionDebris[16];
        for (int i = 0; i < 16; i++)
        {
            GameObject dGO = new GameObject($"Debris_{i}", typeof(RectTransform), typeof(Image));
            dGO.transform.SetParent(explosionRoot, false);
            RectTransform dRt = dGO.GetComponent<RectTransform>();
            dRt.anchorMin = new Vector2(0.5f, 0.5f);
            dRt.anchorMax = new Vector2(0.5f, 0.5f);
            dRt.pivot = new Vector2(0.5f, 0.5f);
            dRt.sizeDelta = new Vector2(16, 16);
            Image dImg = dGO.GetComponent<Image>();
            dImg.sprite = glowSprite;
            dImg.raycastTarget = false;
            dGO.SetActive(false);

            explosionDebris[i] = new ExplosionDebris
            {
                rt = dRt,
                img = dImg
            };
        }
        explosionRoot.gameObject.SetActive(false);

        if (multiplierText != null) multiplierText.transform.SetAsLastSibling();
        if (statusText != null) statusText.transform.SetAsLastSibling();
    }

    void CreateBackgroundRadarDecor(RectTransform parent)
    {
        float[] ratios = { 0.25f, 0.50f, 0.75f };
        string[] labels = { "2.00x", "5.00x", "10.00x" };

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        for (int i = 0; i < ratios.Length; i++)
        {
            GameObject lineGO = new GameObject($"AltLine_{i}", typeof(RectTransform), typeof(Image));
            lineGO.transform.SetParent(parent, false);
            RectTransform lRt = lineGO.GetComponent<RectTransform>();
            lRt.anchorMin = new Vector2(0f, ratios[i]);
            lRt.anchorMax = new Vector2(1f, ratios[i]);
            lRt.pivot = new Vector2(0.5f, 0.5f);
            lRt.anchoredPosition = Vector2.zero;
            lRt.sizeDelta = new Vector2(0f, 1.5f);
            Image lImg = lineGO.GetComponent<Image>();
            lImg.color = new Color(0.12f, 0.17f, 0.25f, 0.45f);
            lImg.raycastTarget = false;

            GameObject tagGO = new GameObject($"AltTag_{i}", typeof(RectTransform), typeof(Text));
            tagGO.transform.SetParent(lineGO.transform, false);
            RectTransform tRt = tagGO.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0f, 0.5f);
            tRt.anchorMax = new Vector2(0f, 0.5f);
            tRt.pivot = new Vector2(0f, 0.5f);
            tRt.anchoredPosition = new Vector2(8f, 10f);
            tRt.sizeDelta = new Vector2(80, 24);
            Text txt = tagGO.GetComponent<Text>();
            txt.font = font;
            txt.fontSize = 13;
            txt.fontStyle = FontStyle.Bold;
            txt.color = new Color(0.35f, 0.45f, 0.60f, 0.6f);
            txt.text = labels[i];
            txt.raycastTarget = false;
        }
    }

    void UpdateFlyingAnimation()
    {
        flightTimer += Time.deltaTime;

        if (flightViewport == null) return;
        Rect viewRect = flightViewport.rect;
        float viewW = Mathf.Max(300f, viewRect.width);
        float viewH = Mathf.Max(200f, viewRect.height);

        float originX = 40f;
        float originY = 32f;
        float usableW = viewW - originX - 90f;
        float usableH = viewH - originY - 80f;

        float maxDisplayTime = Mathf.Max(8.0f, flightTimer * 1.25f);
        float maxDisplayMult = Mathf.Max(2.2f, currentMult * 1.20f);

        curvePoints.Clear();
        int sampleCount = 55;

        for (int i = 0; i < sampleCount; i++)
        {
            float tNorm = (float)i / (sampleCount - 1);
            float sampleTime = tNorm * flightTimer;
            float sampleM = Mathf.Pow(2.71828f, sampleTime * 0.15f);

            float x = originX + (sampleTime / maxDisplayTime) * usableW;
            float yRatio = Mathf.Clamp01((sampleM - 1.0f) / (maxDisplayMult - 1.0f));
            float y = originY + Mathf.Pow(yRatio, 0.82f) * usableH;

            curvePoints.Add(new Vector2(x, y));
        }

        if (curveGraphic != null)
        {
            curveGraphic.UpdateCurve(curvePoints, originY, false);
        }

        Vector2 tipPos = curvePoints[curvePoints.Count - 1];
        Vector2 prevPos = curvePoints[curvePoints.Count - 2];

        if (rocketRoot != null)
        {
            if (!rocketRoot.gameObject.activeSelf) rocketRoot.gameObject.SetActive(true);
            rocketRoot.anchoredPosition = tipPos;

            Vector2 delta = tipPos - prevPos;
            float flightAngle = 25f;
            if (delta.sqrMagnitude > 0.0001f)
            {
                flightAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            }

            float turbulence = (Mathf.PerlinNoise(Time.time * 30f, 0.5f) - 0.5f) * 2.8f;
            rocketRoot.localRotation = Quaternion.Euler(0, 0, flightAngle + turbulence);
        }

        if (flameRoot != null)
        {
            flameRoot.gameObject.SetActive(true);
            float flameFlicker = 1.0f + 0.22f * Mathf.Sin(Time.time * 50f) + Random.Range(-0.08f, 0.08f);
            float speedBoost = Mathf.Clamp(1.0f + flightTimer * 0.05f, 1.0f, 1.85f);
            flameRoot.localScale = new Vector3(flameFlicker * speedBoost, flameFlicker, 1.0f);
        }

        sparkSpawnTimer += Time.deltaTime;
        if (sparkSpawnTimer >= 0.045f)
        {
            sparkSpawnTimer = 0f;
            SpawnExhaustSpark(tipPos, prevPos);
        }
    }

    void SpawnExhaustSpark(Vector2 tipPos, Vector2 prevPos)
    {
        if (sparkPool == null) return;

        Vector2 flightDir = (tipPos - prevPos).normalized;
        Vector2 nozzlePos = tipPos - flightDir * 32f + (Vector2)Random.insideUnitCircle * 4f;
        Vector2 backwardVel = -flightDir * Random.Range(60f, 160f) + new Vector2(Random.Range(-25f, 25f), Random.Range(-25f, 25f));

        ref SparkParticle sp = ref sparkPool[sparkPoolIndex];
        sp.rt.gameObject.SetActive(true);
        sp.rt.anchoredPosition = nozzlePos;
        sp.velocity = backwardVel;
        sp.life = 0f;
        sp.maxLife = Random.Range(0.35f, 0.55f);
        sp.startColor = (Random.value < 0.65f)
            ? new Color(0.0f, 0.95f, 1.0f, 0.9f)
            : new Color(1.0f, 0.65f, 0.15f, 0.9f);
        sp.img.color = sp.startColor;
        sp.rt.localScale = Vector3.one * Random.Range(0.8f, 1.3f);

        sparkPoolIndex = (sparkPoolIndex + 1) % ParticlePoolSize;
    }

    void UpdateParticles()
    {
        if (sparkPool != null)
        {
            for (int i = 0; i < ParticlePoolSize; i++)
            {
                if (!sparkPool[i].rt.gameObject.activeSelf) continue;

                sparkPool[i].life += Time.deltaTime;
                float progress = sparkPool[i].life / sparkPool[i].maxLife;

                if (progress >= 1.0f)
                {
                    sparkPool[i].rt.gameObject.SetActive(false);
                    continue;
                }

                sparkPool[i].rt.anchoredPosition += sparkPool[i].velocity * Time.deltaTime;
                sparkPool[i].velocity *= (1f - Time.deltaTime * 3.5f);

                Color col = sparkPool[i].startColor;
                col.a = Mathf.Lerp(sparkPool[i].startColor.a, 0f, progress);
                sparkPool[i].img.color = col;
                sparkPool[i].rt.localScale = Vector3.one * (1f - progress * 0.7f);
            }
        }

        if (isExplosionActive)
        {
            explosionTimer += Time.deltaTime;
            float t = explosionTimer / 0.65f;

            if (t >= 1.0f)
            {
                isExplosionActive = false;
                if (explosionRoot != null) explosionRoot.gameObject.SetActive(false);
            }
            else
            {
                if (shockwaveRing != null)
                {
                    float ringSize = Mathf.Lerp(20f, 240f, Mathf.Sqrt(t));
                    shockwaveRing.rectTransform.sizeDelta = new Vector2(ringSize, ringSize);
                    float ringAlpha = Mathf.Lerp(0.9f, 0.0f, t * t);
                    shockwaveRing.color = new Color(1.0f, 0.35f, 0.15f, ringAlpha);
                }

                if (explosionDebris != null)
                {
                    for (int i = 0; i < explosionDebris.Length; i++)
                    {
                        if (!explosionDebris[i].rt.gameObject.activeSelf) continue;

                        explosionDebris[i].life += Time.deltaTime;
                        float dp = explosionDebris[i].life / explosionDebris[i].maxLife;
                        if (dp >= 1f)
                        {
                            explosionDebris[i].rt.gameObject.SetActive(false);
                            continue;
                        }

                        explosionDebris[i].rt.anchoredPosition += explosionDebris[i].vel * Time.deltaTime;
                        explosionDebris[i].vel += new Vector2(0, -180f) * Time.deltaTime;

                        Color c = Color.Lerp(Color.yellow, new Color(0.95f, 0.2f, 0.1f, 0f), dp);
                        explosionDebris[i].img.color = c;
                        explosionDebris[i].rt.localScale = Vector3.one * (1.2f - dp);
                    }
                }
            }
        }
    }

    void UpdateWaitingAnimation()
    {
        waitingCountdown -= Time.deltaTime;
        if (waitingCountdown < 0f) waitingCountdown = 0f;

        if (statusText != null)
        {
            statusText.text = $"<color=#00FFAA>NEXT ROUND IN {waitingCountdown:F1}s</color>";
        }

        if (rocketRoot != null)
        {
            if (!rocketRoot.gameObject.activeSelf) rocketRoot.gameObject.SetActive(true);
            float idleBob = Mathf.Sin(Time.time * 4f) * 3.5f;
            rocketRoot.anchoredPosition = new Vector2(40f, 34f + idleBob);
            rocketRoot.localRotation = Quaternion.Euler(0, 0, 26f);
        }

        if (flameRoot != null)
        {
            flameRoot.gameObject.SetActive(true);
            float idleBreath = 0.35f + 0.1f * Mathf.Sin(Time.time * 6f);
            flameRoot.localScale = new Vector3(idleBreath, idleBreath, 1f);
        }
    }

    void UpdateCrashedAnimation()
    {
    }

    void TriggerCrashExplosion(Vector2 pos)
    {
        if (explosionRoot == null) return;

        explosionRoot.gameObject.SetActive(true);
        explosionRoot.anchoredPosition = pos;
        isExplosionActive = true;
        explosionTimer = 0f;

        if (shockwaveRing != null)
        {
            shockwaveRing.rectTransform.sizeDelta = new Vector2(20, 20);
            shockwaveRing.color = new Color(1.0f, 0.4f, 0.15f, 0.9f);
        }

        if (explosionDebris != null)
        {
            for (int i = 0; i < explosionDebris.Length; i++)
            {
                explosionDebris[i].rt.gameObject.SetActive(true);
                explosionDebris[i].rt.anchoredPosition = Vector2.zero;
                float angle = (i / (float)explosionDebris.Length) * 360f * Mathf.Deg2Rad + Random.Range(-0.2f, 0.2f);
                float speed = Random.Range(140f, 320f);
                explosionDebris[i].vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                explosionDebris[i].life = 0f;
                explosionDebris[i].maxLife = Random.Range(0.4f, 0.65f);
                explosionDebris[i].img.color = Color.white;
            }
        }
    }

    void UpdateMultiplier(float mult)
    {
        currentMult = mult;
        if (multiplierText != null)
        {
            multiplierText.text = $"<b><color=#FFFFFF>{mult:F2}</color><color=#00FFAA>x</color></b>";
            float pulseScale = 1.0f + Mathf.PingPong(Time.time * 2.5f, 0.05f) * Mathf.Clamp01(mult / 5f);
            multiplierText.transform.localScale = new Vector3(pulseScale, pulseScale, 1f);

            if (gameObject.activeInHierarchy && Time.frameCount % 10 == 0 && CasinoAudio.Instance)
            {
                CasinoAudio.Instance.PlayTick();
            }
        }
    }

    void UpdateGameState(CrashGameManager.GameState state)
    {
        if (halfBetBtn != null) halfBetBtn.interactable = state == CrashGameManager.GameState.Waiting;
        if (doubleBetBtn != null) doubleBetBtn.interactable = state == CrashGameManager.GameState.Waiting;
        if (betAmountInput != null) betAmountInput.interactable = state == CrashGameManager.GameState.Waiting;

        switch (state)
        {
            case CrashGameManager.GameState.Waiting:
                flightTimer = 0f;
                currentMult = 1.0f;
                waitingCountdown = 5.0f;

                if (statusText != null)
                {
                    statusText.text = "<color=#94A3B8>PREPARING FOR LIFTOFF...</color>";
                    statusText.color = Color.white;
                }
                if (multiplierText != null)
                {
                    multiplierText.text = "<b><color=#FFFFFF>1.00</color><color=#00FFAA>x</color></b>";
                    multiplierText.transform.localScale = Vector3.one;
                }
                if (actionButtonText != null) actionButtonText.text = $"BET ₱{betAmount:F2}";
                if (actionButton != null) actionButton.interactable = true;
                if (backgroundImage != null) backgroundImage.color = bgWaiting;

                if (curveGraphic != null) curveGraphic.ClearCurve();
                if (launchPad != null) launchPad.gameObject.SetActive(true);
                if (explosionRoot != null) explosionRoot.gameObject.SetActive(false);

                if (rocketRoot != null)
                {
                    rocketRoot.gameObject.SetActive(true);
                    rocketRoot.anchoredPosition = new Vector2(40f, 34f);
                    rocketRoot.localRotation = Quaternion.Euler(0, 0, 26f);
                }
                break;

            case CrashGameManager.GameState.Playing:
                flightTimer = 0f;
                if (statusText != null)
                {
                    statusText.text = "<b><color=#00FFAA>▲ LIFTOFF - FLYING!</color></b>";
                    statusText.color = new Color(0.2f, 1f, 0.5f);
                }

                if (CrashGameManager.Instance != null && CrashGameManager.Instance.HasBet)
                {
                    if (actionButtonText != null) actionButtonText.text = "<b>CASH OUT</b>";
                    if (actionButton != null) actionButton.interactable = true;
                }
                else
                {
                    if (actionButtonText != null) actionButtonText.text = "WAITING...";
                    if (actionButton != null) actionButton.interactable = false;
                }

                if (backgroundImage != null) backgroundImage.color = bgPlaying;
                break;

            case CrashGameManager.GameState.Crashed:
                Vector2 crashPos = (curvePoints.Count > 0) ? curvePoints[curvePoints.Count - 1] : new Vector2(40, 32);

                if (statusText != null)
                {
                    statusText.text = $"<b><color=#EF4444>CRASHED @ {currentMult:F2}x</color></b>";
                    statusText.color = new Color(1f, 0.25f, 0.25f);
                }
                if (multiplierText != null)
                {
                    multiplierText.text = $"<b><color=#EF4444>{currentMult:F2}x</color></b>";
                }
                if (actionButton != null) actionButton.interactable = false;
                if (backgroundImage != null) backgroundImage.color = bgCrashed;

                if (rocketRoot != null) rocketRoot.gameObject.SetActive(false);
                TriggerCrashExplosion(crashPos);

                if (curveGraphic != null)
                {
                    curveGraphic.UpdateCurve(curvePoints, 32f, true);
                }

                if (gameObject.activeInHierarchy && CasinoAudio.Instance)
                {
                    CasinoAudio.Instance.PlayExplosion();
                }

                if (isActiveAndEnabled) StartCoroutine(ScreenShake());
                break;
        }
    }

    void OnActionButtonClicked()
    {
        if (CrashGameManager.Instance == null) return;

        if (CrashGameManager.Instance.currentState == CrashGameManager.GameState.Waiting)
        {
            if (CrashGameManager.Instance.PlaceBet(betAmount))
            {
                if (actionButtonText != null) actionButtonText.text = "BET PLACED";
                if (actionButton != null) actionButton.interactable = false;
            }
        }
        else if (CrashGameManager.Instance.currentState == CrashGameManager.GameState.Playing)
        {
            if (CrashGameManager.Instance.CashOut())
            {
                if (CasinoAudio.Instance) CasinoAudio.Instance.PlayWin();
                if (actionButtonText != null) actionButtonText.text = "CASHED OUT";
                if (actionButton != null) actionButton.interactable = false;
            }
        }
    }

    IEnumerator ScreenShake()
    {
        if (graphContainer == null || multiplierText == null) yield break;
        Vector3 origGraphPos = graphContainer.localPosition;
        Vector3 origTextPos = multiplierText.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            float intensity = (0.5f - elapsed) * 55f;
            graphContainer.localPosition = origGraphPos + (Vector3)Random.insideUnitCircle * intensity;
            multiplierText.transform.localPosition = origTextPos + (Vector3)Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        graphContainer.localPosition = origGraphPos;
        multiplierText.transform.localPosition = origTextPos;
    }
}
