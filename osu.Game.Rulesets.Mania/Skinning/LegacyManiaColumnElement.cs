// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Mania.UI;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Mania.Skinning
{
    /// <summary>
    /// A <see cref="CompositeDrawable"/> which is placed somewhere within a <see cref="Column"/>.
    /// </summary>
    public class LegacyManiaColumnElement : LegacyManiaElement
    {
        [Resolved]
        protected Column Column { get; private set; }

        /// <summary>
        /// The column index to use for texture lookups, in the case of no user-provided configuration.
        /// </summary>
        protected int FallbackColumnIndex { get; private set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            if (Stage == null)
                FallbackColumnIndex = Column.Index % 2 + 1;
            else
            {
                int dist = Math.Min(Column.Index, Stage.Columns.Count - Column.Index - 1);
                FallbackColumnIndex = dist % 2 + 1;
            }
        }

        protected override IBindable<T> GetManiaSkinConfig<T>(ISkin skin, LegacyManiaSkinConfigurationLookups lookup, int? index = null)
            => base.GetManiaSkinConfig<T>(skin, lookup, index ?? Column.Index);
    }
}
