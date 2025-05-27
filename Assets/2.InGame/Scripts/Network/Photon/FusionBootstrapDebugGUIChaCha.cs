using UnityEngine.Serialization;

namespace Fusion {
  using System;
  using UnityEngine;
  using System.Collections.Generic;

  /// <summary>
  /// Companion component for <see cref="FusionBootstrapCustom"/>. Automatically added as needed for rendering in-game networking IMGUI.
  /// </summary>
  [RequireComponent(typeof(FusionBootstrapCustom))]
  [ScriptHelp(BackColor = ScriptHeaderBackColor.Steel)]
  public class FusionBootstrapDebugGUIChaCha : Fusion.Behaviour {
    /// <summary>
    /// The GUISkin to use as the base for the scalable in-game UI.
    /// </summary>
    FusionBootstrapCustom _networkDebugStart;
    string _clientCount;
    bool _isMultiplePeerMode;

    Dictionary<FusionBootstrapCustom.Stage, string> _nicifiedStageNames;

#if UNITY_EDITOR

    protected virtual void Reset() {
      _networkDebugStart = EnsureNetworkDebugStartExists();
      _clientCount = _networkDebugStart.AutoClients.ToString();
    }

#endif

    protected virtual void OnValidate() {
      ValidateClientCount();
    }

    protected void ValidateClientCount() {
      if (_clientCount == null) {
        _clientCount = "1";
      } else {
        _clientCount = System.Text.RegularExpressions.Regex.Replace(_clientCount, "[^0-9]", "");
      }
    }
    protected int GetClientCount() {
      try {
        return Convert.ToInt32(_clientCount);
      } catch {
        return 0;
      }
    }

    protected virtual void Awake() {

      _nicifiedStageNames = ConvertEnumToNicifiedNameLookup<FusionBootstrapCustom.Stage>("Fusion Status: ");
      _networkDebugStart = EnsureNetworkDebugStartExists();
      _clientCount = _networkDebugStart.AutoClients.ToString();
      ValidateClientCount();
    }
    protected virtual void Start() {
      _isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
    }

    protected FusionBootstrapCustom EnsureNetworkDebugStartExists() {
      if (_networkDebugStart) {
        if (_networkDebugStart.gameObject == gameObject)
          return _networkDebugStart;
      }

      if (TryGetBehaviour<FusionBootstrapCustom>(out var found)) {
        _networkDebugStart = found;
        return found;
      }

      _networkDebugStart = AddBehaviour<FusionBootstrapCustom>();
      return _networkDebugStart;
    }

    private void Update() {

      var nds = EnsureNetworkDebugStartExists();
      if (!nds.ShouldShowGUI) {
        return;
      }

      var currentstage = nds.CurrentStage;
      if (currentstage != FusionBootstrapCustom.Stage.Disconnected) {
        return;
      }
    }
    public void StartGame(String roomName)
    {
      var nds = EnsureNetworkDebugStartExists();
      nds.DefaultRoomName = roomName;
      if (_isMultiplePeerMode) { 
        StartMultipleSharedClients(nds); 
      } else {
          nds.StartSharedClient(); 
      }
    }

    private void StartMultipleSharedClients(FusionBootstrapCustom nds) {
      int count;
      try {
        count = Convert.ToInt32(_clientCount);
      } catch {
        count = 0;
      }
      nds.StartMultipleSharedClients(count);
    }

    // TODO Move to a utility
    public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null, Dictionary<T, string> nonalloc = null) where T : System.Enum {

      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      if (nonalloc == null) {
        nonalloc = new Dictionary<T, string>();
      } else {
        nonalloc.Clear();
      }

      var names = Enum.GetNames(typeof(T));
      var values = Enum.GetValues(typeof(T));
      for (int i = 0, cnt = names.Length; i < cnt; ++i) {
        sb.Clear();
        if (prefix != null) {
          sb.Append(prefix);
        }
        var name = names[i];
        for (int n = 0; n < name.Length; n++) {
          // If this character is a capital and it is not the first character add a space.
          // This is because we don't want a space before the word has even begun.
          if (char.IsUpper(name[n]) == true && n != 0) {
            sb.Append(" ");
          }

          // Add the character to our new string
          sb.Append(name[n]);
        }
        nonalloc.Add((T)values.GetValue(i), sb.ToString());
      }
      return nonalloc;
    }
#if UNITY_EDITOR

    public static T GetAsset<T>(string Guid) where T : UnityEngine.Object {
      var path = UnityEditor.AssetDatabase.GUIDToAssetPath(Guid);
      if (string.IsNullOrEmpty(path)) {
        return null;
      } else {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
      }
    }
#endif
  }
}