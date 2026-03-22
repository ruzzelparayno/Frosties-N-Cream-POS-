Imports System.Data.SQLite
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.UI.WebControls

Public Class RefundConfirmation
    ' SQLite database path
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private connectionString As String = "Data Source=" & dbPath & ";Version=3;"

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

    Private Sub RefundConfimartion_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox3.Hide()
        PictureBox2.Show()

    End Sub
    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim enteredPassword As String = TextBox1.Text.Trim()
        Dim hashedPassword As String = HashPassword(enteredPassword)
        Dim isPasswordCorrect As Boolean = False

        Try
            Using conn As New SQLiteConnection(connectionString)
                conn.Open()

                ' ✔ role comparison is now CASE-INSENSITIVE
                Dim query As String =
                    "SELECT COUNT(*) 
                     FROM users 
                     WHERE role = 'admin' COLLATE NOCASE 
                     AND Password = @Password"

                Using cmd As New SQLiteCommand(query, conn)
                    ' ✔ Explicit parameter type (important for hashes)
                    cmd.Parameters.Add("@Password", DbType.String).Value = hashedPassword

                    Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If count > 0 Then
                        isPasswordCorrect = True
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Database error: " & ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        If isPasswordCorrect Then
            MessageBox.Show("Admin verified. Proceeding with refund.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show("Incorrect password. Refund cannot proceed.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Me.Hide()
    End Sub


    ' Hide password
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        TextBox1.UseSystemPasswordChar = True
        TextBox1.PasswordChar = "•"c

        PictureBox3.Hide()
        PictureBox2.Show()
    End Sub

    ' Show password
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        TextBox1.UseSystemPasswordChar = False
        TextBox1.PasswordChar = ControlChars.NullChar ' Remove password char to show text

        PictureBox2.Hide()
        PictureBox3.Show()
    End Sub

End Class
