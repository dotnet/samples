'<Snippet1>
Namespace UsageLibrary
Public Class ABaseType
   
   Public Sub BasePublicMethod(argument1 As Integer)
   End Sub
    
End Class 'ABaseType 

Public Class ADerivedType
   Inherits ABaseType
   
   ' Violates rule DoNotDecreaseInheritedMemberVisibility.
   Private Shadows Sub BasePublicMethod(argument1 As Integer)
   End Sub
End Class 'ADerivedType

End Namespace
'</Snippet1>
