import os
import re
import regex

sourcesDir = "../../../sources/"

# Replace the using statements.
def translateHeaders (contents):

  contents = re.sub ("using System.Windows;", "using Avalonia;\nusing Avalonia.Controls;", contents)
  contents = re.sub ("using System.Windows.Data;", "using Avalonia.Data;\nusing Avalonia.Data.Converters;", contents)
  contents = re.sub ("using System.Windows.Markup;", "using Avalonia.Markup.Xaml;", contents)
  contents = re.sub ("using System.Xaml;", "", contents)
  return contents

def translateAnnotations (contents):
  contents = re.sub ("\[MarkupExtensionReturnType\(.*\)\]", "", contents)
  contents = re.sub ("\[ValueConversion\(.*\)\]", "", contents)
  return contents

def translateNames (contents):
  contents = re.sub ("DependencyProperty", "AvaloniaProperty", contents)
  contents = re.sub ("\.ProvideValue\(.*\)", "", contents) # is this true in general?
  return contents

# Translate a .cs file from Wpf to Avalonia equivalent.
def translateCS (sourceFile):
  
  sourceFileName = os.path.basename (sourceFile)
  sourceFileDir = os.path.join (sourcesDir, os.path.dirname (sourceFile))
  destFile = os.path.join (sourceFileDir.replace ("Wpf", "Avalonia"), sourceFileName)
  
  print ("Translating", sourceFileDir, sourceFileName, destFile)

  # read source.
  fle = open (os.path.join (sourceFileDir, sourceFileName), "r")
  contents = fle.read ()
  fle.close ()
  
  contents = translateHeaders (contents)
  contents = translateAnnotations (contents)
  contents = translateNames (contents)
    
  # write translated file.
  os.makedirs (os.path.dirname (destFile), exist_ok = True)
  fle = open (destFile, "w")
  fle.write (contents)
  fle.close ()

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

def translateProperties (contents):
  
  # Currently don't have a way to deal with PopupAnimation.
  contents = re.sub ("PopupAnimation=\".*\"", "", contents)
  
  # As many versions of the visibility property, given that we're translating 3 states, to boolean.
  contents = re.sub ("Visibility=\"\{Binding (.*), Converter=\{sd:VisibleOrCollapsed\}\}\"", r'IsVisible="{Binding \1}"', contents)

  # ContentSource  
  contents = re.sub ("ContentSource=\"(.*?)\"", r'ContentTemplate="\1"', contents)

  # SnapToDevicePixels, AllowsTransparency. Elements that don't seem to have an equivalent.
  contents = re.sub ("SnapsToDevicePixels=\"(.*?)\"", "", contents)
  contents = re.sub ("<Setter Property=\"SnapsToDevicePixels\" Value=\"True\"/>", "", contents)
  contents = re.sub ("AllowsTransparency=\"(.*?)\"", "", contents)
  contents = re.sub ("KeyboardNavigation.DirectionalNavigation=\"Cycle\"", "", contents)  # possibly investigate WrapSelection?


  # ResourceDictionary with source.
  contents = re.sub ("\<ResourceDictionary Source=\"(.*).xaml\" /\>", r'<ResourceInclude Source="\1.axaml" />', contents)

  # Dropshadowbitmap
  contents = re.sub ("<DropShadowBitmapEffect (.*?)/>", r'<DropShadowEffect \1/>', contents) # one line style

  
  # Styles become various forms of Theme.
  contents = re.sub ("\<Style TargetType=\"(.*?)\"(.*?)\/\>", r'<ControlTheme TargetType="\1">\2</ControlTheme>', contents) # one line style

  pat = regex.compile ("<Style(\s[^>]*)*>(((?R)|.)*?)<\/Style>", regex.DOTALL)
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # 1 level of nesting.
  contents = regex.sub (pat, r'<ControlTheme\1>\2</ControlTheme>', contents) # second level of nesting.
  
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
  sourceFileDir = os.path.join (sourcesDir, os.path.dirname (sourceFile))
  name, extension = os.path.splitext (sourceFileName) 
  destFile = os.path.join (sourceFileDir.replace ("Wpf", "Avalonia"), name + ".axaml")
  
  print ("Translating XAML", sourceFileDir, sourceFileName, destFile)

  # read source.
  fle = open (os.path.join (sourceFileDir, sourceFileName), "r")
  contents = fle.read ()
  fle.close ()
  
  # various translators.
  contents = translateConstants (contents)
  contents = translateProperties (contents)
  
  # write translated file.
  os.makedirs (os.path.dirname (destFile), exist_ok = True)
  fle = open (destFile, "w")
  fle.write (contents)
  fle.close ()

#translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/MarkupExtensionHelper.cs")
#translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/LocalizeExtension.cs")
#translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeResourceDictionary.cs")
translateCS ("presentation/Stride.Core.Presentation.Wpf/Themes/ThemeController.cs")

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
