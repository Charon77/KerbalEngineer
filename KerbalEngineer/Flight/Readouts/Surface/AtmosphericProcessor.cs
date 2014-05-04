﻿// Project:	KerbalEngineer
// Author:	CYBUTEK
// License:	Attribution-NonCommercial-ShareAlike 3.0 Unported

#region Using Directives

using System;
using System.Linq;

using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class AtmosphericProcessor : IUpdatable, IUpdateRequest
    {
        #region Instance

        private static readonly AtmosphericProcessor instance = new AtmosphericProcessor();

        /// <summary>
        ///     Gets the current instance of the atmospheric processor.
        /// </summary>
        public static AtmosphericProcessor Instance
        {
            get { return instance; }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether the details are ready to be shown.
        /// </summary>
        public static bool ShowDetails { get; private set; }

        /// <summary>
        ///     Gets the terminal velocity of the active vessel.
        /// </summary>
        public static double TerminalVelocity { get; private set; }

        /// <summary>
        ///     Gets the difference between current velocity and terminal velocity.
        /// </summary>
        public static double Efficiency { get; private set; }

        #endregion

        #region IUpdatable Members

        /// <summary>
        ///     Updates the details by recalculating if requested.
        /// </summary>
        public void Update()
        {
            if (FlightGlobals.ActiveVessel.atmDensity < double.Epsilon)
            {
                ShowDetails = false;
                return;
            }

            ShowDetails = true;

            var mass = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass());
            var drag = FlightGlobals.ActiveVessel.parts.Sum(p => p.GetWetMass() * p.maximum_drag);
            var grav = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude;
            var atmo = FlightGlobals.ActiveVessel.atmDensity;
            var coef = FlightGlobals.DragMultiplier;

            TerminalVelocity = Math.Sqrt((2 * mass * grav) / (atmo * drag * coef));
            Efficiency = FlightGlobals.ship_srfSpeed / TerminalVelocity;
        }

        #endregion

        #region IUpdateRequest Members

        /// <summary>
        ///     Gets and sets whether the updatable object should be updated.
        /// </summary>
        public bool UpdateRequested { get; set; }

        #endregion

        /// <summary>
        ///     Request an update to calculate the details.
        /// </summary>
        public static void RequestUpdate()
        {
            instance.UpdateRequested = true;
        }
    }
}