Imports System.Security.Cryptography
Imports System.Text
Imports SiticoneNetFrameworkUI
Imports System.Data.SQLite

Public Class LoginPass

    Private dbCommand As String = ""
    Private bindingSrc As New BindingSource

    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private consString As String = "Data Source=" & dbPath & ";Version=3;"

    Private connection As New SQLiteConnection(consString)
    Private command As New SQLiteCommand("", connection)

    Public Shared currentUsername As String = ""
    Public Shared currentRole As String = ""

    ' ===============================
    ' 🔐 LOGIN ATTEMPT CONTROL
    ' ===============================
    Private loginAttempts As Integer = 0
    Private Const MAX_ATTEMPTS As Integer = 3
    Private lockUntil As DateTime = DateTime.MinValue

    ' ===============================
    ' 🔐 UNIFIED SHA-256 HASH FUNCTION (FIXED)
    ' ===============================
    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(
                Encoding.UTF8.GetBytes(password.Trim())
            )
            Dim sb As New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString().ToLower() ' ✅ normalize here
        End Using
    End Function

    ' ✅ Reset login attempts (used by forgot password)
    Public Sub ResetLoginAttempts()
        loginAttempts = 0
        lockUntil = DateTime.MinValue
    End Sub

    Private Sub ShowControl(uc As UserControl)
        uc.Dock = DockStyle.Fill
        Login.Panel8.Controls.Clear()
        Login.Panel8.Controls.Add(uc)
    End Sub

    Private Sub LoginPass_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SiticoneTextBox2.UseSystemPasswordChar = True
        SiticoneTextBox2.PasswordChar = "•"c
        PictureBox1.Hide()
        PictureBox2.Show()
    End Sub

    Private Sub SiticoneLabel4_Click(sender As Object, e As EventArgs) Handles SiticoneLabel4.Click
        ShowControl(New fgtPass())
    End Sub

    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click

        ' ===============================
        ' 🔒 CHECK IF LOCKED
        ' ===============================
        If DateTime.Now < lockUntil Then
            Dim remaining As Integer = CInt((lockUntil - DateTime.Now).TotalSeconds)
            MessageBox.Show(
                "Too many failed attempts." & vbCrLf &
                "Please wait " & remaining & " seconds.",
                "Login Locked",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            Exit Sub
        End If

        Dim username As String = SiticoneTextBox1.Text.Trim()
        Dim password As String = SiticoneTextBox2.Text.Trim()

        If username = "" Or password = "" Then
            MessageBox.Show(
                "Please Enter Both Username And Password!!",
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            )
            Exit Sub
        End If

        Dim hashedPassword As String = HashPassword(password)

        Try
            connection.Open()

            Dim query As String =
                "SELECT username, password, role FROM users WHERE username=@username LIMIT 1"

            Dim cmd As New SQLiteCommand(query, connection)
            cmd.Parameters.AddWithValue("@username", username)

            Dim reader As SQLiteDataReader =
                cmd.ExecuteReader(CommandBehavior.SingleRow)

            If reader.Read() Then

                Dim dbPassword As String =
                    Convert.ToString(reader("password")).Trim().ToLower()
                Dim dbRole As String =
                    reader("role").ToString().Trim().ToLower()

                ' ===============================
                ' ✅ FIXED PASSWORD COMPARISON
                ' ===============================
                If dbPassword = hashedPassword Then

                    ' ✅ RESET ATTEMPTS
                    loginAttempts = 0
                    lockUntil = DateTime.MinValue

                    currentUsername = username
                    currentRole = dbRole

                    MessageBox.Show(
                        "Login successful!" & vbCrLf & "Role: " & dbRole,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    )

                    If dbRole = "admin" Then
                        Dashboard.Show()
                        Login.Hide()
                    Else
                        CashierDashboard.Show()
                        Login.Hide()
                    End If

                    SiticoneTextBox1.Clear()
                    SiticoneTextBox2.Clear()

                Else
                    ' ❌ WRONG PASSWORD
                    loginAttempts += 1
                    Dim remainingAttempts As Integer =
                        MAX_ATTEMPTS - loginAttempts

                    If remainingAttempts <= 0 Then
                        lockUntil = DateTime.Now.AddMinutes(2)
                        MessageBox.Show(
                            "Too many failed attempts." & vbCrLf &
                            "Login is locked for 2 minutes.",
                            "Account Locked",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        )
                    Else
                        MessageBox.Show(
                            "Incorrect password." & vbCrLf &
                            "Attempts left: " & remainingAttempts,
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        )
                    End If

                    SiticoneTextBox2.Clear()
                End If

            Else
                ' ❌ USERNAME NOT FOUND
                loginAttempts += 1
                Dim remainingAttempts As Integer =
                    MAX_ATTEMPTS - loginAttempts

                If remainingAttempts <= 0 Then
                    lockUntil = DateTime.Now.AddMinutes(2)
                    MessageBox.Show(
                        "Too many failed attempts." & vbCrLf &
                        "Login is locked for 2 minutes.",
                        "Account Locked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    )
                Else
                    MessageBox.Show(
                        "Username not found." & vbCrLf &
                        "Attempts left: " & remainingAttempts,
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    )
                End If

                SiticoneTextBox1.Clear()
                SiticoneTextBox2.Clear()
            End If

            reader.Close()

        Catch ex As Exception
            MessageBox.Show("ERROR: " & ex.Message)
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try

    End Sub

    ' ===============================
    ' SHOW / HIDE PASSWORD
    ' ===============================
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        SiticoneTextBox2.UseSystemPasswordChar = True
        SiticoneTextBox2.PasswordChar = "•"c
        PictureBox1.Hide()
        PictureBox2.Show()
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        SiticoneTextBox2.UseSystemPasswordChar = False
        PictureBox2.Hide()
        PictureBox1.Show()
    End Sub

    Private Sub SiticoneTextBox_KeyDown(sender As Object, e As KeyEventArgs) _
        Handles SiticoneTextBox1.KeyDown, SiticoneTextBox2.KeyDown

        If e.KeyCode = Keys.Enter Then
            SiticoneButton1.PerformClick()
        End If
    End Sub

End Class
