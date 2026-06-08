using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaModelHandler : MonoBehaviour
{
    [Header("Configurazione Ollama")]
    [SerializeField] private string ollamaUrl = "http://localhost:11434/api/generate";
    [SerializeField] private string modelName = "llama3"; // Es: llama3, phi3, gemma, qwen2

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
}
