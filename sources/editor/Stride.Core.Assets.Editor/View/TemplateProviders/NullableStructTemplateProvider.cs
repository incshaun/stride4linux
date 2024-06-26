// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Stride.Core.Presentation.Quantum.ViewModels;
using Stride.Core.Reflection;

namespace Stride.Core.Assets.Editor.View.TemplateProviders
{
    public class NullableStructTemplateProvider : NodeViewModelTemplateProvider
    {
        public override string Name => "NullableStruct";

        public override bool MatchNode(NodeViewModel node)
        {
            return node.Type.IsNullable() && Nullable.GetUnderlyingType(node.Type).IsStruct();
        }
    }
}
