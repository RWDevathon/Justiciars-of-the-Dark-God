using RimWorld;
using System;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Base class to control logic for Justiciar Ambitions and Inspirations, which are replacements for mental breaks and inspirations respectively. These track and handle success/fail states.
    public abstract class Hediff_MentalTracker : HediffWithComps
    {
        private JusticiarMentalTimerExtension extension;

        public int expirationTick;

        // Ambitions or Inspirations may not want to remove themselves immediately upon completion but be aware that they are complete.
        public bool complete = false;

        // Ambitions are likely to be removed manually rather than automatically, but in the event that they can be removed automatically, it should be done explicitly.
        public bool shouldRemove = false;

        public JusticiarMentalTimerExtension Extension
        {
            get
            {
                if (extension == null)
                {
                    extension = def.GetModExtension<JusticiarMentalTimerExtension>();
                }
                return extension;
            }
        }

        public int TicksRemaining => Math.Max(0, expirationTick - GenTicks.TicksGame);

        public override bool ShouldRemove => shouldRemove;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref expirationTick, "ABF_expirationTick", 0);
            Scribe_Values.Look(ref complete, "ABF_complete", false);
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.TipStringExtra);
                if (expirationTick < GenDate.TicksPerHour)
                {
                    stringBuilder.AppendLine("JDG_MentalTrackerTimeRemaining".Translate(TicksRemaining.ToStringSecondsFromTicks("F0")));
                }
                else
                {
                    stringBuilder.AppendLine("JDG_MentalTrackerTimeRemaining".Translate(TicksRemaining.ToStringTicksToPeriod(allowSeconds: true, shortForm: true, canUseDecimals: true, allowYears: true)));
                }
                return stringBuilder.ToString();
            }
        }

        public override void PostMake()
        {
            expirationTick = GenTicks.TicksGame + Extension.expirationTicks.RandomInRange;
        }

        public abstract void NotifyFailed();

        public abstract void NotifySucceeded();
    }
}
