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
        Debug.Log("[OllamaModelHandler] Start() avviato correttamente.");
    
        await Task.Delay(1500);
        WarmupModel();

        // TEST DIRETTISSIMO
        string testResponse = await SendPromptAsync("Ciao, chi sei?", forceJson: false);
        Debug.Log($"[OllamaTest] Risposta ricevuta: {testResponse}");
    }

    /*
    /// <summary>
    /// Invia il prompt a Ollama in modo asincrono e restituisce la stringa JSON "pulita" dell'NPC.
    /// </summary>
    public async Task<string> SendPromptAsync(string fullPrompt, bool forceJson = true)
    {
        // 1. Prepariamo il payload della richiesta
        OllamaRequestPayload payload = new OllamaRequestPayload
        {
            model = this.modelName,
            prompt = fullPrompt,
            stream = false // Disattiviamo lo streaming per avere la risposta completa in un colpo solo
        };

        if (forceJson)
        {
            payload.format = "json"; // Dice nativamente a Ollama di costringere il modello a sputare JSON
        }

        string jsonPayload = JsonUtility.ToJson(payload);

        // 2. Configuriamo la Web Request HTTP POST
        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 3. Inviamo la richiesta e attendiamo senza bloccare il framerate di Unity
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // Rilascia il controllo a Unity fino al frame successivo
            }

            // 4. Gestione del risultato
            if (request.result == UnityWebRequest.Result.Success)
            {
                string rawServerResponse = request.downloadHandler.text;
                
                // Effettuiamo il primo parsing: scompattiamo il JSON di Ollama
                OllamaRawResponse ollamaJson = JsonUtility.FromJson<OllamaRawResponse>(rawServerResponse);
                
                // Ritorniamo il contenuto di "response", che è il nostro JSON con battuta e scelte
                return ollamaJson.response;
            }
            else
            {
                Debug.LogError($"[OllamaModelHandler] Errore di connessione: {request.error}");
                return string.Empty;
            }
        }
    }
    */

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
