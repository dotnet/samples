// <Snippet1>
open System

[<EntryPoint>]
let main argv =
    // Create a char List for the modern Cyrillic alphabet, 
    // from U+0410 to U+044F.
    let chars = [for codePoint in 0x0410..0x044F do Convert.ToChar(codePoint)]
    
    Console.WriteLine("Current code page: {0}\n", Console.OutputEncoding.CodePage)
    // Display the characters.
    for ch in chars do
        Console.Write("{0}  ", ch)
        if (Console.CursorLeft) >= 70
            then Console.WriteLine()
            else ()
    
    0

// The example displays the following output:
//    Current code page: 437
//    
//    ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?
//    ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?
//    ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?
// </Snippet1>