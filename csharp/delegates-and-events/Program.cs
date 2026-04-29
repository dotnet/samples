using DelegatesAndEvents;

Logger.WriteMessage += LoggingMethods.LogToConsole;
var file = new FileLogger("log.txt");

Logger.LogMessage(Severity.Warning, "Console", "This is a warning message");

Logger.LogMessage(Severity.Information, "Console", "Information message one");
Logger.LogLevel = Severity.Information;

Logger.LogMessage(Severity.Information, "Console", "Information message two");
