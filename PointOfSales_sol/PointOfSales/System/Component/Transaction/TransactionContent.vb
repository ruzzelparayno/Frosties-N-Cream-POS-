Imports System.Data.SQLite
Imports System.Globalization
Imports System.Threading.Tasks

Public Class TransactionContent
    Public Shared Instance As TransactionContent

    Public Property AllowCloseOverlay As Boolean = True
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private connStr As String = "Data Source=" & dbPath & ";Version=3;"
    Private dt As DataTable

    ' ----------------- EVENT FOR REAL-TIME -----------------
    Public Shared Event TransactionAdded()
    Public Shared Sub NotifyTransactionAdded()
        RaiseEvent TransactionAdded()
    End Sub

    ' ----------------- FORM LOAD -----------------
    Private Sub TransactionContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Instance = Me  ' <-- Assign the current form to the Shared instance
        LoadSortOptions()
        LoadTransactions()
        Guna2DataGridView1.ScrollBars = ScrollBars.Vertical
        Guna2DataGridView1.Dock = DockStyle.Fill
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList

        ' =========================================
        ' For Datagridview
        Guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
        New Font("Segoe UI", 10, FontStyle.Bold)
        For Each col As DataGridViewColumn In Guna2DataGridView1.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        ' =========================================

        ' ----------------- SUBSCRIBE TO REAL-TIME EVENT -----------------
        AddHandler TransactionContent.TransactionAdded, AddressOf OnTransactionAdded
    End Sub

    ' ----------------- SORT OPTIONS -----------------
    Private Sub LoadSortOptions()
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("Ticketnumber Ascending")
        ComboBox1.Items.Add("Ticketnumber Descending")
        ComboBox1.Items.Add("TotalAmount Ascending")
        ComboBox1.Items.Add("TotalAmount Descending")
        ComboBox1.SelectedIndex = 1 ' default: Date Descending
    End Sub

    Private Function GetSortQuery() As String
        Select Case ComboBox1.SelectedItem.ToString()

            Case "Ticketnumber Ascending"
                Return "CAST(REPLACE(TicketNumber, '#', '') AS INTEGER) ASC"

            Case "Ticketnumber Descending"
                Return "CAST(REPLACE(TicketNumber, '#', '') AS INTEGER) DESC"

            Case "TotalAmount Ascending"
                Return "TotalAmount ASC"

            Case "TotalAmount Descending"
                Return "TotalAmount DESC"

            Case Else
                Return "SaleDate DESC"

        End Select
    End Function


    ' ----------------- LOAD TRANSACTIONS -----------------
    Public Sub LoadTransactions(Optional ByVal filter As String = "")
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()

                Dim query As String = "
            SELECT 
                TicketNumber, 
                ProductName, 
                SaleDate, 
                Subtotal, 
                DiscountType, 
                Vat, 
                TotalAmount, 
                Status 
            FROM sales 
            WHERE 1=1
        "

                ' ----------------- DATE FILTER -----------------
                If SiticoneDateTimePicker1.Value IsNot Nothing AndAlso SiticoneDateTimePicker2.Value IsNot Nothing Then
                    query &= " AND SaleDate BETWEEN @dateFrom AND @dateTo"
                ElseIf SiticoneDateTimePicker1.Value IsNot Nothing Then
                    query &= " AND SaleDate = @dateFrom"
                ElseIf SiticoneDateTimePicker2.Value IsNot Nothing Then
                    query &= " AND SaleDate <= @dateTo"
                End If

                ' ----------------- SEARCH FILTER -----------------
                If filter <> "" Then
                    query &= " AND (ProductName LIKE @filter OR TicketNumber LIKE @filter OR SaleDate LIKE @filter)"
                End If

                ' ----------------- SORT -----------------
                query &= " ORDER BY " & GetSortQuery()

                Using cmd As New SQLiteCommand(query, conn)
                    ' ----------------- DATE PARAMETERS -----------------
                    If SiticoneDateTimePicker1.Value IsNot Nothing Then
                        cmd.Parameters.AddWithValue("@dateFrom", SiticoneDateTimePicker1.Value.Value.ToString("yyyy-MM-dd"))
                    End If
                    If SiticoneDateTimePicker2.Value IsNot Nothing Then
                        cmd.Parameters.AddWithValue("@dateTo", SiticoneDateTimePicker2.Value.Value.ToString("yyyy-MM-dd"))
                    End If

                    ' ----------------- SEARCH PARAMETER -----------------
                    If filter <> "" Then cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")

                    ' ----------------- FILL DATATABLE -----------------
                    Dim adapter As New SQLiteDataAdapter(cmd)
                    dt = New DataTable()
                    adapter.Fill(dt)
                End Using
            End Using

            ' ----------------- BIND TO DATAGRIDVIEW -----------------
            If Guna2DataGridView1.InvokeRequired Then
                Guna2DataGridView1.Invoke(Sub() Guna2DataGridView1.DataSource = dt)
            Else
                Guna2DataGridView1.DataSource = dt
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading transactions: " & ex.Message)
        End Try
    End Sub

    ' ----------------- REAL-TIME HANDLER -----------------
    Private Sub OnTransactionAdded()
        If Me.InvokeRequired Then
            Me.Invoke(Sub() LoadTransactions())
        Else
            LoadTransactions()
        End If
    End Sub

    ' ----------------- EVENT HANDLERS -----------------
    Private Sub SiticoneButtonTextbox1_TextChanged(sender As Object, e As EventArgs) Handles SiticoneButtonTextbox1.TextChanged
        Dim searchText As String = SiticoneButtonTextbox1.Text.Trim()
        LoadTransactions(searchText)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        LoadTransactions()
    End Sub

    Private Sub SiticoneDateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker1.ValueChanged
        LoadTransactions()
    End Sub

    Private Sub SiticoneDateTimePicker2_ValueChanged(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker2.ValueChanged
        LoadTransactions()
    End Sub

    ' ----------------- OTHER FUNCTIONS -----------------
    Private Function GetTotalRefunded() As Decimal
        Dim totalRefund As Decimal = 0D
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()
                Dim query As String = "SELECT IFNULL(SUM(TotalAmount),0) FROM sales WHERE Status='Refunded'"
                Using cmd As New SQLiteCommand(query, conn)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        totalRefund = Convert.ToDecimal(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error calculating total refunded amount: " & ex.Message)
        End Try
        Return totalRefund
    End Function

    Private Sub RestoreStockForTicket(ticketNumber As String)
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()
                Dim q As String = "SELECT ProductName, Quantity FROM sales WHERE TicketNumber=@TicketNumber AND Status='Completed'"
                Using cmd As New SQLiteCommand(q, conn)
                    cmd.Parameters.AddWithValue("@TicketNumber", ticketNumber)
                    Using reader = cmd.ExecuteReader()
                        Dim toRestore As New List(Of (String, Integer))
                        While reader.Read()
                            Dim pname = reader("ProductName").ToString()
                            Dim qty = If(IsDBNull(reader("Quantity")), 0, Convert.ToInt32(reader("Quantity")))
                            If qty > 0 Then toRestore.Add((pname, qty))
                        End While
                        reader.Close()
                        For Each pair In toRestore
                            Using upd As New SQLiteCommand("UPDATE products SET StockQuantity = StockQuantity + @qty WHERE ProductName=@pname", conn)
                                upd.Parameters.AddWithValue("@qty", pair.Item2)
                                upd.Parameters.AddWithValue("@pname", pair.Item1)
                                upd.ExecuteNonQuery()
                            End Using
                        Next
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error restoring stock for ticket: " & ex.Message, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetProductPrice(productName As String) As String
        Dim price As String = "₱0.00"
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()
                Dim query As String = "SELECT Price FROM products WHERE ProductName=@ProductName LIMIT 1"
                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ProductName", productName.Trim())
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then price = Convert.ToDecimal(result).ToString("₱0.00")
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error getting product price: " & ex.Message)
        End Try
        Return price
    End Function

    ' ----------------- CELL DOUBLE CLICK -----------------
    Private Async Sub Guna2DataGridView1_CellDoubleClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub
        SiticoneOverlay1.Show = True
        Await Task.Delay(1500)
        Dim selectedRow = Guna2DataGridView1.Rows(e.RowIndex)
        Dim ticketNum = selectedRow.Cells("TicketNumber").Value.ToString()
        Dim productName = selectedRow.Cells("ProductName").Value.ToString().Trim()
        Dim priceText = selectedRow.Cells("TotalAmount").Value.ToString()
        Dim totalPrice As Decimal = 0D
        Decimal.TryParse(priceText.Replace("₱", "").Replace(",", "").Trim(), totalPrice)

        Dim mop As String = ""

        ' Optional: get Mode of Payment from database
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()
                Dim query = "SELECT ModeOfPayment FROM sales WHERE TRIM(ProductName) = @ProductName AND TicketNumber = @TicketNumber LIMIT 1"
                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ProductName", productName)
                    cmd.Parameters.AddWithValue("@TicketNumber", ticketNum)
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            If Not IsDBNull(reader("ModeOfPayment")) Then
                                mop = reader("ModeOfPayment").ToString().Trim()
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error getting mode of payment: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            If AllowCloseOverlay Then SiticoneOverlay1.Show = False
        End Try

        ' VAT calculation
        Dim vatRate As Decimal = 0.12D
        Dim vatAmount As Decimal = Math.Round(totalPrice * vatRate, 2)
        Dim subtotalAmount As Decimal = Math.Round(totalPrice - vatAmount, 2)

        ' Send data to refund form
        Dim refundForm As New RefundForm
        refundForm.lbl_getticket.Text = ticketNum
        refundForm.lbl_getproducts.Text = productName
        refundForm.lbl_getprice.Text = "₱" & totalPrice.ToString("N2")
        refundForm.lbl_getsubtotal.Text = "₱" & subtotalAmount.ToString("N2")
        refundForm.lbl_getvat.Text = "₱" & vatAmount.ToString("N2")
        refundForm.lbl_gettotal.Text = "₱" & totalPrice.ToString("N2")
        refundForm.lbl_MOP.Text = If(mop <> "", mop, "")

        refundForm.ShowDialog()

        ' Refund confirmed
        If refundForm.DialogResult = DialogResult.OK Then
            RestoreStockForTicket(ticketNum)
            LoadTransactions()

            Dim shiftForm As ShiftContent = Dashboard.shiftInstance
            If shiftForm IsNot Nothing AndAlso shiftForm.Panel2.Controls.Count > 0 Then
                Dim cashCtrl As CashManagementControll = TryCast(shiftForm.Panel2.Controls(0), CashManagementControll)
                If cashCtrl IsNot Nothing Then
                    Dim totalRefunds As Decimal = GetTotalRefunded()
                    cashCtrl.lbl_srefunds.Text = "₱" & totalRefunds.ToString("N2")
                    cashCtrl.ComputeTotal()
                End If
            End If

            MessageBox.Show("Refund applied and stock restored successfully.", "Refund", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If AllowCloseOverlay Then SiticoneOverlay1.Show = False
        End If
    End Sub

    Private rowToolTip As New ToolTip()
    Private Sub Guna2DataGridView1_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs) Handles Guna2DataGridView1.CellMouseEnter
        Try
            ' Ensure mouse is over a valid row
            If e.RowIndex >= 0 Then
                Dim row As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)
                Dim tooltipText As String = String.Format(
                "TicketNumber: {0}" & vbCrLf &
                "ProductName: {1}" & vbCrLf &
                "SaleDate: {2}" & vbCrLf &
                "Subtotal: {3}" & vbCrLf &
                "DiscountType: {4}" & vbCrLf &
                "VAT: {5}" & vbCrLf &
                "TotalAmount: {6}" & vbCrLf &
                "Status: {7}",
                row.Cells("TicketNumber").Value.ToString(),
                row.Cells("ProductName").Value.ToString(),
                row.Cells("SaleDate").Value.ToString(),
                row.Cells("Subtotal").Value.ToString(),
                row.Cells("DiscountType").Value.ToString(),
                row.Cells("Vat").Value.ToString(),
                row.Cells("TotalAmount").Value.ToString(),
                row.Cells("Status").Value.ToString()
            )

                rowToolTip.SetToolTip(Guna2DataGridView1, tooltipText)
            End If
        Catch ex As Exception
            ' Ignore errors
        End Try
    End Sub
End Class
