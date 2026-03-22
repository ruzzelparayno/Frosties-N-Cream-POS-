Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text

Public Class RefundConfirmation
    Private connectionString As String = "server=localhost;userid=root;password=;database=pos"

    ' --- Hash function for password ---
    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(password.Trim()))
            Dim sb As New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("x2")) ' lowercase hex
            Next
            Return sb.ToString()
        End Using
    End Function

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim enteredPassword As String = TextBox1.Text
        Dim hashedPassword As String = HashPassword(enteredPassword) ' hash the entered password
        Dim isPasswordCorrect As Boolean = False

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                ' Check if there is an admin with the hashed password
                Dim query As String = "SELECT COUNT(*) FROM users WHERE role='admin' AND password=@Password"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Password", hashedPassword)
                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If count > 0 Then
                        isPasswordCorrect = True
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Database error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        If isPasswordCorrect Then
            MessageBox.Show("Admin verified. Proceeding with refund.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show("Incorrect password. Refund cannot proceed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Me.Hide()
    End Sub
End Class
