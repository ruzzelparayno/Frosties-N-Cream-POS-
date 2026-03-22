<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TransactionContent
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.SiticonePanel2 = New SiticoneNetFrameworkUI.SiticonePanel()
        Me.SiticoneButtonTextbox1 = New SiticoneNetFrameworkUI.SiticoneButtonTextbox()
        Me.dgv_transactions = New System.Windows.Forms.DataGridView()
        Me.Panel3.SuspendLayout()
        Me.SiticonePanel2.SuspendLayout()
        CType(Me.dgv_transactions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Controls.Add(Me.dgv_transactions)
        Me.Panel3.Controls.Add(Me.SiticonePanel2)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Padding = New System.Windows.Forms.Padding(10)
        Me.Panel3.Size = New System.Drawing.Size(758, 536)
        Me.Panel3.TabIndex = 8
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
        Me.SiticonePanel2.Controls.Add(Me.SiticoneButtonTextbox1)
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
        Me.SiticonePanel2.Size = New System.Drawing.Size(738, 54)
        Me.SiticonePanel2.TabIndex = 11
        Me.SiticonePanel2.TabStop = True
        Me.SiticonePanel2.TrackSystemTheme = False
        Me.SiticonePanel2.UseBorderGradient = False
        Me.SiticonePanel2.UseMultiGradient = False
        Me.SiticonePanel2.UsePatternTexture = False
        Me.SiticonePanel2.UseRadialGradient = False
        '
        'SiticoneButtonTextbox1
        '
        Me.SiticoneButtonTextbox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SiticoneButtonTextbox1.BackColor = System.Drawing.Color.Transparent
        Me.SiticoneButtonTextbox1.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(238, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SiticoneButtonTextbox1.BorderColor = System.Drawing.Color.FromArgb(CType(CType(99, Byte), Integer), CType(CType(102, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.SiticoneButtonTextbox1.BottomLeftCornerRadius = 8
        Me.SiticoneButtonTextbox1.BottomRightCornerRadius = 8
        Me.SiticoneButtonTextbox1.ButtonColor = System.Drawing.Color.FromArgb(CType(CType(99, Byte), Integer), CType(CType(102, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.SiticoneButtonTextbox1.ButtonHoverColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox1.ButtonImageHover = Nothing
        Me.SiticoneButtonTextbox1.ButtonImageIdle = Nothing
        Me.SiticoneButtonTextbox1.ButtonImagePress = Nothing
        Me.SiticoneButtonTextbox1.ButtonPlaceholderColor = System.Drawing.Color.White
        Me.SiticoneButtonTextbox1.ButtonPlaceholderFont = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.SiticoneButtonTextbox1.ButtonPressColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.SiticoneButtonTextbox1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.SiticoneButtonTextbox1.FocusBorderColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox1.FocusImage = Nothing
        Me.SiticoneButtonTextbox1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(163, Byte), Integer))
        Me.SiticoneButtonTextbox1.HoverBorderColor = System.Drawing.Color.Gray
        Me.SiticoneButtonTextbox1.HoverImage = Nothing
        Me.SiticoneButtonTextbox1.IdleImage = Nothing
        Me.SiticoneButtonTextbox1.Location = New System.Drawing.Point(550, 4)
        Me.SiticoneButtonTextbox1.Name = "SiticoneButtonTextbox1"
        Me.SiticoneButtonTextbox1.PlaceholderColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.SiticoneButtonTextbox1.PlaceholderFocusColor = System.Drawing.Color.FromArgb(CType(CType(79, Byte), Integer), CType(CType(70, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.SiticoneButtonTextbox1.PlaceholderFont = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.SiticoneButtonTextbox1.PlaceholderText = "Search?"
        Me.SiticoneButtonTextbox1.ReadOnlyColors.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.SiticoneButtonTextbox1.ReadOnlyColors.BorderColor = System.Drawing.Color.FromArgb(CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer), CType(CType(200, Byte), Integer))
        Me.SiticoneButtonTextbox1.ReadOnlyColors.ButtonColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.SiticoneButtonTextbox1.ReadOnlyColors.ButtonPlaceholderColor = System.Drawing.Color.Gray
        Me.SiticoneButtonTextbox1.ReadOnlyColors.PlaceholderColor = System.Drawing.Color.FromArgb(CType(CType(150, Byte), Integer), CType(CType(150, Byte), Integer), CType(CType(150, Byte), Integer))
        Me.SiticoneButtonTextbox1.ReadOnlyColors.TextColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer), CType(CType(100, Byte), Integer))
        Me.SiticoneButtonTextbox1.RippleColor = System.Drawing.Color.White
        Me.SiticoneButtonTextbox1.Size = New System.Drawing.Size(161, 43)
        Me.SiticoneButtonTextbox1.TabIndex = 17
        Me.SiticoneButtonTextbox1.TextColor = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(163, Byte), Integer))
        Me.SiticoneButtonTextbox1.TextContent = ""
        Me.SiticoneButtonTextbox1.TextLeftMargin = 0
        Me.SiticoneButtonTextbox1.TopLeftCornerRadius = 8
        Me.SiticoneButtonTextbox1.TopRightCornerRadius = 8
        '
        'dgv_transactions
        '
        Me.dgv_transactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv_transactions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgv_transactions.Location = New System.Drawing.Point(10, 64)
        Me.dgv_transactions.Name = "dgv_transactions"
        Me.dgv_transactions.Size = New System.Drawing.Size(738, 462)
        Me.dgv_transactions.TabIndex = 12
        '
        'TransactionContent
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel3)
        Me.Name = "TransactionContent"
        Me.Size = New System.Drawing.Size(758, 536)
        Me.Panel3.ResumeLayout(False)
        Me.SiticonePanel2.ResumeLayout(False)
        CType(Me.dgv_transactions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SiticoneDashboardCard1 As SiticoneNetFrameworkUI.SiticoneDashboardCard
    Friend WithEvents Panel3 As Panel
    Friend WithEvents dgv_transactions As DataGridView
    Friend WithEvents SiticonePanel2 As SiticonePanel
    Friend WithEvents SiticoneButtonTextbox1 As SiticoneButtonTextbox
End Class
