Imports LAERN_USE.BLL.Controller
Imports LAERN_USE.BLL.Enums


Public Class Form1
    ' شكل تدفق البيانات بيتم على اساس 
    'والكنترول بيكلم controller الواجهه تكلم 
    ' الباقي في ملف نصي 
    '
    Private _controller As StudentController

    '    '🔹 الخطوة 3: ربط القراءة مع الفورم عند التشغيل
    '    ' 🧠 عند تشغيل الفورم، يتم تحميل إعدادات التخزين + ضبط القائمة
    ''' <summary>
    ''' 🔧 برمجياً:
    '''يتم ملء القائمة ComboBox
    '''ثم استدعاء LoadStorageSetting()
    '''ثم بناء StudentController حسب التخزين المختار
    '''🧠 خوارزمياً:
    '''واجهة المستخدم تنطلق ← تتحقق من إعدادات التخزين ← تتصرف بناءً عليه ← تبني النظام ديناميكياً
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'عند تشغيل البرنامج يحدث Form1_Load → LoadStorageSetting() → comboStorage.SelectedIndex → InitializeController()ي
        ComboStorage.Items.Add("CSV")
        ComboStorage.Items.Add("Legacy")
        ComboStorage.Items.Add("xml")
        Dim mode = LoadStorageSetting()
        ComboStorage.SelectedIndex = If(mode = StorageMode.Csv, 0, 1)
        InitializeController()
    End Sub
    '    ' 🧠 عندما يغيّر المستخدم الاختيار، يتم حفظه + إعادة بناء الكنترولر
    Private Sub ComboStorage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboStorage.SelectedIndexChanged
        Dim selectedStorage = ComboStorage.SelectedItem.ToString()
        Dim mode As StorageMode = If(selectedStorage = "CSV", StorageMode.Csv, StorageMode.Legacy) ' يعيد Legacy أو CSV
        InitializeController()
    End Sub
    ' 🧠 إنشاء الكنترولر بناءً على نوع التخزين المختار
    ''' <summary>
    '''      توضيح مهم لوظيفت الدالة
    ''' ✔️ هذه العملية تسمى "إعادة بناء الكائن Controller"
    '''↪️ لأنك تنشئ StudentController جديد
    '''بناءً على خيار المستخدم
    '''↪️ ويتم داخليًا حقن نوع التخزين المناسب عبر
    '''StudentServiceFactory
    '''
    ''' 🎯 الهدف من
    ''' 🎯 الهدف من "إعادة بناء" هو أن يتعامل النظام مع اختيار جديد من ComboBox دون الحاجة لتعديل الكائنات القديمة — وهذه سمة ممتازة لبرامج تعتمد على الفصل والحقن.

    'مــــلاحظة مهمة عامة
    '    ✅ 1. تعريف السياق (Context) ← تحديد نوع التنفيذ
    '📌 الفكرة: تحديد نوع التخزين الذي سيحدد أي كائن سيتم إنشاؤه
    '        المستخدم يختار التخزين عبر ComboBox
    'القيمة تُحوّل إلى سياق (Context) ← وهو StorageMode
    Private Sub InitializeController() 'تهيئة وحدة التحكم
        ' هنا الخطوة الاولى لااستخدام Adapter
        '''        🔎 معنى هذا
        '''المستخدم اختار نوع التخزين
        '''يتم إنشاء
        '''StudentController
        '''بناءً على ذلك

        '🧠 ملخص التدفق في Adapter:
        '''Form1 → Factory → StudentService → LegacyStorageAdapter → LegacyStorage


        ' هنا الخطوة الاولى لااستخدام. Bridge Pattern ⇄ DIP + OCP
        '''     🔎 ماذا يحدث؟
        '''        المستخدم اختار نوع التخزين (CSV / Legacy)
        '''يتم إنشاء StudentController مع حقن التنفيذ المطلوب
        '''
        '🧠 ملخص التدفق في Bridge:
        '''Form1 → StudentController → StudentService ← (Bridge)
        '''                                    ↑
        '''                    [Csv / Legacy] IStudentRepository
        Dim mode = If(ComboStorage.SelectedIndex = 0, StorageMode.Csv, StorageMode.Legacy)
        _controller = New StudentController(mode)
    End Sub

    '🔹 الخطوة 1: إنشاء الملف تلقائيًا عند اختيار التخزين
    ' 🧠 حفظ التخزين إلى ملف Settings.txt
    ''' <summary>
    ''' 🔧 برمجياً:
    '''الدالة تحفظ سطرًا مثل "StorageMode=CSV" في الملف النصي
    '''الملف يُنشأ تلقائيًا بجانب ملف .exe وقت التشغيل
    '''🧠 خوارزمياً:
    '''عند تغيير ComboBox، يتم تنفيذ الدالة لحفظ القيمة الحالية
    ''' </summary>
    ''' <param name="mode">هو نوع التخزين الي تم اختيارة في ComboBox Enum</param>
    Private Sub SaveStorageSetting(mode As StorageMode)
        IO.File.WriteAllText("Settings.txt", $"StorageMode={mode}")
    End Sub

    '🔹 الخطوة 2: قراءة الإعداد تلقائيًا عند بدء التشغيل
    ' 🧠 تحميل التخزين من الملف عند بدء التشغيل
    ''' <summary>
    ''' 🔧 برمجياً:
    '''الدالة تبحث عن الملف وتقرأ السطر
    '''تستخرج القيمة وتحوّلها إلى نوع StorageMode
    '''🧠 خوارزمياً:
    '''تقرأ الإعداد ← تقرر نوع التخزين ← تستدعي Factory ← تُحقن في StudentController
    ''' </summary>
    Private Function LoadStorageSetting() As StorageMode
        If IO.File.Exists("Settings.txt") Then
            Dim value = IO.File.ReadAllText("Settings.txt").Replace("StorageMode=", "").Trim()
            If value = "CSV" Then Return StorageMode.Csv
            If value = "Legacy" Then Return StorageMode.Legacy
        End If
        Return StorageMode.Csv ' ← الوضع الافتراضي إذا لم يوجد ملف
    End Function






    'Private ReadOnly _controller As StudentController = New StudentController()
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim id As Integer = Integer.Parse(txtId.Text)
        Dim name As String = txtNAME.Text
        Dim age As Integer = Integer.Parse(txtAGE.Text)
        '''✅ هذا يحترم مبدأ LSP لأن:
        '''↪️ هذا السطر يُرسل البيانات من Form1 إلى 
        '''StudentController ↪️ ثم يتم إنشاء الكائن
        '''STUDENT4 داخل
        '''Controller وليس في الفورم مباشرة
        '''لكائن STUDENT4 يحتوي بياناته داخليًا
        ''' ويُرسل كوحدة موحّدة للتعامل
        ''' دون الحاجة لتمرير كل خاصية على حدة داخل الوظائف

        _controller.AddStudent(id, name, age)
        MessageBox.Show("تمت إضافة الطالب!")
    End Sub



    Private Sub BtnLoadData_Click(sender As Object, e As EventArgs) Handles BtnLoadData.Click
        Dim students = _controller.GetAllStudents()
        DataGridView1.DataSource = students
    End Sub

End Class
