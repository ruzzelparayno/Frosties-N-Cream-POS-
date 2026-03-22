Imports System.IO
Imports MySql.Data.MySqlClient

Public Class ProductContent
    Public Shared Instance As ProductContent

    ' Event for notifying POS of new product
    Public Event ProductAdded(productId As String)

    ' Event for notifying realtime stock changes
    Public Event ProductStockChanged(productId As String, newStock As Integer)

    Private connectionString As String = "server=localhost;userid=root;password=;database=pos"
    Private conn As New MySqlConnection(connectionString)

    ' ProductID must be STRING
    Private selectedProductID As String = ""
    Private selectedImagePath As String = ""

    ' =========================
    ' Constructor
    ' =========================
    Public Sub New()
        InitializeComponent()
        Instance = Me
    End Sub

    ' =========================================
    '  GET PREFIX BASED ON CATEGORY
    ' =========================================
    Private Function GetCategoryPrefix(categoryName As String) As String
        Dim words = categoryName.Split(" "c)

        If words.Length = 1 Then
            Return words(0).Substring(0, Math.Min(2, words(0).Length)).ToUpper()
        Else
            Return (words(0)(0) & words(1)(0)).ToUpper()
        End If
    End Function

    ' =========================================
    '  GENERATE PRODUCT ID
    ' =========================================
    Private Function GenerateProductID(categoryName As String) As String
        Dim prefix As String = GetCategoryPrefix(categoryName)
        Dim lastNum As Integer = 0

        Try
            conn.Open()

            Dim query As String =
                "SELECT ProductID FROM products 
                 WHERE ProductID LIKE @pre 
                 ORDER BY CAST(SUBSTRING_INDEX(ProductID, '-', -1) AS UNSIGNED) DESC 
                 LIMIT 1"

            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@pre", prefix & "-%")

            Dim result = cmd.ExecuteScalar()

            If result IsNot Nothing Then
                Dim getID As String = result.ToString()

                Dim parts = getID.Split("-"c)
                Dim numPart As String = parts(parts.Length - 1)

                If Integer.TryParse(numPart, lastNum) = False Then
                    lastNum = 0
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("ID Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

        lastNum += 1
        Return $"{prefix}-{lastNum.ToString("000")}"
    End Function

    ' =========================================
    '  LOAD CATEGORIES
    ' =========================================
    Public Sub LoadCategories()
        Try
            conn.Open()
            Dim cmd = New MySqlCommand("SELECT CategoryName FROM categories", conn)
            Dim r = cmd.ExecuteReader()

            cb_cate.Items.Clear()
            While r.Read()
                cb_cate.Items.Add(r("CategoryName").ToString())
            End While

            r.Close()
        Catch ex As Exception
            MessageBox.Show("Load Categories Error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' =========================================
    '  SUBSCRIBE TO CATEGORYFORM
    ' =========================================
    Public Sub SubscribeToCategoryForm(cf As CategoryForm)
        AddHandler cf.CategoryAdded, AddressOf OnCategoryAdded
    End Sub

    ' =========================================
    '  HANDLER FOR NEW CATEGORY
    ' =========================================
    Private Sub OnCategoryAdded(categoryName As String)
        If Not cb_cate.Items.Contains(categoryName) Then
            cb_cate.Items.Add(categoryName)
        End If
    End Sub

    ' =========================================
    '  LOAD INVENTORY
    ' =========================================
    Public Sub LoadInventory()
        Try
            Using c As New MySqlConnection(connectionString)
                c.Open()

                Dim da As New MySqlDataAdapter(
                "SELECT ProductID, ProductName, Price, StockQuantity, CategoryName 
                 FROM products 
                 ORDER BY ProductName ASC", c)

                Dim dt As New DataTable()
                da.Fill(dt)
                Guna2DataGridView1.DataSource = dt
            End Using

        Catch ex As Exception
            MessageBox.Show("Load Inventory Error: " & ex.Message)
        End Try
    End Sub

    Private Sub ProductContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCategories()
        LoadInventory()
        cb_cate.DropDownStyle = ComboBoxStyle.DropDownList
    End Sub

    ' =========================================
    '  CLICK ROW – LOAD DATA
    ' =========================================
    Private Sub Guna2DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then Exit Sub

        Dim row = Guna2DataGridView1.Rows(e.RowIndex)

        selectedProductID = row.Cells("ProductID").Value.ToString()
        SiticoneTextBox1.Text = row.Cells("ProductName").Value.ToString()
        SiticoneTextBox2.Text = row.Cells("StockQuantity").Value.ToString()
        SiticoneTextBox3.Text = row.Cells("Price").Value.ToString()
        cb_cate.SelectedItem = row.Cells("CategoryName").Value.ToString()

        Try
            conn.Open()
            Dim cmd As New MySqlCommand("SELECT ProductImage FROM products WHERE ProductID=@id", conn)
            cmd.Parameters.AddWithValue("@id", selectedProductID)

            Dim imgData = CType(cmd.ExecuteScalar(), Byte())

            If imgData IsNot Nothing Then
                Using ms As New MemoryStream(imgData)
                    pb_ci.Image = Image.FromStream(ms)
                End Using
            Else
                pb_ci.Image = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show("Image Load Error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' =========================================
    '  SELECT IMAGE
    ' =========================================
    Private Sub SiticoneButton2_Click(sender As Object, e As EventArgs) Handles SiticoneButton2.Click
        If selectedProductID <> "" Then
            MessageBox.Show("You are editing an existing product. Click NEW to add a new one.")
            Exit Sub
        End If

        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp"

        If ofd.ShowDialog() = DialogResult.OK Then
            selectedImagePath = ofd.FileName
            pb_ci.Image = Image.FromFile(selectedImagePath)
        End If
    End Sub

    ' =========================================
    '  SAVE NEW PRODUCT
    ' =========================================
    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click

        If SiticoneTextBox1.Text = "" Or SiticoneTextBox2.Text = "" Or
           SiticoneTextBox3.Text = "" Or cb_cate.SelectedIndex = -1 Or
           selectedImagePath = "" Then

            MessageBox.Show("Please complete all fields.")
            Exit Sub
        End If

        Dim newID = GenerateProductID(cb_cate.SelectedItem.ToString())

        Try
            conn.Open()

            Dim sql As String =
                "INSERT INTO products 
                (ProductID, ProductName, StockQuantity, Price, CategoryName, ProductImage)
                VALUES (@id, @name, @stock, @price, @cat, @img)"

            Dim cmd As New MySqlCommand(sql, conn)

            cmd.Parameters.AddWithValue("@id", newID)
            cmd.Parameters.AddWithValue("@name", SiticoneTextBox1.Text)
            cmd.Parameters.AddWithValue("@stock", SiticoneTextBox2.Text)
            cmd.Parameters.AddWithValue("@price", SiticoneTextBox3.Text)
            cmd.Parameters.AddWithValue("@cat", cb_cate.SelectedItem.ToString())
            cmd.Parameters.AddWithValue("@img", File.ReadAllBytes(selectedImagePath))

            cmd.ExecuteNonQuery()

            MessageBox.Show("Product Added! ID: " & newID)

            ' ✅ Raise event to notify POS
            RaiseEvent ProductAdded(newID)

        Catch ex As Exception
            MessageBox.Show("Save Error: " & ex.Message)
        Finally
            conn.Close()
            LoadInventory()
            ResetFields()
        End Try
    End Sub

    Private Sub ResetFields()
        SiticoneTextBox1.Clear()
        SiticoneTextBox2.Clear()
        SiticoneTextBox3.Clear()
        cb_cate.SelectedIndex = -1
        pb_ci.Image = Nothing
        selectedProductID = ""
        selectedImagePath = ""
    End Sub

    ' =========================================
    '  DOUBLE CLICK → OPEN EDIT FORM
    ' =========================================
    Private Sub Guna2DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub

        Dim row = Guna2DataGridView1.Rows(e.RowIndex)

        Dim id = row.Cells("ProductID").Value.ToString()
        Dim name = row.Cells("ProductName").Value.ToString()
        Dim qty = row.Cells("StockQuantity").Value.ToString()
        Dim price = row.Cells("Price").Value.ToString()
        Dim category = row.Cells("CategoryName").Value.ToString()

        Dim productImage As Image = Nothing

        Try
            conn.Open()
            Dim cmd As New MySqlCommand("SELECT ProductImage FROM products WHERE ProductID=@id", conn)
            cmd.Parameters.AddWithValue("@id", id)

            Dim imgData = CType(cmd.ExecuteScalar(), Byte())

            If imgData IsNot Nothing Then
                Using ms As New MemoryStream(imgData)
                    productImage = Image.FromStream(ms)
                End Using
            End If

        Catch ex As Exception
            MessageBox.Show("Image Load Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

        Edit_ProductForm.LoadProductData(id, name, category, qty, price, productImage)
        Edit_ProductForm.Show()
    End Sub

    ' ===========================
    '  NEW: Update stock directly (if calling code already knows newStock)
    ' ===========================
    Public Sub UpdateStockRealtime(productId As String, newStock As Integer)
        Try
            ' Raise event so UI subscribers can react
            RaiseEvent ProductStockChanged(productId, newStock)
        Catch ex As Exception
            ' Swallow to avoid affecting calling transaction
        End Try

        ' Refresh list to reflect any other changes
        LoadInventory()
    End Sub

    ' ===========================
    ' NEW: If calling code only knows ProductName (your Charge form reduces by ProductName),
    '       call this method to fetch new stock and then raise the event.
    ' ===========================
    Public Sub NotifyStockChangedByName(productName As String)
        Try
            Using c As New MySqlConnection(connectionString)
                c.Open()
                Dim cmd As New MySqlCommand("SELECT ProductID, StockQuantity FROM products WHERE ProductName = @pname LIMIT 1", c)
                cmd.Parameters.AddWithValue("@pname", productName)
                Using r As MySqlDataReader = cmd.ExecuteReader()
                    If r.Read() Then
                        Dim pid As String = r("ProductID").ToString()
                        Dim sq As Integer = 0
                        If Integer.TryParse(r("StockQuantity").ToString(), sq) = False Then
                            sq = 0
                        End If

                        ' Raise event with latest values
                        RaiseEvent ProductStockChanged(pid, sq)
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' ignore or log
        Finally
            ' ensure grid refresh
            LoadInventory()
        End Try
    End Sub

    ' ===========================
    ' NEW: Local handler that applies the stock change to the grid immediately
    ' ===========================
    Private Sub ProductContent_ProductStockChanged(productId As String, newStock As Integer) Handles Me.ProductStockChanged
        Try
            ' Update the row in the current DataGridView if present
            For Each row As DataGridViewRow In Guna2DataGridView1.Rows
                If row.IsNewRow Then Continue For
                Try
                    Dim cellVal = If(row.Cells("ProductID").Value, "")
                    If cellVal IsNot Nothing AndAlso cellVal.ToString() = productId Then
                        ' Update cell value (StockQuantity)
                        If row.Cells("StockQuantity") IsNot Nothing Then
                            row.Cells("StockQuantity").Value = newStock
                        End If
                        Exit For
                    End If
                Catch ex As Exception
                    ' ignore row-level errors
                End Try
            Next
        Catch ex As Exception
            ' ignore
        Finally
            ' also reload to be safe
            LoadInventory()
        End Try
    End Sub

End Class
