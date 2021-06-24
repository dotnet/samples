//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Samples.Organization
{
    [ServiceContract]
    public interface IOrgService
    {
        [OperationContract]
        Employee GetEmployee(string employeeId);

        [OperationContract]
        Department GetDepartment(string departmentId);

        [OperationContract]
        Position GetPosition(string positionId);

        [OperationContract]
        PositionType GetPositionType(string positionTypeId);

        [OperationContract]
        IList<Employee> GetAllEmployees();

        [OperationContract]
        IList<Employee> GetEmployeesByPosition(string positionId);

        [OperationContract]
        IList<Department> GetAllDepartments();

        [OperationContract]
        IList<Position> GetAllPositions();

        [OperationContract]
        IList<PositionType> GetAllPositionTypes();
    }    
}
