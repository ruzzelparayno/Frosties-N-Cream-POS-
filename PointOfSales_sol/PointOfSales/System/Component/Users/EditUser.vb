Imports System.Data.SQLite
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks

Public Class EditUser
    Public CurrentUserID As Integer = -1
    Private dbName As String = "pos.db"
    Private dbPath As String = Application.StartupPath & "\" & dbName
    Private connStr As String = "Data Source=" & dbPath & ";Version=3;"

    Private Sub EditUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtAdminPass.UseSystemPasswordChar = True
        txtConfirmPass.UseSystemPasswordChar = True
        txtPassword.UseSystemPasswordChar = True
        txtAdminPass.PasswordChar = "•"c
        txtConfirmPass.PasswordChar = "•"c
        txtPassword.PasswordChar = "•"c
        satextbox.UseSystemPasswordChar = True
        satextbox.PasswordChar = "•"c
        PictureBox2.Show()
        PictureBox3.Show()
        PictureBox5.Show()
        PictureBox7.Show()
        PictureBox1.Hide()
        PictureBox4.Hide()
        PictureBox6.Hide()
        PictureBox8.Hide()
        cbRole.DropDownStyle = ComboBoxStyle.DropDownList
        cb_sq.DropDownStyle = ComboBoxStyle.DropDownList
    End Sub


    Private Function HashText(input As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(input)
            Dim hash = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    Private Async Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click

        If CurrentUserID = -1 Then
            MessageBox.Show("No user selected.")
            Exit Sub
        End If

        ' VALIDATION
        If txtPassword.Text = "" Or txtConfirmPass.Text = "" Or txtAdminPass.Text = "" Then
            MessageBox.Show("Please fill all password fields.")
            Exit Sub
        End If

        If txtPassword.Text <> txtConfirmPass.Text Then
            MessageBox.Show("Passwords do not match.")
            Exit Sub
        End If

        If cb_sq.Text = "" Or satextbox.Text = "" Then
            MessageBox.Show("Please fill the security question and answer.")
            Exit Sub
        End If

        ' VERIFY ADMIN PASSWORD
        Dim adminCorrect As Boolean = False

        Using conn As New SQLiteConnection(connStr)
            conn.Open()

            Dim checkAdmin = "SELECT COUNT(*) FROM users WHERE username='admin' AND password=@pass"
            Using cmd As New SQLiteCommand(checkAdmin, conn)
                cmd.Parameters.AddWithValue("@pass", HashText(txtAdminPass.Text))
                adminCorrect = (Convert.ToInt32(cmd.ExecuteScalar()) = 1)
            End Using
        End Using

        If Not adminCorrect Then
            MessageBox.Show("Admin password is incorrect.")
            Exit Sub
        End If

        ' UPDATE USER DATA
        Try
            Using conn As New SQLiteConnection(connStr)
                conn.Open()

                ' 🔥 Updated to use Secret_Question and Secret_Answer
                Dim query As String =
                    "UPDATE users SET username=@u, email=@e, role=@r, password=@p, " &
                    "Secret_Question=@sq, Secret_Answer=@sa WHERE userid=@id"

                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@u", txtUsername.Text)
                    cmd.Parameters.AddWithValue("@e", txtEmail.Text)
                    cmd.Parameters.AddWithValue("@r", cbRole.Text)
                    cmd.Parameters.AddWithValue("@p", HashText(txtPassword.Text))
                    cmd.Parameters.AddWithValue("@sq", cb_sq.Text)
                    cmd.Parameters.AddWithValue("@sa", HashText(satextbox.Text))  ' 🔥 hashed answer
                    cmd.Parameters.AddWithValue("@id", CurrentUserID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' REFRESH USER LIST + OVERLAY
            UserContent.Instance.LoadUsers()

            Me.Close()

            MessageBox.Show(
                "User updated successfully!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )

            UserContent.Instance.SiticoneOverlay1.Show = True
            Await Task.Delay(1500)
            UserContent.Instance.SiticoneOverlay1.Show = False

        Catch ex As Exception
            MessageBox.Show("Error updating user: " & ex.Message)
        End Try

    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        txtPassword.UseSystemPasswordChar = True
        txtPassword.PasswordChar = "•"c

        PictureBox1.Hide()
        PictureBox2.Show()
    End Sub

    ' Click PictureBox2 → show PictureBox1, show password
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        txtPassword.UseSystemPasswordChar = False

        PictureBox2.Hide()
        PictureBox1.Show()
    End Sub

    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        txtConfirmPass.UseSystemPasswordChar = True
        txtConfirmPass.PasswordChar = "•"c

        PictureBox3.Hide()
        PictureBox4.Show()
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        txtConfirmPass.UseSystemPasswordChar = False

        PictureBox3.Show()
        PictureBox4.Hide()
    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click
        satextbox.UseSystemPasswordChar = True
        satextbox.PasswordChar = "•"c

        PictureBox5.Hide()
        PictureBox6.Show()
    End Sub

    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click
        satextbox.UseSystemPasswordChar = False

        PictureBox5.Show()
        PictureBox6.Hide()
    End Sub
    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        txtAdminPass.UseSystemPasswordChar = True
        txtAdminPass.PasswordChar = "•"c

        PictureBox7.Show()
        PictureBox8.Hide()
    End Sub

    Private Sub PictureBox8_Click(sender As Object, e As EventArgs) Handles PictureBox8.Click
        txtAdminPass.UseSystemPasswordChar = False

        PictureBox8.Show()
        PictureBox7.Hide()
    End Sub

End Class
