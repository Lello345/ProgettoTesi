using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class OllamaServerManager : MonoBehaviour
{
    [SerializeField] private string ollamaHealthUrl = "http://localhost:11434/";
    [SerializeField] private bool killOnQuit = true;

    private Process _ollamaProcess;
    public static bool IsServerReady { get; private set; } = false;

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await EnsureOllamaServerRunningAsync();
    }

    private async Task EnsureOllamaServerRunningAsync()
    {
        // 1. Controlla se Ollama risponde già sulla porta 11434 (es. avviato da tray bar o altro)
        if (await CheckHealthAsync())
        {
            Debug.Log("[OllamaServerManager] Server Ollama già attivo e pronto sulla porta 11434.");
            IsServerReady = true;
            return;
        }

        // 2. Se non risponde, avvia il processo da C#
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "serve",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = false, // Impostati a false per evitare il deadlock del buffer
                RedirectStandardError = false
            };

            _ollamaProcess = new Process { StartInfo = startInfo };
            _ollamaProcess.Start();

            Debug.Log("[OllamaServerManager] Avvio del processo Ollama in corso...");

            // 3. Attesa attiva (Poll) finché la porta 11434 non risponde
            int maxRetries = 10;
            while (maxRetries > 0)
            {
                await Task.Delay(1000); // Aspetta 1 secondo tra i tentativi
                if (await CheckHealthAsync())
                {
                    Debug.Log("[OllamaServerManager] Server Ollama avviato e connessione HTTP stabilita!");
                    IsServerReady = true;
                    return;
                }
                maxRetries--;
            }

            Debug.LogError("[OllamaServerManager] Timeout: Il processo Ollama è partito ma la porta 11434 non risponde.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[OllamaServerManager] Errore durante l'avvio del processo: {e.Message}");
        }
    }

    private async Task<bool> CheckHealthAsync()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(ollamaHealthUrl))
        {
            var op = req.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            return req.result == UnityWebRequest.Result.Success;
        }
    }

    private void OnApplicationQuit()
    {
        if (killOnQuit && _ollamaProcess != null && !_ollamaProcess.HasExited)
        {
            try
            {
                _ollamaProcess.Kill();
                _ollamaProcess.Dispose();
                Debug.Log("[OllamaServerManager] Processo Ollama terminato.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[OllamaServerManager] Errore durante la chiusura: {e.Message}");
            }
        }
    }
}