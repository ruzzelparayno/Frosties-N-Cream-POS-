<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SalesContent
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim BarChartTheme1 As SiticoneNetFrameworkUI.SiticoneBarChart.BarChartTheme = New SiticoneNetFrameworkUI.SiticoneBarChart.BarChartTheme()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SalesContent))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.SiticoneBarChart1 = New SiticoneNetFrameworkUI.SiticoneBarChart()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SiticonePanel2 = New SiticoneNetFrameworkUI.SiticonePanel()
        Me.SiticoneButtonTextbox2 = New SiticoneNetFrameworkUI.SiticoneButtonTextbox()
        Me.SiticoneDropdown1 = New SiticoneNetFrameworkUI.SiticoneDropdown()
        Me.SiticoneDataGridView1 = New SiticoneNetFrameworkUI.SiticoneDataGridView()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.SiticonePanel2.SuspendLayout()
        CType(Me.SiticoneDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.BackColor = System.Drawing.Color.Transparent
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Panel2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(758, 536)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(3, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(752, 262)
        Me.Panel2.TabIndex = 1
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Controls.Add(Me.SiticoneDataGridView1)
        Me.Panel3.Controls.Add(Me.SiticonePanel2)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Padding = New System.Windows.Forms.Padding(10)
        Me.Panel3.Size = New System.Drawing.Size(752, 262)
        Me.Panel3.TabIndex = 8
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.DimGray
        Me.Panel1.Controls.Add(Me.Panel5)
        Me.Panel1.Controls.Add(Me.Panel4)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 271)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(752, 262)
        Me.Panel1.TabIndex = 2
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.Teal
        Me.Panel5.Controls.Add(Me.SiticoneBarChart1)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(0, 44)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(752, 218)
        Me.Panel5.TabIndex = 16
        '
        'SiticoneBarChart1
        '
        Me.SiticoneBarChart1.BackColor = System.Drawing.SystemColors.Control
        Me.SiticoneBarChart1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SiticoneBarChart1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.SiticoneBarChart1.LabelMember = "Month"
        Me.SiticoneBarChart1.Location = New System.Drawing.Point(0, 0)
        Me.SiticoneBarChart1.Name = "SiticoneBarChart1"
        Me.SiticoneBarChart1.Size = New System.Drawing.Size(752, 218)
        Me.SiticoneBarChart1.TabIndex = 0
        BarChartTheme1.AxisLabelColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        BarChartTheme1.BackColor = System.Drawing.SystemColors.Control
        BarChartTheme1.BarColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer))
        BarChartTheme1.BarHighlightColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(181, Byte), Integer), CType(CType(246, Byte), Integer))
        BarChartTheme1.GridColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        BarChartTheme1.Name = "Default Blue"
        BarChartTheme1.ValueLabelColor = System.Drawing.Color.Black
        Me.SiticoneBarChart1.Theme = BarChartTheme1
        Me.SiticoneBarChart1.ValueMember = "Revenue"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.Panel4.Controls.Add(Me.Label5)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(752, 44)
        Me.Panel4.TabIndex = 15
        '
        'Label5
        '
        Me.Label5.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(10, 12)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(732, 21)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Sales Report"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'SiticonePanel2
        '
        Me.SiticonePanel2.AcrylicTintColor = System.Drawing.Color.Transparent
        Me.SiticonePanel2.BackColor = System.Drawing.Color.Transparent
        Me.SiticonePanel2.BorderAlignment = System.Drawing.Drawing2D.PenAlignment.Center
        Me.SiticonePanel2.BorderDashPattern = Nothing
        Me.SiticonePanel2.BorderGradientEndColor = System.Drawing.Color.Transparent
        Me.SiticonePanel2.BorderGradientStartColor = System.Drawing.Color.Transparent
        Me.SiticonePanel2.BorderThickness = 2.0!
        Me.SiticonePanel2.Controls.Add(Me.SiticoneButtonTextbox2)
        Me.SiticonePanel2.Controls.Add(Me.SiticoneDropdown1)
        Me.SiticonePanel2.CornerRadiusBottomLeft = 16.0!
        Me.SiticonePanel2.CornerRadiusBottomRight = 16.0!
        Me.SiticonePanel2.CornerRadiusTopLeft = 16.0!
        Me.SiticonePanel2.CornerRadiusTopRight = 16.0!
        Me.SiticonePanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.SiticonePanel2.EnableAcrylicEffect = False
        Me.SiticonePanel2.EnableMicaEffect = False
        Me.SiticonePanel2.EnableRippleEffect = False
        Me.SiticonePanel2.FillColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.SiticonePanel2.GradientColors = New System.Drawing.Color() {System.Drawing.Color.White, System.Drawing.Color.LightGray, System.Drawing.Color.Gray}
        Me.SiticonePanel2.GradientPositions = New Single() {0!, 0.5!, 1.0!}
        Me.SiticonePanel2.Location = New System.Drawing.Point(10, 10)
        Me.SiticonePanel2.Name = "SiticonePanel2"
        Me.SiticonePanel2.PatternStyle = System.Drawing.Drawing2D.HatchStyle.LargeGrid
        Me.SiticonePanel2.RippleAlpha = 50
        Me.SiticonePanel2.RippleAlphaDecrement = 3
        Me.SiticonePanel2.RippleColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SiticonePanel2.RippleMaxSize = 600.0!
        Me.SiticonePanel2.RippleSpeed = 15.0!
        Me.SiticonePanel2.ShowBorder = False
        Me.SiticonePanel2.Size = New System.Drawing.Size(732, 54)
        Me.SiticonePanel2.TabIndex = 13
        Me.SiticonePanel2.TabStop = True
        Me.SiticonePanel2.TrackSystemTheme = False
        Me.SiticonePanel2.UseBorderGradient = False
        Me.SiticonePanel2.UseMultiGradient = False
        Me.SiticonePanel2.UsePatternTexture = False
        Me.SiticonePanel2.UseRadialGradient = False
        '
        'SiticoneButtonTextbox2
        '
        Me.SiticoneButtonTextbox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SiticoneButtonTextbox2.BackColor = System.Drawing.Color.Transparent
        Me.SiticoneButtonTextbox2.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(238, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SiticoneButtonTextbox2.BorderColor = System.Drawing.Color.FromArgb(CType(CType(99, Byte), Integer), CType(CType(102, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.SiticoneButtonTextbox2.BottomLeftCornerRadius = 8
        Me.SiticoneButtonTextbox2.BottomRightCornerRadius = 8
        Me.SiticoneButtonTextbox2.ButtonColor = System.Drawing.Color.FromArgb(CType(CType(99, Byte), Integer), CType(CType(102, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.SiticoneButtonTextbox2.ButtonHoverColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox2.ButtonImageHover = Nothing
        Me.SiticoneButtonTextbox2.ButtonImageIdle = Nothing
        Me.SiticoneButtonTextbox2.ButtonImagePress = Nothing
        Me.SiticoneButtonTextbox2.ButtonPlaceholderColor = System.Drawing.Color.White
        Me.SiticoneButtonTextbox2.ButtonPlaceholderFont = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.SiticoneButtonTextbox2.ButtonPressColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.SiticoneButtonTextbox2.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.SiticoneButtonTextbox2.FocusBorderColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox2.FocusImage = Nothing
        Me.SiticoneButtonTextbox2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(163, Byte), Integer))
        Me.SiticoneButtonTextbox2.HoverBorderColor = System.Drawing.Color.Gray
        Me.SiticoneButtonTextbox2.HoverImage = Nothing
        Me.SiticoneButtonTextbox2.IdleImage = Nothing
        Me.SiticoneButtonTextbox2.Location = New System.Drawing.Point(544, 4)
        Me.SiticoneButtonTextbox2.Name = "SiticoneButtonTextbox2"
        Me.SiticoneButtonTextbox2.PlaceholderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.SiticoneButtonTextbox2.PlaceholderFocusColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox2.PlaceholderFont = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.SiticoneButtonTextbox2.PlaceholderText = "Search?"
        Me.SiticoneButtonTextbox2.ReadOnlyColors.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.SiticoneButtonTextbox2.ReadOnlyColors.BorderColor = System.Drawing.Color.FromArgb(CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer))
        Me.SiticoneButtonTextbox2.ReadOnlyColors.ButtonColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.SiticoneButtonTextbox2.ReadOnlyColors.ButtonPlaceholderColor = System.Drawing.Color.Gray
        Me.SiticoneButtonTextbox2.ReadOnlyColors.PlaceholderColor = System.Drawing.Color.FromArgb(CType(CType(150, Byte), Integer), CType(CType(150, Byte), Integer), CType(CType(150, Byte), Integer))
        Me.SiticoneButtonTextbox2.ReadOnlyColors.TextColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.SiticoneButtonTextbox2.RippleColor = System.Drawing.Color.White
        Me.SiticoneButtonTextbox2.Size = New System.Drawing.Size(161, 43)
        Me.SiticoneButtonTextbox2.TabIndex = 17
        Me.SiticoneButtonTextbox2.TextColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(163, Byte), Integer))
        Me.SiticoneButtonTextbox2.TextContent = ""
        Me.SiticoneButtonTextbox2.TextLeftMargin = 0
        Me.SiticoneButtonTextbox2.TopLeftCornerRadius = 8
        Me.SiticoneButtonTextbox2.TopRightCornerRadius = 8
        '
        'SiticoneDropdown1
        '
        Me.SiticoneDropdown1.AllowMultipleSelection = False
        Me.SiticoneDropdown1.BackColor = System.Drawing.Color.White
        Me.SiticoneDropdown1.BorderColor = System.Drawing.Color.FromArgb(CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.SiticoneDropdown1.BorderSize = 2
        Me.SiticoneDropdown1.CanBeep = False
        Me.SiticoneDropdown1.CanShake = True
        Me.SiticoneDropdown1.CornerRadius = 8
        Me.SiticoneDropdown1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.SiticoneDropdown1.DataSource = Nothing
        Me.SiticoneDropdown1.DisplayMember = Nothing
        Me.SiticoneDropdown1.DropdownBackColor = System.Drawing.Color.White
        Me.SiticoneDropdown1.DropdownCornerRadius = 6
        Me.SiticoneDropdown1.DropdownWidth = 0
        Me.SiticoneDropdown1.DropShadowEnabled = False
        Me.SiticoneDropdown1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.SiticoneDropdown1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.SiticoneDropdown1.HoveredItemBackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.SiticoneDropdown1.HoveredItemTextColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SiticoneDropdown1.IsReadonly = False
        Me.SiticoneDropdown1.ItemHeight = 30
        Me.SiticoneDropdown1.Location = New System.Drawing.Point(16, 4)
        Me.SiticoneDropdown1.MaxDropDownItems = 8
        Me.SiticoneDropdown1.Name = "SiticoneDropdown1"
        Me.SiticoneDropdown1.PlaceholderColor = System.Drawing.Color.Gray
        Me.SiticoneDropdown1.PlaceholderDisappearsOnFocus = False
        Me.SiticoneDropdown1.PlaceholderText = "Select an option"
        Me.SiticoneDropdown1.SelectedIndex = -1
        Me.SiticoneDropdown1.SelectedItem = Nothing
        Me.SiticoneDropdown1.SelectedItemBackColor = System.Drawing.Color.FromArgb(CType(CType(230, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.SiticoneDropdown1.SelectedItemTextColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SiticoneDropdown1.SelectedValue = Nothing
        Me.SiticoneDropdown1.Size = New System.Drawing.Size(190, 46)
        Me.SiticoneDropdown1.TabIndex = 1
        Me.SiticoneDropdown1.Text = "SiticoneDropdown1"
        Me.SiticoneDropdown1.UnselectedItemTextColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.SiticoneDropdown1.ValueMember = Nothing
        '
        'SiticoneDataGridView1
        '
        Me.SiticoneDataGridView1.AllowUserToResizeColumns = False
        Me.SiticoneDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.SiticoneDataGridView1.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(248, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SiticoneDataGridView1.CellFont = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.SiticoneDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SiticoneDataGridView1.GridTheme = SiticoneNetFrameworkUI.GridTheme.Blue
        Me.SiticoneDataGridView1.HeaderFont = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.SiticoneDataGridView1.Location = New System.Drawing.Point(10, 64)
        Me.SiticoneDataGridView1.Name = "SiticoneDataGridView1"
        Me.SiticoneDataGridView1.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.SiticoneDataGridView1.ShowSampleData = True
        Me.SiticoneDataGridView1.Size = New System.Drawing.Size(732, 188)
        Me.SiticoneDataGridView1.TabIndex = 14
        '
        'SalesContent
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "SalesContent"
        Me.Size = New System.Drawing.Size(758, 536)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.SiticonePanel2.ResumeLayout(False)
        CType(Me.SiticoneDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SiticoneDashboardCard1 As SiticoneNetFrameworkUI.SiticoneDashboardCard
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Panel4 As Panel
    Friend WithEvents SiticoneBarChart1 As SiticoneNetFrameworkUI.SiticoneBarChart
    Friend WithEvents SiticoneDataGridView1 As SiticoneDataGridView
    Friend WithEvents SiticonePanel2 As SiticonePanel
    Friend WithEvents SiticoneButtonTextbox2 As SiticoneButtonTextbox
    Friend WithEvents SiticoneDropdown1 As SiticoneDropdown
End Class
