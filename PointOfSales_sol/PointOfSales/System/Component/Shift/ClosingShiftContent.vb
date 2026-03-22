Imports System.Text
Imports System.Runtime.InteropServices
Imports MySql.Data.MySqlClient

Public Class ClosingShiftContent

    ' Reference to the existing ShiftContent form
    Public Property MainForm As ShiftContent

    ' --- ESC/POS Raw Printing Functions ---
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



    ' --- Close Shift Button ---
    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click
        Dim actualCash As String = SiticoneTextBox1.Text.Trim()

        If String.IsNullOrEmpty(actualCash) Then
            MessageBox.Show("Please insert the actual cash amount.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            SiticoneTextBox1.Focus()
            Return
        End If

        ' Success message
        MessageBox.Show("Thank you! Cash amount recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        ' Hide PictureBox1 and btnCategory in ShiftContent
        If MainForm IsNot Nothing Then
            MainForm.PictureBox1.Visible = False
            MainForm.btnCategory.Visible = False
            MainForm.SiticoneButton3.Visible = True ' Show SiticoneButton3 after shift close
        End If

        ' Print Closing Shift Receipt
        PrintClosingShiftReceipt()

        ' Hide this form
        Me.Hide()
        If MainForm IsNot Nothing AndAlso MainForm.cashmanagementcontroll IsNot Nothing Then
            MainForm.cashmanagementcontroll.Reseeet()
        End If

        ' Show CashManagementControll again
        If MainForm IsNot Nothing Then
            MainForm.cashmanagementcontroll.ResetShift()
            MainForm.ShowControl(MainForm.cashmanagementcontroll)
            MainForm.Panel4.Show()
        End If
    End Sub

    ' --- Helper function to clean label text for printer ---
    ' --- Helper function to format number for printer ---
    Private Function FormatForPrinter(value As String) As String
        Dim amount As Decimal = 0
        ' Try to parse the value, remove Peso sign and commas
        Decimal.TryParse(value.Replace("₱", "").Replace(",", "").Trim(), amount)
        ' Return formatted number with comma and 2 decimals
        Return amount.ToString("N2")
    End Function



    ' --- Helper function to send string to printer using CP850 (supports ñ, á, é, ü) ---
    Private Shared Function SendStringToPrinter(ByVal szPrinterName As String, ByVal szString As String) As Boolean
        Dim pBytes As IntPtr
        ' Use CP850 encoding for extended ASCII characters
        Dim bytes() As Byte = Encoding.GetEncoding(850).GetBytes(szString)

        pBytes = Marshal.AllocCoTaskMem(bytes.Length)
        Marshal.Copy(bytes, 0, pBytes, bytes.Length)

        Dim hPrinter As IntPtr
        Dim di As DOCINFOA = New DOCINFOA()
        di.pDocName = "Closing Shift Receipt"
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

    ' --- Function to print closing shift receipt ---
    Private Sub PrintClosingShiftReceipt()
        If MainForm Is Nothing OrElse MainForm.cashmanagementcontroll Is Nothing Then
            MessageBox.Show("Cash Management not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim cmc = MainForm.cashmanagementcontroll
        Dim line As String = New String("=", 32)
        Dim receipt As New StringBuilder()
        Dim maxChars As Integer = 32 ' for 58mm printer

        ' --- Header ---
        receipt.AppendLine("Frosties N' Cream".PadLeft((maxChars + "Frosties N' Cream".Length) \ 2))
        receipt.AppendLine("10 Rosal St. Doña Manuela Subd.".PadLeft((maxChars + "10 Rosal St. Doña Manuela Subd.".Length) \ 2))
        receipt.AppendLine("Pamplona 3, 1740 Las Piñas".PadLeft((maxChars + "Pamplona 3, 1740 Las Piñas".Length) \ 2))
        receipt.AppendLine("Contact: 0917 557 5485".PadLeft((maxChars + "Contact: 0917 557 5485".Length) \ 2))
        receipt.AppendLine(line)
        receipt.AppendLine("Shift Report".PadLeft((maxChars + "Shift Report".Length) \ 2))
        receipt.AppendLine(line)

        ' --- Shift Opened / Closed ---
        Dim shiftOpened As String = If(cmc.lbl_datet?.Text, "")
        receipt.AppendLine("Shift Opened:".PadRight(20) & shiftOpened.PadLeft(12))
        receipt.AppendLine("Close Shift:".PadRight(20) & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadLeft(12))
        receipt.AppendLine(line)

        ' --- Cash Drawer ---
        receipt.AppendLine(line)
        receipt.AppendLine("CASH DRAWER".PadLeft((maxChars + "CASH DRAWER".Length) \ 2))
        receipt.AppendLine(line)
        receipt.AppendLine()
        receipt.AppendLine("Starting Cash:".PadRight(20) & FormatForPrinter(cmc.lbl_sc1.Text).PadLeft(12))
        receipt.AppendLine("Cash Payments:".PadRight(20) & FormatForPrinter(cmc.lbl_cashp1.Text).PadLeft(12))
        receipt.AppendLine("Cash Refunds:".PadRight(20) & FormatForPrinter(cmc.lbl_refund1.Text).PadLeft(12))
        receipt.AppendLine("Expected Cash:".PadRight(20) & FormatForPrinter(cmc.lbl_eca.Text).PadLeft(12))

        ' --- Actual Cash & Difference ---
        Dim actualCashDecimal As Decimal = 0D
        Decimal.TryParse(SiticoneTextBox1.Text.Replace("₱", "").Replace(",", "").Trim(), actualCashDecimal)

        Dim expectedCashDecimal As Decimal = 0D
        Decimal.TryParse(cmc.lbl_eca.Text.Replace("₱", "").Replace(",", "").Trim(), expectedCashDecimal)

        Dim difference As Decimal = expectedCashDecimal - actualCashDecimal
        receipt.AppendLine("Actual Cash:".PadRight(20) & actualCashDecimal.ToString("N2").PadLeft(12))
        receipt.AppendLine("Difference:".PadRight(20) & difference.ToString("N2").PadLeft(12))

        ' --- Sales Summary ---
        receipt.AppendLine(line)
        receipt.AppendLine("SALES SUMMARY".PadLeft((maxChars + "SALES SUMMARY".Length) \ 2))
        receipt.AppendLine(line)
        receipt.AppendLine()
        receipt.AppendLine("Gross Sales:".PadRight(20) & FormatForPrinter(cmc.lbl_gs.Text).PadLeft(12))
        receipt.AppendLine("Refunds:".PadRight(20) & FormatForPrinter(cmc.lbl_srefunds.Text).PadLeft(12))
        receipt.AppendLine("Discounts:".PadRight(20) & FormatForPrinter(cmc.lbl_sdiscounts.Text).PadLeft(12))
        receipt.AppendLine("Net Sales:".PadRight(20) & FormatForPrinter(cmc.lbl_ns.Text).PadLeft(12))
        receipt.AppendLine("Cash Sales:".PadRight(20) & FormatForPrinter(cmc.lbl_ncash.Text).PadLeft(12))
        receipt.AppendLine("GCash Sales:".PadRight(20) & FormatForPrinter(cmc.lbl_ngcash.Text).PadLeft(12))
        receipt.AppendLine(line)

        ' --- Footer ---
        receipt.AppendLine("Thank you for your service!".PadLeft((maxChars + "Thank you for your service!".Length) \ 2))
        receipt.AppendLine()
        receipt.AppendLine()
        receipt.AppendLine(Chr(&H1D) & "V" & Chr(0)) ' ESC/POS cut

        ' --- Send to printer ---
        SendStringToPrinter("POS-58-Series", receipt.ToString())
    End Sub



End Class
