using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Dock.Model.Avalonia.Controls;
using Dock.Model.Avalonia.Core;
using Dock.Model.Avalonia.Internal;
using Dock.Model.Controls;

namespace Stride.GameStudio.Avalonia.AssetsEditors;

/// <summary>
/// Document dock.
/// </summary>
public class LayoutAnchorablePane : DockBase, IDocumentDock
{
    /// <summary>
    /// Defines the <see cref="CanCreateDocument"/> property.
    /// </summary>
    public static readonly DirectProperty<DocumentDock, bool> CanCreateDocumentProperty = AvaloniaProperty.RegisterDirect<DocumentDock, bool>(nameof(CanCreateDocument), o => o.CanCreateDocument, (o, v) => o.CanCreateDocument = v);

    /// <summary>
    /// Defines the <see cref="DocumentTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDocumentTemplate?> DocumentTemplateProperty =
        AvaloniaProperty.Register<DocumentDock, IDocumentTemplate?>(nameof(DocumentTemplate));

    private bool _canCreateDocument;

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    [JsonPropertyName("CanCreateDocument")]
    public bool CanCreateDocument
    {
        get => _canCreateDocument;
        set => SetAndRaise(CanCreateDocumentProperty, ref _canCreateDocument, value);
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    [JsonIgnore]
    public ICommand? CreateDocument { get; set; }

    /// <summary>
    /// Gets or sets document template.
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public IDocumentTemplate? DocumentTemplate
    {
        get => GetValue(DocumentTemplateProperty);
        set => SetValue(DocumentTemplateProperty, value);
    }

    public LayoutAnchorablePane()
    {
    }

}
