// <Snippet2>
open Microsoft.Win32
open System

[<EntryPoint>]
let main argv =
   let valueName = "Lucida Console"
   let newFont = "simsun.ttc,SimSun"
   
   let key = 
      Registry.LocalMachine.OpenSubKey( 
         @"Software\Microsoft\Windows NT\CurrentVersion\FontLink\SystemLink", 
         true)
   if isNull key
   then Console.WriteLine("Font linking is not enabled.")
   else
      // Determine if the font is a base font.
      let names = key.GetValueNames()
      let (fonts, kind, toAdd) = 
         if Array.Exists(names, fun s -> s.Equals(valueName, StringComparison.OrdinalIgnoreCase)) 
         then
            // Get the value's type.
            let kind = key.GetValueKind(valueName)

            // Type should be RegistryValueKind.MultiString, but we can't be sure.
            let fonts = 
               match kind with
               | RegistryValueKind.String ->  [|key.GetValue(valueName) :?> string |]
               | RegistryValueKind.MultiString -> (key.GetValue(valueName) :?> string array)
               | _ -> [||]
            
            // Determine whether SimSun is a linked font.
            let toAdd = not (Array.Exists(fonts, fun s -> s.IndexOf("SimSun", StringComparison.OrdinalIgnoreCase) >=0))
               
            (fonts, kind, toAdd)
         else
            // Font is not a base font.
            ([||], RegistryValueKind.Unknown, true)

      if toAdd
      then 
         // Font is not a linked font.
         let newFonts = Array.append fonts [|newFont|]

         // Change REG_SZ to REG_MULTI_SZ.
         if kind = RegistryValueKind.String
         then key.DeleteValue(valueName, false)

         key.SetValue(valueName, newFonts, RegistryValueKind.MultiString)
         Console.WriteLine("SimSun added to the list of linked fonts.")
      else
         Console.WriteLine("Font is already linked.")

   
   if not (isNull key) then key.Close()
   0
// </Snippet2>
