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

  contents = re.sub ("using System.Windows.Controls.Primitives;", "using Avalonia.Controls.Primitives;", contents)
  contents = re.sub ("using System.Windows.Controls;", "using Avalonia;\nusing Avalonia.Controls;\nusing Avalonia.Controls.Metadata;", contents)
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
  contents = re.sub ("ICommand ", "ICommandSource ", contents)
  contents = re.sub ("<ICommand>", "<ICommandSource>", contents)
  contents = re.sub (", ICommand>", ", ICommandSource>", contents)
  contents = re.sub ("\(ICommand\)", "(ICommandSource)", contents)
  contents = re.sub ("as ICommand;", " as ICommandSource;", contents)
  contents = re.sub ("RoutedCommand", "ICommandSource", contents) # provisional.
  contents = re.sub ("Command\.CanExecuteChanged \-\=", "Command.Command.CanExecuteChanged -=", contents) # provisional.
  contents = re.sub ("Command\.CanExecuteChanged \+\=", "Command.Command.CanExecuteChanged +=", contents) # provisional.
  contents = re.sub ("\.CanExecute\(", ".Command.CanExecute(", contents) # provisional.
  contents = re.sub ("\.Execute\(", ".Command.Execute(", contents) # provisional.
  
  contents = re.sub ("static DependencyProperty", "static AvaloniaProperty", contents) # provisional.
  contents = re.sub ("DependencyProperty property", "AvaloniaProperty property", contents) # provisional.
  contents = re.sub ("\(DependencyProperty\)", "(AvaloniaProperty)", contents)
  contents = re.sub ("<DependencyProperty>", "<AvaloniaProperty>", contents)
  contents = re.sub (" DependencyProperty ", " AvaloniaProperty ", contents)
  contents = re.sub ("\(DependencyProperty dependencyProperty\)", "(AvaloniaProperty dependencyProperty)", contents)
  
  contents = re.sub ("CancelRoutedEventHandler", "EventHandler<CancelRoutedEventArgs>", contents)
  contents = re.sub ("RoutedPropertyChangedEventHandler<double>", "EventHandler<RoutedEventArgs>", contents)
  contents = re.sub ("ValidationRoutedEventHandler<string>", "EventHandler<CancelRoutedEventArgs>", contents) # provisional
  contents = re.sub ("ExecutedRoutedEventArgs", "RoutedEventArgs", contents)
  contents = re.sub (" RoutedEventHandler ", " EventHandler<RoutedEventArgs> ", contents)
  contents = re.sub ("DependencyPropertyChangedEventArgs e", "AvaloniaPropertyChangedEventArgs e", contents)
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
  contents = re.sub (": FrameworkElement", ": Control", contents)
  contents = re.sub ("typeof\(FrameworkElement\)", "typeof(Control)", contents)
  contents = re.sub ("private FrameworkElement", "private Control", contents)
  contents = re.sub ("UIElement\.", "Control.", contents) 
  contents = re.sub (" UIElement ", " Control ", contents) 
  contents = re.sub (": UIElement", ": Control", contents)
  contents = re.sub ("as UIElement", "as Control", contents)
  contents = re.sub ("\(UIElement\)", "(Control)", contents)
  contents = re.sub ("EventManager.GetRoutedEvents\(\)", "RoutedEventRegistry.Instance.GetAllRegistered ()", contents)

  contents = re.sub ("Keyboard.Focus\(this\);", "this.Focus ();", contents)
  contents = re.sub ("Keyboard.FocusedElement", "TopLevel.GetTopLevel(this).FocusManager.GetFocusedElement ()", contents)

  contents = re.sub ("Application.Current.TryFindResource\(value\)", "ResourceNodeExtensions.FindResource(Application.Current, value)", contents)

  contents = re.sub ("Cursors.SizeWE", "new Cursor(StandardCursorType.SizeWestEast)", contents)

  contents = re.sub (" ImageSource ", " IImage ", contents)
  contents = re.sub ("\(ImageSource ", "(IImage ", contents)
  contents = re.sub (" BitmapScalingMode ", r' BitmapInterpolationMode ', contents)
  contents = re.sub ("BitmapScalingMode.Unspecified", r' BitmapInterpolationMode.Unspecified', contents)
  contents = re.sub ("RenderOptions.SetBitmapScalingMode", r' RenderOptions.SetBitmapInterpolationMode', contents)

  contents = re.sub ("colorPickerRenderSurface\.Fill = new DrawingBrush\(new ImageDrawing\(BitmapSource.Create\(width, height, 96, 96, pf, null, rawImage, rawStride\), new Rect\(0.0f, 0.0f, width, height\)\)\);", r'int size = Marshal.SizeOf(rawImage[0]) * rawImage.Length;\n\t\t\t\t\tIntPtr pnt = Marshal.AllocHGlobal(size);\n\t\t\t\t\tMarshal.Copy(rawImage, 0, pnt, rawImage.Length);\n\t\t\t\t\tcolorPickerRenderSurface.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(pf, AlphaFormat.Premul, pnt, new PixelSize (width, height), new Vector (96, 96), rawStride));\n\t\t\t\t\tMarshal.FreeHGlobal(pnt);', contents)


  contents = re.sub ("= (.*?)\.FindVisualChildOfType\<(.*?)\>\(\);", r'= Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<\2>(\1);', contents)
  contents = re.sub ("!(.*?)\.FindVisualChildOfType\<(.*?)\>\(\)", r'!Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<\2>(\1)', contents)
  contents = re.sub ("= (.*?)\.FindVisualChildrenOfType\<(.*?)\>\(\);", r'= Avalonia.VisualTree.VisualExtensions.GetVisualChildren(\1).Where (x => x is \2).Select (x => (\2) x);', contents)
  contents = re.sub ("\?\? (.*?)\.FindVisualParentOfType\<(.*?)\>\(\);", r'?? Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<\2>((Visual) \1);', contents)
  contents = re.sub ("LogicalTreeHelper\.GetChildren", r'LogicalExtensions.GetLogicalChildren', contents)
  contents = re.sub ("LogicalTreeHelper\.GetParent", r'LogicalExtensions.GetLogicalParent', contents)
  contents = re.sub ("VisualTreeHelper\.GetParent", r'Avalonia.VisualTree.VisualExtensions.GetVisualParent', contents)

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
  contents = re.sub ("FlowDocument", r"string", contents)
  contents = re.sub ("TextBox\.Document", r"TextBox.Text", contents)

  contents = re.sub ("GetTemplateChild\(\"(.*?)\"\) as (.*?);", r'e.NameScope.Find<\2>("\1");', contents)
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
  
  contents = re.sub (", IAttachedObject", "", contents)
  contents = re.sub ("Microsoft\.Xaml\.Behaviors\.Interaction\.GetBehaviors", "Avalonia.Xaml.Interactivity.Interaction.GetBehaviors", contents)
  
  contents = re.sub ("SelectionMode\.Extended", "SelectionMode.Multiple", contents)

  contents = re.sub ("object Convert\(object\[\] values, Type targetType, object parameter, CultureInfo culture\)", r"object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)", contents)
  contents = re.sub ("object Convert\(\[NotNull\] object\[\] values, Type targetType, object parameter, CultureInfo culture\)", r"object? Convert([NotNull] IList<object?> values, Type targetType, object? parameter, CultureInfo culture)", contents)

  contents = re.sub ("Binding\.DoNothing", "BindingOperations.DoNothing", contents)

  contents = re.sub ("FontWeights\.", "FontWeight.", contents)

  contents = re.sub ("DefaultStyleKeyProperty.OverrideMetadata\(typeof\((.*?)\), new FrameworkPropertyMetadata\(typeof\((.*?)\)\)\);", "", contents)

  contents = re.sub ("FrameworkPropertyMetadataOptions\.BindsTwoWayByDefault", "defaultBindingMode : BindingMode.TwoWay", contents)

  contents = re.sub ("var addChild = \(IAddChild\)this;", "", contents)
  contents = re.sub ("addChild\.AddChild", "Bindings.Add", contents)
  return contents

# Translate the various forms of styled property.
def translateProperties (contents):

  commands = ""
  classname = ""

  # Handle DependencyProperty.Register
  # Without initializing argument
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4); // T1", contents)
  
  # PropertyMetadata, one argument, function call.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8)); // T2A", contents)

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

  # 1 argument to FrameworkPropertyMetadata, function call
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\(([^,\)]*?)\)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7(\8)); // T6A", contents)

  # 1 argument to FrameworkPropertyMetadata, typecast.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(\(([^,\)]*?)\)([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, (\7)\8); // T6B", contents)

  # 1 argument to FrameworkPropertyMetadata
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T6", contents)

  # 2 arguments to FrameworkPropertyMetadata, second being a bind TwoWay
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), FrameworkPropertyMetadataOptions\.BindsTwoWayByDefault\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7, defaultBindingMode : BindingMode.TwoWay); // T7", contents)

  # 2 argument to FrameworkPropertyMetadata
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T8", contents)

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
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new FrameworkPropertyMetadata\(([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?), ([^,\)]*?)\)\);")
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[8] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7); // T9", contents)

  
  # Direct properties
  
  pat = re.compile ("public static readonly DependencyPropertyKey (.*?)(\s*)\=(\s*)DependencyProperty.RegisterReadOnly\(nameof\((.*?)\), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\)\);", re.DOTALL)
  # set up backing variables.
  if (re.findall (pat, contents)):
    for match in re.findall (pat, contents):
      vpat = "public " + match[4] + " " + match[3] + " { get { return \(" + match[4] + "\)GetValue\(" + match[0] + ".DependencyProperty\); } private set { SetValue\(" + match[0] + ", value\); } }"
      vsub = "private " + match[4] + " _" + match[3] + ";\n\t\tpublic " + match[4] + " " + match[3] + " { get { return _" + match[3] + "; } private set { SetAndRaise(" + match[0] + ", ref _" + match[3] + ", value); } }"
      #print (match, vpat, vsub)
      contents = re.sub (vpat, vsub, contents)
    contents = re.sub (pat, r"public static readonly DirectProperty<\6, \5> \1 = AvaloniaProperty.RegisterDirect<\6, \5>(nameof (\4), o => o.\4); // T10", contents)
  

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
    
    
    
    
  # handle RegisterAttached.
  contents = re.sub (re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.RegisterAttached\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\((.*?)\)\);", re.DOTALL), r"public static readonly AttachedProperty<\5> \1 = AvaloniaProperty<\5>.RegisterAttached<\6, Control, \5>(\4, \7);", contents)
  
  
  
  
  
  # Routed events.
  pat = re.compile ("public static readonly RoutedEvent (.*?)(\s*)\=(\s*)EventManager.RegisterRoutedEvent\((.*?), RoutingStrategy\.(.*?), typeof\(([^,\)]*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly RoutedEvent \1 = RoutedEvent.Register<\7, RoutedEventArgs>(\4, RoutingStrategies.\5);", contents)
  
  
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
  contents = re.sub (pat, r"protected virtual void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("base.OnMouseDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)
  pat = re.compile ("protected override void OnMouseLeftButtonDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected virtual void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("base.OnMouseLeftButtonDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)
  pat = re.compile ("\(object sender, MouseEventArgs e\)") # just the call to base.
  contents = re.sub (pat, r"(object sender, PointerEventArgs e)", contents)
  pat = re.compile ("protected override void OnMouseLeave\(MouseEventArgs e\)") # just the call to base.
  contents = re.sub (pat, r"protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)", contents)
  pat = re.compile ("base.OnMouseLeave\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerCaptureLost(e);", contents)
  
  contents = re.sub ("MouseButtonEventArgs e", "PointerEventArgs e", contents)
  contents = re.sub ("MouseEventArgs e", "PointerEventArgs e", contents)

  contents = re.sub ("e.LeftButton == MouseButtonState.Pressed", "e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed", contents)
  contents = re.sub ("e.LeftButton == MouseButtonState.Released", "!e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed", contents)
  contents = re.sub ("\.IsMouseCaptured", ".IsPointerOver", contents)
  contents = re.sub ("\.MouseDown", ".PointerPressed", contents)
  contents = re.sub ("\.MouseUp", ".PointerReleased", contents)
  contents = re.sub ("\.MouseMove", ".PointerMoved", contents)
  
  # OnApplyTemplate
  pat = re.compile ("public override void OnApplyTemplate\(\)(\s*){(\s*)base.OnApplyTemplate\(\);")
  contents = re.sub (pat, r"protected override void OnApplyTemplate(TemplateAppliedEventArgs e)\n\t\t{\n\t\t\tbase.OnApplyTemplate(e);", contents)
  pat = re.compile ("public override void OnApplyTemplate\(\)")
  contents = re.sub (pat, r"protected override void OnApplyTemplate(TemplateAppliedEventArgs e)", contents)
  pat = re.compile ("base.OnApplyTemplate\(\);")
  contents = re.sub (pat, r"base.OnApplyTemplate(e);", contents)
  if re.findall ("OnApplyTemplate", contents):
    contents = re.sub (": Control", r": TemplatedControl", contents)
  
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
  
  # Bitmap image
  contents = re.sub ("<BitmapImage x:Key=\"(.*?)\" UriSource=\"pack://application:,,,/Stride.Core.Presentation.Wpf;component/Resources/(.*?)\" />", r'<ImageBrush x:Key="\1" Source="/Resources/\2" />', contents)  
  contents = re.sub ("<BitmapImage x:Key=\"(.*?)\" UriSource=\"\.\./Resources/(.*?)\" />", r'<ImageBrush x:Key="\1" Source="/Resources/\2" />', contents)  
  contents = re.sub ("BitmapScalingMode=\"NearestNeighbor\"", r'BitmapInterpolationMode="LowQuality"', contents)
  
  # As many versions of the visibility property, given that we're translating 3 states, to boolean.
  contents = re.sub ("Visibility=\"\{Binding (.*), Converter=\{sd:VisibleOrCollapsed\}\}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"Hidden\"", r'IsVisible="false"', contents)
  contents = re.sub ("Visibility=\"Collapsed\"", r'IsVisible="false"', contents)
  contents = re.sub ("Visibility=\"{Binding (.*?), Converter={sd:Chained {sd:InvertBool}, {sd:VisibleOrCollapsed}}}\"", r'IsVisible="{Binding \1, Converter={sd:InvertBool}}"', contents)

  # Tooltip
  contents = re.sub ("ToolTip=\"(.*?)\"", r'ToolTip.Tip="\1"', contents)
  contents = re.sub ("Property=\"ToolTip\"", r'Property="ToolTip.Tip"', contents)

  # Case sensitive ok.
  contents = re.sub ("DialogResult=\"Ok\"", r'DialogResult="Ok"', contents)

  # ContentSource  
  contents = re.sub ("ContentSource=\"(.*?)\"", r'ContentTemplate="\1"', contents)
  contents = re.sub ("ContentTemplateSelector=\"(.*?)\"", r'ContentTemplate="\1"', contents)

  # SnapToDevicePixels, AllowsTransparency. Elements that don't seem to have an equivalent.
  contents = re.sub ("SnapsToDevicePixels=\"(.*?)\"", "", contents)
  contents = re.sub ("CanContentScroll=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"(.*?)\"", "", contents)
  
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"(.*?)\" />", "", contents)
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"True\"/>", "", contents)
  contents = re.sub ("AllowsTransparency=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"Cycle\"", "", contents)  # possibly investigate WrapSelection?
  contents = re.sub (re.compile ("<Setter Property=\"WindowChrome.WindowChrome\">(.*?)</Setter>", re.DOTALL), "", contents)  
  contents = re.sub ("WindowChrome.IsHitTestVisibleInChrome=\"(.*?)\"", "", contents)
  contents = re.sub ("DashCap=\"Flat\"", "", contents)
  contents = re.sub ("d:DataContext=\"{d:DesignInstance (.*?)}\"", "", contents)

  # xmlsn:i
  contents = re.sub ("xmlns:i=\"http://schemas.microsoft.com/xaml/behaviors\"", "xmlns:i=\"clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity\"", contents)
  
  
  # ResourceDictionary with source.
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\" /\>", r'<ResourceInclude Source="\1.axaml" />', contents)
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\"/\>", r'<ResourceInclude Source="\1.axaml" />', contents)

  # Dropshadowbitmap
  contents = re.sub ("<DropShadowBitmapEffect (.*?)/>", r'<DropShadowEffect \1/>', contents) # one line style

  # SystemColors
  contents = re.sub ("\{DynamicResource \{x:Static SystemColors\.ActiveCaptionTextBrushKey\}\}", r'{StaticResource ActiveCaptionTextBrushKey}', contents) # one line style  
  
  # Styles become various forms of Theme.
  contents = re.sub ("\<Style TargetType=\"(.*?)\"(.*?)\/\>", r'<ControlTheme TargetType="\1">\2</ControlTheme>', contents) # one line style
  contents = re.sub ("sd:TextBox.Style", r'sd:TextBox.Theme', contents) # one line style
  contents = re.sub ("ListBox.ItemContainerStyle", r'ListBox.ItemContainerTheme', contents) # one line style
  contents = re.sub ("ItemsControl.ItemContainerStyle", r'ItemsControl.ItemContainerTheme', contents) # one line style

  # FIXME: require TargetType.
  pat = regex.compile ("<Style(\s[^>]*)*>(((?R)|.)*?)<\/Style>", regex.DOTALL)
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # 1 level of nesting.
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # second level of nesting.
  
  # Replace access to styles.
  contents = re.sub ("Style=\"(.*?)\"", r'Theme="\1"', contents)
  
  # Ensure all controlthemes have x:Key
  pat = re.compile ("<ControlTheme TargetType=\"(.*?)\" (.*?)>")
  contents = re.sub (pat, r'<ControlTheme x:Key="\1" TargetType="\1" \2>', contents)
  
  # Triggers. These will probably have to be managed by hand. Just comment them out.
  contents = re.sub (re.compile ("\<ControlTemplate\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <ControlTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<DataTemplate\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <DataTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<i:Interaction\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <DataTemplate.Triggers>\1.Triggers> -->", contents)
  contents = re.sub (re.compile ("\<Style\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <ControlTheme.Triggers>\1\.Triggers> -->", contents)
  
  # Fix nested comments.
  contents = removeNestedComments (contents)

  # Fontweight
  contents = re.sub ("FontWeights\.", "FontWeight.", contents)
  contents = re.sub ("FontTheme=\"(.*?)\"", r'FontStyle="\1"', contents)
  
  # Drawing brush
  contents = re.sub ("<DrawingBrush Viewport=\"(.*?)\"", "<DrawingBrush", contents)
  contents = re.sub ("ViewportUnits=\"(.*?)\"", "", contents)
  
  # Grid
  contents = re.sub ("MinWidth=\"{TemplateBinding ActualWidth}\"", "", contents)
  
  # Control
  contents = re.sub ("Control.HorizontalContentAlignment}", "ContentControl.HorizontalContentAlignment}", contents)
  contents = re.sub ("Control.VerticalContentAlignment}", "ContentControl.VerticalContentAlignment}", contents)
  
  # Combobox
  contents = re.sub ("<ComboBox Theme=\"(.*?)\" Text=\"(.*?)\"", r'<ComboBox Theme="\1" PlaceholderText="\2"', contents)
  
  # Templates.
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
  contents = re.sub ("sd:PriorityBinding", "Binding", contents)
  contents = re.sub ("<Setter Property=\"IsCheckable\" Value=\"True\" />", "", contents)
  contents = re.sub (re.compile ("<Storyboard(.*?)</Storyboard>", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("<Setter Property=\"Visibility\" Value=\"(.*?)\"/>", re.DOTALL), "", contents)
  contents = re.sub (re.compile (" IsEnabled=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile (" Visibility=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("DisplayMemberPath=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("AdornerStoryboard=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("DisplayDropAdorner=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("StaysOpen=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("SelectedValuePath=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("ContentStringFormat=\"(.*?)\"", re.DOTALL), "", contents)
  contents = re.sub (re.compile ("IsEditable=\"(.*?)\"", re.DOTALL), "", contents) # might need to look for the FluentAvalonia editable combobox if this is true?
  contents = re.sub (re.compile ("<Setter Property=\"IsEditable\" Value=\"(.*?)\"/>", re.DOTALL), "", contents) 
  
  return contents

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
translateCS ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetViewUserControl.xaml.cs")


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
translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/AssetViewUserControl.xaml")

#PriorityBinding
#TreeViewTemplateSelector
#DragContainer
#ThemeResourceDictionary
