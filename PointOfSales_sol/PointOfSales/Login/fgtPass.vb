Imports System.Security.Cryptography
Imports System.Text
Imports System.Timers
Imports System.Data.SQLite
Imports Org.BouncyCastle.Crypto
Imports SiticoneNetFrameworkUI

Public Class fgtPass

    Public Property UserEmail As String
    Private currentStep As Integer = 1

    ' ===============================
    ' 🔐 ATTEMPT CONTROL (CASE 1 & CASE 2)
    ' ===============================
    Private Const MAX_ATTEMPTS As Integer = 3
    Private Shared attemptCountCase1 As Integer = 0
    Private Shared lockUntilCase1 As DateTime = DateTime.MinValue

    Private Shared attemptCountCase2 As Integer = 0
    Private Shared lockUntilCase2 As DateTime = DateTime.MinValue

    ' 🔹 SHA256 Hash Function
    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(
            Encoding.UTF8.GetBytes(password.Trim())
        )
            Dim sb As New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString().ToLower() ' ✅ normalize HERE
        End Using
    End Function

    Private Sub ShowControl(uc As UserControl)
        uc.Dock = DockStyle.Fill
        Login.Panel8.Controls.Clear()
        Login.Panel8.Controls.Add(uc)
    End Sub

    Private Sub SiticoneLabel4_Click_1(sender As Object, e As EventArgs) Handles SiticoneLabel4.Click
        ShowControl(New LoginPass())
    End Sub

    Private Sub SiticoneButton1_Click(sender As Object, e As EventArgs) Handles SiticoneButton1.Click

        Select Case currentStep

            ' -------------------------------
            ' CASE 1 — CHECK EMAIL
            ' -------------------------------
            Case 1
                If DateTime.Now < lockUntilCase1 Then
                    Dim remaining As Integer = CInt((lockUntilCase1 - DateTime.Now).TotalSeconds)
                    MessageBox.Show("Too many failed attempts for email check." & vbCrLf &
                                    "Please wait " & remaining & " seconds.",
                                    "Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If SiticoneTextBox1.Text.Trim() = "" Then
                    MessageBox.Show("Please enter your email.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Try
                    Dim dbPath As String = Application.StartupPath & "\pos.db"
                    Using conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")
                        conn.Open()

                        Dim query As String = "SELECT Secret_Question FROM users WHERE email=@Email LIMIT 1"
                        Using cmd As New SQLiteCommand(query, conn)
                            cmd.Parameters.AddWithValue("@Email", SiticoneTextBox1.Text.Trim())
                            Dim result As Object = cmd.ExecuteScalar()

                            If result IsNot Nothing Then
                                attemptCountCase1 = 0
                                lockUntilCase1 = DateTime.MinValue

                                SiticoneLabel1.Text = result.ToString()
                                SiticoneLabel1.Visible = True
                                SiticoneTextBox2.Visible = True
                                SiticoneTextBox2.Text = ""

                                UserEmail = SiticoneTextBox1.Text.Trim()
                                currentStep = 2
                                SiticoneButton1.Text = "Next"

                                ' 🔹 Secret answer icons initial state
                                PictureBox5.Show()    ' show-password icon
                                PictureBox6.Hide()    ' hide-password icon
                                PictureBox5.BringToFront()
                                PictureBox6.BringToFront()
                            Else
                                attemptCountCase1 += 1
                                Dim remainingAttempts As Integer = MAX_ATTEMPTS - attemptCountCase1
                                If remainingAttempts <= 0 Then
                                    lockUntilCase1 = DateTime.Now.AddMinutes(2)
                                    MessageBox.Show("Too many failed attempts for email check." & vbCrLf &
                                                    "Locked for 2 minutes.",
                                                    "Locked", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Else
                                    MessageBox.Show("Email not found." & vbCrLf &
                                                    "Attempts left: " & remainingAttempts,
                                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End If
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Error checking email: " & ex.Message)
                End Try

            ' -------------------------------
            ' CASE 2 — CHECK SECRET ANSWER
            ' -------------------------------
            Case 2
                If DateTime.Now < lockUntilCase2 Then
                    Dim remaining As Integer = CInt((lockUntilCase2 - DateTime.Now).TotalSeconds)
                    MessageBox.Show("Too many failed attempts for secret answer." & vbCrLf &
                                    "Please wait " & remaining & " seconds.",
                                    "Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If SiticoneTextBox2.Text.Trim() = "" Then
                    MessageBox.Show("Please enter your secret answer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                Try
                    Dim dbPath As String = Application.StartupPath & "\pos.db"
                    Using conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")
                        conn.Open()

                        Dim query As String = "SELECT Secret_Answer FROM users WHERE email=@Email LIMIT 1"
                        Using cmd As New SQLiteCommand(query, conn)
                            cmd.Parameters.AddWithValue("@Email", UserEmail)
                            Dim storedHash As Object = cmd.ExecuteScalar()

                            If storedHash IsNot Nothing Then
                                Dim enteredHash As String = HashPassword(SiticoneTextBox2.Text.Trim())

                                If enteredHash = storedHash.ToString().Trim().ToLower() Then
                                    attemptCountCase2 = 0
                                    lockUntilCase2 = DateTime.MinValue

                                    MessageBox.Show("Your answer is correct!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                                    currentStep = 3
                                    SiticoneLabel1.Visible = False
                                    SiticoneTextBox2.Visible = False

                                    SiticoneLabel5.Visible = True
                                    SiticoneTextBox4.Visible = True
                                    SiticoneLabel3.Visible = True
                                    SiticoneTextBox3.Visible = True
                                    SiticoneButton1.Text = "Reset Password"

                                    ' 🔹 New password icons initial state
                                    SiticoneTextBox3.UseSystemPasswordChar = True
                                    SiticoneTextBox3.PasswordChar = "●"c
                                    SiticoneTextBox4.UseSystemPasswordChar = True
                                    SiticoneTextBox4.PasswordChar = "●"c

                                    PictureBox1.Show()      ' show-password icon
                                    PictureBox2.Hide()      ' hide-password icon
                                    PictureBox1.BringToFront()
                                    PictureBox2.BringToFront()

                                    PictureBox3.Show()      ' confirm password
                                    PictureBox4.Hide()
                                    PictureBox3.BringToFront()
                                    PictureBox4.BringToFront()

                                Else
                                    attemptCountCase2 += 1
                                    Dim remainingAttempts As Integer = MAX_ATTEMPTS - attemptCountCase2
                                    If remainingAttempts <= 0 Then
                                        lockUntilCase2 = DateTime.Now.AddMinutes(2)
                                        MessageBox.Show("Too many failed attempts for secret answer." & vbCrLf &
                                                        "Locked for 2 minutes.",
                                                        "Locked", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        MessageBox.Show("Incorrect secret answer." & vbCrLf &
                                                        "Attempts left: " & remainingAttempts,
                                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End If
                                End If
                            Else
                                MessageBox.Show("Account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Error checking answer: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

            ' -------------------------------
            ' CASE 3 — RESET PASSWORD
            ' -------------------------------
            Case 3
                If SiticoneTextBox4.Text.Trim() = "" Or SiticoneTextBox3.Text.Trim() = "" Then
                    MessageBox.Show("Please fill out all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If SiticoneTextBox4.Text.Trim() <> SiticoneTextBox3.Text.Trim() Then
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                Try
                    Dim dbPath As String = Application.StartupPath & "\pos.db"
                    Using conn As New SQLiteConnection("Data Source=" & dbPath & ";Version=3;")
                        conn.Open()

                        Dim username As String = ""
                        Dim getUserQuery As String = "SELECT username FROM users WHERE email=@Email LIMIT 1"
                        Using getCmd As New SQLiteCommand(getUserQuery, conn)
                            getCmd.Parameters.AddWithValue("@Email", UserEmail)
                            Dim result = getCmd.ExecuteScalar()
                            If result IsNot Nothing Then
                                username = result.ToString().Trim()
                            Else
                                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit Sub
                            End If
                        End Using

                        Dim newPassword As String = SiticoneTextBox4.Text.Trim()
                        Dim hashedPassword As String = HashPassword(newPassword)

                        Dim updateQuery As String = "UPDATE users SET password=@Password WHERE username=@Username"
                        Using cmd As New SQLiteCommand(updateQuery, conn)
                            cmd.Parameters.AddWithValue("@Password", hashedPassword)
                            cmd.Parameters.AddWithValue("@Username", username.Trim())

                            Dim rowsAffected = cmd.ExecuteNonQuery()
                            If rowsAffected > 0 Then
                                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                attemptCountCase2 = 0
                                lockUntilCase2 = DateTime.MinValue

                                For Each ctrl As Control In Login.Panel8.Controls
                                    Dim loginControl As LoginPass = TryCast(ctrl, LoginPass)
                                    If loginControl IsNot Nothing Then
                                        loginControl.ResetLoginAttempts()
                                    End If
                                Next

                                ShowControl(New LoginPass())
                            Else
                                MessageBox.Show("Failed to update password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End Using
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Failed to update password: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try

        End Select
    End Sub

    ' -------------------------------
    ' UI SETUP
    ' -------------------------------
    Private Sub fgtPass_Load(sender As Object, e As EventArgs) Handles Me.Load
        SiticoneLabel2.Visible = True
        SiticoneTextBox1.Visible = True

        SiticoneLabel1.Visible = False
        SiticoneTextBox2.Visible = False

        SiticoneLabel5.Visible = False
        SiticoneTextBox4.Visible = False
        SiticoneLabel3.Visible = False
        SiticoneTextBox3.Visible = False

        SiticoneButton1.Text = "Next"
    End Sub

    Private Sub fp2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SiticoneTextBox2.PasswordChar = "●"c
        SiticoneTextBox2.UseSystemPasswordChar = True
        SiticoneTextBox3.PasswordChar = "●"c
        SiticoneTextBox3.UseSystemPasswordChar = True
        SiticoneTextBox4.PasswordChar = "●"c
        SiticoneTextBox4.UseSystemPasswordChar = True

        ' Initially hide all icons
        PictureBox1.Hide()
        PictureBox2.Hide()
        PictureBox3.Hide()
        PictureBox4.Hide()
        PictureBox5.Hide()
        PictureBox6.Hide()
    End Sub

    ' -------------------------------
    ' TOGGLE ICONS
    ' -------------------------------
    ' New Password
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        SiticoneTextBox3.UseSystemPasswordChar = False
        PictureBox1.Hide()
        PictureBox2.Show()
        PictureBox2.BringToFront()
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        SiticoneTextBox3.UseSystemPasswordChar = True
        SiticoneTextBox3.PasswordChar = "●"c
        PictureBox2.Hide()
        PictureBox1.Show()
        PictureBox1.BringToFront()
    End Sub

    ' Confirm Password
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        SiticoneTextBox4.UseSystemPasswordChar = False
        PictureBox3.Hide()
        PictureBox4.Show()
        PictureBox4.BringToFront()
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        SiticoneTextBox4.UseSystemPasswordChar = True
        SiticoneTextBox4.PasswordChar = "●"c
        PictureBox4.Hide()
        PictureBox3.Show()
        PictureBox3.BringToFront()
    End Sub

    ' Secret Answer
    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        SiticoneTextBox2.UseSystemPasswordChar = False
        PictureBox5.Hide()
        PictureBox6.Show()
        PictureBox6.BringToFront()
    End Sub

    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        SiticoneTextBox2.UseSystemPasswordChar = True
        SiticoneTextBox2.PasswordChar = "●"c
        PictureBox6.Hide()
        PictureBox5.Show()
        PictureBox5.BringToFront()
    End Sub

    Private Sub SiticoneTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles SiticoneTextBox4.KeyDown, SiticoneTextBox3.KeyDown, SiticoneTextBox2.KeyDown, SiticoneTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            SiticoneButton1.PerformClick()
        End If
    End Sub

End Class
