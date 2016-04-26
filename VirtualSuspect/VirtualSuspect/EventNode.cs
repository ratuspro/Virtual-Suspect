﻿using System.Collections.Generic;

namespace VirtualSuspect
{
    public class EventNode {

        private uint id;

        public uint ID
        {
            get
            {
                return id;
            }
        }

        private int incriminatory;

        public int Incriminatory
        {
            get {
                return incriminatory;
            }
        }

        private ActionNode action;

        public ActionNode Action
        {
            get
            {
                return action;
            }
        }

        private EntityNode time;

        public EntityNode Time
        {
            get
            {
                return time;
            }
        }

        private EntityNode location;

        public EntityNode Location
        {
            get
            {
                return location;
            }
        }

        private List<EntityNode> agent;

        public List<EntityNode> Agent
        {
            get
            {
                return agent;
            }
        }

        private List<EntityNode> theme;

        public List<EntityNode> Theme
        {
            get
            {
                return theme;
            }
        }

        private List<EntityNode> manner;

        public List<EntityNode> Manner
        {
            get
            {
                return manner;
            }
        }

        private List<EntityNode> reason;

        public List<EntityNode> Reason
        {
            get
            {
                return reason;
            }
        }

        public float Know {

            get {

                int totalNumEntities = 0;
                int numKnownEntities = 0;

                //Time
                numKnownEntities += Time.Known ? 1 : 0;
                totalNumEntities++;

                //Location
                numKnownEntities += Location.Known ? 1 : 0;
                totalNumEntities++;

                //Agent
                foreach(EntityNode agent in Agent) {
                    numKnownEntities += agent.Known ? 1 : 0;
                    totalNumEntities++;
                }

                //Manner
                foreach (EntityNode manner in Manner) {
                    numKnownEntities += manner.Known ? 1 : 0;
                    totalNumEntities++;
                }

                //Theme
                foreach (EntityNode theme in Theme) {
                    numKnownEntities += theme.Known ? 1 : 0;
                    totalNumEntities++;
                }

                //Reason
                foreach (EntityNode reason in Reason) {
                    numKnownEntities += reason.Known ? 1 : 0;
                    totalNumEntities++;
                }

                return numKnownEntities / totalNumEntities;
            }
        }

        public EventNode(uint id, int incriminatory, ActionNode action, EntityNode time, EntityNode location) {

            this.id = id;
            this.incriminatory = incriminatory;
            this.action = action;
            this.time = time;
            this.location = location;

            agent = new List<EntityNode>();
            theme = new List<EntityNode>();
            manner = new List<EntityNode>();
            reason = new List<EntityNode>();

        }

        public void AddAgent(EntityNode agent) {

            this.agent.Add(agent);

        }

        public void AddAgent(params EntityNode[] agents) {

            this.agent.AddRange(agents);

        }

        public void AddAgent(List<EntityNode> agents) {

            this.agent.AddRange(agents);

        }

        public void AddTheme(EntityNode theme) {

            this.theme.Add(theme);

        }

        public void AddTheme(params EntityNode[] themes) {

            this.theme.AddRange(themes);

        }

        public void AddTheme(List<EntityNode> themes) {

            this.theme.AddRange(themes);

        }

        public void AddManner(EntityNode manner) {

            this.manner.Add(manner);

        }

        public void AddManner(params EntityNode[] manners) {

            this.manner.AddRange(manners);

        }

        public void AddManner(List<EntityNode> manners) {

            this.manner.AddRange(manners);

        }

        public void AddReason(EntityNode reason) {

            this.reason.Add(reason);

        }

        public void AddReason(params EntityNode[] reasons) {

            this.reason.AddRange(reasons);

        }

        public void AddReason(List<EntityNode> reasons) {

            this.reason.AddRange(reasons);

        }

    }
}