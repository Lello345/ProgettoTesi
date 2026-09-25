using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaModelHandler : MonoBehaviour
{
    [Header("Configurazione Ollama")]
    [SerializeField] private string ollamaUrl = "http://localhost:11434/api/generate";
    [SerializeField] private string modelName = "llama3.2GM"; // Es: llama3, phi3, gemma, qwen2

    // Struttura dati interna per mappare la risposta nativa del server di Ollama
    [System.Serializable]
    private class OllamaRawResponse
    {
        public string model;
        public string response; // Questo conterrà la stringa del NOSTRO JSON generato dall'IA
        public bool done;
    }

    // Struttura dati interna per inviare la richiesta a Ollama
    [System.Serializable]
    private class OllamaRequestPayload
    {
        public string model;
        public string prompt;
        public bool stream;
        public string format; // Utilizzato per forzare il formato "json"
    }

    private async void Start() {
        await Task.Delay(1500);
        WarmupModel();
    }


    public async Task<string> SendPromptAsync(string fullPrompt, bool forceJson = true)
    {
        // FEEDBACK: Segnala in console che la richiesta è partita
        Debug.Log($"[Ollama] Inviando richiesta... Prompt ({fullPrompt.Length} caratteri). Forzatura JSON: {forceJson}");
        
        // CRONOMETRO: Misura il tempo effettivo di risposta
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        OllamaRequestPayload payload = new OllamaRequestPayload
        {
            model = this.modelName,
            prompt = fullPrompt,
            stream = false 
        };

        if (forceJson)
        {
            payload.format = "json"; 
        }

        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            // TIMEOUT: Evita che l'operazione attenda all'infinito (es. 60 secondi massimi)
            request.timeout = 60;

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                // Nota: Qui puoi invocare un evento C# per mostrare un'animazione "NPC sta pensando..." sulla UI
                await Task.Yield(); 
            }

            stopwatch.Stop();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[Ollama] Risposta ricevuta con successo in {stopwatch.ElapsedMilliseconds} ms.");
                string rawServerResponse = request.downloadHandler.text;
                
                OllamaRawResponse ollamaJson = JsonUtility.FromJson<OllamaRawResponse>(rawServerResponse);
                return ollamaJson.response;
            }
            else
            {
                Debug.LogError($"[Ollama] Errore di connessione dopo {stopwatch.ElapsedMilliseconds} ms: {request.error}");
                return string.Empty;
            }
        }
    }

    public async void WarmupModel()
    {
        // Invia una richiesta dummy leggerissima per forzare il caricamento in VRAM
        OllamaRequestPayload payload = new OllamaRequestPayload
        {
            model = this.modelName,
            prompt = "ping",
            stream = false
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();
            Debug.Log("[Ollama] Modello caricato in VRAM e pronto!");
        }
    }
}
