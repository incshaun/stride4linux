import os
import re

sourcesDir = "../../../sources/"

# Replace the using statements.
def translateHeaders (contents):

  contents = re.sub ("using System.Windows;", "using Avalonia;", contents)
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
  
  sourceFileName =  os.path.basename (sourceFile)
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

translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/MarkupExtensionHelper.cs")
translateCS ("presentation/Stride.Core.Translation.Presentation.Wpf/MarkupExtensions/LocalizeExtension.cs")

