using CustomMarshalling;

Console.OutputEncoding = System.Text.Encoding.UTF8;

MarshalStringAsUtf32();
MarshalErrorData();

static void MarshalStringAsUtf32()
{
    Console.WriteLine($"=== Marshal strings as UTF-32 using a custom marshaller ===");
    Console.WriteLine();
    string s = "Ĥȅľľő, Ŵőŕľď!";

    // Marshals string using Utf32StringMarshaller.ManagedToUnmanagedIn
    Console.WriteLine($"--- {nameof(NativeLib.PrintString)}: uses {nameof(Utf32StringMarshaller)}.{nameof(Utf32StringMarshaller.ManagedToUnmanagedIn)}");
    NativeLib.PrintString(s);
    Console.WriteLine();

    string[] toPrint = new string[]
    {
        s,
        "🄷🄴🄻🄻🄾 🅆🄾🅁🄻🄳",
        "Lorem ipsum dolor sit amet"
    };

    // Marshals strings using Utf32StringMarshaller
    Console.WriteLine($"--- {nameof(NativeLib.PrintStrings)}: uses {nameof(Utf32StringMarshaller)}");
    NativeLib.PrintStrings(toPrint, toPrint.Length);
    Console.WriteLine();

    // Marshals strings using Utf32StringMarshaller
    Console.WriteLine($"--- {nameof(NativeLib.ReverseString)}: uses {nameof(Utf32StringMarshaller)}");
    Console.WriteLine($"Original: {s}");
    Console.WriteLine($"Reversed: {NativeLib.ReverseString(s)}");
    Console.WriteLine();

    // Marshals string using Utf32StringMarshaller
    Console.WriteLine($"--- {nameof(NativeLib.ReverseStringInPlace)}: uses {nameof(Utf32StringMarshaller)}");
    Console.WriteLine($"Original: {s}");
    NativeLib.ReverseStringInPlace(ref s);
    Console.WriteLine($"Reversed: {s}");
    Console.WriteLine();
}

static void MarshalErrorData()
{
    Console.WriteLine($"=== Marshal user-defined type {nameof(ErrorData)} ===");
    Console.WriteLine();
    ErrorData errorData = new ErrorData()
    {
        Code = -10,
        IsFatalError = true,
        Message = "✗✗✗✗✗✗"
    };

    // Marshals ErrorData using ErrorDataMarshaller
    Console.WriteLine($"--- {nameof(NativeLib.PrintErrorData)}: uses {nameof(ErrorDataMarshaller)}");
    NativeLib.PrintErrorData(errorData);
    Console.WriteLine();

    // Marshals ErrorData using ErrorDataMarshaller.ThrowOnFatalErrorOut
    Console.WriteLine($"--- {nameof(NativeLib.GetFatalErrorIfNegative)}: uses {nameof(ErrorDataMarshaller)}.{nameof(ErrorDataMarshaller.ThrowOnFatalErrorOut)}");
    Console.WriteLine("Getting error data with code 0");
    ErrorData ret = NativeLib.GetFatalErrorIfNegative(0);
    ret.Print();
    Console.WriteLine();

    Console.WriteLine("Getting error data with code -1");
    try
    {
        ret = NativeLib.GetFatalErrorIfNegative(-1);
    }
    catch (Exception e)
    {
        Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
    }

    Console.WriteLine();

    // Marshals ErrorData using ErrorDataMarshaller
    Console.WriteLine($"--- {nameof(NativeLib.GetErrors)}: uses {nameof(ErrorDataMarshaller)}.{nameof(ErrorDataMarshaller.Out)}");
    int[] codes = new int[] { -3, 2, 5, 12 };
    ErrorData[] errors = NativeLib.GetErrors(codes, codes.Length);
    foreach (ErrorData error in errors)
    {
        error.Print();
        Console.WriteLine();
    }

    ErrorBuffer errorBuffer = new();
    errors.AsSpan().CopyTo(errorBuffer);
    int[] retrievedCodes = new int[4];
    NativeLib.GetErrorCodes(errorBuffer, retrievedCodes);
    foreach (int code in retrievedCodes)
    {
        Console.WriteLine($"Code from error data: {code}");
    }
}
