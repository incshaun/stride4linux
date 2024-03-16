// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Assets.Models;
using Stride.Core.Assets.Editor.View;
using Stride.Core.Presentation.Quantum.ViewModels;
using Avalonia.Animation;

namespace Stride.Assets.Presentation.TemplateProviders
{
    public class AnimationFrameBoxTemplateProvider : NodeViewModelTemplateProvider
    {
        public override string Name { get { return "AnimationFrameBoxTemplateProvider"; } }

        public override bool MatchNode(NodeViewModelBase node)
        {
            return (node.Name.Equals(nameof(AnimationAssetDurationUnchecked.StartAnimationTimeBox)) || node.Name.Equals(nameof(AnimationAssetDurationUnchecked.EndAnimationTimeBox)));
        }
    }
}
