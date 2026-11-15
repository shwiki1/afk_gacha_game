using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerSelect : MonoBehaviour
{
    const string LocalhostServer = "ws://localhost:4001";
    const string AndroidServer = "ws://156.254.7.63:4001";

    readonly Dictionary<string, string> servers = new Dictionary<string, string>
    {
        {"LOCALHOST", LocalhostServer},
        {"AFK_SERVER", AndroidServer},
        {"EUROPE", "wss://central-europe-testing.curseofmirra.com"},
    };

    public static string Name { get; private set; }
    public static string Domain { get; private set; }

    [SerializeField]
    TMP_InputField customServerDomain;

    [SerializeField]
    TMP_Text serverButtonText;

    static ServerSelect()
    {
#if UNITY_EDITOR
        Name = "LOCALHOST";
        Domain = LocalhostServer;
#else
        Name = "AFK_SERVER";
        Domain = AndroidServer;
#endif
    }

    public static void EnsureDefaults()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Domain))
        {
#if UNITY_EDITOR
            Name = "LOCALHOST";
            Domain = LocalhostServer;
#else
            Name = "AFK_SERVER";
            Domain = AndroidServer;
#endif
        }
    }

    void Awake()
    {
        EnsureDefaults();
        UpdateServerButtonText();
    }

    public async void SelectServer(string domainName)
    {
        if (!servers.TryGetValue(domainName, out var domain))
        {
            Debug.LogError($"Unknown server selection: {domainName}");
            return;
        }

        ServerSelect.Name = domainName;
        ServerSelect.Domain = domain;
        UpdateServerButtonText();

        if (SocketConnection.Instance == null)
        {
            Debug.LogWarning("Cannot switch server before SocketConnection has initialized.");
            return;
        }

        await SocketConnection.Instance.CloseConnection();
        SocketConnection.Instance.Init();
    }

    public async void SelectCustomServer()
    {
        string customDomain = customServerDomain == null ? string.Empty : customServerDomain.text;
        if (!TryNormalizeDomain(customDomain, out var normalizedDomain))
        {
            Debug.LogError($"Invalid custom WebSocket domain: {customDomain}");
            return;
        }

        ServerSelect.Name = "CUSTOM";
        ServerSelect.Domain = normalizedDomain;

        if (SocketConnection.Instance == null)
        {
            Debug.LogWarning("Cannot switch server before SocketConnection has initialized.");
            return;
        }

        await SocketConnection.Instance.CloseConnection();
        SocketConnection.Instance.Init();
    }

    void UpdateServerButtonText()
    {
        if (serverButtonText != null)
        {
            serverButtonText.text = ServerSelect.Name;
        }
    }

    static bool TryNormalizeDomain(string value, out string normalizedDomain)
    {
        normalizedDomain = (value ?? string.Empty).Trim().TrimEnd('/');
        if (string.IsNullOrEmpty(normalizedDomain) ||
            !Uri.TryCreate(normalizedDomain, UriKind.Absolute, out var uri) ||
            (!string.Equals(uri.Scheme, "ws", StringComparison.OrdinalIgnoreCase) &&
             !string.Equals(uri.Scheme, "wss", StringComparison.OrdinalIgnoreCase)))
        {
            normalizedDomain = string.Empty;
            return false;
        }

        return true;
    }
}
