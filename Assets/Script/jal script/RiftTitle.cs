using System.Collections;
using UnityEngine;
using TMPro;

// Title that glitches through eras in short bursts
public class RiftTitle : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text redGhost;          // copies of the title drawn behind it
    public TMP_Text cyanGhost;
    public TMP_FontAsset[] eraFonts;   // optional: fonts to flicker through
    public float minWait = 2f;         // time between glitch bursts
    public float maxWait = 5f;
    public float burstTime = 0.35f;
    public float jitter = 6f;          // how far letters jump, in pixels
    public float ghostOffset = 8f;     // how far the red/cyan copies split

    TMP_FontAsset baseFont;
    Vector2 basePos;

    void Start()
    {
        baseFont = title.font;
        basePos = title.rectTransform.anchoredPosition;
        StartCoroutine(GlitchLoop());
    }

    // Realtime waits so the title keeps glitching while a menu panel pauses the game
    IEnumerator GlitchLoop()
    {
        while (true)
        {
            SetGhosts(0f);
            yield return new WaitForSecondsRealtime(Random.Range(minWait, maxWait));

            for (float t = 0f; t < burstTime; t += Time.unscaledDeltaTime)
            {
                if (eraFonts.Length > 0 && Random.value < 0.3f)
                    SetFont(eraFonts[Random.Range(0, eraFonts.Length)]);

                SetGhosts(Random.Range(-ghostOffset, ghostOffset));
                Jitter();
                yield return null;
            }

            SetFont(baseFont);
            title.ForceMeshUpdate(); // snap letters back into place
        }
    }

    // Randomly nudges some letters by moving their mesh vertices
    void Jitter()
    {
        title.ForceMeshUpdate();
        TMP_TextInfo info = title.textInfo;

        for (int i = 0; i < info.characterCount; i++)
        {
            TMP_CharacterInfo c = info.characterInfo[i];
            if (!c.isVisible || Random.value > 0.3f) continue; // only some letters jump

            Vector3 offset = new Vector3(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter), 0f);
            Vector3[] verts = info.meshInfo[c.materialReferenceIndex].vertices;
            for (int v = 0; v < 4; v++) verts[c.vertexIndex + v] += offset;
        }

        title.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    void SetFont(TMP_FontAsset f)
    {
        title.font = f;
        redGhost.font = f;
        cyanGhost.font = f;
    }

    void SetGhosts(float x)
    {
        redGhost.rectTransform.anchoredPosition = basePos + new Vector2(x, 0f);
        cyanGhost.rectTransform.anchoredPosition = basePos - new Vector2(x, 0f);
    }
}
