' Models/UndoAction.vb
Namespace Models
    ''' <summary>
    '''      مــــلاحظات  
    ''' يمثل سجل عملية واحدة (إضافة، تعديل، حذف) لدعم ميزة التراجع.
    ''' فئة UndoAction مخصصة: يتم إنشاؤها خصيصاً لتخزين تفاصيل عملية التراجع/الإعادة [j.79, j.80]. تحتوي هذه الفئة على خصائص مثل type (من ActionType)، وStudentData (للحالة الجديدة)، وOldStudentData (للحالة القديمة)، وOriginalIndex [j.80].
    ''' </summary>

    'وهو قلب نظام التراجع (Undo) في مشروع
    'تخزين سجل دقيق لعملية واحدة تمت على كائن طالب: سواء كانت إضافة، تعديل، أو حذف — مع حفظ الحالة المرتبطة بها، بحيث يمكن التراجع عنها لاحقًا.

    Public Class UndoAction
        Public Property type As ActionType      ' يحدد نوع العملية (إضافة، تعديل، حذف)
        Public Property StudentData As student2 ' تحتوي على الحالة الجديدة للكائن
        Public Property OldStudentData As student2 '(في حالة التعديل) تحتوي على النسخة القديمة قبل التعديل
        Public Property OriginalIndex As Integer ' (في حالة الحذف) يحتفظ بموقع الطالب قبل إزالة الكائن من القائمة
        'مــلاحظة 
        ' يتم تحديد نوع البيانات الخاص به باستخدام الكلمة As.
        ''' <summary>
        ''' مُنشئ لعمليات الإضافة أو الحذف، لا يتطلب نسخة قديمة.
        ''' ● لعملية الإضافة أو الحذف
        ''' نمرر نوع العملية (Add أو Delete)
        '''نمرر كائن الطالب (StudentData)
        '''نمرر رقم الفهرس (في حالة Delete فقط)
        ''' </summary>
        Public Sub New(actionType As ActionType, data As student2, Optional index As Integer = -1)
            Me.type = actionType
            Me.StudentData = data
            Me.OriginalIndex = index
        End Sub
        ''' <summary>
        ''' مُنشئ لعملية التعديل، يتطلب الحالة الجديدة والقديمة.
        ''' مرر الكائن الجديد (بعد التعديل)
        '''نمرر نسخة الكائن قبل التعديل (نُسخت باستخدام Clone)
        '''هذا يسمح لنا بالتراجع الكامل: استبدال النسخة المعدّلة بالقديمة
        ''' </summary>
        Public Sub New(actionType As ActionType, newData As student2, oldData As student2)
            Me.type = actionType
            Me.StudentData = newData
            Me.OldStudentData = oldData
        End Sub
    End Class
End Namespace