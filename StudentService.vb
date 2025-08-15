' BLL/StudentService.vb
Imports System.ComponentModel
Imports System.IO
Imports Complete_program.DAL
Imports Complete_program.Models
Imports DAL
Imports Models

Namespace BLL
    'مــــلاحظات باالكلاس
    'كلمة ReadOnly تعني أنه بمجرد تهيئة هذا الكائن، لا يمكن تغيير الكائن الذي يشير إليه المتغير نفسه، ولكن يمكن إضافة أو إزالة عناصر منه 
    'ستخدام Private ReadOnly يجعل من الصعب على أي كود خارجي تعطيل أو تجاوز القواعد التي تم وضعها في الكلاس، مما يحافظ على سلامة النظام وقابليته للتطوير والاختبار.
    '        _students هو كائن بحد ذاتة وليس وسيط فقط مـــلاحظة 
    '_students وهو مركز إدارة البيانات في الذاكرة لطبقة منطق الأعمال.الكائن 
    'أي تغييرات تحدث في _students (مثل الإضافة أو الحذف أو التعديل)
    '، يتم عكسها تلقائيًا في DataGridView بالواجهة بسبب خاصية الربط التلقائي للـ
    'BindingList، دون الحاجة لتدخل برمجي إضافي في الواجهة 
    'باختصار، المتغير _students هو نفسه الـ BindingList المؤقتة في الذاكرة،
    'وهي مصممة خصيصًا لاحتواء وتنظيم كائنات من النوع student2 التي تمثل بيانات كل طالب، سواء تم تحميلها من ملف أو إضافتها من قبل المستخدم 
    Public Class StudentService

        Private ReadOnly _students As BindingList(Of student2) '..ذه القائمة هي مصدر البيانات الذي يتم ربطة<هو نفسه يمثل القائمة المؤقتة في الذاكرة.>  DataGridView في واجهة المستخدم
        Private ReadOnly _queue As New Queue(Of student2) 'تمثيل لطابور انتظار (مثل تسجيل طلاب مؤجلين)
        ' كل عملية تتم تتسجل كـ(UndoAction)
        ' مثل اضافة تعديل حذف اول مرة تنحط هنا
        'وتنحط كــ(_undoStack)
        Private ReadOnly _undoStack As New Stack(Of UndoAction)
        Private ReadOnly _redoStack As New Stack(Of UndoAction) 'مكدسات لتنفيذ/إلغاء التعديلات باستخدام نمط Command Pattern
        Private ReadOnly _repository As New StudentRepository() 'هذا هو جسر التواصل مع الطبقة الأدنى (DAL) ,,,مثل مبدأ Dependency Inversion ← الطبقة الوسطى لا تهتم بتفاصيل التخزين بل تستدعي واجهة حفظ مستقلة
        Private ReadOnly _form3 As New Design_a_complete_program_for_students_DataGridView4

        ''' <summary>
        '''      حدث عمليتان رئيسيتان   خوارزمية الاجراء Public Sub New()
        '''التحقق من ملف 1 students.csv وإنشائه إذا لم يكن موجودًا: يقوم المُنشئ أولاً بالتحقق مما إذا كان ملف البيانات students.csv
        '''2 تحميل البيانات الأولية: بعد التأكد من وجود الملف، يقوم المُنشئ باستدعاء الدالة _repository.LoadStudents()
        '''موجودًا. إذا لم يكن موجودًا، فإنه يقوم بإنشاء هذا الملف ويكتب فيه سطر رؤوس الأعمدة 
        ''' </summary>
        ''' <remarks>يتم استدعاؤه عند إنشاء كائن StudentService.</remarks>
        ''' </remarks>

        ''' دالة المُنشئ
        '''عند بدء التطبيق، يتم:
        '''التحقق من وجود ملف CSV
        '''تحميل بيانات الطلاب باستخدام DAL
        '''تخزينها في الـ _students للربط مع الواجهة
        '''💡 تحليل باك اند: هذا يعكس مفهوم الـ Initialization الموجه نحو البيانات — وهو نمط يُستخدم دائمًا في التطبيقات القائمة على مصادر بيانات محلية أو خارجية
        Public Sub New() ' يتم استدعاؤها تلقائيًا في كل مرة يتم فيها إنشاء كائن جديد (Instance) من تلك الفئة
            If Not File.Exists("students.csv") Then
                File.WriteAllText("students.csv", "StudentID,Name,Age,Address,EnrollmentYear,StudentClass,Grade" & Environment.NewLine)
            End If
            '''نظرًا لأن الملف فارغ (أو يحتوي على الرؤوس فقط)،
            '''فإن هذه الدالة ستقوم بقراءة الملف
            '''(مع تخطي سطر الرؤوس ، ولن تجد أي بيانات لطلاب، 
            '''وبالتالي ستُرجع قائمة فارغة من كائنات student2 
            Dim loaded = _repository.LoadStudents() 'والتي بدورها تقوم بقراءة البيانات من ملف CSV (سطرًا سطرًا)، وتنشئ كائنات student2 لكل سطر باستخدام مُنشئ
            _students = New BindingList(Of student2)(loaded)
        End Sub
        ''' <summary>
        ''' متى تستخدم؟ عند تحميل الفورم لأول مرة + بعد أي تعديل يحتاج لتحديث DataGridView.
        '''ما تفعل؟ تُرجع القائمة الداخلية _students مباشرةً.
        '''مناسبة؟ لربط الواجهة الحالية بالبيانات المحدثة.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAllStudents() As BindingList(Of student2) 'مهمتها، كما يوحي اسمها، هي جلب جميع كائنات الطلاب (الموجودة في الذاكرة حالياً أو المخزنة بشكل دائم) وإعادتها كـ BindingList(Of student2)
            Return _students '' هنا يتم طلب البيانات من DAL(StudentRepository)
        End Function

        Public Sub SaveAllStudents()
            _repository.SaveStudents(_students.ToList())
        End Sub

        '🧠 الدوال الأساسية في StudentService
        ''' <summary>
        '''   لوظيفة:
        '''   إضافة طالب جديد إلى النظام، مع توليد StudentID آليًا، والتحقق من التكرار، والتسجيل في قائمة التراجع، ثم حفظه على الملف.
        ''' </summary>
        ''' <param name="student"></param>
        ''' <returns></returns>
        ''' 

        '       ذا الطالب المرسل ليس له معرف مسبق (StudentID = 0)، فالنظام يتولّى توليد معرف جديد.
        'If (_students.Any(), ...): تتحقق إذا كانت القائمة تحتوي على طلاب حاليين.
        'إن وُجدوا: تأخذ أعلى StudentID موجود وتضيف عليه 1
        'إن لم يوجدوا: تبدأ بـ 1

        Public Function AddStudent(student As student2) As student2 'فإن الدالة ترجع كائنًا من نوع student2  تحديدًا، هي ترجع كائن الطالب الذي تمت إضافته بنجاح
            If student.StudentID = 0 Then
                'تسجيل العملية للتراجع

                '''_students.Max()=اذى كان هناك طلاب موجودين في القائمة الداخلية للطلاب فانة سوف يتم ارجاع  (Boolean=true) 
                '''_students.Max(Function(s) s.StudentID) + 1: إذا كانت هناك طلاب موجودون (_students.Any() ترجع True) = فان هاذى الجزئ يجد اعلى قيمة للمعرف(StudentID),بين جميع الطلاب الحاليين ويضيف إليها 1 للحصول على معرف فريد جديد 
                ''' إذا لم يكن هناك أي طلاب في القائمة بعد (_students.Any() ترجع False)، فإن هذا يعني أن هذا هو أول طالب يتم إضافته، وبالتالي يتم تعيين StudentID الخاص به إلى 1
                student.StudentID = If(_students.Any(), _students.Max(Function(s) s.StudentID) + 1, 1) 'هدف هذه الجزئية إلى توليد معرفات طلاب فريدة ومتسلسلة تلقائيًا، مما يضمن أن كل طالب جديد يحصل على StudentID فريد ويحافظ على تسلسل المعرفات حتى بعد إعادة تشغيل التطبيق
                '''🧪 أولاً: التحقق من التكرار (Prevent Duplicate Entry) ✅  منع إدخال طالب بنفس الاسم مرتين داخل النظام.  الهدف
                '''_students.Any()=تعني: "هل يوجد أي عنصر داخل ,, _students يطابق الشرط التالي
                '''هذه دالة من LINQ (لغة الاستعلام المدمجة مع .NET)
                '''من منظور باك اند: نستخدمها لفحص وجود كائن بناءً على شرط معين (الاسم مثلاً) بكفاءة وسرعة
                '''ترجع True إذا تم العثور على عنصر واحد على الأقل يحقق الشرط
                '''   Function(s)=    هذا تعريف دالة Lambda: دالة قصيرة داخلية تقوم بوصف "ما هو الشرط" المطلوب التحقق منه
                '''🔹 يعني نمر على كل عنصر داخل _students واحد تلو الآخر، وكل عنصر يُرمز له بـ s، ونطبق عليه
                '''s.name= اسم الطالب الحالي داخل القائمة.
                '''student.name: اسم الطالب الجديد المراد إضافته.
                '''Equals(...): دالة مقارنة نصوص.
                '''StringComparison.OrdinalIgnoreCase: تعني تجاهل حالة الأحرف — "أحمد" = "احمد". 🔹 لماذا مهم؟ لأننا لا نريد أن نُخدع بأن الاسم مختلف فقط بسبب الحرف الأول كبير أو صغير.

                '''هذا يمثل الكاش (Cache) المؤقت اللي نشتغل عليه قبل ما نرسل للـ DAL
                '''🎯 النتيجة الكاملة
                '                كل هذا السطر معًا يعني: 
                '> "لو وُجد داخل القائمة _students أي طالب اسمه يطابق اسم الطالب الحالي (مع تجاهل حالة الأحرف)، إذًا نوقف العملية ونرمي استثناء."
                If _students.Any(Function(s) s.name.Equals(_form3.Name, StringComparison.OrdinalIgnoreCase)) Then 'اذى كان الاسم فريد راح يتم الحفظ في القائمة الداخلية
                    Throw New Exception("طالب بنفس الاسم موجود بالفعل.")

                End If
            End If
            '3. الإضافة إلى القائمة المؤقتة:
            '            تُضاف إلى BindingList داخل الذاكرة.
            'DataGridView يرتبط بها تلقائيًا — بفضل Data Binding.
            _students.Add(student)
            'UndoAction = هنا يتم اضافة كائن جديد  من نوع
            'ActionType.AddStudent =هو عضو من تعداد (Enum) يسمى ActionType، ويحدد أن العملية التي تم إجراؤها هي "إضافة طالب"
            'student = و كائن student2 الذي تم إضافته للتو. يتم تمريره إلى كائن UndoAction ليتم تخزينه كبيانات ضرورية للتراجع عن هذه العملية لاحقًا (أي لحذف هذا الطالب إذا تم طلب التراجع)
            _undoStack.Push(New UndoAction(ActionType.AddStudent, student))
            '        ملاحظة(   )    _studentRepository.SaveStudents(_students) ' حفظ البيانات بعد كل عملية (اختياري، يمكن الحفظ عند إغلاق التطبيق)
            _redoStack.Clear() 'تنظيف مكدس Redo:لأن أي عملية جديدة تُبطل إمكانية إعادة التنفيذ السابقة

            '''6. (اختياري) حفظ دائم في ملف CSV:
            '''    ترسل كل القائمة الحالية إلى DAL لحفظها في CSV.
            '''طبقة BLL لا تهتم بكيفية الحفظ، فقط تستدعي StudentRepository
            _repository.SaveStudents(_students.ToList())
            ' 1. إنشاء الطالب من المصنع
            Dim student As IStudent = StudentFactory.CreateStudent("Guest", 101, "أحمد")

            ' 2. إضافة الطالب عبر منطق الأعمال
            Dim service As New StudentService()
            service.AddStudent(student)

            Return student '
        End Function
        ''' <summary>
        '''   مــلاحظة
        '''   تحديث بيانات الطالب 
        '''   بهذا يمكننا التراجع بشكل دقيق.
        ''' </summary>
        ''' <param name="updated"></param>
        Public Sub UpdateStudent(updated As student2) 'تحديث خصائص الطالب الأصلي في StudentService.UpdateStudent 
            Dim original = _students.FirstOrDefault(Function(s) s.StudentID = updated.StudentID)
            If original IsNot Nothing Then
                Dim oldCopy = original.Clone() 'نسخة الطالب القديمة) يتم إنشاؤها باستخدام original.Clone() قبل تحديث خصائص الطالب الأصلي في StudentService.UpdateStudent [j.107].
                original.name = updated.name
                original.age = updated.age
                original.address = updated.address
                original.enrollmentYear = updated.enrollmentYear
                original.studentClass = updated.studentClass
                original.grade = updated.grade
                _undoStack.Push(New UndoAction(ActionType.EditStudent, original.Clone(), oldCopy))
                _redoStack.Clear()
            End If
        End Sub

        Public Sub DeleteStudent(studentID As Integer)
            Dim student = _students.FirstOrDefault(Function(s) s.StudentID = studentID)
            If student IsNot Nothing Then
                Dim index = _students.IndexOf(student)
                _students.Remove(student)
                _undoStack.Push(New UndoAction(ActionType.DeleteStudent, student, index))
                _redoStack.Clear()
            End If
        End Sub
        'مــــلاحظة مهم
        'اكثر من مرة ثم بعدة عملنا عملية جديد فلن نستطيع التراجع عن اخر عملية كانت موجودة(Undo)اذى قمنا بعمل 

        'UndoLastAction = التراجع عن الإجراء الأخير
        'عملة تقوم بسحب اخر عملية من (_undoStack) وتعكس تاثيرها 
        'Add=Delete
        'Update = يرد OldStudentData اي الحالة القديم 
        'Delete = يرجع يضيف الطالب مرة ثاني الى مكانة الاصلي
        'وبعدما يكمل العملية هاذي يرجع يحط العملية كلة في مكدس(_redoStack)
        ''' <summary>
        ''' الدالة تبدأ بفحص ما إذا كان هناك عملية سابقة مسجلة في مكدس التراجع. إذا كان المكدس فارغًا، تُعيد رسالة تُفيد بأنه لا توجد عمليات للتراجع. وإلا، تقوم بسحب آخر عملية (Pop) من المكدس وتحليل نوعها باستخدام تعبير Select Case.
        ''' </summary>
        ''' <returns></returns>

        '''        UndoLastAction تسترد آخر عملية من Undo Stack، تتصرف بناءً على نوع العملية (إضافة، تعديل، حذف) وتعيد النظام إلى حالته السابقة.
        '''تضمن الدالة ان كل تغيير كان مسجلاً بدقة، سواءً من خلال القائمة أو من خلال حفظ مكان الكائن في القائمة أو عن طريق حفظ نسخة سابقة منه.
        '''تُستخدم مكدسات Undo وRedo لتحسين تجربة المستخدم وإعطائه القدرة على استعادة أو إعادة تنفيذ العمليات.
        Public Function UndoLastAction() As String '
            If _undoStack.Count = 0 Then Return "لا توجد عمليات للتراجع."
            Dim last = _undoStack.Pop() 'سحب آخر عملية: باستخدام Pop() نحصل على آخر عملية تمت تسجيلها.
            '''الشرح:
            '''            البحث عن الطالب: يتم استخدام LINQ للعثور على الطالب في القائمة _students الذي يتطابق مع معرف الطالب في last.StudentData.
            '''إزالة الطالب: إذا تم العثور على الطالب، تتم إزالته من القائمة. هذا يفسّر بأن التراجع عن الإضافة هو حذف الكائن الذي تمت إضافته.
            '''تسجيل العملية في Redo Stack: بعد إزالة الطالب، يتم دفع عملية التراجع (UndoAction) إلى مكدس إعادة التنفيذ (Redo Stack) بحيث يمكن استعادة العملية إذا قرر المستخدم إعادة العملية.
            '''إرجاع رسالة نجاح: تُعيد الدالة رسالة تشير إلى نجاح التراجع عن عملية الإضافة.
            Select Case last.type
                Case ActionType.AddStudent 'تبحث عن الطالب تحذفه.
                    Dim toRemove = _students.FirstOrDefault(Function(s) s.StudentID = last.StudentData.StudentID)
                    If toRemove IsNot Nothing Then
                        _students.Remove(toRemove)
                        _redoStack.Push(New UndoAction(ActionType.AddStudent, toRemove))
                    End If
                    Return $"تم التراجع عن إضافة الطالب: {last.StudentData.name}"
                    '''لشرح:
                    '''                    البحث عن الطالب المعدل: يتم العثور على الطالب الحالي في القائمة باستخدام الـ StudentID.
                    '''تسجيل الحالة الحالية والقديمة في Redo Stack: هنا يتم إنشاء نسخة طبق الأصل من الحالة الحالية (current.Clone()) والحالة القديمة (last.OldStudentData.Clone())، ثم دفعهما إلى مكدس إعادة التنفيذ. هذا يسمح لكلا من Undo وRedo بالعمل بدقة.
                    '''استعادة الحالة القديمة: تُستَخدم القيم الموجودة في last.OldStudentData (النسخة التي تم حفظها قبل التعديل) لاستعادة كل خاصية من خصائص الطالب (كالاسم والعمر والعنوان، إلخ).
                    '''إرجاع رسالة نجاح التراجع: تُعيد الدالة رسالة تفيد بأن عملية تعديل الطالب قد تم التراجع عنها بنجاح.
                Case ActionType.EditStudent 'تستعيد الحالة القديمة باستخدام النسخة المخزنة.
                    Dim current = _students.FirstOrDefault(Function(s) s.StudentID = last.StudentData.StudentID)
                    If current IsNot Nothing Then
                        _redoStack.Push(New UndoAction(ActionType.EditStudent, current.Clone(), last.OldStudentData.Clone()))
                        current.name = last.OldStudentData.name
                        current.age = last.OldStudentData.age
                        current.address = last.OldStudentData.address
                        current.enrollmentYear = last.OldStudentData.enrollmentYear
                        current.studentClass = last.OldStudentData.studentClass
                        current.grade = last.OldStudentData.grade
                        Return $"تم التراجع عن تعديل الطالب: {current.name}"
                    End If
                    '''الشرح:
                    '''                    تحديد موقع الإدراج: تستخدم العملية رقم الفهرس الأصلي (OriginalIndex) الذي كان فيه الطالب قبل الحذف. إذا كان هذا الرقم صالحاً (داخل حدود القائمة)، يتم إعادة إدخال الطالب في موقعه الأصلي.
                    '''معالجة حالة الفهرس غير الصالح: إذا لم يكن الفهرس صالحًا، يتم ببساطة إضافة الطالب في نهاية القائمة.
                    '''تسجيل العملية في Redo Stack: بعد إرجاع الطالب، يتم دفع عملية التراجع المُعكوسة إلى مكدس إعادة التنفيذ بحيث يمكن إعادة الحذف في حال أردت إعادة العملية.
                    '''إرجاع رسالة نجاح: تُعيد الدالة رسالة تؤكد نجاح التراجع عن عملية الحذف.
                Case ActionType.DeleteStudent 'عيد الطالب إلى موقعه الأصلي (أو تضيفه إذا لم يكن الموقع صالحًا).
                    Dim index = last.OriginalIndex
                    If index >= 0 AndAlso index <= _students.Count Then
                        _students.Insert(index, last.StudentData)
                    Else
                        _students.Add(last.StudentData)
                    End If
                    _redoStack.Push(New UndoAction(ActionType.DeleteStudent, last.StudentData, index))
                    Return $"تم التراجع عن حذف الطالب: {last.StudentData.name}"
            End Select
            Return "فشل التراجع."
        End Function
        ''' <summary>_redoStack
        ''' 'مــــلاحظة مهم
        '''اكثر من مرة ثم بعدة عملنا عملية جديد فلن نستطيع التراجع عن اخر عملية كانت موجودة(Undo)اذى قمنا بعمل 
        '''      الوظــيفة
        '''       RedoLastAction  =اعادة الاجراء الاخير 
        '''      اذى حبينا نعمل( RedoLastAction )
        '''      فراح يسحب من(_redoStack )
        '''      , المخزنة فيها من السابق عملية _undoStack
        '''      وينفذ  العملية الي فيها ثم
        '''    يرجع يحط العملية الاصلية مرة ثاني في  
        '''    _undoStack وهاكذا  
        ''' </summary>
        ''' <returns></returns>
        Public Function RedoLastAction() As String
            If _redoStack.Count = 0 Then Return "لا توجد عمليات لإعادة التنفيذ."
            Dim last = _redoStack.Pop()

            Select Case last.type
                Case ActionType.AddStudent
                    _students.Add(last.StudentData)
                    _undoStack.Push(New UndoAction(ActionType.AddStudent, last.StudentData))
                    Return $"تم إعادة إضافة الطالب: {last.StudentData.name}"

                Case ActionType.EditStudent
                    Dim current = _students.FirstOrDefault(Function(s) s.StudentID = last.OldStudentData.StudentID)
                    If current IsNot Nothing Then
                        _undoStack.Push(New UndoAction(ActionType.EditStudent, last.StudentData, current.Clone()))
                        current.name = last.StudentData.name
                        current.age = last.StudentData.age
                        current.address = last.StudentData.address
                        current.enrollmentYear = last.StudentData.enrollmentYear
                        current.studentClass = last.StudentData.studentClass
                        current.grade = last.StudentData.grade
                        Return $"تم إعادة تعديل الطالب: {current.name}"
                    End If

                Case ActionType.DeleteStudent
                    Dim toRemove = _students.FirstOrDefault(Function(s) s.StudentID = last.StudentData.StudentID)
                    If toRemove IsNot Nothing Then
                        _students.Remove(toRemove)
                        _undoStack.Push(New UndoAction(ActionType.DeleteStudent, toRemove, last.OriginalIndex))
                        Return $"تم إعادة حذف الطالب: {toRemove.name}"
                    End If
            End Select
            Return "فشل إعادة التنفيذ."
        End Function

        Public Sub EnqueueStudent(student As student2)
            _queue.Enqueue(student)

        End Sub

        Public Function DequeueStudent() As student2
            If _queue.Count = 0 Then Return Nothing
            Dim student = _queue.Dequeue()
            Return AddStudent(student)
        End Function

        Public Function PeekQueue() As student2() 'تُرجع نسخة من جميع الطلاب المنتظرين دون إزالتهم < للعرض فقط، دون التعديل على الطابور
            Return _queue.ToArray()
        End Function

        Public Function SearchByName(keyword As String) As List(Of student2)
            Return _students.Where(Function(s) s.name.ToLower().Contains(keyword.ToLower())).ToList()
        End Function
        ''' <summary>
        ''' متى تستخدم؟ عند البحث برقم الطالب (ComboBox = "الرقم").
        '''ما تفعل؟ تُرجع طالب واحد فقط (أو Nothing) له نفس المعرف تمامًا.
        '''مناسبة؟ للبحث الحرفي أو لتعديل سجل معين.
        ''' </summary>
        ''' <param name="studentID"></param>
        ''' <returns></returns>
        Public Function SearchByID(studentID As Integer) As student2
            Return _students.FirstOrDefault(Function(s) s.StudentID = studentID)
        End Function
        Public Function SortByField(field As String, Optional ascending As Boolean = True) As List(Of student2)
            ' تحويل القائمة إلى IEnumerable لدعم عمليات LINQ
            Dim query = _students.AsEnumerable()
            ' تحويل الحقل إلى حروف صغيرة لضمان المقارنة الصحيحة
            Select Case field.ToLower()
                            ' إذا كان الترتيب تصاعدي، نستخدم OrderBy، وإلا OrderByDescending
                          '  ذا كانت قيمة ascending صحيحة (True)، سيتم ترتيب القائمة تصاعدياً بواسطة خاصية name لكل طالب.
                              'إذا كانت False، يتم ترتيب القائمة تنازلياً
                Case "name" : query = If(ascending, query.OrderBy(Function(s) s.name), query.OrderByDescending(Function(s) s.name))
                Case "age" : query = If(ascending, query.OrderBy(Function(s) s.age), query.OrderByDescending(Function(s) s.age))
                Case "grade" : query = If(ascending, query.OrderBy(Function(s) s.grade), query.OrderByDescending(Function(s) s.grade))
                Case "id", "studentid" : query = If(ascending, query.OrderBy(Function(s) s.StudentID), query.OrderByDescending(Function(s) s.StudentID))
                    ' إذا لم يتطابق الحقل مع الحالات المُحددة، نُرجع القائمة دون ترتيب
                Case Else : Return _students.ToList() 'إذا لم يتطابق الحقل المدخل مع أيٍ من الحالات المحددة، تُعاد القائمة كما هي بدون ترتيب

            End Select
            ' إعادة القائمة المرتبة كـ List(Of student2)
            Return query.ToList() 'إعادة القائمة المرتبة إلى الطبقة العليا (على سبيل المثال لتحديث DataGridView في الواجهة
        End Function
        Public Interface IStudentRepository
            Sub Save(student As student2)
        End Interface
    End Class
End Namespace
