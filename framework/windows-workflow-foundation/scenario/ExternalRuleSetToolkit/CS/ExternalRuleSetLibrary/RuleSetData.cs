//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace Microsoft.Samples.Rules.ExternalRuleSetLibrary
{
    public class RuleSetData : IComparable<RuleSetData>
    {
        #region Variables and constructor 

        public RuleSetData()
        {
        }

        private string name;
        private string originalName;
        private int majorVersion;
        private int originalMajorVersion;
        private int minorVersion;
        private int originalMinorVersion;
        private string ruleSetDefinition;
        private RuleSet ruleSet;
        private short status;
        private string assemblyPath;
        private string activityName;
        private DateTime modifiedDate;
        private bool dirty;
        private Type activity;

        private WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (this.RuleSet != null)
                    this.RuleSet.Name = name;
            }
        }
        public string OriginalName
        {
            get { return originalName; }
            set { originalName = value; }
        }
        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
        public int OriginalMajorVersion
        {
            get { return originalMajorVersion; }
            set { originalMajorVersion = value; }
        }
        public int MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }
        public int OriginalMinorVersion
        {
            get { return originalMinorVersion; }
            set { originalMinorVersion = value; }
        }
        public string RuleSetDefinition
        {
            get { return ruleSetDefinition; }
            set { ruleSetDefinition = value; }
        }
        public RuleSet RuleSet
        {
            get
            {
                if (ruleSet == null)
                {
                    ruleSet = this.DeserializeRuleSet(ruleSetDefinition);
                }
                return ruleSet;
            }
            set
            {
                ruleSet = value;
                name = ruleSet.Name; 
            }
        }
        public short Status
        {
            get { return status; }
            set { status = value; }
        }
        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { assemblyPath = value; }
        }
        public string ActivityName
        {
            get { return activityName; }
            set { activityName = value; }
        }
        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
            set { modifiedDate = value; }
        }
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        public Type Activity
        {
            get { return activity; }
            set
            {
                activity = value;
                if (activity != null)
                    activityName = activity.ToString();
            }
        }

        #endregion

        #region Methods

        private RuleSet DeserializeRuleSet(string ruleSetXmlDefinition)
        {
            if (!String.IsNullOrEmpty(ruleSetXmlDefinition))
            {
                StringReader stringReader = new StringReader(ruleSetXmlDefinition);
                XmlTextReader reader = new XmlTextReader(stringReader);
                return serializer.Deserialize(reader) as RuleSet;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} - {1}.{2}", name, majorVersion, minorVersion);
        }

        public RuleSetData Clone()
        {
            RuleSetData newData = new RuleSetData();
            newData.Activity = this.Activity;
            //newData.ActivityName = activityName; //Set by setting Activity
            newData.AssemblyPath = this.AssemblyPath;
            newData.Dirty = true;
            newData.MajorVersion = this.MajorVersion;
            newData.MinorVersion = this.MinorVersion;
            newData.Name = name;
            newData.RuleSet = this.RuleSet.Clone();
            newData.Status = 0;

            return newData;
        }

        public RuleSetInfo GetRuleSetInfo()
        {
            return new RuleSetInfo(name, majorVersion, minorVersion);
        }

        #endregion

        #region IComparable<RuleSetData> Members

        public int CompareTo(RuleSetData other)
        {
            if (other != null)
            {
                int nameComparison = String.CompareOrdinal(this.Name, other.Name);
                if (nameComparison != 0)
                    return nameComparison;

                int majorVersionComparison = this.MajorVersion - other.MajorVersion;
                if (majorVersionComparison != 0)
                    return majorVersionComparison;

                int minorVersionComparison = this.MinorVersion - other.MinorVersion;
                if (minorVersionComparison != 0)
                    return minorVersionComparison;

                return 0;
            }
            else
            {
                return 1;
            }
        }

        #endregion
    }
}
