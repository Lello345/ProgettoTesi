using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class OllamaAutoStart : MonoBehaviour
{
    private Process _ollamaProcess;

    private void Start()
    {
        StartOllamaServer();
    }

    private void StartOllamaServer()
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "serve",
                UseShellExecute = false,
                CreateNoWindow = true, // Esegue in background senza aprire la finestra di terminale
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _ollamaProcess = Process.Start(startInfo);
            Debug.Log("[Ollama] Comando 'ollama serve' avviato da Unity.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Ollama] Errore durante l'avvio del processo: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        // Pulisci il processo alla chiusura dell'applicazione se necessario
        if (_ollamaProcess != null && !_ollamaProcess.HasExited)
        {
            _ollamaProcess.Kill();
            _ollamaProcess.Dispose();
        }
    }
}

