' UI/Design_a_complete_program_for_students_DataGridView3.vb
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports BLL
Imports Complete_program.BLL
Imports Complete_program.DAL
Imports Complete_program.Models
Imports Models



'''مــــلاحظات 
'''الوصول من طبقة الواجهة (UI Layer): عندما تحتاج الواجهة الرسومية (الفورم 
'''إلى عرض بيانات الطلاب، فإنها لا تنشئ 
'''BindingList
'''خاص بها، بل تطلب القائمة من
'''StudentService 
'''

' بااستخدام طريقة فصل المسؤوليات بطريقة (Layered Architecture):
'اصبح جوههر عمل الفورم أصبح مسؤولاً فقط عن:
'عرض البيانات في الـ DataGridView.
'تلقي المدخلات من المستخدم.
'استدعاء خدمات من طبقة منطق العمل (BLL) لتنفيذ العمليات.
'عرض رسائل النجاح أو الخطأ للمستخدم.
Public Class Design_a_complete_program_for_students_DataGridView4
    ' تعريف الطبقات والطبقة الخاصة بالخدمات
    Inherits Form
    Private studentService As New StudentService()
    Private students As BindingList(Of student2) ' هو نقطة الربط الحية بين البيانات في ذاكرة التطبيق (التي يديرها StudentService) وبين العرض المرئي لهذه البيانات في الـ DataGridView، مما يوفر تحديثات تلقائية وسلسة للواجهة., و العمود الفقري لربط البيانات (Data Binding) بين منطق التطبيق وواجهة المستخدم الرسومية (GUI)، تحديداً الـ DataGridView.

    Private Sub Design_a_complete_program_for_students_DataGridView4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''' خوارزمية     students = studentService.GetAllStudents()
        '''فإنها تقوم ببساطة بإرجاع القائمة الحالية لجميع الطلاب
        '''(_students BindingList) التي تحتفظ بها فئة StudentService
        '''. هذه الدالة لا تقوم بتحميل البيانات من ملف CSV أو معالجتها في وقت الاستدعاء؛ بل
        '''هي فقط توفر وصولاً للبيانات الموجودة والمدارة بالفعل داخل StudentService 
        '''بحيث يتم استخدامها في واجهات تحديث الفورم عنداحتياج عمليات مختلفى 
        students = studentService.GetAllStudents() ' لا تقوم بتحميل البيانات من ملف CSV أو معالجتها بنفسها في وقت الاستدعاء  < جلب البيانات عبر الخدمة هذا السطر هو نقطة الاتصال الرئيسية بين طبقة الواجهة (UI) وطبقة منطق العمل (BLL) عند بدء تشغيل التطبيق أو عندما تحتاج الواجهة إلى جلب جميع البيانات.
        StudentsDataGridView.DataSource = students

        ComboBoxSearchBy.Items.AddRange({"الاسم", "الرقم"})
        ComboBoxSearchBy.SelectedIndex = 0

        ComboBoxSortBy.Items.AddRange({"Name", "Age", "Grade", "StudentID"})
        ComboBoxSortBy.SelectedIndex = 0
        CheckBoxSortAsc.Checked = True
    End Sub
    ''' <summary>
    ''' تسلسل الأحداث من وجهة نظر الباك اند:🔄
    ''' تسلسل الأحداث من وجهة نظر الباك اند
    '''plaintext
    '''[UI Layer - Button Add]
    '''↳ يجمع البيانات من TextBoxes
    '''↳ يتحقق من صحتها
    '''↳ ينشئ كائن student2 بـ StudentID = 0
    '''↳ يرسل الكائن إلى:
    '''[BLL - StudentService.AddStudent()]
    '''    ↳ يولد ID
    '''    ↳ يتحقق من التكرار
    '''    ↳ يضيف للقائمة
    '''    ↳ يسجل التراجع
    '''    ↳ يحفظ عبر:
    '''[DAL - StudentRepository.SaveStudents()]
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>

    '    اذا يحدث هنا من منظور الباك اند؟
    'فصل البيانات عن المنطق: الواجهة تقتصر على جمع المدخلات؛ لا تقوم بمعالجة أو تعديل البيانات. وهذا يساعد في كتابة كود نظيف وقابل للاختبار
    Private Sub Button_Add_Click(sender As Object, e As EventArgs) Handles Button_Add.Click
        '✅ أولًا: جمع البيانات من المستخدم (الـ UI)
        Dim name = TextBox1.Text.Trim() 'في هذه المرحلة، الواجهة تجمع البيانات كنصوص من الحقول.
        Dim address = TextBox3.Text.Trim()

        Dim age As Integer
        Dim year As Integer
        Dim studentClass As Integer
        Dim grade As Double
        '🧪 ثانيًا: التحقق من صلاحية المدخلات
        '        لدالة تتحقق من: الاسم، العمر، العنوان، السنة، الصف، المعدل.
        'تعيد False إذا وجد خطأ واحد، وتمنع الإضافة.
        If Not ValidateStudentInput(name, TextBox2.Text, address, TextBox4.Text, TextBox5.Text, TextBox6.Text, age, year, studentClass, grade) Then
            Return
        End If
        '''🛠 ثالثًا: إنشاء كائن من نوع student2
        '''2. إنشاء كائن الطالب (Student Object)
        '''بعد التأكد من صلاحية المدخلات، يقوم الواجهة بإنشاء كائن جديد من فئة student2
        '''💡 تحليل معمّق:
        '''        المعرفة رقم الطالب (StudentID) تُعطى هنا كـ 0
        '''هذه علامة تخبر طبقة BLL أن الطالب جديد، فيقوم StudentService بتوليد ID تلقائي.
        '''هذه فكرة ممتازة تفصل الواجهة عن المنطق، وتسمى identity delegation.
        '''البقية تؤخذ من المدخلات التي تم التحقق منها
        '''أي خطأ في النوع (مثلاً رقم خاطئ) يتم رفضه مسبقًا.
        '''StudentID = 0: باستخدام القيمة الافتراضية 0 نخبر النظام أن الطالب جديد وبالتالي يجب توليد معرف فريد له في طبقة منطق العمل (BLL).


        Dim newStudent As New student2(0, name, age, address, year, studentClass, grade) ' يتم تمرير الكائن هاذى بحيث يتم تمريرة الى الكائن الاخر _student2 الموجود في الكلاس StudentService
        '''🤝 رابعًا: تمرير الكائن إلى الطبقة الوسطى (BLL)
        '''        من هنا تبدأ رحلة الكائن عبر المنطق ثم التخزين
        '''يُعالج في StudentService (يُعطيه ID، يتحقق من التكرار…)
        '''ثم يُسجل في _students وUndoStack
        '''وأخيرًا يُمرر إلى DAL ليُحفظ على ملف CSV
        studentService.AddStudent(newStudent) ' يتم تمرير البيانات من مربع الادوات الى الدالةAddStudent
        RefreshGrid()
        ClearInputFields()
    End Sub

    Private Sub Button_Update_Click(sender As Object, e As EventArgs) Handles Button_Update.Click
        If StudentsDataGridView.CurrentRow Is Nothing Then Return

        Dim selectedStudent As student2 = CType(StudentsDataGridView.CurrentRow.DataBoundItem, student2)

        Dim updatedName = TextBox1.Text.Trim()
        Dim updatedAddress = TextBox3.Text.Trim()

        Dim updatedAge As Integer
        Dim updatedYear As Integer
        Dim updatedClass As Integer
        Dim updatedGrade As Double

        If Not ValidateStudentInput(updatedName, TextBox2.Text, updatedAddress, TextBox4.Text, TextBox5.Text, TextBox6.Text, updatedAge, updatedYear, updatedClass, updatedGrade) Then
            Return
        End If
        ' تحديث البيانات
        Dim updatedStudent As New student2(selectedStudent.StudentID, updatedName, updatedAge, updatedAddress, updatedYear, updatedClass, updatedGrade)
        studentService.UpdateStudent(updatedStudent)
        RefreshGrid()
    End Sub

    Private Sub Button_Delete_Click(sender As Object, e As EventArgs) Handles Button_Delete.Click
        If StudentsDataGridView.CurrentRow Is Nothing Then Return
        Dim selectedStudent As student2 = CType(StudentsDataGridView.CurrentRow.DataBoundItem, student2)
        studentService.DeleteStudent(selectedStudent.StudentID)
        RefreshGrid()
        ClearInputFields()
    End Sub

    Private Sub Button_Undo_Click(sender As Object, e As EventArgs) Handles Button_Undo.Click
        Dim msg = studentService.UndoLastAction()
        RefreshGrid()
        MessageBox.Show(msg, "تراجع", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub SaveDataButton_Click(sender As Object, e As EventArgs) Handles SaveDataButton.Click
        studentService.SaveAllStudents()
        MessageBox.Show("تم حفظ البيانات بنجاح", "حفظ", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub LoadDataButton_Click(sender As Object, e As EventArgs) Handles LoadDataButton.Click
        students = studentService.GetAllStudents()
        StudentsDataGridView.DataSource = students
    End Sub

    Private Sub StudentsDataGridView_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles StudentsDataGridView.CellClick
        If e.RowIndex >= 0 Then
            Dim selectedStudent As student2 = CType(StudentsDataGridView.Rows(e.RowIndex).DataBoundItem, student2)
            TextBox1.Text = selectedStudent.name
            TextBox2.Text = selectedStudent.age.ToString()
            TextBox3.Text = selectedStudent.address
            TextBox4.Text = selectedStudent.enrollmentYear.ToString()
            TextBox5.Text = selectedStudent.studentClass.ToString()
            TextBox6.Text = selectedStudent.grade.ToString()
        End If
    End Sub
    ' دالة للتحقق من المدخلات
    Private Function ValidateStudentInput(ByVal name As String, ByVal ageText As String, ByVal address As String,
                                          ByVal enrollmentYearText As String, ByVal studentClassText As String,
                                          ByVal gradeText As String, ByRef parsedAge As Integer,
                                          ByRef parsedEnrollmentYear As Integer, ByRef parsedStudentClass As Integer,
                                          ByRef parsedGrade As Double) As Boolean
        If String.IsNullOrWhiteSpace(name) Then
            MessageBox.Show("الرجاء إدخال اسم الطالب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        ' تحقق من العمر والصف الدراسي وبقية المدخلات...
        If Not Integer.TryParse(ageText, parsedAge) OrElse parsedAge < 5 OrElse parsedAge > 25 Then
            MessageBox.Show("العمر يجب أن يكون بين 5 و 25", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If String.IsNullOrWhiteSpace(address) Then
            MessageBox.Show("الرجاء إدخال عنوان الطالب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If Not Integer.TryParse(enrollmentYearText, parsedEnrollmentYear) OrElse parsedEnrollmentYear < 2000 OrElse parsedEnrollmentYear > Date.Now.Year Then
            MessageBox.Show("سنة الالتحاق يجب أن تكون بين 2000 و السنة الحالية", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If Not Integer.TryParse(studentClassText, parsedStudentClass) OrElse parsedStudentClass < 1 OrElse parsedStudentClass > 12 Then
            MessageBox.Show("الصف الدراسي يجب أن يكون بين 1 و 12", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If Not Double.TryParse(gradeText, parsedGrade) OrElse parsedGrade < 0 OrElse parsedGrade > 100 Then
            MessageBox.Show("المعدل يجب أن يكون بين 0 و 100", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    Private Sub ClearInputFields()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
        TextBox1.Focus()
    End Sub
    ' دالة لتحديث البيانات في الـ Grid
    Private Sub RefreshGrid()
        StudentsDataGridView.DataSource = Nothing
        StudentsDataGridView.DataSource = students
    End Sub

    Private Sub Button_ExportToExcel_Click(sender As Object, e As EventArgs) Handles Button_ExportToExcel.Click
        Dim exportPath As String = Path.Combine(Application.StartupPath, "students_export.csv")

        Using writer As New StreamWriter(exportPath, False, System.Text.Encoding.UTF8)
            writer.WriteLine("StudentID,Name,Age,Address,EnrollmentYear,StudentClass,Grade")
            For Each s As student2 In students
                writer.WriteLine($"{s.StudentID},{s.name},{s.age},{s.address},{s.enrollmentYear},{s.studentClass},{s.grade}")
            Next
        End Using

        MessageBox.Show("تم تصدير البيانات بنجاح إلى Excel (CSV).", "تصدير", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ' فتح الملف تلقائيًا (اختياري)
        If File.Exists(exportPath) Then
            Process.Start("explorer.exe", exportPath)
        End If
    End Sub

    Private Sub Button_Redo_Click(sender As Object, e As EventArgs) Handles Button_Redo.Click
        Dim msg = studentService.RedoLastAction()
        StudentsDataGridView.DataSource = Nothing
        StudentsDataGridView.DataSource = studentService.GetAllStudents()
        MessageBox.Show(msg, "إعادة", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim keyword = TextBoxSearch.Text.Trim()
        Dim result As List(Of student2)

        If ComboBoxSearchBy.SelectedItem.ToString() = "الاسم" Then
            result = studentService.SearchByName(keyword)
        ElseIf ComboBoxSearchBy.SelectedItem.ToString() = "الرقم" Then
            Dim id As Integer
            If Integer.TryParse(keyword, id) Then
                Dim student = studentService.SearchByID(id)
                result = If(student IsNot Nothing, New List(Of student2) From {student}, New List(Of student2))
            Else
                result = New List(Of student2)
            End If
        Else
            result = studentService.GetAllStudents().ToList()
        End If

        StudentsDataGridView.DataSource = result
    End Sub

    Private Sub ButtonSort_Click(sender As Object, e As EventArgs) Handles ButtonSort.Click
        Dim field = ComboBoxSortBy.SelectedItem.ToString()
        Dim asc = CheckBoxSortAsc.Checked
        Dim sorted = studentService.SortByField(field, asc)
        StudentsDataGridView.DataSource = sorted
    End Sub

    Private Sub ButtonUpdateQueue_Click(sender As Object, e As EventArgs) Handles ButtonUpdateQueue.Click
        Dim queue = studentService.PeekQueue()
        ListBoxQueue.Items.Clear()
        For Each s In queue
            ListBoxQueue.Items.Add($"{s.StudentID} - {s.name}")
        Next
    End Sub

    Private Sub Button_Enqueue_Click(sender As Object, e As EventArgs) Handles Button_Enqueue.Click
        ' جمع بيانات الطالب من TextBoxes
        Dim student As New student2 With {
            .name = TextBox1.Text.Trim(),
            .age = CInt(TextBox2.Text),
            .address = TextBox3.Text.Trim(),
            .enrollmentYear = CInt(TextBox4.Text),
            .studentClass = CInt(TextBox5.Text),
            .grade = Double.Parse(TextBox6.Text)
        }

        studentService.EnqueueStudent(student)

        MessageBox.Show("تمت إضافة الطالب إلى قائمة الانتظار.")

        ' 🔁 تحديث القائمة تلقائيًا
        ButtonUpdateQueue.PerformClick()
    End Sub

    Private Sub Button_Dequeue_Click(sender As Object, e As EventArgs) Handles Button_Dequeue.Click
        Dim registered = studentService.DequeueStudent()
        If registered IsNot Nothing Then
            StudentsDataGridView.DataSource = Nothing
            StudentsDataGridView.DataSource = studentService.GetAllStudents()
            MessageBox.Show($"تم تسجيل الطالب: {registered.name}")
        Else
            MessageBox.Show("لا يوجد طلاب في قائمة الانتظار.")
        End If

        ' 🔁 تحديث القائمة تلقائيًا
        ButtonUpdateQueue.PerformClick()
    End Sub

End Class
