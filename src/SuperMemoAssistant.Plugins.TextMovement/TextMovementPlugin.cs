#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   7/16/2020 11:44:59 PM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.TextMovement
{
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Windows.Input;
  using mshtml;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.HotKeys;
  using SuperMemoAssistant.Services.IO.Keyboard;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Services.UI.Configuration;
  using SuperMemoAssistant.Sys.IO.Devices;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class TextMovementPlugin : SentrySMAPluginBase<TextMovementPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public TextMovementPlugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "TextMovement";

    /// <inheritdoc />
    public override bool HasSettings => true;
    public TextMovementCfg Config;

    private const int MaxTextLength = 2000000000;

    #endregion

    /// <inheritdoc />
    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate(HotKeyManager.Instance, Config);
    }

    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<TextMovementCfg>() ?? new TextMovementCfg();
    }


    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {

      LoadConfig();

      // JUMP TO CLOZE
      Svc.HotKeyManager
      .RegisterGlobal(
        "JumpToCloze",
        "Jump to the next cloze marker in the current control",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.J, KeyModifiers.CtrlAlt),
        JumpToCloze
      )

      // JUMP TO NEXT SENTENCE
      .RegisterGlobal(
        "JumpToNextSentence",
        "Jump to the next sentence",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.Down, KeyModifiers.CtrlAlt),
        JumpToNextSentence
      )

      // JUMP TO PREVIOUS SENTENCE
      .RegisterGlobal(
        "JumpToPrevSentence",
        "Jump to the previous sentence",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.Up, KeyModifiers.CtrlAlt),
        JumpToPrevSentence
      );

    }

    /// <summary>
    /// Jump to the cloze marker
    /// </summary>
    private void JumpToCloze()
    {

      var htmlDoc = ContentUtils.GetFocusedHTMLDocument();
      var selObj = htmlDoc?.selection?.createRange() as IHTMLTxtRange;
      if (htmlDoc.IsNull() || selObj.IsNull())
        return;

      // Will work as long as there is only one cloze
      var cloze = htmlDoc.all
        ?.Cast<IHTMLElement>()
        ?.Where(x => x.tagName.ToLower() == "span")
        ?.Where(x => x.className.Contains("cloze"))
        .FirstOrDefault();

      if (cloze.IsNull())
        return;

      // TODO: Add options to select left[ inside ]right 
      // TODO: Add options to select []( inside )
      selObj.moveToElementText(cloze);
      selObj.moveStart("character", 1);
      selObj.collapse();
      selObj.select();
                    
    }

    /// <summary>
    /// Jump to the next sentence.
    /// </summary>
    private void JumpToNextSentence()
    {

      // Get selection and extend to the end
      var selObj = ContentUtils.GetTextSelectionObject();
      if (selObj.IsNull())
        return;

      selObj.moveEnd("character", MaxTextLength);

      var text = selObj.text;
      if (text.IsNullOrEmpty())
        return;

      int idx = text.IndexOf(". ");
      if (idx < 0)
        return;

      selObj.moveStart("character", idx);
      selObj.collapse();
      selObj.select();

    }

    /// <summary>
    /// Jump to the previous sentence.
    /// </summary>
    private void JumpToPrevSentence()
    {

      var selObj = ContentUtils.GetTextSelectionObject();
      if (selObj.IsNull())
        return;

      selObj.moveStart("character", -MaxTextLength);

      var text = selObj.text;
      if (text.IsNullOrEmpty())
        return;

      var reversed = text.Reverse().ToString();

      int idx = reversed.IndexOf(" .");
      if (idx < 0)
        return;

      selObj.moveEnd("character", -idx);
      selObj.collapse(false);
      selObj.select();

    }

    #endregion

    #region Methods

    #endregion
  }
}
