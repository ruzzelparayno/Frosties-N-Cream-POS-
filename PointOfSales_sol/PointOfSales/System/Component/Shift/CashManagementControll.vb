Public Class CashManagementControll
    Dim cashManager As CashManagementControll = Dashboard.cashInstance

    ' -------------------------
    ' Internal state (avoid relying on labels)
    ' -------------------------
    Private _shiftStartCash As Decimal = 0D
    Private _cashPayments As Decimal = 0D      ' cash payments only
    Private _gcashPayments As Decimal = 0D     ' gcash payments only
    Private _currentRefund As Decimal = 0D     ' cumulative refund
    Private _totalDiscounts As Decimal = 0D
    Private _grossSales As Decimal = 0D

    ' Parent reference (unchanged)
    Public Property ParentShift As ShiftContent

    Private _currentShiftID As Integer = 0
    Public ReadOnly Property CurrentShiftID As Integer
        Get
            Return _currentShiftID
        End Get
    End Property


    Public Sub StartShift(shiftID As Integer)
        _currentShiftID = shiftID
    End Sub

    ' -------------------------
    ' Load / Init
    ' -------------------------
    Private Sub cashmanagementcontroll_load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeState()
        UpdateAllLabels()
        clearalllabels()
    End Sub

    Private Sub InitializeState()
        _shiftStartCash = 0D
        _cashPayments = 0D
        _gcashPayments = 0D
        _currentRefund = 0D
        _totalDiscounts = 0D
        _grossSales = 0D
    End Sub

    ' -------------------------
    ' Open Shift button (existing behavior preserved)
    ' -------------------------
    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click
        If SiticoneTextBox1.Text.Trim() = "" Then
            MessageBox.Show("Please enter a shift amount first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        StartShift(1)

        MessageBox.Show("Welcome to Shift!", "Shift Started", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Dim shiftValue As Decimal
        If Decimal.TryParse(SiticoneTextBox1.Text, shiftValue) Then
            _shiftStartCash = shiftValue
        Else
            _shiftStartCash = 0D
        End If

        UpdateAllLabels()

        SiticoneTextBox1.Text = ""

        Dim shiftForm As ShiftContent = Dashboard.shiftInstance
        Dim mainDashboard As Dashboard = Application.OpenForms().OfType(Of Dashboard)().FirstOrDefault()

        If shiftForm Is Nothing Then
            shiftForm = New ShiftContent()
            Dashboard.shiftInstance = shiftForm
            If mainDashboard Is Nothing Then
                MessageBox.Show("Dashboard is not running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End If

        If shiftForm IsNot Nothing Then
            shiftForm.Panel4.Show()
            shiftForm.btnCategory.Visible = True
            shiftForm.PictureBox1.Visible = False
            shiftForm.SiticoneButton3.Hide()
        End If

        Dashboard.posInstance.EnableMainButton()
        Dashboard.SetNavbarItemEnabled(8, False)
        lbl_datet.Text = Date.Now.ToString("yyyy-MM-dd HH:mm:ss")
        clearopenshift()
        displayalllabels()
    End Sub

    ' -------------------------
    ' Public reset / reseet
    ' -------------------------
    Public Sub Reseeet()
        InitializeState()
        UpdateAllLabels()
    End Sub

    ' -------------------------
    ' Cash / GCash updates
    ' -------------------------
    Public Sub UpdateCashManager(amount As Decimal, modeOfPayment As String)
        Select Case modeOfPayment
            Case "Cash"
                UpdateCashTotals(amount, isCash:=True)
            Case "GCash"
                UpdateCashTotals(amount, isCash:=False)
            Case Else
        End Select
    End Sub

    Public Sub UpdateCashTotals(amount As Decimal, isCash As Boolean)
        Try
            If isCash Then
                _cashPayments += amount
            Else
                _gcashPayments += amount
            End If
            UpdateAllLabels()
        Catch ex As Exception
            MessageBox.Show("Error updating totals: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------
    ' Update shift sales (gross sales & discounts)
    ' -------------------------
    Public Sub UpdateShiftSales(ByVal refundValue As Decimal, ByVal discountValue As Decimal, ByVal grossSaleValue As Decimal)
        _totalDiscounts += discountValue
        _grossSales += grossSaleValue
        UpdateAllLabels()
    End Sub

    ' -------------------------
    ' SetRefundAmount (single refund overwrite)
    ' -------------------------
    Public Sub SetRefundAmount(currentRefund As Decimal)
        _currentRefund = currentRefund
        UpdateAllLabels()
    End Sub

    ' -------------------------
    ' AddRefund: cumulative refund
    ' -------------------------
    Public Sub AddRefund(refundAmount As Decimal)
        ' Add new refund to cumulative total
        _currentRefund += refundAmount

        ' Update refund labels
        lbl_refund1.Text = "₱" & _currentRefund.ToString("N2")
        lbl_srefunds.Text = "₱" & _currentRefund.ToString("N2")

        ' Deduct refund from Extra Cash Available
        Dim extraCash As Decimal = _shiftStartCash + _cashPayments - _currentRefund
        lbl_eca.Text = "₱" & extraCash.ToString("N2")

        ' Update net cash
        lbl_ncash.Text = "₱" & (_cashPayments - _currentRefund).ToString("N2")

        ' Optional: update net sales after refund
        Dim netSales As Decimal = (_grossSales - _totalDiscounts - _currentRefund)
        If netSales < 0D Then netSales = 0D
        lbl_ns.Text = "₱" & netSales.ToString("N2")
    End Sub

    ' -------------------------
    ' UpdateAllLabels
    ' -------------------------
    Private Sub UpdateAllLabels()
        lbl_sc1.Text = "₱" & _shiftStartCash.ToString("N2")
        lbl_cashp1.Text = "₱" & _cashPayments.ToString("N2")        ' Cash Payments
        lbl_refund1.Text = "₱" & _currentRefund.ToString("N2")      ' Cash Refund
        lbl_srefunds.Text = "₱" & _currentRefund.ToString("N2")     ' Refunds
        lbl_gs.Text = "₱" & _grossSales.ToString("N2")             ' Gross Sales
        lbl_sdiscounts.Text = "₱" & _totalDiscounts.ToString("N2")  ' Discounts

        Dim extraCash As Decimal = _shiftStartCash + _cashPayments - _currentRefund
        lbl_eca.Text = "₱" & extraCash.ToString("N2")              ' Expected Cash Amount

        Dim netSales As Decimal = (_grossSales - _totalDiscounts - _currentRefund)
        If netSales < 0D Then netSales = 0D
        lbl_ns.Text = "₱" & netSales.ToString("N2")                ' Net Sales

        lbl_ncash.Text = "₱" & (_cashPayments - _currentRefund).ToString("N2")  ' Cash
        lbl_ngcash.Text = "₱" & _gcashPayments.ToString("N2")                  ' GCash
    End Sub


    ' -------------------------
    ' ComputeTotal
    ' -------------------------
    Public Sub ComputeTotal()
        UpdateAllLabels()
    End Sub

    ' -------------------------
    ' Visibility utilities
    ' -------------------------
    Public Sub clearopenshift()
        SiticoneButton1.Hide()
        SiticoneTextBox1.Hide()
        Label1.Hide()
        Label2.Hide()
    End Sub

    Public Sub clearalllabels()
        lbl_datet.Hide()
        lbl_sc1.Hide()
        lbl_cashp1.Hide()
        lbl_refund1.Hide()
        lbl_eca.Hide()
        Label17.Hide()
        Label14.Hide()
        Label12.Hide()
        Label11.Hide()
        Label10.Hide()
        Label6.Hide()
        Label5.Hide()
        Label4.Hide()
        Label3.Hide()
        lbl_gs.Hide()
        lbl_srefunds.Hide()
        lbl_sdiscounts.Hide()
        lbl_nsn.Hide()
        lbl_cashn.Hide()
        lbl_gcashn.Hide()
        lbl_ns.Hide()
        lbl_ncash.Hide()
        lbl_ngcash.Hide()
    End Sub

    Public Sub displayalllabels()
        lbl_datet.Show()
        lbl_sc1.Show()
        lbl_cashp1.Show()
        lbl_refund1.Show()
        lbl_eca.Show()
        Label17.Show()
        Label14.Show()
        Label12.Show()
        Label11.Show()
        Label10.Show()
        Label6.Show()
        Label5.Show()
        Label4.Show()
        Label3.Show()
        lbl_gs.Show()
        lbl_srefunds.Show()
        lbl_sdiscounts.Show()
        lbl_nsn.Show()
        lbl_cashn.Show()
        lbl_gcashn.Show()
        lbl_ns.Show()
        lbl_ncash.Show()
        lbl_ngcash.Show()
    End Sub
    Public Sub UpdateDiscountLabel(discountValue As Decimal)
        Try
            lbl_sdiscounts.Text = "₱" & discountValue.ToString("N2")
        Catch ex As Exception
            MessageBox.Show("Error updating discount label: " & ex.Message)
        End Try
    End Sub

    ' -------------------------
    ' ResetShift
    ' -------------------------
    Public Sub ResetShift()
        InitializeState()
        UpdateAllLabels()
        SiticoneButton1.Visible = True
        SiticoneTextBox1.Visible = True
        Label1.Visible = True
        Label2.Visible = True

        If ParentShift IsNot Nothing Then
            ParentShift.btnCategory.Visible = False
            ParentShift.PictureBox1.Visible = False
            ParentShift.SiticoneButton3.Visible = True
        End If

        lbl_cashn.Visible = False
        lbl_datet.Visible = False
        lbl_cashp1.Visible = False
        lbl_refund1.Visible = False
        lbl_eca.Visible = False
        Label17.Visible = False
        Label5.Visible = False
        Label4.Visible = False
        Label3.Visible = False
        Label14.Visible = False
        Label12.Visible = False
        Label11.Visible = False
        Label10.Visible = False
        Label6.Visible = False
        lbl_gs.Visible = False
        lbl_srefunds.Visible = False
        lbl_sdiscounts.Visible = False
        lbl_nsn.Visible = False
        lbl_gcashn.Visible = False
        lbl_ns.Visible = False
        lbl_ncash.Visible = False
        lbl_ngcash.Visible = False
        lbl_sc1.Visible = False
    End Sub

    Public Sub ApplySale(amount As Decimal, paymentMode As String, discount As Decimal)
        ' Update payments
        If paymentMode = "Cash" Then
            _cashPayments += amount
        ElseIf paymentMode = "GCash" Then
            _gcashPayments += amount
        End If

        ' Update sales
        _grossSales += amount
        _totalDiscounts += discount

        ' Refresh UI
        UpdateAllLabels()
        displayalllabels()
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Dashboard.cashInstance = Me
    End Sub


End Class
