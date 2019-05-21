// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;

using osu.Framework.Bindables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using System;


namespace osu.Game.Rulesets.Osu.Mods
{
    internal class OsuModGrow : Mod, IApplicableToDrawableHitObjects, IApplicableToScoreProcessor
    {
        public override string Name => "Aim Practice";

        public override string Acronym => "AP";

        //public override IconUsage Icon => FontAwesome.Solid.ArrowsAltV;

        public override ModType Type => ModType.Fun;

        public override bool Ranked => false;

        public override string Description => "Play better and the circles shrink!";

        public override double ScoreMultiplier => 1;

        private float radiusScaleFactor;

        //public void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        //{
        //    scoreProcessor.Health.ValueChanged += health =>
        //    {
        //        //currentHp = (float)health.NewValue;
        //        radiusScaleFactor = 1.0f / (float)Math.Pow(2, (2.0f * (float)health.NewValue) - 1.0f);
        //    };
        //}

        public void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables)
        {
            foreach (var drawable in drawables)
            {
                switch (drawable)
                {
                    case DrawableSpinner _:
                        continue;

                    default:
                        drawable.ApplyCustomUpdateState += ApplyCustomState;
                        break;
                }
            }
        }

        protected virtual void ApplyCustomState(DrawableHitObject drawable, ArmedState state)
        {
            var h = (OsuHitObject)drawable.HitObject;

            // apply grow effect
            switch (drawable)
            {
                case DrawableSliderHead _:
                case DrawableSliderTail _:
                    // special cases we should *not* be scaling.
                    break;

                case DrawableSlider _:
                case DrawableHitCircle _:
                {
                        using (drawable.BeginAbsoluteSequence(h.StartTime - h.TimePreempt, true))
                            drawable.ScaleTo(0.2f).Then().ScaleTo(radiusScaleFactor, h.TimePreempt, Easing.OutExpo);
                    break;
                }
            }

        }

        public void ApplyToScoreProcessor(ScoreProcessor scoreProcessor)
        {
            //double hp = scoreProcessor.Health.Value;
            //radiusScaleFactor = 1.0f / (float)Math.Pow(2, 2.0 * (float)hp - 1.0);
            //radiusScaleFactor = 1.0f;

            scoreProcessor.Health.ValueChanged += health =>
            {
                double hp = health.NewValue;
                radiusScaleFactor = 1.0f / (float)Math.Pow(2, 2.0 * (float)hp - 1.0);
                //currentHp = (float)health.NewValue;
                //radiusScaleFactor = 1.0f / (float)Math.Pow(2, (2.0f * (float)health.NewValue) - 1.0f);
            };
        }

        private void Health_ValueChanged(ValueChangedEvent<double> obj)
        {
            throw new NotImplementedException();
        }

        public ScoreRank AdjustRank(ScoreRank rank, double accuracy) => rank;


    }
}


/// --- STEPS ---
/// (optional) Override beatmap circle size to 4 (or default radius of 64 pixels)
/// detect a change in health event (done?)
/// fetch current health percentage
/// apply a curve to resize the radius according the the following cases:   1.0f / (float)Math.Pow(2, (2.0f * (float)health.NewValue) - 1.0f)
/// - hp = 0.0 : circle radius is 2.0x
/// - hp = 0.5 : circle radius is 1.0x
/// - hp - 1.0 : circle radius is 0.5x
