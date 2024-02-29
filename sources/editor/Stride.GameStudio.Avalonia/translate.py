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

  contents = re.sub ("using System.Windows.Controls;", "using Avalonia;\nusing Avalonia.Controls;\nusing Avalonia.Controls.Metadata;", contents)
  contents = re.sub ("using System.Windows.Data;", "using Avalonia.Data;\nusing Avalonia.Data.Converters;", contents)
  contents = re.sub ("using System.Windows.Markup;", "using Avalonia.Markup.Xaml;", contents)
  contents = re.sub ("using System.Windows.Input;", "using Avalonia.Input;", contents)
  contents = re.sub ("using System.Windows;", "using Avalonia;\nusing Avalonia.Controls;\n" + headers, contents)
  contents = re.sub ("using System.Xaml;", "", contents)
  
  # Remove internal, hopefully avoid boxing.
  contents = re.sub ("using Stride.Core.Presentation.Internal;", "", contents)
  
  return contents

def translateAnnotations (contents):
  contents = re.sub ("\[MarkupExtensionReturnType\(.*\)\]", "", contents)
  contents = re.sub ("\[ValueConversion\(.*\)\]", "", contents)
  return contents

def translateNames (contents):
  #contents = re.sub ("DependencyProperty", "AvaloniaProperty", contents)
  contents = re.sub ("ICommand", "ICommandSource", contents)
  contents = re.sub ("RoutedCommand", "ICommandSource", contents) # provisional.
  contents = re.sub ("\.CanExecute\(", ".Command.CanExecute(", contents) # provisional.
  contents = re.sub ("\.Execute\(", ".Command.Execute(", contents) # provisional.
  
  
  contents = re.sub ("CancelRoutedEventHandler", "EventHandler<CancelRoutedEventArgs>", contents)
  contents = re.sub ("ValidationRoutedEventHandler<string>", "EventHandler<CancelRoutedEventArgs>", contents) # provisional
  contents = re.sub ("ExecutedRoutedEventArgs", "RoutedEventArgs", contents)
  contents = re.sub (" RoutedEventHandler ", " EventHandler<RoutedEventArgs> ", contents)
  contents = re.sub ("DependencyPropertyChangedEventArgs e", "AvaloniaPropertyChangedEventArgs e", contents)
  contents = re.sub ("System.Windows.Controls.TextBox", "Avalonia.Controls.TextBox", contents)
  contents = re.sub ("DependencyObject", "AvaloniaObject", contents)
  contents = re.sub ("\.ProvideValue\(.*\)", "", contents) # is this true in general?

  contents = re.sub ("Keyboard.Focus\(this\);", "this.Focus ();", contents)


  contents = re.sub ("BooleanBoxes.FalseBox", "false", contents)
  contents = re.sub ("BooleanBoxes.TrueBox", "true", contents)
  contents = re.sub ("value.Box\(\)", "value", contents)
  return contents

# Translate the various forms of styled property.
def translateProperties (contents):

  # Handle DependencyProperty.Register
  # Without initializing argument
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4);", contents)
  
  # Without handler.
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\(([^,\)]*?)\)\);")
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7);", contents)
  
  pat = re.compile ("public static readonly DependencyProperty (.*?)(\s*)\=(\s*)DependencyProperty.Register\((.*?), typeof\((.*?)\), typeof\((.*?)\), new PropertyMetadata\((.*?), (.*?)\)\);", re.DOTALL)
  commands = ""
  classname = ""
  for match in re.findall (pat, contents):
    #print (match)
    commands += "\t\t\t" + match[0] + ".Changed.AddClassHandler<" + match[5] + ">(" + match[7] + ");\n"
    classname = match[5];
  contents = re.sub (pat, r"public static readonly StyledProperty<\5> \1 = StyledProperty<\5>.Register<\6, \5>(\4, \7);", contents)
  
  # patch in commands to an existing static constructor.
  if len (commands) > 0:
    #print ("Match", "static " + classname + "(\s*)\(\)")
    contents = re.sub (re.compile ("static " + classname + "(\s*)\(\)(\s*){", re.DOTALL), r"static " + classname + "()\n\t\t{\n" + commands, contents)
  
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
  
  # Mouse
  pat = re.compile ("protected override void OnMouseDown\(MouseButtonEventArgs e\)") # no call to base.
  contents = re.sub (pat, r"protected virtual void OnPointerPressed(PointerPressedEventArgs e)", contents)
  pat = re.compile ("base.OnMouseDown\(e\);") # just the call to base.
  contents = re.sub (pat, r"base.OnPointerPressed(e);", contents)
  
  # OnApplyTemplate
  pat = re.compile ("public override void OnApplyTemplate\(\)(\s*){(\s*)base.OnApplyTemplate\(\);")
  contents = re.sub (pat, r"protected override void OnApplyTemplate(TemplateAppliedEventArgs e)\n\t\t{\n\t\t\tbase.OnApplyTemplate(e);", contents)
  
  
  return contents

# Translate a .cs file from Wpf to Avalonia equivalent.
def translateCS (sourceFile):
  
  sourceFileName = os.path.basename (sourceFile)
  sourceFileDir = os.path.dirname (sourceFile)
  generatedFileDir = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"))
  generatedFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), sourceFileName)
  generatedDiffFile = os.path.join (generatedDir, sourceFileDir.replace ("Wpf", "Avalonia"), sourceFileName + ".diff")
  destFile = os.path.join (sourcesDir, sourceFileDir.replace ("Wpf", "Avalonia"), sourceFileName)
  
  #print ("Translating", sourceFileDir, sourceFileName, generatedFileDir, generatedFile, generatedDiffFile, destFile)

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
  contents = re.sub ("BitmapScalingMode=\"NearestNeighbor\"", r'BitmapInterpolationMode="LowQuality"', contents)
  
  # As many versions of the visibility property, given that we're translating 3 states, to boolean.
  contents = re.sub ("Visibility=\"\{Binding (.*), Converter=\{sd:VisibleOrCollapsed\}\}\"", r'IsVisible="{Binding \1}"', contents)
  contents = re.sub ("Visibility=\"Hidden\"", r'IsVisible="false"', contents)
  contents = re.sub ("Visibility=\"Collapsed\"", r'IsVisible="false"', contents)

  # Tooltip
  contents = re.sub ("Tooltip=\"(.*?)\"", r'Tooltip.Tip="\1"', contents)

  # ContentSource  
  contents = re.sub ("ContentSource=\"(.*?)\"", r'ContentTemplate="\1"', contents)

  # SnapToDevicePixels, AllowsTransparency. Elements that don't seem to have an equivalent.
  contents = re.sub ("SnapsToDevicePixels=\"(.*?)\"", "", contents)
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"(.*?)\" />", "", contents)
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"True\"/>", "", contents)
  contents = re.sub ("AllowsTransparency=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"Cycle\"", "", contents)  # possibly investigate WrapSelection?
  contents = re.sub (re.compile ("<Setter Property=\"WindowChrome.WindowChrome\">(.*?)</Setter>", re.DOTALL), "", contents)  
  contents = re.sub ("WindowChrome.IsHitTestVisibleInChrome=\"(.*?)\"", "", contents)

  # ResourceDictionary with source.
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\" /\>", r'<ResourceInclude Source="\1.axaml" />', contents)

  # Dropshadowbitmap
  contents = re.sub ("<DropShadowBitmapEffect (.*?)/>", r'<DropShadowEffect \1/>', contents) # one line style

  
  # Styles become various forms of Theme.
  contents = re.sub ("\<Style TargetType=\"(.*?)\"(.*?)\/\>", r'<ControlTheme TargetType="\1">\2</ControlTheme>', contents) # one line style

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
  contents = re.sub (re.compile ("\<Style\.Triggers>(.*?)\.Triggers>", re.DOTALL), r"<!-- <ControlTheme.Triggers>\1\.Triggers> -->", contents)
  
  # Fix nested comments.
  contents = removeNestedComments (contents)
  
  # FIXME
  # Squash a few functions that are not available yet - but will be.
  contents = re.sub (re.compile ("\<view:TreeViewTemplateSelector(.*)?</view:TreeViewTemplateSelector(.*?)>", re.DOTALL), "", contents)
  contents = re.sub ("sd:PriorityBinding", "Binding", contents)
  contents = re.sub ("<Setter Property=\"IsCheckable\" Value=\"True\" />", "", contents)
  
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
translateCS ("presentation/Stride.Core.Presentation.Wpf/ValueConverters/UDirectoryToString.cs")

#translateXAML ("editor/Stride.Core.Assets.Editor.Wpf/View/CommonResources.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeSelector.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/ExpressionDarkTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/DarkSteelTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/DividedTheme.xaml")
#translateXAML ("presentation/Stride.Core.Presentation.Wpf/Themes/Overrides/LightSteelBlueTheme.xaml")

#PriorityBinding
#TreeViewTemplateSelector
#DragContainer
#ThemeResourceDictionary
