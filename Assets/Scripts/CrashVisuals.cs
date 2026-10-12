using UnityEngine;

public static class CrashVisuals
{
    private static Sprite _rocketSprite;
    private static Sprite _flameSprite;
    private static Sprite _glowDotSprite;
    private static Sprite _ringSprite;

    public static Sprite GetRocketSprite()
    {
        if (_rocketSprite != null) return _rocketSprite;

        int width = 128;
        int height = 64;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] cols = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);
        for (int i = 0; i < cols.Length; i++) cols[i] = clear;

        float midY = 32f;

        for (int y = 0; y < height; y++)
        {
            float dy = y - midY;
            float absDy = Mathf.Abs(dy);

            for (int x = 0; x < width; x++)
            {
                Color pixelColor = clear;

                if (x >= 16 && x <= 56)
                {
                    float wingT = (x - 16f) / 40f;
                    float maxWingY = Mathf.Lerp(27f, 13f, wingT);
                    if (absDy >= 12f && absDy <= maxWingY)
                    {
                        float finT = (absDy - 12f) / (maxWingY - 12f);
                        if (finT > 0.85f)
                        {
                            pixelColor = new Color(0.0f, 0.95f, 0.9f, 0.95f);
                        }
                        else
                        {
                            pixelColor = new Color(0.13f, 0.17f, 0.24f, 0.95f);
                        }
                    }
                }

                float bodyHalfH = 0f;
                if (x >= 18 && x <= 68)
                {
                    bodyHalfH = 13.5f;
                    if (x < 24)
                    {
                        bodyHalfH = Mathf.Lerp(10f, 13.5f, (x - 18f) / 6f);
                    }
                }
                else if (x > 68 && x <= 114)
                {
                    float noseT = (x - 68f) / 46f;
                    bodyHalfH = 13.5f * Mathf.Sqrt(Mathf.Max(0f, 1f - noseT * noseT));
                }

                if (absDy <= bodyHalfH && bodyHalfH > 0f)
                {
                    float vertNorm = dy / 13.5f;

                    Color topShade = new Color(0.96f, 0.98f, 1.0f, 1f);
                    Color botShade = new Color(0.68f, 0.74f, 0.82f, 1f);
                    Color bodyBase = Color.Lerp(botShade, topShade, (vertNorm + 1f) * 0.5f);

                    if (dy > 4f && dy < 9f)
                    {
                        bodyBase = Color.Lerp(bodyBase, Color.white, 0.6f);
                    }

                    if (x >= 24 && x <= 72 && Mathf.Abs(dy - (-1f)) <= 2.2f)
                    {
                        bodyBase = new Color(0.0f, 0.95f, 0.65f, 1f);
                    }

                    if (x == 32 || x == 58)
                    {
                        bodyBase *= 0.75f;
                    }

                    if (x >= 66 && x <= 92 && dy >= 0.5f && dy <= bodyHalfH * 0.88f)
                    {
                        Color canopyTint = new Color(0.08f, 0.65f, 0.95f, 1f);
                        if (dy > bodyHalfH * 0.65f)
                        {
                            canopyTint = Color.Lerp(canopyTint, Color.white, 0.85f);
                        }
                        bodyBase = canopyTint;
                    }

                    if (x >= 110)
                    {
                        bodyBase = new Color(0.0f, 0.95f, 0.9f, 1f);
                    }

                    pixelColor = bodyBase;
                }

                if (x >= 8 && x < 18 && absDy <= 9f)
                {
                    float bellRadius = Mathf.Lerp(9f, 7f, (x - 8f) / 10f);
                    if (absDy <= bellRadius)
                    {
                        if (x < 12)
                        {
                            pixelColor = new Color(1.0f, 0.6f, 0.15f, 1f);
                        }
                        else
                        {
                            pixelColor = new Color(0.2f, 0.24f, 0.32f, 1f);
                        }
                    }
                }

                cols[y * width + x] = pixelColor;
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        _rocketSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        return _rocketSprite;
    }

    public static Sprite GetFlameSprite()
    {
        if (_flameSprite != null) return _flameSprite;

        int width = 96;
        int height = 48;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] cols = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);
        for (int i = 0; i < cols.Length; i++) cols[i] = clear;

        float midY = 24f;

        for (int y = 0; y < height; y++)
        {
            float dy = Mathf.Abs(y - midY);

            for (int x = 0; x < width; x++)
            {
                float u = (float)x / (width - 1);
                float maxH = 18f * Mathf.Pow(u, 0.7f);

                if (dy <= maxH && maxH > 0f)
                {
                    float distFromCenter = dy / maxH;
                    float alpha = (1f - distFromCenter * distFromCenter) * Mathf.Clamp01(u * 1.5f);

                    Color flameCol;
                    if (distFromCenter < 0.35f && u > 0.4f)
                    {
                        flameCol = Color.Lerp(new Color(1f, 0.95f, 0.5f, alpha), Color.white, 0.8f);
                    }
                    else if (distFromCenter < 0.7f)
                    {
                        flameCol = new Color(1.0f, 0.55f, 0.05f, alpha * 0.95f);
                    }
                    else
                    {
                        flameCol = new Color(0.0f, 0.85f, 1.0f, alpha * 0.65f);
                    }

                    cols[y * width + x] = flameCol;
                }
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        _flameSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(1.0f, 0.5f), 100f);
        return _flameSprite;
    }

    public static Sprite GetGlowDotSprite()
    {
        if (_glowDotSprite != null) return _glowDotSprite;

        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] cols = new Color[size * size];
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - radius + 0.5f;
                float dy = y - radius + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float norm = Mathf.Clamp01(dist / radius);
                float alpha = Mathf.Pow(1f - norm, 2.2f);
                cols[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        _glowDotSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return _glowDotSprite;
    }

    public static Sprite GetShockwaveRingSprite()
    {
        if (_ringSprite != null) return _ringSprite;

        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] cols = new Color[size * size];
        float radius = size * 0.5f;
        float ringRadius = radius * 0.75f;
        float ringThickness = 6f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - radius + 0.5f;
                float dy = y - radius + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float distFromRing = Mathf.Abs(dist - ringRadius);

                float alpha = Mathf.Clamp01(1f - (distFromRing / ringThickness));
                alpha = Mathf.Pow(alpha, 1.8f);
                cols[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        _ringSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return _ringSprite;
    }
}
