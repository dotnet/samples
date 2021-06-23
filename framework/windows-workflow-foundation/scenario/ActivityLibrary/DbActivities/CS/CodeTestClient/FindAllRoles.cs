//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;

namespace Microsoft.Samples.Activities.Data
{

    /// <summary>
    /// This activity will query all Role instances in the roles table from 
    /// Norhtwind database and will return an IList of instances of Role class    
    /// </summary>
    public class FindAllRoles: Activity<IList<Role>>
    {
        public FindAllRoles()
        {
            Variable<IList<Role>> roles = new Variable<IList<Role>>();

            this.Implementation = () =>
                new Sequence
                {
                    Variables = { roles },
                    Activities = 
                    {
                        new DbQuery<Role>()
                        {
                            ConfigName = "DbActivitiesSample",
                            Sql = "SELECT * FROM Roles",
                            Mapper = (dataReader) =>
                            {
                                Role role = new Role();
                                role.Code = dataReader["code"].ToString();
                                role.Name = dataReader["name"].ToString();
                                return role;
                            },
                            Result = roles
                        },
                        new Assign<IList<Role>>
                        {
                           To = new OutArgument<IList<Role>>(c => this.Result.Get(c)),
                           Value = roles
                        }
                    }
                };
        }
    }
}
