//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.Samples.Rules.ExternalRuleSetLibrary
{
    public class RuleSetInfo : IComparable<RuleSetInfo>
    {
        private string name;
        private int majorVersion;
        private int minorVersion;

        #region Constructors

        public RuleSetInfo()
        {
        }

        public RuleSetInfo(string ruleSetName)
        {
            name = ruleSetName;
        }

        public RuleSetInfo(string ruleSetName, int ruleSetMajorVersion, int rulesSetMinorVersion)
        {
            name = ruleSetName;
            majorVersion = ruleSetMajorVersion;
            minorVersion = rulesSetMinorVersion;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }

        public int MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }

        #endregion

        #region IComparable<RuleSetInfo> Members

        public int CompareTo(RuleSetInfo other)
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

        #region Methods

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}.{2}", name, majorVersion, minorVersion);
        }

        #endregion
    }
}

