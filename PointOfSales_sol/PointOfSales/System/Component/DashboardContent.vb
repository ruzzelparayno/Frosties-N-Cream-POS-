Imports System.Globalization
Imports System.IO
Imports Guna.Charts.Interfaces
Imports Guna.Charts.WinForms
Imports System.Data.SQLite
Imports SiticoneNetFrameworkUI

Public Class DashboardContent
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private connectionString As String = "Data Source=" & dbPath & ";Version=3;"

    ' ✅ Shared instance for cross-dashboard access
    Public Shared Instance As DashboardContent

    Private Sub DashboardContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Instance = Me
        RefreshDashboard()
        LoadWeeklySales_GunaChart()
        LoadCategoryDoughnutChart()
        LoadRevenueByCategory_LineChart()
        LoadMonthlyTransactions_GunaChart()
        LoadTotalRevenue()
    End Sub

    Private Sub DashboardContent_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible Then RefreshDashboard()
    End Sub

    Private Sub DashboardContent_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        AdjustLayout()
    End Sub
    Private Sub CheckLabelSizeInMax(lbl As Label)
        Dim digitCount As Integer = lbl.Text.Length

        If digitCount >= 6 Then
            lbl.Font = New Font(lbl.Font.FontFamily, 40, FontStyle.Bold)
        Else
            lbl.Font = New Font(lbl.Font.FontFamily, 60, FontStyle.Bold)
        End If

    End Sub
    Private Sub CheckLabelSizeInMini(lbl As Label)
        Dim digitCount As Integer = lbl.Text.Length

        If digitCount >= 6 Then
            lbl.Font = New Font(lbl.Font.FontFamily, 27, FontStyle.Bold)
        Else
            lbl.Font = New Font(lbl.Font.FontFamily, 30, FontStyle.Bold)
        End If
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
            Panel2.Size = New Size(758, 175)
            Label2.Font = New Font(Label2.Font.FontFamily, 20, Label2.Font.Style)
            'Label3.Font = New Font(Label3.Font.FontFamily, 60, Label3.Font.Style) 
            CheckLabelSizeInMax(Label3)
            Label5.Font = New Font(Label5.Font.FontFamily, 20, Label5.Font.Style)
            'Label4.Font = New Font(Label4.Font.FontFamily, 60, Label4.Font.Style)
            CheckLabelSizeInMax(Label4)
            Label7.Font = New Font(Label7.Font.FontFamily, 20, Label7.Font.Style)
            CheckLabelSizeInMax(Label11)

            Label1.Font = New Font(Label1.Font.FontFamily, 20, Label1.Font.Style)
            Label6.Font = New Font(Label6.Font.FontFamily, 20, Label6.Font.Style)
            Label8.Font = New Font(Label8.Font.FontFamily, 20, Label8.Font.Style)
            Label9.Font = New Font(Label9.Font.FontFamily, 20, Label9.Font.Style)
            Label10.Font = New Font(Label10.Font.FontFamily, 20, Label10.Font.Style)

            FlowLayoutPanel2.Padding = New Padding(0, FlowLayoutPanel2.Padding.Top, FlowLayoutPanel2.Padding.Right, FlowLayoutPanel2.Padding.Bottom)
        Else
            Panel2.Size = New Size(758, 125)
            Label2.Font = New Font(Label2.Font.FontFamily, 12, Label2.Font.Style)
            'Label3.Font = New Font(Label3.Font.FontFamily, 40, Label3.Font.Style)
            CheckLabelSizeInMini(Label3)
            Label5.Font = New Font(Label5.Font.FontFamily, 12, Label5.Font.Style)
            'Label4.Font = New Font(Label4.Font.FontFamily, 40, Label4.Font.Style)
            CheckLabelSizeInMini(Label4)
            Label7.Font = New Font(Label7.Font.FontFamily, 12, Label7.Font.Style)

            CheckLabelSizeInMini(Label11)

            Label1.Font = New Font(Label1.Font.FontFamily, 12, Label1.Font.Style)
            Label6.Font = New Font(Label6.Font.FontFamily, 12, Label6.Font.Style)
            Label8.Font = New Font(Label8.Font.FontFamily, 12, Label8.Font.Style)
            Label9.Font = New Font(Label9.Font.FontFamily, 12, Label9.Font.Style)
            Label10.Font = New Font(Label10.Font.FontFamily, 12, Label10.Font.Style)

            FlowLayoutPanel2.Padding = New Padding(0, FlowLayoutPanel2.Padding.Top, FlowLayoutPanel2.Padding.Right, FlowLayoutPanel2.Padding.Bottom)
        End If
    End Sub

    ' =======================
    ' ⚠️ LOW STOCK PRODUCTS
    ' =======================
    Public Sub LoadLowStockProducts()
        Try
            FlowLayoutPanel2.Controls.Clear()
            Dim hasLowStock As Boolean = False

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()

                Dim query As String = "
                    SELECT ProductID, ProductName, ProductImage, Price, StockQuantity
                    FROM products
                    WHERE StockQuantity <= 13
                    ORDER BY StockQuantity ASC;
                "

                Using cmd As New SQLiteCommand(query, conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            hasLowStock = True
                            Dim pname As String = reader("ProductName").ToString()
                            Dim stockQty As Integer = Convert.ToInt32(reader("StockQuantity"))

                            Dim productImage As Image = Nothing
                            If Not IsDBNull(reader("ProductImage")) Then
                                Dim imgBytes() As Byte = DirectCast(reader("ProductImage"), Byte())
                                If imgBytes IsNot Nothing AndAlso imgBytes.Length > 0 Then
                                    Using ms As New MemoryStream(imgBytes)
                                        productImage = Image.FromStream(ms)
                                    End Using
                                End If
                            End If

                            Dim productPanel As New Panel With {
                                .Width = 143,
                                .Height = 160,
                                .BackColor = Color.FromArgb(217, 220, 235),
                                .Margin = New Padding(3)
                            }

                            Dim pic As New PictureBox With {
                                .Width = 64,
                                .Height = 54,
                                .SizeMode = PictureBoxSizeMode.Zoom,
                                .Image = productImage,
                                .Dock = DockStyle.Fill
                            }

                            Dim lblName As New Label With {
                                .AutoSize = False,
                                .TextAlign = ContentAlignment.TopCenter,
                                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                                .Text = pname,
                                .Dock = DockStyle.Top,
                                .ForeColor = Color.FromArgb(75, 75, 75)
                            }

                            Dim lblStock As New Label With {
                                .AutoSize = False,
                                .TextAlign = ContentAlignment.MiddleCenter,
                                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                                .Text = stockQty & " QTY",
                                .Dock = DockStyle.Bottom,
                                .ForeColor = Color.FromArgb(75, 75, 75)
                            }

                            productPanel.Controls.Add(pic)
                            productPanel.Controls.Add(lblName)
                            productPanel.Controls.Add(lblStock)
                            FlowLayoutPanel2.Controls.Add(productPanel)
                        End While
                    End Using
                End Using
            End Using

            FlowLayoutPanel2.Visible = hasLowStock
            Guna2Panel1.Visible = Not hasLowStock

        Catch ex As Exception
            MessageBox.Show("Error loading low-stock products: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' 🧾 ORDERS + SALES
    ' =======================
    Private Sub LoadTodayOrders()
        Try
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                ' SQLite CURRENT_DATE returns YYYY-MM-DD
                Dim query As String = "
                    SELECT COUNT(DISTINCT SalesID)
                    FROM sales
                    WHERE DATE(SaleDate) = DATE('now', 'localtime');
                "
                Using cmd As New SQLiteCommand(query, conn)
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    Label3.Text = count.ToString("N0")
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading today's orders: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadTodayGrossSales()
        Try
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "
                    SELECT IFNULL(SUM(TotalAmount), 0)
                    FROM sales
                    WHERE DATE(SaleDate) = DATE('now', 'localtime');
                "
                Using cmd As New SQLiteCommand(query, conn)
                    Dim totalSales As Decimal = Convert.ToDecimal(cmd.ExecuteScalar())
                    Label4.Text = totalSales.ToString("N2", CultureInfo.InvariantCulture)
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading today's gross sales: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' 🔁 REFRESH DASHBOARD
    ' =======================
    Public Sub RefreshDashboard()
        Try
            LoadLowStockProducts()
            LoadTodayOrders()
            LoadTodayGrossSales()
            LoadWeeklySales_GunaChart()
            LoadCategoryDoughnutChart()
            LoadRevenueByCategory_LineChart()
            LoadMonthlyTransactions_GunaChart()
            LoadTotalRevenue()
        Catch ex As Exception
            MessageBox.Show("Error refreshing dashboard: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' 📊 WEEKLY SALES (SQLite version)
    ' =======================
    Private Sub LoadWeeklySales_GunaChart()
        Try
            GunaChart1.Datasets.Clear()
            Dim dataset As New Guna.Charts.WinForms.GunaBarDataset()
            dataset.Label = "Weekly Sales"
            dataset.FillColors.Add(Color.FromArgb(255, 0, 148, 255))

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()

                ' SQLite: strftime('%w', date) returns day of week 0=Sun,1=Mon,...6=Sat
                Dim query As String = "
                    SELECT day_names.DayName, IFNULL(SUM(s.TotalAmount),0) AS Revenue
                    FROM (SELECT 'Mon' AS DayName, 1 AS DayOfWeek UNION ALL
                          SELECT 'Tue', 2 UNION ALL
                          SELECT 'Wed', 3 UNION ALL
                          SELECT 'Thu', 4 UNION ALL
                          SELECT 'Fri', 5 UNION ALL
                          SELECT 'Sat', 6 UNION ALL
                          SELECT 'Sun', 0) AS day_names
                    LEFT JOIN sales s
                      ON CAST(strftime('%w', s.SaleDate) AS INTEGER) = day_names.DayOfWeek
                      AND strftime('%W', s.SaleDate) = strftime('%W','now','localtime')
                    GROUP BY day_names.DayName, day_names.DayOfWeek
                    ORDER BY day_names.DayOfWeek;
                "

                Using cmd As New SQLiteCommand(query, conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            dataset.DataPoints.Add(reader("DayName").ToString(), Convert.ToDecimal(reader("Revenue")))
                        End While
                    End Using
                End Using
            End Using

            GunaChart1.Datasets.Add(dataset)
            GunaChart1.YAxes.GridLines.Display = True
            GunaChart1.YAxes.Ticks.ForeColor = Color.FromArgb(75, 75, 75)
            GunaChart1.XAxes.Ticks.ForeColor = Color.FromArgb(75, 75, 75)
            GunaChart1.Legend.LabelForeColor = Color.FromArgb(75, 75, 75)
            GunaChart1.Title.ForeColor = Color.FromArgb(75, 75, 75)
            GunaChart1.Update()
        Catch ex As Exception
            MessageBox.Show("Error loading weekly sales: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' CATEGORY DOUGHNUT CHART
    ' =======================
    Private Sub LoadCategoryDoughnutChart()
        Try
            Dim dt As New DataTable()
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "
                    SELECT Category, IFNULL(SUM(TotalAmount),0) AS Amount
                    FROM sales
                    GROUP BY Category;
                "
                Using da As New SQLiteDataAdapter(query, conn)
                    da.Fill(dt)
                End Using
            End Using

            SiticoneDoughnutChart1.DataSource = dt
            SiticoneDoughnutChart1.LabelMember = "Category"
            SiticoneDoughnutChart1.ValueMember = "Amount"
            SiticoneDoughnutChart1.BackColor = Color.FromArgb(217, 220, 235)

            Dim total As Decimal = 0
            For Each row As DataRow In dt.Rows
                If Not IsDBNull(row("Amount")) Then total += Convert.ToDecimal(row("Amount"))
            Next

            SiticoneDoughnutChart1.CenterText = "Total"
            SiticoneDoughnutChart1.CenterSubText = "₱" & total.ToString("N2")
            SiticoneDoughnutChart1.Refresh()
        Catch ex As Exception
            MessageBox.Show("Error loading chart by category: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' REVENUE BY CATEGORY LINE CHART
    ' =======================
    Private Sub LoadRevenueByCategory_LineChart()
        Try
            GunaChart2.Datasets.Clear()
            Dim lineDataset As New Guna.Charts.WinForms.GunaLineDataset()
            lineDataset.Label = "Revenue"
            lineDataset.BorderColor = Color.FromArgb(255, 0, 148, 255)
            lineDataset.PointRadius = 5
            lineDataset.PointFillColors.Add(Color.FromArgb(255, 0, 148, 255))
            lineDataset.PointBorderColors.Add(Color.FromArgb(255, 0, 148, 255))

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "
                    SELECT COALESCE(p.CategoryName,'Uncategorized') AS Category,
                           IFNULL(SUM(s.TotalAmount),0) AS Revenue
                    FROM sales s
                    LEFT JOIN products p ON TRIM(s.ProductName) = TRIM(p.ProductName)
                    GROUP BY COALESCE(p.CategoryName,'Uncategorized')
                    ORDER BY Category ASC;
                "
                Using cmd As New SQLiteCommand(query, conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            lineDataset.DataPoints.Add(reader("Category").ToString(), Convert.ToDouble(reader("Revenue")))
                        End While
                    End Using
                End Using
            End Using

            GunaChart2.Datasets.Add(lineDataset)
            GunaChart2.Update()
        Catch ex As Exception
            MessageBox.Show("Error loading revenue by category: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' MONTHLY TRANSACTIONS
    ' =======================
    Private Sub LoadMonthlyTransactions_GunaChart()
        Try
            GunaChart3.Datasets.Clear()
            Dim barDataset As New Guna.Charts.WinForms.GunaBarDataset()
            barDataset.Label = "Monthly Transactions"
            barDataset.FillColors.Add(Color.FromArgb(255, 0, 148, 255))

            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "
                    SELECT strftime('%m', SaleDate) AS MonthNum,
                           COUNT(SalesID) AS TotalTransactions
                    FROM sales
                    GROUP BY MonthNum
                    ORDER BY MonthNum;
                "
                Using cmd As New SQLiteCommand(query, conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim monthNum As Integer = Convert.ToInt32(reader("MonthNum"))
                            Dim monthName As String = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNum)
                            barDataset.DataPoints.Add(monthName, Convert.ToInt32(reader("TotalTransactions")))
                        End While
                    End Using
                End Using
            End Using

            GunaChart3.Datasets.Add(barDataset)
            GunaChart3.YAxes.GridLines.Display = True
            GunaChart3.YAxes.Ticks.ForeColor = Color.FromArgb(75, 75, 75)
            GunaChart3.XAxes.Ticks.ForeColor = Color.FromArgb(75, 75, 75)
            GunaChart3.Legend.LabelForeColor = Color.FromArgb(75, 75, 75)
            GunaChart3.Update()
        Catch ex As Exception
            MessageBox.Show("Error loading monthly transactions: " & ex.Message)
        End Try
    End Sub

    ' =======================
    ' TOTAL REVENUE
    ' =======================
    Private Sub LoadTotalRevenue()
        Try
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()
                Dim query As String = "
                    SELECT IFNULL(SUM(TotalAmount),0)
                    FROM sales;
                "
                Using cmd As New SQLiteCommand(query, conn)
                    Dim totalRev As Decimal = Convert.ToDecimal(cmd.ExecuteScalar())
                    Label11.Text = "₱" & totalRev.ToString("N2")

                    Dim txt As String = Label11.Text.Replace("₱", "").Replace(",", "").Split("."c)(0)
                    Dim digitCount As Integer = txt.Length

                    If digitCount >= 9 Then
                        Label11.Font = New Font(Label11.Font.FontFamily, 20, FontStyle.Bold)
                    ElseIf digitCount >= 8 Then
                        Label11.Font = New Font(Label11.Font.FontFamily, 24, FontStyle.Bold)
                    Else
                        Label11.Font = New Font(Label11.Font.FontFamily, 28, FontStyle.Bold)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading total revenue: " & ex.Message)
        End Try
    End Sub
End Class
