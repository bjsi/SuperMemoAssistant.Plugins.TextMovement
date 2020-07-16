using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.TextMovement
{

  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
       IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
      "Cancel",
      IsCancel = true)]
  [DialogAction("save",
      "Save",
      IsDefault = true,
      Validates = true)]
  public class TextMovementCfg : CfgBase<TextMovementCfg>, INotifyPropertyChangedEx
  {
    [Title("Text Movement Plugin")]

    [Heading("By Jamesb | Experimental Learning")]

    [Heading("Features:")]
    [Text(@"- Adds operations for conveniently moving the cursor around text in SuperMemo HTML Components.")]

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "Text Movement Settings";
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
