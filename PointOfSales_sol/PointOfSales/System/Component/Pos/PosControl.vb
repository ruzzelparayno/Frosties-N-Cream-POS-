Imports System.IO
Imports Guna.UI2.WinForms
Imports System.Data.SQLite
Imports System.Text.Json

Public Class PosControl
    Public Shared Instance As PosControl
    Dim conn As New SQLiteConnection("Data Source=pos.db;Version=3;")
    Private connectionString As String = "Data Source=pos.db;Version=3;"

    ' -----------------------------
    ' For ticket persistence
    ' -----------------------------
    Private TicketFile As String = Path.Combine(Application.StartupPath, "last_ticket.json")
    Public Shared CurrentTicket As Integer = 1
    Private LastTicketDate As Date = DateTime.Now.Date

    Private Class TicketData
        Public Property LastTicket As Integer
        Public Property TicketDate As Date
    End Class

    Private Sub PosControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Instance = Me
        lbl_vat.Text = "₱0.00"
        lbl_subtotal.Text = "₱0.00"
        lbl_total.Text = "₱0.00"

        SetupDataGrid()
        LoadProducts()
        cb_cate1.DropDownStyle = ComboBoxStyle.DropDownList
        LoadCategories()

        ' Load last ticket
        LoadLastTicketFromFile()
        UpdateTicket()

        ' Subscribe to shared events
        AddHandler ProductContent.ProductAddedShared, AddressOf OnProductAdded
        AddHandler ProductContent.ProductStockChangedShared, AddressOf OnProductStockChanged
        AddHandler CategoryForm.CategoryAddedShared, AddressOf OnCategoryAdded

    End Sub
    Private Sub OnCategoryAdded(categoryName As String)
        ' Ensure UI thread safety
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String)(AddressOf OnCategoryAdded), categoryName)
            Return
        End If

        ' Add new category to cb_cate1 if not already present
        If Not cb_cate1.Items.Contains(categoryName) Then
            cb_cate1.Items.Add(categoryName)
        End If

        ' Select the new category
        cb_cate1.SelectedItem = categoryName
        ' Reload products to show new category products (if any)
        FilterProductsByCategory()
    End Sub


    Private Sub OnProductStockChanged(productId As String, newStock As Integer)
        ' Update FlowLayoutPanel1 product card if needed
        For Each ctrl As Control In FlowLayoutPanel1.Controls
            If TypeOf ctrl Is Panel Then
                Dim card As Panel = CType(ctrl, Panel)
                If card.Tag IsNot Nothing AndAlso card.Tag.ToString() = productId Then
                    ' Optional: update stock display inside card if you have it
                    ' e.g., a hidden Label showing stock
                End If
            End If
        Next

        ' Also refresh ticket grid in case it is in cart
        CalculateTotals()
    End Sub

    ' ===============================
    '  TICKET PERSISTENCE METHODS
    ' ===============================
    Private Function GetLastTicketFromDB() As Integer
        Try
            Using c As New SQLiteConnection(connectionString)
                c.Open()
                Dim cmd As New SQLiteCommand("SELECT TicketNumber FROM sales ORDER BY SaleDate DESC LIMIT 1", c)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Dim lastTicketStr As String = result.ToString().TrimStart("#"c)
                    Dim ticketNum As Integer
                    If Integer.TryParse(lastTicketStr, ticketNum) Then
                        Return ticketNum
                    End If
                End If
            End Using
        Catch ex As Exception
            ' ignore
        End Try
        Return 0
    End Function

    Public Sub LoadLastTicketFromFile()
        Try
            Dim lastTicketInDB As Integer = GetLastTicketFromDB()

            If File.Exists(TicketFile) Then
                Dim json As String = File.ReadAllText(TicketFile)
                Dim obj = JsonSerializer.Deserialize(Of TicketData)(json)
                Dim fileTicket As Integer = If(obj?.LastTicket, 0)

                Dim maxTicket As Integer = Math.Max(lastTicketInDB, fileTicket)
                If obj IsNot Nothing AndAlso obj.TicketDate = DateTime.Now.Date Then
                    CurrentTicket = maxTicket + 1
                Else
                    CurrentTicket = maxTicket + 1
                End If
            Else
                CurrentTicket = lastTicketInDB + 1
            End If
        Catch ex As Exception
            CurrentTicket = 1
        End Try
    End Sub
    Public Sub RefreshAllData()
        LoadCategories()
        LoadProducts()
        SetupDataGrid()
    End Sub


    Private Sub SaveCurrentTicketToFile()
        Try
            Dim obj As New TicketData With {
                .LastTicket = CurrentTicket,
                .TicketDate = DateTime.Now.Date
            }
            Dim json As String = JsonSerializer.Serialize(obj)
            File.WriteAllText(TicketFile, json)
        Catch ex As Exception
            ' ignore
        End Try
    End Sub

    Public Sub NextTicket()
        ' Save ticket before incrementing to keep file in sync
        SaveCurrentTicketToFile()
        CurrentTicket += 1
    End Sub

    Public Shared Function GetFormattedTicket() As String
        Return "#" & CurrentTicket.ToString("D3")
    End Function

    Public Sub UpdateTicket()
        lbl_tickets.Text = GetFormattedTicket()
    End Sub
    ' ===============================
    '  DATA GRID & PRODUCTS
    ' ===============================
    Private Sub SetupDataGrid()
        Guna2DataGridView1.Rows.Clear()
        Guna2DataGridView1.Columns.Clear()

        With Guna2DataGridView1
            .Columns.Add("Quantity", "Qty")
            .Columns.Add("ProductName", "Product Name")
            .Columns.Add("Price", "Price (₱)")
        End With

        If Guna2DataGridView1.Columns.Contains("ProductName") Then
            Guna2DataGridView1.Columns("ProductName").Width = 80
        End If
        If Guna2DataGridView1.Columns.Contains("Quantity") Then
            Guna2DataGridView1.Columns("Quantity").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        End If
        If Guna2DataGridView1.Columns.Contains("Price") Then
            Guna2DataGridView1.Columns("Price").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        End If
    End Sub

    Private Sub LoadCategories()
        Try
            conn.Open()
            Dim query As String = "SELECT CategoryName FROM categories"
            Dim cmd As New SQLiteCommand(query, conn)
            Dim reader As SQLiteDataReader = cmd.ExecuteReader()

            cb_cate1.Items.Clear()

            ' ✅ Add All Products
            cb_cate1.Items.Add("All Products")

            While reader.Read()
                cb_cate1.Items.Add(reader("CategoryName").ToString())
            End While

            reader.Close()

            ' ✅ Select All Products by default
            cb_cate1.SelectedIndex = 0

        Catch ex As Exception
            MessageBox.Show("Error loading categories: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub


    Public Sub LoadProducts()
        Try
            FlowLayoutPanel1.Controls.Clear()

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT ProductID, ProductName, ProductImage, Price FROM products"

                Using cmd As New SQLiteCommand(query, conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim productId As String = reader("ProductID").ToString()
                            Dim pname As String = reader("ProductName").ToString()
                            Dim price As Decimal = Convert.ToDecimal(reader("Price"))
                            Dim productImage As Image = Nothing

                            If Not IsDBNull(reader("ProductImage")) Then
                                Dim imgBytes As Byte() = DirectCast(reader("ProductImage"), Byte())
                                Using ms As New MemoryStream(imgBytes)
                                    productImage = Image.FromStream(ms)
                                End Using
                            End If

                            AddProductCard(productId, pname, productImage, price)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading products: " & ex.Message)
        End Try
    End Sub

    Private Sub AddProductCard(productId As String, productName As String, productImage As Image, Optional productPrice As Decimal = 0D)
        Dim card As New SiticonePanel()
        card.Size = New Size(230, 100)
        card.FillColor = Color.FromArgb(89, 70, 141)
        card.BorderStyle = BorderStyle.None
        card.Margin = New Padding(10)
        card.Tag = productId
        card.Cursor = Cursors.Hand
        card.CornerRadiusBottomLeft = 8
        card.CornerRadiusBottomRight = 8
        card.CornerRadiusTopLeft = 8
        card.CornerRadiusTopRight = 8
        card.ShowBorder = False

        Dim pb As New PictureBox()
        pb.Size = New Size(60, 70)
        pb.Location = New Point(20, 15)
        pb.SizeMode = PictureBoxSizeMode.StretchImage
        pb.Image = If(productImage, SystemIcons.Question.ToBitmap())
        pb.Cursor = Cursors.Hand

        Dim lbl As New Label()
        lbl.Text = productName
        lbl.ForeColor = Color.White
        lbl.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lbl.AutoSize = False
        lbl.Size = New Size(120, 50)
        lbl.Location = New Point(90, 15)
        lbl.Cursor = Cursors.Hand

        Dim lblPrice As New Label()
        lblPrice.Text = "₱" & productPrice.ToString("N2")
        lblPrice.ForeColor = Color.White
        lblPrice.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        lblPrice.AutoSize = True
        lblPrice.Location = New Point(90, 60)
        lblPrice.Cursor = Cursors.Hand

        card.Controls.Add(pb)
        card.Controls.Add(lbl)
        card.Controls.Add(lblPrice)

        Dim clickHandler = Sub(sender As Object, e As EventArgs)
                               AddOrUpdateTicket(productName, productPrice, pb.Image)
                           End Sub

        AddHandler card.Click, clickHandler
        AddHandler pb.Click, clickHandler
        AddHandler lbl.Click, clickHandler
        AddHandler lblPrice.Click, clickHandler

        FlowLayoutPanel1.Controls.Add(card)
    End Sub

    Private Sub AddOrUpdateTicket(productName As String, productPrice As Decimal, Optional productImage As Image = Nothing)
        Try
            Dim stockNow As Integer = GetStockQuantity(productName)
            If stockNow <= 0 Then
                MessageBox.Show("No stock available for this product.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim currentQty As Integer = 1
            Dim foundRow As DataGridViewRow = Nothing

            For Each row As DataGridViewRow In Guna2DataGridView1.Rows
                If row.Cells("ProductName").Value IsNot Nothing AndAlso
               String.Equals(row.Cells("ProductName").Value.ToString(), productName, StringComparison.OrdinalIgnoreCase) Then
                    foundRow = row
                    currentQty = Convert.ToInt32(row.Cells("Quantity").Value)
                    Exit For
                End If
            Next

            Dim editForm As New Edit()
            editForm.SelectedProductName = productName
            editForm.SelectedProductPrice = productPrice
            editForm.SelectedProductImage = productImage

            SiticoneOverlay1.Show = True
            editForm.ParentPOS = Me
            editForm.ShowDialog()
            CalculateTotals()

            If editForm.AllowCloseOverlay Then
                SiticoneOverlay1.Show = False
            End If

        Catch ex As Exception
            MessageBox.Show("Error adding product: " & ex.Message)
        End Try
    End Sub

    ' ===============================
    '  RESET & CALCULATIONS
    ' ===============================
    Public Sub ResetOrder()
        RestoreStockFromCart()
        Guna2DataGridView1.Rows.Clear()
        lbl_vat.Text = "₱0.00"
        lbl_subtotal.Text = "₱0.00"
        lbl_total.Text = "₱0.00"
    End Sub

    Public Sub CalculateTotals()
        Dim total As Decimal = 0D

        For Each row As DataGridViewRow In Guna2DataGridView1.Rows
            If row.Cells("Price").Value IsNot Nothing Then
                total += Convert.ToDecimal(row.Cells("Price").Value)
            End If
        Next

        Dim vatRate As Decimal = 0.12D
        Dim vat As Decimal = Math.Round(total * vatRate, 2)
        Dim subtotal As Decimal = Math.Round(total - vat, 2)

        lbl_subtotal.Text = "₱" & subtotal.ToString("N2")
        lbl_vat.Text = "₱" & vat.ToString("N2")
        lbl_total.Text = "₱" & total.ToString("N2")
    End Sub

    Public Sub RestoreStockFromCart()
        Try
            For Each row As DataGridViewRow In Guna2DataGridView1.Rows
                If row.Cells("ProductName").Value IsNot Nothing Then
                    Dim pname As String = row.Cells("ProductName").Value.ToString()
                    Dim qty As Integer = Convert.ToInt32(row.Cells("Quantity").Value)
                    ChangeStock(pname, qty)
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("RestoreStockFromCart error: " & ex.Message)
        End Try
    End Sub

    Public Function GetStockQuantity(productName As String) As Integer
        Dim stock As Integer = 0
        Try
            Using c As New SQLiteConnection(connectionString)
                c.Open()
                Dim q As String = "SELECT StockQuantity FROM products WHERE ProductName = @ProductName LIMIT 1"
                Using cmd As New SQLiteCommand(q, c)
                    cmd.Parameters.AddWithValue("@ProductName", productName)
                    Dim res = cmd.ExecuteScalar()
                    If res IsNot Nothing Then stock = Convert.ToInt32(res)
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking stock: " & ex.Message)
        End Try
        Return stock
    End Function

    Public Function ChangeStock(productName As String, delta As Integer) As Boolean
        Try
            Using c As New SQLiteConnection(connectionString)
                c.Open()
                Dim q As String = "UPDATE products SET StockQuantity = StockQuantity + @Delta WHERE ProductName = @ProductName"
                Using cmd As New SQLiteCommand(q, c)
                    cmd.Parameters.AddWithValue("@Delta", delta)
                    cmd.Parameters.AddWithValue("@ProductName", productName)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            MessageBox.Show("Error updating stock: " & ex.Message)
            Return False
        End Try
    End Function

    Public Sub ClearTransaction()
        ResetOrder()
        NextTicket()
        UpdateTicket()
    End Sub

    ' ===============================
    '  BUTTON & CATEGORY EVENTS
    ' ===============================
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim chargeForm As New Charge()
        chargeForm.ShowDialog()

        Dim searchText As String = SiticoneButtonTextbox1.Text.Trim().ToLower()

        For Each ctrl As Control In FlowLayoutPanel1.Controls
            If TypeOf ctrl Is Panel Then
                Dim card As Panel = CType(ctrl, Panel)
                Dim lblName As Label = CType(card.Controls(1), Label)

                If lblName.Text.ToLower().StartsWith(searchText) Then
                    card.Visible = True
                Else
                    card.Visible = False
                End If
            End If
        Next
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        If Guna2DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = Guna2DataGridView1.SelectedRows(0)
            Dim pname As String = row.Cells("ProductName").Value.ToString()
            Dim qty As Integer = Convert.ToInt32(row.Cells("Quantity").Value)

            ChangeStock(pname, qty)
            Guna2DataGridView1.Rows.Remove(row)
            CalculateTotals()
        Else
            MessageBox.Show("Please select a product to remove.", "Remove Item", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to clear all items?", "Clear All", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            RestoreStockFromCart()
            Guna2DataGridView1.Rows.Clear()
            lbl_subtotal.Text = "₱0.00"
            lbl_vat.Text = "₱0.00"
            lbl_total.Text = "₱0.00"
            MessageBox.Show("All items have been cleared.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub cb_cate1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cb_cate1.SelectedIndexChanged
        FilterProductsByCategory()
    End Sub

    Private Sub FilterProductsByCategory()
        If cb_cate1.SelectedIndex = -1 Then Exit Sub

        Dim selectedCategory As String = cb_cate1.SelectedItem.ToString()

        ' ✅ If All Products, load everything
        If selectedCategory = "All Products" Then
            LoadProducts()
            Exit Sub
        End If

        Try
            FlowLayoutPanel1.Controls.Clear()

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()

                Dim query As String =
                "SELECT ProductID, ProductName, ProductImage, Price 
                 FROM products 
                 WHERE CategoryName = @cat"

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@cat", selectedCategory)

                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim productId As String = reader("ProductID").ToString()
                            Dim pname As String = reader("ProductName").ToString()
                            Dim price As Decimal = reader("Price")

                            Dim productImage As Image = Nothing
                            If Not IsDBNull(reader("ProductImage")) Then
                                Dim imgBytes As Byte() = DirectCast(reader("ProductImage"), Byte())
                                Using ms As New MemoryStream(imgBytes)
                                    productImage = Image.FromStream(ms)
                                End Using
                            End If

                            AddProductCard(productId, pname, productImage, price)
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error filtering products: " & ex.Message)
        End Try
    End Sub


    ' ===============================
    '  EVENT HANDLER FOR NEW PRODUCT
    ' ===============================
    Private Sub OnProductAdded(productId As String)
        Try
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT ProductID, ProductName, ProductImage, Price, CategoryName FROM products WHERE ProductID=@id"
                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", productId)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim pid As String = reader("ProductID").ToString()
                            Dim pname As String = reader("ProductName").ToString()
                            Dim price As Decimal = Convert.ToDecimal(reader("Price"))
                            Dim prodCategory As String = reader("CategoryName").ToString()
                            Dim productImage As Image = Nothing

                            If Not IsDBNull(reader("ProductImage")) Then
                                Dim imgBytes As Byte() = DirectCast(reader("ProductImage"), Byte())
                                Using ms As New MemoryStream(imgBytes)
                                    productImage = Image.FromStream(ms)
                                End Using
                            End If

                            ' ✅ If category matches OR All Products selected, add it
                            Dim categorySelected As String = If(cb_cate1.SelectedIndex <> -1, cb_cate1.SelectedItem.ToString(), "")
                            If categorySelected = "" OrElse categorySelected = "All Products" OrElse categorySelected = prodCategory Then
                                AddProductCard(pid, pname, productImage, price)
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading new product: " & ex.Message)
        End Try
    End Sub


    Private Sub AdjustLayout()
        Dim parentForm As Form = Nothing
        Dim isMaximized As Boolean = False

        Try
            If Me.Parent IsNot Nothing Then
                parentForm = Me.Parent.FindForm()
            End If
        Catch
        End Try

        If parentForm IsNot Nothing Then
            isMaximized = (parentForm.WindowState = FormWindowState.Maximized)
        End If

        If isMaximized Then
            Panel1.Size = New Size(290, Panel1.Height)
        Else
            Panel1.Size = New Size(240, Panel1.Height)
        End If
    End Sub

    Private Sub PosControl_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        AdjustLayout()
    End Sub

    Private Sub SiticoneButtonTextbox1_TextContentChanged(sender As Object, e As EventArgs) Handles SiticoneButtonTextbox1.TextContentChanged
        Dim searchText As String = SiticoneButtonTextbox1.Text.Trim().ToLower()

        For Each ctrl As Control In FlowLayoutPanel1.Controls
            If TypeOf ctrl Is SiticonePanel Then
                Dim card As SiticonePanel = CType(ctrl, SiticonePanel)

                ' Product name label (you added it as the 2nd control)
                Dim lblName As Label = TryCast(card.Controls.OfType(Of Label)().FirstOrDefault(), Label)

                If lblName IsNot Nothing Then
                    If searchText = "" Then
                        card.Visible = True
                    ElseIf lblName.Text.ToLower().Contains(searchText) Then
                        card.Visible = True
                    Else
                        card.Visible = False
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Guna2Button1_EnabledChanged(sender As Object, e As EventArgs) Handles Guna2Button1.EnabledChanged
        FlowLayoutPanel1.Visible = Guna2Button1.Enabled
    End Sub
    Public Sub EnableMainButton()
        Guna2Button1.Enabled = True
        Guna2Button1.BringToFront()
    End Sub

    Public Sub DisableMainButton()
        Guna2Button1.Enabled = False
        Guna2Button1.BringToFront()

    End Sub

End Class
