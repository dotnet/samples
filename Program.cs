using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS_DemoOOP.Demographics.ValueTypes;
//using CS_DemoOOP.Arithmetic;
//using CS_DemoOOP.Banking;
using CS_DemoOOP.Extension;

namespace CS_DemoOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Person p1 = new Person();
            //p1._id = 101;
            //p1._name = "James";
            //p1._age = 45.5f;

            //Person p2 = new Person(102, "JOhn", 43.6f);
            //p2._age = -43;

            //Console.WriteLine(p1.getData());
            //Console.WriteLine(p2);

            //Console.WriteLine($"Id :{p1._id}, name : { p1._name}, age : {p1._age}");
            //Console.WriteLine($"Id :{p2._id}, name : { p2._name}, age : {p2._age}");



            //Maths m = new Maths();
            //Console.WriteLine(m.Add(20,10));
            //Console.WriteLine(m.Add(20.23456d, 10.97654d));
            //Console.WriteLine(m.Add(DateTime.Now, 10));



            //Employee emp1 = new Employee()
            //{
            //    _id = 101,
            //    _name = "James",
            //    _age = 45.5f,
            //    DeptNo = 10,
            //    Salary = 77000
            //}; //Object initializer C# 3.0

            //Console.WriteLine($"Id: {emp1._id}, Name: {emp1._name}, Age: {emp1._age}, DeptNo: {emp1.DeptNo}, Salary: {emp1.Salary}");
            //Console.WriteLine(emp1.GetData);

            //Person emp2 = new Person Employee(102, "John",42, 232, 48888);
            //Employee emp2 = new Employee(102,"John",43.6f,10,50000);
            //Console.WriteLine($"Id: {emp2._id}, Name: {emp2._name}, Age: {emp2._age}, DeptNo: {emp2.DeptNo}, Salary: {emp2.Salary}");

            //Account acc=new Account();

            //SavingsAccount acc = new SavingsAccount();
            //try
            //{
            //    Console.WriteLine(acc.Balance);
            //    acc.Deposit(10000);
            //    Console.WriteLine(acc.Balance);
            //    acc.Withdraw(5000);
            //    Console.WriteLine(acc.Balance);
            //    acc.Withdraw(4000);
            //    Console.WriteLine(acc.Balance);



            //}
            //catch (ApplicationException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //Console.ReadKey();


            //Console.WriteLine($"{WeekDays.Sunday} : {Convert.ToInt32(WeekDays.Sunday)}");
            //Console.WriteLine($"{WeekDays.Monday} : {Convert.ToInt32(WeekDays.Monday)}");
            //Console.WriteLine($"{WeekDays.Tuesday} : {Convert.ToInt32(WeekDays.Tuesday)}");
            //Console.WriteLine($"{WeekDays.Wednesday} : {Convert.ToInt32(WeekDays.Wednesday)}");
            //Console.WriteLine($"{WeekDays.Thursday} : {Convert.ToInt32(WeekDays.Thursday)}");
            //Console.WriteLine($"{WeekDays.Friday} : {Convert.ToInt32(WeekDays.Friday)}");
            //Console.WriteLine($"{WeekDays.Saturday} : {Convert.ToInt32(WeekDays.Saturday)}");



            //string data = "asdasd. adsasddas. wwww. sdadas.";
            //string data2 = "asdasda sd asdasdhj fasuhd. asduygayusgyudasfdus. asddu gyquqw yusagd yugas wqeygyuge. wqiegy qew geuyg.";
            //Console.WriteLine($"The no of lines in {data} \n: {data.WordCount()}");
            //Console.WriteLine($"The no of sentences in {data2} \n: {data2.SentenceCount()}");



            //var data = new //Anonymous types
            //{
            //    Firstname = "Sparsh",
            //    Lastname = "Pradhan",
            //};

            //Console.WriteLine($"Firstname : {data.Firstname}, Lastname { data.Lastname}");




            SavingsAccount acc = new SavingsAccount();
            acc.LowBalance += Acc_LowBalance;

            Console.WriteLine(acc.Balance);
            acc.Deposit(10000);
            Console.WriteLine(acc.Balance);
            acc.WithDraw(5500);
            Console.WriteLine(acc.Balance);
            acc.WithDraw(4000);
            Console.WriteLine(acc.Balance);



            Console.ReadKey();



        }

        private static void Acc_LowBalance(double amount)
        {
            Console.WriteLine($"You have {amount} left in your ")



        }
    }
}
