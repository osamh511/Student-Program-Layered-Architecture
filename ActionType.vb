' Models/ActionType.vb
Namespace Models
    ''' <summary>
    '''     مــــلاحظات 
    ''' يحدد أنواع العمليات الممكنة في سجل التراجع.
    ''' عرّف أنواع العمليات الممكنة (AddStudent, EditStudent, DeleteStudent) 

    ''' </summary>
    Public Enum ActionType
        AddStudent     ' عملية إضافة طالب
        EditStudent    ' عملية تعديل طالب
        DeleteStudent  ' عملية حذف طالب
    End Enum
End Namespace