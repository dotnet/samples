//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;

namespace Microsoft.Samples.Organization
{
    public class OrgService : IOrgService
    {        
        DataContext dataContext;

        public OrgService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OrgData"].ConnectionString;
            dataContext = new DataContext(connectionString);
        }

        // UI - Select employee
        public IList<Employee> GetAllEmployees()
        {
            Console.WriteLine("GetAllEmployees");
            
            return
               (from emp in dataContext.GetTable<Employee>()
                select new Employee 
                { 
                    Id = emp.Id ,
                    Name = emp.Name,
                    ManagerId = emp.ManagerId,
                    Department = GetDepartment(emp.DepartmentId),
                    Position = GetPosition(emp.PositionId)
                })
                    .ToList();
        }

        // WF - get employee data and manager
        public Employee GetEmployee(string employeeId)
        {
            Console.WriteLine("GetEmployee {0}", employeeId);

            return
               (from emp in dataContext.GetTable<Employee>()
                where emp.Id.Equals(employeeId)
                select new Employee
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    ManagerId = emp.ManagerId,
                    Department = GetDepartment(emp.DepartmentId),
                    Position = GetPosition(emp.PositionId)
                })
                    .FirstOrDefault();
        }

        // WF - get employee data and manager
        public IList<Employee> GetEmployeesByPosition(string positionId)
        {
            Console.WriteLine("GetEmployeesByPosition {0}", positionId);

            return
               (from emp in dataContext.GetTable<Employee>()
                where emp.PositionId.Equals(positionId)
                select new Employee
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    ManagerId = emp.ManagerId,
                    Department = GetDepartment(emp.DepartmentId),
                    Position = GetPosition(emp.PositionId)
                })
                   .ToList();
        }

        // UI - Fill combo
        public IList<Department> GetAllDepartments()
        {
            Console.WriteLine("GetAllDepartments");

            return
                (from dept in dataContext.GetTable<Department>()
                 select new Department
                 {
                     Id = dept.Id.Trim(),
                     Name = dept.Name,
                     OwnerId = dept.OwnerId,
                     Owner = GetEmployee(dept.OwnerId)
                 })
                    .ToList();            
        }

        // WF - Get department manager
        public Department GetDepartment(string departmentId)
        {
            Console.WriteLine("GetDepartment {0}", departmentId);

            return
                  (from dept in dataContext.GetTable<Department>()
                   where dept.Id.Equals(departmentId)
                   select new Department
                   {
                       Id = dept.Id.Trim(),
                       Name = dept.Name,
                       OwnerId = dept.OwnerId
                   })
                      .FirstOrDefault();                       
        }

        // UI - create a position
        public IList<Position> GetAllPositions()
        {
            Console.WriteLine("GetAllPositions");

            return
               (from pos in dataContext.GetTable<Position>()
                select new Position
                {
                    Id = pos.Id.Trim(),
                    Name = pos.Name,
                })
                    .ToList();            
        }

        public Position GetPosition(string positionId)
        {
            Console.WriteLine("GetPosition {0}", positionId);

            return
                (from pos in dataContext.GetTable<Position>()
                 where pos.Id.Equals(positionId)
                 select new Position
                 {
                     Id = pos.Id.Trim(),
                     Name = pos.Name,
                 })
                    .FirstOrDefault();
        }

        public IList<PositionType> GetAllPositionTypes()
        {
            Console.WriteLine("GetAllPositionTypes");

            return
                (from posType in dataContext.GetTable<PositionType>()
                 select posType)
                    .ToList();
        }

        public PositionType GetPositionType(string positionTypeId)
        {
            Console.WriteLine("GetPositionType {0}", positionTypeId);

            return
                (from posType in dataContext.GetTable<PositionType>()
                 where posType.Id.Equals(positionTypeId)
                 select posType)
                    .FirstOrDefault();            
        }
    }
}
