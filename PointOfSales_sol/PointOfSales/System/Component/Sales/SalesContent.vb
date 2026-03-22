Imports System.Data.SQLite
Imports System.Globalization

Public Class SalesContent
    Public Property AllowCloseOverlay As Boolean = True

    ' SQLite connection
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")

    ' -------------------------------------------------------------
    ' LOAD SALES INTO DATAGRID
    ' -------------------------------------------------------------
    Public Sub LoadSalesData(Optional ByVal filter As String = "", Optional ByVal sortBy As String = "SaleDate ASC")
        Try
            conn.Open()

            Dim query As String = "SELECT SalesID, SaleDate, ProductName, TicketNumber, TotalAmount FROM sales WHERE 1=1"

            If filter <> "" Then
                query &= " AND (ProductName LIKE @filter OR SaleDate LIKE @filter OR TicketNumber LIKE @filter)"
            End If

            query &= " ORDER BY " & sortBy

            Dim cmd As New SQLiteCommand(query, conn)

            If filter <> "" Then
                cmd.Parameters.AddWithValue("@filter", "%" & filter & "%")
            End If

            Dim adapter As New SQLiteDataAdapter(cmd)
            Dim dt As New DataTable()
            adapter.Fill(dt)

            Guna2DataGridView1.DataSource = dt

            ' Auto-size columns (same as your original code)
            If Guna2DataGridView1.Columns.Contains("SalesID") Then
                Guna2DataGridView1.Columns("SalesID").AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            End If
            If Guna2DataGridView1.Columns.Contains("TotalAmount") Then
                Guna2DataGridView1.Columns("TotalAmount").AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            End If
            If Guna2DataGridView1.Columns.Contains("SaleDate") Then
                Guna2DataGridView1.Columns("SaleDate").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            End If
            If Guna2DataGridView1.Columns.Contains("ProductName") Then
                Guna2DataGridView1.Columns("ProductName").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If
            If Guna2DataGridView1.Columns.Contains("TicketNumber") Then
                Guna2DataGridView1.Columns("TicketNumber").AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading sales data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' LOAD MONTHLY SALES CHART
    ' -------------------------------------------------------------
    Public Sub LoadMonthlySalesChart()
        Try
            conn.Open()

            Dim currentYear As Integer = DateTime.Now.Year

            Dim query As String = "
            SELECT strftime('%m', SaleDate) AS SaleMonth, SUM(TotalAmount) AS TotalAmount
            FROM sales
            WHERE strftime('%Y', SaleDate) = @year
            GROUP BY strftime('%m', SaleDate)
            ORDER BY SaleMonth
            "

            Dim cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@year", currentYear.ToString())

            Dim reader As SQLiteDataReader = cmd.ExecuteReader()

            Dim monthlySales As New Dictionary(Of Integer, Decimal)

            While reader.Read()
                Dim month As Integer = Convert.ToInt32(reader("SaleMonth"))
                Dim total As Decimal = Convert.ToDecimal(reader("TotalAmount"))
                monthlySales(month) = total
            End While

            reader.Close()

            GunaChart1.Datasets.Clear()

            Dim series As New Guna.Charts.WinForms.GunaBarDataset()
            series.Label = "Sales Report"

            For i As Integer = 1 To 12
                Dim monthName As String = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i)
                Dim value As Decimal = If(monthlySales.ContainsKey(i), monthlySales(i), 0)
                series.DataPoints.Add(monthName, value)
            Next

            GunaChart1.Datasets.Add(series)
            GunaChart1.Update()

        Catch ex As Exception
            MessageBox.Show("Error loading monthly sales chart: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' POPULATE SORT OPTIONS
    ' -------------------------------------------------------------
    Private Sub LoadSortOptions()
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("Ticketnumber Ascending")
        ComboBox1.Items.Add("Ticketnumber Descending")
        ComboBox1.Items.Add("TotalAmount Ascending")
        ComboBox1.Items.Add("TotalAmount Descending")
        ComboBox1.SelectedIndex = 0
    End Sub

    ' -------------------------------------------------------------
    ' RETURN SORT QUERY
    ' -------------------------------------------------------------
    Private Function GetSortQuery() As String
        Select Case ComboBox1.SelectedItem.ToString()
            Case "Date Ascending"
                Return "SaleDate ASC"
            Case "Date Descending"
                Return "SaleDate DESC"
            Case "TotalAmount Ascending"
                Return "TotalAmount ASC"
            Case "TotalAmount Descending"
                Return "TotalAmount DESC"
            Case Else
                Return "SaleDate ASC"
        End Select
    End Function

    ' -------------------------------------------------------------
    ' FILTER BY DATE RANGE + SEARCH + SORT
    ' -------------------------------------------------------------
    Public Sub LoadSalesDataWithFilter()
        Try
            conn.Open()

            Dim query As String = "SELECT SalesID, SaleDate, ProductName, TicketNumber, TotalAmount FROM sales WHERE 1=1"
            Dim cmd As New SQLiteCommand()
            cmd.Connection = conn

            ' --- DATE FILTERS ---
            If SiticoneDateTimePicker1.Value.HasValue AndAlso SiticoneDateTimePicker2.Value.HasValue Then
                query &= " AND SaleDate BETWEEN @dateFrom AND @dateTo"
                cmd.Parameters.AddWithValue("@dateFrom", SiticoneDateTimePicker1.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@dateTo", SiticoneDateTimePicker2.Value.ToString("yyyy-MM-dd"))
            ElseIf SiticoneDateTimePicker1.Value.HasValue Then
                query &= " AND date(SaleDate) = date(@dateFrom)"
                cmd.Parameters.AddWithValue("@dateFrom", SiticoneDateTimePicker1.Value.ToString("yyyy-MM-dd"))
            ElseIf SiticoneDateTimePicker2.Value.HasValue Then
                query &= " AND SaleDate <= @dateTo"
                cmd.Parameters.AddWithValue("@dateTo", SiticoneDateTimePicker2.Value.ToString("yyyy-MM-dd"))
            End If

            ' --- SEARCH ---
            If SiticoneButtonTextbox2.Text <> "" Then
                query &= " AND (ProductName LIKE @filter OR TicketNumber LIKE @filter)"
                cmd.Parameters.AddWithValue("@filter", "%" & SiticoneButtonTextbox2.Text & "%")
            End If

            ' --- SORT ---
            query &= " ORDER BY " & GetSortQuery()

            cmd.CommandText = query

            Dim da As New SQLiteDataAdapter(cmd)
            Dim dt As New DataTable
            da.Fill(dt)

            Guna2DataGridView1.DataSource = dt

        Catch ex As Exception
            MessageBox.Show("Error filtering data: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' -------------------------------------------------------------
    ' REFRESH BOTH GRID + CHART
    ' -------------------------------------------------------------
    Public Sub RefreshSales(Optional ByVal filter As String = "")
        LoadSalesData(filter, GetSortQuery())
        LoadMonthlySalesChart()
    End Sub

    ' -------------------------------------------------------------
    ' FORM LOAD
    ' -------------------------------------------------------------
    Private Sub SalesContent_Load(sender As Object, e As EventArgs) Handles Me.Load
        LoadSortOptions()
        LoadSalesDataWithFilter()
        LoadMonthlySalesChart()

        ' =========================================
        ' For Datagridview
        Guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
        New Font("Segoe UI", 10, FontStyle.Bold)
        For Each col As DataGridViewColumn In Guna2DataGridView1.Columns
            col.SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        ' =========================================

    End Sub

    ' -------------------------------------------------------------
    ' EVENT HANDLERS
    ' -------------------------------------------------------------
    Private Sub SiticoneDateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker1.ValueChanged
        LoadSalesDataWithFilter()
    End Sub

    Private Sub SiticoneDateTimePicker2_ValueChanged(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker2.ValueChanged
        LoadSalesDataWithFilter()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        LoadSalesDataWithFilter()
    End Sub

    Private Sub SiticoneButtonTextbox2_TextChanged(sender As Object, e As EventArgs) Handles SiticoneButtonTextbox2.TextChanged
        LoadSalesDataWithFilter()
    End Sub

    Private Async Sub ComboBox1_Click(sender As Object, e As EventArgs) Handles ComboBox1.Click
        SiticoneOverlay1.Show = True
        Await Task.Delay(2000)
        If AllowCloseOverlay Then SiticoneOverlay1.Show = False
    End Sub

    Private Async Sub SiticoneDateTimePicker1_Click(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker1.Click
        SiticoneOverlay1.Show = True
        Await Task.Delay(2000)
        If AllowCloseOverlay Then SiticoneOverlay1.Show = False
    End Sub

    Private Async Sub SiticoneDateTimePicker2_Click(sender As Object, e As EventArgs) Handles SiticoneDateTimePicker2.Click
        SiticoneOverlay1.Show = True
        Await Task.Delay(2000)
        If AllowCloseOverlay Then SiticoneOverlay1.Show = False
    End Sub

End Class
