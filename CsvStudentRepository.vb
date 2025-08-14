
Imports LAERN_USE.BLL.Interface
Imports LAERN_USE.DomainModels
'''🔸 2. طبقة BLL (Business Logic Layer)
'''✅ تنفّذ العمليات المنطقية على البيانات:
'''التحقق من صحة البيانات
'''تنفيذ العمليات التجارية
'''📌 BLL لا تعرف شيئًا عن التخزين أو الواجهة ↪️ هي فقط مسؤولة عن منطق العمل
'''


'🔸 3. طبقة DAL (Data Access Layer)
'''✅ تنفّذ العمليات الفعلية على قاعدة البيانات أو الملف (CSV مثلاً):
'''قراءة الملف
'''كتابة سطر جديد
'''حذف السطر المطابق
'''📌 DAL لا تعرف شيئًا عن الواجهة أو المستخدم ↪️ هي فقط مسؤولة عن التخزين وتنفيذ العمليات
Namespace DLL
    '3. 🔹 تنفيذ التخزين – داخل طبقة DAL (باستخدام CSV كمثال)
    '    🧑‍💻 برمجياً:
    'هذا هو التنفيذ الفعلي لـ Repository باستخدام ملفات CSV
    '🧠 خوارزمياً:
    'كل عملية تقوم بتحرير الملف حسب نوع الطلب: إضافة، قراءة، تعديل، حذف
    Public Class CsvStudentRepository '🧠 تنفيذ التخزين الفعلي باستخدام ملف CSV

        Implements IStudentRepository

        Private ReadOnly filePath As String = "students.csv"

        ''' <summary>
        '''         🧱 1. Repository Pattern ⇄ DIP + SRP
        '''         🔎 ماذا يحدث؟
        '''في CsvStudentRepository, يتم تحويل الكائن إلى سطر نصي
        '''يُضاف السطر إلى الملف CSV ✔️ الفصل الكامل: لا أحد في الطبقة العليا يعرف أن التخزين يتم عبر ملف!
        ''''يُضاف السطر إلى الملف بحيث انة تحقق الفصل الكامل :وهاذى يعني ان لااحد من الطبقة العليا تعرف ان التخزين يتم عبر ملف CSV

        ''' 
        '''     🧠 ملخص التدفق في Repository:
        '''     Form1 → StudentController → StudentService → IStudentRepository → CsvStudentRepository
        Public Sub Create(student As STUDENT4) Implements IStudentRepository.Create
            Dim line = $"{student.Id},{student.Name},{student.Age}" 'يتم تحويل الكائن إلى سطر نصي
            IO.File.AppendAllText(filePath, line & Environment.NewLine) 'يُضاف السطر إلى الملف بحيث انة تحقق الفصل الكامل :وهاذى يعني ان لااحد من الطبقة العليا تعرف ان التخزين يتم عبر ملف CSV
        End Sub

        Public Function Read(id As Integer) As STUDENT4 Implements IStudentRepository.Read
            Return IO.File.ReadAllLines(filePath).
                Select(Function(line) line.Split(","c)).
                Where(Function(parts) CInt(parts(0)) = id).
                Select(Function(parts) New STUDENT4 With {.Id = CInt(parts(0)), .Name = parts(1), .Age = CInt(parts(2))}).
                FirstOrDefault()
        End Function

        Public Sub Update(student As STUDENT4) Implements IStudentRepository.Update
            Dim lines = IO.File.ReadAllLines(filePath).
                Select(Function(line)
                           Dim parts = line.Split(","c)
                           If CInt(parts(0)) = student.Id Then
                               Return $"{student.Id},{student.Name},{student.Age}"
                           Else
                               Return line
                           End If
                       End Function).
                ToList()
            IO.File.WriteAllLines(filePath, lines)
        End Sub

        Public Sub Delete(id As Integer) Implements IStudentRepository.Delete
            Dim lines = IO.File.ReadAllLines(filePath).
                Where(Function(line) CInt(line.Split(","c)(0)) <> id).
                ToArray()
            IO.File.WriteAllLines(filePath, lines)
        End Sub

        Public Function GetAll() As List(Of STUDENT4) Implements IStudentRepository.GetAll
            Return IO.File.ReadAllLines(filePath).
                Select(Function(line)
                           Dim parts = line.Split(","c)
                           Return New STUDENT4 With {.Id = CInt(parts(0)), .Name = parts(1), .Age = CInt(parts(2))}
                       End Function).
                ToList()
        End Function

    End Class
End Namespace

''🧰 3. DAL: التخزين الحقيقي داخل المشروع مع المحول Adapter Imports LAERN_USE.DomainModels
''Imports LAERN_USE.BLL.Interface

''Namespace DLL

'Imports LAERN_USE.BLL
'Imports LAERN_USE.BLL.Interface
'Imports LAERN_USE.DomainModels
''''🔸 2. طبقة BLL (Business Logic Layer)
''''✅ تنفّذ العمليات المنطقية على البيانات:
''''التحقق من صحة البيانات
''''تنفيذ العمليات التجارية
''''📌 BLL لا تعرف شيئًا عن التخزين أو الواجهة ↪️ هي فقط مسؤولة عن منطق العمل
''''


''🔸 3. طبقة DAL (Data Access Layer)
''''✅ تنفّذ العمليات الفعلية على قاعدة البيانات أو الملف (CSV مثلاً):
''''قراءة الملف
''''كتابة سطر جديد
''''حذف السطر المطابق
''''📌 DAL لا تعرف شيئًا عن الواجهة أو المستخدم ↪️ هي فقط مسؤولة عن التخزين وتنفيذ العمليات
'Namespace DLL
'    '3. 🔹 تنفيذ التخزين – داخل طبقة DAL (باستخدام CSV كمثال)
'    '    🧑‍💻 برمجياً:
'    'هذا هو التنفيذ الفعلي لـ Repository باستخدام ملفات CSV
'    '🧠 خوارزمياً:
'    'كل عملية تقوم بتحرير الملف حسب نوع الطلب: إضافة، قراءة، تعديل، حذف
'    Public Class CsvStudentRepository '🧠 تنفيذ التخزين الفعلي باستخدام ملف CSV
'        '     '🧑‍💻 برمجياً: تعريف الكلاس الذي ينفذ واجهة التخزين
'        '🧠 خوارزمياً: يُخضع نفسه لعقد IStudentRepository ويلتزم بتنفيذ كل وظائف CRUD.
'        Implements IStudentRepository
''🧑‍💻 برمجياً: مسار الملف الذي سيتم استخدامه كمخزن دائم
''🧠 خوارزمياً: يُستخدم لجميع العمليات لفتح وقراءة وكتابة البيانات
'Private ReadOnly filePath As String = "students.csv" '
''''        🧑‍💻 برمجياً:
''''تحويل الكائن إلى تمثيل نصي CSV
''''إضافته كسطر جديد إلى الملف
''''🧠 خوارزمياً:
''''يحوّل الكائن إلى سلسلة
''''يضيفها إلى الملف دون أن يؤثر على المحتوى السابق
'Public Sub Create(student As STUDENT4) Implements IStudentRepository.Create
'    Dim line As String = $"{student.Id},{student.Name},{student.Age}"
'    IO.File.AppendAllText(filePath, line & Environment.NewLine)
'End Sub
''''        🧑‍💻 برمجياً:
''''يقرأ كل السطور من الملف
''''يبحث عن الطالب ذو الرقم المحدد
''''🧠 خوارزمياً:
''''لكل سطر: يفصل البيانات
''''إذا طابق Id، يحوّل السطر إلى كائن ويعيده
''''وإلا يرجع Nothing = لم يُعثر عليه
'Public Function Read(id As Integer) As STUDENT4 Implements IStudentRepository.Read
'    ' الوظيفة الكلي
'    'جلب طالب بواسطة ID
'    Dim lines = IO.File.ReadAllLines(filePath)
'    For Each line In lines
'        Dim parts = line.Split(","c)
'        If CInt(parts(0)) = id Then
'            Return New STUDENT4 With {.Id = id, .Name = parts(1), .Age = CInt(parts(2))}
'        End If
'    Next
'    Return Nothing
'End Function
'' 🔄 نفس الوظيفة باستخدام LINQ
''Public Function Read(id As Integer) As STUDENT4 Implements IStudentRepository.Read
''    Return IO.File.ReadAllLines(filePath).
''Select(Function(line) line.Split(","c)).
''Where(Function(parts) CInt(parts(0)) = id).
''Select(Function(parts) New STUDENT4 With {.Id = CInt(parts(0)), .Name = parts(1), .Age = CInt(parts(2))}).
''FirstOrDefault()
''End Function



''''        🧑‍💻 برمجياً:
''''يحمّل الملف كسطور إلى List
''''يبحث عن السطر المطابق
''''يعدّله
''''يكتب الملف مجددًا
''''🧠 خوارزمياً:
''''حمّل الذاكرة → عدّل السطر → أعد الكتابة بالكامل
''''لا يوجد تعديل مباشر في ملفات CSV، لذا يتم إعادة بناء كامل للملف
'Public Sub Update(student As STUDENT4) Implements IStudentRepository.Update
'    Dim lines = IO.File.ReadAllLines(filePath).ToList()
'    For i = 0 To lines.Count - 1
'        Dim parts = lines(i).Split(","c)
'        If CInt(parts(0)) = student.Id Then
'            lines(i) = $"{student.Id},{student.Name},{student.Age}"
'        End If
'    Next
'    IO.File.WriteAllLines(filePath, lines)
'End Sub
'''' <summary>
''''   ' 🔄 نفس الوظيفة باستخدام LINQ
'''' </summary>
'''' <param Update="student">بااستخدام LINQ</param>
'''' <param name="student"></param>

''    Public Sub Update(student As STUDENT4) Implements IStudentRepository.Update
''        ' 🔄 باستخدام LINQ بدل For Loop
''        Dim lines = IO.File.ReadAllLines(filePath).
''Select(Function(line)
''           Dim parts = line.Split(","c)
''           If CInt(parts(0)) = student.Id Then
''               Return $"{student.Id},{student.Name},{student.Age}"
''           Else
''               Return line
''           End If
''       End Function).
''ToList()
''        IO.File.WriteAllLines(filePath, lines)
''    End Sub



''''🧑‍💻 برمجياً:
''''يقرأ الملف
''''يفلتر السطور التي لا تطابق Id
''''ثم يعيد الكتابة
''''🧠 خوارزمياً:
''''يُحذف السطر من الذاكرة
''''ثم يُكتب الملف من جديد بدون هذا السطر   
'Public Sub Delete(id As Integer) Implements IStudentRepository.Delete
'    Dim lines = IO.File.ReadAllLines(filePath).Where(Function(line) CInt(line.Split(","c)(0)) <> id).ToArray()
'    IO.File.WriteAllLines(filePath, lines)
'End Sub
'Public Function GetAll() As List(Of STUDENT4) Implements IStudentRepository.GetAll
'    Dim students = IO.File.ReadAllLines(filePath).
'                Select(Function(line)
'                           Dim parts = line.Split(","c)
'                           Return New STUDENT4 With {.Id = CInt(parts(0)), .Name = parts(1), .Age = CInt(parts(2))}
'                       End Function).
'                ToList()
'    Return students
'End Function
''End Namespace


