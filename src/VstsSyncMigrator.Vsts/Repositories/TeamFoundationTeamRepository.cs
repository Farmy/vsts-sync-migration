﻿using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections.ObjectModel;
using VstsSyncMigrator.DataContracts;
using VstsSyncMigrator.Vsts.Contexts;

namespace VstsSyncMigrator.Vsts
{
    public class TeamFoundationTeamRepository : IRepository<Team>
    {
        TeamProject teamProject;

        public TeamFoundationTeamRepository(TeamProject teamProject)
        {
            this.teamProject = teamProject;
        }

        public Team Add(Team entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Team entity)
        {
            throw new NotImplementedException();
        }

        public Team Get(Expression<Func<Team, bool>> filter)
        {
            return GetAll().AsQueryable().Where(filter).SingleOrDefault();
        }

        public ICollection<Team> GetAll()
        {
            ICollection<Team> localTeams = new Collection<Team>();
            CollectionContext sourceCollection = new CollectionContext(teamProject);
            WorkItemStoreContext sourceStore = new WorkItemStoreContext(sourceCollection, WorkItemStoreFlags.BypassRules);
            TfsTeamService sourceTS = sourceCollection.Collection.GetService<TfsTeamService>();
            List<TeamFoundationTeam> remoteTeams = sourceTS.QueryTeams(teamProject.Name).ToList();
            foreach (TeamFoundationTeam team in remoteTeams)
            {
                localTeams.Add(new Team {Name = team.Name, Project = team.Project, Description = team.Description });
            }

            return localTeams;
        }

        public bool Update(Team entity)
        {
            throw new NotImplementedException();
        }
    }
}
