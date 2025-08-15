' DAL/StudentRepository.vb - تعديل لتوليد الملف تلقائيًا إذا لم يكن موجود
Imports System.IO
Imports Complete_program.Models
Imports Models

Namespace DAL

    Public Class StudentRepository
        Private ReadOnly filePath As String = "students.csv"
        ''' <summary>
        ''' هذه الدالة هي الوحيدة التي تعرف تفاصيل كيفية قراءة البيانات من ملف students.csv.
        '''تقوم بقراءة كل سطر، وتحويله إلى كائن student2، ثم تضيفه إلى BindingList(Of student2) محلية.
        '''ترجع هذه الـ BindingList الممتلئة إلى StudentService
        ''' </summary>
        ''' <returns></returns>
        Public Function LoadStudents() As List(Of student2) 'ذه الدالة هي الوحيدة التي تعرف تفاصيل كيفية قراءة البيانات من ملف students.csv
            Dim students As New List(Of student2)()

            If Not File.Exists(filePath) Then
                File.WriteAllText(filePath, "StudentID,Name,Age,Address,EnrollmentYear,StudentClass,Grade" & Environment.NewLine) 'هناكذالك اذى لم يكون الملفstudent.cscموجودا يتم كتابتة مع سطر رؤؤس الاعمدة
                Return students
            End If

            Using reader As New StreamReader(filePath)
                reader.ReadLine()
                While Not reader.EndOfStream
                    Dim line = reader.ReadLine()
                    Dim fields = line.Split(","c)

                    If fields.Length = 7 Then
                        Try
                            Dim s As New student2(
                                Integer.Parse(fields(0)),
                                fields(1),
                                Integer.Parse(fields(2)),
                                fields(3),
                                Integer.Parse(fields(4)),
                                Integer.Parse(fields(5)),
                                Double.Parse(fields(6))
                            )
                            students.Add(s)
                        Catch
                        End Try
                    End If
                End While
            End Using

            Return students
        End Function

        Public Sub SaveStudents(students As List(Of student2))
            Using writer As New StreamWriter(filePath, False)
                writer.WriteLine("StudentID,Name,Age,Address,EnrollmentYear,StudentClass,Grade") 'بالإضافة إلى ذلك، يتم كتابة أسماء الأعمدة أيضًا في دالة في كل مرة يتم فيها حفظ البيانات لكي نضمن انة ملف csvمنظما
                For Each s In students
                    writer.WriteLine($"{s.StudentID},{s.name},{s.age},{s.address},{s.enrollmentYear},{s.studentClass},{s.grade}")
                Next
            End Using
        End Sub
    End Class
End Namespace
