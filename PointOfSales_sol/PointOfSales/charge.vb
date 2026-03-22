Imports MySql.Data.MySqlClient
Imports System.Text
Imports System.Runtime.InteropServices

Public Class Charge
    ' Sub-forms (your other forms)
    Public Shared cashForm As New Charge_Cash()
    Public Shared gcashForm As New Charge_Gcash()
    Public Shared successForm As New Charge_Success()

    Private originalTotal As Decimal = 0D

    Private Sub Charge_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Ensure Dashboard POS exists
        Dim posForm As PosControl = Dashboard.posInstance
        If posForm Is Nothing Then
            MessageBox.Show("POS screen not found. Please open the POS module first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Embed child forms into your siticoneflatpanel1
        Try
            ' Prepare cash form
            cashForm.Dock = DockStyle.Fill

            ' Prepare gcash form
            gcashForm.Dock = DockStyle.Fill

            ' Clear panel and add both forms (we'll show/hide)
            SiticoneFlatPanel1.Controls.Clear()
            SiticoneFlatPanel1.Controls.Add(cashForm)
            SiticoneFlatPanel1.Controls.Add(gcashForm)

            cashForm.Show()
            gcashForm.Hide()
        Catch ex As Exception
            ' If embed fails, continue (forms might already be added)
        End Try

        ' Wire text changed events from child forms to local handlers
        Try
            AddHandler cashForm.txt_cashs.TextChanged, AddressOf txt_cashs_TextChanged
            AddHandler gcashForm.txt_ref.TextChanged, AddressOf txt_ref_TextChanged
        Catch ex As Exception
            ' ignore if child controls not present
        End Try

        ' Find lbl_total inside pos (recursive)
        Dim lblTotalInPanel As Label = FindLabelRecursive(posForm, "lbl_total")

        If lblTotalInPanel IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(lblTotalInPanel.Text) Then
            Dim cleanValue As String = lblTotalInPanel.Text.Replace("₱", "").Replace(",", "").Trim()
            Decimal.TryParse(cleanValue, originalTotal)
        Else
            originalTotal = 0D
        End If

        lbl_totalpaid.Text = "₱" & originalTotal.ToString("N2")

        ' Initialize child controls
        Try
            cashForm.txt_cashs.Clear()
            cashForm.lbl_change.Text = "₱0.00"
            cashForm.lbl_totalP.Text = "₱0.00"
        Catch ex As Exception
        End Try
        Try
            gcashForm.txt_ref.Hide() ' keep reference hidden until GCash selected
            ' initialize GCash total textbox and make it read-only
            gcashForm.txt_TotalP.Text = lbl_totalpaid.Text
            gcashForm.txt_TotalP.Enabled = False
        Catch ex As Exception
        End Try
        ' --- NEW: Ensure only one discount checkbox can be checked at a time ---
        AddHandler cb_employee.CheckedChanged, AddressOf SingleCheckDiscount
        AddHandler cb_pwd.CheckedChanged, AddressOf SingleCheckDiscount
        AddHandler cb_senior.CheckedChanged, AddressOf SingleCheckDiscount
    End Sub

    Private Sub SingleCheckDiscount(sender As Object, e As EventArgs)
        ' Ensure only the checkbox that was clicked remains checked
        Dim changedCb As CheckBox = TryCast(sender, CheckBox)
        If changedCb Is Nothing Then Return

        If changedCb.Checked Then
            ' Uncheck the others
            If changedCb IsNot cb_employee Then cb_employee.Checked = False
            If changedCb IsNot cb_pwd Then cb_pwd.Checked = False
            If changedCb IsNot cb_senior Then cb_senior.Checked = False
        End If

        ' Reapply discount
        ApplyDiscount()
    End Sub


    ' Recursive label finder (unchanged)
    Private Function FindLabelRecursive(parent As Control, labelName As String) As Label
        For Each ctrl As Control In parent.Controls
            If TypeOf ctrl Is Label AndAlso ctrl.Name = labelName Then
                Return DirectCast(ctrl, Label)
            ElseIf ctrl.HasChildren Then
                Dim found As Label = FindLabelRecursive(ctrl, labelName)
                If found IsNot Nothing Then Return found
            End If
        Next
        Return Nothing
    End Function

    ' Refresh totals each time form becomes visible
    Public Sub RefreshTotalPaid()
        cb_employee.Checked = False
        cb_pwd.Checked = False
        cb_senior.Checked = False

        ' Clear child inputs
        Try
            cashForm.txt_cashs.Clear()
            gcashForm.txt_ref.Clear()
        Catch ex As Exception
        End Try

        Dim subtotal As Decimal = 0D
        Dim posForm As PosControl = Dashboard.posInstance

        If posForm IsNot Nothing Then
            Dim lblTotalInPanel As Label = FindLabelRecursive(posForm, "lbl_total")
            If lblTotalInPanel IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(lblTotalInPanel.Text) Then
                Dim cleanValue As String = lblTotalInPanel.Text.Replace("₱", "").Replace(",", "").Trim()
                Decimal.TryParse(cleanValue, subtotal)
            End If
        End If

        originalTotal = subtotal
        Me.Tag = subtotal
        lbl_totalpaid.Text = "₱" & Math.Round(subtotal, 2).ToString("N2")

        ' reflect into child form if needed
        ' reflect into child form if needed
        Try
            cashForm.lbl_totalP.Text = "₱" & Math.Round(subtotal, 2).ToString("N2")
        Catch ex As Exception
        End Try

        ' reflect into gcash textbox (read-only)
        Try
            gcashForm.txt_TotalP.Text = "₱" & Math.Round(subtotal, 2).ToString("N2")
            gcashForm.txt_TotalP.Enabled = False
        Catch ex As Exception
        End Try

    End Sub

    ' When SiticoneRadioButton1 (Cash) is selected
    Private Sub SiticoneRadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles SiticoneRadioButton1.CheckedChanged
        If SiticoneRadioButton1.Checked Then
            ' Show cash panel form, hide gcash form
            Try
                cashForm.Show()
                gcashForm.Hide()
            Catch ex As Exception
            End Try

            ' Show/hide appropriate fields
            Try
                gcashForm.txt_ref.Hide()
            Catch ex As Exception
            End Try
            Try
                cashForm.txt_cashs.Show()
            Catch ex As Exception
            End Try

            ' clear or keep gcash total hidden (optional)
            Try
                gcashForm.txt_TotalP.Text = ""
                gcashForm.txt_TotalP.Enabled = False
            Catch ex As Exception
            End Try
        End If
    End Sub


    ' When SiticoneRadioButton2 (GCash) is selected
    Private Sub SiticoneRadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles SiticoneRadioButton2.CheckedChanged
        If SiticoneRadioButton2.Checked Then
            Try
                gcashForm.Show()
                cashForm.Hide()
            Catch ex As Exception
            End Try

            Try
                gcashForm.txt_ref.Show()
            Catch ex As Exception
            End Try
            Try
                cashForm.txt_cashs.Hide()
                cashForm.txt_cashs.Clear()
            Catch ex As Exception
            End Try

            ' AUTO DISPLAY TOTALPAID LIKE CASH (and disable editing)
            Try
                ' use lbl_totalpaid which already contains the formatted ₱ amount
                gcashForm.txt_TotalP.Text = lbl_totalpaid.Text
                gcashForm.txt_TotalP.Enabled = False
            Catch ex As Exception
            End Try
        End If
    End Sub


    ' Discount checkbox handler
    Private Sub DiscountChanged(sender As Object, e As EventArgs)
        ApplyDiscount()
    End Sub

    Private Sub ApplyDiscount()
        Dim total As Decimal = originalTotal
        If cb_employee.Checked OrElse cb_pwd.Checked OrElse cb_senior.Checked Then
            total = Math.Round(total * 0.8D, 2)
        End If

        lbl_totalpaid.Text = "₱" & total.ToString("N2")

        ' update child total display (cash)
        Try
            cashForm.lbl_totalP.Text = "₱" & total.ToString("N2")
        Catch ex As Exception
        End Try

        ' update GCash textbox (read-only)
        Try
            gcashForm.txt_TotalP.Text = "₱" & total.ToString("N2")
            gcashForm.txt_TotalP.Enabled = False
        Catch ex As Exception
        End Try

        CalculateChange()
    End Sub


    ' proxy for cash text changed (wired via AddHandler)
    Private Sub txt_cashs_TextChanged(sender As Object, e As EventArgs)
        CalculateChange()
    End Sub
    ' Inside Charge class
    Private Sub txt_ref_TextChanged(sender As Object, e As EventArgs)
        Dim tb As TextBox = gcashForm.txt_ref
        Dim reference As String = tb.Text.Trim()

        ' Limit to max 12 characters
        If reference.Length > 12 Then
            tb.Text = reference.Substring(0, 12)
            tb.SelectionStart = tb.Text.Length
        End If
    End Sub




    Private Sub CalculateChange()
        Dim cash As Decimal = 0D
        Dim totalPaid As Decimal = 0D

        Dim cashText As String = ""
        Try
            cashText = cashForm.txt_cashs.Text
        Catch ex As Exception
            cashText = ""
        End Try

        Decimal.TryParse(cashText.Replace("₱", "").Replace(",", "").Trim(), cash)
        Decimal.TryParse(lbl_totalpaid.Text.Replace("₱", "").Replace(",", "").Trim(), totalPaid)

        Dim change As Decimal = cash - totalPaid

        If change >= 0D Then
            Try
                cashForm.lbl_change.Text = "₱" & change.ToString("N2")
            Catch ex As Exception
            End Try
        Else
            Try
                cashForm.lbl_change.Text = "₱0.00"
            Catch ex As Exception
            End Try
        End If
    End Sub

    ' Main charge button (handles SiticoneButton1)
    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click
        ' forward to the main charge logic
        ProcessCharge()
    End Sub

    ' Extracted charge logic to keep things tidy
    Private Sub ProcessCharge()
        Dim cash As Decimal = 0D
        Dim totalPaid As Decimal = 0D
        Dim cashText As String = ""
        Try
            cashText = cashForm.txt_cashs.Text.Replace("₱", "").Replace(",", "").Trim()
        Catch ex As Exception
            cashText = ""
        End Try

        Decimal.TryParse(lbl_totalpaid.Text.Replace("₱", "").Replace(",", "").Trim(), totalPaid)

        If totalPaid <= 0 Then
            MessageBox.Show("Invalid or missing total amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim modeOfPayment As String = ""
        Dim reference As String = ""
        Try
            reference = gcashForm.txt_ref.Text.Trim()
        Catch ex As Exception
            reference = ""
        End Try

        Dim posForm As PosControl = Dashboard.posInstance
        If posForm Is Nothing Then
            MessageBox.Show("POS form not found inside Dashboard!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Payment mode validation
        If SiticoneRadioButton1.Checked Then
            modeOfPayment = "Cash"
            If Not Decimal.TryParse(cashText, cash) Then
                MessageBox.Show("Please enter a valid cash amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            If cash < totalPaid Then
                MessageBox.Show("Cash is not enough!" & vbCrLf &
                                 "Total: ₱" & totalPaid.ToString("N2") & vbCrLf &
                                 "Cash Entered: ₱" & cash.ToString("N2"),
                                 "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            Dim change As Decimal = cash - totalPaid

            Try
                cashForm.lbl_change.Text = "₱" & change.ToString("N2")
            Catch ex As Exception
            End Try

        ElseIf SiticoneRadioButton2.Checked Then
            modeOfPayment = "GCash"
            If String.IsNullOrWhiteSpace(reference) Then
                MessageBox.Show("Please enter GCash Reference Number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' duplicate reference check
            Try
                Using conn As New MySqlConnection("server=localhost;userid=root;password=;database=pos")
                    conn.Open()
                    Dim checkQuery As String = "SELECT COUNT(*) FROM sales WHERE Reference = @Reference"
                    Using checkCmd As New MySqlCommand(checkQuery, conn)
                        checkCmd.Parameters.AddWithValue("@Reference", reference)
                        Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                        If count > 0 Then
                            MessageBox.Show("This GCash Reference Number is already used!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Sub
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error checking reference number: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End Try

        Else
            MessageBox.Show("Please select a mode of payment!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' ----------------- SUCCESS ACTIONS -----------------
        ' 1. Update Cash Management Totals
        Dim cashMgr As CashManagementControll = Nothing
        Dim shiftForm As ShiftContent = Dashboard.shiftInstance

        If shiftForm IsNot Nothing Then
            Try
                If shiftForm.Panel2.Controls.Count > 0 AndAlso TypeOf shiftForm.Panel2.Controls(0) Is CashManagementControll Then
                    cashMgr = DirectCast(shiftForm.Panel2.Controls(0), CashManagementControll)
                End If
            Catch ex As Exception
            End Try
        End If

        If cashMgr IsNot Nothing Then
            Try
                Dim discountAmt As Decimal = 0D
                Dim netTotal As Decimal = originalTotal   ' Default no discount

                ' Apply discount if any checkbox is checked
                If cb_employee.Checked OrElse cb_pwd.Checked OrElse cb_senior.Checked Then
                    discountAmt = Math.Round(originalTotal * 0.2D, 2)
                    netTotal = originalTotal - discountAmt   ' ✔ CORRECT
                End If

                ' Update Cash Manager (payment + MOP)
                cashMgr.UpdateCashManager(totalPaid, modeOfPayment)

                ' ✔ Update shift sales correctly
                '   Gross = original
                '   Discount = discountAmt
                '   Net = after discount
                cashMgr.UpdateShiftSales(
            refundValue:=0D,
            discountValue:=discountAmt,
            grossSaleValue:=originalTotal
        )

                ' Update discount label
                cashMgr.UpdateDiscountLabel(discountAmt)



            Catch ex As Exception
            End Try
        End If



        ' 2. Save to DB
        SaveToDatabase(posForm, modeOfPayment, reference, totalPaid)
        ' ✅ Refresh Dashboard content after successful transaction
        ' ✅ Refresh dashboard after successful sale
        Try
            If Dashboard.DashboardContent IsNot Nothing Then
                Dashboard.DashboardContent.RefreshDashboard()
            End If
        Catch ex As Exception
            MessageBox.Show("Dashboard refresh failed: " & ex.Message)
        End Try

        ' 3. Print and clear
        MessageBox.Show("Payment successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        PrintReceipt()
        Try
            posForm.ClearTransaction()
        Catch ex As Exception
        End Try

        Me.Hide()
    End Sub

    Private Sub SaveToDatabase(posForm As PosControl, modeOfPayment As String, reference As String, totalPaid As Decimal)
        Try
            Using conn As New MySqlConnection("server=localhost;userid=root;password=;database=pos")
                conn.Open()

                Using tr As MySqlTransaction = conn.BeginTransaction()
                    Try
                        Dim itemsList As New List(Of String)
                        Dim totalAmount As Decimal = 0D
                        Dim totalVAT As Decimal = 0D
                        Dim totalSubTotal As Decimal = 0D

                        ' Determine discount percentage once (assuming same discount for all items)
                        Dim discountType As String = "None"
                        Dim discountPercent As Decimal = 0D
                        If cb_employee.Checked Then
                            discountType = "EMPLOYEE"
                            discountPercent = 0.2D
                        ElseIf cb_pwd.Checked Then
                            discountType = "PWD"
                            discountPercent = 0.2D
                        ElseIf cb_senior.Checked Then
                            discountType = "SENIOR"
                            discountPercent = 0.2D
                        End If

                        ' Loop through items to build formatted string and calculate totals
                        For i As Integer = 0 To posForm.Guna2DataGridView1.Rows.Count - 1
                            Dim row = posForm.Guna2DataGridView1.Rows(i)
                            If row.IsNewRow Then Continue For

                            Dim qty As Integer = 0
                            If row.Cells(0).Value IsNot Nothing Then Integer.TryParse(row.Cells(0).Value.ToString(), qty)

                            Dim pname As String = ""
                            Dim productCell = row.Cells("ProductName")
                            If productCell IsNot Nothing AndAlso productCell.Value IsNot Nothing Then pname = productCell.Value.ToString()

                            Dim pricePerUnit As Decimal = 0D
                            If row.Cells(2).Value IsNot Nothing Then Decimal.TryParse(row.Cells(2).Value.ToString(), pricePerUnit)

                            ' Add to items list in format "1x Spaghetti"
                            If qty > 0 AndAlso Not String.IsNullOrEmpty(pname) Then
                                itemsList.Add($"{qty}x {pname}")
                            End If

                            ' Calculate discounted price, VAT, and subtotal
                            Dim discountedPrice As Decimal = pricePerUnit * (1 - discountPercent)
                            Dim vatAmount As Decimal = discountedPrice * 0.12D
                            Dim subTotal As Decimal = discountedPrice - vatAmount

                            ' Accumulate totals
                            totalAmount += discountedPrice * qty
                            totalVAT += vatAmount * qty
                            totalSubTotal += subTotal * qty

                            ' Reduce stock
                            Using updateCmd As New MySqlCommand("
                            UPDATE products 
                            SET StockQuantity = StockQuantity - @SoldQty 
                            WHERE ProductName = @ProductName
                        ", conn, tr)
                                updateCmd.Parameters.AddWithValue("@SoldQty", qty)
                                updateCmd.Parameters.AddWithValue("@ProductName", pname)
                                updateCmd.ExecuteNonQuery()
                            End Using

                            ' Notify ProductContent
                            If ProductContent.Instance IsNot Nothing AndAlso Not String.IsNullOrEmpty(pname) Then
                                ProductContent.Instance.NotifyStockChangedByName(pname)
                            End If
                        Next

                        ' Combine items into single string
                        Dim formattedItems As String = String.Join(Environment.NewLine, itemsList)

                        ' Insert single record into sales table
                        Using cmd As New MySqlCommand("
                        INSERT INTO sales 
                        (TicketNumber, SaleDate, ProductName, Price, Quantity, SubTotal, VAT, TotalAmount, ModeOfPayment, Reference, DiscountType, Status)
                        VALUES 
                        (@TicketNumber, @SaleDate, @ProductName, @Price, @Quantity, @SubTotal, @VAT, @TotalAmount, @ModeOfPayment, @Reference, @DiscountType, @Status)
                    ", conn, tr)
                            cmd.Parameters.AddWithValue("@TicketNumber", If(posForm.lbl_tickets.Text, ""))
                            cmd.Parameters.AddWithValue("@SaleDate", DateTime.Now)
                            cmd.Parameters.AddWithValue("@ProductName", formattedItems)  ' all items grouped
                            cmd.Parameters.AddWithValue("@Price", totalAmount)           ' total price
                            cmd.Parameters.AddWithValue("@Quantity", 1)                  ' grouped as 1 sale
                            cmd.Parameters.AddWithValue("@SubTotal", totalSubTotal)
                            cmd.Parameters.AddWithValue("@VAT", totalVAT)
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount)
                            cmd.Parameters.AddWithValue("@ModeOfPayment", modeOfPayment)
                            cmd.Parameters.AddWithValue("@Reference", reference)
                            cmd.Parameters.AddWithValue("@DiscountType", discountType)
                            cmd.Parameters.AddWithValue("@Status", "Completed")
                            cmd.ExecuteNonQuery()
                        End Using

                        tr.Commit()



                    Catch exInner As Exception
                        tr.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            ' Reload inventory
            If Dashboard.productInstance IsNot Nothing Then
                Dashboard.productInstance.LoadInventory()
            End If
        Catch ex As Exception
            MessageBox.Show("Error saving sales or updating stock: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub






    ' When form visibility changes, refresh totals
    Private Sub Charge_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible Then RefreshTotalPaid()
    End Sub


    ' --- ESC/POS raw printing function ---
    <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function OpenPrinter(ByVal szPrinter As String, ByRef hPrinter As IntPtr, ByVal pd As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function ClosePrinter(ByVal hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function StartDocPrinter(ByVal hPrinter As IntPtr, ByVal level As Integer, ByRef di As DOCINFOA) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function EndDocPrinter(ByVal hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function StartPagePrinter(ByVal hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function EndPagePrinter(ByVal hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Private Shared Function WritePrinter(ByVal hPrinter As IntPtr, ByVal pBytes As IntPtr, ByVal dwCount As Integer, ByRef dwWritten As Integer) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Private Structure DOCINFOA
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDataType As String
    End Structure

    Private Shared Function SendStringToPrinter(ByVal szPrinterName As String, ByVal szString As String) As Boolean
        Dim pBytes As IntPtr
        ' Convert string to bytes using CP437 (handles ñ, á, é, etc.)
        Dim bytes() As Byte = Encoding.GetEncoding(437).GetBytes(szString)
        pBytes = Marshal.AllocCoTaskMem(bytes.Length)
        Marshal.Copy(bytes, 0, pBytes, bytes.Length)

        Dim hPrinter As IntPtr
        Dim di As DOCINFOA = New DOCINFOA()
        di.pDocName = "Receipt"
        di.pDataType = "RAW"

        If OpenPrinter(szPrinterName, hPrinter, IntPtr.Zero) Then
            If StartDocPrinter(hPrinter, 1, di) Then
                If StartPagePrinter(hPrinter) Then
                    Dim dwWritten As Integer = 0
                    WritePrinter(hPrinter, pBytes, bytes.Length, dwWritten)
                    EndPagePrinter(hPrinter)
                End If
                EndDocPrinter(hPrinter)
            End If
            ClosePrinter(hPrinter)
        End If
        Marshal.FreeCoTaskMem(pBytes)
        Return True
    End Function

    ' --- Main function to print receipt ---
    Public Sub PrintReceipt()
        Dim posForm As PosControl = Dashboard.posInstance
        If posForm Is Nothing Then
            MessageBox.Show("POS form not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim modePayment As String = If(SiticoneRadioButton1.Checked, "Cash", If(SiticoneRadioButton2.Checked, "GCash", "N/A"))
        Dim reference As String = If(modePayment = "GCash", gcashForm.txt_ref.Text.Trim(), "")

        ' --- Calculate total price from DataGridView (unit price includes VAT) ---
        Dim totalPrice As Decimal = 0
        For i As Integer = 0 To posForm.Guna2DataGridView1.Rows.Count - 1
            If posForm.Guna2DataGridView1.Rows(i).IsNewRow Then Continue For
            Dim qty As Integer = Convert.ToInt32(posForm.Guna2DataGridView1.Rows(i).Cells(0).Value)
            Dim price As Decimal = Convert.ToDecimal(posForm.Guna2DataGridView1.Rows(i).Cells(2).Value)
            totalPrice += price * qty
        Next

        ' --- VAT and subtotal ---
        Dim vatRate As Decimal = 0.12D
        Dim vatAmount As Decimal = Math.Round(totalPrice * vatRate, 2, MidpointRounding.AwayFromZero)
        Dim subtotal As Decimal = Math.Round(totalPrice - vatAmount, 2, MidpointRounding.AwayFromZero)
        Dim totalAmount As Decimal = totalPrice

        ' --- Determine discount ---
        Dim discountRate As Decimal = 0D
        Dim discountType As String = ""
        If cb_employee.Checked Then
            discountRate = 0.2D
            discountType = "Emp"
        ElseIf cb_senior.Checked Then
            discountRate = 0.2D
            discountType = "SC"
        ElseIf cb_pwd.Checked Then
            discountRate = 0.2D
            discountType = "PWD"
        End If

        Dim discountAmount As Decimal = Math.Round(totalAmount * discountRate, 2)
        Dim amountDue As Decimal = Math.Round(totalAmount - discountAmount, 2)

        ' --- Cash and change ---
        Dim cashPaid As Decimal = 0
        Dim change As Decimal = 0
        If modePayment = "Cash" Then
            Decimal.TryParse(cashForm.txt_cashs.Text.Replace("₱", "").Replace(",", "").Trim(), cashPaid)
            change = Math.Max(cashPaid - amountDue, 0)
        Else
            cashPaid = amountDue
            change = 0
        End If

        Dim line As String = "================================"
        Dim receipt As New StringBuilder()
        Dim maxChars As Integer = 32 ' 58mm printer approx 32 chars per line


        ' --- ORDER LIST WITH SAME HEADER ---
        receipt.AppendLine("Frosties N' Cream".PadLeft((maxChars + "Frosties N' Cream".Length) \ 2))
        receipt.AppendLine("10 Rosal St. Doña Manuela Subd.".PadLeft((maxChars + "10 Rosal St. Doña Manuela Subd.".Length) \ 2))
        receipt.AppendLine("Pamplona 3,  1740 Las Piñas.".PadLeft((maxChars + "Pamplona 3,  1740 Las Piñas.".Length) \ 2))
        receipt.AppendLine("Contact: 0917 557 5485".PadLeft((maxChars + "Contact: 0917 557 5485".Length) \ 2))
        receipt.AppendLine(line)

        ' Order title
        receipt.AppendLine("ORDER LIST".PadLeft((maxChars + "ORDER LIST".Length) \ 2))
        receipt.AppendLine(line)

        ' --- Column widths ---
        Dim qtyWidth As Integer = 6
        Dim productWidth As Integer = maxChars - qtyWidth

        ' --- Header centered as a block ---
        Dim headerText As String = "QTY".PadRight(qtyWidth) & "PRODUCT".PadRight(productWidth)
        Dim headerPadding As Integer = (maxChars - headerText.Length) \ 2
        receipt.AppendLine(New String(" "c, headerPadding) & headerText)
        receipt.AppendLine(line)

        ' --- Items centered in same block ---
        For i As Integer = 0 To posForm.Guna2DataGridView1.Rows.Count - 1
            If posForm.Guna2DataGridView1.Rows(i).IsNewRow Then Continue For

            Dim qty As String = posForm.Guna2DataGridView1.Rows(i).Cells(0).Value.ToString().PadRight(qtyWidth)
            Dim pname As String = posForm.Guna2DataGridView1.Rows(i).Cells(1).Value.ToString().PadRight(productWidth)

            Dim itemText As String = qty & pname
            Dim itemPadding As Integer = (maxChars - itemText.Length) \ 2

            receipt.AppendLine(New String(" "c, itemPadding) & itemText)
        Next

        receipt.AppendLine(line)
        receipt.AppendLine("Thank you for order!".PadLeft((maxChars + "Thank you for order!".Length) \ 2))
        receipt.AppendLine()
        receipt.AppendLine()

        ' --- Header ---
        receipt.AppendLine("Frosties N' Cream".PadLeft((maxChars + "Frosties N' Cream".Length) \ 2))
        receipt.AppendLine("10 Rosal St. Doña Manuela Subd.".PadLeft((maxChars + "10 Rosal St. Doña Manuela Subd.".Length) \ 2))
        receipt.AppendLine("Pamplona 3,  1740 Las Piñas.".PadLeft((maxChars + "Pamplona 3,  1740 Las Piñas.".Length) \ 2))
        receipt.AppendLine("Contact: 0917 557 5485".PadLeft((maxChars + "Contact: 0917 557 5485".Length) \ 2))
        receipt.AppendLine(line)

        ' --- Ticket info ---
        receipt.AppendLine("Ticket #: " & posForm.lbl_tickets.Text)
        receipt.AppendLine("Date: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        receipt.AppendLine(line)

        ' --- Product header ---
        receipt.AppendLine("QTY".PadRight(4) & "PRODUCT".PadRight(16) & "PRICE".PadLeft(12))
        receipt.AppendLine(line)

        ' --- Items ---
        For i As Integer = 0 To posForm.Guna2DataGridView1.Rows.Count - 1
            If posForm.Guna2DataGridView1.Rows(i).IsNewRow Then Continue For
            Dim qty As Integer = Convert.ToInt32(posForm.Guna2DataGridView1.Rows(i).Cells(0).Value)
            Dim pname As String = posForm.Guna2DataGridView1.Rows(i).Cells(1).Value.ToString()
            Dim price As Decimal = Convert.ToDecimal(posForm.Guna2DataGridView1.Rows(i).Cells(2).Value) * qty

            receipt.AppendLine(qty.ToString().PadRight(4) & pname.PadRight(16) & price.ToString("N2").PadLeft(12))
        Next

        receipt.AppendLine(line)

        ' --- Totals with discount ---
        receipt.AppendLine("Subtotal:".PadRight(20) & subtotal.ToString("N2").PadLeft(12))
        receipt.AppendLine("VAT (12%):".PadRight(20) & vatAmount.ToString("N2").PadLeft(12))
        receipt.AppendLine("Total:".PadRight(20) & totalAmount.ToString("N2").PadLeft(12))

        If discountRate > 0 Then
            Dim discountLabel As String = "Discount (" & discountType & " 20%):"
            receipt.AppendLine(discountLabel.PadRight(20) & discountAmount.ToString("N2").PadLeft(12))
        End If


        receipt.AppendLine("Amount Due:".PadRight(20) & amountDue.ToString("N2").PadLeft(12))

        ' --- Cash and change ---
        If modePayment = "Cash" Then
            receipt.AppendLine("Cash:".PadRight(20) & cashPaid.ToString("N2").PadLeft(12))
            receipt.AppendLine("Change:".PadRight(20) & change.ToString("N2").PadLeft(12))
        End If

        receipt.AppendLine("Mode of Payment: " & modePayment)
        If modePayment = "GCash" Then
            receipt.AppendLine("Reference #: " & reference)
        End If

        ' --- Footer ---
        receipt.AppendLine(line)
        receipt.AppendLine("VAT REG. TIN: 123-456-789-000")
        receipt.AppendLine("BIR Permit No.: 1234-5678-9012-3456")
        receipt.AppendLine("Date Issued: 2024-01-01")
        receipt.AppendLine("Valid Until: 2025-12-31")
        receipt.AppendLine(line)
        receipt.AppendLine("Thank you for your purchase!".PadLeft((maxChars + "Thank you for your purchase!".Length) \ 2))
        receipt.AppendLine()
        receipt.AppendLine()
        receipt.AppendLine()
        receipt.Append(Chr(&H1D) & "V" & Chr(0)) ' ESC/POS cut

        SendStringToPrinter("POS-58-Series", receipt.ToString())
    End Sub



    Private Sub SiticoneImageButton1_Click(sender As Object, e As EventArgs) Handles SiticoneImageButton1.Click
        Me.Close()
        Dashboard.posInstance.BringToFront()
    End Sub
End Class
