Imports System.Data
Imports System.Data.SQLite
Imports System.Security.Cryptography
Imports System.Text
Imports iText.IO.Font.Constants
Imports iText.Kernel.Colors
Imports iText.Kernel.Font
Imports iText.Kernel.Pdf
Imports iText.Layout
Imports iText.Layout.Borders
Imports iText.Layout.Element
Imports iText.Layout.Properties
Imports SiticoneNetFrameworkUI

Public Class AccountContent
    ' 🔹 SQLite connection string
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")

    Dim boldFont As PdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)

    ' 🔹 Function to hash a string with SHA256
    Private Function HashText(input As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)
            Dim hash As Byte() = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function
    Private Sub SiticoneButton2_Click(sender As Object, e As EventArgs) Handles SiticoneButton2.Click
        Try
            Dim exportFile As String = ""

            Using sfd As New SaveFileDialog()
                sfd.Filter = "SQLite Database (*.db)|*.db"
                sfd.Title = "Export Database"
                sfd.FileName = "pos_backup.db"
                If sfd.ShowDialog() = DialogResult.OK Then
                    exportFile = sfd.FileName
                Else
                    Exit Sub
                End If
            End Using

            ' Copy the database file to the selected location
            Dim dbPath As String = Application.StartupPath & "\pos.db"
            System.IO.File.Copy(dbPath, exportFile, True)

            MessageBox.Show("Database exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Export Error: " & ex.Message)
        End Try
    End Sub
    Private Sub SiticoneButton4_Click(sender As Object, e As EventArgs) Handles SiticoneButton4.Click
        Try
            Dim importFile As String = ""

            ' ---------------------------
            ' Open file dialog to select the .db file
            ' ---------------------------
            Using ofd As New OpenFileDialog()
                ofd.Filter = "SQLite Database (*.db)|*.db"
                ofd.Title = "Import Database"
                If ofd.ShowDialog() = DialogResult.OK Then
                    importFile = ofd.FileName
                Else
                    Exit Sub
                End If
            End Using

            Dim dbPath As String = Application.StartupPath & "\pos.db"

            ' ---------------------------
            ' Delete current DB before copying
            ' ---------------------------
            If System.IO.File.Exists(dbPath) Then
                System.IO.File.Delete(dbPath)
            End If

            ' ---------------------------
            ' Copy selected DB into application folder
            ' ---------------------------
            System.IO.File.Copy(importFile, dbPath, True)

            MessageBox.Show("Database imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' ---------------------------
            ' Refresh all visible modules
            ' ---------------------------
            ' ProductContent
            If ProductContent.Instance IsNot Nothing Then
                ProductContent.Instance.LoadInventory()
            End If

            ' UserContent
            If UserContent.Instance IsNot Nothing Then
                UserContent.Instance.LoadUsers()
            End If

            ' --- REFRESH TRANSACTIONS ---
            If TransactionContent.Instance IsNot Nothing Then
                TransactionContent.NotifyTransactionAdded()  ' <- THIS IS THE KEY
            End If

            ' PosControl
            If PosControl.Instance IsNot Nothing Then
                PosControl.Instance.RefreshAllData()

                ' Reload ticket from DB so lbl_tickets shows correct number
                PosControl.Instance.LoadLastTicketFromFile()  ' <-- reload last ticket
                PosControl.Instance.UpdateTicket()            ' <-- update the label
            End If


            ' Optional: refresh other modules if needed (TransactionContent, etc.)

        Catch ex As Exception
            MessageBox.Show("Import Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub AccountContent_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set password chars
        txt_cpass.PasswordChar = "•"c
        txt_pass.PasswordChar = "•"c
        txt_pass2.PasswordChar = "•"c
        txt_op.PasswordChar = "•"c
        txt_np.PasswordChar = "•"c

        ' Lock username/email fields
        txt_uname.Enabled = False
        txt_mail.Enabled = False

        ' Hide all password change controls initially
        HidePasswordChangeControls()

        ' Load current user info
        LoadLoggedInUserInfo()
    End Sub

    ' 🔹 Load info for currently logged-in user
    Public Sub LoadLoggedInUserInfo()
        Try
            If LoginPass.currentUsername = "" Then Exit Sub

            conn.Open()
            Dim query As String = "SELECT username, email FROM users WHERE username=@uname LIMIT 1"
            Dim cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@uname", LoginPass.currentUsername)

            Dim reader As SQLiteDataReader = cmd.ExecuteReader(CommandBehavior.SingleRow)
            If reader.Read() Then
                txt_uname.Text = reader("username").ToString()
                txt_mail.Text = reader("email").ToString()
                txt_pass2.Text = ""
            End If
            reader.Close()
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Error loading account info: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    ' 🔹 Verify current password
    Private Sub SiticoneButton3_Click(sender As Object, e As EventArgs) Handles SiticoneButton3.Click
        Try
            If txt_pass.Text.Trim = "" Then
                MessageBox.Show("Please enter your current password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            conn.Open()
            Dim query = "SELECT password FROM users WHERE username=@uname AND email=@mail LIMIT 1"
            Dim cmd As New SQLiteCommand(query, conn)
            cmd.Parameters.AddWithValue("@uname", txt_uname.Text.Trim())
            cmd.Parameters.AddWithValue("@mail", txt_mail.Text.Trim())

            Dim reader = cmd.ExecuteReader()
            If reader.Read() Then
                Dim dbPassword = reader("password").ToString()
                Dim hashedInputPass = HashText(txt_pass.Text.Trim())

                If hashedInputPass = dbPassword Then
                    ' ✅ Verification successful
                    MessageBox.Show("Account verified! You can now change your password.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Disable old password fields
                    txt_pass.Enabled = False
                    txt_op.Enabled = False
                    txt_op.Text = txt_pass.Text ' pre-fill old password

                    ' Show all password change controls including SiticoneButton1 inside Panel15
                    ShowPasswordChangeControls()
                Else
                    MessageBox.Show("Current password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("Account not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            reader.Close()
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Database error: " & ex.Message)
        End Try
    End Sub

    ' 🔹 Change password
    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click
        Try
            If txt_np.Text.Trim = "" Or txt_cpass.Text.Trim = "" Then
                MessageBox.Show("Please enter a new password and confirm it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If txt_np.Text.Trim <> txt_cpass.Text.Trim Then
                MessageBox.Show("New password and confirm password do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            conn.Open()
            Dim updateQuery = "UPDATE users SET password=@newPass WHERE username=@uname"
            Dim updateCmd As New SQLiteCommand(updateQuery, conn)
            updateCmd.Parameters.AddWithValue("@newPass", HashText(txt_np.Text.Trim))
            updateCmd.Parameters.AddWithValue("@uname", txt_uname.Text.Trim())
            Dim rowsAffected = updateCmd.ExecuteNonQuery()

            If rowsAffected > 0 Then
                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                HidePasswordChangeControls()
                ClearPasswordFields()
            Else
                MessageBox.Show("Failed to update password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            txt_pass.Enabled = True
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("Database error: " & ex.Message)
        End Try
    End Sub

    ' 🔹 Show all password change controls including SiticoneButton1
    Private Sub ShowPasswordChangeControls()
        Panel9.Show()
        Panel16.Show()
        Panel18.Show()
        Panel15.Show() ' Panel containing SiticoneButton1

        lbl_op.Show()
        txt_op.Show()
        lbl_np.Show()
        lbl_cp.Show()
        txt_cpass.Show()
        txt_np.Show()

        SiticoneButton1.Show()
        SiticoneButton1.BringToFront()
    End Sub

    ' 🔹 Hide all password change controls and button
    Private Sub HidePasswordChangeControls()
        Panel9.Hide()
        Panel16.Hide()
        Panel18.Hide()
        Panel15.Hide()
        lbl_op.Hide()
        txt_op.Hide()
        lbl_np.Hide()
        lbl_cp.Hide()
        txt_cpass.Hide()
        txt_np.Hide()
        SiticoneButton1.Hide()
    End Sub

    ' 🔹 Clear password fields
    Private Sub ClearPasswordFields()
        txt_pass.Clear()
        txt_op.Clear()
        txt_np.Clear()
        txt_cpass.Clear()
    End Sub
End Class
