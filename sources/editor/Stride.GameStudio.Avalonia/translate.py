import os
import re
import regex # for more interesting regex operations.
import diff_match_patch

sourcesDir = "../../../sources/"
generatedDir = "../../../generated"

def readSource (sourceFile):

  fle = open (sourceFile, "r")
  contents = fle.read ()
  fle.close ()
  return contents

def writeDest (contents, generatedFile, generatedDiffFile, destFile):
    # write translated file.
  os.makedirs (os.path.dirname (generatedFile), exist_ok = True)
  fle = open (generatedFile, "w")
  fle.write (contents)
  fle.close ()
  
  # Now we try to preserve any changes that have been made.
  # Either the destFile already exists, and there is no diff - so generate a diff 
  # OR
  # No destFile exists and there is a diff - so patch to create the dest.
  # OR
  # Both destFile and diff exist - warn the user to remove one or the other.
  # OR
  # Neither exist, just copy generated file directly to destination.
  if os.path.isfile (destFile):
    if os.path.isfile (generatedDiffFile):
      print ("Both destination and diff file exist. Remove one to have it regenerated.")
      print ("  Remove the destination file to have it regenerated in response to third party updates in the repo")
      print ("  Remove the diff file to discard local changes to the class e.g., if translation filters change.")
      print ()
      print ("To remove destination file:")
      print ("  rm " + destFile)
      print ()
      print ("To remove diff file:")
      print ("  rm " + generatedDiffFile)
      print ()
      exit (0)
    else:
      # No diff, generate.
      fle = open (destFile, "r")
      destcontents = fle.read ()
      fle.close ()
      dmp = diff_match_patch.diff_match_patch ()
      dmp.Patch_Margin = 100
      patches = dmp.patch_make (contents, destcontents)
      diff = dmp.patch_toText (patches)
      if len (diff) > 0:
        print ("Destination file already exists, generating diff file: " + generatedDiffFile)
        os.makedirs (os.path.dirname (generatedDiffFile), exist_ok = True)
        fle = open (generatedDiffFile, "w")
        fle.writelines (diff)
        fle.close ()
  else:
    if os.path.isfile (generatedDiffFile):
      # No destination file, generate from diff.
      print ("Creating destination file " + destFile + " from translation and an existing diff")
      fle = open (generatedDiffFile, "r")
      diffcontents = fle.read ()
      fle.close ()
      dmp = diff_match_patch.diff_match_patch ()
      patches = dmp.patch_fromText (diffcontents)
      new_text, check = dmp.patch_apply(patches, contents)
      for f in check:
        if not f:
          print ("Patch failed")
      fle = open (destFile, "w")
      fle.writelines (new_text)
      fle.close ()
    else:
      # Neither file exists, just write directly to destination.
      print ("Creating destination file " + destFile + " by translation")
      os.makedirs (os.path.dirname (destFile), exist_ok = True)
      fle = open (destFile, "w")
      fle.writelines (contents)
      fle.close ()


# Replace the using statements.
def translateHeaders (contents):

  headers = ""
  if re.findall ("DataTemplate", contents):
    headers += "using Avalonia.Markup.Xaml.Templates;\n"
  if re.findall ("RoutedEvent", contents):
    headers += "using Avalonia.Interactivity;\n"
  if re.findall ("OnApplyTemplate", contents):
    headers += "using Avalonia.Controls.Primitives;\n"
  if re.findall ("BitmapScalingMode", contents):
    headers += "using Avalonia.Media.Imaging;\n"
  if re.findall ("TextRange", contents):
    headers += "using Avalonia.Media.TextFormatting;\n"
  if re.findall ("EventHandler", contents):
    headers += "using System;\n"
  if re.findall ("VisualTreeHelper", contents):
    headers += "using Avalonia.VisualTree;\n"
  if re.findall ("LogicalTreeHelper", contents):
    headers += "using Avalonia.LogicalTree;\n"
  if re.findall ("Convert", contents):
    headers += "using System.Collections.Generic;\n"
  if re.findall ("TextTrimming", contents):
    headers += "using Avalonia.Media;\n"
  if re.findall ("BindsTwoWayByDefault", contents):
    headers += "using Avalonia.Data;\n"
  if re.findall ("FindVisualChildrenOfType", contents):
    headers += "using System.Linq;\n"
  if re.findall ("Animation", contents):
    headers += "using Avalonia.Animation;\n"
  if re.findall ("BitmapSource", contents):
    headers += "using System.Runtime.InteropServices;\nusing Avalonia.Platform;\n"
  if re.findall ("OnIsKeyboardFocusWithinChanged", contents):
    headers += "using Avalonia.Input;\n"
  if re.findall ("TemplatePart", contents):
    headers += "using Avalonia.Controls.Metadata;\n"
  if re.findall ("DataTemplateSelector", contents):
    headers += "using Avalonia.Controls.Templates;\n"
  if re.findall ("Orientation", contents):
    headers += "using Avalonia.Layout;\n"
  if re.findall ("OnItemsChanged", contents):
    headers += "using System.Collections.Generic;\nusing System.Collections.Specialized;\n"
  if re.findall ("OnItemsSourceChanged", contents):
    headers += "using System.Collections.Generic;\nusing System.Collections.Specialized;\n"
  if re.findall ("ItemsPresenter", contents):
    headers += "using Avalonia.Controls.Presenters;\n"
  if re.findall ("ICommand", contents):
    headers += "using System.Windows.Input;\n"
  if re.findall ("OnLostKeyboardFocus", contents):
    headers += "using Avalonia.Interactivity;\n"
  if re.findall ("DependsOn", contents):
    headers += "using Avalonia.Metadata;\n"
  if re.findall ("FlowDirection", contents):
    headers += "using Avalonia.Media;\n"
  if re.findall ("DragEventArgs", contents):
    headers += "using Avalonia.Media;\n"
  if re.findall (" Style ", contents):
    headers += "using Avalonia.Styling;\n"
  if re.findall ("PropertyPath", contents):
    headers += "using Avalonia.Data.Core;\n"

  contents = re.sub ("using System.Windows.Controls.Primitives;", "using Avalonia.Controls.Primitives;", contents)
  contents = re.sub ("using System.Windows.Controls;", "using Avalonia;\nusing Avalonia.Controls;\nusing Avalonia.Controls.Metadata;", contents)
  contents = re.sub ("using System.Windows.Shapes;", "using Avalonia.Controls.Shapes;", contents)
  contents = re.sub ("using System.Windows.Data;", "using Avalonia.Data;\nusing Avalonia.Data.Converters;", contents)
  contents = re.sub ("using System.Windows.Markup;", "using Avalonia.Markup.Xaml;", contents)
  contents = re.sub ("using System.Windows.Threading;", "using Avalonia.Threading;", contents)
  contents = re.sub ("using System.Windows.Input;", "using Avalonia.Input;", contents)
  contents = re.sub ("using System.Windows.Documents;", "using Avalonia.Controls.Documents;", contents)
  contents = re.sub ("using System.Windows.Media.Animation;", "using Avalonia.Animation;", contents)
  contents = re.sub ("using System.Windows.Media.Imaging;", "using Avalonia.Media.Imaging;", contents)
  contents = re.sub ("using System.Windows.Media;", "using Avalonia.Media;", contents)
  contents = re.sub ("using Microsoft.Xaml.Behaviors;", "using Avalonia.Xaml.Interactivity;", contents)
  contents = re.sub ("using System.Windows.Interop;", "", contents)
  contents = re.sub ("using System.Windows;", "using Avalonia;\nusing Avalonia.Controls;\n", contents)
  contents = re.sub ("using System.Xaml;", "", contents)
  
  if re.findall ("RoutedCommand", contents):
    headers += "using System.Windows.Input;\n"
  
  
  if len (headers) > 0:
    contents = re.sub ("\nnamespace", headers + "\nnamespace", contents)
  
  # Remove internal, hopefully avoid boxing.
  contents = re.sub ("using Stride.Core.Presentation.Internal;", "", contents)

  contents = re.sub ("using Point = System.Windows.Point;", "using Point = Avalonia.Point;", contents)
  contents = re.sub ("using Rectangle = System.Windows.Shapes.Rectangle;", "using Rectangle = Avalonia.Controls.Shapes.Rectangle;", contents)
  
  return contents

def translateAnnotations (contents):
  contents = re.sub ("\[MarkupExtensionReturnType\(.*\)\]", "", contents)
  contents = re.sub ("\[ValueConversion\(.*\)\]", "", contents)
  return contents

def translateNames (contents):
  #contents = re.sub ("DependencyProperty", "AvaloniaProperty", contents)
  
  # Suspended, have added RoutedCommand.
  #contents = re.sub ("ICommand ", "ICommandSource ", contents)
  #contents = re.sub ("<ICommand>", "<ICommandSource>", contents)
  #contents = re.sub (", ICommand>", ", ICommandSource>", contents)
  #contents = re.sub ("\(ICommand\)", "(ICommandSource)", contents)
  #contents = re.sub ("as ICommand;", " as ICommandSource;", contents)
  #contents = re.sub ("RoutedCommand", "ICommandSource", contents) # provisional.
  #contents = re.sub ("Command\.CanExecuteChanged \-\=", "Command.Command.CanExecuteChanged -=", contents) # provisional.
  #contents = re.sub ("Command\.CanExecuteChanged \+\=", "Command.Command.CanExecuteChanged +=", contents) # provisional.
  #contents = re.sub ("\.CanExecute\(", ".Command.CanExecute(", contents) # provisional.
  #contents = re.sub ("\.Execute\(", ".Command.Execute(", contents) # provisional.

  # Convert:
  #          public static RoutedCommand CollapseAllFolders { get; } = new RoutedCommand(nameof(CollapseAllFolders), typeof(TreeView));
  #          private static void OnCollapseAllFolders(object sender, RoutedEventArgs e)
  # to:
  #          public static RoutedCommand<TreeView> CollapseAllFolders { get; } = new RoutedCommand<TreeView>(OnCollapseAllFolders);
  #          private static void OnCollapseAllFolders(TreeView sender)

  contents = re.sub ("public static RoutedCommand ", "public static ICommand ", contents)
  
  #contents = re.sub ("RoutedDependencyPropertyChangedEventHandler", "PropertyChangedEventHandler", contents) # provisional.
  contents = re.sub ("static DependencyProperty", "static AvaloniaProperty", contents) # provisional.
  contents = re.sub ("DependencyProperty property", "AvaloniaProperty property", contents) # provisional.
  contents = re.sub ("DependencyProperty dependencyProperty", "AvaloniaProperty dependencyProperty", contents) # provisional.
  contents = re.sub ("\(DependencyProperty\)", "(AvaloniaProperty)", contents)
  contents = re.sub ("<DependencyProperty>", "<AvaloniaProperty>", contents)
  contents = re.sub (" DependencyProperty ", " AvaloniaProperty ", contents)
  contents = re.sub ("\(DependencyProperty dependencyProperty\)", "(AvaloniaProperty dependencyProperty)", contents)
  
  contents = re.sub ("CancelRoutedEventHandler", "EventHandler<CancelRoutedEventArgs>", contents)
  contents = re.sub ("RoutedPropertyChangedEventHandler<double>", "EventHandler<RoutedEventArgs>", contents)
  #contents = re.sub ("ValidationRoutedEventHandler<string>", "EventHandler<CancelRoutedEventArgs>", contents) # provisional - wrong
  #contents = re.sub ("ValidationRoutedEventArgs<string> e", "RoutedEventArgs e", contents) # provisional - this class now translated.
  contents = re.sub ("ExecutedRoutedEventArgs", "RoutedEventArgs", contents)
  contents = re.sub ("CanExecuteRoutedEventArgs", "RoutedEventArgs", contents)
  contents = re.sub (" RoutedEventHandler ", " EventHandler<RoutedEventArgs> ", contents)
  contents = re.sub (" DependencyPropertyChangedEventArgs e", "AvaloniaPropertyChangedEventArgs e", contents)
  contents = re.sub (" DependencyPropertyChangedEventArgs args", "AvaloniaPropertyChangedEventArgs args", contents)
  contents = re.sub ("private void OnValueChanged\(object sender, RoutedPropertyChangedEventArgs<double> e\)", "private void OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)", contents)
  contents = re.sub ("\(RoutedEventHandler\)", "(EventHandler)", contents)
  
  
  contents = re.sub ("System.Windows.Controls.TextBox", "Avalonia.Controls.TextBox", contents)
  contents = re.sub ("System.Windows.Data.MultiBinding", "Avalonia.Data.MultiBinding", contents)
  contents = re.sub ("System.Windows.Media", "Avalonia.Media", contents)
  contents = re.sub (" DependencyObject ", " AvaloniaObject ", contents)
  contents = re.sub ("\(DependencyObject ", "(AvaloniaObject ", contents)
  contents = re.sub ("<DependencyObject,", "<AvaloniaObject,", contents)
  contents = re.sub (" DependencyObject>", " AvaloniaObject>", contents)
  contents = re.sub ("<DependencyObject>", "<AvaloniaObject>", contents)
  contents = re.sub (": DependencyObject", ": AvaloniaObject", contents)
  contents = re.sub ("as DependencyObject", "as AvaloniaObject", contents)
  contents = re.sub ("private DependencyProperty", "private AvaloniaProperty", contents)
  contents = re.sub ("DependencyProperty.UnsetValue", "AvaloniaProperty.UnsetValue", contents)
  contents = re.sub ("\.ProvideValue\(.*\)", "", contents) # is this true in general?
  contents = re.sub ("\(FrameworkElement ", "(Control ", contents)
  contents = re.sub (" FrameworkElement;", " Control;", contents)
  contents = re.sub ("<FrameworkElement", "<Control", contents) 
  contents = re.sub (", FrameworkElement>", ", Control>", contents) 
  contents = re.sub (" FrameworkElement ", " Control ", contents) 
  contents = re.sub (": FrameworkElement", ": Control", contents)
  contents = re.sub ("typeof\(FrameworkElement\)", "typeof(Control)", contents)
  contents = re.sub ("private FrameworkElement", "private Control", contents)
  contents = re.sub ("UIElement\.", "Control.", contents) 
  contents = re.sub (" UIElement ", " Control ", contents) 
  contents = re.sub ("<UIElement>", "<Control>", contents) 
  contents = re.sub ("\(UIElement ", "(Control ", contents) 
  contents = re.sub (": UIElement", ": Control", contents)
  contents = re.sub ("as UIElement", "as Control", contents)
  contents = re.sub ("\(UIElement\)", "(Control)", contents)
  contents = re.sub ("EventManager.GetRoutedEvents\(\)", "RoutedEventRegistry.Instance.GetAllRegistered ()", contents)

  contents = re.sub ("Keyboard.Focus\(this\);", "this.Focus ();", contents)
  contents = re.sub ("Keyboard.FocusedElement", "TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement ()", contents)

  contents = re.sub ("Application.Current.TryFindResource\((.*?)\)", r"ResourceNodeExtensions.FindResource(Application.Current, \1)", contents)

  contents = re.sub ("Cursors.SizeWE", "new Cursor(StandardCursorType.SizeWestEast)", contents)
  contents = re.sub ("Cursors.ScrollAll", "new Cursor(StandardCursorType.SizeAll)", contents) # no equivalent.

  contents = re.sub (" ImageSource ", " IImage ", contents)
  contents = re.sub ("\(ImageSource\)", "(IImage)", contents)
  contents = re.sub (" ImageSource;", " IImage;", contents)
  contents = re.sub ("<ImageSource>", "<IImage>", contents)
  contents = re.sub (" ImageSource>", " IImage>", contents)
  contents = re.sub ("\(ImageSource ", "(IImage ", contents)
  contents = re.sub ("RenderOptions\.BitmapScalingMode=\"Linear\"", r'RenderOptions.BitmapInterpolationMode="MediumQuality"', contents)
  contents = re.sub (" BitmapScalingMode ", r' BitmapInterpolationMode ', contents)
  contents = re.sub ("BitmapScalingMode.Unspecified", r' BitmapInterpolationMode.Unspecified', contents)
  contents = re.sub ("RenderOptions.SetBitmapScalingMode", r' RenderOptions.SetBitmapInterpolationMode', contents)
  contents = re.sub ("<Setter Property=\"RenderOptions\.BitmapScalingMode\" Value=\"NearestNeighbor\" />", r'<Setter Property="RenderOptions.BitmapInterpolationMode" Value="LowQuality" />', contents)

  contents = re.sub ("colorPickerRenderSurface\.Fill = new DrawingBrush\(new ImageDrawing\(BitmapSource.Create\(width, height, 96, 96, pf, null, rawImage, rawStride\), new Rect\(0.0f, 0.0f, width, height\)\)\);", r'int size = Marshal.SizeOf(rawImage[0]) * rawImage.Length;\n\t\t\t\t\tIntPtr pnt = Marshal.AllocHGlobal(size);\n\t\t\t\t\tMarshal.Copy(rawImage, 0, pnt, rawImage.Length);\n\t\t\t\t\tcolorPickerRenderSurface.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(pf, AlphaFormat.Premul, pnt, new PixelSize (width, height), new Vector (96, 96), rawStride));\n\t\t\t\t\tMarshal.FreeHGlobal(pnt);', contents)


  contents = re.sub ("= (.*?)\.FindVisualChildOfType\<(.*?)\>\(\);", r'= global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<\2>(\1);', contents)
  contents = re.sub ("!(.*?)\.FindVisualChildOfType\<(.*?)\>\(\)", r'!global::Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<\2>(\1)', contents)
  contents = re.sub ("= (.*?)\.FindVisualChildrenOfType\<(.*?)\>\(\)", r'= global::Avalonia.VisualTree.VisualExtensions.GetVisualChildren(\1).Where (x => x is \2).Select (x => (\2) x)', contents)
  contents = re.sub ("\?\? (.*?)\.FindVisualParentOfType\<(.*?)\>\(\);", r'?? global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<\2>((Visual) \1);', contents)
  contents = re.sub ("\(([a-zA-Z]*?)\.FindVisualParentOfType\<(.*?)\>\(\),", r'(global::Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<\2>((Visual) \1),', contents)
  contents = re.sub ("LogicalTreeHelper\.GetChildren", r'LogicalExtensions.GetLogicalChildren', contents)
  contents = re.sub ("LogicalTreeHelper\.GetParent", r'LogicalExtensions.GetLogicalParent', contents)
  contents = re.sub ("VisualTreeHelper\.GetParent", r'global::Avalonia.VisualTree.VisualExtensions.GetVisualParent', contents)

  contents = re.sub ("<ButtonBase>", "<Button>", contents)
  contents = re.sub ("\(ButtonBase\)", "(Button)", contents)
  contents = re.sub ("as ButtonBase;", "as Button;", contents)

  contents = re.sub ("StyledProperty<Brush>", "StyledProperty<IBrush>", contents)
  contents = re.sub (", Brush>", ", IBrush>", contents)

  contents = re.sub ("PixelFormats.Bgr32", "PixelFormats.Bgra8888", contents)

  contents = re.sub ("Window\.GetWindow\((.*?)\)", r"TopLevel.GetTopLevel(\1 as Control) as Window", contents)

  # RichTextBox - provisional fix.
  contents = re.sub ("RichTextBox", r"Avalonia.Controls.TextBox", contents)
  contents = re.sub ("new FlowDocument\(new Paragraph\(\)\)", r'new string("")', contents)
  contents = re.sub ("FlowDocumentScrollViewer", r"ScrollViewer", contents)
  contents = re.sub ("FlowDocument", r"string", contents)
  contents = re.sub ("TextBox\.Document", r"TextBox.Text", contents)

  contents = re.sub ("GetTemplateChild\(\"(.*?)\"\) as (.*?);", r'e.NameScope.Find<\2>("\1");', contents)
  contents = re.sub ("GetTemplateChild\((.*?)\) as (.*?);", r'e.NameScope.Find<\2>(\1);', contents)
  contents = re.sub ("CheckTemplatePart<(.*?)>\(GetTemplateChild\(\"(.*?)\"\)\)", r'CheckTemplatePart<\1>(e.NameScope.Find<\1>("\2"))', contents)
  
  contents = re.sub ("BooleanBoxes.FalseBox", "false", contents)
  contents = re.sub ("BooleanBoxes.TrueBox", "true", contents)
  contents = re.sub ("value.Box\(\)", "value", contents)
  contents = re.sub ("result.Box\(\)", "result", contents)
  
  # Containers
  contents = re.sub ("protected override AvaloniaObject GetContainerForItemOverride\(\)", "protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)", contents)
  contents = re.sub (re.compile ("protected override bool IsItemItsOwnContainerOverride\(object item\)(.*?){(\s*)return item is (.*?);(\s*)}", re.DOTALL), r"protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)\n\t\t{\n\t\t\treturn NeedsContainer<\3>(item, out recycleKey);\n\t\t}", contents)
  contents = re.sub (re.compile ("protected override void PrepareContainerForItemOverride\(AvaloniaObject element, object item\)(\s*){(\s*)base.PrepareContainerForItemOverride\(element, item\);", re.DOTALL), "protected override void PrepareContainerForItemOverride(Control element, object? item, int index)\n\t\t{\n\t\t\tbase.PrepareContainerForItemOverride(element, item, index);", contents)
  contents = re.sub ("protected override void ClearContainerForItemOverride\(AvaloniaObject element, object item\)", "protected override void ClearContainerForItemOverride(Control element)", contents)
  contents = re.sub ("base.ClearContainerForItemOverride\(element, item\);", "base.ClearContainerForItemOverride(element);", contents)
  contents = re.sub ("RaiseEvent\(new (.*?)\((.*?)ClearItemEvent, (.*?), item\)\);", r"RaiseEvent(new \1(\2ClearItemEvent, \3, null));", contents)

  contents = re.sub (": HeaderedItemsControl", ": Avalonia.Controls.TreeViewItem", contents)

  contents = re.sub ("protected override void OnRender\(DrawingContext localDrawingContext\)", "public override void Render(DrawingContext localDrawingContext)", contents)
  
  contents = re.sub (", IAttachedObject", "", contents)
  contents = re.sub ("Microsoft\.Xaml\.Behaviors\.Interaction\.GetBehaviors", "Avalonia.Xaml.Interactivity.Interaction.GetBehaviors", contents)
  
  contents = re.sub ("SelectionMode\.Extended", "SelectionMode.Multiple", contents)

  contents = re.sub ("object Convert\(object\[\] values, Type targetType, object parameter, CultureInfo culture\)", r"object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)", contents)
  contents = re.sub ("object Convert\(\[NotNull\] object\[\] values, Type targetType, object parameter, CultureInfo culture\)", r"object? Convert([NotNull] IList<object?> values, Type targetType, object? parameter, CultureInfo culture)", contents)

  contents = re.sub ("Binding\.DoNothing", "BindingOperations.DoNothing", contents)

  contents = re.sub ("FontWeights\.", "FontWeight.", contents)

  contents = re.sub ("FrameworkPropertyMetadataOptions\.BindsTwoWayByDefault", "defaultBindingMode : BindingMode.TwoWay", contents)

  contents = re.sub ("var addChild = \(IAddChild\)this;", "", contents)
  contents = re.sub ("addChild\.AddChild", "Bindings.Add", contents)

  contents = re.sub ("DataTemplateSelector", "IDataTemplate", contents)
  contents = re.sub ("e\.VerticalChange", "e.ExtentDelta.Y", contents)
  contents = re.sub ("e\.HorizontalChange", "e.ExtentDelta.X", contents)
  
  contents = re.sub ("DataGridEx", "DataGrid", contents)

  contents = re.sub (": Selector", ": SelectingItemsControl", contents)
  
  contents = re.sub ("using TextDocument = ICSharpCode.AvalonEdit.Document.TextDocument;", "using TextDocument = AvaloniaEdit.Document.TextDocument;", contents)
  
  contents = re.sub ("Dispatcher.CurrentDispatcher.BeginInvoke", "Dispatcher.UIThread.InvokeAsync", contents)
  contents = re.sub ("Dispatcher.BeginInvoke(DispatcherPriority.Input, (.*?));", r'Dispatcher.UIThread.InvokeAsync(\1, DispatcherPriority.Input);', contents)

  contents = re.sub ("ModifierKeys\.Windows", "KeyModifiers.Meta", contents)
  contents = re.sub ("ModifierKeys", "KeyModifiers", contents)
  contents = re.sub ("\.PreviewKeyDown \+=", ".KeyDown +=", contents)
  contents = re.sub ("\.PreviewKeyUp \+=", ".KeyUp +=", contents)

  contents = re.sub ("Visibility != Visibility\.Visible", r'!IsVisible', contents)
  
  return contents

# Translate the various forms of styled property.
def translateProperties (contents):

  commands = ""
  classname = ""
  affectsmeasure = ""

  # Handle DependencyProperty.Register
  # Without initializing argument,private.
  pat = re.compile ("private static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"private static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T1A", contents)
  
  
  # Without initializing argument
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T1", contents)
  
  # PropertyMetadata, one argument, function call.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8)); // T2A", contents)

  # 1 argument to PropertyMetadata, first is function call.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)Changed\)\);")
  for match in re.findall (pat, contents):
    print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[6] + "Changed);\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T4A", contents)

  # Without handler.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T2", contents)

  # 2 argument to PropertyMetadata, first is function call.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[8] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8)); // T4", contents)
  
  # 2 arguments, anything.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match, match[5])
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T5", contents)

  # FrameworkPropertyMetadata with initializer.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata(\s*){([^})]*?)}\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T6E - warning check meta data.", contents)

  # 1 argument to FrameworkPropertyMetadata, function call
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8)); // T6A", contents)

  # 1 argument to FrameworkPropertyMetadata, typecast.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(\(([^,\)]*?)\)([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, (\7)\8); // T6B", contents)

  # 1 argument to FrameworkPropertyMetadata, seems to be callback.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(On([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (mat ch, match[5])
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(On" + match[6] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T6F", contents)

  # 1 argument to FrameworkPropertyMetadata, seems to be callback, not readonly
  pat = re.compile ("public static DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(On([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match, match[5])
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(On" + match[6] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T6G", contents)

  # 1 argument to FrameworkPropertyMetadata, seems to be callback, not readonly, not callback
  pat = re.compile ("public static DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\((.*?)\)\);")
  contents = re.sub (pat, r"public static StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T6P", contents)

  # 1 argument to FrameworkPropertyMetadata
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T6", contents)

  # 2 arguments to FrameworkPropertyMetadata, second being a bind TwoWay
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), FrameworkPropertyMetadataOptions\.BindsTwoWayByDefault\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7, defaultBindingMode : BindingMode.TwoWay); // T7", contents)

  # 2 arguments to FrameworkPropertyMetadata, second being a affects render, initial value 2 parameters
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(new ([^,\)]*?)\(([^,\)]*?),([^,\)]*?)\), FrameworkPropertyMetadataOptions\.AffectsRender\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, new \7(\8, \9)); // T7A", contents)

  # 2 arguments to FrameworkPropertyMetadata, second being a affects render, initial value 1 parameter
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(new ([^,\)]*?)\(([^,\)]*?)\), FrameworkPropertyMetadataOptions\.AffectsRender\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, new \7(\8)); // T7B", contents)

  # 2 argument to FrameworkPropertyMetadata, one is affects measure.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), FrameworkPropertyMetadataOptions.AffectsMeasure\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    classname = match[5];
    if len (affectsmeasure) > 0:
      affectsmeasure += ", "
    affectsmeasure += match[0]
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T8C", contents)

  # 2 argument to FrameworkPropertyMetadata, second null
  pat = re.compile ("public static DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), null\)\);")
  contents = re.sub (pat, r"public static StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T8K", contents)

  # 2 argument to FrameworkPropertyMetadata, not readonly
  pat = re.compile ("public static DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T8N", contents)

  # 2 argument to FrameworkPropertyMetadata
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T8", contents)

  # 2 argument to FrameworkPropertyMetadata, one is affects measure.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), FrameworkPropertyMetadataOptions.AffectsMeasure\), ([^,\)]*?)\);")
  for match in re.findall (pat, contents):
    print (match)
    classname = match[5];
    if len (affectsmeasure) > 0:
      affectsmeasure += ", "
    affectsmeasure += match[0]
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7, validate: \8); // T8A", contents)

  # 2 argument to FrameworkPropertyMetadata, first is new call, one is affects measure.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(new ([^,\)]*?)\(([^\)]*?)\), FrameworkPropertyMetadataOptions.AffectsMeasure\), ([^,\)]*?)\);")
  for match in re.findall (pat, contents):
    print (match)
    classname = match[5];
    if len (affectsmeasure) > 0:
      affectsmeasure += ", "
    affectsmeasure += match[0]
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, new \7(\8), validate: \9); // T8B", contents)

  # Deal with case with frameworkpropertymetadata with 3 arguments, first is a typecast.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(\(([^,\)]*?)\)([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[9] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, (\7)\8, \9); // T9D", contents)

  # Deal with case with frameworkpropertymetadata with 3 arguments
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[8] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7, \8); // T9E", contents)

  # Deal with case with frameworkpropertymetadata with 4 arguments, any
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[8] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7, \8, coerce: \10); // T9C", contents)
  
  # Deal with case with frameworkpropertymetadata with 6 arguments, first a function.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[9] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8), \9, coerce: \11); // T9A", contents)

  # Deal with case with frameworkpropertymetadata with 6 arguments. 
  #FIXME: deal with parameters.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[8] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T9 FIXME", contents)

  
  # Direct properties

  # property metadata, no arguments.
  pat = re.compile ("private static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(nameof\((.*?)\), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      classname = match[5];
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"private static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10H1", contents)

  pat = re.compile ("public static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(\"(.*?)\", typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(\"\"\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      classname = match[5];
      #print (match, "\n", vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r'public static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>("\4", o => o.\4); // T10H3', contents)

  pat = re.compile ("private static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(\"(.*?)\", typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(BooleanBoxes.FalseBox\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value.Box\(\)\); } }"
      vsub = "private " + match[4] + " _" + match[3] + " = false;\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      classname = match[5];
      print (match, "\n", vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r'private static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>("\4", o => o.\4); // T10H4', contents)

  pat = re.compile ("private static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(\"(.*?)\", typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      classname = match[5];
      print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"private static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10H2", contents)

  
  pat = re.compile ("private static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(\"(.*?)\", typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
      classname = match[5];
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"private static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10H", contents)
  
  pat = re.compile ("public static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(nameof\((.*?)\), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"public static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10", contents)
  
  pat = re.compile ("protected static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(nameof\((.*?)\), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"protected static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10A", contents)
  

  pat = re.compile ("public static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(\"(.*?)\", typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\)\);", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      # one form of variable definion.
      vpat = "public " + match[4] + " " + match[3] + " => \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\);"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
      
      # second form.
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
      
    contents = re.sub (pat, r'public static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>("\4", o => o.\4); // T11', contents)
  
  # Attached Properties
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\)\);", re.DOTALL)
  contents = re.sub (pat, r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4); // T12A", contents)
  
  
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\), new UIPropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      #print (match)
      commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
      classname = match[5];
    contents = re.sub (pat, r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4, \7); // T12", contents)

  pat = re.compile ("public static DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);")
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      #print (match)
      commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
      classname = match[5];
    contents = re.sub (pat, r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4, \7); // T12A", contents)

  
  
  # handle keyboard focus handlers.
  # GotFocusEvent.AddClassHandler<PropertyView>((x, e) => x.OnIsKeyboardFocusWithinChanged(e)); (add to static constructor)
  pat = re.compile ("protected override void OnIsKeyboardFocusWithinChanged\(DependencyPropertyChangedEventArgs e\)(\s)*{(\s)*base.OnIsKeyboardFocusWithinChanged\(e\);", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      #print (match)
      commands += "\t\t\t" + "GotFocusEvent.AddClassHandler<" + classname + ">((x, e) => x.OnIsKeyboardFocusWithinChanged(e));\n"
    contents = re.sub (pat, r"protected void OnIsKeyboardFocusWithinChanged(GotFocusEventArgs e)\n\t\t{", contents)    
  
  if len (affectsmeasure) > 0:
    commands += "\t\t\tAffectsMeasure<" + classname + ">(" + affectsmeasure + ");\n"


  # handle RegisterAttached.
 
  # Two args. 
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,]*?), ([^,\)]*?)\)\);", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      print (match)
      commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
      classname = match[5];
    contents = re.sub (pat, r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4, \7); // T20A", contents)
  
  
  contents = re.sub (re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\((.*?)\)\);", re.DOTALL), r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4, \7); // T20", contents)
  
  

  
  # patch in commands to an existing static constructor.
  if len (commands) > 0:
    #print (commands, classname)
    #print ("Match", "static " + classname + "(\s*)\(\)")
    pat = re.compile ("static " + classname + "(\s*)\(\)(\s*){", re.DOTALL)
    if re.findall (pat, contents): # already a static class.
      contents = re.sub (pat, r"static " + classname + "()\n\t\t{\n" + commands, contents)
    else:
      pat = re.compile ("public " + classname + "(\s*)\(\)(\s*){", re.DOTALL)
      if re.findall (pat, contents): # already a static class.
        contents = re.sub (pat, r"static " + classname + "()\n\t\t{\n" + commands + "\t\t}\n\n\t\tpublic " + classname + "()\n\t\t{", contents)
      else:
        pat = re.compile ("public class " + classname + "(.*?){", re.DOTALL) # add to classheader.
        if re.findall (pat, contents): # already a static class.
          contents = re.sub (pat, r"public class " + classname + r"\1{\n\t\tstatic " + classname + "()\n\t\t{\n" + commands + "\t\t}\n", contents)
        else:
          pat = re.compile ("public abstract class (.*?) (.*?){", re.DOTALL) # add to classheader.
          if re.findall (pat, contents): # already a static class.
            contents = re.sub (pat, r"public abstract class \1 \2{\n\t\tstatic \1 ()\n\t\t{\n" + commands + "\t\t}\n", contents)
          else:                                                                                         
            pat = re.compile ("public static class (.*?)(\s)*{", re.DOTALL) # add to classheader.
            if re.findall (pat, contents): # already a static class.
              contents = re.sub (pat, r"public static class \1\n\t{\n\t\tstatic \1 ()\n\t\t{\n" + commands + "\t\t}\n", contents)
            else:
              pat = re.compile ("public sealed class (.*?) : (.*?)(\s)*{", re.DOTALL) # add to classheader.
              if re.findall (pat, contents): # already a static class.
                contents = re.sub (pat, r"public sealed class \1 : \2\n\t{\n\t\tstatic \1 ()\n\t\t{\n" + commands + "\t\t}\n", contents)
              else:
                print ("Could not find a spot to add the property classhandlers")
                exit (0)
      
  #print (commands)
    
    
    
  # Overridemetadata
  
  # Triggers not yet translated.  
  pat = re.compile ("([^\.\(\s]*?)\.OverrideMetadata\(typeof\(([^\),]*?)\), new FrameworkPropertyMetadata { DefaultUpdateSourceTrigger = UpdateSourceTrigger.Explicit }\);")
  contents = re.sub (pat, r" // T30", contents)
  
  # Where relevant, set the correct types, and move outside static constructor.
  #contents = re.sub ("DefaultStyleKeyProperty.OverrideMetadata\(typeof\((.*?)\), new FrameworkPropertyMetadata\(typeof\((.*?)\)\)\);", r"protected override Type StyleKeyOverride { get { return typeof(\2 \1); } }", contents)
  contents = re.sub ("DefaultStyleKeyProperty.OverrideMetadata\(typeof\((.*?)\), new FrameworkPropertyMetadata\(typeof\((.*?)\)\)\);", r"// FIXME  T31", contents)


  
  # Routed events.
  pat = re.compile ("public static readonly RoutedEvent (.*?)(\s*)\=(\s*)EventManager.RegisterRoutedEvent\((.*?), RoutingStrategy\.(.*?), typeof\(([^,\)]*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly RoutedEvent \1 = RoutedEvent.Register<\7, RoutedEventArgs>(\4, RoutingStrategies.\5); // T21", contents)

  contents = re.sub ("protected override void OnItemsChanged\(object sender, ItemsChangedEventArgs args\)", "protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)", contents)
  contents = re.sub ("protected override void OnItemsChanged\(NotifyCollectionChangedEventArgs e\)", "FIXME protected void OnItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) // Add to constructor: Items.CollectionChanged += OnItemsViewCollectionChanged;", contents)
  contents = re.sub ("protected override void OnItemsSourceChanged\(IEnumerable oldValue, IEnumerable newValue\)", "protected override void OnItemsSourceChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)", contents)
  
  # Scrolling.
  contents = re.sub ("IScrollInfo", "ILogicalScrollable", contents)
  contents = re.sub ("protected override void BringIndexIntoView\(int index\)", "protected override bool BringIntoView(Control target, Rect targetRect)", contents)
  
  
  # Focus
  pat = re.compile ("protected override void OnGotKeyboardFocus\(KeyboardFocusChangedEventArgs e\)(\s*){(\s*)base.OnGotKeyboardFocus\(e\);")
  contents = re.sub (pat, r"protected override void OnGotFocus(GotFocusEventArgs e)\n\t\t{\n\t\t\tbase.OnGotFocus(e);", contents)
  pat = re.compile ("protected override void OnLostKeyboardFocus\(KeyboardFocusChangedEventArgs e\)") # with no call to base.
  contents = re.sub (pat, r"protected override void OnLostFocus(RoutedEventArgs e)", contents)
  pat = re.compile ("base.OnLostKeyboardFocus\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnLostFocus(e);", contents)

  # OnInitialize
  pat = re.compile ("protected override void OnInitialized\(EventArgs e\)(\s*){(\s*)base.OnInitialized\(e\);")
  contents = re.sub (pat, r"protected override void OnInitialized()\n\t\t{\n\t\t\tbase.OnInitialized();", contents)
  
  # Mouse
  pat = re.compile ("protected override void OnMouseDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("base.OnMouseDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)

  pat = re.compile ("protected override void OnMouseLeftButtonDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseRightButtonDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerPressed(PointerPressedEventArgs e) // Right", contents)
  pat = re.compile ("base.OnMouseLeftButtonDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)

  pat = re.compile ("protected override void OnMouseLeftButtonUp\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerReleased(PointerReleasedEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseRightButtonUp\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerReleased(PointerReleasedEventArgs e) // Right", contents)
  pat = re.compile ("base.OnMouseLeftButtonUp\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerReleased(e);", contents)

  pat = re.compile ("protected override void OnMouseMove\(PointerEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerMoved(PointerEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseMove\(MouseEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerMoved(PointerEventArgs e)", contents)

  pat = re.compile ("protected override void OnMouseUp\(PointerEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerReleased(PointerReleasedEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseUp\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerReleased(PointerReleasedEventArgs e)", contents)

  pat = re.compile ("protected override void OnMouseWheel\(MouseWheelEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerWheelChanged(PointerWheelEventArgs e)", contents)
  pat = re.compile ("base.OnMouseWheel\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerWheelChanged(e);", contents)

  pat = re.compile ("\(object sender, MouseEventArgs e\)") # just the call to base.
  contents = re.sub (pat, r"(object sender, PointerEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseLeave\(MouseEventArgs e\)") # just the call to base.
  contents = re.sub (pat, r"protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)", contents)
  pat = re.compile ("base.OnMouseLeave\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerCaptureLost(e);", contents)
  pat = re.compile ("protected override void OnPreviewMouseWheel\(MouseWheelEventArgs e\)") # just the call to base.
  contents = re.sub (pat, r"protected override void OnPointerWheelChanged(PointerWheelEventArgs e)", contents)
  pat = re.compile ("protected override void OnPreviewMouseDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("base.OnPreviewMouseWheel\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerWheelChanged(e);", contents)
  pat = re.compile ("base.OnPreviewMouseDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)

  pat = re.compile ("protected override void OnPreviewKeyDown\(KeyEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected override void OnKeyDown(KeyEventArgs e)", contents)
  pat = re.compile ("base.OnPreviewKeyDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnKeyDown(e);", contents)

  
  contents = re.sub ("MouseButtonEventArgs e", "PointerEventArgs e", contents)
  contents = re.sub ("MouseEventArgs e", "PointerEventArgs e", contents)
  contents = re.sub ("MouseWheelEventArgs e", "PointerWheelEventArgs e", contents)

  contents = re.sub ("e.LeftButton == MouseButtonState.Pressed", "e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed", contents)
  contents = re.sub ("e.LeftButton == MouseButtonState.Released", "!e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed", contents)
  contents = re.sub ("\.IsMouseCaptured", ".IsPointerOver", contents)
  contents = re.sub ("\.MouseDown", ".PointerPressed", contents)
  contents = re.sub ("\.MouseUp", ".PointerReleased", contents)
  contents = re.sub ("\.MouseMove", ".PointerMoved", contents)
  contents = re.sub ("\.MouseDoubleClick", ".DoubleTapped", contents)
  
  # OnApplyTemplate
  pat = re.compile ("public override void OnApplyTemplate\(\)(\s*){(\s*)base.OnApplyTemplate\(\);")
  contents = re.sub (pat, r"protected override void OnApplyTemplate(TemplateAppliedEventArgs e)\n\t\t{\n\t\t\tbase.OnApplyTemplate(e);", contents)
  pat = re.compile ("public override void OnApplyTemplate\(\)")
  contents = re.sub (pat, r"protected override void OnApplyTemplate(TemplateAppliedEventArgs e)", contents)
  pat = re.compile ("base.OnApplyTemplate\(\);")
  contents = re.sub (pat, r"base.OnApplyTemplate(e);", contents)
  if re.findall ("OnApplyTemplate", contents):
    contents = re.sub (": Control", r": TemplatedControl", contents)

  contents = re.sub ("DependencyPropertyKey ", "StyledProperty<double> ", contents)

  
  return contents

# Translate a .cs file from Wpf to Avalonia equivalent.
def translateCS (sourceFile):
  
  sourceFileName = os.path.basename (sourceFile)
  name, extension = os.path.splitext (sourceFileName) 
  if name.endswith (".xaml"):
    basename, _ = os.path.splitext (name)
    basename += ".axaml"
  else:
    basename = name
  sourceFileDir = os.path.dirname (sourceFile)
  generatedFileDir = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"))
  generatedFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), basename + ".cs")
  generatedDiffFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), basename + ".cs" + ".diff")
  destFile = os.path.join (sourcesDir, sourceFileDir.replace ("Wpf", "Avalonia"), basename + ".cs")
  
  #print ("Translating", sourceFileDir, sourceFileName, generatedFileDir, generatedFile, generatedDiffFile, destFile, name, extension)

  # read source.
  contents = readSource (os.path.join (sourcesDir, sourceFileDir, sourceFileName))
  
  contents = translateHeaders (contents)
  contents = translateProperties (contents)
  contents = translateAnnotations (contents)
  contents = translateNames (contents)
    
  # write translated file.
  writeDest (contents, generatedFile, generatedDiffFile, destFile)

  #os.makedirs (os.path.dirname (generatedFile), exist_ok = True)
  #fle = open (generatedFile, "w")
  #fle.write (contents)
  #fle.close ()
  
  ## Now we try to preserve any changes that have been made.
  ## Either the destFile already exists, and there is no diff - so generate a diff 
  ## OR
  ## No destFile exists and there is a diff - so patch to create the dest.
  ## OR
  ## Both destFile and diff exist - warn the user to remove one or the other.
  ## OR
  ## Neither exist, just copy generated file directly to destination.
  #if os.path.isfile (destFile):
    #if os.path.isfile (generatedDiffFile):
      #print ("Both destination and diff file exist. Remove one to have it regenerated.")
      #print ("  Remove the destination file to have it regenerated in response to third party updates in the repo")
      #print ("  Remove the diff file to discard local changes to the class e.g., if translation filters change.")
      #print ()
      #print ("To remove destination file:")
      #print ("  rm " + destFile)
      #print ()
      #print ("To remove diff file:")
      #print ("  rm " + generatedDiffFile)
      #print ()
      #exit (0)
    #else:
      ## No diff, generate.
      #fle = open (destFile, "r")
      #destcontents = fle.read ()
      #fle.close ()
      #dmp = diff_match_patch.diff_match_patch ()
      #patches = dmp.patch_make (contents, destcontents)
      #diff = dmp.patch_toText (patches)
      #if len (diff) > 0:
        #print ("Destination file already exists, generating diff file: " + generatedDiffFile)
        #os.makedirs (os.path.dirname (generatedDiffFile), exist_ok = True)
        #fle = open (generatedDiffFile, "w")
        #fle.writelines (diff)
        #fle.close ()
  #else:
    #if os.path.isfile (generatedDiffFile):
      ## No destination file, generate from diff.
      #print ("Creating destination file " + destFile + " from translation and an existing diff")
      #fle = open (generatedDiffFile, "r")
      #diffcontents = fle.read ()
      #fle.close ()
      #dmp = diff_match_patch.diff_match_patch ()
      #patches = dmp.patch_fromText (diffcontents)
      #new_text, _ = dmp.patch_apply(patches, contents)
      #fle = open (destFile, "w")
      #fle.writelines (new_text)
      #fle.close ()
    #else:
      ## Neither file exists, just write directly to destination.
      #print ("Creating destination file " + destFile + " by translation")
      #fle = open (destFile, "w")
      #fle.writelines (contents)
      #fle.close ()
    

def translateConstants (contents):
  contents = re.sub ("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "xmlns=\"https://github.com/avaloniaui\"", contents)
  contents = re.sub ("/Stride.Core.Presentation.Wpf;component/Themes/ThemeSelector.axaml", "avares://Stride.Core.Presentation/Themes/AThemeSelector.axaml", contents)
  contents = re.sub ("xmlns:view=\"clr-namespace:Stride.Core.Presentation.Quantum.View;assembly=Stride.Core.Presentation.Quantum\"", "xmlns:view=\"clr-namespace:Stride.Core.Presentation.Quantum.View;assembly=Stride.Core.Presentation.Quantum.Avalonia\"", contents)
  contents = re.sub ("xmlns:viewModel=\"clr-namespace:Stride.Core.Assets.Editor.ViewModel\"", "xmlns:viewModel=\"clr-namespace:Stride.Core.Assets.Editor.ViewModel;assembly=Stride.Core.Assets.Editor\"", contents)
  contents = re.sub ("xmlns:assetCommands=\"clr-namespace:Stride.Core.Assets.Editor.Quantum.NodePresenters.Commands\"", "xmlns:assetCommands=\"clr-namespace:Stride.Core.Assets.Editor.Quantum.NodePresenters.Commands;assembly=Stride.Core.Assets.Editor\"", contents)
  contents = re.sub ("xmlns:strings=\"clr-namespace:Stride.Core.Assets.Editor.Resources.Strings\"", "xmlns:strings=\"clr-namespace:Stride.Core.Assets.Editor.Avalonia.Resources.Strings\"", contents)
  contents = re.sub ("xmlns:ctrl=\"clr-namespace:Stride.Core.Presentation.Controls\"", "xmlns:ctrl=\"clr-namespace:Stride.Core.Presentation.Controls;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:cvt=\"clr-namespace:Stride.Core.Presentation.ValueConverters\"", "xmlns:cvt=\"clr-namespace:Stride.Core.Presentation.ValueConverters;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:behaviors=\"clr-namespace:Stride.Core.Presentation.Behaviors\"", "xmlns:behaviors=\"clr-namespace:Stride.Core.Presentation.Behaviors;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:themes=\"clr-namespace:Stride.Core.Presentation.Themes\"", "xmlns:themes=\"clr-namespace:Stride.Core.Presentation.Themes;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:me=\"clr-namespace:Stride.Core.Presentation.MarkupExtensions\"", "xmlns:me=\"clr-namespace:Stride.Core.Presentation.MarkupExtensions;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:wnd=\"clr-namespace:Stride.Core.Presentation.Windows\"", "xmlns:wnd=\"clr-namespace:Stride.Core.Presentation.Windows;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:commands=\"clr-namespace:Stride.Core.Presentation.Controls.Commands\"", "xmlns:commands=\"clr-namespace:Stride.Core.Presentation.Controls.Commands;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:interactivity=\"clr-namespace:Stride.Core.Presentation.Interactivity\"", "xmlns:interactivity=\"clr-namespace:Stride.Core.Presentation.Interactivity;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  contents = re.sub ("xmlns:cmd=\"clr-namespace:Stride.Core.Presentation.Commands\"", "xmlns:cmd=\"clr-namespace:Stride.Core.Presentation.Commands;assembly=Stride.Core.Presentation.Avalonia\"", contents)
  return contents

# flag any nested comments, and break up the --, which causes compliance issues.
def removeNestedComments (contents):
  last = 0
  resultingContent = ""
  incount = 0
  outcount = 0
  
  while last < len (contents):
    startPosition = contents.find ("<!--", last)
    endPosition = contents.find ("-->", last)
    
    #print (last, startPosition, endPosition, incount, outcount, len (contents))
    
    if startPosition >= 0:
      if startPosition < endPosition:
        resultingContent += contents[last:startPosition]
        incount += 1 # nested comment.
        if incount - outcount > 1:
          resultingContent += "--><!--"
        elif incount - outcount == 1:
          resultingContent += "<!--"
        last = startPosition + 4
      else: #ending comment.
        outcount += 1
        resultingContent += contents[last:endPosition]
        last = endPosition + 3
        if incount - outcount > 0:
          resultingContent += "--><!--"
        else:
          resultingContent += "-->"
    else: # no nesting.
      if endPosition >= 0:
        outcount += 1
        resultingContent += contents[last:endPosition]
        last = endPosition + 3
        if incount - outcount > 0:
          resultingContent += "--><!--"
        else:
          resultingContent += "-->"
      else:
        resultingContent += contents[last:]
        last = len (contents)
              
  #print (resultingContent)
  return resultingContent

def translateTags (contents):
  
  # Currently don't have a way to deal with PopupAnimation.
  contents = re.sub ("PopupAnimation=\".*\"", "", contents)
  
  # Part names.
  contents = re.sub ("x:Name=\"Popup\"", 'x:Name="PART_Popup"', contents)
  
  # Toolbar
  contents = re.sub ("<ToolBarTray ([^>]*?)>", r'<StackPanel Orientation="Horizontal" \1>', contents)
  contents = re.sub ("</ToolBarTray>", r'</StackPanel>', contents)
  contents = re.sub ("<ToolBarTray.Resources>", r'<StackPanel.Resources>', contents)
  contents = re.sub ("</ToolBarTray.Resources>", r'</StackPanel.Resources>', contents)
  
  # May want HeaderedItemsControl if header field.
  contents = re.sub ("<ToolBar ([^/>]*?)>", r'<ItemsControl \1>\n\t<ItemsControl.ItemsPanel>\n\t\t<ItemsPanelTemplate>\n\t\t\t<StackPanel Orientation="Horizontal">\n\t\t\t</StackPanel>\n\t\t</ItemsPanelTemplate>\n\t</ItemsControl.ItemsPanel>', contents)
  contents = re.sub ("<ToolBar ([^/>]*?)/>", r'<ItemsControl \1>\n\t<ItemsControl.ItemsPanel>\n\t\t<ItemsPanelTemplate>\n\t\t\t<StackPanel Orientation="Horizontal">\n\t\t\t</StackPanel>\n\t\t</ItemsPanelTemplate>\n\t</ItemsControl.ItemsPanel>\n\t</ItemsControl>', contents)
  contents = re.sub ("<ToolBar>", r'<ItemsControl>\n\t<ItemsControl.ItemsPanel>\n\t\t<ItemsPanelTemplate>\n\t\t\t<StackPanel Orientation="Horizontal">\n\t\t\t</StackPanel>\n\t\t</ItemsPanelTemplate>\n\t</ItemsControl.ItemsPanel>', contents)
  contents = re.sub ("</ToolBar>", r'</ItemsControl>', contents)
  contents = re.sub (" ToolBar\.", r' ItemsControl.', contents)
  contents = re.sub ("<ToolBar.ItemTemplate>", r'<ItemsControl.ItemTemplate>', contents)
  contents = re.sub ("</ToolBar.ItemTemplate>", r'</ItemsControl.ItemTemplate>', contents)
  
  contents = re.sub (re.compile ("<ItemsControl([^>]*?)Header=\"(.*?)\"([^>]*?)>(.*?)</ItemsControl>", re.DOTALL), r'<HeaderedItemsControl\1Header="\2"\3>\4</HeaderedItemsControl>', contents)
  
  
  # Bitmap image
  contents = re.sub ("<BitmapImage x:Key=\"(.*?)\" UriSource=\"pack://application:,,,/Stride.Core.Presentation.Wpf;component/Resources/(.*?)\" />", r'<ImageBrush x:Key="\1" Source="/Resources/\2" />', contents)  
  contents = re.sub ("<BitmapImage x:Key=\"(.*?)\" UriSource=\"\.\./Resources/(.*?)\" />", r'<ImageBrush x:Key="\1" Source="/Resources/\2" />', contents)  
  contents = re.sub ("BitmapScalingMode=\"NearestNeighbor\"", r'BitmapInterpolationMode="LowQuality"', contents)
  contents = re.sub (", NearestNeighbor}", r', LowQuality}', contents)

  #contents = re.sub ("<Setter Property=\"RenderOptions\.BitmapScalingMode\" Value=\"NearestNeighbor\"([^>]*?)/>", r'<Setter Property="RenderOptions.BitmapInterpolationMode" Value="LowQuality"\1/>', contents)  # This works without the setter?
  contents = re.sub ("<Setter Property=\"RenderOptions\.BitmapScalingMode\" Value=\"NearestNeighbor\"([^>]*?)/>", r'', contents)  # This works without the setter?
  contents = re.sub ("<ImageBrush ImageSource=", r'<ImageBrush Source=', contents)
  contents = re.sub ("RenderOptions\.BitmapScalingMode=\"Linear\"", r'RenderOptions.BitmapInterpolationMode="MediumQuality"', contents)
  contents = re.sub ("RenderOptions\.BitmapScalingMode=\"HighQuality\"", r'RenderOptions.BitmapInterpolationMode="HighQuality"', contents)
  #contents = re.sub ("<Setter Property=\"RenderOptions\.BitmapScalingMode\" Value=\"NearestNeighbor\" />", r'<Setter Property="RenderOptions.BitmapInterpolationMode" Value="LowQuality" />', contents)
  contents = re.sub ("<Setter Property=\"RenderOptions\.BitmapScalingMode\" Value=\"NearestNeighbor\" />", r'', contents) # not accessible at the moment.
  
  # remap bitmap images.
  contents = re.sub ("Source=\"{StaticResource UpdateSelectedAssetsFromSource}\"", r'Source="{Binding Source={StaticResource UpdateSelectedAssetsFromSource}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Source=\"{StaticResource ImageNewAsset}\"", r'Source="{Binding Source={StaticResource ImageNewAsset}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Source=\"{StaticResource UpdateAllAssetsWithModifiedSource}\"", r'Source="{Binding Source={StaticResource UpdateAllAssetsWithModifiedSource}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Source=\"{Binding Source={StaticResource ImageNewAsset}, Path=Source}\"", r'Source="{Binding Source={StaticResource ImageNewAsset}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Source=\"{StaticResource ImageReimportEffects}\"", r'Source="{Binding Source={StaticResource ImageReimportEffects}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Source=\"{StaticResource ImageReimportEffects}\"", r'Source="{Binding Source={StaticResource ImageReimportEffects}, Path=Source}"', contents) 
  contents = re.sub ("Source=\"{StaticResource ImageEditAsset}\"", r'Source="{Binding Source={StaticResource ImageEditAsset}, Path=Source}" Stretch="None"', contents) 
  contents = re.sub ("Stretch=\"None\" Stretch", r'Stretch', contents) 
  contents = re.sub ("Stretch=\"None\" Width", r'Width', contents) 
  
  # As many versions of the visibility property, given that we're translating 3 states, to boolean.
  contents = re.sub (" Visibility=\"\{Binding (.*), Converter=\{sd:VisibleOrCollapsed\}\}\"", r' IsVisible="{Binding \1}"', contents)
  contents = re.sub (" Visibility=\"Hidden\"", r' IsVisible="false"', contents)
  contents = re.sub (" Visibility=\"Collapsed\"", r' IsVisible="false"', contents)
  contents = re.sub (" Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:InvertBool}, {sd:VisibleOrCollapsed}}}\"", r' IsVisible="{Binding \1, Converter={sd:InvertBool}}"', contents)
  
  contents = re.sub ("Visibility=\"{TemplateBinding (.*?), Converter={cvt:VisibleOrCollapsed}}\"", r'IsVisible="{TemplateBinding \1}"', contents)
  contents = re.sub ("Visibility=\"{TemplateBinding (.*?), Converter={cvt:Chained {cvt:InvertBool}, {cvt:VisibleOrCollapsed}}}\"", r'IsVisible="{TemplateBinding \1, Converter={cvt:InvertBool}}"', contents)

  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:ObjectToBool}, {sd:VisibleOrHidden}}}\"", r'IsVisible="{Binding \1, Converter={sd:ObjectToBool}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:NumericToBool}, {sd:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={sd:NumericToBool}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:IsGreater}, {sd:VisibleOrCollapsed},  Parameter1={sd:Double 0}}}\"", r'IsVisible="{Binding \1, Converter={sd:IsGreater},  ConverterParameter={sd:Double 0}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:NumericToBool}, {sd:InvertBool}, {sd:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:NumericToBool}, {sd:InvertBool}}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}, {sd:VisibleOrCollapsed}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}}, FallbackValue=false}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:VisibleOrHidden}, FallbackValue={sd:Hidden}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:CountEnumerable}, {sd:NumericToBool}, {sd:VisibleOrCollapsed}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:CountEnumerable}, {sd:NumericToBool}}, FallbackValue=false}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:CountEnumerable}, {sd:NumericToBool}, {sd:VisibleOrCollapsed}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:CountEnumerable}, {sd:NumericToBool}}, FallbackValue=false}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:VisibleOrCollapsed}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}, {sd:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:ObjectToBool}, {sd:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={sd:ObjectToBool}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), FallbackValue={sd:Collapsed}, Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}, {sd:VisibleOrHidden}}}\"", r'IsVisible="{Binding \1, FallbackValue=false, Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}, {sd:VisibleOrHidden}}, Mode=OneWay}\"", r'IsVisible="{Binding \1, Converter={sd:Chained {sd:ObjectToBool}, {sd:InvertBool}}, Mode=OneWay}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:VisibleOrCollapsed}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding Converter={sd:Chained {sd:MatchType}, {sd:VisibleOrCollapsed}, Parameter1={x:Type svm:SceneRootViewModel}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding Converter={sd:MatchType}, ConverterParameter={x:Type svm:SceneRootViewModel}, FallbackValue=false}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:VisibleOrCollapsed}, ConverterParameter={sd:False}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:NumericToBool}, {sd:VisibleOrCollapsed}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1, Converter={sd:NumericToBool}, FallbackValue=false}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:Chained {cvt:ObjectToBool}, {cvt:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={cvt:ObjectToBool}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:VisibleOrHidden}, FallbackValue={me:Hidden}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:Chained {cvt:IsEqual}, {cvt:VisibleOrCollapsed}, Parameter1={me:Int {x:Static ctrl:VectorEditingMode.Length}}}, RelativeSource={RelativeSource Mode=TemplatedParent}}\"", r'IsVisible="{Binding \1, Converter={cvt:IsEqual}, ConverterParameter={me:Int {x:Static ctrl:VectorEditingMode.Length}}, RelativeSource={RelativeSource Mode=TemplatedParent}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:Chained {cvt:IsEqual}, {cvt:InvertBool}, {cvt:VisibleOrCollapsed}, Parameter1={me:Int {x:Static ctrl:VectorEditingMode.Length}}}, RelativeSource={RelativeSource Mode=TemplatedParent}}\"", r'IsVisible="{Binding \1, Converter={cvt:Chained {cvt:IsEqual}, {cvt:InvertBool}, ConverterParameter={me:Int {x:Static ctrl:VectorEditingMode.Length}}}, RelativeSource={RelativeSource Mode=TemplatedParent}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:VisibleOrHidden},FallbackValue={me:Hidden}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={cvt:VisibleOrCollapsed}, FallbackValue={me:Hidden}}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"Visible\"", r'IsVisible="true"', contents)
  contents = re.sub ("Visibility=\"{TemplateBinding ([a-zA-Z]*?)}\"", r'IsVisible="{TemplateBinding \1}"', contents)

  contents = re.sub ("Visibility=\"{Binding Converter={sd:Chained {sd:MatchType}, {sd:VisibleOrCollapsed}, Parameter1={(.*?)}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding Converter={sd:MatchType}, ConverterParameter={\1}, FallbackValue={sd:False}}"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:MatchType}, {sd:VisibleOrCollapsed}, Parameter1={(.*?)}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding \1, Converter={sd:MatchType}, ConverterParameter={\2}, FallbackValue={sd:False}}"', contents)
  contents = re.sub ("Visibility=\"{Binding Converter={sd:Chained {sd:ObjectToBool}, {sd:VisibleOrCollapsed}}, FallbackValue={sd:Collapsed}}\"", r'IsVisible="{Binding Converter={sd:ObjectToBool}}, FallbackValue={sd:False}}"', contents)
  contents = re.sub ("Visibility=\"{Binding Converter={sd:Chained {sd:ObjectToBool}, {sd:VisibleOrCollapsed}, Parameter2={sd:False}}, FallbackValue={sd:Visible}}\"", r'IsVisible="{Binding Converter={sd:ObjectToBool}, ConverterParameter={sd:False}, FallbackValue={sd:True}}"', contents)


  contents = re.sub ("Property=\"Visibility\" Value=\"Hidden\"", r'Property="IsVisible" Value="false"', contents)
  contents = re.sub ("Property=\"Visibility\" Value=\"Collapsed\"", r'Property="IsVisible" Value="false"', contents)
  contents = re.sub ("Property=\"Visibility\" TargetName=\"(.*?)\" Value=\"Collapsed\"", r'Property="IsVisible" TargetName="\1" Value="false"', contents)
  contents = re.sub ("Property=\"Visibility\" Value=\"Collapsed\" TargetName=\"(.*?)\"", r'Property="IsVisible" TargetName="\1" Value="false"', contents)
  contents = re.sub ("Property=\"Visibility\" TargetName=\"(.*?)\" Value=\"Visible\"", r'Property="IsVisible" TargetName="\1" Value="true"', contents)
  contents = re.sub ("Property=\"Visibility\" Value=\"Visible\" TargetName=\"(.*?)\"", r'Property="IsVisible" TargetName="\1" Value="true"', contents)

  # Tooltip
  contents = re.sub ("ToolTipService.ToolTip=\"(.*?)\"", r'ToolTip.Tip="\1"', contents)
  contents = re.sub ("ToolTip=\"(.*?)\"", r'ToolTip.Tip="\1"', contents)
  contents = re.sub ("Property=\"ToolTip\"", r'Property="ToolTip.Tip"', contents)
  contents = re.sub ("<ToolTipService\.ToolTip>", r'<ToolTip.Tip>', contents)
  contents = re.sub ("</ToolTipService\.ToolTip>", r'</ToolTip.Tip>', contents)
  contents = re.sub ("ToolTipService.ShowOnDisabled=\"True\"", r'', contents)
  contents = re.sub ("ToolTipService.IsEnabled", r'ToolTip.IsEnabled', contents)

  # x:Reference. Better solution pending.
  contents = re.sub ("{x:Reference (.*?)}", r'{Binding ElementName=\1}', contents)

  # Case sensitive ok.
  contents = re.sub ("DialogResult=\"OK\"", r'DialogResult="Ok"', contents)

  # isitemshost - only read access.
  contents = re.sub ("IsItemsHost=\"True\"", r'', contents)
  contents = re.sub ("IsItemsHost=\"true\"", r'', contents)

  # ContentSource   - is this a content + templatebinding?
  contents = re.sub ("<ContentPresenter([^>]*?)ContentSource=\"(.*?)\"([^>]*?)/>", r'<ContentPresenter\1Content="{TemplateBinding \2}"\3/>', contents)
  #contents = re.sub ("<ContentPresenter([^>]*?)ContentTemplate=\"(.*?)\"([^>]*?)ContentSource=\"(.*?)\"([^>]*?)>", r'<ContentPresenter\1ContentTemplate="\2"\3\5>', contents)
  #contents = re.sub ("<ContentPresenter([^>]*?)ContentSource=\"(.*?)\"([^>]*?)>", r'<ContentPresenter\1ContentTemplate="{Binding \2}"\3>', contents)
  #contents = re.sub ("ContentSource=\"(.*?)\"", r'ContentTemplate="\1"', contents)
  #contents = re.sub ("ContentTemplateSelector=\"(.*?)\"", r'ContentTemplate="\1"', contents)
  # ContentTemplateSelector will probably have to be manually managed in the short term.
  contents = re.sub (re.compile ("<ContentPresenter([^>]*?)ContentTemplate=\"([^>]*?)\"([^>]*?)ContentTemplateSelector=\"([^>]*?)\"([^>]*?)/>", re.DOTALL), r'<ContentPresenter\1ContentTemplate="\2"\3\5/>\n\t<!-- <ContentPresenter\1ContentTemplate="\2"\3ContentTemplateSelector="\4"\5/> -->', contents)  # all in one line
  contents = re.sub (re.compile ("<ContentPresenter([^>]*?)ContentTemplate=\"([^>]*?)\"([^>]*?)ContentTemplateSelector=\"([^>]*?)\"([^>/]*?)>", re.DOTALL), r'<ContentPresenter\1ContentTemplate="\2"\3\5>\n\t<!-- <ContentPresenter\1ContentTemplate="\2"\3ContentTemplateSelector="\4"\5> -->', contents) # just the start tag
  contents = re.sub (re.compile ("<ToggleButton([^>]*?)ContentTemplate=\"([^>]*?)\"([^>]*?)ContentTemplateSelector=\"([^>]*?)\"([^>]*?)/>", re.DOTALL), r'<ToggleButton\1ContentTemplate="\2"\3\5/>\n\t<!-- <ToggleButton\1ContentTemplate="\2"\3ContentTemplateSelector="\4"\5/> -->', contents)



  # SnapToDevicePixels, AllowsTransparency. Elements that don't seem to have an equivalent.
  contents = re.sub ("SnapsToDevicePixels=\"(.*?)\"", "", contents)
  contents = re.sub ("ScrollViewer\.CanContentScroll=\"(.*?)\"", "", contents)
  contents = re.sub ("CanContentScroll=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"(.*?)\"", "", contents)

  contents = re.sub ("\.ScrollToTop()", ".ScrollToHome()", contents)
  
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"(.*?)\" />", "", contents)
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"True\"/>", "", contents)
  contents = re.sub ("AllowsTransparency=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"Cycle\"", "", contents)  # possibly investigate WrapSelection?
  contents = re.sub (re.compile ("<Setter Property=\"WindowChrome.WindowChrome\">(.*?)</Setter>", re.DOTALL), "", contents)  
  contents = re.sub ("WindowChrome.IsHitTestVisibleInChrome=\"(.*?)\"", "", contents)
  contents = re.sub ("<Setter Property=\"WindowChrome.IsHitTestVisibleInChrome\" Value=\"True\"/>", "", contents)
  contents = re.sub ("DashCap=\"Flat\"", "", contents)
  #contents = re.sub ("d:DataContext=\"{d:DesignInstance (.*?)}\"", "", contents) # possibly bad. Version in header needs to be handled differently.
  contents = re.sub ("mc:Ignorable=\"d\" d:DataContext=\"{d:DesignInstance ([^}]*?)}\"([^>]*?)>", r'mc:Ignorable="d"\2>\n\t<Design.DataContext>\n\t\t<\1 />\n\t</Design.DataContext>', contents)
  contents = re.sub ("d:DesignWidth=\"300\" d:DataContext=\"{d:DesignInstance ([^}]*?)}\"([^>]*?)>", r'd:DesignWidth="300" >\n\t<Design.DataContext>\n\t\t<\1 />\n\t</Design.DataContext>', contents)
  contents = re.sub ("<Setter([^>]*?) d:DataContext=\"{d:DesignInstance ([^}]*?)}\"([^>]*?)>", r' <Setter\1>\n\t<Design.DataContext>\n\t\t<\2 />\n\t</Design.DataContext>\n\t</Setter>', contents)
  contents = re.sub (" d:DataContext=\"{d:DesignInstance ([^}]*?)}\"([^>]*?)>", r'\2>\n\t<Design.DataContext>\n\t\t<\1 />\n\t</Design.DataContext>', contents)

  # xmlsn:i
  contents = re.sub ("xmlns:i=\"http://schemas.microsoft.com/xaml/behaviors\"", "xmlns:i=\"clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity\"", contents)
  contents = re.sub ("xmlns:vm=\"clr-namespace:Stride.GameStudio.ViewModels\"", "xmlns:vm=\"clr-namespace:Stride.GameStudio.Avalonia.ViewModels\"", contents)
  contents = re.sub ("xmlns:strings=\"clr-namespace:Stride.GameStudio.Resources.Strings\"", "xmlns:strings=\"clr-namespace:Stride.GameStudio.Avalonia.Resources.Strings\"", contents)
  contents = re.sub ("xmlns:gl=\"clr-namespace:Stride.GameStudio.Layout.Behaviors\"", "xmlns:gl=\"clr-namespace:Stride.GameStudio.Avalonia.Layout.Behaviors\"", contents)
  contents = re.sub ("xmlns:gh=\"clr-namespace:Stride.GameStudio.Helpers\"", "xmlns:gh=\"clr-namespace:Stride.GameStudio.Avalonia.Helpers\"", contents)
  contents = re.sub ("xmlns:xcad=\"https://github.com/Dirkster99/AvalonDock\"", "", contents)
  contents = re.sub ("x:Class=\"Stride.GameStudio.View.GameStudioWindow\"", "x:Class=\"Stride.GameStudio.Avalonia.View.GameStudioWindow\"", contents)
  contents = re.sub ("x:Class=\"Stride.Core.Presentation.Themes.ThemeSelector\"", "", contents)
  


  # dock
  contents = regex.sub (regex.compile ("<xcad:DockingManager (.*?)>", regex.DOTALL), r'<DockControl \1 InitializeLayout="True" InitializeFactory="True">\n\t<DockControl.Factory>\n\t\t<Factory />\n\t</DockControl.Factory>', contents)
  contents = re.sub ("xcad:LayoutRoot", "RootDock", contents)
  contents = re.sub ("xcad:LayoutPanel", "ProportionalDock", contents)
  contents = re.sub ("xcad:LayoutAnchorablePane", "DocumentDock", contents)
  contents = re.sub ("xcad:LayoutAnchorable", "Document", contents)
  contents = re.sub ("<xcad:LayoutDocumentPane>", "", contents)
  contents = re.sub ("</xcad:LayoutDocumentPane>", "", contents)
  contents = re.sub ("</xcad:DockingManager>", "</DockControl>", contents)
  contents = regex.sub (regex.compile ("gh:AvalonDockHelper.IsVisible=\".*?\"", regex.DOTALL), "", contents)
  
  #contents = re.sub ("Value=\"{sd:False}\"", 'Value="false"', contents) # not needed, fails in some cases.
  
  contents = re.sub ("TextSearch\.TextPath", 'TextSearch.Text', contents)
  
  # ResourceDictionary with source.
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\" /\>", r'<ResourceInclude Source="\1.axaml" />', contents)
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\"/\>", r'<ResourceInclude Source="\1.axaml" />', contents)
  contents = re.sub ("<ResourceInclude Source=\"pack://application:,,,/Stride.Core.Presentation.Wpf;component/Resources/VectorResources.axaml\" />", r'<ResourceInclude Source="avares://Stride.Core.Presentation.Avalonia/Resources/VectorResources.axaml" />\n\t<ResourceInclude Source="../ValueConverters/SystemColors.axaml" />', contents)

  # Dropshadowbitmap
  contents = re.sub ("<DropShadowBitmapEffect (.*?)/>", r'<DropShadowEffect \1/>', contents) # one line style

  # SystemColors
  contents = re.sub ("\{DynamicResource \{x:Static SystemColors\.ActiveCaptionTextBrushKey\}\}", r'{StaticResource ActiveCaptionTextBrushKey}', contents) # one line style  
  contents = re.sub ("\{DynamicResource \{x:Static SystemColors\.ControlTextBrushKey\}\}", r'{StaticResource ControlTextBrushKey}', contents) # one line style  
  contents = re.sub ("\{DynamicResource \{x:Static SystemColors\.GrayTextBrushKey\}\}", r'{StaticResource GrayTextBrushKey}', contents) # one line style  
  contents = re.sub ("\{DynamicResource \{x:Static SystemColors\.HighlightTextBrushKey\}\}", r'{StaticResource HighlightTextBrushKey}', contents) # one line style  
  
  # Styles become various forms of Theme.
  contents = re.sub ("\<Style TargetType=\"([^\"].*?)\"(.*?)\/\>", r'<ControlTheme TargetType="\1"\2></ControlTheme>', contents) # one line style
  contents = re.sub ("sd:TextBox\.Style", r'sd:TextBox.Theme', contents) # one line style
  contents = re.sub ("ListBox\.Style", r'ListBox.Theme', contents) # one line style
  contents = re.sub ("Button\.Style", r'Button.Theme', contents) # one line style
  contents = re.sub ("Path\.Style", r'Path.Theme', contents) # one line style
  contents = re.sub ("Image\.Style", r'Image.Theme', contents) 
  contents = re.sub ("sd:TagControl\.Style", r'sd:TagControl.Theme', contents) # one line style
  contents = re.sub ("ListBox\.ItemContainerStyle", r'ListBox.ItemContainerTheme', contents) # one line style
  contents = re.sub ("ItemsControl\.ItemContainerStyle", r'ItemsControl.ItemContainerTheme', contents) # one line style

  contents = re.sub ("FrameworkElement\.Resources", r'Control.Resources', contents) # one line style
  contents = re.sub ("<FrameworkElement", r'<Control', contents) # one line style
  contents = re.sub ("<FrameworkElement", r'<Control', contents) # one line style
  contents = re.sub ("</FrameworkElement", r'</Control', contents) # one line style

  # FIXME: require TargetType.
  pat = regex.compile ("<Style(\s[^>]*)*>(((?R)|.)*?)<\/Style>", regex.DOTALL)
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # 1 level of nesting.
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # second level of nesting.
  
  # Replace access to styles.
  contents = re.sub ("Style=\"(.*?)\"", r'Theme="\1"', contents)

  contents = re.sub ("ToolBarTray.IsLocked=\"True\"", r'', contents)

  # buttonbase
  contents = re.sub ("{x:Type ButtonBase}", "{x:Type Button}", contents)


  # Setters
  contents = re.sub ("<Setter Property=\"WindowStyle\" Value=\"None\"/>", "", contents)
  contents = re.sub ("<Setter Property=\"ResizeMode\" Value=\"NoResize\" />", "", contents)
  contents = re.sub ("<Setter Property=\"HasDropShadow\" Value=\"(.*?)\"/>", "", contents)
  contents = re.sub ("<Setter Property=\"OverridesDefaultStyle\" Value=\"(.*?)\"([^>]*?)/>", "", contents)
  contents = re.sub ("<Setter Property=\"FocusVisualStyle\" Value=\"(.*?)\"([^>]*?)/>", "", contents)
  contents = re.sub ("<Setter Property=\"Stylus.IsFlicksEnabled\" Value=\"False\" />", "", contents)
  contents = re.sub ("<Setter Property=\"ScrollViewer.CanContentScroll\" Value=\"True\" />", "", contents)
  contents = re.sub ("<Setter Property=\"AllowDrop\" Value=\"true\" />", "", contents)
  contents = re.sub ("<Setter Property=\"HorizontalContentAlignment\" Value=\"(.*?)\"(.*?)/>", r'<Setter Property="HorizontalAlignment" Value="\1" />', contents)
  contents = re.sub ("<Setter Property=\"VerticalContentAlignment\" Value=\"(.*?)\"(.*?)/>", r'<Setter Property="VerticalAlignment" Value="\1" />', contents)
  contents = re.sub ("<Setter Property=\"StrokeLineJoin\" Value=\"Round\" />", '<Setter Property="StrokeJoin" Value="Round" />', contents)
  
  # Systemparameters - to local file, in presentation.windows.
  contents = re.sub (" SystemParameters\.", r" wnd:SystemParameters.", contents)
  
  # transformcollection
  contents = re.sub ("<TransformCollection>", r"", contents)
  contents = re.sub ("</TransformCollection>", r"", contents)
  
  # RichTextBox
  contents = re.sub ("<RichTextBox", r"<TextBox", contents)
  contents = re.sub ("</RichTextBox", r"</TextBox", contents)

  # textbox
  contents = re.sub ("<TextBox([^>]*?)ScrollViewer.HorizontalScrollBarVisibility=\"Auto\"([^>]*?)>", r"<TextBox\1\2>", contents)
  contents = re.sub ("<TextBox([^>]*?)ScrollViewer.VerticalScrollBarVisibility=\"Auto\"([^>]*?)>", r"<TextBox\1\2>", contents)
  contents = re.sub ("<TextBox([^>]*?)HorizontalScrollBarVisibility=\"Auto\"([^>]*?)>", r"<TextBox\1\2>", contents)
  contents = re.sub ("<TextBox([^>]*?)VerticalScrollBarVisibility=\"Auto\"([^>]*?)>", r"<TextBox\1\2>", contents)

  # ScrollBar
  contents = re.sub ("{TemplateBinding ComputedHorizontalScrollBarVisibility}", r"{TemplateBinding HorizontalScrollBarVisibility}", contents)
  contents = re.sub ("{TemplateBinding ComputedVerticalScrollBarVisibility}", r"{TemplateBinding VerticalScrollBarVisibility}", contents)
  contents = re.sub ("ViewportSize=\"{TemplateBinding ViewportWidth}\"", r'ViewportSize="{TemplateBinding Viewport}"', contents)
  contents = re.sub ("ViewportSize=\"{TemplateBinding ViewportHeight}\"", r'ViewportSize="{TemplateBinding Viewport}"', contents)
  contents = re.sub ("{TemplateBinding ScrollableHeight}", r"{TemplateBinding Extent}", contents)
  contents = re.sub ("{TemplateBinding ScrollableWidth}", r"{TemplateBinding Extent}", contents)

  contents = re.sub ("{TemplateBinding ItemTemplateSelector}", r"{TemplateBinding ItemTemplate}", contents)
  contents = re.sub ("{TemplateBinding SelectionBoxItemTemplate}", r"{TemplateBinding ItemTemplate}", contents)
  contents = re.sub ("IsReadOnly=\"{TemplateBinding IsReadOnly}\"", r"", contents) # should limit this to templates that lack this property, such as combobox.
  contents = re.sub ("Padding=\"{TemplateBinding Control.Padding}\"", r"", contents)
  contents = re.sub ("Padding=\"0,3\"", r"", contents)
  contents = re.sub ("Margin=\"{TemplateBinding Control.Padding}\"", r"", contents)
  #contents = re.sub ("Text=\"{TemplateBinding TrimmedText}\"", r'Text="{TemplateBinding Text}"', contents)
  contents = re.sub ("Text=\"{TemplateBinding MenuItem.InputGestureText}\"", r'Text="{TemplateBinding MenuItem.InputGesture}"', contents)
  contents = re.sub ("{TemplateBinding ActualHeight}", r'{TemplateBinding Height}', contents)

  # inputbindings
  contents = re.sub ("<Button.InputBindings>", r"", contents)
  contents = re.sub ("</Button.InputBindings>", r"", contents)

  contents = re.sub ("IsSubmenuOpen", r"IsSubMenuOpen", contents)
  
  # menuitem
  contents = re.sub ("{x:Static MenuItem.SeparatorStyleKey}", r"MenuItem.SeparatorStyleKey", contents)
  contents = re.sub ("{x:Static MenuItem.TopLevelHeaderTemplateKey}", r"MenuItem.TopLevelHeaderTemplateKey", contents)
  contents = re.sub ("{x:Static MenuItem.TopLevelItemTemplateKey}", r"MenuItem.TopLevelItemTemplateKey", contents)
  contents = re.sub ("{x:Static ItemsControl.ButtonStyleKey}", r"ItemsControl.ButtonStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.CheckBoxStyleKey}", r"ItemsControl.CheckBoxStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.RadioButtonStyleKey}", r"ItemsControl.RadioButtonStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.ToggleButtonStyleKey}", r"ItemsControl.ToggleButtonStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.ComboBoxStyleKey}", r"ItemsControl.ComboBoxStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.MenuStyleKey}", r"ItemsControl.MenuStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.TextBoxStyleKey}", r"ItemsControl.TextBoxStyleKey", contents)
  contents = re.sub ("{x:Static ItemsControl.SeparatorStyleKey}", r"ItemsControl.SeparatorStyleKey", contents)

  contents = re.sub ("Overrides/ExpressionDarkTheme.xaml", r"Overrides/ExpressionDarkTheme.axaml", contents)
  contents = re.sub ("Overrides/Overrides/DarkSteelTheme.xaml", r"Overrides/DarkSteelTheme.axaml", contents)
  contents = re.sub ("Overrides/DividedTheme.xaml", r"Overrides/DividedTheme.axaml", contents)
  contents = re.sub ("Overrides/LightSteelBlueTheme.xaml", r"Overrides/LightSteelBlueTheme.axaml", contents)
  
  
  # Flow document
  contents = re.sub ("FlowDocumentScrollViewer", r"ScrollViewer", contents)
  contents = re.sub ("<GroupBox", r"<HeaderedContentControl", contents)
  contents = re.sub ("</GroupBox", r"</HeaderedContentControl", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type FlowDocument}\">(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)x:Key=\"{x:Static local:XamlMarkdown(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type PasswordBox}(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type RichTextBox}(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type GroupBox}(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"{x:Static GridView(.*?)</ControlTheme>", regex.DOTALL), r"", contents)  # maybe replace with datagrid?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"{x:Type GridView(.*?)</ControlTheme>", regex.DOTALL), r"", contents)  # maybe replace with datagrid?
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type ListView(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"Hyperlink(.*?)</ControlTheme>", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type ToolBar(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  #contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"TagToolBar(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"NuclearButtonFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"RadioButtonFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"CheckBoxFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"ExpanderHeaderFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"ButtonFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"ListViewItemFocusVisual(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?

  contents = regex.sub (regex.compile ("<Setter Property=\"Template\">(\s*?)<Setter.Value>(\s*?)<ControlTemplate TargetType=\"{x:Type ContextMenu}(.*?)</ControlTemplate>(\s*?)</Setter.Value>(\s*?)</Setter>", regex.DOTALL), r"", contents) # temp, possible to resolve?
  contents = regex.sub (regex.compile ("<Setter Property=\"Template\">(\s*?)<Setter.Value>(\s*?)<ControlTemplate TargetType=\"{x:Type Menu}(.*?)</ControlTemplate>(\s*?)</Setter.Value>(\s*?)</Setter>", regex.DOTALL), r"", contents) # temp, possible to resolve?

  #contents = regex.sub (regex.compile ("<ControlTemplate x:Key=\"CheckBoxTemplate(.*?)</ControlTemplate>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTemplate x:Key=\"RadioButtonTemplate(.*?)</ControlTemplate>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("BulletDecorator.Bullet", regex.DOTALL), r"Panel", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("BulletDecorator", regex.DOTALL), r"Panel", contents) # maybe add support later?

  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type StatusBar(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = regex.sub (regex.compile ("<ControlTheme TargetType=\"{x:Type IFrameworkInputElement(.*?)</ControlTheme>", regex.DOTALL), r"", contents) # maybe add support later?
  contents = re.sub ("<BorderGapMaskConverter x:Key=\"BorderGapMaskConverter\" />", "", contents)
  contents = re.sub ("<EventSetter Event=\"Loaded\" Handler=\"Image_Loaded\" />", "", contents)
  contents = re.sub ("<MouseBinding(.*?)/>", "", contents)
  contents = regex.sub (regex.compile ("<ItemContainerTemplate(.*?)</ItemContainerTemplate>", regex.DOTALL), r"<DataTemplate\1</DataTemplate>", contents)

  contents = regex.sub (regex.compile ("<ControlTheme x:Key=\"TagToolBarStyle\" TargetType=\"{x:Type ToolBar}\">", regex.DOTALL), r'<ControlTheme x:Key="TagToolBarStyle" TargetType="{x:Type ItemsControl}">', contents) 
  contents = regex.sub (regex.compile ("<ControlTemplate TargetType=\"{x:Type ToolBar}\">", regex.DOTALL), r'<ControlTemplate TargetType="{x:Type ItemsControl}">', contents) 
  contents = regex.sub ("<ToolBarOverflowPanel(.*?)x:Name=\"PART_ToolBarOverflowPanel\"(.*?)/>", r'', contents) 
  contents = regex.sub ("<ToolBarPanel(.*?)x:Name=\"PART_ToolBarPanel\"(.*?)/>", r'', contents) 
  contents = regex.sub (regex.compile ("<ToggleButton([^>]*?)IsVisible=\"{TemplateBinding HasOverflowItems}\" x:Name=\"OverflowButton\"([^>]*?)>(.*?)</ToggleButton>", regex.DOTALL), r'', contents) 

  # togglebutton
  contents = re.sub ("FocusVisualTheme=\"(.*?)\"", r"", contents)
  
  contents = re.sub ("FocusManager\.IsFocusScope=\"True\"", "", contents)
  contents = regex.sub (regex.compile ("<Setter Property=\"FocusVisualStyle\"(.*?)</Setter>", regex.DOTALL), r"", contents)

  # Ensure all controlthemes have x:Key
  pat = re.compile ("<ControlTheme(.*?)TargetType=\"(.*?)\"(.*?)>")
  # screen matches.
  contents = pat.sub (lambda match: match.group () if "x:Key" in match.group () else r'<ControlTheme x:Key="' + match.group(2) + '"' + match.group(1) + r'TargetType="' + match.group(2) + r'"' + match.group(3) + r' >', contents)
  
  # no keys when theme is part of template.
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)>(\s*?)<ControlTheme x:Key=\"(.*?)\"(.*?)</ControlTheme>(.*?)</ControlTheme>", regex.DOTALL), r"<ControlTemplate\1>\2<ControlTheme \4</ControlTheme>\5</ControlTheme>", contents)
  contents = regex.sub (regex.compile ("\.Theme>(\s*?)<ControlTheme x:Key=\"(.*?)\" TargetType=", regex.DOTALL), r".Theme>\1<ControlTheme TargetType=", contents)
  
  contents = re.sub ("<ControlTheme x:Key=\"TextBlock\"", '<ControlTheme x:Key="{x:Type TextBlock}"', contents)
  
  # Fontweight
  contents = re.sub ("FontWeights\.", "FontWeight.", contents)
  contents = re.sub ("FontTheme=\"(.*?)\"", r'FontStyle="\1"', contents)
  
  # Drawing brush
  contents = re.sub ("<DrawingBrush Viewport=\"(.*?)\"", "<DrawingBrush", contents)
  contents = re.sub ("ViewportUnits=\"(.*?)\"", "", contents)
  contents = re.sub ("MappingMode=\"Absolute\"", "", contents)
  contents = re.sub ("Viewbox=\"(.*?)\"", "", contents)
  contents = re.sub ("ViewboxUnits=\"(.*?)\"", "", contents)
  contents = re.sub ("Viewport=\"(.*?)\"", "", contents)
  contents = re.sub ("StrokeLineJoin=", "StrokeJoin=", contents)

  # x:Name
  contents = re.sub (re.compile ("<TranslateTransform([^>]*?)x:Name=\"(.*?)\"([^>]*?)/>", re.DOTALL), r"<TranslateTransform\1\3/>", contents)
  contents = re.sub (re.compile ("<ColumnDefinition([^>]*?)x:Name=\"(.*?)\"([^>]*?)/>", re.DOTALL), r"<ColumnDefinition\1\3/>", contents)
  contents = re.sub (re.compile ("<RowDefinition([^>]*?)x:Name=\"(.*?)\"([^>]*?)/>", re.DOTALL), r"<RowDefinition\1\3/>", contents)

  
  # Gestures
  contents = re.sub ("InputGestureText=", "InputGesture=", contents)
  contents = re.sub ("\"MouseDoubleClick\"", "\"DoubleTapped\"", contents)
  
  # Grid
  contents = re.sub ("MinWidth=\"{TemplateBinding ActualWidth}\"", "", contents)
  contents = re.sub ("view:DataGridEx", "DataGrid", contents)
  
  # Tab panel.
  contents = re.sub ("<TabPanel", r'<Panel', contents)
  contents = re.sub ("</TabPanel", r'</Panel', contents)

  # LayoutTransform  
  contents = re.sub ("Grid.LayoutTransform", "LayoutTransformControl", contents)
  contents = re.sub ("ContentPresenter.LayoutTransform", "LayoutTransformControl", contents)
  contents = re.sub ("Panel.LayoutTransform", "LayoutTransformControl", contents)
  contents = re.sub ("Border.LayoutTransform", "LayoutTransformControl", contents)
  
  # nest controls inside layouttransform.
  contents = re.sub (re.compile ("<ContentPresenter([^>]*?)>(\s*?)<LayoutTransformControl>(.*?)</LayoutTransformControl>(\s*?)</ContentPresenter>", re.DOTALL), r"<LayoutTransformControl>\3\n\t<ContentPresenter\1>\n\t</ContentPresenter>\n</LayoutTransformControl>", contents)
  
  contents = re.sub (re.compile ("<Panel([^>]*?)>(\s*?)<LayoutTransformControl>(.*?)</LayoutTransformControl>(.*?)</Panel>", re.DOTALL), r"<LayoutTransformControl>\3\n\t<Panel\1>\4</Panel>\n\t</LayoutTransformControl>", contents)
  
  contents = regex.sub (regex.compile ("<Grid ([^>]*?)>(\s*?)<LayoutTransformControl>(.*?)</LayoutTransformControl>(((?!</Grid>).)*)</Grid>(\s*?)</Grid>", regex.DOTALL), r"<LayoutTransformControl>\3\n\t<Grid \1>\4</Grid>\n\t</Grid>\n\t</LayoutTransformControl>", contents) # nested grid
  contents = re.sub (re.compile ("<Grid([^>]*?)>(\s*?)<LayoutTransformControl>(.*?)</LayoutTransformControl>(.*?)</Grid>", re.DOTALL), r"<LayoutTransformControl>\3\n\t<Grid\1>\4</Grid>\n\t</LayoutTransformControl>", contents)

  #contents = re.sub ("TransformGroup", "LayoutTransformControl.LayoutTransform", contents)
  #contents = re.sub ("<LayoutTransformControl.LayoutTransform.Children>", "", contents)
  #contents = re.sub ("</LayoutTransformControl.LayoutTransform.Children>", "", contents)
  contents = re.sub (re.compile ("<TransformGroup>(\s*?)<TransformGroup.Children>(.*?)</TransformGroup.Children>(\s*?)</TransformGroup>", re.DOTALL), r"<LayoutTransformControl.LayoutTransform>\2\n\t</LayoutTransformControl.LayoutTransform>", contents)
  contents = re.sub (re.compile ("<LayoutTransformControl>(\s*?)<TransformGroup>(.*?)</TransformGroup>(.*?)</LayoutTransformControl>", re.DOTALL), r"<LayoutTransformControl>\1<LayoutTransformControl.LayoutTransform>\n\t<TransformGroup>\2</TransformGroup>\n\t</LayoutTransformControl.LayoutTransform>\3</LayoutTransformControl>", contents)

  # Control
  contents = re.sub ("Control.HorizontalContentAlignment}", "ContentControl.HorizontalContentAlignment}", contents)
  contents = re.sub ("Control.VerticalContentAlignment}", "ContentControl.VerticalContentAlignment}", contents)

  contents = re.sub ("CommandTarget=", "CommandParameter=", contents)
  contents = re.sub ("\"CommandTarget\"", '"CommandParameter"', contents)
  
  # Combobox
  contents = re.sub ("<ComboBox Theme=\"(.*?)\" Text=\"(.*?)\"", r'<ComboBox Theme="\1" PlaceholderText="\2"', contents)
  
  # Remove routed commands using application commands.
  contents = re.sub ("<sd:CommandBindingBehavior RoutedCommand=\"ApplicationCommands\.Delete\" (.*?)/>", r'<!-- <sd:CommandBindingBehavior RoutedCommand="ApplicationCommands.Delete" \1/> -->', contents)
  contents = re.sub ("<sd:CommandBindingBehavior RoutedCommand=\"ApplicationCommands\.Paste\" (.*?)/>", r'<!-- <sd:CommandBindingBehavior RoutedCommand="ApplicationCommands.Paste" \1/> -->', contents)
  
  # true/false. 
  #contents = re.sub ("\"{sd:True}\"", r'"true"', contents) # is required.
  #contents = re.sub ("\"{sd:False}\"", r'"false"', contents)
 
  # HierarchicalDataTemplace
  contents = re.sub ("HierarchicalDataTemplate", r'TreeDataTemplate', contents)
  
  # Adorners
  contents = re.sub (re.compile ("AdornerStoryboard=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("DisplayDropAdorner=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("<AdornerDecorator>", re.DOTALL), "<StackPanel>", contents)
  contents = re.sub (re.compile ("</AdornerDecorator>", re.DOTALL), "</StackPanel>", contents)
  contents = re.sub (re.compile ("<sd:ContainTextAdornerBehavior />", re.DOTALL), "", contents)
  
  # Track
  contents = re.sub ("Track\.IncreaseRepeatButton", r'Track.IncreaseButton', contents)
  contents = re.sub ("Track\.DecreaseRepeatButton", r'Track.DecreaseButton', contents)
  
  # Templates.
  
  # Sometimes replace .Resources with .DataTemplates
  #contents = re.sub (re.compile ("<ControlTemplate([^>]*?)>([^<]*?)<ControlTemplate.Resources>([^<]*?)<ControlTheme(.*?)</ControlTheme>([^<]*?)</ControlTemplate.Resources>(.*?)</ControlTheme>", re.DOTALL), r'<ControlTemplate\1>\2<ControlTheme\4</ControlTheme>\6</ControlTheme>', contents)
  contents = re.sub ("<ControlTemplate\.Resources>", r'', contents)
  contents = re.sub ("</ControlTemplate\.Resources>", r'', contents)
  contents = re.sub ("<Style\.Resources>", r'<ControlTheme.Resources>', contents)
  contents = re.sub ("</Style\.Resources>", r'</ControlTheme.Resources>', contents)
  
  
  
  # Tricky rule - templates need to have a template tag added around them, if they don't already have.
  pat = re.compile ("<([^>]*?) ([^>]*?)>[ \n]*?<DataTemplate/>[ \n]*?</(.*?)>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      #print (match)
      contents = re.sub (pat, r"<\1 \2>\n\t<\1.Template>\n\t\t<DataTemplate/>\n\t</\1.Template>\n</\3>", contents)    

  # Template tag, with more complex structure.
  pat = re.compile ("<([^>-]*?) ([^>]*?)>[ \n]*?<DataTemplate([^>\.]*?)>(.*?)</DataTemplate>[ \n]*?</(.*?)>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      #print (match)
      pass
    contents = re.sub (pat, r"<\1 \2>\n\t<\1.Template>\n\t\t<DataTemplate\3>\n\t\4</DataTemplate>\n</\1.Template>\n</\5>", contents)    

  # Template tag, with more complex structure, particularly with OverriddenProviderNames.
  pat = re.compile ("<([^>-]*?) ([^>]*?)>[ \n]*?<([^>-]*?).OverriddenProviderNames>(.*?)</([^>-]*?).OverriddenProviderNames>[ \n]*?<DataTemplate([^>]*?)>(.*?)</DataTemplate>[ \n]*</(?:\\1)>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      print (match[0])
    contents = re.sub (pat, r"<\1 \2>\n\t<\3.OverriddenProviderNames>\4\n\t</\5.OverriddenProviderNames>\n<\1.Template>\n\t\t<DataTemplate\6>\n\t\7</DataTemplate>\n</\1.Template>\n</\1>", contents)    
    
  # FIXME
  # Squash a few functions that are not available yet - but will be.
  contents = re.sub (re.compile ("\<view:TreeViewTemplateSelector(.*)?</view:TreeViewTemplateSelector(.*?)>", re.DOTALL), "", contents)
  #contents = re.sub ("sd:PriorityBinding", "Binding", contents)
  contents = re.sub ("<Setter Property=\"IsCheckable\" Value=\"True\" />", "", contents)
  contents = re.sub (re.compile ("<Storyboard(.*?)</Storyboard>", re.DOTALL), "", contents)
  #contents = re.sub (re.compile ("<Setter Property=\"Visibility\" Value=\"(.*?)\"/>", re.DOTALL), "", contents)
  contents = re.sub (re.compile (" IsEnabled=\"(.*?)\"", re.DOTALL), "", contents)
  #contents = re.sub (re.compile (" Visibility=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("DisplayMemberPath=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("StaysOpen=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("SelectedValuePath=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("ContentStringFormat=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("IsEditable=\"(.*?)\"", re.DOTALL), "", contents) # might need to look for the FluentAvalonia editable combobox if this is true?
  #contents = re.sub (re.compile ("<Setter Property=\"IsEditable\" Value=\"(.*?)\"/>", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("Theme=\"{StaticResource {x:Static ItemsControl.ToggleButtonStyleKey}}\"", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("ToolTipService\.InitialShowDelay=\"1\"", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("<CollectionViewSource (.*?)/CollectionViewSource>", re.DOTALL), r"<!--<CollectionViewSource \1)/CollectionViewSource>-->", contents) 
  #contents = re.sub (re.compile ("<CollectionViewSource ([^/]*?)/>", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("VirtualizingPanel\.IsVirtualizing=\"True\"", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("<Setter Property=\"VirtualizingStackPanel.IsVirtualizing\" Value=\"False\" />", re.DOTALL), "", contents) 
  contents = re.sub (re.compile ("VirtualizingPanel\.VirtualizationMode=\"Recycling\"", re.DOTALL), "", contents) 
  #contents = re.sub (re.compile ("IsChecked=\"(.*?)\"", re.DOTALL), "", contents)  # hopefully in new avalonia # valid for togglebuttons.
  contents = re.sub (re.compile ("IsCheckable=\"False\"", re.DOTALL), "", contents)  # hopefully in new avalonia
  contents = re.sub (re.compile ("SelectionMode=\"Extended\"", re.DOTALL), "", contents)  # hopefully in new avalonia
  contents = re.sub (re.compile ("<KeyBinding(.*?)/>", re.DOTALL), "", contents)  # temporary
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.LineUpCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.LineDownCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.PageDownCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.PageUpCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.LineLeftCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.LineRightCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.PageRightCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"ScrollBar.PageLeftCommand\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"Slider.IncreaseLarge\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Command=\"Slider.DecreaseLarge\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("<ContentPresenter([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<ContentPresenter\1", contents)
  contents = regex.sub (regex.compile ("<ContentPresenter([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<ContentPresenter\1", contents)
  contents = regex.sub (regex.compile ("<Path([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<Path\1", contents)
  contents = regex.sub (regex.compile ("<Path([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<Path\1", contents)
  contents = regex.sub (regex.compile ("<Panel([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<Panel\1", contents)
  contents = regex.sub (regex.compile ("<Panel([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<Panel\1", contents)
  contents = regex.sub (regex.compile ("<Border([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<Border\1", contents)
  contents = regex.sub (regex.compile ("<Border([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<Border\1", contents)
  contents = regex.sub (regex.compile ("<Grid([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<Grid\1", contents)
  contents = regex.sub (regex.compile ("<Grid([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<Grid\1", contents)
  contents = regex.sub (regex.compile ("<Rectangle([^>]*?)Height=\"Auto\"", regex.DOTALL), r"<Rectangle\1", contents)
  contents = regex.sub (regex.compile ("<Rectangle([^>]*?)Width=\"Auto\"", regex.DOTALL), r"<Rectangle\1", contents)
  contents = regex.sub (regex.compile ("x:Shared=\"False\"", regex.DOTALL), r"", contents)
  contents = regex.sub (regex.compile ("Path=Source\.\(BitmapSource\.PixelHeight\)", regex.DOTALL), r"Path=Size.Height", contents)
  contents = regex.sub (regex.compile ("Path=Source\.\(BitmapSource\.PixelWidth\)", regex.DOTALL), r"Path=Size.Width", contents)
  contents = regex.sub (regex.compile ("ContentTemplate=\"{TemplateBinding HeaderedContentControl.HeaderTemplate}\"", regex.DOTALL), r'ContentTemplate="{TemplateBinding HeaderTemplate}"', contents)

  # Force a textpresenter into textbox.
  contents = re.sub (re.compile ("<ControlTemplate x:Key=\"TextBoxTemplate\" TargetType=\"{x:Type TextBox}\">(.*?)<ScrollViewer (.*?)/>", re.DOTALL), r'<ControlTemplate x:Key="TextBoxTemplate" TargetType="{x:Type TextBox}">\1<ScrollViewer \2>\n\t\t<TextPresenter x:Name="PART_TextPresenter" Text="{TemplateBinding Text, Mode=TwoWay}" CaretIndex="{TemplateBinding CaretIndex}" SelectionStart="{TemplateBinding SelectionStart}" SelectionEnd="{TemplateBinding SelectionEnd}" TextAlignment="{TemplateBinding TextAlignment}" TextWrapping="{TemplateBinding TextWrapping}" LineHeight="{TemplateBinding LineHeight}" LetterSpacing="{TemplateBinding LetterSpacing}" PasswordChar="{TemplateBinding PasswordChar}" RevealPassword="{TemplateBinding RevealPassword}" SelectionBrush="{TemplateBinding SelectionBrush}" SelectionForegroundBrush="{TemplateBinding SelectionForegroundBrush}" CaretBrush="{TemplateBinding CaretBrush}" HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" VerticalAlignment="{TemplateBinding VerticalContentAlignment}"/>\n\t</ScrollViewer>', contents)
  contents = re.sub (re.compile ("<ControlTheme x:Key=\"ctrl:TextBox\" TargetType=\"ctrl:TextBox\"(.*?)<ScrollViewer(.*?)/>", re.DOTALL), r'<ControlTheme x:Key="ctrl:TextBox" TargetType="ctrl:TextBox"\1<ScrollViewer\2>\n\t\t<TextPresenter x:Name="PART_TextPresenter" Text="{TemplateBinding Text, Mode=TwoWay}" CaretIndex="{TemplateBinding CaretIndex}" SelectionStart="{TemplateBinding SelectionStart}" SelectionEnd="{TemplateBinding SelectionEnd}" TextAlignment="{TemplateBinding TextAlignment}" TextWrapping="{TemplateBinding TextWrapping}" LineHeight="{TemplateBinding LineHeight}" LetterSpacing="{TemplateBinding LetterSpacing}" PasswordChar="{TemplateBinding PasswordChar}" RevealPassword="{TemplateBinding RevealPassword}" SelectionBrush="{TemplateBinding SelectionBrush}" SelectionForegroundBrush="{TemplateBinding SelectionForegroundBrush}" CaretBrush="{TemplateBinding CaretBrush}" HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" VerticalAlignment="{TemplateBinding VerticalContentAlignment}"/>\n\t</ScrollViewer>', contents)
  contents = re.sub (re.compile ("<ControlTheme x:Key=\"ctrl:NumericTextBox\" TargetType=\"ctrl:NumericTextBox\"(.*?)<ScrollViewer(.*?)/>", re.DOTALL), r'<ControlTheme x:Key="ctrl:NumericTextBox" TargetType="ctrl:NumericTextBox"\1<ScrollViewer\2>\n\t\t<TextPresenter x:Name="PART_TextPresenter" Text="{TemplateBinding Text, Mode=TwoWay}" CaretIndex="{TemplateBinding CaretIndex}" SelectionStart="{TemplateBinding SelectionStart}" SelectionEnd="{TemplateBinding SelectionEnd}" TextAlignment="{TemplateBinding TextAlignment}" TextWrapping="{TemplateBinding TextWrapping}" LineHeight="{TemplateBinding LineHeight}" LetterSpacing="{TemplateBinding LetterSpacing}" PasswordChar="{TemplateBinding PasswordChar}" RevealPassword="{TemplateBinding RevealPassword}" SelectionBrush="{TemplateBinding SelectionBrush}" SelectionForegroundBrush="{TemplateBinding SelectionForegroundBrush}" CaretBrush="{TemplateBinding CaretBrush}" HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" VerticalAlignment="{TemplateBinding VerticalContentAlignment}"/>\n\t</ScrollViewer>', contents)
  
  # fix avalonia specific additions.
  #contents = re.sub ("<ControlTheme x:Key=\"{x:Type Menu}\" TargetType=\"{x:Type Menu}\" >", '<ControlTheme x:Key="{x:Type Menu}" TargetType="{x:Type Menu}" BasedOn="{StaticResource {x:Type Menu}}">', contents)

  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type Button}\"(((?!</ControlTemplate>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type Button}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTemplate>', contents) # have to force a content value in buttons?
  
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type ToggleButton}\"(((?!</ControlTemplate>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type ToggleButton}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTemplate>', contents) # have to force a content value in buttons?

  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type ComboBoxItem}\"(((?!</ControlTemplate>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type ComboBoxItem}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTemplate>', contents) 
  
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type Expander}\"(((?!</ControlTemplate>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type Expander}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTemplate>', contents) 
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type Expander}\"(((?!</ControlTemplate>).)*)<Border IsVisible=\"(.*?)\"(.*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type Expander}"\2<Border IsVisible="{Binding IsExpanded, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}" \5>\6</ControlTemplate>', contents) 
  
  #contents = regex.sub ("<ContentPresenter(.*?)x:Name=\"HeaderHost\"(.*?)>", r'<ContentPresenter Content="{TemplateBinding Header}"\1x:Name="HeaderHost"\2>', contents) 

  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)TargetType=\"{x:Type ListBoxItem}\"(((?!</ControlTheme>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTheme>).)*)</ControlTheme>", regex.DOTALL), r'<ControlTheme\1TargetType="{x:Type ListBoxItem}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTheme>', contents) 

  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)TargetType=\"{x:Type ctrl:TreeViewItem}\"(.*?)ContentSource=\"Header\"(((?!ControlTheme>).)*)</ControlTheme>", regex.DOTALL), r'<ControlTheme\1TargetType="{x:Type ctrl:TreeViewItem}"\2\3</ControlTheme>', contents) 
  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)TargetType=\"{x:Type ctrl:TreeView}\"(.*?)<Setter Property=\"VerticalAlignment\" Value=\"Center\" />(((?!ControlTheme>).)*)</ControlTheme>", regex.DOTALL), r'<ControlTheme\1TargetType="{x:Type ctrl:TreeView}"\2<!-- <Setter Property=\"VerticalAlignment\" Value=\"Center\" /> -->\3</ControlTheme>', contents) 

  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)TargetType=\"{x:Type ToolTip}\"(((?!</ControlTheme>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTheme>).)*)</ControlTheme>", regex.DOTALL), r'<ControlTheme\1TargetType="{x:Type ToolTip}"\2<ContentPresenter Content="{TemplateBinding Content}" ContentTemplate="{TemplateBinding ContentTemplate}" \4>\6</ControlTheme>', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type ScrollBar}\" TargetType=\"{x:Type ScrollBar}\" >", r'<ControlTheme x:Key="{x:Type ScrollBar}" TargetType="{x:Type ScrollBar}" BasedOn="{StaticResource {x:Type ScrollBar}}" >', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type TextBox}\" TargetType=\"{x:Type TextBox}\" >", r'<ControlTheme x:Key="{x:Type TextBox}" TargetType="{x:Type TextBox}" BasedOn="{StaticResource {x:Type TextBox}}" >', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type TabControl}\" TargetType=\"{x:Type TabControl}\" >", r'<ControlTheme x:Key="{x:Type TabControl}" TargetType="{x:Type TabControl}" BasedOn="{StaticResource {x:Type TabControl}}" >', contents) 
  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type TabItem}\"(.*?)TargetType=\"{x:Type TabItem}\" >", r'<ControlTheme x:Key="{x:Type TabItem}"\1TargetType="{x:Type TabItem}" BasedOn="{StaticResource {x:Type TabItem}}" >', contents) 
  contents = regex.sub (regex.compile ("<ControlTheme([^>]*?)TargetType=\"{x:Type TabItem}\"(((?!</ControlTheme>).)*)<ContentPresenter (((?!Content=).)*?)>(((?!ControlTheme>).)*)</ControlTheme>", regex.DOTALL), r'<ControlTheme\1TargetType="{x:Type TabItem}"\2<ContentPresenter Content="{TemplateBinding Content}" \4>\6</ControlTheme>', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type ContextMenu}\" TargetType=\"{x:Type ContextMenu}\" >", r'<ControlTheme x:Key="{x:Type ContextMenu}" TargetType="{x:Type ContextMenu}" BasedOn="{StaticResource {x:Type ContextMenu}}">', contents) 
  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type Menu}\" TargetType=\"{x:Type Menu}\" >", r'<ControlTheme x:Key="{x:Type Menu}" TargetType="{x:Type Menu}" BasedOn="{StaticResource {x:Type Menu}}">', contents) 
  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type MenuItem}\" TargetType=\"{x:Type MenuItem}\" >", r'<ControlTheme x:Key="{x:Type MenuItem}" TargetType="{x:Type MenuItem}" BasedOn="{StaticResource {x:Type MenuItem}}">', contents) 
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type MenuItem}\"(((?!</ControlTemplate>).)*)<Popup (.*?)x:Name=\"SubMenuPopup\"(.*?)>(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type MenuItem}"\2<Popup \4x:Name="PART_Popup"\5>\6</ControlTemplate>', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type ToggleButton}\" TargetType=\"{x:Type ToggleButton}\" >", r'<ControlTheme x:Key="{x:Type ToggleButton}" TargetType="{x:Type ToggleButton}" BasedOn="{StaticResource {x:Type ToggleButton}}">', contents) 

  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type ComboBox}\" TargetType=\"{x:Type ComboBox}\" >", r'<ControlTheme x:Key="{x:Type ComboBox}" TargetType="{x:Type ComboBox}" BasedOn="{StaticResource {x:Type ComboBox}}">', contents) 
  contents = regex.sub ("<ControlTheme x:Key=\"{x:Type ComboBoxItem}\" TargetType=\"{x:Type ComboBoxItem}\" >", r'<ControlTheme x:Key="{x:Type ComboBoxItem}" TargetType="{x:Type ComboBoxItem}" BasedOn="{StaticResource {x:Type ComboBoxItem}}">', contents) 
  contents = regex.sub (regex.compile ("<ControlTemplate([^>]*?)TargetType=\"{x:Type ComboBox}\"(((?!</ControlTemplate>).)*)<StackPanel   />(((?!ControlTemplate>).)*)</ControlTemplate>", regex.DOTALL), r'<ControlTemplate\1TargetType="{x:Type ComboBox}"\2<ItemsPresenter Name="PART_ItemsPresenter" Margin="{DynamicResource ComboBoxDropdownContentMargin}" ItemsPanel="{TemplateBinding ItemsPanel}" />\4</ControlTemplate>', contents) 
  
  contents = regex.sub ("<RowDefinition(\s*?)/>", r'<RowDefinition Height="Auto" />', contents) 
  contents = regex.sub ("<ColumnDefinition(\s*?)/>", r'<ColumnDefinition Width="Auto" />', contents) 
  contents = regex.sub ("<RowDefinition(.*?)MinHeight=\"{TemplateBinding MinHeight}\" />", r'<RowDefinition\1 />', contents) 
  
  contents = regex.sub ("<ItemsPresenter  Margin=\"0,0,1,0\"/>", r'<ItemsPresenter  Margin="0,0,1,0" ItemsPanel="{TemplateBinding ItemsPanel}"/>', contents) 
  contents = regex.sub ("<Track (((?!Value=).)*?)>", r'<Track \1 Value="{TemplateBinding Value, Mode=TwoWay}">', contents) 

  contents = regex.sub ("<RepeatButton(.*?)x:Name=\"DecreaseRepeat\"(.*?)>", r'<RepeatButton\1x:Name="PART_LineUpButton"\2>', contents) 
  contents = regex.sub ("<RepeatButton(.*?)x:Name=\"IncreaseRepeat\"(.*?)>", r'<RepeatButton\1x:Name="PART_LineDownButton"\2>', contents) 
  contents = regex.sub ("<RepeatButton(.*?)x:Name=\"PageUp\"(.*?)>", r'<RepeatButton\1x:Name="PART_PageUpButton"\2>', contents) 
  contents = regex.sub ("<RepeatButton(.*?)x:Name=\"PageDown\"(.*?)>", r'<RepeatButton\1x:Name="PART_PageDownButton"\2>', contents) 

#  contents = regex.sub ("DragCursor=\"/Stride.Core.Presentation.Wpf;component/Resources/Cursors/CursorDrag.cur\"", r'DragCursor="avares://Stride.Core.Presentation.Avalonia/Resources/Cursors/CursorDrag.cur"', contents) 
  contents = regex.sub ("DragCursor=\"/Stride.Core.Presentation.Wpf;component/Resources/Cursors/CursorDrag.cur\"", r'', contents) # see previous line, not supported yet.

  contents = regex.sub ("x:Key=\"ItemsControl.ButtonStyleKey\"", r'x:Key="ItemsControlButtonStyleKey"', contents) 

  contents = regex.sub ("x:Key=\"ctrl:TextBox\"", r'x:Key="{x:Type ctrl:TextBox}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:NumericTextBox\"", r'x:Key="{x:Type ctrl:NumericTextBox}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:TimeSpanEditor\"", r'x:Key="{x:Type ctrl:TimeSpanEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:DateTimeEditor\"", r'x:Key="{x:Type ctrl:DateTimeEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Vector2Editor\"", r'x:Key="{x:Type ctrl:Vector2Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Vector3Editor\"", r'x:Key="{x:Type ctrl:Vector3Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Vector4Editor\"", r'x:Key="{x:Type ctrl:Vector4Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Int2Editor\"", r'x:Key="{x:Type ctrl:Int2Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Int3Editor\"", r'x:Key="{x:Type ctrl:Int3Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:Int4Editor\"", r'x:Key="{x:Type ctrl:Int4Editor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:RectangleFEditor\"", r'x:Key="{x:Type ctrl:RectangleFEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:RectangleEditor\"", r'x:Key="{x:Type ctrl:RectangleEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:RotationEditor\"", r'x:Key="{x:Type ctrl:RotationEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:MatrixEditor\"", r'x:Key="{x:Type ctrl:MatrixEditor}"', contents) 
  contents = regex.sub ("x:Key=\"ctrl:TextLogViewer\"", r'x:Key="{x:Type ctrl:TextLogViewer}"', contents) 

  contents = regex.sub ("<ImageBrush x:Key=\"ImageAdvancedEditionVector\" Source=\"/Resources/Images/gear--pencil.png\" />", r'<ImageBrush x:Key="ImageAdvancedEditionVector" Source="/Resources/Images/gear-pencil.png" />', contents) 
  contents = regex.sub ("<ImageBrush x:Key=\"ImageAdvancedEditionVector\" Source=\"/Resources/Images/calendar--pencil.png\" />", r'<ImageBrush x:Key="ImageAdvancedEditionVector" Source="/Resources/Images/calendar-pencil.png" />', contents) 

  contents = regex.sub ("Source=\"{StaticResource ImageAdvancedEditionVector}\"", r'Source="{Binding Source={StaticResource ImageAdvancedEditionVector}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageCloseWindow}\"", r'Source="{Binding Source={StaticResource ImageCloseWindow}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageMaximizeWindow}\"", r'Source="{Binding Source={StaticResource ImageMaximizeWindow}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageMinimizeWindow}\"", r'Source="{Binding Source={StaticResource ImageMinimizeWindow}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageRestoreWindow}\"", r'Source="{Binding Source={StaticResource ImageRestoreWindow}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageErrorDialog}\"", r'Source="{Binding Source={StaticResource ImageErrorDialog}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageInformationDialog}\"", r'Source="{Binding Source={StaticResource ImageInformationDialog}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageQuestionDialog}\"", r'Source="{Binding Source={StaticResource ImageQuestionDialog}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageWarningDialog}\"", r'Source="{Binding Source={StaticResource ImageWarningDialog}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImagePickDateTime}\"", r'Source="{Binding Source={StaticResource ImagePickDateTime}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageReset}\"", r'Source="{Binding Source={StaticResource ImageReset}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageLocked}\"", r'Source="{Binding Source={StaticResource ImageLocked}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageUnlocked}\"", r'Source="{Binding Source={StaticResource ImageUnlocked}, Path=Source}"', contents) 
  contents = regex.sub ("Source=\"{StaticResource ImageLength}\"", r'Source="{Binding Source={StaticResource ImageLength}, Path=Source}"', contents) 

  # Triggers. These will probably have to be managed by hand. Just comment them out.
  
  # Handle those triggers we can.
  pat = re.compile ("<ControlTheme(((?!</ControlTheme>).)*)<ControlTemplate.Triggers>(.*?)</ControlTemplate.Triggers>(((?!ControlTheme>).)*)</ControlTheme>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in reversed(list(re.finditer (pat, contents))):
      context = match.group (1)
      triggers = match.group (3)
      (styles, triggers) = rewriteTriggers (triggers, context)
      contents = contents[:match.start ()] + r"<ControlTheme" + context + r"<ControlTemplate.Triggers>" + triggers + r"</ControlTemplate.Triggers>" + match.group(4) + styles + r"</ControlTheme>" + contents[match.end ():] 
  
  pat = re.compile ("<ControlTheme(((?!</ControlTheme>).)*)<Style.Triggers>(.*?)</Style.Triggers>(((?!ControlTheme>).)*)</ControlTheme>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in reversed(list(re.finditer (pat, contents))):
      context = match.group (1)
      triggers = match.group (3)
      (styles, triggers) = rewriteTriggers (triggers, context)
      contents = contents[:match.start ()] + r"<ControlTheme" + context + r"<ControlTheme.Triggers>" + triggers + r"</ControlTheme.Triggers>" + match.group(4) + styles + r"</ControlTheme>" + contents[match.end ():] 
  
  pat = re.compile ("<ControlTemplate(((?!</ControlTemplate>).)*)<ControlTemplate.Triggers>(.*?)</ControlTemplate.Triggers>(((?!ControlTemplate>).)*)</ControlTemplate>", re.DOTALL)
  if (re.findall (pat, contents)):
    for match in reversed(list(re.finditer (pat, contents))):
      context = match.group (1)
      triggers = match.group (3)
      (styles, triggers) = rewriteTriggers (triggers, context)
      contents = contents[:match.start ()] + r"<ControlTemplate" + context + r"<ControlTemplate.Triggers>" + triggers + r"</ControlTemplate.Triggers>" + match.group(4) + "<!--" +  styles + "-->" + r"</ControlTemplate>" + contents[match.end ():] 

  contents = re.sub (re.compile ("\<ControlTemplate\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <ControlTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<DataTemplate\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <DataTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<i:Interaction\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <DataTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<Style\.Triggers>(.*?)</Style\.Triggers>", re.DOTALL), r"<!-- <ControlTheme.Triggers>\1</ControlTheme.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<ControlTheme\.Triggers>(.*?)</ControlTheme\.Triggers>", re.DOTALL), r"<!-- <ControlTheme.Triggers>\1</ControlTheme.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<Rectangle\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <Rectangle.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("<Trigger.ExitActions>(.*?)</Trigger.ExitActions>", re.DOTALL), r"<!-- <Trigger.ExitActions>\1</Trigger.ExitActions> -->", contents)
  contents = re.sub (re.compile ("<Trigger.EnterActions>(.*?)</Trigger.EnterActions>", re.DOTALL), r"<!-- <Trigger.EnterActions>\1</Trigger.EnterActions> -->", contents)

  # Fix nested comments.
  contents = removeNestedComments (contents)
  
  contents = regex.sub (regex.compile ("<!-- <ControlTheme\.Triggers>(\s*?)<Trigger Property=\"Orientation\" Value=\"Horizontal\">(\s*?)<Setter Property=\"Height\" Value=\"18\" />(\s*?)<Setter Property=\"Template\" Value=\"{StaticResource HorizontalScrollBarControlTemplate}\" />(\s*?)</Trigger>(\s*?)<Trigger Property=\"Orientation\" Value=\"Vertical\">(\s*?)<Setter Property=\"Width\" Value=\"18\" />(\s*?)<Setter Property=\"Template\" Value=\"{StaticResource VerticalScrollBarControlTemplate}\" />(\s*?)</Trigger>(\s*?)</ControlTheme\.Triggers> -->", regex.DOTALL), r'<Style Selector="^[Orientation=Horizontal]">\n<Setter Property="Height" Value="18" />\n<Setter Property="Template" Value="{StaticResource HorizontalScrollBarControlTemplate}" />\n</Style>\n<Style Selector="^[Orientation=Vertical]">\n<Setter Property="Width" Value="18" />\n<Setter Property="Template" Value="{StaticResource VerticalScrollBarControlTemplate}" />\n</Style>', contents) 

  contents = regex.sub (regex.compile ("<ControlTemplate x:Key=\"TreeExpanderToggleButton\" TargetType=\"{x:Type ToggleButton}\">(\s*?)<Grid Background=\"Transparent\">(\s*?)<Path HorizontalAlignment=\"Center\" x:Name=\"Up_Arrow\" VerticalAlignment=\"Center\" Fill=\"{StaticResource GlyphBrush}\"(\s*?)Data=\"M 0 6 V 0 l 5 3 z\" RenderTransformOrigin=\"0.5,0.5\" Stretch=\"Uniform\" StrokeThickness=\"0\"/>(\s*?)<Path IsVisible=\"false\" HorizontalAlignment=\"Center\" x:Name=\"Down_Arrow\" VerticalAlignment=\"Center\" Fill=\"{StaticResource GlyphBrush}\"(\s*?)Data=\"M 0 0 H 6 L 3 6 Z\" RenderTransformOrigin=\"0.5,0.5\" Stretch=\"Uniform\" StrokeThickness=\"0\"/>", regex.DOTALL), r'<ControlTemplate x:Key="TreeExpanderToggleButton" TargetType="{x:Type ToggleButton}">\n<Grid Background="Transparent">\n<Path HorizontalAlignment="Center" x:Name="Up_Arrow" VerticalAlignment="Center" Fill="{StaticResource GlyphBrush}" Data="M 0 6 V 0 l 5 3 z" RenderTransformOrigin="0.5,0.5" Stretch="Uniform" StrokeThickness="0" IsVisible="{Binding $parent[ToggleButton].IsChecked}"/>\n<Path HorizontalAlignment="Center" x:Name="Down_Arrow" VerticalAlignment="Center" Fill="{StaticResource GlyphBrush}" Data="M 0 0 H 6 L 3 6 Z" RenderTransformOrigin="0.5,0.5" Stretch="Uniform" StrokeThickness="0" IsVisible="{Binding !$parent[ToggleButton].IsChecked}"/>', contents) 
  
#<Style Selector="^[Orientation=Horizontal]">
        #<Setter Property="Height" Value="18" />
        #<Setter Property="Template" Value="{StaticResource HorizontalScrollBarControlTemplate}" />
    #</Style>
    #<Style Selector="^[Orientation=Vertical]">
        #<Setter Property="Width" Value="18" />
        #<Setter Property="Template" Value="{StaticResource VerticalScrollBarControlTemplate}" />
    #</Style>
    #<!-- <ControlTheme.Triggers>
      #<Trigger Property="Orientation" Value="Horizontal">
        #<Setter Property="Height" Value="18" />
        #<Setter Property="Template" Value="{StaticResource HorizontalScrollBarControlTemplate}" />
      #</Trigger>
      #<Trigger Property="Orientation" Value="Vertical">
        #<Setter Property="Width" Value="18" />
        #<Setter Property="Template" Value="{StaticResource VerticalScrollBarControlTemplate}" />
      #</Trigger>
    #</Style\.Triggers> -->  
    
  return contents

def rewriteTriggers (triggers, context):
  print ("\n\n\n\nTriggers", triggers)
  
  styles = ""
  validProperties = [ ":disabled", ":not(:empty)", ":empty", ":selected", ":pointerover", ":checked", ":not(:checked)"]
  
  pat = re.compile ("<Trigger Property=\"(.*?)\" Value=\"(.*?)\"(\s*?)>(((?!</Trigger>).)*)</Trigger>", re.DOTALL)
  if (re.findall (pat, triggers)):
    for match in reversed(list(re.finditer (pat, triggers))):
      prop = match.group (1)
      value = match.group (2)
      setters = match.group (4)
      (prop, value) = substituteTriggers (prop, value)
      print ("T:", prop, value, setters)
      if prop in validProperties:
        if value != None:
          styles += '\t<Style Selector="^[' + prop + '=' + value + ']">\n'
        else:
          styles += '\t<Style Selector="^' + prop + '">\n'
        styles += rewriteSetters (setters, context)
        styles += '\t</Style>\n'
        triggers = triggers[:match.start ()] + triggers[match.end ():]
        print (styles, triggers)
  
  return (styles, triggers)

def substituteTriggers (prop, value):
  if prop == "IsEnabled" and (value == "False" or value == "false"):
    return (":disabled", None)
  if prop == "Role" and value == "SubmenuHeader":
    return (":not(:empty)", None)
  if prop == "Role" and value == "SubmenuItem":
    return (":empty", None)
  if prop == "IsHighlighted" and value == "True":
    return (":selected", None)
  if prop == "IsMouseOver" and (value == "True" or value == "true"):
    return (":pointerover", None)
  if prop == "IsChecked" and (value == "True" or value == "true"):
    return (":checked", None)
  if prop == "IsChecked" and (value == "False" or value == "false"):
    return (":not(:checked)", None)

  return (prop, value)

def rewriteSetters (setters, context):
  
  # rewrite those with a target name.
  pat = re.compile ("<Setter ((Property|TargetName|Value)=\"([^>]*?)\"(\s*?))((Property|TargetName|Value)=\"([^>]*?)\"(\s*?))((Property|TargetName|Value)=\"([^>]*?)\"(\s*?))/>", re.DOTALL)
  print ("hthh", pat, setters)
  if (re.findall (pat, setters)):
    for match in reversed(list(re.finditer (pat, setters))):
      d = {}
      d[match.group(2)] = match.group(3)
      d[match.group(6)] = match.group(7)
      d[match.group(10)] = match.group(11)
      prop = d["Property"]
      value = d["Value"]
      target = d["TargetName"]
      cpat = re.compile ("<([^<>]*?) ([^>]*?)x:Name=\"" + target + "\"([^>]*?)>", re.DOTALL)
      print ("S:", prop, value, target, setters, context, cpat)
      
      if (re.findall (cpat, context)):
        rmatch = re.findall (cpat, context)[0]
        ttype = rmatch[0]
        print ("Found", rmatch)
        
        updatedsetter = '\t\t<Style Selector="^ /template/ ' + ttype + '#' + target + '">\n'
        updatedsetter += '\t\t\t<Setter Property="' + prop + '" Value="' + value + '"/>\n'
        updatedsetter += '\t\t</Style>\n'
        setters = setters[:match.start ()] + updatedsetter + setters[match.end ():]
        print ("Update", updatedsetter)
      else:
        setters = setters[:match.start ()] + "<!--" + match.group(0) + "-->" + setters[match.end ():]
  
  return setters

def translateXAML (sourceFile):
  
  sourceFileName = os.path.basename (sourceFile)
  sourceFileDir = os.path.dirname (sourceFile)
  name, extension = os.path.splitext (sourceFileName) 
  generatedFileDir = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"))
  generatedFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), name + ".axaml")
  generatedDiffFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), name + ".axaml" + ".diff")
  destFile = os.path.join (sourcesDir, sourceFileDir.replace ("Wpf", "Avalonia"), name + ".axaml")
  
  print ("Translating XAML", sourceFileDir, sourceFileName, destFile)

  # read source.
  contents = readSource (os.path.join (sourcesDir, sourceFileDir, sourceFileName))
  
  # various translators.
  contents = translateConstants (contents)
  contents = translateTags (contents)
  
  # write translated file.
  writeDest (contents, generatedFile, generatedDiffFile, destFile)
  
  #os.makedirs (os.path.dirname (destFile), exist_ok = True)
  #fle = open (destFile, "w")
  #fle.write (contents)
  #fle.close ()

#translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/MarkupExtensionHelper.cs")
#translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/LocalizeExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeResourceDictionary.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeController.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TextBox.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TextBoxBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UDirectoryToString.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/TemplateBrowserUserControl.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DataTypeTemplateSelector.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/SumNum.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ConverterHelper.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/DoubleExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Extensions/ImageExtensions.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/ImageExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ImageThemingUtilities.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/IconTheme.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Drawing/HslColor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeTypeExtensions.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/IconThemeSelector.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Commands/DisabledCommand.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ObjectToBool.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/OneWayValueConverter.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/Status/Views/ToolTipHelper.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/NumericToBool.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/InvertBool.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UFileToString.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/WorkProgressWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/ModalWindow.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ButtonCloseWindowBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/CloseWindowBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TextLogViewer.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Interactivity/BehaviorCollection.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/NumericTextBoxDragBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/MouseMoveCaptureBehaviorBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TreeViewItemEventArgs.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/ExpandableItemsControl.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Extensions/DependencyObjectExtensions.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/TreeViewBindableSelectedItemsBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/BindableSelectedItemsBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/DeferredBehaviorBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/BoolToParam.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/StaticResourceConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/NumericToThickness.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/ThicknessExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/MultiBinding.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/EnumValues.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/View/DefaultTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/CategoryNodeTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/AbstractTypeTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/InlinedPropertyValueTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/InlinedPropertyTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ListItemTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ListTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ArrayItemTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DataTypeTemplateSelector.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/EnumTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/SetTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DictionaryEnumKeyTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/GenericDictionaryNodeTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/NullableStructTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/UnloadableObjectTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ArrayTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DictionaryNumberKeyTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/NullableTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DictionaryStringKeyTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ObjectTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ContentReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/DictionaryTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/RangedValueTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/SettingsWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/EnumToDisplayName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/AllEqualMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/DegreeAngleSingle.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MaxNum.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/SumSize.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UFileToFileNameWithExt.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/AndMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/EmptyStringToBool.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MinNum.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ObjectToFullTypeName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/SumThickness.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MultiBindingToTuple.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ObjectToType.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/Take.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UFileToUri.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/CamelCaseTextConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MultiChained.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ObjectToTypeName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/TextToMarkdownFlowDocumentConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UnderlyingType.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/Chained.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ExtendedOrSingle.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/Multiply.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/OneWayMultiValueConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ThicknessMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/CharToString.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/FormatString.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MultiplyMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ToDouble.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ValueToUnset.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/CharToUnicode.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/IntToBool.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MultiValueConverterBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/OrMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ToLower.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/VectorEditingModeToBoolean.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ColorConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/NotSupportedTypeToTypeName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/TrimString.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/CompareNum.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/IsEqualToParam.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/NullToUnset.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/StringConcat.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/TypeToNamespace.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ItemToIndex.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/StringEquals.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/TypeToTypeName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/XOrMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/CountEnumerable.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/JoinStrings.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/NumericToSize.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/SumMultiConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/Yield.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/DateTimeToString.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/MatchType.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UFileToFileName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/VectorEditingMode.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Extensions/SystemColorExtensions.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TrimmingSource.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AbstractNodeEntryToDisplayName.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AbstractNodeEntryMatchesNodeValue.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/PropertyView.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/PropertyViewItemEventArgs.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/PropertyViewItem.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/PropertyViewItemDragDropBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/DragDropBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/IDragDropBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Trimming.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/OnComboBoxClosedWithSelectionBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/OnEventCommandBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/OnEventBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Core/AnonymousEventHandler.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/TypeToResource.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Core/FocusManager.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/CharInputBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/NumericTextBox.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/DifferentValuesToNull.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/DifferentValuesToString.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/ValueConverterBase.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/TextBoxPropertyValueValidationBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Adorners/HighlightAdornerState.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Adorners/HighlightBorderAdorner.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ToggleButtonPopupBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/ColorPicker.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/ValidateTextBoxAfterSlidingBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ChangeCursorOnSliderThumbBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TimeSpanEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/DateTimeEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Vector2Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/VectorEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/VectorEditorBase.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/NodeViewModelTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TypeMatchTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/DifferentValueToParam.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/TextBoxVectorPropertyValueValidationBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Vector3Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Vector4Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Int2Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Int3Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Int4Editor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/RectangleEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/RectangleFEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/RotationEditor.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/MatrixEditor.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/ReferenceHostDragDropBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ListBoxBindableSelectedItemsBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/FlagEnumToObservableList.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/BehaviorProperties.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/EnumToResource.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/ContentReferenceToUrl.cs")
#translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/ValueConverters/LocalizableConverter.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/ContentReferenceToAsset.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetViewUserControl.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Controls/EditableContentListBox.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/OnEventSetPropertyBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/ToolTipExtension.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/SetContentTemplateCommand.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/NameBreakingLine.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/VirtualizingTilePanel.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/SizeExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/TilePanelNavigationBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/ListBoxDragDropBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/TilePanelThumbnailPrioritizationBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/BringSelectionToViewBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/CommandBindingBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TagControl.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AssetFilterViewModelToFullDisplayName.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/FilteringComboBox.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/FilteringComboBoxSort.cs")
#translateCS ("/tmp/a.Wpf/ActivateParentPaneOnGotFocusBehavior.cs")
#translateCS ("/tmp/a.Wpf/LayoutAnchorableActivateOnLocationChangedBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/ActivateOnLocationChangedBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ActivateOnCollectionChangedBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Controls/GridLogViewer.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/PriorityBinding.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/AddItemUserControl.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Interactivity/Interaction.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/SearchComboBox.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/FrameworkElementDragDropBehavior.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/StrideDefaultAssetsPlugin.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ComputeCurveTemplateProviders.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/AnimationFrameTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/EntityComponentReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/AnimationFrameBoxTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/EntityReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/EntityComponentCollectionTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ModelComponentMaterialTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ModelNodeLinkNameTemplateProvider.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/TemplateProviders/ContentReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/GameSettingsFiltersTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/GameSettingAddConfigurationTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/ValueConverters/TimeToFrames.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/ValueConverters/NodeToCameraSlotIndex.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/Converters/CodeActionsConverter.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/SimpleCodeTextEditor.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/Converters/CodeActionToGlyphConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Commands/DisabledCommand.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/NumericTextBoxDragBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/MouseMoveCaptureBehaviorBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/CanvasView/CanvasView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/EntityHierarchyEditor/Views/EntityHierarchyEditorView.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/TrueExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/FalseExtension.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/ScriptEditorView.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/ScriptTextEditor.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/GameEditor/Services/EditorGameRecoveryService.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/GameEngineHost.cs")
#translateCS ("editor/Stride.GameStudio/View/GameStudioWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/MenuItemCloseWindowBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/SessionExplorerHelper.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TreeViewItem.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/TreeViewElementFinder.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/DragOverAutoScrollBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/TreeViewAutoExpandBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/TreeViewDragDropBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AssetToExpandedAtInitialization.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/TreeViewStopEditOnLostFocusBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/ActivateOnLogBehavior.cs")
#translateCS ("/tmp/a.Wpf/LayoutAnchorableActivateOnLogBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/PropertyViewFilteringBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ItemsControlCollectionViewBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Core/DependencyPropertyWatcher.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/PropertyViewDragDropBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/PropertyViewAutoExpandNodesBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Extensions/WindowHelper.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/VirtualizingTreePanel.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/SettingsCategoryToExpandedAtInitialization.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ShaderClassNodeMixinReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/SkeletonModelPropertyTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/SpriteFontFontNamePropertyTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ScriptVariableReferenceTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/TemplateProviders/ScriptTextEditorTemplateProvider.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/BindScriptTextEditorWorkspaceProjectIdBehavior.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/UIEditor/Views/ThicknessEditor.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/VisualScriptEditor/Converters/AvailableVariableReferenceValueConverter.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Windows/MessageDialogBase.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/MarkdownTextBlock.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Commands/UtilityCommands.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Windows/MessageBox.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Windows/CheckedMessageBox.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/CanvasView/TrackerControl.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/SelectionRectangleBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/ScaleBar.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/UnitSystem.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Commands/SystemCommand.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Commands/SystemCommands.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/ResizeBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/KeyValueGrid.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/IntExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/Commands/ControlCommands.cs")
#translateCS ("editor/Stride.Editor.Wpf/Preview/View/StridePreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/StrideTemplates.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/ProjectSelectionWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/TextBoxCloseWindowBehavior.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Windows/DialogHelper.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/ObjectBrowserUserControl.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/DoubleClickCloseWindowBehavior.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/TemplateSampleGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/UpdatePlatformsTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/UpdatePlatformsWindows.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/SolutionPlatformViewModel.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/NewGameTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ScriptTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ScriptNameWindow.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/GameTemplateWindow.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ProjectLibraryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/HeightmapFactoryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ColliderShapeHullFactoryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ColliderShapeStaticMeshFactoryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ProceduralModelFactoryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/SkyboxFactoryTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/GraphicsCompositorTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/SpriteSheetFromFileTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ModelFromFileTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/AnimationFromFileTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/VideoFromFileTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/SoundFromFileTemplateGenerator.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ModelAssetTemplateWindow.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Templates/ProjectLibraryWindow.xaml.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/ValueConverters/EntityComponentToResource.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/View/AddEntityComponentUserControl.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/ListBoxHighlightedItemBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AbstractNodeEntryToType.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetPickerWindow.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Extensions/ControlExtensions.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/NewProjectWindow.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/NotificationWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/HyperlinkCloseWindowBehavior.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/AddItemWindow.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Controls/PopupModalWindow.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/AddAssets/View/ItemTemplatesWindow.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/PackagePickerWindow.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/Components/FixAssetReferences/Views/FixAssetReferencesWindow.xaml.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/ValueConverters/AssetViewModelToUrl.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Core/ValidationRoutedEvent.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragContainer.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/DragWindow.cs")
#translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/Behaviors/DragDrop/DragDropHelper.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/UIPageEditor/Views/UIPageEditorView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/UIEditor/Views/UIEditorView.xaml.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/XamlRootExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/MarkupExtensions/GuidExtension.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/AssetCompositeGameEditor/Views/AssetCompositeHierarchyTreeViewHelper.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/AnimationPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/EntityPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/HeightmapPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/MaterialPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/ModelPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/ScenePreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/SkyboxPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/SoundPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/SpriteFontPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/SpriteSheetPreviewView.cs")
#translateCS ("editor/Stride.Assets.Presentation.Wpf/Preview/Views/TexturePreviewView.cs")
translateCS ("presentation/Stride.Core.Presentation.Wpf/Behaviors/SliderDragFromTrackBehavior.cs")


#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/CommonResources.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeSelector.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/ExpressionDarkTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/DarkSteelTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/DividedTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/LightSteelBlueTheme.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/TemplateBrowserUserControl.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/ImageDictionary.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/WorkProgressWindow.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/generic.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/DefaultPropertyTemplateProviders.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/SettingsWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetViewUserControl.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ThemeSelector.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ExpressionDark/TableflowView.ExpressionDark.normalcolor.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ExpressionDark/TableflowView.GridElementTemplates.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ExpressionDark/Resources/Common.Resources.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ExpressionDark/Resources/ExpressionDark.normalcolor.Resources.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/ExpressionDark/Resources/TableView.ExpressionDark.normalcolor.Graphics.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Themes/generic.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/AddItemUserControl.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/ImageDictionary.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/AnimationPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/EntityPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/Resources/Icons.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/Resources/ThemeScriptEditor.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/EntityHierarchyEditor/Views/EntityHierarchyEditorView.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/ScriptEditor/ScriptEditorView.xaml")
#translateXAML ("editor/Stride.GameStudio/View/GameStudioWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/MaterialPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/SkeletonPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/SpriteFontPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/UIPropertyTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/GraphicsCompositorTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/VisualScriptingTemplates.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/VisualScriptEditor/Views/GraphTemplates.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Resources/VectorResources.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/ProjectSelectionWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/ObjectBrowserUserControl.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Templates/UpdatePlatformsWindows.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Templates/ScriptNameWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Templates/GameTemplateWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Templates/ModelAssetTemplateWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Templates/ProjectLibraryWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/View/AddEntityComponentUserControl.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetPickerWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/NewProjectWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/NotificationWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/TemplateDescriptions/Views/AddItemWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/AddAssets/View/ItemTemplatesWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/PackagePickerWindow.xaml")
#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/Components/FixAssetReferences/Views/FixAssetReferencesWindow.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/AssetEditors/UIEditor/Views/UIEditorView.xaml")
#translateXAML ("editor/Stride.Editor.Wpf/Themes/Generic.xaml")
#translateXAML ("editor/Stride.Assets.Presentation.Wpf/Themes/Generic.xaml")

#PriorityBinding
#TreeViewTemplateSelector
#DragContainer
#ThemeResourceDictionary
