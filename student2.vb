' Models/student2.vb
Namespace Models
    ''' <summary>
    ''' يمثل كائن الطالب مع خصائصه.
    ''' </summary>
    Public Class student2
        Public Property StudentID As Integer     ' رقم الطالب
        Public Property name As String          ' اسم الطالب
        Public Property age As Integer          ' عمر الطالب
        Public Property address As String       ' عنوان الطالب
        Public Property enrollmentYear As Integer ' سنة الالتحاق
        Public Property studentClass As Integer ' الصف الدراسي
        Public Property grade As Double         ' معدل الطالب

        ' مُنشئ فارغ لاستخدامه مع أدوات الربط
        'هذا المُنشئ الفارغ موجود بشكل أساسي لدعم تقنيات الربط بالبيانات
        '(Data Binding) في .NET Framework، ويسمح لأدوات مثل DataGridView بإنشاء كائنات student2 بشكل ضمني إذا لزم الأمر،
        Public Sub New()
        End Sub
        'هذا المنشئ هو المسؤول عن تهيئة كائن student2 بجميع قيمه الأساسية عند إنشائه [j.104].
        ' مُنشئ مخصص لتعيين جميع القيم مباشرة
        'تُستخدم هذه الوظيفة، على سبيل المثال، في طبقة منطق الأعمال (StudentService) لإنشاء نسخة احتياطية من بيانات الطالب القديمة قبل إجراء التعديلات، وذلك لدعم وظيفة التراجع (Undo) 
        Public Sub New(id As Integer, name As String, age As Integer, address As String,
                       enrollmentYear As Integer, studentClass As Integer, grade As Double)
            Me.StudentID = id
            Me.name = name
            Me.age = age
            Me.address = address
            Me.enrollmentYear = enrollmentYear
            Me.studentClass = studentClass
            Me.grade = grade
        End Sub
        'مـــلاحظة 
        'نسخ عميق" (deep copy) يدويًا.Clon=
        'المعنى: هذا هو جوهر عملية النسخ. New student2(...) يقوم بـ
        '1.
        'تخصيص مساحة جديدة في الذاكرة لكائن student2 جديد تمامًا.
        '2.

        'استدعاء المُنشئ(Constructor) الخاص بفئة student2 الذي يستقبل جميع الخصائص كمعاملات (parameters).
        'تُنشئ كائنًا جديدًا وتنسخ قيم الحقول (properties) من الكائن الأصلي إلى الكائن الجديد. إذا كانت هذه الحقول أنواع قيمة (مثل Integer, Double, String)، فإنه يتم نسخ قيمها مباشرة. أما إذا كانت الحقول أنواع مرجعية (Reference types) - أي كائنات أخرى - فإنها تنسخ فقط المرجع (reference) وليس الكائن نفسه
        ' إذا قمنا فقط بتخزين مرجع (reference) إلى الكائن الأصلي، فإن أي تعديلات لاحقة على الكائن الأصلي ستؤثر أيضًا على "النسخة القديمة" المخزنة، مما يجعل عملية التراجع غير ممكنة أو خاطئة. Clone() تضمن أن لدينا نسخة مستقلة تمامًا من البيانات في تلك اللحظة الزمنية 

        ' نسخ الكائن لإعادة استخدامه في عمليات التراجع
        'يعني أن هذه الدالة ستُعيد (Return) كائنًا واحدًا من النوع student2 
        ' مــــلاحظة 
        'student2() أو As List(Of student2) إذا كانت قائمة (List) [مثال على قائمة
        'BindingList(Of student2) في لكن As student2 تعني كائنًا مفردًا.
        'تُستخدم لإنشاء نسخة جديدة مستقلة من الكائن الحالي (Me)

        ''' <summary>
        '''  تم انشاء دالة النسخ العميق لمصلحت(_undoStack اوUndoLastAction)
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As student2 ''تعني أن الدالة Clone ستعيد كائنًا جديدًا من نوع student2 (وليس مصفوفة)، وهذا الكائن الجديد سيكون نسخة من الكائن الحالي. هذه النسخة ضرورية للحفاظ على "الحالة السابقة" للكائن، مما يتيح إمكانية التراجع عن التغييرات بشكل صحيح في نظام التراجع
            Return New student2(Me.StudentID, Me.name, Me.age, Me.address,
                                 Me.enrollmentYear, Me.studentClass, Me.grade)
        End Function
    End Class
End Namespace