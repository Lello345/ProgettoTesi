using UnityEngine;

[ExecuteAlways] // Funziona anche nell'editor
public class DebugFlipTracker : MonoBehaviour
{
    private SpriteRenderer sr;
    private bool lastFlipState;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) lastFlipState = sr.flipX;
    }

    void Update()
    {
        if (sr != null && sr.flipX != lastFlipState)
        {
            lastFlipState = sr.flipX;
            
            // Stampa esattamente chi o cosa ha cambiato il FlipX
            Debug.LogError($"[FLIP DETECTED] FlipX è cambiato in: {sr.flipX}\nStack Trace:\n{System.Environment.StackTrace}");
            Debug.Break();
        }
    }
}
