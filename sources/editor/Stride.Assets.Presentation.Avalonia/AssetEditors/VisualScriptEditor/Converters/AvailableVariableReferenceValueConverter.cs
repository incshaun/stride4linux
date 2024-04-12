// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stride.Core.Presentation.ValueConverters;
using System.Collections.Generic;
using Stride.Assets.Presentation.AssetEditors.VisualScriptEditor;

namespace Stride.Assets.Presentation.AssetEditors.VisualScriptEditor.Converters
{
    public class AvailableVariableReferenceValueConverter : OneWayMultiValueConverter<AvailableVariableReferenceValueConverter>
    {
        public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count != 2)
                throw new InvalidOperationException("This multi converter must be invoked with two elements");

            var semanticModel = values[0] as SemanticModel;
//             var methodViewModel = values[1] as VisualScriptMethodEditorViewModel;
// FIXME - requires renabling this class in the project file.
            

//             if (semanticModel == null || methodViewModel == null)
//                 return null;
// 
//             // Try to find method in the syntax tree with same Id as our methodViewModel
//             var methodId = methodViewModel.Method.Method.Id.ToString();
//             var syntaxTree = semanticModel.SyntaxTree;
//             
//             // Use annotation to find same method
//             var methodNode = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault(x => x.GetAnnotations("Method").FirstOrDefault()?.Data == methodId);
// 
//             // Find start span of the method body
//             var firstStatement = methodNode?.Body?.Statements.FirstOrDefault();
//             if (firstStatement == null)
//                 return null;
// 
//             var startSpan = firstStatement.SpanStart;
// 
//             return semanticModel.LookupSymbols(startSpan).Where(x => x is IFieldSymbol || x is IPropertySymbol || x is IParameterSymbol);
return null;
        }
    }
}
