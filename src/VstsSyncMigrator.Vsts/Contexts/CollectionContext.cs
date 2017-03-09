﻿using Microsoft.TeamFoundation.Client;
using System;
using System.Diagnostics;
using Microsoft.TeamFoundation;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using VstsSyncMigrator.DataContracts;

namespace VstsSyncMigrator.Vsts
{
    public class CollectionContext
    {
        private TeamProject _teamProject;
        private TfsTeamProjectCollection _Collection;

        public TfsTeamProjectCollection Collection
        {
            get
            {
                Connect();
                return _Collection;
            }
        }

        public string Name
        {
            get
            {
                return _teamProject.Name;
            }
        }

        public CollectionContext(TeamProject teamProject)
        {
            this._teamProject = teamProject;
        }

        public void Connect()
        {
            if (_Collection == null)
            {
                Stopwatch connectionTimer = new Stopwatch();
                connectionTimer.Start();
                Trace.WriteLine("Creating TfsTeamProjectCollection Object ");
                _Collection = new TfsTeamProjectCollection(_teamProject.CollectionUrl);
                try
                {
                    Trace.WriteLine(string.Format("Connected to {0} ", _Collection.Uri.ToString()));
                    Trace.WriteLine(string.Format("validating security for {0} ", _Collection.AuthorizedIdentity.ToString()));
                    _Collection.EnsureAuthenticated();
                    connectionTimer.Stop();
                    Telemetry.Current.TrackEvent("ConnectionEstablished",
                      new Dictionary<string, string> {
                            { "CollectionUrl", _teamProject.CollectionUrl.ToString() },
                            { "TeamProjectName",  _teamProject.Name}
                      },
                      new Dictionary<string, double> {
                            { "ConnectionTimer", connectionTimer.ElapsedMilliseconds }
                      });
                    Trace.TraceInformation(string.Format(" Access granted "));
                }
                catch (TeamFoundationServiceUnavailableException ex)
                {
                    Telemetry.Current.TrackException(ex,
                       new Dictionary<string, string> {
                            { "CollectionUrl", _teamProject.CollectionUrl.ToString() },
                            { "TeamProjectName",  _teamProject.Name}
                       },
                       new Dictionary<string, double> {
                            { "ConnectionTimer", connectionTimer.ElapsedMilliseconds }
                       });
                    Trace.TraceWarning(string.Format("  [EXCEPTION] {0}", ex.Message));
                    throw ex;
                }
            }            
        }
    }
}