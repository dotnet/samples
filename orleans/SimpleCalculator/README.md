# SimpleCalculator

SimpleCalculator is a small C# console application created for learning and practice purposes.

The project demonstrates basic mathematical operations and clean separation between user input logic and calculation logic.

---

## 🎯 Purpose

The main goal of this project is to:

* Practice C# fundamentals
* Understand how to separate logic from the entry point (`Program.cs`)
* Work with classes and methods
* Prepare the project for future improvements such as interfaces and unit testing

---

## 🧠 Project Overview

The application allows the user to:

1. Enter two numbers
2. Choose a mathematical operator (`+`, `-`, `*`, `/`)
3. Receive the calculated result in the console

All calculations are handled by a separate `Calculator` class.

---

## 📁 Project Structure

```
SimpleCalculator
│
├── SimpleCalculator
│   ├── Calculator.cs
│   ├── Program.cs
│   └── SimpleCalculator.csproj
│
├── README.md
└── SimpleCalculator.sln
```

---

## 🧮 Calculator Class

The `Calculator` class contains simple methods for basic math operations:

* Sum
* Subtract
* Multiply
* Divide

This keeps the program logic clean and easy to understand.

---

## ▶️ How to Run the Project

Make sure you have **.NET SDK** installed.

### Run from terminal

Navigate to the project folder and run:

```bash
dotnet run
```

Or from the solution root:

```bash
dotnet run --project SimpleCalculator
```

---

## 🛠 Technologies Used

* C#
* .NET
* Console Application

---

## 🚀 Future Improvements

Planned improvements for the next versions:

* Introduce interfaces for operations
* Add unit tests with NUnit
* Improve input validation
* Extend with more mathematical operations
* Refactor to a class library + console UI

---

## 📌 Notes

This project is created as a learning exercise and will be gradually improved as new concepts are learned.

---
